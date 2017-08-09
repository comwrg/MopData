using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MopData;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Tranfer;

namespace Worker
{
    class Program
    {
        
        static void Main(string[] args)
        {

            string path = @"C:\Users\Wrg\Desktop\mobile11111111\我的表格1.txt";
            if (args.Length > 0)
            {
                using (PipeStream pipeClient =
                    new AnonymousPipeClientStream(PipeDirection.In, args[0]))
                {
                    // Show that anonymous Pipes do not support Message mode.
                    try
                    {
//                        Console.WriteLine("[CLIENT] Setting ReadMode to \"Message\".");
                        pipeClient.ReadMode = PipeTransmissionMode.Byte;
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine("[CLIENT] Execption:\n    {0}", e.Message);
                    }

//                    Console.WriteLine("[CLIENT] Current TransmissionMode: {0}.",
//                        pipeClient.TransmissionMode);

                    using (StreamReader sr = new StreamReader(pipeClient))
                    {
                        // Display the read text to the console
                        string temp;

                        // Wait for 'sync message' from the server.
                        do
                        {
                            Console.WriteLine("[CLIENT] Wait for sync...");
                            temp = sr.ReadLine();
                        }
                        while (!temp.StartsWith("SYNC"));

                        // Read the server data and echo to the console.
                        while ((temp = sr.ReadLine()) != null)
                        {
                            Console.WriteLine("[CLIENT] Echo: " + temp);
                            path = temp;
                            Console.WriteLine(path);
                        }
                    }
                }
            }
//            Console.Write("[CLIENT] Press Enter to continue...");
//            Console.ReadLine();

//            for (int i = 0; i < 10; i++)
//            {
//                Console.WriteLine(i);
//                Thread.Sleep(100);
//            }
//            return;

            int num = 0;
            foreach (string mobile in File.ReadAllLines(path))
            {
                while (Interlocked.Exchange(ref num, num) > 50)
                {
//                    Console.WriteLine(1);
                    Thread.Sleep(1000);
                }

                Interlocked.Increment(ref num);
                Task.Factory.StartNew(() =>
                {
                    Begin(mobile);
                    Interlocked.Decrement(ref num);
                });
            }

            while (Interlocked.Exchange(ref num, num) != 0)
            {
                Thread.Sleep(5 * 1000);
            }
            
//            Console.ReadKey();
        }
        static string url = File.ReadAllText("url.txt");
        private static void Begin(string mobile)
        {
            Console.WriteLine(mobile);
            var m = new MopData.Mop(mobile, url);

            #region Base Info

            var res = Encoding.GetEncoding("GBK").GetString(m.GetBaseInfo().RawBytes);
            if (res.Contains("false"))
                return;
            var userInfo =
                JsonConvert.DeserializeObject<UserBaseInfoJson.RootObject>(
                    res);
            mysql.Execute($"INSERT INTO baseinfo (手机号) VALUES({mobile})");
            foreach (UserBaseInfoJson.Basicinfo basicinfo in userInfo.userBaseInfo.basicinfo)
            {
                mysql.Execute($"UPDATE baseinfo SET {basicinfo.title}='{basicinfo.context}'");
            }

            #endregion

            #region Business Info

            var businessInfo = Encoding.GetEncoding("GBK").GetString(m.GetBusinessInfo().RawBytes);
            var mc = Regex.Matches(businessInfo, "secondvalue\":\"(.*?)\"");
            List<string> list = new List<string>();
            foreach (Match match in mc)
                list.Add($"'{match.Groups[1]}'");
            mysql.Execute($"INSERT INTO businessinfo VALUES('{mobile}', {string.Join("\n", list.ToArray())})");
            #endregion

            #region Consume Info
            mysql.Execute($"INSERT INTO consumeinfo (手机号) VALUES('{mobile}')");
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
                    mysql.Execute($"UPDATE consumeinfo SET {MONTH[month - 1]}月消费={m1.Groups[2].Value} WHERE 手机号='{mobile}'");
                    //continue;
                }
                //secondvalue=201706/已使用优惠额度/2097.15 MB
                m1 = Regex.Match(str, @"201\d(\d+)/已使用优惠额度.*?/(\d+\.\d+) ");
                if (m1.Success)
                {
                    var month = Convert.ToInt32(m1.Groups[1].Value);
                    mysql.Execute($"UPDATE consumeinfo SET {MONTH[month - 1]}月流量={m1.Groups[2].Value} WHERE 手机号='{mobile}'");
                }
            }

            #endregion

            #region Recommend Info

            var recommendInfo = Encoding.GetEncoding("GBK").GetString(m.GetRecomendInfo().RawBytes);
            mc = Regex.Matches(recommendInfo, "prog_name\":\"(.*?)\"");
            list.Clear();
            foreach (Match match in mc)
            {
                var name = match.Groups[1].ToString();
                if (!string.IsNullOrEmpty(name))
                    list.Add(name);
            }
            mysql.Execute($"INSERT INTO recommendinfo VALUES('{mobile}', '{string.Join("\n", list.ToArray())}')");

            #endregion
        }
        private static MysqlHelper mysql = new MysqlHelper();
    }
}
