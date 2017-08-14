using System.IO;
using System.IO.Pipes;

namespace Pipe
{
    public class PipeClient
    {
        public PipeStream Pipe { get; set; }
        public StreamReader PipeReader { get; set; }

        public PipeClient(string arg)
        {
            Pipe = new AnonymousPipeClientStream(PipeDirection.In, arg) {ReadMode = PipeTransmissionMode.Byte};
            PipeReader = new StreamReader(Pipe);
        }

        /// <summary>
        /// Sync
        /// </summary>
        /// <returns></returns>
        public string Receive()
        {
            return PipeReader.ReadLine();
        }
    }
}