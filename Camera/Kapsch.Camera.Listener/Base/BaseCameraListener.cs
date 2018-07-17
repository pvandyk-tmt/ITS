using System;
using System.Collections.Concurrent;
using System.Threading;
using Kapsch.Camera.Translator.Interfaces;
using Kapsch.Device.Listener.Events;
using Kapsch.Device.Listener.Interfaces;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;

namespace Kapsch.Camera.Listener.Base
{
    public abstract class BaseCameraListener
    {
        protected BaseCameraListener(IListener listener, ITranslator translator)
        {
            Translator = translator;
            Listener = listener;
            DataPoints = new ConcurrentBag<AtPointModel>();
        }

        public ConcurrentBag<AtPointModel> DataPoints { get; private set; }

        internal ITranslator Translator { get; set; }

        internal IListener Listener { get; set; }

        public string Name { get; set; }

        private Timer Timer { get; set; }

        public virtual bool Connect()
        {
            if (Listener == null || Translator == null)
            {
                throw new Exception("You cannot connect to the adapter if you do not have a Listener or Translator!");
            }

            Listener.ListenEventReceived += Listener_ListenEventReceived;
            Timer = new Timer(obj => Run(), null, 100, 1000 * Listener.Configuration.ListenEveryMilliseconds);

            return true;
        }

        public virtual void Listener_ListenEventReceived(object sender, ListenEvent e)
        {
            Add(e.Message);
        }

        private void Run()
        {
            try
            {
                Timer.Dispose();
                Listener.Connect();
            }
            finally
            {
                Timer = new Timer(obj => Run(), null, 100, 1000 * Listener.Configuration.ListenEveryMilliseconds);
            }
        }

        public virtual bool Disconnect()
        {
            if (Listener != null)
            {
                Listener.Disconnect();
            }

            if (Timer != null)
                Timer.Dispose();

            return true;
        }
        
        protected virtual bool Add(object textLine)
        {
            if (Translator != null)
            {
                Translator.TextLine = textLine;
                DataPoints.Add(Translator.Translate());
                return true;
            }

            return false;
        }
    }
}
