using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tranfer;

namespace Filter
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private string black = File.ReadAllText("./black.txt");
        private void btn_filter_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT * FROM baseinfo
            JOIN businessinfo  ON baseinfo.手机号 = businessinfo.手机号
            JOIN consumeinfo   ON baseinfo.手机号 = consumeinfo.手机号
            JOIN recommendinfo ON baseinfo.手机号 = recommendinfo.手机号";
            List<string> list = new List<string>();
            if (txt_attribution.Text != string.Empty)
            {
                list.Add($"归属='{txt_attribution.Text}'");
            }
            if (txt_package.Text != string.Empty)
            {
                list.Add($"在用套餐='{txt_package.Text}'");
            }
            if (txt_userStatus.Text != string.Empty)
            {
                list.Add($"用户状态='{txt_userStatus.Text}'");
            }
            if (txt_beginTime.Text != string.Empty)
            {

            }
            if (txt_billingType.Text != string.Empty)
            {
                list.Add($"出账类型='{txt_billingType.Text}'");
            }
            if (txt_userInfo.Text != string.Empty)
            {
                list.Add($"集团信息='{txt_userInfo.Text}'");
            }
            if (txt_latestBindingTime.Text != string.Empty)
            {
                list.Add($"最晚捆绑时间<'{txt_latestBindingTime.Text}'");
            }
            if (txt_businessInfo.Text != string.Empty)
            {
                list.Add($"业务信息='{txt_businessInfo.Text}'");
            }
            if (txt_recommendInfo.Text != string.Empty)
            {
                string[] arr = txt_recommendInfo.Text.Split(';');
                foreach (string s in arr)
                {
                    list.Add($"推荐信息 LIKE '%{s}%'");
                }
            }

            if (txt_billing.Text != string.Empty)
            {
                list.AddRange(Tranfer(txt_billing.Text, "消费"));
            }
            if (txt_flow.Text != string.Empty)
            {
                list.AddRange(Tranfer(txt_flow.Text, "流量"));
            }

            sql += $" WHERE {string.Join(" AND ", list.ToArray())}";
            Console.WriteLine(sql);

            MysqlHelper mysql = new MysqlHelper();
            var cmd = mysql.Conn.CreateCommand();
            cmd.CommandText = sql;
            var read = cmd.ExecuteReader();
            int column = 1;
            //            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            //            xlApp.Visible = true;
            //            Workbook wb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            //            Worksheet ws = wb.Worksheets[1];


            using (StreamWriter sw = new StreamWriter($"./{DateTime.Now:yyyyMMdd.hh.mm.ss}.txt", true, Encoding.Default))
            {
                sw.WriteLine($"手机号\t姓名\t归属\t在用套餐\t用户状态\t开打时间\t出账类型\t集团信息\t最晚捆绑时间\t业务信息\t一月消费\t一月流量\t二月消费\t二月流量\t三月消费\t三月流量\t四月消费\t四月流量\t五月消费\t五月流量\t六月消费\t六月流量\t七月消费\t七月流量\t八月消费\t八月流量\t九月消费\t九月流量\t十月消费\t十月流量\t十一月消费\t十一月流量\t十二月消费\t十二月流量\t推荐信息");
                while (read.Read())
                {
                    list.Clear();
                    string mobile = string.Empty;
                    if (black.Contains(mobile))
                        continue;
                    for (int i = 0; i < read.FieldCount; i++)
                    {
                        string str = read[i].ToString();
                        if (i == 0)
                        {
                            mobile = str;
                        }
                        if (str != mobile || i == 0)
                        {
                            list.Add(str.Replace('\n', ' '));
                        }
                    }
                    sw.WriteLine(string.Join("\t", list.ToArray()));
                }
            }
        }

        private List<string> Tranfer(string txt, string suffix)
        {
            var mc = Regex.Matches(txt, "(\\d{1,2})-(\\d+)([\\+-])");
            string[] MONTH = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
            List<string> list = new List<string>();
            foreach (Match m in mc)
            {
                int month = int.Parse(m.Groups[1].Value);
                string value = m.Groups[2].Value;
                string flag = m.Groups[3].Value;

                string sMonth = MONTH[month - 1];
                string sFlag = null;
                if (flag == "+")
                    sFlag = ">";
                else if (flag == "-")
                    sFlag = "<";

                 list.Add($"{sMonth}月{suffix}{sFlag}{value}");
            }
            return list;
        }
    }


}
