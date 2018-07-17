﻿using System.IO;
using Newtonsoft.Json;
using RestSharp.Serializers;

// http://patrickrileyblogs.blogspot.co.za/2014/02/restsharp-using-jsonnet-serializer.html
namespace Kapsch.Core.Gateway.Clients.Serializer
{
    public class RestSharpJsonNetSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public RestSharpJsonNetSerializer()

        {
            ContentType = "application/json";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        public RestSharpJsonNetSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";
            _serializer = serializer;
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
 
                    _serializer.Serialize(jsonTextWriter, obj);
 
                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }
 
        public string DateFormat { get; set; }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string ContentType { get; set; }
    }

}
