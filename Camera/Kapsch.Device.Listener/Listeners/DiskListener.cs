using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Kapsch.Device.Listener.Enums;
using Kapsch.Device.Listener.Events;
using Kapsch.Device.Listener.Interfaces;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;
using NLog;

namespace Kapsch.Device.Listener.Listeners
{
    public class DiskListener : IListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private volatile bool _continiousRead;
        private Thread _readThread;

        public DiskListener(IListenerConfiguration configuration)
        {
            _continiousRead = false;

            if (!(configuration is DiskConfigurationModel))
            {
                throw new Exception(string.Format("The listener for device {0} is not a disk type listener!", configuration.DeviceName));
            }

            Configuration = configuration;
            DiskConfigurationModel dc = (DiskConfigurationModel)configuration;

            Logger.Info("Created file listener for device on {0}", dc.FilePath);
        }

        public event EventHandler<ListenEvent> ListenEventReceived;
        public IListenerConfiguration Configuration { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        
        public bool IsConnected
        {
            get { return _continiousRead; }
        }

        public bool Disconnect()
        {
            _continiousRead = false;
            return true;
        }

        public void Send(string data)
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            if (!IsConnected)
            {
                _continiousRead = true;

                if (_readThread == null)
                {
                    _readThread = new Thread(PollDisk);
                    _readThread.Start();
                }
            }

            return IsConnected;
        }

        private void PollDisk()
        {
            do
            {
                try
                {
                    DiskConfigurationModel dc = (DiskConfigurationModel)Configuration;

                    IEnumerable<string> files = (from file in Directory.EnumerateFiles(dc.FilePath, dc.SearchPattern, SearchOption.AllDirectories)
                        select file).ToList();

                    foreach (var fileFullName in files)
                    {
                        using (var file = new StreamReader(fileFullName))
                        {
                            string line;

                            while ((line = file.ReadLine()) != null)
                            {
                                if (!string.IsNullOrWhiteSpace(line))
                                    OnListenEventReceived(new ListenEvent { Message = line });
                            }

                            file.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Trace("Poll Error" + ex.Message);
                }
                finally
                {
                    Thread.Sleep(Configuration.ListenEveryMilliseconds);
                }

            } while (_continiousRead);

            KillThread();
        }

        private void KillThread()
        {
            try
            {
                if (_readThread != null)
                {
                    _readThread.Join(500);
                    _readThread.Abort();
                    _readThread = null;
                }
            }
            catch
            {
                _readThread = null;
            }
        }

        private void OnListenEventReceived(ListenEvent e)
        {
            if (ListenEventReceived != null)
                ListenEventReceived.Invoke(this, e);
        }
    }
}