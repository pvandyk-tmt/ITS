
namespace Kapsch.ITS.Gateway.Models.Adjudicate
{
    public class OffenceCodeModel
    {
        public int Code { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Code.ToString();
        }
    }
}
