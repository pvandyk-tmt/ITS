package za.co.kapsch.console.General;

/**
 * Created by CSenekal on 2017/06/30.
 */
import android.app.Application;
import android.content.Context;

import za.co.kapsch.shared.LibApp;

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