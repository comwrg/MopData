using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tranfer;

namespace Upload
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = @"文本文件|*.txt";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            txt_path.Text = ofd.FileName;
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {
            var lines = File.ReadAllLines(txt_path.Text, Encoding.Default);
            pro.Maximum = lines.Length;
            pro.Value = 0;
            string str = string.Empty;
            if (rb_sex.Checked)
                str = "性别";
            else if (rb_address.Checked)
                str = "地址";
            else if (rb_model.Checked)
                str = "机型";
            else
                return;
            Task.Factory.StartNew(() =>
            {
                var mysql = new MysqlHelper();
                foreach (var line in lines)
                {
                    var arr = line.Split('\t');
                    if (arr.Length != 2)
                        continue;

                    mysql.Execute($"INSERT INTO localinfo (手机号) VALUES('{arr[0]}')");
                    mysql.Execute($"UPDATE localinfo SET {str}='{arr[1]}'");
                    Invoke(new Action(() => { pro.Value++; }));
                }
            });
        }
    }
}