using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using NAPS2.Scan;

namespace Scandalous.Core.Services;

/// <summary>
/// Discovers eSCL scanners on macOS using the system's dns-sd tool.
/// NAPS2's built-in ESCL discovery uses raw mDNS sockets which conflict
/// with macOS's mDNSResponder (port 5353). This service uses dns-sd,
/// which delegates to mDNSResponder, to find scanners and then constructs
/// ScanDevice instances with direct HTTP(S) URLs so the NAPS2 ESCL driver
/// connects without needing its own mDNS queries.
/// </summary>
internal static partial class MacEsclDiscoveryService
{
    /// <summary>How long to keep waiting after the last new browse result before stopping.</summary>
    private static readonly TimeSpan BrowseSettleTime = TimeSpan.FromMilliseconds(500);

    internal static async Task<List<ScanDevice>> DiscoverAsync(
        TimeSpan browseTimeout, CancellationToken ct = default)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return [];

        // Browse for _uscan._tcp (HTTP) and _uscans._tcp (HTTPS) in parallel.
        var httpTask = BrowseServicesAsync("_uscan._tcp", browseTimeout, ct);
        var httpsTask = BrowseServicesAsync("_uscans._tcp", browseTimeout, ct);

        await Task.WhenAll(httpTask, httpsTask);

        var browseResults = new List<(string Name, string ServiceType, bool Tls)>();
        foreach (var name in await httpTask)
            browseResults.Add((name, "_uscan._tcp", false));
        foreach (var name in await httpsTask)
            browseResults.Add((name, "_uscans._tcp", true));

        // Deduplicate by name, preferring the TLS variant.
        var unique = browseResults
            .GroupBy(r => r.Name)
            .Select(g => g.OrderByDescending(r => r.Tls).First())
            .ToList();

        // Resolve all discovered services in parallel.
        var resolveTasks = unique.Select(async entry =>
        {
            try
            {
                return await ResolveServiceAsync(entry.Name, entry.ServiceType, entry.Tls, ct);
            }
            catch
            {
                return null;
            }
        });

        var results = await Task.WhenAll(resolveTasks);
        return results.Where(d => d != null).Cast<ScanDevice>().ToList();
    }

    private static async Task<List<string>> BrowseServicesAsync(
        string serviceType, TimeSpan maxTimeout, CancellationToken ct)
    {
        var names = new HashSet<string>();
        var settleCts = new CancellationTokenSource();

        using var process = StartDnsSd(["-B", serviceType, "local."]);
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data == null) return;
            var match = BrowseLineRegex().Match(e.Data);
            if (match.Success)
            {
                var name = match.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(name) && names.Add(name))
                {
                    // Reset the settle timer each time a new service is found.
                    settleCts.CancelAfter(BrowseSettleTime);
                }
            }
        };
        process.BeginOutputReadLine();

        // Wait for: (a) the settle timer after finding results,
        //           (b) the hard max timeout, or (c) caller cancellation.
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            ct, settleCts.Token);
        linkedCts.CancelAfter(maxTimeout);

        try { await Task.Delay(Timeout.Infinite, linkedCts.Token); }
        catch (OperationCanceledException) { }

        KillProcess(process);
        return [.. names];
    }

    private static async Task<ScanDevice?> ResolveServiceAsync(
        string instanceName, string serviceType, bool tls, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<string>();

        using var process = StartDnsSd(["-L", instanceName, serviceType, "local."]);
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null && e.Data.Contains("can be reached at"))
                tcs.TrySetResult(e.Data);
        };
        process.BeginOutputReadLine();

        // Wait for the result line or a 2-second hard timeout.
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(2));
        using var reg = timeoutCts.Token.Register(() => tcs.TrySetCanceled());

        string reachLine;
        try { reachLine = await tcs.Task; }
        catch (OperationCanceledException) { KillProcess(process); return null; }

        // We already got the line we need — kill immediately.
        KillProcess(process);

        var reachMatch = ReachableRegex().Match(reachLine);
        if (!reachMatch.Success)
            return null;

        var hostname = reachMatch.Groups[1].Value.TrimEnd('.');
        var port = int.Parse(reachMatch.Groups[2].Value);

        // We still need the TXT record for the resource path; the -L output
        // may have already printed it alongside the reach line. Do a quick
        // second lookup just for TXT if we didn't get it from the reach line.
        var rsMatch = ResourcePathRegex().Match(reachLine);
        var resourcePath = rsMatch.Success ? rsMatch.Groups[1].Value : "/eSCL";

        IPAddress? ip;
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostname, ct);
            ip = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                 ?? addresses.FirstOrDefault();
        }
        catch
        {
            return null;
        }

        if (ip == null)
            return null;

        var scheme = tls || port == 443 ? "https" : "http";
        var url = $"{scheme}://{ip}:{port}{resourcePath}";

        return new ScanDevice(Driver.Escl, url, instanceName);
    }

    private static Process StartDnsSd(string[] args)
    {
        var process = new Process();
        process.StartInfo.FileName = "dns-sd";
        foreach (var arg in args)
            process.StartInfo.ArgumentList.Add(arg);
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        return process;
    }

    private static void KillProcess(Process process)
    {
        try { process.Kill(); } catch { }
    }

    // Matches "Add" lines in dns-sd -B output.
    // Groups: (1) instance name (everything after the service type column).
    [GeneratedRegex(@"\bAdd\b\s+\d+\s+\d+\s+\S+\s+\S+\s+(.+)$", RegexOptions.Multiline)]
    private static partial Regex BrowseLineRegex();

    // Matches "can be reached at HOSTNAME:PORT" in dns-sd -L output.
    [GeneratedRegex(@"can be reached at\s+(\S+?):(\d+)")]
    private static partial Regex ReachableRegex();

    // Matches the rs= key in the TXT record.
    [GeneratedRegex(@"\brs=(/\S+)")]
    private static partial Regex ResourcePathRegex();
}
