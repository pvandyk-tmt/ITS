using System;

namespace Kapsch.Core.Correspondence
{
    [Serializable]
    public class EmailAttachment
    {
        public string Name { get; set; }
        public string Mimetype { get; set; }
        public byte[] Contents { get; set; }
    }
}
