using Kapsch.Camera.Translator.Interfaces;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;

namespace Kapsch.Camera.Translator.Translators.Mock
{
   public class MockTranslator : ITranslator
    {
        public AtPointModel Translate()
        {
            return TextLine as AtPointModel;
        }

        public object TextLine { get; set; }
        public string Name { get; set; }
    }
}
