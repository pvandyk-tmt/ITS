using Kapsch.ITS.App.Common.Models;
using System.Drawing;

namespace Kapsch.ITS.App.Common
{
    public interface IShellApplication
    {
        bool HasAccess(AuthenticatedUser authenticatedUser);
        void Show(AuthenticatedUser authenticatedUser);
        string MenuLabel { get; }
        Bitmap MenuImage { get; }
    }
}
