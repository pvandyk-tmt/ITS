namespace Kapsch.Core
{
    public interface ICorrespondent
    {
        long ID { get; set; }
        string MobileNumber { get; set; }
        string Email { get; set; }
    }
}
