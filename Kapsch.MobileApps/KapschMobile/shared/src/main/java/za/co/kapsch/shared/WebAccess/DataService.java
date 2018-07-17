package za.co.kapsch.shared.WebAccess;

import android.app.Activity;
import android.os.AsyncTask;
import android.support.v4.app.Fragment;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.lang.reflect.Type;
import java.util.concurrent.ExecutionException;

import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Utilities;


/**
 * Created by csenekal on 2016-09-12.
 */
public class DataService {

    public static final int FAILED = 1;
    public static final int SUCCESS = 2;
    public static final int WEB_REQUEST = 3;

    private Activity mActivity;
    private WebResult mWebResult;
    private WebClient mWebClient;
    private boolean mShowBusyIndicator;
    private IAsyncProcessCallBack mAsyncProcessCallBack;

    private class Task extends AsyncTask<Object, AsyncResultModel, AsyncResultModel> {
        @Override
        protected AsyncResultModel doInBackground(Object... params) {

            try{
                return executeRequest(
                        (String)params[1],
                        (String)params[2],
                        params[3],
                        (byte[])params[4],
                        (int)params[5],
                        (Type)params[6],
                        (boolean)params[7],
                        (String)params[8],
                        (boolean)params[9]);

            }catch(Exception e){
                return new AsyncResultModel(SUCCESS, null, Utilities.exceptionMessage(e, "doInBackground"), (int)params[4]);
            }
        }

        private <T, U> AsyncResultModel executeRequest(String requestMethod, String url, T request, byte[] data, int processId, Type jsonResultType, boolean autherizationRequired, String filename, boolean streamResult){

            U response = null;

            try {
                response = getRequest(requestMethod, url, request, data, jsonResultType, autherizationRequired, filename, streamResult);

                if (response == null){
                    if ((mWebResult.getCode() != WebClient.SC_OK) || (mWebResult.getError() != null)) {
                        return new AsyncResultModel(FAILED, null, String.format("Request Failed: %s", mWebResult.getError()), processId);
                    }
                }

            }catch (Exception e){
                return new AsyncResultModel(FAILED, null, Utilities.exceptionMessage(e, String.format("executeRequest() ProcessId:%d", processId)), processId);
            }

            if (response == null){
                response = (U)request;
            }

            return new AsyncResultModel(SUCCESS, response, null, processId);
        }

        @Override
        protected void onPostExecute(AsyncResultModel result) {

            try {
                super.onPostExecute(result);

                if (mActivity != null) {
                    if (mShowBusyIndicator) Utilities.busyProgressBarEx(mActivity, false);
                }

                if (mAsyncProcessCallBack != null) {
                    mAsyncProcessCallBack.finishedCallBack(result);
                }
            }catch (Exception e){
                MessageManager.showMessage(Utilities.exceptionMessage(e, "onPostExecute"), ErrorSeverity.High);
            }
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();

            if (mActivity != null) {
                if (mShowBusyIndicator) Utilities.busyProgressBarEx(mActivity, true);
            }
        }

        @Override
        protected void onProgressUpdate(AsyncResultModel... values) {
            super.onProgressUpdate(values);
            if (mAsyncProcessCallBack != null) {
                mAsyncProcessCallBack.progressCallBack(values[0]);
            }
        }
    }

    public DataService(IAsyncProcessCallBack asyncProcessCallBack){

        mAsyncProcessCallBack = asyncProcessCallBack;

        if (asyncProcessCallBack instanceof Fragment){
            mActivity = ((Fragment)asyncProcessCallBack).getActivity();
        }else if (asyncProcessCallBack instanceof Activity) {
            mActivity = (Activity) asyncProcessCallBack;
        }
    }

    public DataService(IAsyncProcessCallBack asyncProcessCallBack, Activity activity){

        mActivity = activity;
        mAsyncProcessCallBack = asyncProcessCallBack;
    }

    public <T> String request(
            String requestMethod,
            String url,
            T request,
            byte[] data,
            int processId,
            Type jsonResultType,
            boolean autherizationRequired,
            String filename,
            boolean waitForTask,
            boolean showBusyIndicator,
            boolean multiThreadPool,
            boolean streamResult){

        try {
            mShowBusyIndicator = showBusyIndicator;

            if (waitForTask == true) {
                try {
                    if (multiThreadPool == true) {
                        new Task().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, WEB_REQUEST, requestMethod, url, request, data, processId, jsonResultType, autherizationRequired, filename, streamResult).get();
                    }else {
                        new Task().execute(WEB_REQUEST, requestMethod, url, request, data, processId, jsonResultType, autherizationRequired, filename, streamResult).get();
                    }
                } catch (InterruptedException e) {
                    return Utilities.exceptionMessage(e, "request()");
                } catch (ExecutionException e) {
                    return Utilities.exceptionMessage(e, "request()");
                }
                return null;
            }

            if (multiThreadPool == true) {
                new Task().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, WEB_REQUEST, requestMethod, url, request, data, processId, jsonResultType, autherizationRequired, filename, streamResult);
            }else {
                new Task().execute(WEB_REQUEST, requestMethod, url, request, data, processId, jsonResultType, autherizationRequired, filename, streamResult);
            }

            return null;
        }catch (IllegalStateException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DataService::request()"), ErrorSeverity.High);
            return null;
        }
    }

    private <T, U> U getRequest (
            String requestMethod,
            String url,
            T request,
            byte[] data,
            Type jsonResultType,
            boolean autherizationRequired,
            String filename,
            boolean streamResult) throws Exception {

            return webRequest(getWebClient(),
                    url,
                    requestMethod,
                    request == null ? null : new Gson().toJson(request),
                    data,
                    autherizationRequired,
                    "yyyy-MM-dd'T'HH:mm:ss",
                    jsonResultType,
                    filename,
                    streamResult);
    }

    private <T> T webRequest(
            WebClient webClient,
            String url,
            String requestMethod,
            String jsonString,
            byte[] data,
            boolean useAuthorizationKey,
            String jsonDateFormat,
            Type jsonResultType,
            String fileName,
            boolean streamResult) throws Exception {

        try {
            mWebResult = webClient.webRequest(url, requestMethod, jsonString, data, useAuthorizationKey, fileName, streamResult);

            if (mWebResult.getCode() == WebClient.SC_OK) {
                if (streamResult == true) {
                    return (T) mWebResult.getArrayResult();
                }else{
                    return fromGson(mWebResult.getResult(), jsonDateFormat, jsonResultType);
                }
            } else {
                return null;
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DataService::webRequest"), ErrorSeverity.None);
        }

        return null;
    }

    private <T> T fromGson(String jsonString, String dateFormat, Type type){

        Gson gson = null;

        if (dateFormat != null) {
            gson = new GsonBuilder().setDateFormat(dateFormat).create();
        }else{
            gson = new Gson();
        }

        return (T) gson.fromJson(jsonString, type);
    }

    private String toGson(Object object, String dateFormat){

        Gson gson = null;

        if (dateFormat != null) {
            gson = new GsonBuilder().setDateFormat(dateFormat).create();
        }else{
            gson = new Gson();
        }

        return gson.toJson(object);
    }

    private WebClient getWebClient(){

        if (mWebClient == null){
            mWebClient = new WebClient(null);
            return mWebClient;
        }

        return mWebClient;
    }
}
