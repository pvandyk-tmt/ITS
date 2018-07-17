package za.co.kapsch.iticket.WebAccess;

import android.app.Service;
import android.content.Intent;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.support.v4.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import microsoft.aspnet.signalr.client.ConnectionState;
import microsoft.aspnet.signalr.client.LogLevel;
import microsoft.aspnet.signalr.client.Logger;
import microsoft.aspnet.signalr.client.Platform;
import microsoft.aspnet.signalr.client.http.android.AndroidPlatformComponent;
import microsoft.aspnet.signalr.client.hubs.HubConnection;
import microsoft.aspnet.signalr.client.hubs.HubProxy;
import microsoft.aspnet.signalr.client.hubs.SubscriptionHandler1;
import microsoft.aspnet.signalr.client.transport.ServerSentEventsTransport;
import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.iticket.Models.SectionModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Enums.ErrorSeverity;

/**
 * Created by CSenekal on 2017/05/17.
 */
public class DistanceOverTimeInfringementService extends Service {

    private HubConnection mConnection;
    private static final int FAILED = 1;
    private static final int SUCCESS = 2;

    @Override
    public void onCreate() {
        super.onCreate();

        Platform.loadPlatformComponent(new AndroidPlatformComponent());
    }

    @SuppressWarnings("deprecation")
    @Override
    public void onStart(Intent intent, int startId) {
        super.onStart(intent, startId);
        start();
    }

    private void start() {

        Logger logger = new Logger() {

            @Override
            public void log(String message, LogLevel level) {
                System.out.println(message);
            }
        };

        if (mConnection != null){
            if (mConnection.getState() != ConnectionState.Disconnected){
                return;
            }
        }

        mConnection = new HubConnection(DistanceOverTimeGatewayUrls.getDistanceOverTimeSignalRHubUrl());//"http://192.168.0.33:88/SignalR/hubs/");

        HubProxy mProxy = mConnection.createHubProxy("NotificationHub"); //the hub
        mConnection.getHeaders().put("HardwareCode", Utilities.getDeviceId());

        mConnection.start(new ServerSentEventsTransport(logger));

        while (mConnection.getState() == ConnectionState.Connecting) {
            try {
                Thread.sleep(500);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        if (mConnection.getState() == ConnectionState.Disconnected){
            return;
        }

        mProxy.on("onStatusChanged", new SubscriptionHandler1<String>() {

            Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd'T'HH:mm:ss").create();
            Intent intent = new Intent(Constants.DOT_INFRINGEMENT_ACTION);

            @Override
            public void run(String jsonData) {
                try {
                    if (jsonData.equals("null")) return;

                    SectionModel sectionModel = gson.fromJson(jsonData, SectionModel.class);
                    intent.putExtra(Constants.DOT_INFRINGEMENT_RESULT, sectionModel);
                    LocalBroadcastManager.getInstance(App.getContext()).sendBroadcast(intent);

                } catch (Exception e) {
                    MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.None);
                }
            }
        }, String.class);

//        mProxy.on("OnSectionPointConnectionReceived", new SubscriptionHandler1<String>() {
//
//            Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd'T'HH:mm:ss").create();
//
//            @Override
//            public void run(String jsonData) {
//                try {
//                    SectionPointModel sectionPoint = gson.fromJson(jsonData, SectionPointModel.class);
//
//                } catch (Exception e) {
//                    MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.None);
//                }
//            }
//        }, String.class);

//        mProxy.on("OnSectionConnectionReceived", new SubscriptionHandler1<String>() {
//
//            Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd'T'HH:mm:ss").create();
//
//            @Override
//            public void run(String jsonData) {
//                try {
//                    SectionModel section = gson.fromJson(jsonData, SectionModel.class);
//
//                } catch (Exception e) {
//                    MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.None);
//                }
//            }
//        }, String.class);
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onDestroy() {
        if (mConnection != null && mConnection.getState() == ConnectionState.Connected) {
            mConnection.disconnect();
        }

        super.onDestroy();
    }
}
