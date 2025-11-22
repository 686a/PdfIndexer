namespace PDFIndexer
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
            this.QueryInputBox = new System.Windows.Forms.TextBox();
            this.IndexAllButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.FilenameLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.WebViewVirtualPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button4 = new System.Windows.Forms.Button();
            this.DetachButton = new System.Windows.Forms.Button();
            this.OpenInNewWindowButton = new System.Windows.Forms.Button();
            this.noFileLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DuplicateMangerButton = new PDFIndexer.IconTextButtonControl();
            this.WebViewVirtualPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // QueryInputBox
            // 
            this.QueryInputBox.Font = new System.Drawing.Font("Malgun Gothic", 10F);
            this.QueryInputBox.Location = new System.Drawing.Point(43, 15);
            this.QueryInputBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QueryInputBox.Name = "QueryInputBox";
            this.QueryInputBox.Size = new System.Drawing.Size(1194, 25);
            this.QueryInputBox.TabIndex = 2;
            this.QueryInputBox.TextChanged += new System.EventHandler(this.QueryInputBox_TextChanged);
            // 
            // IndexAllButton
            // 
            this.IndexAllButton.Location = new System.Drawing.Point(1107, 737);
            this.IndexAllButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.IndexAllButton.Name = "IndexAllButton";
            this.IndexAllButton.Size = new System.Drawing.Size(92, 32);
            this.IndexAllButton.TabIndex = 4;
            this.IndexAllButton.Text = "모두 인덱싱";
            this.IndexAllButton.UseVisualStyleBackColor = true;
            this.IndexAllButton.Click += new System.EventHandler(this.IndexAllButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(73, 6);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(247, 21);
            this.progressBar1.TabIndex = 5;
            // 
            // FilenameLabel
            // 
            this.FilenameLabel.AutoSize = true;
            this.FilenameLabel.Location = new System.Drawing.Point(358, 55);
            this.FilenameLabel.Name = "FilenameLabel";
            this.FilenameLabel.Size = new System.Drawing.Size(53, 15);
            this.FilenameLabel.TabIndex = 6;
            this.FilenameLabel.Text = "filename";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1000, 737);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 32);
            this.button2.TabIndex = 9;
            this.button2.Text = "Index Missing";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 21);
            this.label1.TabIndex = 10;
            this.label1.Text = "Not Ready";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WebViewVirtualPanel
            // 
            this.WebViewVirtualPanel.Controls.Add(this.label2);
            this.WebViewVirtualPanel.Location = new System.Drawing.Point(361, 85);
            this.WebViewVirtualPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.WebViewVirtualPanel.Name = "WebViewVirtualPanel";
            this.WebViewVirtualPanel.Size = new System.Drawing.Size(876, 646);
            this.WebViewVirtualPanel.TabIndex = 12;
            this.WebViewVirtualPanel.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(207, 30);
            this.label2.TabIndex = 0;
            this.label2.Text = "WebView Proxy\r\n이 부분에 PdfWebView가 표시됩니다";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 85);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(343, 646);
            this.flowLayoutPanel1.TabIndex = 13;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "검색 결과";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PDFIndexer.Properties.Resources.SearchIcon;
            this.pictureBox1.Location = new System.Drawing.Point(12, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(25, 25);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // button4
            // 
            this.button4.Image = global::PDFIndexer.Properties.Resources.SettingsIcon;
            this.button4.Location = new System.Drawing.Point(1205, 737);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(32, 32);
            this.button4.TabIndex = 20;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // DetachButton
            // 
            this.DetachButton.Image = global::PDFIndexer.Properties.Resources.OpenInNewIcon;
            this.DetachButton.Location = new System.Drawing.Point(1208, 49);
            this.DetachButton.Name = "DetachButton";
            this.DetachButton.Size = new System.Drawing.Size(29, 29);
            this.DetachButton.TabIndex = 19;
            this.DetachButton.UseVisualStyleBackColor = true;
            this.DetachButton.Click += new System.EventHandler(this.DetachButton_Click);
            // 
            // OpenInNewWindowButton
            // 
            this.OpenInNewWindowButton.Image = global::PDFIndexer.Properties.Resources.OpenInBrowserIcon;
            this.OpenInNewWindowButton.Location = new System.Drawing.Point(1173, 49);
            this.OpenInNewWindowButton.Name = "OpenInNewWindowButton";
            this.OpenInNewWindowButton.Size = new System.Drawing.Size(29, 29);
            this.OpenInNewWindowButton.TabIndex = 18;
            this.OpenInNewWindowButton.UseVisualStyleBackColor = true;
            // 
            // noFileLabel
            // 
            this.noFileLabel.AutoSize = true;
            this.noFileLabel.Location = new System.Drawing.Point(750, 350);
            this.noFileLabel.Name = "noFileLabel";
            this.noFileLabel.Size = new System.Drawing.Size(135, 15);
            this.noFileLabel.TabIndex = 23;
            this.noFileLabel.Text = "선택된 파일이 없습니다";
            this.noFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Location = new System.Drawing.Point(12, 738);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(334, 32);
            this.panel1.TabIndex = 24;
            // 
            // DuplicateMangerButton
            // 
            this.DuplicateMangerButton.Content = "중복 관리자";
            this.DuplicateMangerButton.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.DuplicateMangerButton.Icon = global::PDFIndexer.Properties.Resources.ContentCopyIcon;
            this.DuplicateMangerButton.IconSizeOffset = 0;
            this.DuplicateMangerButton.Location = new System.Drawing.Point(880, 737);
            this.DuplicateMangerButton.Name = "DuplicateMangerButton";
            this.DuplicateMangerButton.Size = new System.Drawing.Size(114, 32);
            this.DuplicateMangerButton.TabIndex = 22;
            this.DuplicateMangerButton.UseVisualStyleBackColor = true;
            this.DuplicateMangerButton.Click += new System.EventHandler(this.DuplicateMangerButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1249, 781);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.noFileLabel);
            this.Controls.Add(this.DuplicateMangerButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.DetachButton);
            this.Controls.Add(this.OpenInNewWindowButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.WebViewVirtualPanel);
            this.Controls.Add(this.FilenameLabel);
            this.Controls.Add(this.IndexAllButton);
            this.Controls.Add(this.QueryInputBox);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.WebViewVirtualPanel.ResumeLayout(false);
            this.WebViewVirtualPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox QueryInputBox;
        private System.Windows.Forms.Button IndexAllButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label FilenameLabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel WebViewVirtualPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OpenInNewWindowButton;
        private System.Windows.Forms.Button DetachButton;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private IconTextButtonControl DuplicateMangerButton;
        private System.Windows.Forms.Label noFileLabel;
        private System.Windows.Forms.Panel panel1;
    }
}

