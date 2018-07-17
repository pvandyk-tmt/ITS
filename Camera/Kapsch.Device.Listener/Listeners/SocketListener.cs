using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Kapsch.Device.Listener.Enums;
using Kapsch.Device.Listener.Events;
using Kapsch.Device.Listener.Interfaces;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;
using NLog;

namespace Kapsch.Device.Listener.Listeners
{
    public class SocketListener : IListener, IDisposable
    {
        private static readonly Regex TagRegex = new Regex(@"(.*)(?:\n)");
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);
        private readonly Socket _receiver;

        public SocketListener(IListenerConfiguration configuration)
        {
            if (!(configuration is SocketConfigurationModel))
            {
                throw new Exception(string.Format("The listener for device {0} is not a socket type listener!", configuration.DeviceName));
            }

            Configuration = configuration;
            SocketConfigurationModel sc = (SocketConfigurationModel) configuration;

            Log.Info("Created file listener for device on {0}:{1}", sc.IpAddress, sc.IpPort);
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(sc.IpAddress), sc.IpPort);

            _receiver = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public IPEndPoint RemoteEndPoint { get; private set; }
        public IListenerConfiguration Configuration { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public bool IsConnected
        {
            get
            {
                if (_receiver == null)
                    return false;

                var blockingState = false;

                try
                {
                    blockingState = _receiver.Blocking;
                    _receiver.Blocking = false;
                    _receiver.Send(new byte[1], 0, 0);
                }
                catch (SocketException e)
                {
                    if (e.NativeErrorCode.Equals(10035))
                    {
                        Log.Error("{0} is still connected, but the send would block.", RemoteEndPoint);
                    }

                    else
                    {
                        Log.Error("{0} is disconnected with error code {1}.", RemoteEndPoint, e.NativeErrorCode);
                    }

                    try
                    {
                        _receiver.Disconnect(true);
                    }
                    catch
                    {
                    }
                }
                finally
                {
                    _receiver.Blocking = blockingState;
                }

                return _receiver.Connected;
            }
        }

        public bool Connect()
        {
            try
            {
                PingDevice();

                if (ConnectionStatus == ConnectionStatus.Operational)
                {
                    if (!IsConnected)
                    {
                        _receiver.BeginConnect(RemoteEndPoint, ConnectCallback, _receiver);
                        _connectDone.WaitOne();

                        var state = new StateObject {WorkSocket = _receiver};

                        _receiver.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                    }

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Disconnect()
        {
            if (_receiver.Connected)
            {
                _receiver.Disconnect(true);
            }

            return true;
        }

        public void Send(string data)
        {
            if (_receiver == null)
                return;

            var byteData = Encoding.UTF8.GetBytes(data.Trim());
            Log.Trace("Sending data {0} to {1}.", data, _receiver.RemoteEndPoint);

            _receiver.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, _receiver);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_receiver.Connected)
                {
                    _receiver.Shutdown(SocketShutdown.Both);
                    _receiver.Close();

                    _receiver.Dispose();
                }
            }
        }

        private void PingDevice()
        {
            using (var ping = new Ping())
            {
                var pingReply = ping.Send(RemoteEndPoint.Address.ToString());

                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    ConnectionStatus = pingReply.RoundtripTime <= 500 ? ConnectionStatus.Operational : ConnectionStatus.Intermittent;
                }
                else
                {
                    ConnectionStatus = ConnectionStatus.Offline;
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject) ar.AsyncState;
                var client = state.WorkSocket;

                var bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var strRead = Encoding.UTF8.GetString(state.Buffer, 0, bytesRead);
                    state.Data.Append(strRead);

                    var matches = TagRegex.Matches(state.Data.ToString());

                    if (matches.Count > 0)
                    {
                        Process(matches);

                        var index = strRead.LastIndexOf('\n');
                        if (index + 1 <= strRead.Length)
                        {
                            state.Data = new StringBuilder(strRead.Substring(index + 1, strRead.Length - index - 1));
                        }
                    }

                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Receive failed on {0}.", RemoteEndPoint), ex);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                var sender = (Socket) ar.AsyncState;
                sender.EndConnect(ar);

                Log.Info("Socket connected to {0}.", RemoteEndPoint);

                _connectDone.Set();
            }
            catch (SocketException se)
            {
                Log.Error(string.Format("Socket failed to connect to {0}.", RemoteEndPoint), se);
                _connectDone.Set();
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Socket failed to connect to {0}.", RemoteEndPoint), ex);
                _connectDone.Set();
            }
        }

        private void Process(MatchCollection matches)
        {
            for (var i = 0; i < matches.Count; i++)
            {
                var originalMessage = matches[i].Value;

                if (!string.IsNullOrWhiteSpace(originalMessage))
                    OnListenEventReceived(new ListenEvent {Message = originalMessage});
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket) ar.AsyncState;

                var bytesSent = handler.EndSend(ar);
                Log.Info("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Send failed from {0}.", RemoteEndPoint), ex);
            }
        }

        private void OnListenEventReceived(ListenEvent e)
        {
            if (ListenEventReceived != null)
                ListenEventReceived.Invoke(this, e);
        }

        public event EventHandler<ListenEvent> ListenEventReceived;
    }
}