package za.co.tmt.iticket;

import android.app.Application;
import android.content.Context;

/**
 * Created by csenekal on 2016-09-07.
 */
public class App extends Application{

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