namespace Kapsch.ITS.Gateway.Models.Correspondence
{
    public class SendResponseModel
    {
        public string ReferenceNumber { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
    }
}
