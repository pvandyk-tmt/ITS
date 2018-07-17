package za.co.kapsch.console.Models;

/**
 * Created by CSenekal on 2017/07/11.
 */
public class ActivityLaunchInfoModel {

    private String mPackageName;
    private String mClassName;
    private int mRequestCode;

    public String getPackageName() {
        return mPackageName;
    }

    public void setPackageName(String packageName) {
        mPackageName = packageName;
    }

    public String getClassName() {
        return mClassName;
    }

    public void setClassName(String className) {
        mClassName = className;
    }

    public int getRequestCode() {
        return mRequestCode;
    }

    public void setRequestCode(int requestCode) {
        mRequestCode = requestCode;
    }
}
