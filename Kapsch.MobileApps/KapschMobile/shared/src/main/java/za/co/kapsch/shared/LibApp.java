package za.co.kapsch.shared;

/**
 * Created by CSenekal on 2017/06/30.
 */
import android.app.Application;
import android.content.Context;
import android.support.multidex.MultiDexApplication;

/**
 * Created by csenekal on 2016-09-07.
 */
public class LibApp extends MultiDexApplication {

    private static Context mContext;

    @Override
    public void onCreate() {
        super.onCreate();
        mContext = getApplicationContext();
    }

    public static Context getContext(){
        return mContext;
    }
}

//public class LibApp extends Application{
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