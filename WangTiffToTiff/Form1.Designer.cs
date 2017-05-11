namespace WangTiffToTiff
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btBrowseOutputFile = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.tbOutputFilePath = new System.Windows.Forms.TextBox();
            this.btBrowseInputFile = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.tbInputFilePath = new System.Windows.Forms.TextBox();
            this.btConvertFile = new System.Windows.Forms.Button();
            this.singleFileConversionGroupBox = new System.Windows.Forms.GroupBox();
            this.btConvertFolder = new System.Windows.Forms.Button();
            this.btBrowseDstFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbOutputFolderPath = new System.Windows.Forms.TextBox();
            this.btBrowsSrcFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbInputFolderPath = new System.Windows.Forms.TextBox();
            this.multipleFileConversionGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBoxTextOutput = new System.Windows.Forms.GroupBox();
            this.rbTestTextOutput = new System.Windows.Forms.RadioButton();
            this.rbReferenceTextOutput = new System.Windows.Forms.RadioButton();
            this.rbNoTextOutput = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.singleFileConversionGroupBox.SuspendLayout();
            this.multipleFileConversionGroupBox.SuspendLayout();
            this.groupBoxTextOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // btBrowseOutputFile
            // 
            this.btBrowseOutputFile.Location = new System.Drawing.Point(834, 69);
            this.btBrowseOutputFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btBrowseOutputFile.Name = "btBrowseOutputFile";
            this.btBrowseOutputFile.Size = new System.Drawing.Size(52, 31);
            this.btBrowseOutputFile.TabIndex = 46;
            this.btBrowseOutputFile.Text = "...";
            this.btBrowseOutputFile.UseVisualStyleBackColor = true;
            this.btBrowseOutputFile.Click += new System.EventHandler(this.btBrowseOutputFile_Click);
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(15, 65);
            this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(86, 20);
            this.Label4.TabIndex = 45;
            this.Label4.Text = "Output file:";
            // 
            // tbOutputFilePath
            // 
            this.tbOutputFilePath.Location = new System.Drawing.Point(162, 69);
            this.tbOutputFilePath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbOutputFilePath.Name = "tbOutputFilePath";
            this.tbOutputFilePath.ReadOnly = true;
            this.tbOutputFilePath.Size = new System.Drawing.Size(661, 26);
            this.tbOutputFilePath.TabIndex = 44;
            // 
            // btBrowseInputFile
            // 
            this.btBrowseInputFile.Location = new System.Drawing.Point(834, 29);
            this.btBrowseInputFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btBrowseInputFile.Name = "btBrowseInputFile";
            this.btBrowseInputFile.Size = new System.Drawing.Size(52, 31);
            this.btBrowseInputFile.TabIndex = 43;
            this.btBrowseInputFile.Text = "...";
            this.btBrowseInputFile.UseVisualStyleBackColor = true;
            this.btBrowseInputFile.Click += new System.EventHandler(this.btBrowseInputFile_Click);
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(15, 25);
            this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(74, 20);
            this.Label3.TabIndex = 42;
            this.Label3.Text = "Input file:";
            // 
            // tbInputFilePath
            // 
            this.tbInputFilePath.Location = new System.Drawing.Point(162, 29);
            this.tbInputFilePath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbInputFilePath.Name = "tbInputFilePath";
            this.tbInputFilePath.ReadOnly = true;
            this.tbInputFilePath.Size = new System.Drawing.Size(661, 26);
            this.tbInputFilePath.TabIndex = 41;
            // 
            // btConvertFile
            // 
            this.btConvertFile.Location = new System.Drawing.Point(396, 109);
            this.btConvertFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btConvertFile.Name = "btConvertFile";
            this.btConvertFile.Size = new System.Drawing.Size(218, 51);
            this.btConvertFile.TabIndex = 47;
            this.btConvertFile.Text = "Convert file!";
            this.btConvertFile.UseVisualStyleBackColor = true;
            this.btConvertFile.Click += new System.EventHandler(this.btConvertFile_Click);
            // 
            // singleFileConversionGroupBox
            // 
            this.singleFileConversionGroupBox.Controls.Add(this.Label3);
            this.singleFileConversionGroupBox.Controls.Add(this.Label4);
            this.singleFileConversionGroupBox.Controls.Add(this.btConvertFile);
            this.singleFileConversionGroupBox.Controls.Add(this.tbInputFilePath);
            this.singleFileConversionGroupBox.Controls.Add(this.btBrowseOutputFile);
            this.singleFileConversionGroupBox.Controls.Add(this.tbOutputFilePath);
            this.singleFileConversionGroupBox.Controls.Add(this.btBrowseInputFile);
            this.singleFileConversionGroupBox.Location = new System.Drawing.Point(32, 42);
            this.singleFileConversionGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.singleFileConversionGroupBox.Name = "singleFileConversionGroupBox";
            this.singleFileConversionGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.singleFileConversionGroupBox.Size = new System.Drawing.Size(951, 177);
            this.singleFileConversionGroupBox.TabIndex = 48;
            this.singleFileConversionGroupBox.TabStop = false;
            this.singleFileConversionGroupBox.Text = "Single File Conversion";
            // 
            // btConvertFolder
            // 
            this.btConvertFolder.Location = new System.Drawing.Point(396, 135);
            this.btConvertFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btConvertFolder.Name = "btConvertFolder";
            this.btConvertFolder.Size = new System.Drawing.Size(218, 51);
            this.btConvertFolder.TabIndex = 55;
            this.btConvertFolder.Text = "Convert folder !";
            this.btConvertFolder.UseVisualStyleBackColor = true;
            this.btConvertFolder.Click += new System.EventHandler(this.btConvertFolder_Click);
            // 
            // btBrowseDstFolder
            // 
            this.btBrowseDstFolder.Location = new System.Drawing.Point(834, 95);
            this.btBrowseDstFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btBrowseDstFolder.Name = "btBrowseDstFolder";
            this.btBrowseDstFolder.Size = new System.Drawing.Size(52, 31);
            this.btBrowseDstFolder.TabIndex = 54;
            this.btBrowseDstFolder.Text = "...";
            this.btBrowseDstFolder.UseVisualStyleBackColor = true;
            this.btBrowseDstFolder.Click += new System.EventHandler(this.btBrowseOutputFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 95);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.TabIndex = 53;
            this.label1.Text = "Output folder:";
            // 
            // tbOutputFolderPath
            // 
            this.tbOutputFolderPath.Location = new System.Drawing.Point(162, 95);
            this.tbOutputFolderPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbOutputFolderPath.Name = "tbOutputFolderPath";
            this.tbOutputFolderPath.ReadOnly = true;
            this.tbOutputFolderPath.Size = new System.Drawing.Size(661, 26);
            this.tbOutputFolderPath.TabIndex = 52;
            // 
            // btBrowsSrcFolder
            // 
            this.btBrowsSrcFolder.Location = new System.Drawing.Point(834, 48);
            this.btBrowsSrcFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btBrowsSrcFolder.Name = "btBrowsSrcFolder";
            this.btBrowsSrcFolder.Size = new System.Drawing.Size(52, 31);
            this.btBrowsSrcFolder.TabIndex = 51;
            this.btBrowsSrcFolder.Text = "...";
            this.btBrowsSrcFolder.UseVisualStyleBackColor = true;
            this.btBrowsSrcFolder.Click += new System.EventHandler(this.btBrowseInputFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 20);
            this.label2.TabIndex = 50;
            this.label2.Text = "Input folder:";
            // 
            // tbInputFolderPath
            // 
            this.tbInputFolderPath.Location = new System.Drawing.Point(162, 48);
            this.tbInputFolderPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbInputFolderPath.Name = "tbInputFolderPath";
            this.tbInputFolderPath.ReadOnly = true;
            this.tbInputFolderPath.Size = new System.Drawing.Size(661, 26);
            this.tbInputFolderPath.TabIndex = 49;
            // 
            // multipleFileConversionGroupBox
            // 
            this.multipleFileConversionGroupBox.Controls.Add(this.groupBoxTextOutput);
            this.multipleFileConversionGroupBox.Controls.Add(this.btConvertFolder);
            this.multipleFileConversionGroupBox.Controls.Add(this.label2);
            this.multipleFileConversionGroupBox.Controls.Add(this.btBrowseDstFolder);
            this.multipleFileConversionGroupBox.Controls.Add(this.tbInputFolderPath);
            this.multipleFileConversionGroupBox.Controls.Add(this.btBrowsSrcFolder);
            this.multipleFileConversionGroupBox.Controls.Add(this.tbOutputFolderPath);
            this.multipleFileConversionGroupBox.Controls.Add(this.label1);
            this.multipleFileConversionGroupBox.Location = new System.Drawing.Point(33, 228);
            this.multipleFileConversionGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.multipleFileConversionGroupBox.Name = "multipleFileConversionGroupBox";
            this.multipleFileConversionGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.multipleFileConversionGroupBox.Size = new System.Drawing.Size(951, 328);
            this.multipleFileConversionGroupBox.TabIndex = 56;
            this.multipleFileConversionGroupBox.TabStop = false;
            this.multipleFileConversionGroupBox.Text = "Multiple File Conversion";
            // 
            // groupBoxTextOutput
            // 
            this.groupBoxTextOutput.Controls.Add(this.rbTestTextOutput);
            this.groupBoxTextOutput.Controls.Add(this.rbReferenceTextOutput);
            this.groupBoxTextOutput.Controls.Add(this.rbNoTextOutput);
            this.groupBoxTextOutput.Location = new System.Drawing.Point(20, 195);
            this.groupBoxTextOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxTextOutput.Name = "groupBoxTextOutput";
            this.groupBoxTextOutput.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBoxTextOutput.Size = new System.Drawing.Size(867, 111);
            this.groupBoxTextOutput.TabIndex = 56;
            this.groupBoxTextOutput.TabStop = false;
            this.groupBoxTextOutput.Text = "Text Output";
            // 
            // rbTestTextOutput
            // 
            this.rbTestTextOutput.AutoSize = true;
            this.rbTestTextOutput.Location = new System.Drawing.Point(336, 29);
            this.rbTestTextOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbTestTextOutput.Name = "rbTestTextOutput";
            this.rbTestTextOutput.Size = new System.Drawing.Size(65, 24);
            this.rbTestTextOutput.TabIndex = 58;
            this.rbTestTextOutput.TabStop = true;
            this.rbTestTextOutput.Text = "Test";
            this.rbTestTextOutput.UseVisualStyleBackColor = true;
            this.rbTestTextOutput.CheckedChanged += new System.EventHandler(this.radioButtonTestTextOutput_CheckedChanged);
            // 
            // rbReferenceTextOutput
            // 
            this.rbReferenceTextOutput.AutoSize = true;
            this.rbReferenceTextOutput.Location = new System.Drawing.Point(20, 65);
            this.rbReferenceTextOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbReferenceTextOutput.Name = "rbReferenceTextOutput";
            this.rbReferenceTextOutput.Size = new System.Drawing.Size(109, 24);
            this.rbReferenceTextOutput.TabIndex = 59;
            this.rbReferenceTextOutput.TabStop = true;
            this.rbReferenceTextOutput.Text = "Reference";
            this.rbReferenceTextOutput.UseVisualStyleBackColor = true;
            this.rbReferenceTextOutput.CheckedChanged += new System.EventHandler(this.radioButtonReferenceTextOutput_CheckedChanged);
            // 
            // rbNoTextOutput
            // 
            this.rbNoTextOutput.AutoSize = true;
            this.rbNoTextOutput.Location = new System.Drawing.Point(20, 31);
            this.rbNoTextOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbNoTextOutput.Name = "rbNoTextOutput";
            this.rbNoTextOutput.Size = new System.Drawing.Size(72, 24);
            this.rbNoTextOutput.TabIndex = 57;
            this.rbNoTextOutput.TabStop = true;
            this.rbNoTextOutput.Text = "None";
            this.rbNoTextOutput.UseVisualStyleBackColor = true;
            this.rbNoTextOutput.CheckedChanged += new System.EventHandler(this.radioButtonNoTextOutput_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip1.TabIndex = 58;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 577);
            this.Controls.Add(this.multipleFileConversionGroupBox);
            this.Controls.Add(this.singleFileConversionGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.singleFileConversionGroupBox.ResumeLayout(false);
            this.singleFileConversionGroupBox.PerformLayout();
            this.multipleFileConversionGroupBox.ResumeLayout(false);
            this.multipleFileConversionGroupBox.PerformLayout();
            this.groupBoxTextOutput.ResumeLayout(false);
            this.groupBoxTextOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btBrowseOutputFile;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TextBox tbOutputFilePath;
        internal System.Windows.Forms.Button btBrowseInputFile;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox tbInputFilePath;
        internal System.Windows.Forms.Button btConvertFile;
        private System.Windows.Forms.GroupBox singleFileConversionGroupBox;
        internal System.Windows.Forms.Button btConvertFolder;
        internal System.Windows.Forms.Button btBrowseDstFolder;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox tbOutputFolderPath;
        internal System.Windows.Forms.Button btBrowsSrcFolder;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox tbInputFolderPath;
        private System.Windows.Forms.GroupBox multipleFileConversionGroupBox;
        private System.Windows.Forms.GroupBox groupBoxTextOutput;
        private System.Windows.Forms.RadioButton rbTestTextOutput;
        private System.Windows.Forms.RadioButton rbReferenceTextOutput;
        private System.Windows.Forms.RadioButton rbNoTextOutput;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}

