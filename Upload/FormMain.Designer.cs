namespace Upload
{
    partial class FormMain
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
            this.txt_path = new System.Windows.Forms.TextBox();
            this.btn_open = new System.Windows.Forms.Button();
            this.pro = new System.Windows.Forms.ProgressBar();
            this.btn_upload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_path
            // 
            this.txt_path.Location = new System.Drawing.Point(12, 12);
            this.txt_path.Name = "txt_path";
            this.txt_path.Size = new System.Drawing.Size(429, 20);
            this.txt_path.TabIndex = 0;
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(445, 10);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(75, 23);
            this.btn_open.TabIndex = 1;
            this.btn_open.Text = "...";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // pro
            // 
            this.pro.Location = new System.Drawing.Point(12, 38);
            this.pro.Name = "pro";
            this.pro.Size = new System.Drawing.Size(429, 23);
            this.pro.TabIndex = 2;
            // 
            // btn_upload
            // 
            this.btn_upload.Location = new System.Drawing.Point(445, 38);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(75, 23);
            this.btn_upload.TabIndex = 3;
            this.btn_upload.Text = "上传";
            this.btn_upload.UseVisualStyleBackColor = true;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 78);
            this.Controls.Add(this.btn_upload);
            this.Controls.Add(this.pro);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.txt_path);
            this.Name = "FormMain";
            this.Text = "性别地址上传";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_path;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.ProgressBar pro;
        private System.Windows.Forms.Button btn_upload;
    }
}

