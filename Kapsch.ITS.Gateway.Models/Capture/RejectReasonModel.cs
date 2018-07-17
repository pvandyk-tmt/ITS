
namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class RejectReasonModel
    {
        public int ID { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
