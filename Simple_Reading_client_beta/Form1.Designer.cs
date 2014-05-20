namespace Simple_Reading_client_beta
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbLink = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbTags = new System.Windows.Forms.Label();
            this.lbCat = new System.Windows.Forms.Label();
            this.tbNotes = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbText = new System.Windows.Forms.RichTextBox();
            this.plLogin = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btLogin = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.plLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(715, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbLink);
            this.panel2.Controls.Add(this.lbDate);
            this.panel2.Controls.Add(this.lbTags);
            this.panel2.Controls.Add(this.lbCat);
            this.panel2.Controls.Add(this.tbNotes);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(258, 416);
            this.panel2.TabIndex = 4;
            // 
            // lbLink
            // 
            this.lbLink.AutoSize = true;
            this.lbLink.Location = new System.Drawing.Point(26, 489);
            this.lbLink.Name = "lbLink";
            this.lbLink.Size = new System.Drawing.Size(46, 13);
            this.lbLink.TabIndex = 10;
            this.lbLink.Text = "Ссылка";
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Location = new System.Drawing.Point(27, 462);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(33, 13);
            this.lbDate.TabIndex = 9;
            this.lbDate.Text = "Дата";
            // 
            // lbTags
            // 
            this.lbTags.AutoSize = true;
            this.lbTags.Location = new System.Drawing.Point(27, 433);
            this.lbTags.Name = "lbTags";
            this.lbTags.Size = new System.Drawing.Size(31, 13);
            this.lbTags.TabIndex = 8;
            this.lbTags.Text = "Тэги";
            // 
            // lbCat
            // 
            this.lbCat.AutoSize = true;
            this.lbCat.Location = new System.Drawing.Point(26, 409);
            this.lbCat.Name = "lbCat";
            this.lbCat.Size = new System.Drawing.Size(66, 13);
            this.lbCat.TabIndex = 7;
            this.lbCat.Text = "Категории: ";
            // 
            // tbNotes
            // 
            this.tbNotes.Location = new System.Drawing.Point(0, 292);
            this.tbNotes.Name = "tbNotes";
            this.tbNotes.Size = new System.Drawing.Size(200, 96);
            this.tbNotes.TabIndex = 6;
            this.tbNotes.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(58, 263);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(258, 245);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 24);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(715, 560);
            this.webBrowser1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbText);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(114, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(691, 416);
            this.panel1.TabIndex = 7;
            // 
            // tbText
            // 
            this.tbText.Location = new System.Drawing.Point(378, 206);
            this.tbText.Name = "tbText";
            this.tbText.ReadOnly = true;
            this.tbText.Size = new System.Drawing.Size(100, 96);
            this.tbText.TabIndex = 5;
            this.tbText.Text = "";
            // 
            // plLogin
            // 
            this.plLogin.Controls.Add(this.label3);
            this.plLogin.Controls.Add(this.label2);
            this.plLogin.Controls.Add(this.btLogin);
            this.plLogin.Controls.Add(this.label1);
            this.plLogin.Controls.Add(this.tbPassword);
            this.plLogin.Controls.Add(this.tbLogin);
            this.plLogin.Location = new System.Drawing.Point(0, 144);
            this.plLogin.Name = "plLogin";
            this.plLogin.Size = new System.Drawing.Size(417, 440);
            this.plLogin.TabIndex = 11;
            this.plLogin.VisibleChanged += new System.EventHandler(this.plLogin_VisibleChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(249, 273);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Пароль";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(249, 220);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Логин";
            // 
            // btLogin
            // 
            this.btLogin.Location = new System.Drawing.Point(307, 368);
            this.btLogin.Name = "btLogin";
            this.btLogin.Size = new System.Drawing.Size(75, 23);
            this.btLogin.TabIndex = 3;
            this.btLogin.Text = "Войти";
            this.btLogin.UseVisualStyleBackColor = true;
            this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 2;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(307, 270);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(151, 20);
            this.tbPassword.TabIndex = 1;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(307, 214);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(151, 20);
            this.tbLogin.TabIndex = 0;
            // 
            // Form1
            // 
            this.AcceptButton = this.btLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 584);
            this.Controls.Add(this.plLogin);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.plLogin.ResumeLayout(false);
            this.plLogin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.RichTextBox tbNotes;
        private System.Windows.Forms.Label lbTags;
        private System.Windows.Forms.Label lbCat;
        private System.Windows.Forms.Label lbLink;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox tbText;
        private System.Windows.Forms.Panel plLogin;
        private System.Windows.Forms.Button btLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;

    }
}

