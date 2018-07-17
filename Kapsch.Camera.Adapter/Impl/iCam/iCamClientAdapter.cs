using Kapsch.Camera.Adapters.Impl.iCam;
using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Configuration;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Monitor;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public sealed class iCamClientAdapter : BaseTCPAdapter
    {
        private static readonly Regex TagRegex = new Regex(@"(.*)(?:\n)");
        private static readonly string DestinationFolder = ConfigurationManager.AppSettings["iCamDestinationFolder"];
        private static readonly int CheckIntervalInSeconds = int.Parse(ConfigurationManager.AppSettings["CheckIntervalInSeconds"]);

        public readonly ConcurrentDictionary<string, iCamListener> _iCamCollection = new ConcurrentDictionary<string, iCamListener>();

        protected override void PrepareDevices()
        {
            var configurationService = new ConfigurationService(Authorize.SessionToken);

            var filters = new List<FilterModel>();
            filters.Add(new FilterModel { PropertyName = "CameraAdapterType", Operation = Gateway.Models.Shared.Enums.Operation.Equals, Value = CameraAdapterType.iCamClient });
            filters.Add(new FilterModel { PropertyName = "IsEnabled", Operation = Gateway.Models.Shared.Enums.Operation.Equals, Value = "1" });
            filters.Add(new FilterModel { PropertyName = "CameraConnectionType", Operation = Gateway.Models.Shared.Enums.Operation.Equals, Value = CameraConnectionType.Tcp });
            filters.Add(new FilterModel { PropertyName = "ConnectToHost", Operation = Gateway.Models.Shared.Enums.Operation.Equals, Value = "0" });

            var devices = configurationService.GetCameraPaginatedList(filters, Gateway.Models.Shared.Enums.FilterJoin.And, true, "ID", 1, 1000000);


            foreach (var device in devices.Models)
            {
                var config = JsonConvert.DeserializeObject<dynamic>(device.ConfigJson);
                var key = string.Format("{0}:{1}", config.Ip, config.Port);

                if (!_iCamCollection.Keys.Contains(key))
                {
                    var camListener = new iCamListener(device.ID, device.FriendlyName, (string)config.Ip, (int)config.Port);
                    camListener.iCamEventReceived += OnICamEventReceived;

                    _iCamCollection.TryAdd(key, camListener);
                }
            }
        }

        protected override void Initialise()
        {
            Timer = new Timer((obj) => Run(), null, 100, 1000 * CheckIntervalInSeconds);
        }

        public override void Shutdown()
        {
            _iCamCollection.Values.ToList().ForEach(f => f.SetMonitoring(false));

            if (Timer != null)
                Timer.Dispose();
        }

        public async void Run()
        {
            try
            {
                Timer.Dispose();

                PrepareDevices();

                await Task.WhenAll(_iCamCollection.Values.Select(f => PingCamera(f))).ConfigureAwait(false);
                await Task.WhenAll(_iCamCollection.Values.Where(f => f.CameraConnectivity == CameraStatusType.Operational).Select(f => QueryCameraInfo(f))).ConfigureAwait(false);

                foreach (var icamListener in _iCamCollection.Values.Where(f => f.CameraConnectivity == CameraStatusType.Operational).Where(fileListener => !fileListener.IsConnected))
                {
                    icamListener.SetMonitoring(true);
                }
            }
            finally
            {
                Timer = new Timer((obj) => Run(), null, 100, 1000 * CheckIntervalInSeconds);
            }            
        }

        public async Task PingCamera(iCamListener iCamListener)
        {
            using (var ping = new Ping())
            {
                var pingReply = await ping.SendPingAsync(iCamListener.RemoteEndPoint.Address.ToString());
                if (pingReply.Status == IPStatus.Success)
                {
                    iCamListener.CameraConnectivity = (pingReply.RoundtripTime <= 500) ? CameraStatusType.Operational : CameraStatusType.Intermittent;
                }
                else
                {
                    iCamListener.CameraConnectivity = CameraStatusType.Offline;
                }

                var monitorService = new MonitorService(Authorize.SessionToken);
                monitorService.AddCameraStatus(
                    new CameraStatusModel
                    {
                        DeviceID = iCamListener.DeviceId,
                        CameraStatusType = iCamListener.CameraConnectivity
                    });
            }
        }

        public async Task QueryCameraInfo(iCamListener iCamListener)
        {
            var icamClient = new RestClient(string.Format("http://{0}", iCamListener.RemoteEndPoint.Address.ToString()));
            var request = new RestRequest("caminfo.json", Method.GET);
            var response = await icamClient.ExecuteTaskAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var camInfo = DeserializeObject(response.Content);
                    if (camInfo != null)
                    {
                        await SendCameraInfo(iCamListener.DeviceId, camInfo);
                    }
                }
                catch
                {
                    // Something happened, now what?
                }               
            }
        }

        protected override void Cleanup() { }

        private async Task SendCameraInfo(long deviceID, iCamInfo camInfo)
        {
            var model = new CameraLastStatisticModel();
            model.DeviceID = deviceID;
            model.SerialNumber = camInfo.SerialNumber;
            model.Operator = camInfo.Operator;
            model.SmdType = camInfo.SmdType;
            model.SystemStatus = camInfo.SystemStatus;
            
            if (camInfo.Location != null)
            {
                model.LocationCode = camInfo.Location.Code;
                model.LocationDescription = camInfo.Location.Description;
                model.LocationType = camInfo.Location.Type;
                model.LocationGPS = camInfo.Location.GPS;
                model.LocationZoneLight = int.Parse(camInfo.Location.ZoneLight);
                model.LocationZonePT = int.Parse(camInfo.Location.ZonePT);
                model.LocationZoneHeavy = int.Parse(camInfo.Location.ZoneHeavy);
                model.LocationThresholdLight = int.Parse(camInfo.Location.ThresholdLight);
                model.LocationThresholdPT = int.Parse(camInfo.Location.ThresholdPT);
                model.LocationThresholdHeavy = int.Parse(camInfo.Location.ThresholdHeavy);
            }

            if (camInfo.LastInfingement != null)
            {
                model.LastInfingementTime = camInfo.LastInfingement.Time; //" : "14:08:55.38";
                model.LastInfingementSpeed = int.Parse(camInfo.LastInfingement.Speed);
                model.LastInfingementDistance = int.Parse(camInfo.LastInfingement.Distance);
                model.LastInfingementPlate = camInfo.LastInfingement.Plate;
                model.LastInfingementType = camInfo.LastInfingement.Type;
            }

            if (camInfo.LastVoSI != null)
            {
                model.LastVoSITime = camInfo.LastVoSI.Time; //14:11:33.81";
                model.LastVoSIPlate = camInfo.LastVoSI.Plate;
                model.LastVoSIReason = camInfo.LastVoSI.Reason;
            }

            if (camInfo.SessionStatistics != null)
            {
                model.SessionStatisticsUptime = camInfo.SessionStatistics.Uptime; //" : "4:02:58";
                model.SessionStatisticsVehicleCount = camInfo.SessionStatistics.VehicleCount;
                model.SessionStatisticsInfringementCount = camInfo.SessionStatistics.InfringementCount;
                model.SessionStatisticsInfringementRate = (decimal)camInfo.SessionStatistics.InfringementRate;//2.2;
                model.SessionStatisticsVehicleHourlyRate = camInfo.SessionStatistics.VehicleHourlyRate;
                model.SessionStatisticsSpeedInfringementCount = camInfo.SessionStatistics.SpeedInfringementCount;
                model.SessionStatisticsRedlightInfringementCount = camInfo.SessionStatistics.RedlightInfringementCount;
                model.SessionStatisticsHeadwayInfringementCount = camInfo.SessionStatistics.HeadwayInfringementCount;
                model.SessionStatisticsStoplineInfringementCount = camInfo.SessionStatistics.StoplineInfringementCount;
                model.SessionStatisticsYellowlineInfringementCount = camInfo.SessionStatistics.YellowlineInfringementCount;
                model.SessionStatisticsLineViolationCount = camInfo.SessionStatistics.LineViolationCount;
                model.SessionStatisticsEightyFivePercentileSpeed = camInfo.SessionStatistics.EightyFivePercentileSpeed;
                model.SessionStatisticsAverageSpeed = camInfo.SessionStatistics.AverageSpeed;
                model.SessionStatisticsStandardDeviation = camInfo.SessionStatistics.StandardDeviation;
                model.SessionStatisticsMaximumSpeed = camInfo.SessionStatistics.MaximumSpeed;
                model.SessionStatisticsVoSICount = camInfo.SessionStatistics.VoSICount;
            }

            if (camInfo.DayStatistics != null)
            {
                model.DayStatisticsUptime = camInfo.DayStatistics.Uptime;
                model.DayStatisticsVehicleCount = camInfo.DayStatistics.VehicleCount;
                model.DayStatisticsInfringementCount = camInfo.DayStatistics.InfringementCount;
                model.DayStatisticsInfringementRate = (decimal)camInfo.DayStatistics.InfringementRate;
                model.DayStatisticsVehicleHourlyRate = camInfo.DayStatistics.VehicleHourlyRate;
                model.DayStatisticsSpeedInfringementCount = camInfo.DayStatistics.SpeedInfringementCount;
                model.DayStatisticsRedlightInfringementCount = camInfo.DayStatistics.RedlightInfringementCount;
                model.DayStatisticsHeadwayInfringementCount = camInfo.DayStatistics.HeadwayInfringementCount;
                model.DayStatisticsStoplineInfringementCount = camInfo.DayStatistics.StoplineInfringementCount;
                model.DayStatisticsYellowlineInfringementCount = camInfo.DayStatistics.YellowlineInfringementCount;
                model.DayStatisticsLineViolationCount = camInfo.DayStatistics.LineViolationCount;
                model.DayStatisticsEightyFivePercentileSpeed = camInfo.DayStatistics.EightyFivePercentileSpeed;
                model.DayStatisticsAverageSpeed = camInfo.DayStatistics.AverageSpeed;
                model.DayStatisticsStandardDeviation = camInfo.DayStatistics.StandardDeviation;
                model.DayStatisticsMaximumSpeed = camInfo.DayStatistics.MaximumSpeed;
                model.DayStatisticsVoSICount = camInfo.DayStatistics.VoSICount;
            }

            var monitorService = new MonitorService(Authorize.SessionToken);
            await monitorService.AddCameraStatistics(model);
        }

        private void OnICamEventReceived(object sender, iCamEventArgs args)
        {
            var camListener = ((iCamListener) sender);

            Execute(
                new iCamEventModel
                {
                    Message = args.OriginalMessage,
                    HostId = camListener.DeviceId,
                    HostIp = camListener.RemoteEndPoint.Address.ToString(),
                    HostName  = camListener.DeviceName
                });
        }

        private void Execute(iCamEventModel model)
        {
            try
            {
                var parameters = model.Message.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parameters.Length != 2)
                    throw new Exception("Invalid message format.");

                var fileName = parameters[1].Replace("/home/SafeTcam/", string.Empty).Trim('\n').Replace("/", "\\");

                if (fileName.Contains(".enc"))
                {
                    var sourceFile = Path.Combine("\\\\" + model.HostIp, fileName);

                    fileName = fileName.Replace("dvd\\00\\00\\", string.Empty);

                    var destinationFile = Path.Combine(DestinationFolder, model.HostName, fileName);

                    Log.InfoFormat("Copying file source ({0}) to destination ({1}).", sourceFile, destinationFile);

                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
                    File.Copy(sourceFile, destinationFile, true);

                    Log.InfoFormat("Copied file source ({0}) successfully.", sourceFile);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Message ({0}) caused an error.", model.Message), ex);
            }
        }

        /// <summary>
        /// Workaround for the content not in the correct JSON format. 
        /// Deserializes the object.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        private iCamInfo DeserializeObject(string content)
        {
            content = content.Replace("\"Speed\" : 00", "\"Speed\" : ");
            content = content.Replace("\"Speed\" : 0", "\"Speed\" : ");
            content = content.Replace("\"Distance\" : 00", "\"Distance\" : ");
            content = content.Replace("\"Distance\" : 0", "\"Distance\" : ");
            content = content.Replace("\"Speed\" : --,", "\"Speed\" : 0,");
            content = content.Replace("\"Distance\" : --,", "\"Distance\" : 0,");

            return JsonConvert.DeserializeObject<iCamInfo>(content);
        }

        public Timer Timer { get; set; }
        public string CameraName { get; set; }
    }
}
