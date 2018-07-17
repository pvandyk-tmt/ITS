using Kapsch.Core.Data.Enums;

namespace Kapsch.Core.Correspondence
{
    public interface IPayload
    {
        CorrespondenceType CorrespondenceType { get; }
        string SubType { get; }
        string CorrespondenceContext { get; }
    }
}
