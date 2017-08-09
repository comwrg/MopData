using System;
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
            Task.Factory.StartNew(() =>
            {
                var mysql = new MysqlHelper();
                foreach (var line in lines)
                {
                    var arr = line.Split('\t');
                    if (arr.Length != 4)
                        continue;
                    for (var i = 0; i < arr.Length; i++)
                        arr[i] = $"'{arr[i]}'";
                    mysql.Execute($"INSERT INTO localinfo VALUES({string.Join(",", arr)})");
                    Invoke(new Action(() => { pro.Value++; }));
                }
            });
        }
    }
}