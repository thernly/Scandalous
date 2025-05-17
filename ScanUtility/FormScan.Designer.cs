namespace ScanUtility
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
            pictureBox1 = new PictureBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            imageList1 = new ImageList(components);
            groupScanControls = new GroupBox();
            LabelStatus = new Label();
            LabelStatusLabel = new Label();
            TextBoxBaseFilename = new TextBox();
            LabelBaseFilename = new Label();
            groupBox1 = new GroupBox();
            chkExcludeBlankPages = new CheckBox();
            chkAutoDeskew = new CheckBox();
            grpDocumentOptions = new GroupBox();
            LabelDpi = new Label();
            ComboBoxDpi = new ComboBox();
            radioDocumentCombined = new RadioButton();
            radioDocumentIndividual = new RadioButton();
            grpColorMode = new GroupBox();
            radioButtonColor = new RadioButton();
            radioButtonBlackWhite = new RadioButton();
            radioButtonGrayscale = new RadioButton();
            lstScanners = new ListBox();
            btnGetScannerList = new Button();
            buttonLoad = new Button();
            scanButton = new Button();
            LabelOutputFolder = new Label();
            buttonOutputFolder = new Button();
            buttonNext = new Button();
            buttonPrevious = new Button();
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
            pictureBox1.Size = new Size(589, 643);
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
            groupScanControls.Controls.Add(buttonLoad);
            groupScanControls.Controls.Add(scanButton);
            groupScanControls.Controls.Add(LabelOutputFolder);
            groupScanControls.Controls.Add(buttonOutputFolder);
            groupScanControls.Controls.Add(buttonNext);
            groupScanControls.Controls.Add(buttonPrevious);
            groupScanControls.Dock = DockStyle.Left;
            groupScanControls.Location = new Point(0, 0);
            groupScanControls.Name = "groupScanControls";
            groupScanControls.Size = new Size(376, 643);
            groupScanControls.TabIndex = 11;
            groupScanControls.TabStop = false;
            // 
            // LabelStatus
            // 
            LabelStatus.AutoSize = true;
            LabelStatus.BorderStyle = BorderStyle.FixedSingle;
            LabelStatus.Location = new Point(22, 442);
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
            LabelStatusLabel.Location = new Point(19, 421);
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
            groupBox1.Controls.Add(chkExcludeBlankPages);
            groupBox1.Controls.Add(chkAutoDeskew);
            groupBox1.Location = new Point(16, 229);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(349, 57);
            groupBox1.TabIndex = 20;
            groupBox1.TabStop = false;
            groupBox1.Text = "Scan Options";
            // 
            // chkExcludeBlankPages
            // 
            chkExcludeBlankPages.AutoSize = true;
            chkExcludeBlankPages.Checked = true;
            chkExcludeBlankPages.CheckState = CheckState.Checked;
            chkExcludeBlankPages.Location = new Point(115, 24);
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
            grpDocumentOptions.Controls.Add(LabelDpi);
            grpDocumentOptions.Controls.Add(ComboBoxDpi);
            grpDocumentOptions.Controls.Add(radioDocumentCombined);
            grpDocumentOptions.Controls.Add(radioDocumentIndividual);
            grpDocumentOptions.Location = new Point(16, 166);
            grpDocumentOptions.Name = "grpDocumentOptions";
            grpDocumentOptions.Size = new Size(349, 57);
            grpDocumentOptions.TabIndex = 19;
            grpDocumentOptions.TabStop = false;
            grpDocumentOptions.Text = "Document Options";
            // 
            // LabelDpi
            // 
            LabelDpi.AutoSize = true;
            LabelDpi.Location = new Point(274, 22);
            LabelDpi.Name = "LabelDpi";
            LabelDpi.Size = new Size(25, 15);
            LabelDpi.TabIndex = 21;
            LabelDpi.Text = "DPI";
            // 
            // ComboBoxDpi
            // 
            ComboBoxDpi.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxDpi.FormattingEnabled = true;
            ComboBoxDpi.Items.AddRange(new object[] { "150", "300", "600", "1200" });
            ComboBoxDpi.Location = new Point(215, 17);
            ComboBoxDpi.Name = "ComboBoxDpi";
            ComboBoxDpi.Size = new Size(54, 23);
            ComboBoxDpi.TabIndex = 20;
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
            grpColorMode.Location = new Point(16, 103);
            grpColorMode.Name = "grpColorMode";
            grpColorMode.Size = new Size(349, 57);
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
            lstScanners.Location = new Point(16, 298);
            lstScanners.Name = "lstScanners";
            lstScanners.Size = new Size(349, 79);
            lstScanners.TabIndex = 17;
            // 
            // btnGetScannerList
            // 
            btnGetScannerList.Location = new Point(16, 383);
            btnGetScannerList.Name = "btnGetScannerList";
            btnGetScannerList.Size = new Size(114, 22);
            btnGetScannerList.TabIndex = 16;
            btnGetScannerList.Text = "Get Scanner List";
            btnGetScannerList.UseVisualStyleBackColor = true;
            btnGetScannerList.Click += ButtonGetScannerList_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(178, 572);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(116, 23);
            buttonLoad.TabIndex = 11;
            buttonLoad.Text = "Load Images";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += ButtonLoad_Click;
            // 
            // scanButton
            // 
            scanButton.Location = new Point(16, 601);
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
            // buttonNext
            // 
            buttonNext.Anchor = AnchorStyles.Bottom;
            buttonNext.Location = new Point(259, 601);
            buttonNext.Name = "buttonNext";
            buttonNext.Size = new Size(75, 24);
            buttonNext.TabIndex = 7;
            buttonNext.Text = "Next";
            buttonNext.UseVisualStyleBackColor = true;
            buttonNext.Click += ButtonNext_Click;
            // 
            // buttonPrevious
            // 
            buttonPrevious.Anchor = AnchorStyles.Bottom;
            buttonPrevious.Location = new Point(178, 601);
            buttonPrevious.Name = "buttonPrevious";
            buttonPrevious.Size = new Size(75, 24);
            buttonPrevious.TabIndex = 6;
            buttonPrevious.Text = "Previous";
            buttonPrevious.UseVisualStyleBackColor = true;
            buttonPrevious.Click += ButtonPrevious_Click;
            // 
            // FormScan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1013, 643);
            Controls.Add(groupScanControls);
            Controls.Add(pictureBox1);
            Name = "FormScan";
            Text = "ScanUtility";
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
        private FolderBrowserDialog folderBrowserDialog1;
        private ImageList imageList1;
        private GroupBox groupScanControls;
        private Button buttonLoad;
        private Button scanButton;
        private Label LabelOutputFolder;
        private Button buttonOutputFolder;
        private Button buttonNext;
        private Button buttonPrevious;
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
        private ComboBox ComboBoxDpi;
        private Label LabelDpi;
    }
}
