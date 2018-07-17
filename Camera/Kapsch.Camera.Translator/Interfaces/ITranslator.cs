using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;

namespace Kapsch.Camera.Translator.Interfaces
{
    public interface ITranslator
    {
        AtPointModel Translate();
        object TextLine { get; set; }
        string Name { get; set; }
    }
}
