using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Broker
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs ev)
        {
            
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() != DialogResult.OK)
                return;
            txt_path.Text = fbd.SelectedPath;
        }

        private int num;
        private void btn_begin_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(txt_path.Text, "*.txt");
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
            Process pipeClient = new Process();

            pipeClient.StartInfo.FileName = "Worker.exe";

            using (AnonymousPipeServerStream pipeServer =
                new AnonymousPipeServerStream(PipeDirection.Out,
                    HandleInheritability.Inheritable))
            {
                // Show that anonymous pipes do not support Message mode.
                try
                {
                    //                    Console.WriteLine("[SERVER] Setting ReadMode to \"Message\".");
                    pipeServer.ReadMode = PipeTransmissionMode.Byte;
                }
                catch (NotSupportedException e)
                {
                    //                    Console.WriteLine("[SERVER] Exception:\n    {0}", e.Message);
                }

                //                Console.WriteLine("[SERVER] Current TransmissionMode: {0}.",
                //                    pipeServer.TransmissionMode);

                // Pass the client process a handle to the server.
                pipeClient.StartInfo.Arguments =
                    pipeServer.GetClientHandleAsString();
                pipeClient.StartInfo.UseShellExecute = false;
                pipeClient.Start();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        sw.WriteLine("SYNC");
                        sw.WriteLine(path);
                        //                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        //                        Console.Write("[SERVER] Enter text: ");
                        //                        sw.WriteLine(Console.ReadLine());
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("[SERVER] Error: {0}", e.Message);
                }
            }

            pipeClient.WaitForExit();
            pipeClient.Close();
//            Console.WriteLine("[SERVER] Client quit. Server terminating.");
        }
    }
}