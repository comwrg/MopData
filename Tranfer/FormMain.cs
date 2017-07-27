using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Tranfer
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }
        MysqlHelper mysql = new MysqlHelper();
        private void btn_run_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles("./sqlite", "*.sqlite");
            foreach (string file in files)
            {
                Task.Factory.StartNew(() =>
                {
                    SqliteHelper sqlite = new SqliteHelper(file);
                    using (var cmdSqlite = sqlite.Conn.CreateCommand())
                    {
                        
                        
                        Invoke(new Action(() =>
                        {
                            cmdSqlite.CommandText = "SELECT count(*) FROM MobileINfo";
                            var r = cmdSqlite.ExecuteReader();
                            r.Read();
                            progressBar1.Maximum += Convert.ToInt32(r[0]);
                            r.Close();
                        }));

                        cmdSqlite.CommandText = "SELECT * FROM MobileInfo";
                        var reader = cmdSqlite.ExecuteReader();
                        while (reader.Read())
                        {
                            List<string> list = new List<string>();
                            var mobile = reader.GetString(0);
                            list.Add(mobile);


                            // 1 - 8
                            for (int i = 1; i < 9; i++)
                            {
                                list.Add(reader.IsDBNull(i) ? "''" : $"'{reader.GetString(i)}'");
                            }
                            Begin($"INSERT INTO baseinfo VALUES({string.Join(",", list.ToArray())})");

                            list.Clear();
                            list.Add(mobile);
                            list.Add(reader.IsDBNull(9) ? "''" : $"'{reader.GetString(9)}'");
                            Begin($"INSERT INTO businessinfo VALUES({string.Join(",", list.ToArray())})");

                            list.Clear();
                            list.Add(mobile);
                            for (int i = 10; i < 34; i++)
                            {
                                var val = reader.IsDBNull(i) ? 0 : reader.GetDouble(i);
                                list.Add(val.ToString(CultureInfo.InvariantCulture));
                            }
                            Begin($"INSERT INTO consumeinfo VALUES({string.Join(",", list.ToArray())})");

                            list.Clear();
                            list.Add(mobile);
                            list.Add(reader.IsDBNull(34) ? "''" : $"'{reader.GetString(34)}'");
                            Begin($"INSERT INTO recommendinfo VALUES({string.Join(",", list.ToArray())})");

                            Invoke(new Action(() =>
                            {
                                progressBar1.Value++;
                                label1.Text = $@"{progressBar1.Value} / {progressBar1.Maximum}";
                            }));
                        }
                    }

                });
                
            }


            

        }
        
        private void Begin(string s)
        {
            using (var cmdMysql = mysql.Conn.CreateCommand())
            {
                cmdMysql.CommandText = s;
                try
                {
                    cmdMysql.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}