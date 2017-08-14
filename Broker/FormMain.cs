using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pipe;

namespace Broker
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private string beginTime;
        private void FormMain_Load(object sender, EventArgs ev)
        {
            beginTime = GetTime();
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() != DialogResult.OK)
                return;
            txt_path.Text = fbd.SelectedPath;
        }

        private int num;
        private Dictionary<string, bool> dic = new Dictionary<string, bool>();
        private void btn_begin_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(txt_path.Text, "*.txt");
            dic = new Dictionary<string, bool>();
            foreach (string file in files)
                dic.Add(file, false);
            int max = int.Parse(txt_num.Text);
            Console.WriteLine(max);
            Task.Factory.StartNew(() =>
            {
                foreach (string file in files)
                {
                    while (Interlocked.Exchange(ref num, num) > max)
                    {
                        Thread.Sleep(1000);
                    }

                    Worker_Task(file);
                }
            });
        }



        private void Worker_Task(string path)
        {
            Interlocked.Increment(ref num);
            Task.Factory.StartNew(() =>
            {
                Worker(path);
                Interlocked.Decrement(ref num);
            });
        }

        private void Worker(string path)
        {
            PipeServer pipeServer = new PipeServer("Worker.exe");
            pipeServer.Send("SYNC");
            pipeServer.Send(path);
            pipeServer.WaitForExit();
            dic[path] = true;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            #region Create Directory

            if (dic.Count == 0)
                return;

            string b = $"./{beginTime}-{GetTime()}";
            Directory.CreateDirectory(b);
            string finish = $"{b}/已完成";
            Directory.CreateDirectory(finish);
            string nofinish = $"{b}/未完成";
            Directory.CreateDirectory(nofinish);

            #endregion

            foreach (var d in dic)
            {
                File.Copy(d.Key, d.Value ? finish : nofinish);
            }

           
        }

        private string GetTime()
        {
            return $"{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}