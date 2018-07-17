#region

using System;
using System.Collections.Generic;
using TMT.Core.Camera.Base;

#endregion

namespace TMT.Core.Camera.RedRoom.Base
{
    /// <summary>
    /// 	The 'Mediator' abstract class
    /// </summary>
    public abstract class cStudio
    {
        protected readonly Dictionary<string, cCamera> Participants = new Dictionary<string, cCamera>();

        public string Path { get; set; }

        public virtual void Register(cCamera participant)
        {
            if (!Participants.ContainsKey(participant.pName + ":" + participant.pVersion))
            {
                Participants[participant.pName + ":" + participant.pVersion] = participant;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name = "filterAfterDate"></param>
        /// <returns></returns>
        public virtual List<cFilm> GetDevelopableFilms(DateTime? filterAfterDate)
        {
            return new List<cFilm>();
        }

        public virtual void DevelopFilm(cFilm film)
        {

        }

        public virtual void ClearFilm(cFilm film)
        {

        }

        public virtual List<cFilm> GetDevelopableVideo(string fileName)
        {
            return new List<cFilm>();
        }

        public virtual string DevelopVideo(cFilm film, string fileName)
        {
            return string.Empty;
        }
    }
}