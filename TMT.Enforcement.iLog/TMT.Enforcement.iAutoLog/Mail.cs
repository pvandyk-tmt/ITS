using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;
using TMT.Enforcement.ErrorWriting;

namespace TMT.Enforcement.iAutoLog
{
    /// <summary>
    /// Email class
    /// </summary>
    public class Mail
    {
        private string mHost;
        private int mPort;
        private string mUserName;
        private string mPassword;
        private string mAddressFrom;
        private string mAddressGroupTo;
        private bool mEnableSsl;
        private bool mUseDefaultCredentials;

        private ErrorLogging errorWriting;

        private Mail()
        {
        }

        public Mail(string host, int port, string userName, string password, string addressFrom, string addressGroupTo, bool enableSsl, bool useDefaultCredentials)
        {
            mHost = host;
            mPort = port;
            mUserName = userName;
            mPassword = password;
            mAddressFrom = addressFrom;
            mAddressGroupTo = addressGroupTo;
            mEnableSsl = enableSsl;
            mUseDefaultCredentials = useDefaultCredentials;
        }

        /// <summary>
        /// Used to send a mail with/without an attachment 
        /// </summary>
        public void sendMail(string subject, string body, Attachment logAttachment, Attachment exceptionLogAttachment = null)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(mHost))
                {
                    client.UseDefaultCredentials = mUseDefaultCredentials;
                    client.Credentials = new NetworkCredential(mUserName, mPassword);
                    client.EnableSsl = mEnableSsl;

                    if (mPort > 0)
                    {
                        client.Port = mPort;
                    }

                    string[] emailList = mAddressGroupTo.Split(';');

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(mAddressFrom);
                        message.BodyEncoding = Encoding.UTF8;
                        message.Body = body;
                        message.Subject = subject;

                        foreach (string emailAddress in emailList)
                        {
                            message.To.Add(emailAddress);
                        }

                        if (!object.ReferenceEquals(null, logAttachment))
                        {
                            message.Attachments.Add(logAttachment);
                        }

                        if (!object.ReferenceEquals(null, exceptionLogAttachment))
                        {
                            message.Attachments.Add(exceptionLogAttachment);
                        }

                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                errorWriting = new ErrorLogging();
                errorWriting.WriteErrorLog(string.Format("Exception during mailing on machine {0}: [{1}]", Environment.MachineName, ex.Message));             
            }
        }
    }
}
