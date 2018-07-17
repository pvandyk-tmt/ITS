package za.co.kapsch.iticket.iCam;

import android.app.Activity;
import android.graphics.Bitmap;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.Constants;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/08/17.
 */
public class ICamClient {

    private static ICamClient mInstance;

    public static final int SUCCESS = 1;
    public static final int FAILED = 0;

    public static final int REACHABLE = 1;
    public static final int CONNECT = 2;
    public static final int PASSAGE = 3;
    public static final int EVENT = 4;
    public static final int MONITOR_VEHICLES = 5;
    public static final int READ_SOCKET = 6;
    public static final int CLOSE_SOCKET = 7;

    private static final String xmlClosingTag = "/>";
    private static final String vlnsTag = "vlns";
    private static final String infringementTag = "infringement";

    private Socket mSocket;
    private Handler mHandler;
    private Activity mActivity;
    private double mVlnsTimeStamp;
    private double mInfringementTimeStamp;
    private IAsyncProcessCallBack mAsyncProcessCallBack;

    private List<String> mICamXmlStreamList;

    public synchronized static ICamClient getInstance(IAsyncProcessCallBack asyncProcessCallBack, Activity activity)
    {
        if (mInstance == null)
        {
            mInstance = new ICamClient(asyncProcessCallBack, activity);
        }else{
            mInstance.setActivity(activity);
            mInstance.setAsyncProcessCallBack(asyncProcessCallBack);
        }

        return mInstance;
    }

    public void setActivity(Activity activity){
        mActivity = activity;
    }

    public void setAsyncProcessCallBack( IAsyncProcessCallBack asyncProcessCallBack){
        mAsyncProcessCallBack = asyncProcessCallBack;
    }

    public ICamClient(IAsyncProcessCallBack asyncProcessCallBack, Activity activity){

        mActivity = activity;
        mAsyncProcessCallBack = asyncProcessCallBack;

        mHandler = new Handler(Looper.getMainLooper()) {
            @Override
            public void handleMessage(Message inputMessage) {

                switch (inputMessage.what) {
                    case REACHABLE:
                        handleReachable(inputMessage);
                        break;
                    case CONNECT:
                        handleConnect(inputMessage);
                        break;
                    case PASSAGE:
                        handlePassage(inputMessage);
                        break;
                    case EVENT:
                        handleEvent(inputMessage);
                        break;
                    case CLOSE_SOCKET:
                        handleCloseSocket(inputMessage);
                        break;
                    case MONITOR_VEHICLES:
                        handleMonitorVehicles(inputMessage);
                        break;
                    default:
                        super.handleMessage(inputMessage);
                }
            }
        };
    }

    private void handleEvent(Message inputMessage){
        switch (inputMessage.arg1) {
            case SUCCESS:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(SUCCESS, inputMessage.obj, null, EVENT));
                break;
            case FAILED:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(FAILED, null, (String) inputMessage.obj, EVENT));
        }
    }

    private void handleConnect(Message inputMessage){
        switch (inputMessage.arg1) {
            case SUCCESS:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(SUCCESS, inputMessage.obj, null, CONNECT));
                break;
            case FAILED:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(FAILED, null, (String) inputMessage.obj, CONNECT));
        }
    }

    private void handlePassage(Message inputMessage){
        switch (inputMessage.arg1) {
            case SUCCESS:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(SUCCESS, null, null, PASSAGE));
                break;
            case FAILED:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(FAILED, null, (String) inputMessage.obj, PASSAGE));
        }
    }

    private void handleCloseSocket(Message inputMessage){
        switch (inputMessage.arg1) {
            case SUCCESS:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(SUCCESS, null, null, CLOSE_SOCKET));
                break;
            case FAILED:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(FAILED, null, (String) inputMessage.obj, CLOSE_SOCKET));
        }
    }

    private void handleReachable(Message inputMessage){
        switch (inputMessage.arg1) {
            case SUCCESS:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(SUCCESS, null, null, REACHABLE));
                break;
            case FAILED:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(FAILED, null, null, REACHABLE));
                break;
        }
    }

    private void handleMonitorVehicles(Message inputMessage){
        switch (inputMessage.arg1) {
            case SUCCESS:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(SUCCESS, null, null, MONITOR_VEHICLES));
                break;
            case FAILED:
                mAsyncProcessCallBack.progressCallBack(new AsyncResultModel(FAILED, null, null, MONITOR_VEHICLES));
                break;
        }
    }

    public void run(final String ipAddress, final int port) {

        Utilities.busyProgressBarEx(mActivity, true);

        new Thread(new Runnable() {
            public void run() {
                monitorVehicles(ipAddress, port);
            }
        }).start();
     }

    private void monitorVehicles(final String ipAddress, final int port){

        mSocket = null;

        try {
            if (isReachable(ipAddress, port, 5000) == false) {
                publishFailure(REACHABLE, null);
                return;
            }

            mSocket = connect(ipAddress, port);
            monitorConnection(ipAddress, port);
            readSocket(ipAddress);

        }catch (Exception e) {
            publishFailure(MONITOR_VEHICLES, Utilities.exceptionMessage(e, "ICamClientEx::monitor()"));
        } finally {
            closeSocket(mSocket);
        }
    }

    private void monitorConnection(final String ipAddress, final int port){

        new Thread(new Runnable() {
           public void run() {
                try {
                    while (mSocket.isClosed() == false) {
                        if (isReachable(ipAddress, port, 5000) == true) {
                            Thread.sleep(5000);
                            if (mSocket.isConnected() == true) {
                                publishSuccess(REACHABLE, null);
                            }
                        } else {
                            if (mSocket.isConnected() == true) {
                                publishFailure(REACHABLE, null);
                            }
                        }
                    }

                    publishFailure(REACHABLE, null);

                } catch (Exception e) {
                    publishFailure(REACHABLE, Utilities.exceptionMessage(e, "ICamClientEx::monitorConnection()"));
                }
            }
        }).start();
    }

    private boolean isReachable(String addr, int openPort, int timeOutMillis) {

        try (Socket socket = new Socket()) {
            socket.connect(new InetSocketAddress(addr, openPort), timeOutMillis);
            return true;
        } catch (IOException e) {
            return false;
        } catch (Exception e) {
            return false;
        }
    }

    private Socket connect(String ipAddress, int port) {
        try {
            InetAddress serverAddr;
            serverAddr = InetAddress.getByName(ipAddress);
            Socket socket = new Socket(serverAddr, port);
            socket.setSoTimeout(0);
            publishSuccess(CONNECT, null);
            return socket;
        } catch (IOException e) {
            publishFailure(CONNECT, Utilities.exceptionMessage(e, "ICamClientEx::connect() 1"));
        } catch (Exception e) {
            publishFailure(CONNECT, Utilities.exceptionMessage(e, "ICamClientEx::connect() 2"));
        }
        return null;
    }

//    private void readSocket(String ipAddress){
//
//        InputStream inputStream = null;
//        ByteArrayOutputStream outputStream = null;
//
//        try {
//            inputStream = mSocket.getInputStream();
//            outputStream = new ByteArrayOutputStream(1024);
//            byte[] buffer = new byte[1024];
//
//            int bytesRead;
//
//            mVlnsTimeStamp = 0d;
//            mInfringementTimeStamp = 0d;
//
//            while( mSocket.isConnected() == true) {
//                if (inputStream.available() > 0) {
//                    if (((bytesRead = inputStream.read(buffer)) != -1)) {
//                        outputStream.write(buffer, 0, bytesRead);
//                        String response = outputStream.toString("UTF-8");
//                        String[] list = response.split("\n");
//                        promoteNextItemInList(ipAddress, list);
//                    }
//                }
//            }
//        } catch (IOException e) {
//            if (Utilities.exceptionMessage(e, Constants.EMPTY_STRING).contains("SocketTimeoutException") == true){
//                return;
//            }else {
//                publishFailure(READ_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::readSocket() 1"));
//            }
//        } catch (Exception e) {
//            publishFailure(READ_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::readSocket() 2"));
//        } finally {
//            closeStreams(inputStream, outputStream);
//        }
//    }

    private void readSocket(String ipAddress){

        String response = null;
        InputStream inputStream = null;
        ByteArrayOutputStream outputStream = null;

        try {
            inputStream = mSocket.getInputStream();
            //outputStream = new ByteArrayOutputStream(1024);
            byte[] buffer = new byte[1024];

            int bytesRead;

            mVlnsTimeStamp = 0d;
            mInfringementTimeStamp = 0d;

            outputStream = new ByteArrayOutputStream(1024);

            while( mSocket.isConnected() == true) {

                if (inputStream.available() > 0) {
                    if (((bytesRead = inputStream.read(buffer)) != -1)) {
                        outputStream.write(buffer, 0, bytesRead);
                        response = outputStream.toString("UTF-8");
                        String[] list = response.split("\n");

                        addTags(list);

                        //promoteNextItemInList(ipAddress, list);
                        manageICamXmlStreamListSize(mICamXmlStreamList);
                        promoteNextItemInListEx(ipAddress, mICamXmlStreamList);
                    }
                }
                //start new output stream after every vln tag read
                if (response != null) {
                    if (response.contains("vlns")) {
                        if (outputStream != null) {
                            outputStream.close();
                            outputStream = new ByteArrayOutputStream(1024);
                        }
                    }
                }

            }
        } catch (IOException e) {
            if (Utilities.exceptionMessage(e, Constants.EMPTY_STRING).contains("SocketTimeoutException") == true){
                return;
            }else {
                publishFailure(READ_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::readSocket() 1"));
            }
        } catch (Exception e) {
            publishFailure(READ_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::readSocket() 2"));
        } finally {
            closeStreams(inputStream, outputStream);
            closeSocket(mSocket);
        }
    }

    private void addTags(String[] list){

        if ( mICamXmlStreamList == null){
            mICamXmlStreamList = new ArrayList<>();
        }

        for (int index = 0 ; index < list.length; index++) {

            if (hasStartTag(list[index]) && hasClosingTag(list[index])){
                mICamXmlStreamList.add(list[index]);
            }

            if (hasStartTag(list[index]) && hasClosingTag(list[index]) == false){
                mICamXmlStreamList.add(list[index]);
            }

            if ((hasStartTag(list[index]) == false) && hasClosingTag(list[index])){
               //If element is first element in list then concatonate to last element in ICamXmlStreamList
                if (index == 0){
                    String partialElement = mICamXmlStreamList.get(mICamXmlStreamList.size()-1);
                    partialElement = String.format("%s%s", partialElement, list[index]);
                    mICamXmlStreamList.set(mICamXmlStreamList.size()-1, partialElement);
                }
            }
        }
    }

    private void manageICamXmlStreamListSize(List<String> iCamXmlStreamList){

        while (iCamXmlStreamList.size() > 20){
            iCamXmlStreamList.remove(0);
        }
    }

    private boolean hasStartTag(String tag){
        return (tag.contains(vlnsTag) || tag.contains(infringementTag));
    }

    private boolean hasClosingTag(String tag){
        return tag.contains(xmlClosingTag);
    }

    private void closeSocket(Socket socket){
        try {
            if (socket != null) {
                socket.close();
                //socket = null;
                publishSuccess(CLOSE_SOCKET, null);
            }
            else{
                publishSuccess(CLOSE_SOCKET, null);
            }
        } catch (IOException e) {
            publishFailure(CLOSE_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::closeSocket() 1"));
        } catch (Exception e) {
            publishFailure(CLOSE_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::closeSocket() 2"));
        }
    }

    private void closeStreams(InputStream inputStream, ByteArrayOutputStream outputStream){
        try {
            if (inputStream != null){
                inputStream.close();
            }

            if (outputStream != null){
                outputStream.close();
            }
        } catch (IOException e) {
            publishFailure(CLOSE_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::closeStreams() 1"));
        } catch (Exception e) {
            publishFailure(CLOSE_SOCKET, Utilities.exceptionMessage(e, "ICamClientEx::closeStreams() 2"));
        }
    }

    private void promoteNextItemInListEx(final String ipAddress, List<String> list){

        try {
            double vlnsTimeStamp;
            double infringementTimeStamp;

            for (String item : list) {
                if (item.contains("vlns")) {

                    final ICamVlns iCamVlns = Utilities.deserializeXml("vlns", ICamVlns.class, new ICamVlnsConverter(), item);
                    vlnsTimeStamp = Double.parseDouble(iCamVlns.getTimestamp());
                    if (vlnsTimeStamp > mVlnsTimeStamp) {
                        mVlnsTimeStamp = vlnsTimeStamp;
                        publishSuccess(PASSAGE, null);
                        if (iCamVlns.hasVosi() == true) {
                            getThumbImage(ipAddress, new ICamEvent(iCamVlns, null));
                        }
                    }

                } else if (item.contains("infringement")) {

                    final ICamInfringement iCamInfringement = Utilities.deserializeXml("infringement", ICamInfringement.class, new ICamInfringementConverter(), item);
                    infringementTimeStamp = Double.parseDouble(iCamInfringement.getTimestamp());
                    if (infringementTimeStamp > mInfringementTimeStamp) {
                        //
                        ICamVlns iCamVlns = findInfringementVlnsEx(list, infringementTimeStamp);
                        //
                        if (iCamVlns != null) {
                            mInfringementTimeStamp = infringementTimeStamp;
                            publishSuccess(PASSAGE, null);
                            iCamInfringement.setIcamVlns(findInfringementVlnsEx(list, infringementTimeStamp));
                            getThumbImage(ipAddress, new ICamEvent(null, iCamInfringement));
                        }
                    }
                }
            }
        }catch (Exception e){
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::promoteNextItemInList()"));
        }
    }

//    private void promoteNextItemInList(final String ipAddress, String[] list){
//
//        try {
//            double vlnsTimeStamp;
//            double infringementTimeStamp;
//
//            for (String item : list) {
//                if (item.contains("vlns")) {
//
//                    final ICamVlns iCamVlns = Utilities.deserializeXml("vlns", ICamVlns.class, new ICamVlnsConverter(), item);
//                    vlnsTimeStamp = Double.parseDouble(iCamVlns.getTimestamp());
//                    if (vlnsTimeStamp > mVlnsTimeStamp) {
//                        mVlnsTimeStamp = vlnsTimeStamp;
//                        publishSuccess(PASSAGE, null);
//                        if (iCamVlns.hasVosi() == true) {
//                            getImage(ipAddress, new ICamEvent(iCamVlns, null));
//                        }
//                    }
//
//                } else if (item.contains("infringement")) {
//
//                    final ICamInfringement iCamInfringement = Utilities.deserializeXml("infringement", ICamInfringement.class, new ICamInfringementConverter(), item);
//                    infringementTimeStamp = Double.parseDouble(iCamInfringement.getTimestamp());
//                    if (infringementTimeStamp > mInfringementTimeStamp) {
//
//                        //
//                        ICamVlns iCamVlns = findInfringementVlns(list, infringementTimeStamp);
//                        //
//                        if (iCamVlns != null) {
//                            mInfringementTimeStamp = infringementTimeStamp;
//                            publishSuccess(PASSAGE, null);
//                            iCamInfringement.setIcamVlns(iCamVlns);
//                            getImage(ipAddress, new ICamEvent(null, iCamInfringement));
//                        }
//                    }
//                }
//            }
//        }catch (Exception e){
//            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::promoteNextItemInList()"));
//        }
//    }

//    private ICamVlns findInfringementVlns(String[] list, double infringementTimeStamp){
//
//        double vlnsTimeStamp;
//
//        for (String item : list) {
//            if (item.contains("vlns")) {
//
//                final ICamVlns iCamVlns = Utilities.deserializeXml("vlns", ICamVlns.class, new ICamVlnsConverter(), item);
//                vlnsTimeStamp = Double.parseDouble(iCamVlns.getTimestamp());
//                if (vlnsTimeStamp == infringementTimeStamp) {
//                    return iCamVlns;
//                }
//            }
//        }
//
//        return null;
//    }

    private ICamVlns findInfringementVlnsEx(List<String> list, double infringementTimeStamp){

        double vlnsTimeStamp;

        for (String item : list) {
            if (item.contains("vlns")) {

                final ICamVlns iCamVlns = Utilities.deserializeXml("vlns", ICamVlns.class, new ICamVlnsConverter(), item);
                vlnsTimeStamp = Double.parseDouble(iCamVlns.getTimestamp());
                if (vlnsTimeStamp == infringementTimeStamp) {
                    return iCamVlns;
                }
            }
        }

        return null;
    }

//    private Bitmap getImageEx(String ipAddress, ICamEvent iCamEvent){
//
//        Socket socket = null;
//        InetAddress serverAddr = null;
//        InputStream inputStream = null;
//        OutputStream outputStream = null;
//        ByteArrayOutputStream byteArrayOutputStream = null;
//
//        try {
//            serverAddr = InetAddress.getByName(ipAddress);
//            socket = new Socket(serverAddr, 80);
//            socket.setSoTimeout(60000);//0 is no time out
//
//            String data = null;
//            if (iCamEvent.getICamInfringement() != null) {
//                data = "GET " + iCamEvent.getICamInfringement().getFilename() + System.getProperty("line.separator") + System.getProperty("line.separator");
//            }
//
//            if (iCamEvent.getICamVlns() != null) {
//                data = "GET " + iCamEvent.getICamVlns().getFilename() + System.getProperty("line.separator") + System.getProperty("line.separator");
//            }
//
//            outputStream = socket.getOutputStream();
//            PrintWriter output = new PrintWriter(outputStream);
//            output.println(data);
//            output.flush();
//
//            byteArrayOutputStream = new ByteArrayOutputStream(1024);
//            byte[] buffer = new byte[1024];
//
//            int bytesRead;
//            inputStream = socket.getInputStream();
//
//            while ((bytesRead = inputStream.read(buffer)) != -1) {
//                if (mSocket == null){
//                    break;
//                }
//                byteArrayOutputStream.write(buffer, 0, bytesRead);
//            }
//
//            if (iCamEvent.getICamInfringement() != null) {
//                //iCamEvent.getICamInfringement().setImage(Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray()));
//                return Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray());
//            }
//
//            if (iCamEvent.getICamVlns() != null) {
//                //iCamEvent.getICamVlns().setImage(Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray()));
//                return Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray());
//            }
//
//            //publishSuccess(EVENT, iCamEvent);
//
//        } catch (UnknownHostException e) {
//            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 1"));
//        } catch (IOException e) {
//            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 2"));
//        } catch (Exception e) {
//            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 3"));
//        }finally {
//            try {
//                if (socket != null) {
//                    socket.close();
//                }
//                if (inputStream != null) {
//                    inputStream.close();
//                }
//                if (outputStream != null) {
//                    outputStream.close();
//                }
//                if (byteArrayOutputStream!= null) {
//                    byteArrayOutputStream.close();
//                }
//            } catch (IOException e) {
//                publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 4"));
//            }
//        }
//
//        return null;
//    }

    private void getThumbImage(String ipAddress, ICamEvent iCamEvent){

        Socket socket = null;
        InetAddress serverAddr = null;
        InputStream inputStream = null;
        OutputStream outputStream = null;
        ByteArrayOutputStream byteArrayOutputStream = null;

        try {
            serverAddr = InetAddress.getByName(ipAddress);
            socket = new Socket(serverAddr, 80);
            socket.setSoTimeout(60000);//0 is no time out

            String data = null;
            if (iCamEvent.getICamInfringement() != null) {
                data = "GET " + iCamEvent.getICamInfringement().getFilename() + System.getProperty("line.separator") + System.getProperty("line.separator");
            }

            if (iCamEvent.getICamVlns() != null) {
                data = "GET " + iCamEvent.getICamVlns().getFilename() + System.getProperty("line.separator") + System.getProperty("line.separator");
            }

            outputStream = socket.getOutputStream();
            PrintWriter output = new PrintWriter(outputStream);
            output.println(data);
            output.flush();

            byteArrayOutputStream = new ByteArrayOutputStream(1024);
            byte[] buffer = new byte[1024];

            int bytesRead;
            inputStream = socket.getInputStream();

            while ((bytesRead = inputStream.read(buffer)) != -1) {
                if (mSocket == null){
                    break;
                }
                byteArrayOutputStream.write(buffer, 0, bytesRead);
            }

            if (iCamEvent.getICamInfringement() != null) {
                iCamEvent.getICamInfringement().setThumbImage(Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray()));
            }

            if (iCamEvent.getICamVlns() != null) {
                iCamEvent.getICamVlns().setThumbImage(Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray()));
            }

            publishSuccess(EVENT, iCamEvent);

        } catch (UnknownHostException e) {
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 1"));
        } catch (IOException e) {
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 2"));
        } catch (Exception e) {
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 3"));
        }finally {
            try {
                if (socket != null) {
                    socket.close();
                }
                if (inputStream != null) {
                    inputStream.close();
                }
                if (outputStream != null) {
                    outputStream.close();
                }
                if (byteArrayOutputStream!= null) {
                    byteArrayOutputStream.close();
                }
            } catch (IOException e) {
                publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 4"));
            }
        }
    }

    private void getImage(String ipAddress, ICamEvent iCamEvent){

        Socket socket = null;
        InetAddress serverAddr = null;
        InputStream inputStream = null;
        OutputStream outputStream = null;
        ByteArrayOutputStream byteArrayOutputStream = null;

        try {
            serverAddr = InetAddress.getByName(ipAddress);
            socket = new Socket(serverAddr, 80);
            socket.setSoTimeout(60000);//0 is no time out

            String data = null;
            if (iCamEvent.getICamInfringement() != null) {
                data = "GET " + iCamEvent.getICamInfringement().getFilename() + System.getProperty("line.separator") + System.getProperty("line.separator");
            }

            if (iCamEvent.getICamVlns() != null) {
                data = "GET " + iCamEvent.getICamVlns().getFilename() + System.getProperty("line.separator") + System.getProperty("line.separator");
            }

            outputStream = socket.getOutputStream();
            PrintWriter output = new PrintWriter(outputStream);
            output.println(data);
            output.flush();

            byteArrayOutputStream = new ByteArrayOutputStream(1024);
            byte[] buffer = new byte[1024];

            int bytesRead;
            inputStream = socket.getInputStream();

            while ((bytesRead = inputStream.read(buffer)) != -1) {
                if (mSocket == null){
                    break;
                }
                byteArrayOutputStream.write(buffer, 0, bytesRead);
            }

            if (iCamEvent.getICamInfringement() != null) {
                iCamEvent.getICamInfringement().setImage(Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray()));
            }

            if (iCamEvent.getICamVlns() != null) {
                iCamEvent.getICamVlns().setImage(Utilities.byteArrayToBitmap(byteArrayOutputStream.toByteArray()));
            }

            publishSuccess(EVENT, iCamEvent);

        } catch (UnknownHostException e) {
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 1"));
        } catch (IOException e) {
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 2"));
        } catch (Exception e) {
            publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 3"));
        }finally {
            try {
                if (socket != null) {
                    socket.close();
                }
                if (inputStream != null) {
                    inputStream.close();
                }
                if (outputStream != null) {
                    outputStream.close();
                }
                if (byteArrayOutputStream!= null) {
                    byteArrayOutputStream.close();
                }
            } catch (IOException e) {
                publishFailure(EVENT, Utilities.exceptionMessage(e, "ICamClientEx::getImage() 4"));
            }
        }
    }

    private void publishSuccess(int what, Object object) {
        Message completeMessage = mHandler.obtainMessage(what, SUCCESS, 0, object);
        completeMessage.sendToTarget();
    }

    private void publishFailure(int what, Object object) {
        Message completeMessage = mHandler.obtainMessage(what, FAILED, 0, object);
        completeMessage.sendToTarget();
    }

    public boolean closeSocket(boolean hideProgressBar){

        if (mSocket != null) {
            if (mSocket.isConnected()) {
                closeSocket(mSocket);
                if (hideProgressBar == true) {
                    if (mActivity != null) {
                        Utilities.busyProgressBarEx(mActivity, false);
                    }
                }
                return true;
            }
        }

        return false;
    }
}

