using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kapsch.ITS.App.Common.Models;

namespace Kapsch.ITS.App
{
    public static class ApplicationSession
    {
        public static bool IsAuthenticated
        {
            get
            {
                return AuthenticatedUser != null;
            }
        }

        public static AuthenticatedUser AuthenticatedUser { get; set; }
    }
}
