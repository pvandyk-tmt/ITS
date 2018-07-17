#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TMT.Core.Camera.Base;
using TMT.Core.Camera.Interfaces;

#endregion

namespace TMT.Core.Camera.RedRoom
{
    public class cConfiguration
    {
        public List<cCamera> GetCameras(string path, string configFile)
        {
            Configuration configuration = new Configuration();

            CameraConfigs config = toConfiguration(path, configFile);
            List<cCamera> cameras = new List<cCamera>();

            if (config != null)
            {
                foreach (var cameraConfig in config.Items)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFile(configuration.GetDLLPath()+ "\\" + cameraConfig.AssemblyName + ".dll");

                        foreach (Type t in asm.GetTypes())
                        {
                            if (t.Namespace == cameraConfig.NameSpace)
                            {
                                cCamera ch = Activator.CreateInstance(t, new object[] { cameraConfig.CameraVersion, cameraConfig.CameraName, cameraConfig.DateFormat, new Size(cameraConfig.MaxWidth, cameraConfig.MaxHeight) }) as cCamera;
                                if (ch != null)
                                {
                                    cameras.Add(ch);
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return cameras;
               
        }

        public static string serializeObjectToXML(Type objectType, object item)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(objectType);
                MemoryStream memoryStream = new MemoryStream();
                using (XmlTextWriter xmlTextWriter =
                    new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = Formatting.None })
                {
                    xmlSerializer.Serialize(xmlTextWriter, item);
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    string xmlText = new UTF8Encoding().GetString(memoryStream.ToArray());
                    memoryStream.Dispose();
                    return xmlText;
                }
            }
            catch
            {
                return null;
            }
        }

        public static CameraConfigs toConfiguration(string path, string xml)
        {
            XmlSerializer mySerializer = null;
            FileStream myFileStream = null;
            try
            {
                CameraConfigs myObject;
                // Construct an instance of the XmlSerializer with the type
                // of object that is being deserialized.
                mySerializer = new XmlSerializer(typeof(CameraConfigs));
                // To read the file, create a FileStream.
                myFileStream = new FileStream(Path.Combine(path, "CameraConfig.xml"), FileMode.Open);
                // Call the Deserialize method and cast to the object type.
                myObject = (CameraConfigs)mySerializer.Deserialize(myFileStream);

                return myObject;
                
            }
            catch
            {
                return null;
            }
            finally
            {
                if (mySerializer != null) mySerializer = null;
                if (myFileStream != null) myFileStream.Close();
            }
        }        
    }
}