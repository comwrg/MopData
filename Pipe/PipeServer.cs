using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Mail;

namespace Pipe
{
    public class PipeServer
    {
        public AnonymousPipeServerStream Pipe { get; set; }
        public StreamWriter PipeWriter { get; set; }
        public Process Process { get; set; }

        public PipeServer(string filename)
        {
            Process = new Process {StartInfo = {FileName = filename}};

            Pipe =
                new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable) {ReadMode = PipeTransmissionMode.Byte};
            Process.StartInfo.Arguments = Pipe.GetClientHandleAsString();
            Process.StartInfo.UseShellExecute = false;
            Process.Start();
            Pipe.DisposeLocalCopyOfClientHandle();
            
            PipeWriter = new StreamWriter(Pipe) { AutoFlush = true};
        }

        public void Send(string msg)
        {
            PipeWriter.WriteLine(msg);
        }

        /// <summary>
        /// Sync
        /// </summary>
        public void WaitForExit()
        {
            Process.WaitForExit();
            Process.Close();
        }
    }
}