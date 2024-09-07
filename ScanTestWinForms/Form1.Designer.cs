namespace WinFormsApp1
{
    partial class Form1
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
            scanButton = new Button();
            pictureBox1 = new PictureBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            button1 = new Button();
            label1 = new Label();
            imageList1 = new ImageList(components);
            buttonPrevious = new Button();
            buttonNext = new Button();
            buttonLoad = new Button();
            LabelMode = new Label();
            radioButtonGrayscale = new RadioButton();
            radioButtonBlackWhite = new RadioButton();
            radioButtonColor = new RadioButton();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // scanButton
            // 
            scanButton.Location = new Point(16, 125);
            scanButton.Name = "scanButton";
            scanButton.Size = new Size(75, 23);
            scanButton.TabIndex = 0;
            scanButton.Text = "Scan";
            scanButton.UseVisualStyleBackColor = true;
            scanButton.Click += scanButton_ClickAsync;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Dock = DockStyle.Right;
            pictureBox1.Location = new Point(424, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(589, 823);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(16, 50);
            button1.Name = "button1";
            button1.Size = new Size(94, 23);
            button1.TabIndex = 2;
            button1.Text = "Output Folder";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Location = new Point(125, 54);
            label1.MinimumSize = new Size(200, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 17);
            label1.TabIndex = 3;
            label1.Text = "c:\\test";
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // buttonPrevious
            // 
            buttonPrevious.Location = new Point(262, 670);
            buttonPrevious.Name = "buttonPrevious";
            buttonPrevious.Size = new Size(75, 23);
            buttonPrevious.TabIndex = 4;
            buttonPrevious.Text = "Previous";
            buttonPrevious.UseVisualStyleBackColor = true;
            buttonPrevious.Click += buttonPrevious_Click;
            // 
            // buttonNext
            // 
            buttonNext.Location = new Point(343, 670);
            buttonNext.Name = "buttonNext";
            buttonNext.Size = new Size(75, 23);
            buttonNext.TabIndex = 5;
            buttonNext.Text = "Next";
            buttonNext.UseVisualStyleBackColor = true;
            buttonNext.Click += buttonNext_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(16, 171);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(116, 23);
            buttonLoad.TabIndex = 6;
            buttonLoad.Text = "Load Images";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += button2_Click;
            // 
            // LabelMode
            // 
            LabelMode.AutoSize = true;
            LabelMode.Location = new Point(21, 88);
            LabelMode.Name = "LabelMode";
            LabelMode.Size = new Size(38, 15);
            LabelMode.TabIndex = 7;
            LabelMode.Text = "Mode";
            // 
            // radioButtonGrayscale
            // 
            radioButtonGrayscale.AutoSize = true;
            radioButtonGrayscale.Checked = true;
            radioButtonGrayscale.Location = new Point(126, 88);
            radioButtonGrayscale.Name = "radioButtonGrayscale";
            radioButtonGrayscale.Size = new Size(75, 19);
            radioButtonGrayscale.TabIndex = 8;
            radioButtonGrayscale.TabStop = true;
            radioButtonGrayscale.Text = "Grayscale";
            radioButtonGrayscale.UseVisualStyleBackColor = true;
            // 
            // radioButtonBlackWhite
            // 
            radioButtonBlackWhite.AutoSize = true;
            radioButtonBlackWhite.Location = new Point(207, 88);
            radioButtonBlackWhite.Name = "radioButtonBlackWhite";
            radioButtonBlackWhite.Size = new Size(100, 19);
            radioButtonBlackWhite.TabIndex = 9;
            radioButtonBlackWhite.Text = "Black && White";
            radioButtonBlackWhite.UseVisualStyleBackColor = true;
            // 
            // radioButtonColor
            // 
            radioButtonColor.AutoSize = true;
            radioButtonColor.Location = new Point(313, 88);
            radioButtonColor.Name = "radioButtonColor";
            radioButtonColor.Size = new Size(54, 19);
            radioButtonColor.TabIndex = 10;
            radioButtonColor.Text = "Color";
            radioButtonColor.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1013, 823);
            Controls.Add(radioButtonColor);
            Controls.Add(radioButtonBlackWhite);
            Controls.Add(radioButtonGrayscale);
            Controls.Add(LabelMode);
            Controls.Add(buttonLoad);
            Controls.Add(buttonNext);
            Controls.Add(buttonPrevious);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Controls.Add(scanButton);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button scanButton;
        private PictureBox pictureBox1;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button1;
        private Label label1;
        private ImageList imageList1;
        private Button buttonPrevious;
        private Button buttonNext;
        private Button buttonLoad;
        private Label LabelMode;
        private RadioButton radioButtonGrayscale;
        private RadioButton radioButtonBlackWhite;
        private RadioButton radioButtonColor;
    }
}
