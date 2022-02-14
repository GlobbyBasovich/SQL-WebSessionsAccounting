namespace WebSessionsAccounting
{
    partial class FormBrowser
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
            this.buttonGo = new System.Windows.Forms.Button();
            this.comboBoxUrl = new System.Windows.Forms.ComboBox();
            this.comboBoxBrowser = new System.Windows.Forms.ComboBox();
            this.labelBrowser = new System.Windows.Forms.Label();
            this.labelOS = new System.Windows.Forms.Label();
            this.comboBoxOS = new System.Windows.Forms.ComboBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.webBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonGo.Location = new System.Drawing.Point(297, 37);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(75, 23);
            this.buttonGo.TabIndex = 1;
            this.buttonGo.Text = "Посетить";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // comboBoxUrl
            // 
            this.comboBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxUrl.Location = new System.Drawing.Point(47, 39);
            this.comboBoxUrl.Name = "comboBoxUrl";
            this.comboBoxUrl.Size = new System.Drawing.Size(244, 20);
            this.comboBoxUrl.TabIndex = 0;
            this.comboBoxUrl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxUrl_KeyUp);
            // 
            // comboBoxBrowser
            // 
            this.comboBoxBrowser.FormattingEnabled = true;
            this.comboBoxBrowser.Location = new System.Drawing.Point(252, 12);
            this.comboBoxBrowser.Name = "comboBoxBrowser";
            this.comboBoxBrowser.Size = new System.Drawing.Size(120, 21);
            this.comboBoxBrowser.TabIndex = 3;
            // 
            // labelBrowser
            // 
            this.labelBrowser.AutoSize = true;
            this.labelBrowser.Location = new System.Drawing.Point(197, 15);
            this.labelBrowser.Name = "labelBrowser";
            this.labelBrowser.Size = new System.Drawing.Size(49, 13);
            this.labelBrowser.TabIndex = 3;
            this.labelBrowser.Text = "Браузер";
            // 
            // labelOS
            // 
            this.labelOS.AutoSize = true;
            this.labelOS.Location = new System.Drawing.Point(19, 15);
            this.labelOS.Name = "labelOS";
            this.labelOS.Size = new System.Drawing.Size(22, 13);
            this.labelOS.TabIndex = 4;
            this.labelOS.Text = "ОС";
            // 
            // comboBoxOS
            // 
            this.comboBoxOS.FormattingEnabled = true;
            this.comboBoxOS.Location = new System.Drawing.Point(47, 12);
            this.comboBoxOS.Name = "comboBoxOS";
            this.comboBoxOS.Size = new System.Drawing.Size(120, 21);
            this.comboBoxOS.TabIndex = 2;
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(12, 42);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(29, 13);
            this.labelUrl.TabIndex = 7;
            this.labelUrl.Text = "URL";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelUrl);
            this.panel1.Controls.Add(this.labelOS);
            this.panel1.Controls.Add(this.comboBoxOS);
            this.panel1.Controls.Add(this.buttonGo);
            this.panel1.Controls.Add(this.comboBoxUrl);
            this.panel1.Controls.Add(this.labelBrowser);
            this.panel1.Controls.Add(this.comboBoxBrowser);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 71);
            this.panel1.TabIndex = 0;
            // 
            // webBrowser
            // 
            this.webBrowser.ActivateBrowserOnCreation = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 71);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(384, 240);
            this.webBrowser.TabIndex = 1;
            this.webBrowser.AddressChanged += new System.EventHandler<CefSharp.AddressChangedEventArgs>(this.webBrowser_AddressChanged);
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 311);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(400, 350);
            this.Name = "FormBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Просмотр веб-страниц";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBrowser_FormClosing);
            this.Resize += new System.EventHandler(this.FormBrowser_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.ComboBox comboBoxUrl;
        private System.Windows.Forms.ComboBox comboBoxBrowser;
        private System.Windows.Forms.Label labelBrowser;
        private System.Windows.Forms.Label labelOS;
        private System.Windows.Forms.ComboBox comboBoxOS;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Panel panel1;
        private CefSharp.WinForms.ChromiumWebBrowser webBrowser;
    }
}