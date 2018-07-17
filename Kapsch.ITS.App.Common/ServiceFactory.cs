using System;

namespace Kapsch.ITS.App.Common
{
    public class ServiceFactory<T>
    {
        public static Func<T> CreateService;

        public static T GetService()
        {
            if (CreateService == null) throw new NotImplementedException("No way to create the service");

            return CreateService.Invoke();
        }
    }
}
