

using NAPS2.Scan.Exceptions;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private DocumentScanner scanner;
    private int _currentImageNumber = 0;
    private readonly List<string> _imageFileList;

    public Form1()
    {
        InitializeComponent();
        scanner = new DocumentScanner();
        scanner.PageScanned += OnPageScanned;
        _imageFileList = new List<string>();
    }

    private async void scanButton_ClickAsync(object sender, EventArgs e)
    {
        _imageFileList.Clear();
        try
        {
            await scanner.ScanDocumentsFromFeeder(label1.Text, GetScannerColorMode());
        }
        catch (DeviceFeederEmptyException)
        {
            MessageBox.Show("The device feeder is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (ScanDriverUnknownException)
        {
            MessageBox.Show($"Scan Driver Unknown Exception", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        folderBrowserDialog1.SelectedPath = label1.Text;
        var result = folderBrowserDialog1.ShowDialog();
        if (result == DialogResult.OK)
        {
            label1.Text = folderBrowserDialog1.SelectedPath;
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

    private void buttonPrevious_Click(object sender, EventArgs e)
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

    private void buttonNext_Click(object sender, EventArgs e)
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

    private void button2_Click(object sender, EventArgs e)
    {

        _imageFileList.AddRange(Directory.GetFiles(label1.Text, "*.png"));
        pictureBox1.Image = Image.FromFile(_imageFileList[0]);
    }

    private ScannerColorMode GetScannerColorMode()
    {
        if (radioButtonGrayscale.Checked) { return ScannerColorMode.Grayscale; }
        if (radioButtonBlackWhite.Checked) { return ScannerColorMode.BlackAndWhite; }
        if (radioButtonColor.Checked) { return ScannerColorMode.Color; }
        return ScannerColorMode.Grayscale;
    }
    
}
