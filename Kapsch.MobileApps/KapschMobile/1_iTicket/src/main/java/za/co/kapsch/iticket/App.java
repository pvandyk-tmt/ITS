package za.co.kapsch.iticket;

import android.app.Application;
import android.content.Context;

import za.co.kapsch.shared.LibApp;

/**
 * Created by csenekal on 2016-09-07.
 */
public class App extends LibApp {

    @Override
    public void onCreate() {
        super.onCreate();
    }
}

//public class App extends Application{
//
//    private static Context mContext;
//
//    @Override
//    public void onCreate() {
//        super.onCreate();
//        mContext = getApplicationContext();
//    }
//
//    public static Context getContext(){
//        return mContext;
//    }
//}