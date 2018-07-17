using System;

namespace Kapsch.Device.Listener.Events
{
    public class ListenEvent : EventArgs
    {
        public long Timestamp { get; set; }
        public object Message { get; set; }
    }
}
