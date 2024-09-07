namespace WinFormsApp1
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
            radioButtonColor = new RadioButton();
            radioButtonBlackWhite = new RadioButton();
            radioButtonGrayscale = new RadioButton();
            LabelMode = new Label();
            buttonLoad = new Button();
            scanButton = new Button();
            label1 = new Label();
            buttonOutputFolder = new Button();
            buttonNext = new Button();
            buttonPrevious = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupScanControls.SuspendLayout();
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
            groupScanControls.Controls.Add(radioButtonColor);
            groupScanControls.Controls.Add(radioButtonBlackWhite);
            groupScanControls.Controls.Add(radioButtonGrayscale);
            groupScanControls.Controls.Add(LabelMode);
            groupScanControls.Controls.Add(buttonLoad);
            groupScanControls.Controls.Add(scanButton);
            groupScanControls.Controls.Add(label1);
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
            // radioButtonColor
            // 
            radioButtonColor.AutoSize = true;
            radioButtonColor.Location = new Point(244, 56);
            radioButtonColor.Name = "radioButtonColor";
            radioButtonColor.Size = new Size(54, 19);
            radioButtonColor.TabIndex = 15;
            radioButtonColor.Text = "Color";
            radioButtonColor.UseVisualStyleBackColor = true;
            // 
            // radioButtonBlackWhite
            // 
            radioButtonBlackWhite.AutoSize = true;
            radioButtonBlackWhite.Location = new Point(138, 56);
            radioButtonBlackWhite.Name = "radioButtonBlackWhite";
            radioButtonBlackWhite.Size = new Size(100, 19);
            radioButtonBlackWhite.TabIndex = 14;
            radioButtonBlackWhite.Text = "Black && White";
            radioButtonBlackWhite.UseVisualStyleBackColor = true;
            // 
            // radioButtonGrayscale
            // 
            radioButtonGrayscale.AutoSize = true;
            radioButtonGrayscale.Checked = true;
            radioButtonGrayscale.Location = new Point(57, 56);
            radioButtonGrayscale.Name = "radioButtonGrayscale";
            radioButtonGrayscale.Size = new Size(75, 19);
            radioButtonGrayscale.TabIndex = 13;
            radioButtonGrayscale.TabStop = true;
            radioButtonGrayscale.Text = "Grayscale";
            radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // LabelMode
            // 
            LabelMode.AutoSize = true;
            LabelMode.Location = new Point(6, 58);
            LabelMode.Name = "LabelMode";
            LabelMode.Size = new Size(38, 15);
            LabelMode.TabIndex = 12;
            LabelMode.Text = "Mode";
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(16, 155);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(116, 23);
            buttonLoad.TabIndex = 11;
            buttonLoad.Text = "Load Images";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += this.buttonLoad_Click;
            // 
            // scanButton
            // 
            scanButton.Location = new Point(16, 109);
            scanButton.Name = "scanButton";
            scanButton.Size = new Size(75, 23);
            scanButton.TabIndex = 10;
            scanButton.Text = "Scan";
            scanButton.UseVisualStyleBackColor = true;
            scanButton.Click += this.scanButton_ClickAsync;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Location = new Point(115, 30);
            label1.MinimumSize = new Size(200, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 17);
            label1.TabIndex = 9;
            label1.Text = "c:\\test";
            // 
            // buttonOutputFolder
            // 
            buttonOutputFolder.Location = new Point(6, 26);
            buttonOutputFolder.Name = "buttonOutputFolder";
            buttonOutputFolder.Size = new Size(94, 23);
            buttonOutputFolder.TabIndex = 8;
            buttonOutputFolder.Text = "Output Folder";
            buttonOutputFolder.UseVisualStyleBackColor = true;
            buttonOutputFolder.Click += this.buttonOutputFolder_Click;
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
            buttonNext.Click += this.buttonNext_Click;
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
            buttonPrevious.Click += this.buttonPrevious_Click;
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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupScanControls.ResumeLayout(false);
            groupScanControls.PerformLayout();
            ResumeLayout(false);
        }


        #endregion
        private PictureBox pictureBox1;
        private FolderBrowserDialog folderBrowserDialog1;
        private ImageList imageList1;
        private GroupBox groupScanControls;
        private RadioButton radioButtonColor;
        private RadioButton radioButtonBlackWhite;
        private RadioButton radioButtonGrayscale;
        private Label LabelMode;
        private Button buttonLoad;
        private Button scanButton;
        private Label label1;
        private Button buttonOutputFolder;
        private Button buttonNext;
        private Button buttonPrevious;
    }
}
