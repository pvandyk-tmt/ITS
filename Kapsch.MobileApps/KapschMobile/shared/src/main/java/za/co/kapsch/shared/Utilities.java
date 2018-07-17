package za.co.kapsch.shared;


import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.media.Ringtone;
import android.media.RingtoneManager;
import android.net.Uri;
import android.net.wifi.WifiManager;
import android.os.Environment;
import android.os.Parcel;
import android.provider.Settings;
import android.support.v4.app.Fragment;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;

import android.telephony.TelephonyManager;
import android.text.TextUtils;
import android.util.Base64;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.Adapter;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.graphics.Paint;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.MultiFormatWriter;
import com.google.zxing.WriterException;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.common.StringUtils;
import com.journeyapps.barcodescanner.BarcodeEncoder;
import com.thoughtworks.xstream.XStream;
import com.thoughtworks.xstream.converters.Converter;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.lang.reflect.Method;
import java.nio.ByteBuffer;
import java.nio.channels.FileChannel;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.SQLException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.logging.ErrorManager;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.PaymentContext;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserActivityLogModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.orm.UserActivityLogRepository;

/**
 * Created by csenekal on 2016-07-17.
 */
public class Utilities {

    public static final String APP_DATA = "AppData";
    private final static char[] hexArray = "0123456789ABCDEF".toCharArray();
    private static String EMPTY_STRING = "";
    private static final String ITICKET_FOLDER = "TMT iTicket";
    private static final String SAVED_PRINTER_FIELD_SEPERATOR = "-seperator-";
    private static final String PRINTER_MAC_ADDRESS = "printerMacAddress";

    public static String getString(int resourceId) {
        return LibApp.getContext().getResources().getString(resourceId);
    }

    public static void displayOkMessage(String message, Activity activity) {
        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(activity);
        dlgAlert.setMessage(message);
        dlgAlert.setTitle("App message...");
        dlgAlert.setPositiveButton("OK", null);
        dlgAlert.setCancelable(true);
        dlgAlert.create().show();
    }

    public static void displayOkMessageEx(String message, Context context, DialogInterface.OnClickListener OnClickListener) {
        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(context);
        dlgAlert.setMessage(message);
        dlgAlert.setTitle("App message...");
        dlgAlert.setPositiveButton("Ok", OnClickListener);
        dlgAlert.setCancelable(false);
        dlgAlert.create().show();
    }

//    public static void displayDecisionMessage(String message, Fragment fragment) {
//        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(fragment.getActivity());
//        dlgAlert.setMessage(message);
//        dlgAlert.setTitle("App message...");
//        dlgAlert.setNegativeButton("No", (DialogInterface.OnClickListener) fragment);
//        dlgAlert.setPositiveButton("Yes", (DialogInterface.OnClickListener) fragment);
//        dlgAlert.setCancelable(true);
//        dlgAlert.create().show();
//    }

    public static void displayDecisionMessage(String message, Context context, DialogInterface.OnClickListener OnClickListener) {
        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(context);
        dlgAlert.setMessage(message);
        dlgAlert.setTitle("App message...");
        dlgAlert.setNegativeButton("No", OnClickListener);
        dlgAlert.setPositiveButton("Yes", OnClickListener);
        dlgAlert.setCancelable(true);
        dlgAlert.create().show();
    }

    public static void displayDecisionMessage(String message, Activity activity) {
        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(activity);
        dlgAlert.setMessage(message);
        dlgAlert.setTitle("App message...");
        dlgAlert.setNegativeButton("No", (DialogInterface.OnClickListener) activity);
        dlgAlert.setPositiveButton("Yes", (DialogInterface.OnClickListener) activity);
        dlgAlert.setCancelable(true);
        dlgAlert.create().show();
    }

    public static byte[] readFileFromAssets(String fileName, Activity activity) throws IOException {
        InputStream stream = null;
        try {
            stream = activity.getAssets().open(fileName);
            int size = stream.available();
            byte[] buffer = new byte[size];
            stream.read(buffer);
            return buffer;
        } catch (IOException e) {
            return null;
        } finally {
            if (stream != null) {
                stream.close();
            }
        }
    }

    public static boolean SavePrinterDetails(String friendlyName, String macAddress) {
        String currentPrinter = String.format("%s%s%s", friendlyName, SAVED_PRINTER_FIELD_SEPERATOR, macAddress);
        return Utilities.addStringToSharedPreference(PRINTER_MAC_ADDRESS, currentPrinter);
    }

    public static boolean printerConfigured() {
        String[] details = Utilities.getPrinterDetails();
        return details.length == 2;
    }

    public static boolean printerConfiguredEx() {
        return (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false);
   }

    public static String[] getPrinterDetails() {
        String currentPrinter = Utilities.getStringFromSharedPreference(PRINTER_MAC_ADDRESS);
        return currentPrinter.split(SAVED_PRINTER_FIELD_SEPERATOR);
    }

    public static String getStringFromSharedPreference(String key) {
        SharedPreferences appData = LibApp.getContext().getSharedPreferences(APP_DATA, Context.MODE_PRIVATE);
        return appData.getString(key, "");
    }

    public static boolean addStringToSharedPreference(String key, String value) {
        SharedPreferences sharedpreferences = LibApp.getContext().getSharedPreferences(APP_DATA, Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedpreferences.edit();
        editor.putString(key, value);
        return editor.commit();
    }

    public static String dateToString(Date date) {
        if (date == null) return null;
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.DATE_FORMAT);
        return simpleDateFormat.format(date);
    }

    public static String dateTimeToString(Date date) {
        if (date == null) return null;
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.DATETIME_FORMAT);
        return simpleDateFormat.format(date);
    }

    public static String timeToString(Date date) {
        if (date == null) return null;
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.TIME_FORMAT);
        return simpleDateFormat.format(date);
    }

    public static Date stringToDate(String date, String format) throws ParseException {
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(format);

        Date value = simpleDateFormat.parse(date);

        return simpleDateFormat.parse(date);
    }

    //get current date as follows
    //year = Calendar.get(Calendar.YEAR);
    //month = mCalendar.get(Calendar.MONTH);
    //day = mCalendar.get(Calendar.DAY_OF_MONTH) + Constants.DAYS_60; //add 60 days to date
    public static Date getDate(int year, int month, int day, int hour, int minute, int second) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.YEAR, year);
        calendar.set(Calendar.MONTH, month);
        calendar.set(Calendar.DAY_OF_MONTH, day);
        calendar.set(Calendar.HOUR_OF_DAY, hour);
        calendar.set(Calendar.MINUTE, minute);
        calendar.set(Calendar.SECOND, second);
        return calendar.getTime();
    }

    public static Date addDaysToDate(int days) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.DAY_OF_MONTH, calendar.get(Calendar.DAY_OF_MONTH) + days);
        return calendar.getTime();
    }

    public static Calendar getCalendarAddDays(int days) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.DAY_OF_MONTH, calendar.get(Calendar.DAY_OF_MONTH) + days);
        return calendar;
    }

    public static Calendar getCalendar(int year, int month, int day, int hour, int minute, int second) {

        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.YEAR, year < 200 ? year + 1900 : year);
        calendar.set(Calendar.MONTH, month);
        calendar.set(Calendar.DAY_OF_MONTH, day);
        calendar.set(Calendar.HOUR_OF_DAY, hour);
        calendar.set(Calendar.MINUTE, minute);
        calendar.set(Calendar.SECOND, second);
        return calendar;
    }

    public static Calendar getCalendarDate(Date date) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.YEAR, date.getYear() < 200 ? date.getYear() + 1900 : date.getYear());
        calendar.set(Calendar.MONTH, date.getMonth());
        calendar.set(Calendar.DAY_OF_MONTH, date.getDate());
        calendar.set(Calendar.HOUR_OF_DAY, date.getHours());
        calendar.set(Calendar.MINUTE, date.getMinutes());
        calendar.set(Calendar.SECOND, date.getSeconds());
        return calendar;
    }

    public static Date addDaysToDate(Date date, int days) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.YEAR, date.getYear() < 200 ? date.getYear() + 1900 : date.getYear());
        calendar.set(Calendar.MONTH, date.getMonth());
        calendar.set(Calendar.DAY_OF_MONTH, date.getDate());
        calendar.set(Calendar.HOUR_OF_DAY, date.getHours());
        calendar.set(Calendar.MINUTE, date.getMinutes());
        calendar.set(Calendar.SECOND, date.getSeconds());
        calendar.set(Calendar.DAY_OF_MONTH, calendar.get(Calendar.DAY_OF_MONTH) + days);
        return calendar.getTime();
    }

    public static Date getDateAddMinutes(int minutes) {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.MINUTE, calendar.get(Calendar.MINUTE) + minutes);
        return calendar.getTime();
    }

    public static String byteArrayToString(byte[] data) {

        String result;
        try {
            result = new String(data, "UTF-8");
        } catch (UnsupportedEncodingException e) {

            result = "";
            for (int i = 0; i < data.length; i++)
                result = result + (char) data[i];
        }
        return result;
    }

    public static String bytesToHex(byte[] bytes) {
        char[] hexChars = new char[bytes.length * 2];
        for (int j = 0; j < bytes.length; j++) {
            int v = bytes[j] & 0xFF;
            hexChars[j * 2] = hexArray[v >>> 4];
            hexChars[j * 2 + 1] = hexArray[v & 0x0F];
        }
        return new String(hexChars);
    }

    public static void playNotificationTone() {
        try {
            Uri notification = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
            Ringtone r = RingtoneManager.getRingtone(LibApp.getContext(), notification);
            r.play();
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "playNotificationTone"), ErrorSeverity.High);
        }
    }

//    public static String getDeviceId() {
//        return "359711071680978";
//    }
//
//    public static String getDeviceSerialNumber() {
//        return "CQGQ8T9SL7E6ZSMR";
//    }

    public static String getDeviceId() {

        TelephonyManager telephonyManager = (TelephonyManager) LibApp.getContext().getSystemService(Context.TELEPHONY_SERVICE);
        String deviceId = telephonyManager.getDeviceId();

        if (deviceId == null) {
            deviceId = Settings.Secure.getString(LibApp.getContext().getContentResolver(), Settings.Secure.ANDROID_ID);
        }

        if (deviceId == null) {
            MessageManager.showMessage("getDeviceId() failed", ErrorSeverity.High);
        }

        return deviceId;
    }

    public static String getDeviceSerialNumber() {
        String serial = null;

        try {
            Class<?> c = Class.forName("android.os.SystemProperties");
            Method get = c.getMethod("get", String.class);
            serial = (String) get.invoke(c, "ro.serialno");
            return serial;
        } catch (Exception ignored) {
            return null;
        }
    }

    public static void showKeyboard(Activity activity) {
        InputMethodManager inputMethodManager = (InputMethodManager) activity.getSystemService(Context.INPUT_METHOD_SERVICE);
        //inputMethodManager.showSoftInput(InputMethodManager.SHOW_FORCED, InputMethodManager.HIDE_IMPLICIT_ONLY);
        inputMethodManager.toggleSoftInput(InputMethodManager.SHOW_FORCED, InputMethodManager.HIDE_IMPLICIT_ONLY);
    }

    public static void hideKeyboard(Activity activity) {
        InputMethodManager inputMethodManager = (InputMethodManager) activity.getSystemService(Activity.INPUT_METHOD_SERVICE);
        //Find the currently focused view, so we can grab the correct window token from it.
        View view = activity.getCurrentFocus();
        //If no view currently has focus, create a new one, just so we can grab a window token from it
        if (view == null) {
            view = new View(activity);
        }
        inputMethodManager.hideSoftInputFromWindow(view.getWindowToken(), 0);
    }

    public static void hideLinearLayout(LinearLayout linearLayout) {

        if (linearLayout == null) return;

        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
        layoutParams.height = 0;
        layoutParams.width = 0;
        linearLayout.setLayoutParams(layoutParams);
    }

    public static void showLinearLayout(LinearLayout linearLayout) {

        if (linearLayout == null) return;

        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
        layoutParams.height = ViewGroup.LayoutParams.WRAP_CONTENT;
        layoutParams.width = ViewGroup.LayoutParams.MATCH_PARENT;
        linearLayout.setLayoutParams(layoutParams);
    }

    public static void hideView(View view) {

        if (view == null) return;

        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) view.getLayoutParams();
        layoutParams.height = 0;
        layoutParams.width = 0;
        view.setLayoutParams(layoutParams);
    }

    public static void showView(View view) {

        if (view == null) return;

        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) view.getLayoutParams();
        layoutParams.height = ViewGroup.LayoutParams.WRAP_CONTENT;
        layoutParams.width = ViewGroup.LayoutParams.MATCH_PARENT;
        view.setLayoutParams(layoutParams);
    }

    public static void showViewWrapContent(View view) {

        if (view == null) return;

        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) view.getLayoutParams();
        layoutParams.height = ViewGroup.LayoutParams.WRAP_CONTENT;
        layoutParams.width = ViewGroup.LayoutParams.WRAP_CONTENT;
        view.setLayoutParams(layoutParams);
    }

    public static void busyProgressBarEx(Activity activity, boolean visible) {

        if (activity == null) return;

        ViewGroup contentView = (ViewGroup) activity.findViewById(android.R.id.content);

        if (((ViewGroup) activity.findViewById(android.R.id.content)).getChildCount() > 0) {
            ViewGroup viewGroup = (ViewGroup) contentView.getChildAt(0);

            if (viewGroup == null) {
                MessageManager.showMessage("insert android:id=@+id/rootView into activity parent relative view", ErrorSeverity.Low);
                return;
            }

            View progressBar = viewGroup.findViewWithTag("busyProgressBar");

            if (progressBar != null) {
                if (visible == false) {
                    progressBar.setVisibility(View.GONE);
                    viewGroup.removeView(progressBar);
                    return;
                }
            }

            if (visible == false) return;

            if ((progressBar != null) && (visible == true)) return;

            progressBar = new ProgressBar(activity, null, android.R.attr.progressBarStyleLargeInverse);
            ((ProgressBar) progressBar).setIndeterminate(true);

            progressBar.setTag("busyProgressBar");
            progressBar.setVisibility(View.VISIBLE);
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(100, 100);
            layoutParams.addRule(RelativeLayout.CENTER_IN_PARENT, RelativeLayout.TRUE);
            progressBar.setLayoutParams(layoutParams);
            viewGroup.addView(progressBar);
        }
    }

    public static <T> T deserializeXml(String aliasType, Class type, Converter converter, String xml) {

        XStream xStream = new XStream();
        xStream.aliasType(aliasType, type);
        xStream.aliasSystemAttribute("", "class");
        xStream.registerConverter(converter);

        return (T) xStream.fromXML(xml);
    }

    public static String exceptionMessage(Exception e, String additionInfo) {
        String cause;

        try {
            cause = e.getCause().getCause().getMessage();
        } catch (Exception ex) {
            cause = EMPTY_STRING;
        }

        if (e.getMessage() == null) {
            return String.format("%s: %s: %s", additionInfo, cause, e.toString());
        } else
            return String.format("%s: %s: %s", additionInfo, cause, e.getMessage());
    }

    public static byte[] bitmapToPNGBytes(Bitmap bitmap) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.PNG, 90, stream);
        return stream.toByteArray();
    }

    public static byte[] bitmapToJPGBytes(Bitmap bitmap) {
        if (bitmap == null) return null;
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG, 90, stream);
        return stream.toByteArray();
    }

    public static byte[] bitmapToByteArray(Bitmap bitmap) {
        ByteBuffer byteBuffer = ByteBuffer.allocate(bitmap.getByteCount());
        bitmap.copyPixelsToBuffer(byteBuffer);
        return byteBuffer.array();
    }

    public static Bitmap byteArrayToBitmap(byte[] imageBuffer) {
        if (imageBuffer == null) return null;

        return BitmapFactory.decodeByteArray(imageBuffer, 0, imageBuffer.length);
    }

    public static Bitmap getResizedBitmap(Bitmap image, int bitmapWidth, int bitmapHeight) {
        return Bitmap.createScaledBitmap(image, bitmapWidth, bitmapHeight, true);
    }

    public static Bitmap getResizedBitmap(Bitmap image, int maxSize) {
        int width = image.getWidth();
        int height = image.getHeight();

        float bitmapRatio = (float) width / (float) height;
        if (bitmapRatio > 1) {
            width = maxSize < width ? maxSize : width;
            height = (int) (width / bitmapRatio);
        } else {
            height = maxSize < height ? maxSize : height;
            width = (int) (height * bitmapRatio);
        }

        return Bitmap.createScaledBitmap(image, width, height, true);
    }

    public static byte[] getResizedBitmap(byte[] imageBuffer, int resizeFactor) {
        Bitmap bitmap = byteArrayToBitmap(imageBuffer);

        int maxSize = bitmap.getWidth() / resizeFactor;
        int width = bitmap.getWidth();
        int height = bitmap.getHeight();

        float bitmapRatio = (float) width / (float) height;
        if (bitmapRatio > 1) {
            width = maxSize < width ? maxSize : width;
            height = (int) (width / bitmapRatio);
        } else {
            height = maxSize < height ? maxSize : height;
            width = (int) (height * bitmapRatio);
        }

        return bitmapToByteArray(Bitmap.createScaledBitmap(bitmap, width, height, true));
    }

    public static File getTicketFile(String filename) {

        File ticktetFolder = Environment.getExternalStoragePublicDirectory(ITICKET_FOLDER);
        if (!ticktetFolder.isDirectory()) {
            ticktetFolder.mkdir();
        }

        return new File(ticktetFolder + String.format("/%s", filename));
    }

    public static void startBarcodeScanActivity(Fragment fragment, int scanRequestCode) {
        try {
            Intent launchIntent = new Intent();
            launchIntent.setClassName("za.co.tmt.iticket", "com.manateeworks.ActivityCapture");

            if (launchIntent != null) {
                fragment.startActivityForResult(launchIntent, scanRequestCode);
            }
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    public static void startBarcodeScanActivity(Activity activity, int scanRequestCode) {
        try {
            Intent launchIntent = new Intent();
            launchIntent.setClassName("za.co.tmt.iticket", "com.manateeworks.ActivityCapture");

            if (launchIntent != null) {
                activity.startActivityForResult(launchIntent, scanRequestCode);
            }
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    public static Bitmap getQrCode(String qrCode) {

        MultiFormatWriter multiFormatWriter = new MultiFormatWriter();

        try {
            BitMatrix bitMatrix = multiFormatWriter.encode(qrCode, BarcodeFormat.QR_CODE, 200, 200);
            return new BarcodeEncoder().createBitmap(bitMatrix);
        } catch (WriterException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "shared.Utilities::getQrCode()"), ErrorSeverity.High);
            return null;
        }
    }

    public static Bitmap getBarcode128(String value) {

        MultiFormatWriter multiFormatWriter = new MultiFormatWriter();

        try {
            BitMatrix bitMatrix = multiFormatWriter.encode(value, BarcodeFormat.CODE_128, 550, 100);
            return new BarcodeEncoder().createBitmap(bitMatrix);
        } catch (WriterException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "shared.Utilities::getQrCode()"), ErrorSeverity.High);
            return null;
        }
    }

    public static Bitmap getBarcode128Ex(String value, int width, int height) {

        MultiFormatWriter multiFormatWriter = new MultiFormatWriter();

        try {
            BitMatrix bitMatrix = multiFormatWriter.encode(value, BarcodeFormat.CODE_128, width, height);
            return new BarcodeEncoder().createBitmap(bitMatrix);
        } catch (WriterException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "shared.Utilities::getQrCode()"), ErrorSeverity.High);
            return null;
        }
    }

    public static int getYear() {
        return Calendar.getInstance().get(Calendar.YEAR);
    }

    public static int getYearInCentury() {
        return Calendar.getInstance().get(Calendar.YEAR) - 2000;
    }

    public static int getDayOfYear() {
        return Calendar.getInstance().get(Calendar.DAY_OF_YEAR);
    }

    public static UserModel getUser(Activity activity) {

        try {
            UserModel user = activity.getIntent().getParcelableExtra(Constants.USER);

            if (user == null) {
                MessageManager.showMessage(Utilities.getString(R.string.user_cannot_be_null), ErrorSeverity.None);
                return null;
            } else {
                SessionModel.getInstance().setUserId(user.getId());
                SessionModel.getInstance().setUserName(user.getUserName());
                SessionModel.getInstance().setPassword(user.getPassword());
                SessionModel.getInstance().setUser(user);
                return user;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getUser()"), ErrorSeverity.None);
            return null;
        }
    }

    public static MobileDeviceModel getMobileDevice(Activity activity) {

        try {
            MobileDeviceModel mobileDevice = activity.getIntent().getParcelableExtra(Constants.MOBILE_DEVICE);

            if (mobileDevice == null) {
                MessageManager.showMessage(Utilities.getString(R.string.mobile_device_cannot_be_null), ErrorSeverity.None);
                return null;
            } else {
                SessionModel.getInstance().setInternalDeviceID(mobileDevice.getID());
                return mobileDevice;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getMobileDevice()"), ErrorSeverity.None);
            return null;
        }
    }

    public static void setPrinterMacAddress(Activity activity) {

        try {
            String printerMacAddress = activity.getIntent().getStringExtra(Constants.PRINTER_MAC_ADDRESS);

            if (printerMacAddress == null) {
                MessageManager.showMessage(Utilities.getString(R.string.printer_mac_address_cannot_be_null), ErrorSeverity.None);
            } else {
                SessionModel.getInstance().setPrinterMacAddress(printerMacAddress);
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getPrinterMacAddress()"), ErrorSeverity.None);
        }
    }

    public static String getEndPoint(Activity activity, String endPoint) {

        try {
            String result = activity.getIntent().getStringExtra(endPoint);

            if (result == null) {
                //MessageManager.showMessage(Utilities.getString(R.string.end), ErrorSeverity.None);
                return null;
            } else {
                return result;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getEndPoint()"), ErrorSeverity.None);
            return null;
        }
    }

    public static DistrictModel getDistrict(Activity activity) {

        try {

            DistrictModel district = activity.getIntent().getParcelableExtra(Constants.DISTRICT);

            if (district == null) {
                MessageManager.showMessage(Utilities.getString(R.string.district_cannot_be_null), ErrorSeverity.None);
                return null;
            } else {
                SessionModel.getInstance().setDistrict(district);
                return district;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getDistrict()"), ErrorSeverity.None);
            return null;
        }
    }

    public static PaymentContext getPaymentContext(Activity activity) {

        try {

            PaymentContext paymentContext = (PaymentContext)activity.getIntent().getSerializableExtra(Constants.PAYMENT_CONTEXT);

            if (paymentContext == null) {
                MessageManager.showMessage(Utilities.getString(R.string.payment_context_cannot_be_null), ErrorSeverity.None);
                return null;
            } else {
                return paymentContext;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getPaymentContext()"), ErrorSeverity.None);
            return null;
        }
    }

    public static SearchFinesCriteriaType getSearchFinesCriteria(Activity activity) {

        try {

            SearchFinesCriteriaType paymentContext = (SearchFinesCriteriaType)activity.getIntent().getSerializableExtra(Constants.SEARCH_FINES_CRITERIA);

            if (paymentContext == null) {
                MessageManager.showMessage(Utilities.getString(R.string.search_fines_criteria_cannot_be_null), ErrorSeverity.None);
                return null;
            } else {
                return paymentContext;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Shared.Utilities::getSearchFinesCriteria()"), ErrorSeverity.None);
            return null;
        }
    }


    public static void writeNullableLong(Parcel out, Long value) {

        if (value == null) {
            out.writeByte((byte) 0);
            return;
        }

        out.writeByte((byte) 1);
        out.writeLong(value);
    }

    public static Long readNullableLong(Parcel in) {

        byte nullIndicator = in.readByte();

        return nullIndicator == 0 ? null : in.readLong();
    }

    public static double stringToDouble(String value) {
        try {
            return Double.parseDouble(value);
        } catch (Exception e) {
            return 0;
        }
    }

    public static void writeNullableInteger(Parcel out, Integer value) {

        if (value == null) {
            out.writeByte((byte) 0);
            return;
        }

        out.writeByte((byte) 1);
        out.writeInt(value);
    }

    public static List<String> getRegexMatches(String inputString, String regEx) {

        List<String> list = new ArrayList<>();
        Pattern pattern = Pattern.compile(regEx);

        Matcher matcher = pattern.matcher(inputString);

        while (matcher.find()) {
            list.add(matcher.group());
        }

        return list;
    }

    public static int indexOf(final Adapter adapter, String value) {
        for (int index = 0, count = adapter.getCount(); index < count; ++index) {
            if (adapter.getItem(index).toString().equals(value)) {
                return index;
            }
        }
        return -1;
    }

    public static String rectifyTelephoneNumber(String telephoneNumber) {

        String rectifiedTelephoneNumber = telephoneNumber.replaceAll("[^0-9]", "");
        rectifiedTelephoneNumber = rectifiedTelephoneNumber.replaceFirst("^0+(?!$)", "");

        return rectifiedTelephoneNumber;
    }

    public static void writeBufferToFile(byte[] buffer, String filename) throws IOException {
        File file = new File(filename);
        FileOutputStream fileOutputStream = null;

        try {
            fileOutputStream = new FileOutputStream(file);
            fileOutputStream.write(buffer);
        } finally {
            if (fileOutputStream != null) {
                fileOutputStream.close();
            }
        }
    }

    public static boolean overwriteDatabaseFile(String PackageName, String filename) {

        try {

            File iTicketFolder = Environment.getExternalStoragePublicDirectory(Constants.ITICKET_FOLDER);

            if (iTicketFolder.canWrite()) {
                String destinationDBPath = String.format("/data/data/%s/databases/%s", PackageName, filename);//"/data/data/za.co.kapsch.iticket/databases/iTicket.db";
                String sourceDBName = filename;
                File destinationDB = new File(destinationDBPath);
                File sourceDB = new File(iTicketFolder, sourceDBName);

                if (destinationDB.exists()) {
                    destinationDB.delete();
                }

                if (sourceDB.exists()) {
                    FileChannel src = new FileInputStream(sourceDB).getChannel();
                    FileChannel dst = new FileOutputStream(destinationDB).getChannel();
                    dst.transferFrom(src, 0, src.size());
                    src.close();
                    dst.close();
                }
            }
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Utilities::overwriteDatabaseFile()"), ErrorSeverity.High);
            return false;
        }

        return true;
    }

    public static Integer readNullableInteger(Parcel in) {

        byte nullIndicator = in.readByte();

        return nullIndicator == 0 ? null : in.readInt();
    }

    public static String removeNonLetters(String telephoneNumber) {
        return telephoneNumber.replaceAll("[^\\p{L}\\p{Nd} ]+", "").trim();
    }

    public static void setActionBar(AppCompatActivity activity, int customActionBarID, int subTextID, String subTextA) {

        activity.getSupportActionBar().setDisplayShowTitleEnabled(false);
        View view = activity.getLayoutInflater().inflate(customActionBarID, null);

        TextView textView = (TextView) view.findViewById(subTextID);
        textView.setText(subTextA);

        Toolbar.LayoutParams layout = new Toolbar.LayoutParams(Toolbar.LayoutParams.FILL_PARENT, Toolbar.LayoutParams.FILL_PARENT);
        activity.getSupportActionBar().setCustomView(view, layout);

        activity.getSupportActionBar().setDisplayShowCustomEnabled(true);
        activity.getSupportActionBar().setCustomView(view);

        Toolbar parent = (Toolbar) view.getParent();
        parent.setPadding(0, 0, 0, 0);
        parent.setContentInsetsAbsolute(0, 0);
    }

    public static List<String> wrapTextOnLength(String text, int lineCharLength) {

        if (text == null) return null;

        List<String> stringList = new ArrayList<>();

        String[] words = text.split(" ");

        if (words.length > 1) {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < words.length; i++) {
                if ((stringBuilder.toString().length() + words[i].length()) <= lineCharLength) {
                    stringBuilder.append(String.format(" %s", words[i]));
                } else {
                    stringList.add(stringBuilder.toString().trim());
                    stringBuilder = new StringBuilder();
                    stringBuilder.append(words[i]);
                }
            }
            stringList.add(stringBuilder.toString().trim());
            return stringList;
        }

        stringList.add(text);
        return stringList;
    }

    public static byte[] getFileData(File evidenceFile) throws IOException {

        String filename = evidenceFile.getAbsolutePath();
        File file = new File(filename);
        int fileSize = (int) file.length();

        byte fileData[] = new byte[fileSize];
        byte partialData[] = new byte[fileSize];
        FileInputStream inputStream = null;

        int totalBytesRead = 0;

        try {
            inputStream = new FileInputStream(filename);
            do {
                int bytesRead = (inputStream.read(partialData, totalBytesRead == 0 ? 0 : totalBytesRead - 1, fileSize - totalBytesRead));
                System.arraycopy(partialData, 0, fileData, totalBytesRead == 0 ? 0 : totalBytesRead - 1, bytesRead);
                totalBytesRead += bytesRead;
            } while (totalBytesRead != fileSize);

            return fileData;
        } finally {
            if (inputStream != null) {
                inputStream.close();
            }
        }
    }

    public static boolean copyDatabaseFile(String currentDBPath) {

        try {
            File iTicketFolder = Environment.getExternalStoragePublicDirectory(Constants.ITICKET_FOLDER);
            File data = Environment.getDataDirectory();

            if (iTicketFolder.canWrite()) {
                String backupDBPath = String.format("%s.db", getDeviceId());
                File currentDB = new File(currentDBPath);
                File backupDB = new File(iTicketFolder, backupDBPath);

                if (backupDB.exists()) {
                    backupDB.delete();
                }

                if (currentDB.exists()) {
                    FileChannel src = new FileInputStream(currentDB).getChannel();
                    FileChannel dst = new FileOutputStream(backupDB).getChannel();
                    dst.transferFrom(src, 0, src.size());
                    src.close();
                    dst.close();
                }
            }
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Utilities::copyDatabaseFile()"), ErrorSeverity.High);
            return false;
        }

        return true;
    }

    public static int getInstalledAppVersion(String packageName) {
        try {
            PackageInfo pInfo = LibApp.getContext().getPackageManager().getPackageInfo(packageName, 0);
            return pInfo.versionCode;
        } catch (PackageManager.NameNotFoundException e) {
            //MessageManager.showMessage(Utilities.exceptionMessage(e, "console.Utilities::getInstalledAppVersion(), Name not found exception"), ErrorSeverity.High);
            return -1;
        }
    }

    public static int getDownloadedApkVersion(String apkFileName) {
        final PackageManager packageManager = LibApp.getContext().getPackageManager();
        File file = Utilities.getTicketFile(apkFileName);
        PackageInfo info = packageManager.getPackageArchiveInfo(file.getPath(), 0);
        if (info != null) {
            return info.versionCode;
        }
        return -1;
    }

    public static Resources getResourceFromApplication(Context context, String packageName) {
        try {
            return context.getPackageManager().getResourcesForApplication(packageName);
        } catch (PackageManager.NameNotFoundException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "console.Utilities::getResourceFromApplication(), Name not found exception"), ErrorSeverity.High);
            return null;
        }
    }

    public static boolean installApk(String packageName) {

        int installedAppVersionCode = getInstalledAppVersion(packageName);

        String apkFileName = String.format("%s.apk", packageName);
        int downloadedAppVersionCode = getDownloadedApkVersion(apkFileName);

        return (installedAppVersionCode < downloadedAppVersionCode);
    }

    public static void startPaymentApplication(
            UserModel user,
            DistrictModel district,
            MobileDeviceModel mobileDevice,
            SearchFinesCriteriaType searchFinesCriteriaType,
            String coreGateway,
            String itsGateway,
            String evrGateway,
            String printerMacAddress,
            String vln,
            String idNumber){

        try {
            Intent launchIntent = new Intent();
            launchIntent.putExtra(Constants.USER, user);
            launchIntent.putExtra(Constants.DISTRICT, district);
            launchIntent.putExtra(Constants.MOBILE_DEVICE, mobileDevice);
            launchIntent.putExtra(Constants.PAYMENT_CONTEXT, PaymentContext.TrafficFines);
            launchIntent.putExtra(Constants.SEARCH_FINES_CRITERIA, searchFinesCriteriaType);
            launchIntent.putExtra(Constants.CORE_END_POINT, coreGateway);
            launchIntent.putExtra(Constants.ITS_END_POINT, itsGateway);
            launchIntent.putExtra(Constants.EVR_END_POINT, evrGateway);
            launchIntent.putExtra(Constants.PRINTER_MAC_ADDRESS, printerMacAddress);
            launchIntent.putExtra(Constants.VLN, vln);
            launchIntent.putExtra(Constants.ID_NUMBER, idNumber);
            launchIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            launchIntent.setClassName("za.co.kapsch.ipayment", "za.co.kapsch.ipayment.MainActivity");

            if (launchIntent != null) {
                LibApp.getContext().startActivity(launchIntent);
            }
        } catch (Exception e) {
            za.co.kapsch.shared.MessageManager.showMessage(Utilities.exceptionMessage(e, "VosiActionActivity::startPaymentApplication()"), ErrorSeverity.High);
        }
    }

    public static void wifiOn() {

        try {
            WifiManager wifiManager = (WifiManager) LibApp.getContext().getApplicationContext().getSystemService(Context.WIFI_SERVICE);

            if (wifiManager.isWifiEnabled() == true) {
                return;
            }

            wifiManager.setWifiEnabled(true);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "shared.Utilities::wifiOn()" ), ErrorSeverity.High);
        }
    }

    public static void wifiOff() {

        try {
            WifiManager wifiManager = (WifiManager)LibApp.getContext().getApplicationContext().getSystemService(Context.WIFI_SERVICE);

            if(wifiManager.isWifiEnabled() == false){
                return;
            }

            wifiManager.setWifiEnabled(false);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "shared.Utilities::wifiOff()" ), ErrorSeverity.High);
        }
    }

    public static boolean isEven(int number){

        if ((number % 2) == 0) {
            return true;
        }

        return false;
    }

    public static String computeSHA256Hash(String password)
    {
        try
        {
            MessageDigest mdSha256 = MessageDigest.getInstance("SHA-256");
            mdSha256.update(password.getBytes("UTF8"));

            byte[] data = mdSha256.digest();

            return Base64.encodeToString(data, 0, data.length, Base64.NO_WRAP);
        } catch (NoSuchAlgorithmException e1) {
            MessageManager.showMessage(Utilities.exceptionMessage(e1, "NoSuchAlgorithmException::computeSHA256Hash"), ErrorSeverity.High);
        }  catch (UnsupportedEncodingException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "UnsupportedEncodingException::computeSHA256Hash"), ErrorSeverity.High);
        }catch (Exception ex){
            MessageManager.showMessage(Utilities.exceptionMessage(ex, "Exception::computeSHA256Hash"), ErrorSeverity.High);
        }

        return null;
    }

    public static boolean isListNullOrEmpty(List<Object> list){

        if(list == null) return true;

        if (list.size() == 0) return true;

        return false;
    }

    public static String padRight(String value, int numberOfPads, char padValue) {

        String formatString = String.format("%s%d%s", "%-", numberOfPads, "s");
        return String.format(formatString, value).replace(' ', padValue);
    }

    public static String padLeft(String value, int numberOfPads, char padValue) {

        String formatString = String.format("%s%d%s", "%", numberOfPads, "s");
        return String.format(formatString, value).replace(' ', padValue);
    }

    public static String trimString(String source) {

        if ((source == null) || (source.length() == 0)) return source;

        return trimEnd(trimStart(source));
    }

    public static String trimStart(String source) {

        if ((source == null) || (source.length() == 0)) return source;

        int pos = 0;

        while (pos < source.length() && Character.isWhitespace(source.charAt(pos))) {
            pos++;
        }

        return source.substring(pos);
    }

    public static String trimEnd(String source) {

        if ((source == null) || (source.length() == 0)) return source;

        int pos = source.length() - 1;

        while ((pos >= 0) && Character.isWhitespace(source.charAt(pos))) {
            pos--;
        }

        pos++;

        return (pos < source.length()) ? source.substring(0, pos) : source;
    }

    public static boolean validateUserAccess(UserModel user, String packageName){

        List<SystemFunctionModel> systemFunctions = user.getSystemFunctions();

        for(SystemFunctionModel systemFunction : systemFunctions){
            if (packageName.toUpperCase().contains(systemFunction.getName().toUpperCase())){
                return true;
            }
        }

        //String[] parts = packageName.split("\\.");
        //String message = String.format(Utilities.getString(R.string.user_does_not_have_access), parts[parts.length-1].toUpperCase());
        //MessageManager.showMessage(message, ErrorSeverity.None);

        return false;
    }

    public static String formatReferenceNumber(String referenceNumber){

        if (referenceNumber.length() < 16){
            MessageManager.showMessage("reference number not of correct length", ErrorSeverity.None);
        }

        return String.format("%s %s %s %s",
                    referenceNumber.substring(0,4),
                    referenceNumber.substring(4,8),
                    referenceNumber.substring(8,12),
                    referenceNumber.substring(12,16));
    }

    public static void logUserActivity(String category, String actionDescription){
        try {
            UserActivityLogModel userActivityLog = new UserActivityLogModel();
            userActivityLog.setDeviceID(Utilities.getDeviceId());
            UserModel user = SessionModel.getInstance().getUser();
            userActivityLog.setCredentialID(user == null ? null : user.getCredentialID());
            userActivityLog.setCreatedTimestamp(Calendar.getInstance().getTime());
            userActivityLog.setCategory(category);
            userActivityLog.setActionDescription(actionDescription);

            UserActivityLogRepository.create(userActivityLog);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Utilities::logUserActivity-1"), ErrorSeverity.High);
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Utilities::logUserActivity-2"), ErrorSeverity.High);
        }
    }

    public static String userActivityLogActionDescription(String[] descriptionList){

        StringBuilder stringBuilder = new StringBuilder();

        for(String description : descriptionList) {

            stringBuilder.append(String.format("%s%s", description, "-"));
        }

        String result = stringBuilder.toString();

        return result.substring(0, result.length()-1);
    }

    public static List<String> wrapText(String text, int lineLength){

        if (TextUtils.isEmpty(text) == true) {
            return null;
        }

        String[] textList = text.split(" ");
        List<String> allLines = new ArrayList<>();
        StringBuilder currentLine = new StringBuilder();

        for(String string: textList) {

            if (currentLine.length() + string.length() < lineLength) {
                currentLine.append(String.format("%s ", string));
            }else{
                allLines.add(currentLine.toString().trim());
                currentLine.delete(0, currentLine.length());
                currentLine.append(String.format("%s ", string));
            }
        }

        if (currentLine.length() > 0){
            allLines.add(currentLine.toString());
        }

        return allLines;
    }

    public static float getTextWidth(String text){

        if (text != null) {
            return new Paint().measureText(text);
        }

        return 0;
    }

    public final static boolean isValidEmail(CharSequence email) {
        if (email == null) {
            return false;
        } else {
            return android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches();
        }
    }

    public static String nullOrEmptyString(String value, boolean returnNotAvailable){

        if (returnNotAvailable == true) {
            return TextUtils.isEmpty(value) ? LibApp.getContext().getResources().getString(R.string.n_a) : value;
        }else{
            return TextUtils.isEmpty(value) ? Constants.EMPTY_STRING : value;
        }
    }
}
