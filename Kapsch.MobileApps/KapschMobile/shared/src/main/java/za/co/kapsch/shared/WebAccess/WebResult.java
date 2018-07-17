package za.co.kapsch.shared.WebAccess;

/**
 * Created by csenekal on 2016-08-08.
 */
public class WebResult {

    private int mCode;
    private String mError;
    private String mResult;
    private byte[] mArrayResult;

    public int getCode() {
        return mCode;
    }

    public void setCode(int code) {
        mCode = code;
    }

    public String getError() {
        return mError;
    }

    public void setError(String error) {
        mError = error;
    }

    public String getResult() {
        return mResult;
    }

    public void setResult(String result) {
        mResult = result;
    }

    public byte[] getArrayResult() { return mArrayResult; }

    public void setArrayResult(byte[] arrayResult) { mArrayResult = arrayResult; }

    public void clear(){
        mCode = 0;
        mError = null;
        mResult = null;
        mArrayResult = null;
    }
}
