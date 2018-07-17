#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading;
using TMT.Core.Camera.Base;
using TMT.Core.Camera.RedRoom.Base;

#endregion

namespace TMT.Core.Camera.RedRoom
{
    /// <summary>
    /// 	The 'ConcreteMediator' class
    /// </summary>
    public class cPhysicalStudio : cStudio
    {
        public List<cFilm> Films { get; private set; }

        #region Delegates

        public delegate void FilmAddedDelegate(object sender, cFilm film);

        #endregion

        public event FilmAddedDelegate evFilmAdded;

        /// <summary>
        /// 	Gets all the films for the cameraInterface, developing a sample picture from each to
        /// 	make sure the partici
        /// </summary>
        /// <param name = "filterAfterDate"></param>
        /// <returns></returns>
        public override List<cFilm> GetDevelopableFilms(DateTime? filterAfterDate)
        {
            Films = new List<cFilm>();

            string logTestIndicator = ConfigurationManager.AppSettings["LogTestIndicator"];
            
            foreach (var participant in Participants)
            {
                if (Directory.Exists(Path))
                {
                    string[] files = Directory.GetFiles(Path, participant.Value.pEncFileName, SearchOption.AllDirectories);

                    bool skip = false;
                    DirectoryInfo parentToSkip = null;

                    foreach (string file in files)
                    {
                        DirectoryInfo parent = Directory.GetParent(file);

                        if (parent != null && parent.Name.ToUpper() != "AVGSPEED" && parent.Name.ToUpper() != "VOSI")
                        {
                            if (parent.FullName.ToUpper().Contains("TEST") == true && logTestIndicator.ToUpper() == "FALSE")
                            {
                                continue;
                            }

                            //It might be that the file has zero length
                            long length = new System.IO.FileInfo(file).Length;
                            if (length == 0)
                            {
                                continue;
                            }

                            if (skip)
                            {
                                if (parent.FullName == parentToSkip.FullName)
                                {
                                    continue;
                                }
                            }

                            DateTime? lastWrite;
                            try
                            {
                                lastWrite = parent.LastWriteTime;
                            }
                            catch
                            {
                                lastWrite = null;
                            }

                            if (filterAfterDate == null || lastWrite == null || (lastWrite.Value.Date > filterAfterDate.Value.Date))
                            {
                                bool exists = Films.Any(film => film.pPath == parent.FullName);

                                if (!exists)
                                {
                                    FileInfo fi = new FileInfo(file);
                                    cFilm f = new cFilm { pPath = parent.FullName, pCreationTime = lastWrite };

                                    if (participant.Value.developPictureFile(f, fi.Name))
                                    {
                                        f.pCameraDriver = participant.Value;
                                        participant.Value.ReadFilesFromDisk(f);

                                        if (evFilmAdded != null)
                                        {
                                            evFilmAdded(this, f);
                                        }

                                        Films.Add(f);

                                        skip = true;
                                        parentToSkip = parent;
                                    }
                                    else
                                    {
                                        f.Dispose();
                                    }
                                }
                            }

                            //skip = true;
                            //parentToSkip = parent;
                        }
                    }
                }
            }

            if (Films.Count > 0)
                Films.Sort(compare);

            return Films;
        }

        public override List<cFilm> GetDevelopableVideo(string fileName)
        {
            Films = new List<cFilm>();

            foreach (var participant in Participants)
            {
                if (Directory.Exists(Path))
                {
                    string[] files = Directory.GetFiles(Path, fileName, SearchOption.AllDirectories);

                    bool skip = false;
                    DirectoryInfo parentToSkip = null;

                    foreach (string file in files)
                    {
                        DirectoryInfo parent = Directory.GetParent(file);
                        
                        if (parent != null)
                        {
                            //It might be that the file has zero length
                            long length = new System.IO.FileInfo(file).Length;
                            if (length == 0)
                            {
                                continue;
                            }

                            if (skip)
                            {
                                if (parent.FullName == parentToSkip.FullName)
                                {
                                    continue;
                                }
                            }

                            bool exists = Films.Any(film => film.pPath == parent.FullName);

                            if (!exists)
                            {
                                FileInfo fi = new FileInfo(file);
                                cFilm f = new cFilm { pPath = parent.FullName};

                                if (participant.Value.developPictureFile(f, fi.Name))
                                {
                                    f.pCameraDriver = participant.Value;
                                    //participant.Value.ReadFilesFromDisk(f);

                                    Films.Add(f);                                    
                                }
                            }

                            skip = true;
                            parentToSkip = parent;
                        }
                    }
                }
            }
            return Films;
        }

        private static int compare(cFilm a, cFilm b)
        {
            int diff;

            if (a.pCreationTime != null && b.pCreationTime != null)
            {
                diff = DateTime.Compare(b.pCreationTime.Value, a.pCreationTime.Value);
                if (diff != 0)
                    return diff;
            }

            cPictureFile aPictureFile = a.getFirstValidPictureFile();
            cPictureFile bPictureFile = b.getFirstValidPictureFile();

            if (aPictureFile == null || bPictureFile == null)
            {
                return 0;
            }

            diff = System.String.CompareOrdinal(bPictureFile.pLocationCode, aPictureFile.pLocationCode);
            if (diff != 0)
                return diff;

            diff = System.String.CompareOrdinal(bPictureFile.pFormattedSession, aPictureFile.pFormattedSession);

            return diff;
        }

        public override void DevelopFilm(cFilm film)
        {
            cCamera cameraInterface = film.pCameraDriver;

            if (cameraInterface == null)
            {
                throw new Exception("This film does not belong to a camera!");
            }

            cameraInterface.developFilm(film);
        }

        public override void ClearFilm(cFilm film)
        {
            film.clearFilm();
        }

        public override string DevelopVideo(cFilm film, string fileName)
        {
            cCamera cameraInterface = film.pCameraDriver;

            if (cameraInterface == null)
            {
                throw new Exception("This film does not belong to a camera!");
            }

            return cameraInterface.developVideo(film, Path, fileName);
        }
    }
}