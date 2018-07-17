using Kapsch.Camera.Listener.Base;
using Kapsch.Camera.Translator.Interfaces;
using Kapsch.Camera.Translator.Translators.iCam;
using Kapsch.Device.Listener.Interfaces;

namespace Kapsch.Camera.Listener.Listeners.iCam
{
    public class iCamCameraListener : BaseCameraListener
    {
        /// <summary>
        /// Uses the default iCam Translator
        /// </summary>
        /// <param name="listener"></param>
        public iCamCameraListener(IListener listener) : this(listener, new InfringementLineTextTranslator())
        {

        }

        public iCamCameraListener(IListener listener, ITranslator translator) : base(listener, translator)
        {

        }

        
    }
}