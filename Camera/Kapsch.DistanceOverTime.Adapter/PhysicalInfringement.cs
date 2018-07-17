using System.Globalization;
using System.IO;
using System.IO.Compression;
using Kapsch.RTE.Gateway.Models;
using Kapsch.RTE.Gateway.Models.Camera;

namespace Kapsch.DistanceOverTime.Adapter
{
    public static class PhysicalInfringement
    {
        public static bool Create(OverSectionModel model, string destinationPath)
        {
            var fileName = Path.Combine(destinationPath, model.FileName);
            var dir = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(dir))
                if (dir != null) Directory.CreateDirectory(dir);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        var meta = archive.CreateEntry("metadata.txt");

                        using (var entryStream = meta.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.WriteLine(model.SectionCode + "|" +
                                                   model.SectionDescription.Replace("|", " ") + "|" +
                                                   model.AverageSpeed + "|" + model.TravelDistance + "|" + model.Zone +
                                                   "|" + model.AtPointA.EventDateTime.ToString(model.DateFormat, CultureInfo.InvariantCulture) + "|" +
                                                   model.AtPointB.EventDateTime.ToString(model.DateFormat, CultureInfo.InvariantCulture) + "|" + model.Vln + "|" +
                                                   model.DateFormat + "|" + model.MachineId + "|" + model.FrameNumber);
                        }

                        var data = archive.CreateEntry("data.txt");

                        using (var entryStream = data.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.WriteLine(model.AtPointA.SourceTextLine);
                            streamWriter.WriteLine(model.AtPointB.SourceTextLine);
                        }
                    }

                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }
                }

                using (var newFile = ZipFile.Open(fileName, ZipArchiveMode.Update))
                {
                    newFile.CreateEntryFromFile(Path.Combine(model.AtPointA.ImagePhysicalFileAndPath, model.AtPointA.ImageName), model.AtPointA.ImageName, CompressionLevel.Optimal);
                    newFile.CreateEntryFromFile(Path.Combine(model.AtPointB.ImagePhysicalFileAndPath, model.AtPointA.ImageName), model.AtPointB.ImageName, CompressionLevel.Optimal);

                    newFile.CreateEntryFromFile(Path.Combine(model.AtPointA.PlateImagePhysicalFileAndPath, model.AtPointA.PlateImageName), model.AtPointA.PlateImageName.Replace(".jpg", "_PLATE_1.jpg"), CompressionLevel.Optimal);
                    newFile.CreateEntryFromFile(Path.Combine(model.AtPointB.PlateImagePhysicalFileAndPath, model.AtPointA.PlateImageName), model.AtPointB.PlateImageName.Replace(".jpg", "_PLATE_2.jpg"), CompressionLevel.Optimal);
                }

                return true;
            }
            catch
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                return false;
            }
        }
    }
}