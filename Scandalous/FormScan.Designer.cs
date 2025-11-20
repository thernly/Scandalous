namespace Scandalous
{
    partial class FormScan
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScan));
            pictureBox1 = new PictureBox();
            folderBrowserDialogOutputFolder = new FolderBrowserDialog();
            imageList1 = new ImageList(components);
            groupScanControls = new GroupBox();
            LabelStatus = new Label();
            LabelStatusLabel = new Label();
            TextBoxBaseFilename = new TextBox();
            LabelBaseFilename = new Label();
            groupBox1 = new GroupBox();
            LabelTessdataLanguage = new Label();
            labelTessdataFolder = new Label();
            buttonTesseractDataPath = new Button();
            checkBoxOcr = new CheckBox();
            RadioButtonFlatbed = new RadioButton();
            RadioButtonFeederSimplex = new RadioButton();
            RadioButtonFeederDuplex = new RadioButton();
            LabelDpi = new Label();
            ComboBoxDpi = new ComboBox();
            chkExcludeBlankPages = new CheckBox();
            chkAutoDeskew = new CheckBox();
            grpDocumentOptions = new GroupBox();
            radioDocumentCombined = new RadioButton();
            radioDocumentIndividual = new RadioButton();
            grpColorMode = new GroupBox();
            radioButtonColor = new RadioButton();
            radioButtonBlackWhite = new RadioButton();
            radioButtonGrayscale = new RadioButton();
            lstScanners = new ListBox();
            btnGetScannerList = new Button();
            scanButton = new Button();
            LabelOutputFolder = new Label();
            buttonOutputFolder = new Button();
            folderBrowserDialogTessdataFolder = new FolderBrowserDialog();
            comboBoxLanguageCode = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupScanControls.SuspendLayout();
            groupBox1.SuspendLayout();
            grpDocumentOptions.SuspendLayout();
            grpColorMode.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Dock = DockStyle.Right;
            pictureBox1.Location = new Point(424, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(589, 663);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // groupScanControls
            // 
            groupScanControls.Controls.Add(LabelStatus);
            groupScanControls.Controls.Add(LabelStatusLabel);
            groupScanControls.Controls.Add(TextBoxBaseFilename);
            groupScanControls.Controls.Add(LabelBaseFilename);
            groupScanControls.Controls.Add(groupBox1);
            groupScanControls.Controls.Add(grpDocumentOptions);
            groupScanControls.Controls.Add(grpColorMode);
            groupScanControls.Controls.Add(lstScanners);
            groupScanControls.Controls.Add(btnGetScannerList);
            groupScanControls.Controls.Add(scanButton);
            groupScanControls.Controls.Add(LabelOutputFolder);
            groupScanControls.Controls.Add(buttonOutputFolder);
            groupScanControls.Dock = DockStyle.Left;
            groupScanControls.Location = new Point(0, 0);
            groupScanControls.Name = "groupScanControls";
            groupScanControls.Size = new Size(401, 663);
            groupScanControls.TabIndex = 11;
            groupScanControls.TabStop = false;
            // 
            // LabelStatus
            // 
            LabelStatus.AutoSize = true;
            LabelStatus.BorderStyle = BorderStyle.FixedSingle;
            LabelStatus.Location = new Point(22, 532);
            LabelStatus.MaximumSize = new Size(340, 86);
            LabelStatus.MinimumSize = new Size(340, 52);
            LabelStatus.Name = "LabelStatus";
            LabelStatus.Size = new Size(340, 52);
            LabelStatus.TabIndex = 24;
            LabelStatus.Text = "Not Started";
            // 
            // LabelStatusLabel
            // 
            LabelStatusLabel.AutoSize = true;
            LabelStatusLabel.Location = new Point(19, 511);
            LabelStatusLabel.Name = "LabelStatusLabel";
            LabelStatusLabel.Size = new Size(39, 15);
            LabelStatusLabel.TabIndex = 23;
            LabelStatusLabel.Text = "Status";
            // 
            // TextBoxBaseFilename
            // 
            TextBoxBaseFilename.Location = new Point(113, 55);
            TextBoxBaseFilename.MaxLength = 200;
            TextBoxBaseFilename.Name = "TextBoxBaseFilename";
            TextBoxBaseFilename.Size = new Size(252, 23);
            TextBoxBaseFilename.TabIndex = 22;
            TextBoxBaseFilename.Text = "output";
            // 
            // LabelBaseFilename
            // 
            LabelBaseFilename.AutoSize = true;
            LabelBaseFilename.Location = new Point(11, 57);
            LabelBaseFilename.Name = "LabelBaseFilename";
            LabelBaseFilename.Size = new Size(82, 15);
            LabelBaseFilename.TabIndex = 21;
            LabelBaseFilename.Text = "Base Filename";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBoxLanguageCode);
            groupBox1.Controls.Add(LabelTessdataLanguage);
            groupBox1.Controls.Add(labelTessdataFolder);
            groupBox1.Controls.Add(buttonTesseractDataPath);
            groupBox1.Controls.Add(checkBoxOcr);
            groupBox1.Controls.Add(RadioButtonFlatbed);
            groupBox1.Controls.Add(RadioButtonFeederSimplex);
            groupBox1.Controls.Add(RadioButtonFeederDuplex);
            groupBox1.Controls.Add(LabelDpi);
            groupBox1.Controls.Add(ComboBoxDpi);
            groupBox1.Controls.Add(chkExcludeBlankPages);
            groupBox1.Controls.Add(chkAutoDeskew);
            groupBox1.Location = new Point(16, 217);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(364, 165);
            groupBox1.TabIndex = 20;
            groupBox1.TabStop = false;
            groupBox1.Text = "Scan Options";
            // 
            // LabelTessdataLanguage
            // 
            LabelTessdataLanguage.AutoSize = true;
            LabelTessdataLanguage.Location = new Point(35, 125);
            LabelTessdataLanguage.Name = "LabelTessdataLanguage";
            LabelTessdataLanguage.Size = new Size(137, 15);
            LabelTessdataLanguage.TabIndex = 30;
            LabelTessdataLanguage.Text = "Tessdata Language Code";
            // 
            // labelTessdataFolder
            // 
            labelTessdataFolder.AutoSize = true;
            labelTessdataFolder.BorderStyle = BorderStyle.FixedSingle;
            labelTessdataFolder.Location = new Point(185, 95);
            labelTessdataFolder.MinimumSize = new Size(160, 0);
            labelTessdataFolder.Name = "labelTessdataFolder";
            labelTessdataFolder.Size = new Size(160, 17);
            labelTessdataFolder.TabIndex = 29;
            labelTessdataFolder.Text = "c:\\tessdata";
            // 
            // buttonTesseractDataPath
            // 
            buttonTesseractDataPath.Location = new Point(70, 91);
            buttonTesseractDataPath.Name = "buttonTesseractDataPath";
            buttonTesseractDataPath.Size = new Size(102, 23);
            buttonTesseractDataPath.TabIndex = 28;
            buttonTesseractDataPath.Text = "Tessdata Folder";
            buttonTesseractDataPath.UseVisualStyleBackColor = true;
            buttonTesseractDataPath.Click += ButtonTesseractDataPath_Click;
            // 
            // checkBoxOcr
            // 
            checkBoxOcr.AutoSize = true;
            checkBoxOcr.Checked = true;
            checkBoxOcr.CheckState = CheckState.Checked;
            checkBoxOcr.Location = new Point(14, 95);
            checkBoxOcr.Name = "checkBoxOcr";
            checkBoxOcr.Size = new Size(50, 19);
            checkBoxOcr.TabIndex = 27;
            checkBoxOcr.Text = "OCR";
            checkBoxOcr.UseVisualStyleBackColor = true;
            // 
            // RadioButtonFlatbed
            // 
            RadioButtonFlatbed.AutoSize = true;
            RadioButtonFlatbed.Location = new Point(236, 59);
            RadioButtonFlatbed.Name = "RadioButtonFlatbed";
            RadioButtonFlatbed.Size = new Size(64, 19);
            RadioButtonFlatbed.TabIndex = 26;
            RadioButtonFlatbed.Text = "Flatbed";
            RadioButtonFlatbed.UseVisualStyleBackColor = true;
            // 
            // RadioButtonFeederSimplex
            // 
            RadioButtonFeederSimplex.AutoSize = true;
            RadioButtonFeederSimplex.Location = new Point(121, 59);
            RadioButtonFeederSimplex.Name = "RadioButtonFeederSimplex";
            RadioButtonFeederSimplex.Size = new Size(105, 19);
            RadioButtonFeederSimplex.TabIndex = 25;
            RadioButtonFeederSimplex.Text = "Feeder Simplex";
            RadioButtonFeederSimplex.UseVisualStyleBackColor = true;
            // 
            // RadioButtonFeederDuplex
            // 
            RadioButtonFeederDuplex.AutoSize = true;
            RadioButtonFeederDuplex.Checked = true;
            RadioButtonFeederDuplex.Location = new Point(15, 59);
            RadioButtonFeederDuplex.Name = "RadioButtonFeederDuplex";
            RadioButtonFeederDuplex.Size = new Size(100, 19);
            RadioButtonFeederDuplex.TabIndex = 24;
            RadioButtonFeederDuplex.TabStop = true;
            RadioButtonFeederDuplex.Text = "Feeder Duplex";
            RadioButtonFeederDuplex.UseVisualStyleBackColor = true;
            // 
            // LabelDpi
            // 
            LabelDpi.AutoSize = true;
            LabelDpi.Location = new Point(320, 25);
            LabelDpi.Name = "LabelDpi";
            LabelDpi.Size = new Size(25, 15);
            LabelDpi.TabIndex = 23;
            LabelDpi.Text = "DPI";
            // 
            // ComboBoxDpi
            // 
            ComboBoxDpi.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxDpi.FormattingEnabled = true;
            ComboBoxDpi.Items.AddRange(new object[] { "150", "300", "600", "1200" });
            ComboBoxDpi.Location = new Point(261, 20);
            ComboBoxDpi.Name = "ComboBoxDpi";
            ComboBoxDpi.Size = new Size(54, 23);
            ComboBoxDpi.TabIndex = 22;
            // 
            // chkExcludeBlankPages
            // 
            chkExcludeBlankPages.AutoSize = true;
            chkExcludeBlankPages.Checked = true;
            chkExcludeBlankPages.CheckState = CheckState.Checked;
            chkExcludeBlankPages.Location = new Point(122, 24);
            chkExcludeBlankPages.Name = "chkExcludeBlankPages";
            chkExcludeBlankPages.Size = new Size(133, 19);
            chkExcludeBlankPages.TabIndex = 1;
            chkExcludeBlankPages.Text = "Exclude Blank Pages";
            chkExcludeBlankPages.UseVisualStyleBackColor = true;
            // 
            // chkAutoDeskew
            // 
            chkAutoDeskew.AutoSize = true;
            chkAutoDeskew.Checked = true;
            chkAutoDeskew.CheckState = CheckState.Checked;
            chkAutoDeskew.Location = new Point(14, 24);
            chkAutoDeskew.Name = "chkAutoDeskew";
            chkAutoDeskew.Size = new Size(100, 19);
            chkAutoDeskew.TabIndex = 0;
            chkAutoDeskew.Text = "Auto De-skew";
            chkAutoDeskew.UseVisualStyleBackColor = true;
            // 
            // grpDocumentOptions
            // 
            grpDocumentOptions.Controls.Add(radioDocumentCombined);
            grpDocumentOptions.Controls.Add(radioDocumentIndividual);
            grpDocumentOptions.Location = new Point(16, 154);
            grpDocumentOptions.Name = "grpDocumentOptions";
            grpDocumentOptions.Size = new Size(364, 57);
            grpDocumentOptions.TabIndex = 19;
            grpDocumentOptions.TabStop = false;
            grpDocumentOptions.Text = "Document Options";
            // 
            // radioDocumentCombined
            // 
            radioDocumentCombined.AutoSize = true;
            radioDocumentCombined.Checked = true;
            radioDocumentCombined.Location = new Point(14, 19);
            radioDocumentCombined.Name = "radioDocumentCombined";
            radioDocumentCombined.Size = new Size(81, 19);
            radioDocumentCombined.TabIndex = 19;
            radioDocumentCombined.TabStop = true;
            radioDocumentCombined.Text = "Combined";
            radioDocumentCombined.UseVisualStyleBackColor = true;
            // 
            // radioDocumentIndividual
            // 
            radioDocumentIndividual.AutoSize = true;
            radioDocumentIndividual.Location = new Point(115, 19);
            radioDocumentIndividual.Name = "radioDocumentIndividual";
            radioDocumentIndividual.Size = new Size(77, 19);
            radioDocumentIndividual.TabIndex = 18;
            radioDocumentIndividual.Text = "Individual";
            radioDocumentIndividual.UseVisualStyleBackColor = true;
            // 
            // grpColorMode
            // 
            grpColorMode.Controls.Add(radioButtonColor);
            grpColorMode.Controls.Add(radioButtonBlackWhite);
            grpColorMode.Controls.Add(radioButtonGrayscale);
            grpColorMode.Location = new Point(16, 91);
            grpColorMode.Name = "grpColorMode";
            grpColorMode.Size = new Size(364, 57);
            grpColorMode.TabIndex = 18;
            grpColorMode.TabStop = false;
            grpColorMode.Text = "Color Mode";
            // 
            // radioButtonColor
            // 
            radioButtonColor.AutoSize = true;
            radioButtonColor.Location = new Point(236, 24);
            radioButtonColor.Name = "radioButtonColor";
            radioButtonColor.Size = new Size(54, 19);
            radioButtonColor.TabIndex = 18;
            radioButtonColor.Text = "Color";
            radioButtonColor.UseVisualStyleBackColor = true;
            // 
            // radioButtonBlackWhite
            // 
            radioButtonBlackWhite.AutoSize = true;
            radioButtonBlackWhite.Location = new Point(115, 24);
            radioButtonBlackWhite.Name = "radioButtonBlackWhite";
            radioButtonBlackWhite.Size = new Size(100, 19);
            radioButtonBlackWhite.TabIndex = 17;
            radioButtonBlackWhite.Text = "Black && White";
            radioButtonBlackWhite.UseVisualStyleBackColor = true;
            // 
            // radioButtonGrayscale
            // 
            radioButtonGrayscale.AutoSize = true;
            radioButtonGrayscale.Checked = true;
            radioButtonGrayscale.Location = new Point(14, 24);
            radioButtonGrayscale.Name = "radioButtonGrayscale";
            radioButtonGrayscale.Size = new Size(75, 19);
            radioButtonGrayscale.TabIndex = 16;
            radioButtonGrayscale.TabStop = true;
            radioButtonGrayscale.Text = "Grayscale";
            radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // lstScanners
            // 
            lstScanners.FormattingEnabled = true;
            lstScanners.Location = new Point(16, 388);
            lstScanners.Name = "lstScanners";
            lstScanners.Size = new Size(349, 79);
            lstScanners.TabIndex = 17;
            // 
            // btnGetScannerList
            // 
            btnGetScannerList.Location = new Point(16, 473);
            btnGetScannerList.Name = "btnGetScannerList";
            btnGetScannerList.Size = new Size(114, 22);
            btnGetScannerList.TabIndex = 16;
            btnGetScannerList.Text = "Get Scanner List";
            btnGetScannerList.UseVisualStyleBackColor = true;
            btnGetScannerList.Click += ButtonGetScannerList_Click;
            // 
            // scanButton
            // 
            scanButton.Location = new Point(16, 618);
            scanButton.Name = "scanButton";
            scanButton.Size = new Size(75, 23);
            scanButton.TabIndex = 10;
            scanButton.Text = "Scan";
            scanButton.UseVisualStyleBackColor = true;
            scanButton.Click += ScanButton_ClickAsync;
            // 
            // LabelOutputFolder
            // 
            LabelOutputFolder.AutoSize = true;
            LabelOutputFolder.BorderStyle = BorderStyle.FixedSingle;
            LabelOutputFolder.Location = new Point(115, 30);
            LabelOutputFolder.MinimumSize = new Size(250, 0);
            LabelOutputFolder.Name = "LabelOutputFolder";
            LabelOutputFolder.Size = new Size(250, 17);
            LabelOutputFolder.TabIndex = 9;
            LabelOutputFolder.Text = "c:\\test";
            // 
            // buttonOutputFolder
            // 
            buttonOutputFolder.Location = new Point(6, 26);
            buttonOutputFolder.Name = "buttonOutputFolder";
            buttonOutputFolder.Size = new Size(94, 23);
            buttonOutputFolder.TabIndex = 8;
            buttonOutputFolder.Text = "Output Folder";
            buttonOutputFolder.UseVisualStyleBackColor = true;
            buttonOutputFolder.Click += ButtonOutputFolder_Click;
            // 
            // comboBoxLanguageCode
            // 
            comboBoxLanguageCode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLanguageCode.FormattingEnabled = true;
            comboBoxLanguageCode.Location = new Point(185, 122);
            comboBoxLanguageCode.Name = "comboBoxLanguageCode";
            comboBoxLanguageCode.Size = new Size(121, 23);
            comboBoxLanguageCode.TabIndex = 31;
            // 
            // FormScan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1013, 663);
            Controls.Add(groupScanControls);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormScan";
            Text = "Scandalous";
            FormClosing += FormScan_Closing;
            Load += FormScan_Load;
            Shown += FormScan_Shown;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupScanControls.ResumeLayout(false);
            groupScanControls.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grpDocumentOptions.ResumeLayout(false);
            grpDocumentOptions.PerformLayout();
            grpColorMode.ResumeLayout(false);
            grpColorMode.PerformLayout();
            ResumeLayout(false);
        }


        #endregion
        private PictureBox pictureBox1;
        private FolderBrowserDialog folderBrowserDialogOutputFolder;
        private ImageList imageList1;
        private GroupBox groupScanControls;
        private Button scanButton;
        private Label LabelOutputFolder;
        private Button buttonOutputFolder;
        private Button btnGetScannerList;
        private ListBox lstScanners;
        private GroupBox grpColorMode;
        private RadioButton radioButtonColor;
        private RadioButton radioButtonBlackWhite;
        private RadioButton radioButtonGrayscale;
        private GroupBox grpDocumentOptions;
        private RadioButton radioDocumentCombined;
        private RadioButton radioDocumentIndividual;
        private GroupBox groupBox1;
        private CheckBox chkExcludeBlankPages;
        private CheckBox chkAutoDeskew;
        private Label LabelBaseFilename;
        private TextBox TextBoxBaseFilename;
        private Label LabelStatusLabel;
        private Label LabelStatus;
        private Label LabelDpi;
        private ComboBox ComboBoxDpi;
        private RadioButton RadioButtonFeederDuplex;
        private RadioButton RadioButtonFlatbed;
        private RadioButton RadioButtonFeederSimplex;
        private CheckBox checkBoxOcr;
        private Button buttonTesseractDataPath;
        private Label labelTessdataFolder;
        private FolderBrowserDialog folderBrowserDialogTessdataFolder;
        private Label LabelTessdataLanguage;
        private ComboBox comboBoxLanguageCode;
    }
}
