using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MopData
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private SqliteHelper Sql { get; set; }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // test cookie is it effective
            if (!(new Mop("")).IsEffective)
            {
                MessageBox.Show("COOKIES无效");
                Application.Exit();
            }
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
                txt_load.Text = ofd.FileName;
        }

        private void btn_begin_Click(object sender, EventArgs e)
        {
            Sql = new SqliteHelper();
            progressBar1.Maximum = File.ReadAllLines(txt_load.Text).Length;
            
            Task.Factory.StartNew(() =>
            {
                int num = 0;
                foreach (var mobile in File.ReadAllLines(txt_load.Text))
                {
                    while (num>10)
                    {
                        Thread.Sleep(1000);
                    }
                    Invoke(new Action(() =>
                    {
                        num++;
                        lab_num.Text = num.ToString();
                    }));
                    Task.Factory.StartNew(() =>
                    {
                        var m = new Mop(mobile);
                        using (var cmd = new SQLiteCommand(Sql.Conn))
                        {
                            cmd.CommandText = $"SELECT * FROM MobileInfo WHERE 手机号={mobile}";
                            var reader = cmd.ExecuteReader();
                            if (!reader.HasRows)
                            {
                                using (var cmd1 = new SQLiteCommand(Sql.Conn))
                                {
                                    cmd1.CommandText = $"INSERT INTO MobileInfo (手机号) VALUES({mobile})";
                                    cmd1.ExecuteNonQuery();
                                }
                            }
                        }

                        string res = Encoding.GetEncoding("GBK").GetString(m.GetBaseInfo().RawBytes);
                        if (res.Contains("false"))
                            return;
                        var userInfo =
                                JsonConvert.DeserializeObject<UserBaseInfoJson.RootObject>(
                                    res);
                        using (var cmd = new SQLiteCommand(Sql.Conn))
                        {
                            //开打时间 出账类型 集团信息 最晚捆绑时间  
                            cmd.CommandText =
                                $"UPDATE MobileInfo SET " +
                                $"姓名='{userInfo.userBaseInfo.basicinfo[0].context}'," +
                                $"归属='{userInfo.userBaseInfo.basicinfo[3].context}'," +
                                $"开打时间='{userInfo.userBaseInfo.basicinfo[4].context}'," +
                                $"在用套餐='{userInfo.userBaseInfo.basicinfo[5].context}'," +
                                $"用户状态='{userInfo.userBaseInfo.basicinfo[6].context}'," +
                                $"出账类型='{userInfo.userBaseInfo.basicinfo[7].context}'," +
                                $"集团信息='{userInfo.userBaseInfo.basicinfo[11].context}'," +
                                $"最晚捆绑时间='{userInfo.userBaseInfo.basicinfo[14].context}'" +
                                $"  WHERE 手机号={mobile}";
                            cmd.ExecuteNonQuery();
                        }


                        var businessInfo = Encoding.GetEncoding("GBK").GetString(m.GetBusinessInfo().RawBytes);
                        var mc = Regex.Matches(businessInfo, "secondvalue\":\"(.*?)\"");
                        var info = new List<string>();
                        foreach (Match match in mc)
                            info.Add(match.Groups[1].ToString());
                        using (var cmd = new SQLiteCommand(Sql.Conn))
                        {
                            cmd.CommandText =
                                $"UPDATE MobileInfo SET 业务信息='{string.Join("\n", info.ToArray())}' WHERE 手机号={mobile}";
                            cmd.ExecuteNonQuery();
                        }
                        


                        var consumeInfo = Encoding.GetEncoding("GBK").GetString(m.GetConsumeInfo().RawBytes);
                        mc = Regex.Matches(consumeInfo, "secondvalue\":\"(.*?)\"");
                        string[] MONTH = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
                        foreach (Match match in mc)
                        {
                            var str = match.Groups[1].Value;
                            var m1 = Regex.Match(str, @"(\d+) 月消费/(\d+\.\d+)");
                            if (m1.Success)
                            {
                                var month = Convert.ToInt32(m1.Groups[1].Value);
                                using (var cmd = new SQLiteCommand(Sql.Conn))
                                {
                                    cmd.CommandText =
                                        $"UPDATE MobileInfo SET {MONTH[month - 1]}月消费={m1.Groups[2].Value} WHERE 手机号={mobile}";
                                    cmd.ExecuteNonQuery();
                                }

                                continue;
                            }
                            //secondvalue=201706/已使用优惠额度/2097.15 MB
                            m1 = Regex.Match(str, @"201\d(\d+)/已使用优惠额度.*?/(\d+\.\d+) ");
                            if (m1.Success)
                            {
                                var month = Convert.ToInt32(m1.Groups[1].Value);
                                using (var cmd = new SQLiteCommand(Sql.Conn))
                                {
                                    cmd.CommandText =
                                        $"UPDATE MobileInfo SET {MONTH[month - 1]}月流量={m1.Groups[2].Value} WHERE 手机号={mobile}";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }


                        var recommendInfo = Encoding.GetEncoding("GBK").GetString(m.GetRecomendInfo().RawBytes);
                        mc = Regex.Matches(recommendInfo, "prog_name\":\"(.*?)\"");
                        info = new List<string>();
                        foreach (Match match in mc)
                        {
                            var name = match.Groups[1].ToString();
                            if (!string.IsNullOrEmpty(name))
                                info.Add(name);
                        }
                        using (var cmd = new SQLiteCommand(Sql.Conn))
                        {
                            cmd.CommandText =
                                $"UPDATE MobileInfo SET 推荐信息='{string.Join("\n", info.ToArray())}' WHERE 手机号={mobile}";
                            cmd.ExecuteNonQuery();
                        }

                    }).ContinueWith((task) =>
                    {
                        Invoke(new Action(() =>
                        {
                            num--;
                            progressBar1.PerformStep();
                            lab_pro.Text = $@"{progressBar1.Value} / {progressBar1.Maximum}";
                            
                        }));
                    });
                }
            });
        }
    }
}