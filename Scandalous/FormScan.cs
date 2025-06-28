using NAPS2.Scan.Exceptions;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;


namespace ScanUtility;

public partial class FormScan : Form
{
    private readonly IDocumentScanner _scanner;
    private readonly IConfigurationManager _configManager;
    private readonly List<string> _imageFileList;

    public FormScan(IDocumentScanner scanner, IConfigurationManager configManager)
    {
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        
        InitializeComponent();
        _scanner.PageScanned += OnPageScanned;
        _imageFileList = [];
    }

    private async void ScanButton_ClickAsync(object sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        LabelStatus.Text = "Building configuration...";
        ScanConfiguration scanConfiguration;
        try
        {
            scanConfiguration = BuildScanConfigurationFromUI();
        }
        catch (Exception ex)
        {
            HandleScanException(ex);
            return;
        }

        try
        {
            PrepareForScan();
            await _scanner.ScanDocuments(scanConfiguration);
            LabelStatus.Text = "Scanning completed.";
        }
        catch (DeviceFeederEmptyException)
        {
            ShowExceptionMessage("The device feeder is empty.");
        }
        catch (ScanDriverUnknownException)
        {
            ShowExceptionMessage("The scan driver is unknown. Please check the scanner connection and drivers.");
        }
        catch (Exception ex)
        {
            HandleScanException(ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
        ShowPDF(scanConfiguration);
    }

    private ScanConfiguration BuildScanConfigurationFromUI()
    {
        var documentOptions = radioDocumentCombined.Checked ? DocumentOptions.Combined : DocumentOptions.Individual;
        var colorMode = GetScannerColorMode();
        var dpi = int.Parse(ComboBoxDpi.Text);
        var scannerPaperSource = GetScannerPaperSource();
        return new ScanConfiguration(
            LabelOutputFolder.Text,
            TextBoxBaseFilename.Text,
            colorMode,
            documentOptions,
            chkAutoDeskew.Checked,
            chkExcludeBlankPages.Checked,
            dpi,
            scannerPaperSource,
            checkBoxOcr.Checked,
            labelTessdataFolder.Text
        );
    }

    private void HandleScanException(Exception ex)
    {
        ShowExceptionMessage($"An error occurred: {ex.Message}");
        Cursor = Cursors.Default;
    }

    private void PrepareForScan()
    {
        _imageFileList.Clear();
        pictureBox1.Image = null;
        LabelStatus.Text = "Scanning...";
    }

    private void ShowExceptionMessage(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        LabelStatus.Text = message;
    }

    private ScannerPaperSource GetScannerPaperSource()
    {
        if (RadioButtonFeederDuplex.Checked) return ScannerPaperSource.FeederDuplex;
        if (RadioButtonFeederSimplex.Checked) return ScannerPaperSource.FeederSimplex;
        if (RadioButtonFlatbed.Checked) return ScannerPaperSource.Flatbed;
        return ScannerPaperSource.Auto;
    }

    private void ButtonOutputFolder_Click(object sender, EventArgs e)
    {
        folderBrowserDialogOutputFolder.SelectedPath = LabelOutputFolder.Text;
        var result = folderBrowserDialogOutputFolder.ShowDialog();
        if (result == DialogResult.OK)
        {
            LabelOutputFolder.Text = folderBrowserDialogOutputFolder.SelectedPath;
        }
    }

    private void OnPageScanned(object? sender, PageScannedEventArgs e)
    {
        _imageFileList.Add(e.ImageFilePath);

        using var image = Image.FromFile(e.ImageFilePath);
        pictureBox1.Image = image;
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox1.Refresh();
    }

    private ScannerColorMode GetScannerColorMode()
    {
        if (radioButtonGrayscale.Checked) { return ScannerColorMode.Grayscale; }
        if (radioButtonBlackWhite.Checked) { return ScannerColorMode.BlackAndWhite; }
        if (radioButtonColor.Checked) { return ScannerColorMode.Color; }
        return ScannerColorMode.Grayscale;
    }

    private async void ButtonGetScannerList_Click(object sender, EventArgs e)
    {
        var devices = await _scanner.GetScanDevicesAsync();
        foreach (var device in devices)
        {
            lstScanners.Items.Add(device.Name);
        }

    }

    private void ShowPDF(ScanConfiguration scanConfiguration)
    {
        if (scanConfiguration.DocumentOptions == DocumentOptions.Combined)
        {

            var pdfFilePath = Path.Combine(scanConfiguration.OutputFolder, $"{scanConfiguration.OutputBaseFileName}.pdf");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfFilePath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                LabelStatus.Text = $"There was an error opening the PDF in a browser: {ex.Message}";
            }
        }
    }

    private async void FormScan_Load(object sender, EventArgs e)
    {
        var scanConfiguration = await _configManager.LoadConfigurationAsync();
        ApplyConfigurationToUI(scanConfiguration);
    }

    private void ApplyConfigurationToUI(ScanConfiguration scanConfiguration)
    {
        LabelOutputFolder.Text = scanConfiguration.OutputFolder;
        TextBoxBaseFilename.Text = scanConfiguration.OutputBaseFileName;
        chkAutoDeskew.Checked = scanConfiguration.AutoDeskew;
        chkExcludeBlankPages.Checked = scanConfiguration.ExcludeBlankPages;
        radioDocumentIndividual.Checked = scanConfiguration.DocumentOptions == DocumentOptions.Individual;
        radioDocumentCombined.Checked = scanConfiguration.DocumentOptions == DocumentOptions.Combined;
        radioButtonGrayscale.Checked = scanConfiguration.ColorMode == ScannerColorMode.Grayscale;
        radioButtonBlackWhite.Checked = scanConfiguration.ColorMode == ScannerColorMode.BlackAndWhite;
        radioButtonColor.Checked = scanConfiguration.ColorMode == ScannerColorMode.Color;
        RadioButtonFeederDuplex.Checked = scanConfiguration.ScannerPaperSource == ScannerPaperSource.FeederDuplex;
        RadioButtonFeederSimplex.Checked = scanConfiguration.ScannerPaperSource == ScannerPaperSource.FeederSimplex;
        RadioButtonFlatbed.Checked = scanConfiguration.ScannerPaperSource == ScannerPaperSource.Flatbed;
        ComboBoxDpi.Text = scanConfiguration.ScanResolutionDPI.ToString();
        checkBoxOcr.Checked = scanConfiguration.OcrEnabled;
        labelTessdataFolder.Text = scanConfiguration.TessdataFolder;
        PopulateLanguageCodesDropDownList(scanConfiguration.TessdataLanguageCode);
    }

    private void PopulateLanguageCodesDropDownList(string userSelectedLanguageCode)
    {         
        comboBoxLanguageCode.Items.Clear();
        var languageCodes = _configManager.GetInstalledTessdataLanguageCodes(labelTessdataFolder.Text);
        foreach (var code in languageCodes)
        {
            comboBoxLanguageCode.Items.Add(code);
        }
        if (comboBoxLanguageCode.Items.Count > 0)
        {
            if (!string.IsNullOrEmpty(userSelectedLanguageCode) && comboBoxLanguageCode.Items.Contains(userSelectedLanguageCode))
            {
                comboBoxLanguageCode.SelectedItem = userSelectedLanguageCode; // Select user's preferred language code if available
            }
            else
            {
                comboBoxLanguageCode.SelectedIndex = 0; // Fallback to the first item if user's preferred code is not available
            }
        }
    }

    private void FormScan_Shown(object sender, EventArgs e)
    {
        TextBoxBaseFilename.SelectAll();
        TextBoxBaseFilename.Focus();
        ComboBoxDpi.SelectedIndex = 1;
    }

    private async void FormScan_Closing(object sender, FormClosingEventArgs e)
    {
        var scannerPaperSource = GetScannerPaperSource();
        var selectedLanguageCode = comboBoxLanguageCode.SelectedItem?.ToString() ?? "eng"; // Default to "eng" if no selection
        var scanConfiguration = new ScanConfiguration(LabelOutputFolder.Text, TextBoxBaseFilename.Text, GetScannerColorMode(),
            radioDocumentCombined.Checked ? DocumentOptions.Combined : DocumentOptions.Individual, chkAutoDeskew.Checked,
            chkExcludeBlankPages.Checked, int.Parse(ComboBoxDpi.Text), scannerPaperSource, checkBoxOcr.Checked, labelTessdataFolder.Text,
            selectedLanguageCode);
        await _configManager.SaveConfigurationAsync(scanConfiguration);
    }

    private void ButtonTesseractDataPath_Click(object sender, EventArgs e)
    {
        folderBrowserDialogTessdataFolder.SelectedPath = labelTessdataFolder.Text;
        var result = folderBrowserDialogTessdataFolder.ShowDialog();
        if (result == DialogResult.OK)
        {
            labelTessdataFolder.Text = folderBrowserDialogTessdataFolder.SelectedPath;
        }

    }
}
