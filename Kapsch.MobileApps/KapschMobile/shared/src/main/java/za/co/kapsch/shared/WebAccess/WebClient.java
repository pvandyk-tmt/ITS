package za.co.kapsch.shared.WebAccess;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;

import com.google.gson.Gson;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import za.co.kapsch.shared.LibApp;
import za.co.kapsch.shared.Models.CredentialModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.R;
import za.co.kapsch.shared.Utilities;


/**
 * Created by csenekal on 2016-08-07.
 */

public class WebClient {

    public static final int SC_OK = 200;
    private WebResult mWebResult;
    private za.co.kapsch.shared.Interfaces.IWebClientCallBack mWebClientCallBack;
    private static final int CONNECTION_NOT_AVAILABLE = -1;
    public static final String REQUEST_METHOD_GET = "GET";
    public static final String REQUEST_METHOD_POST = "POST";
    private static int HTTP_READ_TIMEOUT = 40000;
    private static int HTTP_CONNECT_TIMEOUT = 30000;

    public WebClient(za.co.kapsch.shared.Interfaces.IWebClientCallBack webClientCallBack){
        mWebClientCallBack = webClientCallBack;
    }

    private SessionModel getSession(){

        if (SessionModel.getInstance().isRequestNewSession() == false) {

            if (SessionModel.getInstance().sessionTokenExpired() == false) {
                return SessionModel.getInstance();
            }
        }

        //Gson gson = new GsonBuilder().disableHtmlEscaping().create();//= sign is encoded to \u003d. Hence you need to use disableHtmlEscaping().
        WebResult webResult = webRequest(
                CoreGatewayUrls.getSessionUrl(),
                REQUEST_METHOD_POST,
                new Gson().toJson(new CredentialModel(
                        SessionModel.getInstance().getUserName(),
                        SessionModel.getInstance().getPassword())),
                null,
                false);

        if (webResult.getCode() == SC_OK) {
            SessionModel.getInstance().setSession(new Gson().fromJson(webResult.getResult(), SessionModel.class));
            return SessionModel.getInstance();
        } else {
            return null;
        }
    }

    public WebResult webRequest(String urlString, String requestMethod, String jsonData, byte[] bufferData, boolean useAuthorizationKey) {
       return webRequest(urlString, requestMethod, jsonData, bufferData, useAuthorizationKey, null, false);
    }

    public WebResult webRequest(String urlString, String requestMethod, String jsonData, byte[] bufferData, boolean useAuthorizationKey, String outputFileName, boolean streamResult) {

        mWebResult = new WebResult();
        String authorizationKey = null;

        //TODO: check here for expired or invalid session token
        if (useAuthorizationKey == true) {
            SessionModel session = getSession();
            if (session == null) {
                return mWebResult;
            }

            authorizationKey = session.getSessionToken();
        }

        ConnectivityManager connMgr = (ConnectivityManager)LibApp.getContext().getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connMgr.getActiveNetworkInfo();

        mWebResult.clear();
        if (networkInfo != null && networkInfo.isConnected()) {
            if (streamResult == true){
                if (outputFileName == null) {
                    fileDownloadRequest(urlString, requestMethod, authorizationKey);
                }else {
                    fileDownloadRequest(urlString, requestMethod, authorizationKey, outputFileName);
                }
            }else {
                mWebResult.setResult(getRequest(urlString, requestMethod, jsonData, bufferData, authorizationKey));
            }
        }
        else {
            mWebResult.setCode(CONNECTION_NOT_AVAILABLE);
            mWebResult.setError(LibApp.getContext().getResources().getString(R.string.message_network_not_available));
            return mWebResult;
        }

        return mWebResult;
    }

    private class Task extends AsyncTask<Object, Void, String> {
        @Override
        protected String doInBackground(Object... urls) {
            return getRequest((String)urls[0], (String)urls[1], (String)urls[2], (byte[])urls[3], (String)urls[4]);
        }

        // onPostExecute displays the results of the AsyncTask.
        @Override
        protected void onPostExecute(String result) {
            mWebResult.setResult(result);
            mWebClientCallBack.webClientCallBackMethod(mWebResult);
        }
    }

    private String getRequest(String urlString, String requestMethod, String jsonData, byte[] bufferData, String authorizationKey){

        HttpURLConnection conn = null;
        InputStream inputStream = null;

        try {
            conn = getUrlConnection(urlString, requestMethod, jsonData, bufferData, authorizationKey, null);
            addBody(conn, jsonData, bufferData);
            conn.connect();

            mWebResult.setCode(conn.getResponseCode());
            if (conn.getResponseCode() == HttpURLConnection.HTTP_OK) {

                inputStream = conn.getInputStream();
                String readResult = readIt(inputStream);
                if (readResult != null) {
                    return readResult;
                }

                mWebResult.setError("getRequest(): inputStream read failed");
            }
            else{
                inputStream = conn.getErrorStream();
                mWebResult.setError(readIt(inputStream));
            }
        } catch (MalformedURLException e) {
            mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 1"));
        } catch (Exception e) {
            mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 2"));
        } finally {
            conn.disconnect();
            if (inputStream != null) {
                try {
                    inputStream.close();
                } catch (IOException e) {
                    mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 3"));
                }
            }
        }

        return null;
    }

    private boolean fileDownloadRequest(String urlString, String requestMethod, String authorizationKey, String outputFilename){

        InputStream inputStream = null;
        FileOutputStream fileOutputStream = null;

        HttpURLConnection conn = null;
        try {
            conn = getUrlConnection(urlString, requestMethod, null, null, authorizationKey, outputFilename);
            //conn.setRequestProperty("content-type", "application/octet-stream");
            //conn.setRequestProperty("Content-Disposition", "attachment; filename=iTicket.apk");
            conn.connect();

            mWebResult.setCode(conn.getResponseCode());
            if (conn.getResponseCode() == HttpURLConnection.HTTP_OK) {

                inputStream = conn.getInputStream();

                File outputFile = Utilities.getTicketFile(outputFilename);
                if (outputFile.exists()) {
                    outputFile.delete();
                }

                fileOutputStream = new FileOutputStream(outputFile);

                byte[] buffer = new byte[1024];
                int length = 0;
                while ((length = inputStream.read(buffer)) != -1) {
                    fileOutputStream.write(buffer, 0, length);
                }

                return true;
            }else{
                inputStream = conn.getErrorStream();
                mWebResult.setError(readIt(inputStream));
            }

        } catch (MalformedURLException e) {
            mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 1"));
        } catch (Exception e) {
            mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 2"));
        } finally {
            conn.disconnect();
            if (inputStream != null) {
                try {
                    inputStream.close();
                    if (fileOutputStream != null) {
                        fileOutputStream.close();
                    }
                } catch (IOException e) {
                    mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 3"));
                }
            }
        }

        return false;
    }

    private boolean fileDownloadRequest(String urlString, String requestMethod, String authorizationKey){

        InputStream inputStream = null;
        ByteArrayOutputStream outputStream = null;

        HttpURLConnection conn = null;
        try {
            conn = getUrlConnection(urlString, requestMethod, null, null, authorizationKey, null);
            conn.connect();

            mWebResult.setCode(conn.getResponseCode());
            if (conn.getResponseCode() == HttpURLConnection.HTTP_OK) {

                inputStream = conn.getInputStream();

                outputStream = new ByteArrayOutputStream();

                byte[] buffer = new byte[1024];
                int length = 0;
                while ((length = inputStream.read(buffer)) != -1) {
                    outputStream.write(buffer, 0, length);
                }

                mWebResult.setArrayResult(outputStream.toByteArray());
                return true;
            }else{
                inputStream = conn.getErrorStream();
                mWebResult.setError(readIt(inputStream));
            }

        } catch (MalformedURLException e) {
            mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 1"));
        } catch (Exception e) {
            mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 2"));
        } finally {
            conn.disconnect();
            if (inputStream != null) {
                try {
                    inputStream.close();
                    if (outputStream != null) {
                        outputStream.close();
                    }
                } catch (IOException e) {
                    mWebResult.setError(Utilities.exceptionMessage(e, "getRequest() 3"));
                }
            }
        }

        return false;
    }

    private HttpURLConnection getUrlConnection(String urlString, String requestMethod, String jsonData, byte[] bufferData, String authorizationKey, String filename) throws IOException {

        URL url = new URL(urlString);
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();

        if (bufferData != null){
            conn.setUseCaches(false);
            conn.setRequestProperty("Connection", "Keep-Alive");
            conn.setRequestProperty("Cache-Control", "no-cache");
            conn.setRequestProperty("Content-Length", Integer.toString(bufferData.length));
        }

        if (jsonData != null) {
            conn.setRequestProperty("Content-Length", Integer.toString(jsonData.getBytes().length));
            conn.setRequestProperty("content-type", "application/json; charset=utf-8");
        }

        if (filename != null){
            conn.setRequestProperty("content-type", "application/octet-stream");
            conn.setRequestProperty("Content-Disposition", String.format("attachment; filename=%s", filename));
        }

        if (authorizationKey != null) {
             conn.setRequestProperty("Authorization", String.format("SessionToken %s", authorizationKey));
        }

        conn.setReadTimeout(HTTP_READ_TIMEOUT);
        conn.setConnectTimeout(HTTP_CONNECT_TIMEOUT);
        conn.setRequestMethod(requestMethod);

        if (requestMethod.equals(REQUEST_METHOD_POST)) {
            conn.setDoOutput(true);
        }

        return conn;
    }

    private void addBody(HttpURLConnection conn, String jsonData, byte[] bufferData) throws Exception {

        OutputStream outputStream = null;
        DataOutputStream dataOutputStream = null;

        try {
            BufferedWriter writer = null;

            if (bufferData != null) {
                dataOutputStream = new DataOutputStream(conn.getOutputStream());
            } else if (jsonData != null) {
                outputStream = conn.getOutputStream();
                writer = new BufferedWriter(new OutputStreamWriter(outputStream, "UTF-8"));
            }

            try {
                if (bufferData != null) {
                    dataOutputStream.write(bufferData);
                } else if (jsonData != null) {
                    writer.write(jsonData);
                }
            }finally{
                if (writer != null) {
                    writer.flush();
                    writer.close();
                }
            }

        } finally {
            if (outputStream != null) {
                outputStream.close();
            }
            if (dataOutputStream != null) {
                dataOutputStream.close();
            }
        }
    }

//if you were downloading image data, you might decode and display it like this:
//    InputStream is = null;
//    ...
//    Bitmap bitmap = BitmapFactory.decodeStream(is);
//    ImageView imageView = (ImageView) findViewById(R.id.image_view);
//    imageView.setImageBitmap(bitmap);

    private String readIt(InputStream is) throws Exception {
        try {
            if (is != null) {
                BufferedReader reader = new BufferedReader(new InputStreamReader(is, "UTF-8"), 8);

                StringBuilder sb = new StringBuilder();
                String line;
                while ((line = reader.readLine()) != null) {
                    sb.append(line).append("\n");
                }

                return sb.toString();
            }
        }finally {
            is.close();
        }
        return null;
    }
}

