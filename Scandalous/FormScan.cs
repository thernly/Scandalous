using NAPS2.Scan;
using NAPS2.Scan.Exceptions;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using System.Runtime.CompilerServices;

namespace Scandalous;

public partial class FormScan : Form
{
    private readonly IDocumentScanner _scanner;
    private readonly IConfigurationManager _configManager;
    private readonly IScanConfigurationMapper _configMapper;
    private readonly IPdfService _pdfService;
    private readonly ILanguageCodeService _languageService;
    private readonly IScanExceptionHandler _exceptionHandler;
    private readonly List<string> _imageFileList;
    private ScanConfiguration _scanConfiguration;

    public FormScan(
        IDocumentScanner scanner, 
        IConfigurationManager configManager,
        IScanConfigurationMapper configMapper,
        IPdfService pdfService,
        ILanguageCodeService languageService,
        IScanExceptionHandler exceptionHandler)
    {
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        _configMapper = configMapper ?? throw new ArgumentNullException(nameof(configMapper));
        _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        
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
            var uiState = BuildUIStateFromControls();
            scanConfiguration = _configMapper.BuildConfigurationFromUIState(uiState);
        }
        catch (Exception ex)
        {
            var result = _exceptionHandler.HandleScanException(ex);
            ShowExceptionMessage(result.UserMessage);
            return;
        }

        try
        {
            PrepareForScan();
            await _scanner.ScanDocuments(scanConfiguration);
            LabelStatus.Text = "Scanning completed.";
        }
        catch (Exception ex)
        {
            var result = _exceptionHandler.HandleScanException(ex);
            ShowExceptionMessage(result.UserMessage);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
        
        ShowPDF(scanConfiguration);
    }

    private UIState BuildUIStateFromControls()
    {
        return new UIState
        {
            OutputFolder = LabelOutputFolder.Text,
            BaseFileName = TextBoxBaseFilename.Text,
            AutoDeskew = chkAutoDeskew.Checked,
            ExcludeBlankPages = chkExcludeBlankPages.Checked,
            DocumentCombined = radioDocumentCombined.Checked,
            DocumentIndividual = radioDocumentIndividual.Checked,
            ColorModeGrayscale = radioButtonGrayscale.Checked,
            ColorModeBlackWhite = radioButtonBlackWhite.Checked,
            ColorModeColor = radioButtonColor.Checked,
            FeederDuplex = RadioButtonFeederDuplex.Checked,
            FeederSimplex = RadioButtonFeederSimplex.Checked,
            Flatbed = RadioButtonFlatbed.Checked,
            Dpi = int.Parse(ComboBoxDpi.Text),
            OcrEnabled = checkBoxOcr.Checked,
            TessdataFolder = labelTessdataFolder.Text,
            SelectedLanguageCode = comboBoxLanguageCode.SelectedItem?.ToString() ?? _languageService.GetDefaultLanguageCode(),
            SelectedScannerName = lstScanners.SelectedItem?.ToString() ?? string.Empty
        };
    }

    private async Task LoadScannerList()
    {
        lstScanners.Items.Clear();
        var devices = await _scanner.GetScanDevicesAsync();
        foreach (var device in devices)
        {
            lstScanners.Items.Add(device.Name);
        }
        await SetSelectedScanner();
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

    private async void ButtonGetScannerList_Click(object sender, EventArgs e)
    {
        await LoadScannerList();
    }

    private void ShowPDF(ScanConfiguration scanConfiguration)
    {
        if (scanConfiguration.DocumentOptions == DocumentOptions.Combined)
        {
            try
            {
                var pdfFilePath = _pdfService.GetPdfFilePath(scanConfiguration);
                if (_pdfService.PdfFileExists(pdfFilePath))
                {
                    _pdfService.OpenPdfFile(pdfFilePath);
                }
            }
            catch (Exception ex)
            {
                var result = _exceptionHandler.HandleScanException(ex);
                LabelStatus.Text = result.UserMessage;
            }
        }
    }

    private async void FormScan_Load(object sender, EventArgs e)
    {
        _scanConfiguration = await _configManager.LoadConfigurationAsync();
        await LoadScannerList();
        ApplyConfigurationToUI(_scanConfiguration);
        await SetSelectedScanner();
    }

    private async Task SetSelectedScanner()
    {
        if (lstScanners.Items.Count > 0 && !string.IsNullOrEmpty(_scanConfiguration.SelectedScannerName))
        {
            lstScanners.SelectedItem = _scanConfiguration.SelectedScannerName;
        }
        else if (lstScanners.Items.Count > 0)
        {
            lstScanners.SelectedIndex = 0;
        }
    }

    private void ApplyConfigurationToUI(ScanConfiguration scanConfiguration)
    {
        var uiState = _configMapper.BuildUIStateFromConfiguration(scanConfiguration);
        
        LabelOutputFolder.Text = uiState.OutputFolder;
        TextBoxBaseFilename.Text = uiState.BaseFileName;
        chkAutoDeskew.Checked = uiState.AutoDeskew;
        chkExcludeBlankPages.Checked = uiState.ExcludeBlankPages;
        radioDocumentIndividual.Checked = uiState.DocumentIndividual;
        radioDocumentCombined.Checked = uiState.DocumentCombined;
        radioButtonGrayscale.Checked = uiState.ColorModeGrayscale;
        radioButtonBlackWhite.Checked = uiState.ColorModeBlackWhite;
        radioButtonColor.Checked = uiState.ColorModeColor;
        RadioButtonFeederDuplex.Checked = uiState.FeederDuplex;
        RadioButtonFeederSimplex.Checked = uiState.FeederSimplex;
        RadioButtonFlatbed.Checked = uiState.Flatbed;
        ComboBoxDpi.Text = uiState.Dpi.ToString();
        checkBoxOcr.Checked = uiState.OcrEnabled;
        labelTessdataFolder.Text = uiState.TessdataFolder;
        
        PopulateLanguageCodesDropDownList(uiState.TessdataFolder, uiState.SelectedLanguageCode);
    }

    private void PopulateLanguageCodesDropDownList(string tessdataFolder, string userSelectedLanguageCode)
    {         
        comboBoxLanguageCode.Items.Clear();
        var languageCodes = _languageService.GetAvailableLanguageCodes(tessdataFolder, userSelectedLanguageCode);
        
        foreach (var code in languageCodes)
        {
            comboBoxLanguageCode.Items.Add(code);
        }
        
        if (comboBoxLanguageCode.Items.Count > 0)
        {
            var bestLanguageCode = _languageService.GetBestLanguageCode(tessdataFolder, userSelectedLanguageCode);
            comboBoxLanguageCode.SelectedItem = bestLanguageCode;
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
        var uiState = BuildUIStateFromControls();
        var scanConfiguration = _configMapper.BuildConfigurationFromUIState(uiState);
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
