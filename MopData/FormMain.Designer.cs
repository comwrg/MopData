namespace MopData
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
            this.txt_load = new System.Windows.Forms.TextBox();
            this.btn_load = new System.Windows.Forms.Button();
            this.btn_begin = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lab_pro = new System.Windows.Forms.Label();
            this.lab_num = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_load
            // 
            this.txt_load.Location = new System.Drawing.Point(12, 12);
            this.txt_load.Name = "txt_load";
            this.txt_load.ReadOnly = true;
            this.txt_load.Size = new System.Drawing.Size(231, 20);
            this.txt_load.TabIndex = 0;
            // 
            // btn_load
            // 
            this.btn_load.Location = new System.Drawing.Point(249, 12);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(75, 23);
            this.btn_load.TabIndex = 1;
            this.btn_load.Text = "导入";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // btn_begin
            // 
            this.btn_begin.Location = new System.Drawing.Point(330, 12);
            this.btn_begin.Name = "btn_begin";
            this.btn_begin.Size = new System.Drawing.Size(75, 23);
            this.btn_begin.TabIndex = 2;
            this.btn_begin.Text = "开始";
            this.btn_begin.UseVisualStyleBackColor = true;
            this.btn_begin.Click += new System.EventHandler(this.btn_begin_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 56);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(393, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 3;
            // 
            // lab_pro
            // 
            this.lab_pro.AutoSize = true;
            this.lab_pro.Location = new System.Drawing.Point(184, 62);
            this.lab_pro.Name = "lab_pro";
            this.lab_pro.Size = new System.Drawing.Size(30, 13);
            this.lab_pro.TabIndex = 4;
            this.lab_pro.Text = "0 / 0";
            // 
            // lab_num
            // 
            this.lab_num.AutoSize = true;
            this.lab_num.Location = new System.Drawing.Point(411, 62);
            this.lab_num.Name = "lab_num";
            this.lab_num.Size = new System.Drawing.Size(13, 13);
            this.lab_num.TabIndex = 5;
            this.lab_num.Text = "0";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 95);
            this.Controls.Add(this.lab_num);
            this.Controls.Add(this.lab_pro);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_begin);
            this.Controls.Add(this.btn_load);
            this.Controls.Add(this.txt_load);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_load;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.Button btn_begin;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lab_pro;
        private System.Windows.Forms.Label lab_num;
    }
}

