namespace projectucp1
{
    partial class FormLogin
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
            this.BtnLogin = new System.Windows.Forms.Button();
            this.TxtBoxUsername = new System.Windows.Forms.TextBox();
            this.TxtBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LblWelcome = new System.Windows.Forms.Label();
            this.chkGunakanSqlInjection = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // BtnLogin
            // 
            this.BtnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnLogin.Location = new System.Drawing.Point(338, 349);
            this.BtnLogin.Name = "BtnLogin";
            this.BtnLogin.Size = new System.Drawing.Size(153, 48);
            this.BtnLogin.TabIndex = 0;
            this.BtnLogin.Text = "Login";
            this.BtnLogin.UseVisualStyleBackColor = true;
            this.BtnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // TxtBoxUsername
            // 
            this.TxtBoxUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBoxUsername.Location = new System.Drawing.Point(338, 154);
            this.TxtBoxUsername.Name = "TxtBoxUsername";
            this.TxtBoxUsername.Size = new System.Drawing.Size(226, 33);
            this.TxtBoxUsername.TabIndex = 1;
            // 
            // TxtBoxPassword
            // 
            this.TxtBoxPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBoxPassword.Location = new System.Drawing.Point(338, 217);
            this.TxtBoxPassword.Name = "TxtBoxPassword";
            this.TxtBoxPassword.PasswordChar = '*';
            this.TxtBoxPassword.Size = new System.Drawing.Size(226, 33);
            this.TxtBoxPassword.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(166, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Username";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(166, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 29);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password";
            // 
            // LblWelcome
            // 
            this.LblWelcome.AutoSize = true;
            this.LblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblWelcome.Location = new System.Drawing.Point(267, 69);
            this.LblWelcome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblWelcome.Name = "LblWelcome";
            this.LblWelcome.Size = new System.Drawing.Size(242, 37);
            this.LblWelcome.TabIndex = 5;
            this.LblWelcome.Text = "Silahkan Login";
            this.LblWelcome.Click += new System.EventHandler(this.LblWelcome_Click);
            // 
            // chkGunakanSqlInjection
            // 
            this.chkGunakanSqlInjection.AutoSize = true;
            this.chkGunakanSqlInjection.Location = new System.Drawing.Point(338, 288);
            this.chkGunakanSqlInjection.Name = "chkGunakanSqlInjection";
            this.chkGunakanSqlInjection.Size = new System.Drawing.Size(312, 24);
            this.chkGunakanSqlInjection.TabIndex = 7;
            this.chkGunakanSqlInjection.Text = "Mode Rentan (Gunakan SQL Injection)";
            this.chkGunakanSqlInjection.UseVisualStyleBackColor = true;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 449);
            this.Controls.Add(this.chkGunakanSqlInjection);
            this.Controls.Add(this.LblWelcome);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtBoxPassword);
            this.Controls.Add(this.TxtBoxUsername);
            this.Controls.Add(this.BtnLogin);
            this.Name = "FormLogin";
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnLogin;
        private System.Windows.Forms.TextBox TxtBoxUsername;
        private System.Windows.Forms.TextBox TxtBoxPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LblWelcome;
        private System.Windows.Forms.CheckBox chkGunakanSqlInjection;
    }
}

