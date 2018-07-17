namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class CaptureTypeModel
    {
        public int ID;
        public string Description;
        public decimal Amount;

        public override string ToString()
        {
            return Description;
        }
    }
}
