using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Correspondence
{
    public class EmailPayload : IPayload
    {
        private string subType;
        private string context;
        private string subject;
        private string textContent;
        private string htmlContent;
        private List<EmailAttachment> attachments;

        //private Guid correspondenceItemGuid;

        public EmailPayload(string context, string subject, string textContent, string htmlContent, List<EmailAttachment> attachments = null)
        {
            this.subject = subject;
            this.context = context;
            this.textContent = textContent;
            this.htmlContent = htmlContent;
            this.attachments = attachments;
        }

        public EmailPayload(string context, string subType, string subject, string textContent, string htmlContent, List<EmailAttachment> attachments = null)
        {
            this.subType = subType;
            this.subject = subject;
            this.context = context;
            this.textContent = textContent;
            this.htmlContent = htmlContent;
            this.attachments = attachments;
        }

        public void AddAttachment(EmailAttachment attachment)
        {
            if (this.attachments == null)
                this.attachments = new List<EmailAttachment>();

            this.attachments.Add(attachment);
        }

        public string Serialize()
        {
            return string.Empty;
        }

        public CorrespondenceType CorrespondenceType
        {
            get
            {
                return CorrespondenceType.Email;
            }
        }

        public string SubType
        {
            get
            {
                return this.subType;
            }
        }

        public string Subject
        {
            get
            {
                return this.subject;
            }
        }

        public string TextContent
        {
            get
            {
                return this.textContent;
            }
        }

        public string HtmlContent
        {
            get
            {
                return this.htmlContent;
            }
        }

        public List<EmailAttachment> Attachments
        {
            get
            {
                return this.attachments;
            }

            set
            {
                this.attachments = value;
            }
        }

        public string CorrespondenceContext
        {
            get
            {
                return this.context;
            }
        }
    }
}
