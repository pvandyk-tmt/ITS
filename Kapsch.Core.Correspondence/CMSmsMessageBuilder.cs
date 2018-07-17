using System;
using Newtonsoft.Json.Linq;

namespace Kapsch.Core.Correspondence
{
    public class CMSmsMessageBuilder
    {
        public string CreateMessage(Guid productToken,
                            string sender,
                            string recipient,
                            string message)
        {
            var jsonObj = new JObject
            {
                { "Messages", new JObject
                    {
                        { "Authentication", new JObject { { "ProductToken", productToken } } },
                        { "Msg", new JArray { new JObject { { "From", sender }, { "To", new JArray { new JObject { { "Number", recipient } } } }, { "Body", new JObject { { "Content", message } } } } } }
                    } }
            };

            return jsonObj.ToString();
            //return new JObject
            //{
            //    ["Messages"] = new JObject
            //    {
            //        ["Authentication"] = new JObject
            //        {
            //            ["ProductToken"] = productToken
            //        },
            //        ["Msg"] = new JArray {
            //            new JObject {
            //                ["From"] = sender,
            //                ["To"] = new JArray {
            //                    new JObject { ["Number"] = recipient }
            //                },
            //                ["Body"] = new JObject {
            //                    ["Content"] = message
            //                }
            //            }
            //        }
            //    }
            //}.ToString();
        }

        public string GetContentType()
        {
            return "application/json";
        }
    }
}
