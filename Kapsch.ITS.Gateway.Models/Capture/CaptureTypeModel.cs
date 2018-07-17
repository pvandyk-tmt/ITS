
namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class CaptureTypeModel
    {
        public CaptureTypeModel()
        {
            ID = -1;
            Code = string.Empty;
            Amount = 0;
        }

        public override string ToString()
        {
            return Type;
        }

        public int ID { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Beskrywing { get; set; }
        public decimal Amount { get; set; }
    }
}
