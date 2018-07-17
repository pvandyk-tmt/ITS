package za.co.kapsch.ipayment.WebAccess;

import android.app.Service;
import android.content.Intent;
import android.os.Handler;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.support.v4.content.LocalBroadcastManager;

import microsoft.aspnet.signalr.client.ConnectionState;
import microsoft.aspnet.signalr.client.ErrorCallback;
import microsoft.aspnet.signalr.client.LogLevel;
import microsoft.aspnet.signalr.client.Logger;
import microsoft.aspnet.signalr.client.Platform;
import microsoft.aspnet.signalr.client.http.android.AndroidPlatformComponent;
import microsoft.aspnet.signalr.client.hubs.HubConnection;
import microsoft.aspnet.signalr.client.hubs.HubProxy;
import microsoft.aspnet.signalr.client.transport.ServerSentEventsTransport;
import za.co.kapsch.ipayment.Enums.BroadcastSource;
import za.co.kapsch.ipayment.General.App;
import za.co.kapsch.ipayment.General.Constants;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/09/12.
 */
public class PaymentTransactionService extends Service {

    private static final int TRANSACTION_CONFIRMATION_TIMEOUT = 120000;

    private static HubConnection mConnection;
    private static final Object mConnectionLock = new Object();

    private Handler handler = new Handler();
    private static boolean mBroadcastCompleted;
    private static final Object mBroadcastCompletedLock = new Object();

    private static String mTransactionToken;
    private static final Object mTransactionTokenLock = new Object();

    @Override
    public void onCreate() {
        super.onCreate();
        Platform.loadPlatformComponent(new AndroidPlatformComponent());
    }

    @SuppressWarnings("deprecation")
    @Override
    public void onStart(Intent intent, int startId) {
        super.onStart(intent, startId);
        start(intent.getStringExtra("TransactionToken"));
    }

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {

            disconnectSignalR();
            while (getConnectionState() != ConnectionState.Disconnected) {}

            if (isBroadcastCompleted() == true) return;

            Intent intent = new Intent(Constants.TRANSACTION_CONFIRMATION_ACTION);
            intent.putExtra(Constants.BROADCAST_SOURCE, BroadcastSource.Timeout.getCode());
            intent.putExtra(Constants.TRANSACTION_TOKEN, getTransactionToken());
            LocalBroadcastManager.getInstance(App.getContext()).sendBroadcast(intent);

            setBroadcastCompleted(true);
        }
    };

    private void start(String transactionToken) {

        try {
            Logger logger = new Logger() {

                @Override
                public void log(String message, LogLevel level) {
                    System.out.println(message);
                }
            };

            if (mConnection != null) {
                if (getConnectionState() != ConnectionState.Disconnected) {
                    return;
                }
            }

            mConnection = new HubConnection(CoreGatewayUrls.getPaymentTransactionSignalRHubUrl());
            setTransactionToken(transactionToken);
            mConnection.getHeaders().put("TransactionToken", getTransactionToken());
            HubProxy mProxy = mConnection.createHubProxy("paymentTransactionHub");
            mConnection.start(new ServerSentEventsTransport(logger));

            while (getConnectionState() == ConnectionState.Connecting) {
                try {
                    Thread.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }

            if (getConnectionState() == ConnectionState.Disconnected) {
                return;
            }

            mProxy.subscribe(new Object() {

                Intent intent = new Intent(Constants.TRANSACTION_CONFIRMATION_ACTION);

                @SuppressWarnings("unused")
                public void onStatusChanged(String transactionToken, int status, double amount) {

                    //MessageManager.showMessage(String.format("Token: %s Status: %d Amount: %d", transactionToken, status, amount), ErrorSeverity.None);
                    try {
                        if (isBroadcastCompleted() == true) return;

                        intent.putExtra(Constants.BROADCAST_SOURCE, BroadcastSource.TransactionService.getCode());
                        intent.putExtra(Constants.TRANSACTION_TOKEN, transactionToken);
                        intent.putExtra(Constants.TRANSACTION_STATUS, status);
                        intent.putExtra(Constants.TRANSACTION_AMOUNT, amount);
                        LocalBroadcastManager.getInstance(App.getContext()).sendBroadcast(intent);

                        setBroadcastCompleted(true);

                    } catch (Exception e) {
                        MessageManager.showMessage(Utilities.exceptionMessage(e, "PaymentTransactionService::onStatusChanged()"), ErrorSeverity.None);
                    }
                }
            });

            // Subscribe to the error event
            mConnection.error(new ErrorCallback() {

                @Override
                public void onError(Throwable error) {
                    MessageManager.showMessage(error.getMessage(), ErrorSeverity.None);
                }
            });

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PaymentTransactionService::start()"), ErrorSeverity.Low);
        }

        mBroadcastCompleted = false;
        handler.postDelayed(runnable, TRANSACTION_CONFIRMATION_TIMEOUT);
    }

    public void setBroadcastCompleted(boolean value){
        synchronized (mBroadcastCompletedLock) {
            mBroadcastCompleted = value;
        }
    }

    public boolean isBroadcastCompleted(){
        synchronized (mBroadcastCompletedLock) {
            return mBroadcastCompleted;
        }
    }

    public void disconnectSignalR(){
        synchronized (mConnectionLock) {
            if (mConnection != null && mConnection.getState() == ConnectionState.Connected) {
                mConnection.disconnect();
            }
        }
    }

    public ConnectionState getConnectionState(){
        synchronized (mConnectionLock) {
            return mConnection.getState();
        }
    }

    public void setTransactionToken(String transactionToken){
        synchronized (mTransactionTokenLock) {
            mTransactionToken = transactionToken;
        }
    }

    public String getTransactionToken(){
        synchronized (mTransactionTokenLock) {
            return mTransactionToken;
        }
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onDestroy() {
        disconnectSignalR();
        super.onDestroy();
    }
}



