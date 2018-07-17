using Kapsch.Camera.Adapters.Impl;
using System.Collections;

namespace Kapsch.Camera.Adapters.Interfaces
{
    public interface IAdapter
    {
        IEnumerable Process();
        void Shutdown();

        string Name { get; set; }
    }
}
