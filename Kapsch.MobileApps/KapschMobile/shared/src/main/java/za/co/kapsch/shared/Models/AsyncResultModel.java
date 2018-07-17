package za.co.kapsch.shared.Models;

/**
 * Created by csenekal on 2016-11-11.
 */
public class AsyncResultModel {
    private int mProcessResult;
    private Object mObject;
    private String mMessage;
    private int mProcessId;

    public AsyncResultModel(int processResult, Object object, String message, int processId) {
        mProcessResult = processResult;
        mObject = object;
        mMessage = message;
        mProcessId = processId;
    }

    public int getProcessResult() {
        return mProcessResult;
    }

    public String getMessage() {
        return mMessage;
    }

    public void setMessage(String message) {
        this.mMessage = message;
    }

    public int getProcessId() {
        return mProcessId;
    }

    public Object getObject() {
        return mObject;
    }

    public void setObject(Object object) {
        mObject = object;
    }
}
