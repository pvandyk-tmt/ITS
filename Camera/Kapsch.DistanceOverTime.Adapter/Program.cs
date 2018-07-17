using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Kapsch.Camera.Listener.Base;
using Kapsch.DistanceOverTime.Adapter.Calculations;
using Kapsch.DistanceOverTime.Adapter.Factory;
using Kapsch.DistanceOverTime.Adapter.Framework;
using Kapsch.RTE.Gateway.Clients;
using Kapsch.RTE.Gateway.Models.Camera;
using Kapsch.RTE.Gateway.Models.Configuration.Dot;
using NLog;

namespace Kapsch.DistanceOverTime.Adapter
{
    internal class Program
    {
        private static Defaults _defaults;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            try
            {
                var clp = new CommandLineParser(args);
                _defaults = new Defaults();
                clp.Parse(_defaults);

                if (args.Length == 1)
                {
                    if (args[0] == "help" || args[0] == "/help" || args[0] == "?" || args[0] == "/h")
                    {
                        Console.WriteLine("Command Line Parameter Assistence");
                        Console.WriteLine("Note:  Any value with a space must be enclosed in quotation marks");
                        clp.Help(_defaults);
                    }

                    return;
                }

                if (_defaults.HasErrors())
                {
                    Console.WriteLine("Command Line Parameter Assistence");
                    Console.WriteLine("Note:  Any value with a space must be enclosed in quotation marks");
                    clp.Help(_defaults);
                }
                else
                {
                    var t = new Thread(ThreadLoop);
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static void ThreadLoop(object callback)
        {
            Logger.Info("Reading config settings...");
            Logger.Info("Starting New Distance Over Time Adapter for Service Type {0} - DO NOT CLOSE!!", _defaults.Listener);

            BaseCameraListener cameraListenerA = ListenerFactory.GetListener(_defaults, ListenerFactory.PointDefinition.PointA);
            cameraListenerA.Connect();

            BaseCameraListener cameraListenerB = ListenerFactory.GetListener(_defaults, ListenerFactory.PointDefinition.PointB);
            cameraListenerB.Connect();

            List<AtPointModel> pointsA = new List<AtPointModel>();
            List<AtPointModel> pointsB = new List<AtPointModel>();

            ConfigurationDotService configurationDotService = new ConfigurationDotService();
            AtPointService atPointService = new AtPointService();
            OverSectionService overSectionService = new OverSectionService();

            DateTime timeout = DateTime.Now.AddMinutes(1);

            Logger.Info("Reading config settings.This can take some time as the service connects to the listener...");

            SectionConfigurationModel sectionConfigurationModel = null;
            AtPointModel a = null;
            AtPointModel b = null;

            int retryCounter = 0;
            int maxRetryCounter = 3;
            while (true)
            {
                if (DateTime.Now > timeout)
                {
                    Logger.Error("Cannot connect to the listener or no data received. Stopping adapter service after timeout of 1 minute.");
                    return;
                }

                try
                {
                    if (cameraListenerA.DataPoints.Count > 0 && cameraListenerB.DataPoints.Count > 0)
                    {
                        if (a == null)
                        {
                            cameraListenerA.DataPoints.TryTake(out a);
                            atPointService.Create(a);
                        }

                        if (b == null)
                        {
                            cameraListenerB.DataPoints.TryTake(out b);
                            atPointService.Create(b);
                        }

                        if (a != null && b != null)
                        {
                            sectionConfigurationModel = configurationDotService.GetSectionConfiguration(a.SectionPointCode, b.SectionPointCode);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retryCounter++;
                    Logger.Error(ex);
                }

                if (retryCounter > maxRetryCounter)
                {
                    Logger.Error("After trying {0} times, the service cannot register this adapter - the REST service might be down! Stopping adapter service!", maxRetryCounter);
                    return;
                }
            }

            if (!configurationDotService.RegisterAdapter(sectionConfigurationModel, _defaults.Listener, Helper.HeartbeatSeconds))
            {
                Logger.Error("Cannot register this adapter - there is a similar adapter configured already! Stopping adapter service!");
                return;
            }

            int listenerCounterTimeoutThreshold = Helper.ListenerCounterTimeout;
            int listenerCounterTimeout = 0;
            retryCounter = 0;

            DateTime? nextHeartBeat = null;

            while (true)
            {
                nextHeartBeat = SendHeartBeat(nextHeartBeat, configurationDotService, sectionConfigurationModel);

                //if (listenerCounterTimeout > listenerCounterTimeoutThreshold)
                //{
                //    Logger.Error("Warning!! After trying {0} times, it seems like the listener is not reading data! Check the Adapters!", listenerCounterTimeoutThreshold);
                //    listenerCounterTimeout = 0;
                //}

                if (retryCounter > maxRetryCounter)
                {
                    Logger.Error("After trying {0} times, the service cannot read / post data - the REST service might be down! Check the Service!", maxRetryCounter);
                    retryCounter = 0;
                }

                try
                {
                    while (cameraListenerA.DataPoints.Count > 0)
                    {
                        AtPointModel m;
                        if (cameraListenerA.DataPoints.TryTake(out m))
                        {
                            atPointService.Create(m);
                            pointsA.Add(m);
                        }
                    }

                    while (cameraListenerB.DataPoints.Count > 0)
                    {
                        AtPointModel m;
                        if (cameraListenerB.DataPoints.TryTake(out m))
                        {
                            atPointService.Create(m);
                            pointsB.Add(m);
                        }
                    }

                    nextHeartBeat = SendHeartBeat(nextHeartBeat, configurationDotService, sectionConfigurationModel);

                    pointsA = pointsA.OrderBy(c => c.EventDateTime).ToList();
                    pointsB = pointsB.OrderBy(c => c.EventDateTime).ToList();

                    if (pointsA.Count == 0 || pointsB.Count == 0)
                    {
                        listenerCounterTimeout++;
                        Thread.Sleep(1000);
                        continue;
                    }

                    listenerCounterTimeout = 0;

                    SectionCalculator sectionCalculator = new SectionCalculator(
                            sectionConfigurationModel.LevenshteinMatchDistance,
                            sectionConfigurationModel.SectionDistanceInMeter,
                            sectionConfigurationModel.SectionDescription,
                            sectionConfigurationModel.SectionCode,
                            pointsA,
                            pointsB);

                    List<SectionCalculationResult> offences = sectionCalculator.Calculate();

                    foreach (SectionCalculationResult sectionCalculationResult in offences)
                    {
                        nextHeartBeat = SendHeartBeat(nextHeartBeat, configurationDotService, sectionConfigurationModel);

                        OverSectionModel model = new OverSectionModel
                        {
                            AtPointA = sectionCalculationResult.AtPointEnd,
                            AtPointB = sectionCalculationResult.AtPointEnd,
                            Zone = sectionCalculationResult.Zone,
                            Vln = sectionCalculationResult.Vln,
                            MachineId = sectionCalculationResult.MachineId,
                            DateFormat = sectionCalculationResult.DateFormat,
                            SectionDescription = sectionCalculationResult.SectionDescription,
                            SectionCode = sectionCalculationResult.SectionCode,
                            AverageAnprAccuracy = sectionCalculationResult.AverageAnprAccuracy,
                            AverageSpeed = sectionCalculationResult.AverageSpeed,
                            GraceSpeed = sectionCalculationResult.GraceSpeed,
                            SectionDistanceInMeter = sectionCalculationResult.SectionDistanceInMeter,
                            TravelDistance = sectionCalculationResult.TravelDistance,
                            TripDuration = sectionCalculationResult.TripDuration,
                            IsOffence = sectionCalculationResult.IsOffence,
                            FileName = sectionCalculationResult.FileName,
                            FrameNumber = sectionCalculationResult.FrameNumber
                        };

                        if (sectionConfigurationModel.CreatePhysicalInfringement)
                        {
                            if (string.IsNullOrEmpty(Helper.PhysicalInfringementPath))
                            {
                                Logger.Error("WARNING!!! The Configuration is setup to create physical infringments but the application config file does not contain a valid path. Please stop this process and update the path!!");
                            }

                            PhysicalInfringement.Create(model, Helper.PhysicalInfringementPath);
                        }

                        string paths = Path.Combine(sectionCalculationResult.AtPointStart.ImagePhysicalFileAndPath, sectionCalculationResult.AtPointStart.ImageName);
                        model.AtPointA.Image = File.ReadAllBytes(paths);

                        string pathe = Path.Combine(sectionCalculationResult.AtPointEnd.ImagePhysicalFileAndPath, sectionCalculationResult.AtPointEnd.ImageName);
                        model.AtPointB.Image = File.ReadAllBytes(pathe);

                        overSectionService.PostData(model);
                        Logger.Info("Added Offence");

                    }
                }
                catch (Exception ex)
                {
                    retryCounter++;
                    Logger.Error(ex);
                }
                
                Thread.Sleep(500);
            }
        }

        private static DateTime SendHeartBeat(DateTime? nextHeartBeat, ConfigurationDotService configurationDotService, SectionConfigurationModel sectionConfigurationModel)
        {
            if (nextHeartBeat == null || DateTime.Now > nextHeartBeat.Value)
            {
                configurationDotService.SendHeartbeatToAdapter(sectionConfigurationModel, _defaults.Listener, Helper.HeartbeatSeconds);
                return DateTime.Now.AddSeconds(Helper.HeartbeatSeconds);
            }

            return nextHeartBeat.Value;
        }

        private static string ReadLine(int timeoutms)
        {
            Logger.Info("Done! Waiting for next run. Enter Q to quit or wait " + timeoutms / 1000 + " seconds and the process will continue...");

            ReadLineDelegate d = Console.ReadLine;
            var result = d.BeginInvoke(null, null);
            result.AsyncWaitHandle.WaitOne(timeoutms); //timeout e.g. 15000 for 15 secs

            if (result.IsCompleted)
                return d.EndInvoke(result);

            return "";
        }

        private delegate string ReadLineDelegate();
    }
}