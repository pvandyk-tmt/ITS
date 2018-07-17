using System.Net.Sockets;
using System.Text;

namespace Kapsch.Device.Listener
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public StateObject()
        {
            Buffer = new byte[BufferSize];
            Data = new StringBuilder();
        }

        public byte[] Buffer { get; set; }
        public StringBuilder Data { get; set; }
        public object Item { get; set; }
        public Socket WorkSocket { get; set; }
    }
}