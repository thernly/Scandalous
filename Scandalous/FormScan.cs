using NAPS2.Scan.Exceptions;


namespace ScanUtility;

public partial class FormScan : Form
{
    private readonly DocumentScanner scanner;
    private readonly List<string> _imageFileList;

    public FormScan()
    {
        InitializeComponent();
        scanner = new DocumentScanner();
        scanner.PageScanned += OnPageScanned;
        _imageFileList = [];
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
        var scannerPaperSource = GetScannerPaperSource();


        ScanConfiguration scanConfiguration;
        try
        {
            scanConfiguration = new ScanConfiguration(LabelOutputFolder.Text, TextBoxBaseFilename.Text, colorMode,
                                                          documentOptions, chkAutoDeskew.Checked, chkExcludeBlankPages.Checked, dpi,
                                                          scannerPaperSource, checkBoxOcr.Checked, labelTessdataFolder.Text);
        }
        catch (Exception ex)
        {
            ShowExceptionMessage($"An error occurred: {ex.Message}");
            Cursor = Cursors.Default;
            return;
        }

        try
        {
            _imageFileList.Clear();
            pictureBox1.Image = null;
            LabelStatus.Text = "Scanning...";
            await scanner.ScanDocuments(scanConfiguration);
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
            ShowExceptionMessage($"An error occurred: {ex.Message}");
        }
        finally
        {
            Cursor = Cursors.Default;
        }
        ShowPDF(scanConfiguration);
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
        RadioButtonFeederDuplex.Checked = scanConfiguration.ScannerPaperSource == ScannerPaperSource.FeederDuplex;
        RadioButtonFeederSimplex.Checked = scanConfiguration.ScannerPaperSource == ScannerPaperSource.FeederSimplex;
        RadioButtonFlatbed.Checked = scanConfiguration.ScannerPaperSource == ScannerPaperSource.Flatbed;
        ComboBoxDpi.Text = scanConfiguration.ScanResolutionDPI.ToString();
        checkBoxOcr.Checked = scanConfiguration.OcrEnabled;
        labelTessdataFolder.Text = scanConfiguration.TessdataFolder;
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
        var scannerPaperSource = GetScannerPaperSource();
        var scanConfiguration = new ScanConfiguration(LabelOutputFolder.Text, TextBoxBaseFilename.Text, GetScannerColorMode(),
            radioDocumentCombined.Checked ? DocumentOptions.Combined : DocumentOptions.Individual, chkAutoDeskew.Checked,
            chkExcludeBlankPages.Checked, int.Parse(ComboBoxDpi.Text), scannerPaperSource, checkBoxOcr.Checked, labelTessdataFolder.Text);
        await configManager.SaveConfigurationAsync(scanConfiguration);
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
