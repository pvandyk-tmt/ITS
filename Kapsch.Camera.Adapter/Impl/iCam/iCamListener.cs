using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Enums;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public class iCamListener
    {
        private static readonly Regex TagRegex = new Regex(@"(.*)(?:\n)");
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Socket _receiver = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly ManualResetEvent _connectDone = new ManualResetEvent(false);

        public iCamListener(long deviceId, string deviceName, string ipAddress, int port = 7001)
        {
            Log.InfoFormat("Created file listener for device on {0}:{1}", ipAddress, port);
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            DeviceId = deviceId;
            DeviceName = deviceName;
        }

        public string DeviceName { get; set; }

        public bool SetMonitoring(bool isActive)
        {
            try
            {
                if (isActive)
                {
                    _receiver.BeginConnect(RemoteEndPoint, new AsyncCallback(ConnectCallback), _receiver);
                    _connectDone.WaitOne();

                    var state = new StateObject { WorkSocket = _receiver };

                    _receiver.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (_receiver.Connected)
                    {
                        _receiver.Disconnect(true);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Send(string data)
        {
            if (_receiver == null)
                return;

            var byteData = Encoding.UTF8.GetBytes(data.Trim());
            Log.DebugFormat("Sending data {0} to {1}.", data, _receiver.RemoteEndPoint.ToString());

            _receiver.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), _receiver);
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

                    blockingState = this._receiver.Blocking;

                    var tmp = new byte[1];

                    _receiver.Blocking = false;
                    _receiver.Send(tmp, 0, 0);
                }
                catch (SocketException e)
                {
                    if (e.NativeErrorCode.Equals(10035))
                    {
                        Log.Error(string.Format("{0} is still connected, but the send would block.", RemoteEndPoint));
                    }

                    else
                    {
                        Log.Error(string.Format("{0} is disconnected with error code {1}.", RemoteEndPoint, e.NativeErrorCode));
                    }

                    try
                    {
                        _receiver.Disconnect(true);
                    }
                    catch
                    {
                    }

                    return false;
                }
                finally
                {
                    _receiver.Blocking = blockingState;
                }

                return true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                var client = state.WorkSocket;

                var bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var strRead = Encoding.UTF8.GetString(state.Buffer, 0, bytesRead);
                    state.Data.Append(strRead);

                    var matches = TagRegex.Matches(state.Data.ToString());
                    if (matches != null && matches.Count > 0)
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
                var sender = (Socket)ar.AsyncState;
                sender.EndConnect(ar);

                Log.DebugFormat("Socket connected to {0}.", RemoteEndPoint.ToString());

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
                    OniCamEventReceived(new iCamEventArgs { OriginalMessage = originalMessage });
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket)ar.AsyncState;

                var bytesSent = handler.EndSend(ar);
                Log.DebugFormat("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Send failed from {0}.", RemoteEndPoint), ex);
            }
        }

        private void OniCamEventReceived(iCamEventArgs e)
        {
            if (iCamEventReceived != null)
                iCamEventReceived.Invoke(this, e);
        }

        public event EventHandler<iCamEventArgs> iCamEventReceived;
        public CameraStatusType CameraConnectivity { get; set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public long DeviceId { get; set; }
    }
}
