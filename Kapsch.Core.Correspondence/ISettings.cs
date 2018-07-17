using Kapsch.Core.Data.Enums;

namespace Kapsch.Core.Correspondence
{
    public interface ISettings
    {
        CorrespondenceType CorrespondenceType { get; }
        string CorrespondenceSubType { get; }
        CorrespondenceProvider CorrespondenceProvider { get; }
    }
}
