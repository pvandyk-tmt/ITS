using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Atlas;
using TMT.Core.Camera.Base;
using System.Windows.Forms;
using TMT.Enforcement.iLog.Persistence;
using TMT.Core.Camera.RedRoom;
using System.IO;
using TMT.Enforcement.iLog;
using TMT.Enforcement.ErrorWriting;
using System.Reflection;
using System.Timers;
using TMT.Enforcement.iAutoLog.Properties;
using Quartz;
using Quartz.Impl;
using Quartz.Job;
using System.Net.Mail;

namespace TMT.Enforcement.iAutoLog
{
    public class MainService: IAmAHostedProcess
    {
        private IScheduler _scheduler;
        private ErrorLogging errorWriting;
        private cDataAccess dataAccess;
        private string mImagePath;

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            errorWriting = new ErrorLogging();
            errorWriting.WriteErrorLog("Service has started on " + Environment.MachineName);

            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["CoreContext"];
            dataAccess = new cDataAccess(setting.ConnectionString);


            bool success = dataAccess.getImagePath(Environment.MachineName, out mImagePath);
            if (!success)
            {
                errorWriting.WriteErrorLog("Invalid Image path. Please check the computer name!");
                Stop();
            }

            try
            {
                _scheduler = StdSchedulerFactory.GetDefaultScheduler();
          
                _scheduler.Start();
        
                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<ProcessJob>()
                    .Build();

                IJobDetail job2 = JobBuilder.Create<ProcessJob>()
                    .Build();    

                // Trigger the job to run now, and then repeat 0
                ITrigger trigger = TriggerBuilder.Create()
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(6, 0))
                    .Build();
            
                // Tell quartz to schedule the job using our trigger
                _scheduler.ScheduleJob(job, trigger);

                ITrigger trigger2 = TriggerBuilder.Create()
                    .StartNow()                    
                    .Build();
                
                _scheduler.ScheduleJob(job2, trigger2);
            }
            catch (SchedulerException ex)
            {
                errorWriting.WriteErrorLog(ex.Message);
            }

        }

        public void Stop()
        {
            errorWriting.WriteErrorLog("AutoiLog service Stopped : " + Environment.MachineName);
            SendMail("AutoiLog service stopped", "The AutoiLog Service has been manually stopped on " + Environment.MachineName + ". Please investigate.");
            _scheduler.Shutdown();
        }

        private void SendMail(string subject, string body)
        {
            Mail mail = 
                new Mail(
                    Settings.Default.SmtpHost, 
                    Settings.Default.SmtpPort, 
                    Settings.Default.SmtpUserName,
                    Settings.Default.SmtpPassword, 
                    Settings.Default.SmtpMailFrom, 
                    Settings.Default.MailDistributionList,
                    Settings.Default.MailEnableSsl, 
                    Settings.Default.SmtpUseDefaultCredentials);

            Attachment attachment = null;            

            mail.sendMail(subject, body, attachment);
        }
    }
}
