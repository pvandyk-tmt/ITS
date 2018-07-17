//package za.co.kapsch.console.General;
//
//import android.app.Activity;
//import android.app.AlarmManager;
//import android.app.AlertDialog;
//import android.content.Context;
//import android.content.DialogInterface;
//import android.content.SharedPreferences;
//import android.content.pm.PackageInfo;
//import android.content.pm.PackageManager;
//import android.content.res.Resources;
//import android.graphics.Bitmap;
//import android.graphics.BitmapFactory;
//import android.media.Ringtone;
//import android.media.RingtoneManager;
//import android.net.Uri;
//import android.os.Environment;
//import android.provider.Settings;
//import android.support.v4.app.Fragment;
//import android.support.v7.app.AppCompatActivity;
//import android.support.v7.widget.Toolbar;
//import android.telephony.TelephonyManager;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.inputmethod.InputMethodManager;
//import android.widget.Adapter;
//import android.widget.ImageButton;
//import android.widget.LinearLayout;
//import android.widget.ProgressBar;
//import android.widget.RelativeLayout;
//import android.widget.TextView;
//
//import com.thoughtworks.xstream.XStream;
//import com.thoughtworks.xstream.converters.Converter;
//
//import java.io.ByteArrayOutputStream;
//import java.io.File;
//import java.io.FileInputStream;
//import java.io.FileOutputStream;
//import java.io.IOException;
//import java.io.InputStream;
//import java.io.UnsupportedEncodingException;
//import java.lang.reflect.Field;
//import java.lang.reflect.InvocationTargetException;
//import java.lang.reflect.Method;
//import java.nio.ByteBuffer;
//import java.nio.channels.FileChannel;
//import java.util.ArrayList;
//import java.util.Calendar;
//import java.util.Date;
//import java.text.ParseException;
//import java.text.SimpleDateFormat;
//import java.util.List;
//import java.util.Map;
//import java.util.TimeZone;
//import java.util.regex.Matcher;
//import java.util.regex.Pattern;
//
//import za.co.kapsch.console.BuildConfig;
//import za.co.kapsch.console.Enums.ErrorSeverity;
//import za.co.kapsch.shared.Models.DistrictModel;
//import za.co.kapsch.shared.Models.UserModel;
//import za.co.kapsch.console.R;
//import za.co.kapsch.console.orm.DistrictRepository;
//
///**
// * Created by csenekal on 2016-07-17.
// */
//public class Utilities {
//
//    public static final String APP_DATA = "AppData";
//    private final static char[] hexArray = "0123456789ABCDEF".toCharArray();
//    private static final String DATE_PATTERN = "(0?[1-9]|1[012]) [/.-] (0?[1-9]|[12][0-9]|3[01]) [/.-] ((19|20)\\d\\d)";
//    private static final Pattern IP_ADDRESS
//            = Pattern.compile(
//            "((25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9])\\.(25[0-5]|2[0-4]"
//                    + "[0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]"
//                    + "[0-9]{2}|[1-9][0-9]|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}"
//                    + "|[1-9][0-9]|[0-9]))");
//
//    public static String getString(int resourceId){
//        return  App.getContext().getResources().getString(resourceId);
//    }
//
//    public static void displayOkMessage(String message, Activity activity) {
//        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(activity);
//        dlgAlert.setMessage(message);
//        dlgAlert.setTitle("App message...");
//        dlgAlert.setPositiveButton("OK", null);
//        dlgAlert.setCancelable(true);
//        dlgAlert.create().show();
//    }
//
//    public static void displayDecisionMessage(String message, Fragment fragment) {
//        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(fragment.getActivity());
//        dlgAlert.setMessage(message);
//        dlgAlert.setTitle("App message...");
//        dlgAlert.setNegativeButton("No", (DialogInterface.OnClickListener) fragment);
//        dlgAlert.setPositiveButton("Yes", (DialogInterface.OnClickListener) fragment);
//        dlgAlert.setCancelable(true);
//        dlgAlert.create().show();
//    }
//
//    public static void displayDecisionMessage(String message, Activity activity) {
//        AlertDialog.Builder dlgAlert = new AlertDialog.Builder(activity);
//        dlgAlert.setMessage(message);
//        dlgAlert.setTitle("App message...");
//        dlgAlert.setNegativeButton("No", (DialogInterface.OnClickListener) activity);
//        dlgAlert.setPositiveButton("Yes", (DialogInterface.OnClickListener) activity);
//        dlgAlert.setCancelable(true);
//        dlgAlert.create().show();
//    }
//
//    public static byte[] readFileFromAssets(String fileName, Activity activity) throws IOException {
//        InputStream stream = null;
//        try {
//            stream = activity.getAssets().open(fileName);
//            int size = stream.available();
//            byte[] buffer = new byte[size];
//            stream.read(buffer);
//            return buffer;
//        } catch (IOException e) {
//            MessageManager.showMessage(activity.getResources().getString(R.string.asset_file_not_found), ErrorSeverity.High);
//            return null;
//        } finally {
//            if (stream != null) {
//                stream.close();
//            }
//        }
//    }
//
//    public static String getStringFromSharedPreference(String key) {
//        SharedPreferences appData = App.getContext().getSharedPreferences(APP_DATA, Context.MODE_PRIVATE);
//        return appData.getString(key, "");
//    }
//
//    public static boolean addStringToSharedPreference(String key, String value) {
//        SharedPreferences sharedpreferences = App.getContext().getSharedPreferences(APP_DATA, Context.MODE_PRIVATE);
//        SharedPreferences.Editor editor = sharedpreferences.edit();
//        editor.putString(key, value);
//        return editor.commit();
//    }
//
//    public static String dateToString(Date date) {
//        if (date == null) return null;
//        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.DATE_FORMAT);
//        return simpleDateFormat.format(date);
//    }
//
//    public static String dateTimeToString(Date date) {
//        if (date == null) return null;
//        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.DATETIME_FORMAT);
//        return simpleDateFormat.format(date);
//    }
//
//    public static String timeToString(Date date) {
//        if (date == null) return null;
//        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.TIME_FORMAT);
//        return simpleDateFormat.format(date);
//    }
//
//    public static Date stringToDate(String date, String format) throws ParseException {
//        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(format);
//
//        Date value = simpleDateFormat.parse(date);
//
//        return simpleDateFormat.parse(date);
//    }
//
//    //get current date as follows
//    //year = Calendar.get(Calendar.YEAR);
//    //month = mCalendar.get(Calendar.MONTH);
//    //day = mCalendar.get(Calendar.DAY_OF_MONTH) + Constants.DAYS_60; //add 60 days to date
//    public static Date getDate(int year, int month, int day, int hour, int minute, int second) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.YEAR, year);
//        calendar.set(Calendar.MONTH, month);
//        calendar.set(Calendar.DAY_OF_MONTH, day);
//        calendar.set(Calendar.HOUR_OF_DAY, hour);
//        calendar.set(Calendar.MINUTE, minute);
//        calendar.set(Calendar.SECOND, second);
//        return calendar.getTime();
//    }
//
//    public static Date addDaysToDate(int days) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.DAY_OF_MONTH, calendar.get(Calendar.DAY_OF_MONTH) + days);
//        return calendar.getTime();
//    }
//
//    public static Calendar getCalendarAddDays(int days) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.DAY_OF_MONTH, calendar.get(Calendar.DAY_OF_MONTH) + days);
//        return calendar;
//    }
//
//    public static Calendar getCalendar(int year, int month, int day, int hour, int minute, int second) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.YEAR, year);
//        calendar.set(Calendar.MONTH, month);
//        calendar.set(Calendar.DAY_OF_MONTH, day);
//        calendar.set(Calendar.HOUR_OF_DAY, hour);
//        calendar.set(Calendar.MINUTE, minute);
//        calendar.set(Calendar.SECOND, second);
//        return calendar;
//    }
//
//    public static Calendar getCalendarDate(Date date) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.YEAR, date.getYear() + 1900);
//        calendar.set(Calendar.MONTH, date.getMonth());
//        calendar.set(Calendar.DAY_OF_MONTH, date.getDate());
//        calendar.set(Calendar.HOUR_OF_DAY, date.getHours());
//        calendar.set(Calendar.MINUTE, date.getMinutes());
//        calendar.set(Calendar.SECOND, date.getSeconds());
//        return calendar;
//    }
//
//    public static Date addDaysToDate(Date date, int days) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.YEAR, date.getYear() + 1900);
//        calendar.set(Calendar.MONTH, date.getMonth());
//        calendar.set(Calendar.DAY_OF_MONTH, date.getDate());
//        calendar.set(Calendar.HOUR_OF_DAY, date.getHours());
//        calendar.set(Calendar.MINUTE, date.getMinutes());
//        calendar.set(Calendar.SECOND, date.getSeconds());
//        calendar.set(Calendar.DAY_OF_MONTH, calendar.get(Calendar.DAY_OF_MONTH) + days);
//        return calendar.getTime();
//    }
//
//    public static Date getDateAddMinutes(int minutes) {
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(Calendar.MINUTE, calendar.get(Calendar.MINUTE) + minutes);
//        return calendar.getTime();
//    }
//
//    public static String byteArrayToString(byte[] data) {
//
//        String result;
//        try {
//            result = new String(data, "UTF-8");
//        } catch (UnsupportedEncodingException e) {
//
//            result = "";
//            for (int i = 0; i < data.length; i++)
//                result = result + (char) data[i];
//        }
//        return result;
//    }
//
//    public static String bytesToHex(byte[] bytes) {
//        char[] hexChars = new char[bytes.length * 2];
//        for (int j = 0; j < bytes.length; j++) {
//            int v = bytes[j] & 0xFF;
//            hexChars[j * 2] = hexArray[v >>> 4];
//            hexChars[j * 2 + 1] = hexArray[v & 0x0F];
//        }
//        return new String(hexChars);
//    }
//
//    public static byte[] bitmapToPNGBytes(Bitmap bitmap) {
//        ByteArrayOutputStream stream = new ByteArrayOutputStream();
//        bitmap.compress(Bitmap.CompressFormat.PNG, 90, stream);
//        return stream.toByteArray();
//    }
//
//    public static byte[] bitmapToJPGBytes(Bitmap bitmap) {
//        if (bitmap == null) return null;
//        ByteArrayOutputStream stream = new ByteArrayOutputStream();
//        bitmap.compress(Bitmap.CompressFormat.JPEG, 90, stream);
//        return stream.toByteArray();
//    }
//
//    public static byte[] bitmapToByteArray(Bitmap bitmap) {
//        ByteBuffer byteBuffer = ByteBuffer.allocate(bitmap.getByteCount());
//        bitmap.copyPixelsToBuffer(byteBuffer);
//        return byteBuffer.array();
//    }
//
//    public static Bitmap byteArrayToBitmap(byte[] imageBuffer) {
//        if (imageBuffer == null) return null;
//
//        return BitmapFactory.decodeByteArray(imageBuffer, 0, imageBuffer.length);
//    }
//
//    public static Bitmap getResizedBitmap(Bitmap image, int bitmapWidth, int bitmapHeight) {
//        return Bitmap.createScaledBitmap(image, bitmapWidth, bitmapHeight, true);
//    }
//
//    public static Bitmap getResizedBitmap(Bitmap image, int maxSize) {
//        int width = image.getWidth();
//        int height = image.getHeight();
//
//        float bitmapRatio = (float) width / (float) height;
//        if (bitmapRatio > 1) {
//            width = maxSize < width ? maxSize : width;
//            height = (int) (width / bitmapRatio);
//        } else {
//            height = maxSize < height ? maxSize : height;
//            width = (int) (height * bitmapRatio);
//        }
//
//        return Bitmap.createScaledBitmap(image, width, height, true);
//    }
//
//    public static byte[] getResizedBitmap(byte[] imageBuffer, int resizeFactor) {
//        Bitmap bitmap = byteArrayToBitmap(imageBuffer);
//
//        int maxSize = bitmap.getWidth() / resizeFactor;
//        int width = bitmap.getWidth();
//        int height = bitmap.getHeight();
//
//        float bitmapRatio = (float) width / (float) height;
//        if (bitmapRatio > 1) {
//            width = maxSize < width ? maxSize : width;
//            height = (int) (width / bitmapRatio);
//        } else {
//            height = maxSize < height ? maxSize : height;
//            width = (int) (height * bitmapRatio);
//        }
//
//        return bitmapToByteArray(Bitmap.createScaledBitmap(bitmap, width, height, true));
//    }
//
////    public static Bitmap getResizedBitmap(byte[] imageBuffer, int resizeFactor) {
////
////        Bitmap bitmap = byteArrayToBitmap(imageBuffer);
////
////        int width = bitmap.getWidth()/resizeFactor;
////        int height = bitmap.getHeight()/resizeFactor;
////
////        return Bitmap.createScaledBitmap(bitmap, width, height, true);
////    }
//
////    public static byte[] getResizedBitmapEx(byte[] imageBuffer, int resizeFactor) {
////
////        Bitmap bitmap = byteArrayToBitmap(imageBuffer);
////
////        int width = bitmap.getWidth()/resizeFactor;
////        int height = bitmap.getHeight()/resizeFactor;
////
////        Bitmap scaledBitmap = Bitmap.createScaledBitmap(bitmap, width, height, true);
////
////        byte[] scaledImageBuffer = bitmapToByteArray(scaledBitmap);
////
////        return scaledImageBuffer;
////
////        //return bitmapToByteArray(scaledBitmap);
////    }
//
////    public static byte[] getResizedBitmapEx(Bitmap image, int resizeFactor) {
////        int width = image.getWidth()/resizeFactor;
////        int height = image.getHeight()/resizeFactor;
////
////        return bitmapToByteArray(Bitmap.createScaledBitmap(image, width, height, true));
////    }
//
//    public static List<String> wrapTextOnLength(String text, int lineCharLength) {
//
//        if (text == null) return null;
//
//        List<String> stringList = new ArrayList<>();
//
//        String[] words = text.split(" ");
//
//        if (words.length > 1) {
//            StringBuilder stringBuilder = new StringBuilder();
//            for (int i = 0; i < words.length; i++) {
//                if ((stringBuilder.toString().length() + words[i].length()) <= lineCharLength) {
//                    stringBuilder.append(String.format(" %s", words[i]));
//                } else {
//                    stringList.add(stringBuilder.toString().trim());
//                    stringBuilder = new StringBuilder();
//                    stringBuilder.append(words[i]);
//                }
//            }
//            stringList.add(stringBuilder.toString().trim());
//            return stringList;
//        }
//
//        stringList.add(text);
//        return stringList;
//    }
//
//    //RelativeLayout is the outer parent container
//    public static void busyProgressBar(Activity activity, RelativeLayout layout, boolean visible) {
//
//        View progressBar = layout.findViewWithTag("busyProgressBar");
//
//        if (progressBar != null) {
//            if (visible == false) {
//                progressBar.setVisibility(View.GONE);
//                layout.removeView(progressBar);
//                return;
//            }
//        }
//
//        if ((progressBar != null) && (visible == true)) return;
//
//        progressBar = new ProgressBar(activity, null, android.R.attr.progressBarStyleHorizontal);
//        ((ProgressBar) progressBar).setIndeterminate(true);
//        ((ProgressBar) progressBar).setIndeterminateDrawable(activity.getResources().getDrawable(R.drawable.circular_progress_bar));
//
//        progressBar.setTag("busyProgressBar");
//        progressBar.setVisibility(View.VISIBLE);
//        progressBar.setBackground(activity.getResources().getDrawable(R.drawable.circle_shape));
//        RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(200, 200);
//        layoutParams.addRule(RelativeLayout.CENTER_IN_PARENT, RelativeLayout.TRUE);
//        progressBar.setLayoutParams(layoutParams);
//        layout.addView(progressBar);
//    }
//
//    public static void busyProgressBarEx(Activity activity, boolean visible) {
//
//        if (activity == null) return;
//
//        ViewGroup viewGroup = (ViewGroup) activity.findViewById(R.id.rootView);
//
//        if (viewGroup == null) {
//            MessageManager.showMessage("android:id=@+id/rootView into activity parent relative view", ErrorSeverity.Low);
//            return;
//        }
//
//        View progressBar = viewGroup.findViewWithTag("busyProgressBar");
//
//        if (progressBar != null) {
//            if (visible == false) {
//                progressBar.setVisibility(View.GONE);
//                viewGroup.removeView(progressBar);
//                return;
//            }
//        }
//
//        if (visible == false) return;
//
//        if ((progressBar != null) && (visible == true)) return;
//
//        //progressBar = new ProgressBar(activity, null, android.R.attr.progressBarStyleHorizontal);
//        progressBar = new ProgressBar(activity, null, android.R.attr.progressBarStyleLargeInverse);
//        ((ProgressBar) progressBar).setIndeterminate(true);
//        //((ProgressBar) progressBar).setIndeterminateDrawable(activity.getResources().getDrawable(R.drawable.circular_progress_bar));
//
//        progressBar.setTag("busyProgressBar");
//        progressBar.setVisibility(View.VISIBLE);
//        //progressBar.setBackground(activity.getResources().getDrawable(R.drawable.circle_shape));
//        //RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(200, 200);
//        RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(100, 100);
//        layoutParams.addRule(RelativeLayout.CENTER_IN_PARENT, RelativeLayout.TRUE);
//        progressBar.setLayoutParams(layoutParams);
//        viewGroup.addView(progressBar);
//    }
//
//    public static Activity getActivity() {
//
//        Class activityThreadClass = null;
//        try {
//            activityThreadClass = Class.forName("android.app.ActivityThread");
//
//            Object activityThread = activityThreadClass.getMethod("currentActivityThread").invoke(null);
//            Field activitiesField = activityThreadClass.getDeclaredField("mActivities");
//            activitiesField.setAccessible(true);
//
//            Map<Object, Object> activities = (Map<Object, Object>) activitiesField.get(activityThread);
//            if (activities == null)
//                return null;
//
//            for (Object activityRecord : activities.values()) {
//                Class activityRecordClass = activityRecord.getClass();
//                Field pausedField = activityRecordClass.getDeclaredField("paused");
//                pausedField.setAccessible(true);
//                if (!pausedField.getBoolean(activityRecord)) {
//                    Field activityField = activityRecordClass.getDeclaredField("activity");
//                    activityField.setAccessible(true);
//                    Activity activity = (Activity) activityField.get(activityRecord);
//                    return activity;
//                }
//            }
//        } catch (ClassNotFoundException e) {
//            return null;
//        } catch (NoSuchMethodException e) {
//            return null;
//        } catch (IllegalAccessException e) {
//            return null;
//        } catch (InvocationTargetException e) {
//            return null;
//        } catch (NoSuchFieldException e) {
//            return null;
//        }
//
//        return null;
//    }
//
//    public static void setDeviceTime(Activity activity, Date date) {
//
////        Calendar date = new GregorianCalendar(newDate);
////        int year = date.get(Calendar.YEAR);  // 2012
////        int month = date.get(Calendar.MONTH);  // 9 - October!!!
////        int day = date.get(Calendar.DAY_OF_MONTH);  // 5
//
//        Calendar calendar = Calendar.getInstance();
//        calendar.set(date.getYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds());
//        AlarmManager am = (AlarmManager) activity.getSystemService(Context.ALARM_SERVICE);
//        am.setTime(calendar.getTimeInMillis());
//    }
//
////    public static String getDeviceId() {
////        return "359711071680978";
////    }
////
////    public static String getDeviceSerialNumber() {
////        return "CQGQ8T9SL7E6ZSMR";
////    }
//
//    public static String getDeviceId() {
//
//        TelephonyManager telephonyManager = (TelephonyManager) App.getContext().getSystemService(Context.TELEPHONY_SERVICE);
//        String deviceId = telephonyManager.getDeviceId();
//
//        if (deviceId == null) {
//            deviceId = Settings.Secure.getString(App.getContext().getContentResolver(), Settings.Secure.ANDROID_ID);
//        }
//
//        if (deviceId == null){
//            MessageManager.showMessage("getDeviceId() failed", ErrorSeverity.High);
//        }
//
//        return  deviceId;
//    }
//
//    public static String getDeviceSerialNumber(){
//        String serial = null;
//
//        try {
//            Class<?> c = Class.forName("android.os.SystemProperties");
//            Method get = c.getMethod("get", String.class);
//            serial = (String) get.invoke(c, "ro.serialno");
//            return serial;
//        } catch (Exception ignored) {
//            return null;
//        }
//    }
//
//    public static String exceptionMessage(Exception e, String additionInfo) {
//        String cause;
//
//        try {
//            cause = e.getCause().getCause().getMessage();
//        } catch (Exception ex) {
//            cause = Constants.EMPTY_STRING;
//        }
//
//        if (e.getMessage() == null) {
//            return String.format("%s: %s: %s", additionInfo, cause, e.toString());
//        } else
//            return String.format("%s: %s: %s", additionInfo, cause, e.getMessage());
//    }
//
//    public static Date UtcNow() {
//        Calendar calendar = Calendar.getInstance(TimeZone.getTimeZone("GMT"));
//        return calendar.getTime();
//    }
//
//    public static double stringToDouble(String value) {
//        try {
//            return Double.parseDouble(value);
//        } catch (Exception e) {
//            return 0;
//        }
//    }
//
//    public static int stringToInt(String value) {
//        try {
//            return Integer.parseInt(value);
//        } catch (Exception e) {
//            return 0;
//        }
//    }
//
//    public static long stringToLong(String value) {
//        try {
//            return Long.parseLong(value);
//        } catch (Exception e) {
//            return 0;
//        }
//    }
//
//    public static long rectifyTelephoneNumber(String telephoneNumber) {
//
//        String rectifiedTelephoneNumber = telephoneNumber.replaceAll("[^0-9]", "");
//        rectifiedTelephoneNumber = rectifiedTelephoneNumber.replaceFirst("^0+(?!$)", "");
//
//        try {
//            return Long.parseLong(rectifiedTelephoneNumber);
//        } catch (Exception e) {
//            return -1;
//        }
//    }
//
//    public static String removeNonLetters(String telephoneNumber) {
//        return telephoneNumber.replaceAll("[^\\p{L}\\p{Nd} ]+", "").trim();
//    }
//
//    public static void writeBufferToFile(byte[] buffer, String filename) throws IOException {
//        File file = new File(filename);
//        FileOutputStream fileOutputStream = null;
//
//        try {
//            fileOutputStream = new FileOutputStream(file);
//            fileOutputStream.write(buffer);
//        } finally {
//            if (fileOutputStream != null) {
//                fileOutputStream.close();
//            }
//        }
//    }
//
//    public static byte[] readBufferFromFile(String filename) throws IOException {
//
//        File file = Utilities.getTicketFile(filename);
//        if (file == null) return null;
//
//        //String path = file.getAbsolutePath();
//        //File file = new File(filename);
//        int fileSize = (int) file.length();
//
//        byte fileData[] = new byte[fileSize];
//        byte partialData[] = new byte[fileSize];
//        FileInputStream inputStream = null;
//
//        int totalBytesRead = 0;
//
//        try {
//            inputStream = new FileInputStream(file.getAbsoluteFile());
//            do {
//                int bytesRead = (inputStream.read(partialData, totalBytesRead == 0 ? 0 : totalBytesRead - 1, fileSize - totalBytesRead));
//                System.arraycopy(partialData, 0, fileData, totalBytesRead == 0 ? 0 : totalBytesRead - 1, bytesRead);
//                totalBytesRead += bytesRead;
//            } while (totalBytesRead != fileSize);
//
//            return fileData;
//        } finally {
//            if (inputStream != null) {
//                inputStream.close();
//            }
//        }
//    }
//
//    public static File getTicketFile(String filename) {
//        File ticktetFolder = Environment.getExternalStoragePublicDirectory(Constants.ITICKET_FOLDER);
//        if (!ticktetFolder.isDirectory()) {
//            ticktetFolder.mkdir();
//        }
//
//        return new File(ticktetFolder + String.format("/%s", filename));
//    }
//
//    public static byte[] getFileData(File evidenceFile) throws IOException {
//
//        String filename = evidenceFile.getAbsolutePath();
//        File file = new File(filename);
//        int fileSize = (int) file.length();
//
//        byte fileData[] = new byte[fileSize];
//        byte partialData[] = new byte[fileSize];
//        FileInputStream inputStream = null;
//
//        int totalBytesRead = 0;
//
//        try {
//            inputStream = new FileInputStream(filename);
//            do {
//                int bytesRead = (inputStream.read(partialData, totalBytesRead == 0 ? 0 : totalBytesRead - 1, fileSize - totalBytesRead));
//                System.arraycopy(partialData, 0, fileData, totalBytesRead == 0 ? 0 : totalBytesRead - 1, bytesRead);
//                totalBytesRead += bytesRead;
//            } while (totalBytesRead != fileSize);
//
//            return fileData;
//        } finally {
//            if (inputStream != null) {
//                inputStream.close();
//            }
//        }
//    }
//
//    public static int indexOf(final Adapter adapter, String value) {
//        for (int index = 0, count = adapter.getCount(); index < count; ++index) {
//            if (adapter.getItem(index).toString().equals(value)) {
//                return index;
//            }
//        }
//        return -1;
//    }
//
//    public static List<String> getRegexMatches(String inputString, String regEx) {
//
//        List<String> list = new ArrayList<>();
//        Pattern pattern = Pattern.compile(regEx);
//
//        Matcher matcher = pattern.matcher(inputString);
//
//        while (matcher.find()) {
//            list.add(matcher.group());
//        }
//
//        return list;
//    }
//
//    public static boolean updateApkExists(String apkFileName) {
//        final PackageManager packageManager = App.getContext().getPackageManager();
//        File file = Utilities.getTicketFile(apkFileName);
//        PackageInfo info = packageManager.getPackageArchiveInfo(file.getPath(), 0);
//
//        if (info != null) {
//            if (BuildConfig.VERSION_CODE < info.versionCode) {
//                return true;
//            }
//        }
//        return false;
//    }
//
//    public static boolean installApk(String packageName) {
//
//        int installedAppVersionCode = getInstalledAppVersion(packageName);
//
//        String apkFileName = String.format("%s.apk", packageName);
//        int downloadedAppVersionCode = getDownloadedApkVersion(apkFileName);
//
//        return (installedAppVersionCode < downloadedAppVersionCode);
//    }
//
//    public static int getDownloadedApkVersion(String apkFileName){
//        final PackageManager packageManager = App.getContext().getPackageManager();
//        File file = Utilities.getTicketFile(apkFileName);
//        PackageInfo info = packageManager.getPackageArchiveInfo(file.getPath(), 0);
//        if (info != null) {
//            return info.versionCode;
//        }
//        return -1;
//    }
//
//    public static int getInstalledAppVersion(String packageName){
//        try {
//            PackageInfo pInfo = App.getContext().getPackageManager().getPackageInfo(packageName, 0);
//            return pInfo.versionCode;
//        }catch (PackageManager.NameNotFoundException e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "console.Utilities::getInstalledAppVersion(), Name not found exception"), ErrorSeverity.High);
//            return -1;
//        }
//    }
//
//    public static Resources getResourceFromApplication(Context context, String packageName){
//        try {
//            return context.getPackageManager().getResourcesForApplication(packageName);
//        }catch (PackageManager.NameNotFoundException e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "console.Utilities::getResourceFromApplication(), Name not found exception"), ErrorSeverity.High);
//            return null;
//        }
//    }
//
//    public static void setActionBar(AppCompatActivity activity, String subTextA) {
//
//        activity.getSupportActionBar().setDisplayShowTitleEnabled(false);
//        View view = activity.getLayoutInflater().inflate(R.layout.custom_action_bar, null);
//
//        TextView textView = (TextView) view.findViewById(R.id.subText);
//        textView.setText(subTextA);
//
//        Toolbar.LayoutParams layout = new Toolbar.LayoutParams(Toolbar.LayoutParams.FILL_PARENT, Toolbar.LayoutParams.FILL_PARENT);
//        activity.getSupportActionBar().setCustomView(view, layout);
//
//        activity.getSupportActionBar().setDisplayShowCustomEnabled(true);
//        activity.getSupportActionBar().setCustomView(view);
//
//        Toolbar parent = (Toolbar) view.getParent();
//        parent.setPadding(0, 0, 0, 0);
//        parent.setContentInsetsAbsolute(0, 0);
//    }
//
////    public static UserModel getSetupUser() throws SQLException {
////        UserModel user = UserRepository.getSetupUser();
////
////        if (user != null) {
////            SessionModel.getInstance().setUserName(user.getUserName());
////            SessionModel.getInstance().setPassword(user.getPassword());
////            SessionModel.getInstance().setInfrastructureNumber(user.getInfrastructureNumber());
////        }
////
////        return user;
////    }
//
//    public static UserModel getDummyUser(){
//        UserModel user = new UserModel();
//
//        user.setFirstName("Robert");
//        user.setLastName("Pattersen");
//
//        return user;
//    }
//
//    public static <T> T  deserializeXml(String aliasType, Class type, Converter converter, String xml){
//
//        XStream xStream = new XStream();
//        xStream.aliasType(aliasType, type);
//        xStream.aliasSystemAttribute("", "class");
//        xStream.registerConverter(converter);
//
//        return (T) xStream.fromXML(xml);
//    }
//
//    public static void playNotificationTone() {
//        try {
//            Uri notification = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
//            Ringtone r = RingtoneManager.getRingtone(App.getContext(), notification);
//            r.play();
//        } catch (Exception e) {
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "playNotificationTone"), ErrorSeverity.High);
//        }
//    }
//
//    public static Long getDistrictID(){
//
//        try {
//            DistrictModel district = DistrictRepository.getDistrict();
//            if (district == null){
//                return (long)-1;
//            }
//            return district.getID();
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "getDistrictCode()"), ErrorSeverity.High);
//            return (long)-1;
//        }
//    }
//
//    public static boolean copyDatabaseFile(){
//
//        try {
//            File iTicketFolder = Environment.getExternalStoragePublicDirectory(Constants.ITICKET_FOLDER);
//            File data = Environment.getDataDirectory();
//
//            if (iTicketFolder.canWrite()) {
//                String currentDBPath = "/data/data/za.co.kapsch.iticket/databases/iTicket.db";
//                String backupDBPath = String.format("%s.db", getDeviceId());
//                File currentDB = new File(currentDBPath);
//                File backupDB = new File(iTicketFolder, backupDBPath);
//
//                if (backupDB.exists()){
//                    backupDB.delete();
//                }
//
//                if (currentDB.exists()) {
//                    FileChannel src = new FileInputStream(currentDB).getChannel();
//                    FileChannel dst = new FileOutputStream(backupDB).getChannel();
//                    dst.transferFrom(src, 0, src.size());
//                    src.close();
//                    dst.close();
//                }
//           }
//        } catch (Exception e) {
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "Utilities::copyDatabaseFile()"), ErrorSeverity.High);
//            return false;
//        }
//
//        return true;
//    }
//
//    public static boolean overwriteDatabaseFile(){
//
//        try {
//            File iTicketFolder = Environment.getExternalStoragePublicDirectory(Constants.ITICKET_FOLDER);
//
//            if (iTicketFolder.canWrite()) {
//                String destinationDBPath = "/data/data/za.co.kapsch.iticket/databases/iTicket.db";
//                String sourceDBName = "iTicket.db";
//                File destinationDB = new File(destinationDBPath);
//                File sourceDB = new File(iTicketFolder, sourceDBName);
//
//                if (destinationDB.exists()){
//                    destinationDB.delete();
//                }
//
//                if (sourceDB.exists()) {
//                    FileChannel src = new FileInputStream(sourceDB).getChannel();
//                    FileChannel dst = new FileOutputStream(destinationDB).getChannel();
//                    dst.transferFrom(src, 0, src.size());
//                    src.close();
//                    dst.close();
//                }
//            }
//        } catch (Exception e) {
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "Utilities::overwriteDatabaseFile()"), ErrorSeverity.High);
//            return false;
//        }
//
//        return true;
//    }
//
//    public static boolean validateSaIdNumber(String idNumber){
//
//        try {
//            if (idNumber.length() != 13) return false;
//
//            String dateOfBirth = idNumber.substring(0, 6);
//
//            if (validateDate(dateOfBirth) == false) return false;
//
//            int citizenship = Integer.parseInt(idNumber.substring(10, 11));
//            if ((citizenship != 0) && (citizenship != 1)) return false;
//
//            return (getControlDigit(idNumber) == Integer.parseInt(idNumber.substring(12,13)));
//
//        }catch (Exception e){
//            return false;
//        }
//    }
//
//    public static int getControlDigit(String idNumber)
//    {
//        int d = -1;
//        try {
//
//            int a = 0;
//            for(int i = 0; i < 6; i++){
//                a += Integer.parseInt(idNumber.substring(i*2, i*2+1));
//            }
//
//            int b = 0;
//            for(int i = 0; i < 6; i++){
//                b = b*10 + Integer.parseInt(idNumber.substring(i*2+1, i*2+2));
//            }
//            b *= 2;
//
//            int c = 0;
//            do
//            {
//                c += b % 10;
//                b = b / 10;
//            }
//            while(b > 0);
//
//            c += a;
//            d = 10 - (c % 10);
//            if(d == 10) d = 0;
//        }
//        catch(Exception e) {
//        }
//        return d;
//    }
//
//    public static boolean validateDate(String date){
//
//        String day = date.substring(4, 6);
//        String month = date.substring(2, 4);
//        int year = Integer.parseInt(date.substring(0, 2));
//
//        if (day.equals("31") &&
//                (month.equals("4") || month .equals("6") || month.equals("9") ||
//                 month.equals("11") || month.equals("04") || month .equals("06") ||
//                 month.equals("09"))) {
//            return false; // only 1,3,5,7,8,10,12 has 31 days
//        } else if (month.equals("2") || month.equals("02")) {
//            //leap year
//            if(year % 4==0){
//                if(day.equals("30") || day.equals("31")){
//                    return false;
//                } else{
//                    return true;
//                }
//            } else{
//                if(day.equals("29")||day.equals("30")||day.equals("31")){
//                    return false;
//                } else{
//                    return true;
//                }
//            }
//        }
//
//        return (Integer.parseInt(month) < 13);
//    }
//
//    public static void hideKeyboard(Activity activity) {
//        InputMethodManager inputMethodManager = (InputMethodManager) activity.getSystemService(Activity.INPUT_METHOD_SERVICE);
//        //Find the currently focused view, so we can grab the correct window token from it.
//        View view = activity.getCurrentFocus();
//        //If no view currently has focus, create a new one, just so we can grab a window token from it
//        if (view == null) {
//            view = new View(activity);
//        }
//        inputMethodManager.hideSoftInputFromWindow(view.getWindowToken(), 0);
//    }
//
//    public static void hideLinearLayout(LinearLayout linearLayout){
//
//        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
//        layoutParams.height = 0;
//        layoutParams.width = 0;
//        linearLayout.setLayoutParams(layoutParams);
//    }
//
//    public static void showLinearLayout(LinearLayout linearLayout){
//
//        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
//        layoutParams.height = ViewGroup.LayoutParams.WRAP_CONTENT;
//        layoutParams.width = ViewGroup.LayoutParams.MATCH_PARENT;
//        linearLayout.setLayoutParams(layoutParams);
//    }
//
//    public static boolean validateIPAddressPort(String ipAddressPort){
//        String[] ipAddress = ipAddressPort.split(":");
//
//        if (ipAddress.length != 2){
//            return false;
//        }
//
//        boolean ipAddressValid = validateIPAddress(ipAddress[0].trim());
//
//        boolean portValid = false;
//        try {
//            portValid = validatePort(Integer.parseInt(ipAddress[1].trim()));
//        }catch (Exception e){
//            return false;
//        }
//
//        return ipAddressValid && portValid;
//    }
//
//    public static boolean validateIPAddress(String ipAddress){
//        return IP_ADDRESS.matcher(ipAddress).matches();
//    }
//
//    public static boolean validatePort(int port){
//        return ( port <= 65535 && port >= 0 );
//    }
//
//    public static boolean validateDateEx(final String date){
//
//        Pattern pattern = Pattern.compile(DATE_PATTERN);
//        Matcher matcher;
//
//        matcher = pattern.matcher(date);
//
//        if(matcher.matches()){
//            matcher.reset();
//
//            if(matcher.find()){
//                String day = matcher.group(1);
//                String month = matcher.group(2);
//                int year = Integer.parseInt(matcher.group(3));
//
//                if (day.equals("31") &&
//                        (month.equals("4") || month .equals("6") || month.equals("9") ||
//                                month.equals("11") || month.equals("04") || month .equals("06") ||
//                                month.equals("09"))) {
//                    return false; // only 1,3,5,7,8,10,12 has 31 days
//                }
//
//                else if (month.equals("2") || month.equals("02")) {
//                    //leap year
//                    if(year % 4==0){
//                        if(day.equals("30") || day.equals("31")){
//                            return false;
//                        }
//                        else{
//                            return true;
//                        }
//                    }
//                    else{
//                        if(day.equals("29")||day.equals("30")||day.equals("31")){
//                            return false;
//                        }
//                        else{
//                            return true;
//                        }
//                    }
//                }
//
//                else{
//                    return true;
//                }
//            }
//
//            else{
//                return false;
//            }
//        }
//        else{
//            return false;
//        }
//    }
// }
