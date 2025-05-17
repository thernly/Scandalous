using NAPS2.Scan.Exceptions;


namespace ScanUtility;

public partial class FormScan : Form
{
    private readonly DocumentScanner scanner;
    private int _currentImageNumber = 0;
    private readonly List<string> _imageFileList;
    //private List<ScanDevice>? _devices;

    public FormScan()
    {
        InitializeComponent();
        scanner = new DocumentScanner();
        scanner.PageScanned += OnPageScanned;
        _imageFileList = new List<string>();
    }

    private async void ScanButton_ClickAsync(object sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        LabelStatus.Text = "Building configuration...";
        var documentOptions = DocumentOptions.Individual;
        if (radioDocumentCombined.Checked)
        {
            documentOptions = DocumentOptions.Combined;
        }
        var colorMode = GetScannerColorMode();
        var dpi = int.Parse(ComboBoxDpi.Text);

        ScanConfiguration scanConfiguration;
        try
        {
            scanConfiguration = new ScanConfiguration(LabelOutputFolder.Text, TextBoxBaseFilename.Text, colorMode,
                                                          documentOptions, chkAutoDeskew.Checked, chkExcludeBlankPages.Checked, dpi,
                                                          ScannerPaperSource.FeederDuplex);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LabelStatus.Text = $"{ex.Message}";
            Cursor = Cursors.Default;
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LabelStatus.Text = $"An error occurred: {ex.Message}";
            Cursor = Cursors.Default;
            return;
        }
        _imageFileList.Clear();
        try
        {
            LabelStatus.Text = "Scanning...";
            await scanner.ScanDocumentsFromFeeder(scanConfiguration);
            LabelStatus.Text = "Scanning completed.";
        }
        catch (DeviceFeederEmptyException)
        {
            MessageBox.Show("The device feeder is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LabelStatus.Text = "The device feeder is empty.";
        }
        catch (ScanDriverUnknownException)
        {
            MessageBox.Show($"Scan Driver Unknown Exception", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LabelStatus.Text = "Scan Driver Unknown.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LabelStatus.Text = $"An error occurred: {ex.Message}";
        }
        finally
        {
            Cursor = Cursors.Default;
        }
        ShowPDF(scanConfiguration);
    }

    private void ButtonOutputFolder_Click(object sender, EventArgs e)
    {
        folderBrowserDialog1.SelectedPath = LabelOutputFolder.Text;
        var result = folderBrowserDialog1.ShowDialog();
        if (result == DialogResult.OK)
        {
            LabelOutputFolder.Text = folderBrowserDialog1.SelectedPath;
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

    private void ButtonPrevious_Click(object sender, EventArgs e)
    {
        var count = _imageFileList.Count;
        if (count > 0)
        {
            _currentImageNumber--;
            if (_currentImageNumber < 0)
            {
                _currentImageNumber = count - 1;
            }
            pictureBox1.Image = Image.FromFile(_imageFileList[_currentImageNumber]);
        }

    }

    private void ButtonNext_Click(object sender, EventArgs e)
    {
        var count = _imageFileList.Count;
        if (count > 0)
        {
            _currentImageNumber++;
            if (_currentImageNumber > count - 1)
            {
                _currentImageNumber = 0;
            }
            pictureBox1.Image = Image.FromFile(_imageFileList[_currentImageNumber]);
        }

    }

    private void ButtonLoad_Click(object sender, EventArgs e)
    {
        Cursor = Cursors.WaitCursor;
        try
        {
            _imageFileList.AddRange(Directory.GetFiles(LabelOutputFolder.Text, "*.png"));
            pictureBox1.Image = Image.FromFile(_imageFileList[0]);
        }
        finally { Cursor = Cursors.Default; }
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
        var devices = await scanner.GetScanDevicesAsync();
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
        var configManager = new ConfigurationManager();
        var scanConfiguration = await configManager.LoadConfigurationAsync();
        LabelOutputFolder.Text = scanConfiguration.OutputFolder;
        TextBoxBaseFilename.Text = scanConfiguration.OutputBaseFileName;
        chkAutoDeskew.Checked = scanConfiguration.AutoDeskew;
        chkExcludeBlankPages.Checked = scanConfiguration.ExcludeBlankPages;
        radioDocumentIndividual.Checked = scanConfiguration.DocumentOptions == DocumentOptions.Individual;
        radioDocumentCombined.Checked = scanConfiguration.DocumentOptions == DocumentOptions.Combined;
        radioButtonGrayscale.Checked = scanConfiguration.ColorMode == ScannerColorMode.Grayscale;
        radioButtonBlackWhite.Checked = scanConfiguration.ColorMode == ScannerColorMode.BlackAndWhite;
        radioButtonColor.Checked = scanConfiguration.ColorMode == ScannerColorMode.Color;
        ComboBoxDpi.Text = scanConfiguration.ScanResolutionDPI.ToString();
    }

    private void FormScan_Shown(object sender, EventArgs e)
    {
        TextBoxBaseFilename.SelectAll();
        TextBoxBaseFilename.Focus();
        ComboBoxDpi.SelectedIndex = 1;
    }

    private async void FormScan_Closing(object sender, FormClosingEventArgs e)
    {
        var configManager = new ConfigurationManager();
        var scanConfiguration = new ScanConfiguration(LabelOutputFolder.Text, TextBoxBaseFilename.Text, GetScannerColorMode(),
            radioDocumentCombined.Checked ? DocumentOptions.Combined : DocumentOptions.Individual, chkAutoDeskew.Checked,
            chkExcludeBlankPages.Checked, int.Parse(ComboBoxDpi.Text), ScannerPaperSource.FeederDuplex);
        await configManager.SaveConfigurationAsync(scanConfiguration);
    }
}
