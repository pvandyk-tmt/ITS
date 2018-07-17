using System;
using System.Threading;
using Kapsch.Camera.Listener.Base;
using Kapsch.Camera.Translator.Interfaces;
using Kapsch.Camera.Translator.Translators.Mock;
using Kapsch.Device.Listener.Interfaces;
using Kapsch.DistanceOverTime.Adapter;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;
using Kapsch.RTE.Gateway.Models.Camera.Enum;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener;
using NLog;

namespace Kapsch.Camera.Listener.Listeners.Mock
{
    public class MockCameraListener : BaseCameraListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private volatile bool _continiousRead;
        private Thread _readThread;

        /// <summary>
        ///     Uses the default iCam Translator
        /// </summary>
        /// <param name="listener"></param>
        public MockCameraListener(IListener listener) : this(listener, new MockTranslator())
        {

        }

        public MockCameraListener(IListener listener, ITranslator translator) : base(listener, translator)
        {
            _continiousRead = false;

            if (!(listener.Configuration is MockConfigurationModel))
            {
                throw new Exception(string.Format("The listener for device {0} is not a mock type listener!", listener.Configuration.DeviceName));
            }
        }

        public bool IsConnected
        {
            get { return _continiousRead; }
            
        }

        public override bool Disconnect()
        {
            _continiousRead = false;
            return true;
        }

        public override bool Connect()
        {
            if (!IsConnected)
            {
                _continiousRead = true;

                if (_readThread == null)
                {
                    _readThread = new Thread(PollMock);
                    _readThread.Start();
                }
            }

            return IsConnected;
        }

        private void PollMock()
        {
            MockConfigurationModel mc = (MockConfigurationModel) Listener.Configuration;

            if (mc.Seed < 1000)
            {
                mc.Seed = 1000;
            }

            Random random = new Random(mc.Seed);

            string[] vlnListLight = {"CA370097", "CA404871", "CA502246", "CA636432", "CL32644"};
            string[] vlnListHeavy = {"BY38GZGP", "CA383630", "CA702912", "CA776761", "CA782572"};

            do
            {
                try
                {
                    AtPointModel model = new AtPointModel
                    {
                        AnprAccuracy = 95,
                        Image = null,
                        ImagePhysicalFileAndPath = @"C:\Temp",
                        MachineId = "MachineId",
                        PlateImagePhysicalFileAndPath = @"C:\Temp",
                        PointLocation = new LocationModel
                        {
                            GpsLatitude = "",
                            GpsLongitude = ""
                        },
                        SectionPointCode = mc.LocationCode,
                        SerialNumber = "SerialNumber",
                        SourceTextLine = "",
                        VosiReason = "",
                        SectionPointDescription = ""
                    };

                    if (random.Next(0, 10) % 2 == 0)
                    {
                        model.Classification = new ClassificationZoneModel
                        {
                            Classification = VehicleClassificationEnum.Heavy,
                            Zone = 80,
                            Grace = 10
                        };

                        model.Vln = vlnListHeavy[random.Next(0, 5)];
                        model.ImageName = "Heavy_" + model.Vln + ".jpg";
                        model.Speed = random.Next(70, 110);
                    }
                    else
                    {
                        model.Classification = new ClassificationZoneModel
                        {
                            Classification = VehicleClassificationEnum.Light,
                            Zone = 120,
                            Grace = 10
                        };

                        model.Vln = vlnListLight[random.Next(0, 5)];
                        model.ImageName = "Light_" + model.Vln + ".jpg";
                        model.Speed = random.Next(100, 160);
                    }

                    model.PlateImageName = model.Vln + ".jpg";

                    model.EventDateTime = DateTime.Now.AddSeconds(random.Next(mc.TimeOffsetSecondsStart, mc.TimeOffsetSecondsEnd));

                    if (random.Next(0, 10) % 2 == 0)
                    {
                        model.ShotDirection = DirectionEnum.Away;
                    }
                    else
                    {
                        model.ShotDirection = DirectionEnum.Towards;
                    }

                    model.ShotDistance = random.Next(45, 190);

                    model.HashVln = VlnHash.Hash(model.Vln);

                    Add(model);

                }
                catch (Exception ex)
                {
                    Logger.Trace("Poll Error" + ex.Message);
                }
                finally
                {
                    Thread.Sleep(Listener.Configuration.ListenEveryMilliseconds);
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
    }
}