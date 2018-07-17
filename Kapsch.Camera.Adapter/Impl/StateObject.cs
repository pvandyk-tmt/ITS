using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public class StateObject
    {
        public const int BufferSize = 1024;

        public object Item { get; set; }
        public Socket WorkSocket { get; set; }
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Data = new StringBuilder();
    }
}
