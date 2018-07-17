using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Enforcement.ErrorWriting
{
    public class ErrorLogging
    {
        public void WriteErrorLog(string Message)
        {
            string ErrorLogFilePath = ConfigurationManager.AppSettings["ErrorLogFilePath"];
            Directory.CreateDirectory(ErrorLogFilePath);

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(ErrorLogFilePath + "\\" + String.Format("{0:yyyyMMdd}", DateTime.Now) + "_LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            { }
        }

        public void WriteErrorLog(Exception ex)
        {
            
            try
            {
                string ErrorLogFilePath = ConfigurationManager.AppSettings["ErrorLogFilePath"];
                Directory.CreateDirectory(ErrorLogFilePath);

                StreamWriter sw = null;

                sw = new StreamWriter(ErrorLogFilePath + "\\" + String.Format("{0:yyyyMMdd}", DateTime.Now) + "_ExceptionLogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": Exception: " + ex.Source.ToString().Trim() + "; Exception Message: " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            { }
        }

        public void WriteErrorLog(string ex, string additionalInfo)
        {
            string ErrorLogFilePath = ConfigurationManager.AppSettings["ErrorLogFilePath"];
            Directory.CreateDirectory(ErrorLogFilePath);

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(ErrorLogFilePath + "\\" + String.Format("{0:yyyyMMdd}", DateTime.Now) + "_ExceptionLogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + additionalInfo + "; Exception Message: " + ex);
                sw.Flush();
                sw.Close();
            }
            catch
            { }
        }
    }
}
