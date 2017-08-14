using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MopData;
using Newtonsoft.Json;
using Pipe;

namespace Worker
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string path = @"C:\Users\Wrg\Desktop\mobile11111111\我的表格1.txt";

            if (args.Length > 0)
            {
                PipeClient pipeClient = new PipeClient(args[0]);
                string s = pipeClient.Receive();
                if (s != "SYNC")
                    return;
                path = pipeClient.Receive();
            }

            Directory.CreateDirectory("result");
            sw = new StreamWriter($"result/{Path.GetFileName(path)}", false, Encoding.Default) {AutoFlush = true};
            
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
        private static StreamWriter sw;
        private static bool first = false;
        private static void Begin(string mobile)
        {
            Console.WriteLine(mobile);
            var m = new MopData.Mop(mobile, url);
            List<string> temp = new List<string>();
            List<string> headers = new List<string>();
            List<string> items = new List<string>();

            headers.Add("手机号");
            items.Add(mobile);

            #region Base Info

            var res = Encoding.GetEncoding("GBK").GetString(m.GetBaseInfo().RawBytes);
            if (res.Contains("false"))
                return;
            var userInfo = JsonConvert.DeserializeObject<UserBaseInfoJson.RootObject>(res);
            int[] arr = {0, 3, 4, 5, 6, 7, 11, 14};
            foreach (int i in arr)
            {
                headers.Add(userInfo.userBaseInfo.basicinfo[i].title);
                items.Add(userInfo.userBaseInfo.basicinfo[i].context);
            }

            #endregion

            #region Business Info

            headers.Add("业务信息");
            var businessInfo = Encoding.GetEncoding("GBK").GetString(m.GetBusinessInfo().RawBytes);
            var mc = Regex.Matches(businessInfo, "secondvalue\":\"(.*?)\"");
            temp.Clear();
            foreach (Match match in mc)
                temp.Add($"'{match.Groups[1]}'");
            items.Add(string.Join(" ", temp.ToArray()));
            
            #endregion

            #region Consume Info
            
            var consumeInfo = Encoding.GetEncoding("GBK").GetString(m.GetConsumeInfo().RawBytes);
            string[] MONTH = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
            Match ma;

            headers.Add("余额");
            ma = Regex.Match(consumeInfo, @"当前余额/(\d+\.\d+)");
            items.Add(ma.Success ? ma.Groups[1].Value : "0");

            for (int i = 1; i < 13; i++)
            {
                ma = Regex.Match(consumeInfo, $"{i} 月消费/(.*?)\"");
                headers.Add($"{MONTH[i - 1]}月消费");
                items.Add(ma.Success ? ma.Groups[1].Value : "0");

                ma = Regex.Match(consumeInfo, $@"201\d0?{i}/已使用优惠额度.*?/(\d+\.\d+) ");
                headers.Add($"{MONTH[i - 1]}月流量");
                items.Add(ma.Success ? ma.Groups[1].Value : "0");
            }
            
            #endregion

            #region Recommend Info

            headers.Add("推荐信息");
            var recommendInfo = Encoding.GetEncoding("GBK").GetString(m.GetRecomendInfo().RawBytes);
            mc = Regex.Matches(recommendInfo, "prog_name\":\"(.*?)\"");
            temp.Clear();
            foreach (Match match in mc)
            {
                var name = match.Groups[1].ToString();
                if (!string.IsNullOrEmpty(name))
                    temp.Add(name);
            }
            items.Add(string.Join(" ", temp.ToArray()));

            headers.Add("终端类型");
            ma = Regex.Match(recommendInfo, "terminal_type\":\"(.*?)\"");
            items.Add(ma.Groups[1].Value);

            headers.Add("绑定终端");
            ma = Regex.Match(recommendInfo, "is_bound_terminal\":\"(.*?)\"");
            items.Add(ma.Groups[1].Value);

            headers.Add("换机时长(月)");
            ma = Regex.Match(recommendInfo, "terminal_change_time\":\"(.*?)\"");
            items.Add($"{ma.Groups[1].Value}");

            headers.Add("是否办理宽带");
            ma = Regex.Match(recommendInfo, "is_broad_band\":\"(.*?)\"");
            items.Add(ma.Groups[1].Value);

            #endregion

            if (!first)
            {
                sw.WriteLine(string.Join("\t", headers));
                first = true;
            }
            lock (sw)
            {
                sw.WriteLine(string.Join("\t", items));
                sw.Flush();
            }
            
        }
    }
}
