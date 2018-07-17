namespace Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces
{
    public interface IListenerConfiguration
    {
        int ListenEveryMilliseconds { get; set; }
        int DeviceId { get; set; }
        string DeviceName { get; set; }
    }
}
