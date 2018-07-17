package za.co.kapsch.shared.Printer;

import android.graphics.Bitmap;
import android.graphics.Paint;
import android.text.TextUtils;

import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionBuilder;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.graphics.internal.ZebraImageAndroid;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.LibApp;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.R;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-08-23.
 */
public class ZebraPrinter {

    private String mMacAddress;

    public static final String LABEL_FORMAT_START = "^XA";
    public static final String UTF_8 = "^CI28";
    public static final String LABEL_ORIENTATION_NORMAL = "^PON";
    public static final String LABEL_ORIENTATION_INVERSE = "^POI";
    public static final String LABEL_LENTH = "^LL";
    public static final String LABEL_FORMAT_END = "^XZ";
    public static final String LABEL_HOME_POS = "^LH";
    public static final String FIELD_ORG_FROM_HOME = "^FO";
    public static final String FONT = "^A";
    public static final String FONT_TYPE_0_A = "0,25,20";
    public static final String FONT_TYPE_0_B = "0,20,18";
    public static final String FONT_TYPE_0_C = "0,25,25";
    public static final String FONT_TYPE_0_C_M = "0,30,30";
    public static final String FONT_TYPE_0_C_L = "0,35,35";
    public static final String FONT_TYPE_A = "A";
    public static final String FONT_TYPE_A_X2 = "A,15";
    public static final String FONT_TYPE_A_X3 = "A,30";
    public static final String FONT_TYPE_B = "B";
    public static final String FONT_TYPE_D = "D";
    public static final String FONT_TYPE_D_X2 = "D,36";
    public static final String FONT_TYPE_E = "E";
    public static final String FONT_TYPE_E_X2 = "E,56";
    public static final String FONT_TYPE_H = "H";
    public static final String FONT_TYPE_H_X2 = "H,44";
    public static final String FONT_TYPE_F = "F";
    public static final String START_FIELD_DATA = "^FD";
    public static final String END_FIELD_DATA = "^FS";
    public static final String MEDIA_DARKNESS = "^MD";
    public static final String LABEL_LENGTH_PLACE_HOLDER = "LABEL_LENGTH_PLACE_HOLDER";
    public static final String PRINT_WIDTH = "^PW400";
    public static final String DOWNLOAD_DIRECT_BITMAP = "^DD";
    public static final String IMAGE = "IMAGE";

    //public static final int mSlipDotWidth = 540;
    public static final int mSlipDotWidth = 580;

    public static final String FONT_TYPE_0_C_L_MAX_LINE_CHARS = "01234567890123456789012345678901234";
    public static final String FONT_TYPE_0_C_MAX_LINE_CHARS = "012345678901234567890123456789012345678901234567";

    private static String mError;

    private int mTopOffset;
    private List<PrintSection> mPrintSections = new ArrayList<>();
    private StringBuilder mStringBuilder;

    public ZebraPrinter(String macAddress){
        mMacAddress = macAddress;
    }

    public static String addField(int left, int top, String fontType, String text) {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.append(FIELD_ORG_FROM_HOME);
        stringBuilder.append(String.format("%1$d,%2$d", left, top));
        stringBuilder.append(FONT);
        stringBuilder.append(fontType);
        stringBuilder.append(START_FIELD_DATA);
        stringBuilder.append(text);
        stringBuilder.append(END_FIELD_DATA);

        return stringBuilder.toString();
    }

    public static byte[] setLabelLength(int labelLength){
        return String.format("%s%s%s%d%s",
                ZebraPrinter.LABEL_FORMAT_START,
                ZebraPrinter.UTF_8,
                ZebraPrinter.LABEL_LENTH,
                labelLength,
                ZebraPrinter.LABEL_FORMAT_END).getBytes();
    }

//    public static byte[] setLabelLength(int labelLength){
//        return String.format("%s%s%d%s",
//                ZebraPrinter.LABEL_FORMAT_START,
//                ZebraPrinter.LABEL_LENTH,
//                labelLength,
//                ZebraPrinter.LABEL_FORMAT_END).getBytes();
//    }

    public static String initialiseLabel(boolean finalSection){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.append(LABEL_FORMAT_START);
        stringBuilder.append(UTF_8);
        stringBuilder.append(LABEL_ORIENTATION_INVERSE);
        stringBuilder.append(String.format("%s%s", LABEL_LENTH, LABEL_LENGTH_PLACE_HOLDER));
        stringBuilder.append(setHomePosition(0, finalSection ? 50 : 0));
        return stringBuilder.toString();
    }

    public static int getLeftOffsetToCenterTextEx(String paperWidthText, String text){
        float paperWidth = new Paint().measureText(paperWidthText);
        float textWith = new Paint().measureText(text);
        float ratio = textWith / paperWidth;
        float leftOffset = (mSlipDotWidth - (mSlipDotWidth * ratio)) / 2;
        return (int)leftOffset-10;
    }

    public static String printImage(Connection connection, Bitmap image, int leftOffset, int topOffset, int width, int height){
        try {
            com.zebra.sdk.printer.ZebraPrinter printer = ZebraPrinterFactory.getInstance(connection);
            printer.printImage(new ZebraImageAndroid(image), leftOffset, topOffset, width, height, false);
        } catch (ConnectionException e) {
            return e.getMessage();
        } catch (ZebraPrinterLanguageUnknownException e) {
            return e.getMessage();
        } catch (Exception e){
            return e.getMessage();
        }
        return "";
    }

    public static String finaliseLabel() {
        return  LABEL_FORMAT_END;
    }


    public void addTextItem(String text, String font, int left, boolean newLine, int sectionIndex) {
        addTextItem(text, font, left, newLine, sectionIndex, true, true, false);
    }

    public void addTextItemOmitNA(String text, String font, int left, boolean newLine, int sectionIndex) {
        addTextItem(text, font, left, newLine, sectionIndex, false, true, false);
    }

    public void addTextItem(String text, String font, int left, boolean newLine, int sectionIndex, boolean wrapText) {
        addTextItem(text, font, left, newLine, sectionIndex, true, wrapText, false);
    }

    public void addTextItem(String text, String font, int left, boolean newLine, int sectionIndex, boolean wrapText, boolean omitLineOnNullOrEmptyText) {
        addTextItem(text, font, left, newLine, sectionIndex, true, wrapText, omitLineOnNullOrEmptyText);
    }

    //if left = -1 the field is centered.
    private void addTextItem(String text, String font, int left, boolean newLine, int sectionIndex, boolean notAvailableIndicator, boolean wrapText, boolean omitLineOnNullOrEmptyText) {

        if (omitLineOnNullOrEmptyText == true){
            if (TextUtils.isEmpty(text) == true){
                return;
            }
        }

        if (TextUtils.isEmpty(text)) {
            if (notAvailableIndicator == true) {
                text = LibApp.getContext().getResources().getString(R.string.n_a);
            }else{
                return;
            }
        }

        if (mPrintSections.size() < sectionIndex + 1){
            mPrintSections.add(new PrintSection());
        }

        PrintSection printSection = mPrintSections.get(sectionIndex);

        int maxChars = maxCharsPerLineByOffset(font, left);

        List<String> lines = null;
        if (wrapText == true) {
            lines = Utilities.wrapTextOnLength(text, maxChars);
        }else{
            lines = new ArrayList<>();
            lines.add(text);
        }

        if (lines == null) return;

        for(int i = 0; i < lines.size(); i++){
            if (i == 0 ) {
                if (newLine == true) {
                    mTopOffset = mTopOffset + getFontLineHeight(font);
                }
            }else{
                mTopOffset = mTopOffset + getFontLineHeight(font);
            }

            printSection.addPrintItem(
                    new PrintItem(mTopOffset,
                            (left == -1) ? centreLabel(lines.get(i), font) : left,
                            lines.get(i),
                            font,
                            null,
                            0,
                            0));
        }
    }

    public void addImageItem(Bitmap image, int width, int height, int left, int sectionIndex){
        if (mPrintSections.size() < sectionIndex + 1){
            mPrintSections.add(new PrintSection());
        }

        PrintSection printSection = mPrintSections.get(sectionIndex);
        printSection.addPrintItem(new PrintItem(0, left, null, null, image, width, height));
    }

    public void setTopOffset(int topOffset){
        mTopOffset = topOffset;
    }

    public void print() throws Exception {
        Connection printerConn = null;
        try {
            printerConn = ConnectionBuilder.build("BT:" + mMacAddress);
            if (printerConn != null) {
                printerConn.open();
            }
            print(printerConn);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }finally {
            if (printerConn != null) {
                printerConn.close();
            }
         }
    }

    private static String setHomePosition(int left, int top) {
        return String.format("%1$s%2$d,%3$d", LABEL_HOME_POS, left, top);
    }

    //mediaDarknessValue -30 to +30
    private static String adjustMediaDarkness(String mediaDarknessValue) {
        return String.format("%1$s%2$s", MEDIA_DARKNESS, mediaDarknessValue);
    }

    private void print(Connection printerConnection) throws ConnectionException {
        String error = "";
        for(int i = 0; i < mPrintSections.size(); i ++) {
            mStringBuilder = new StringBuilder();
            print(printerConnection, mPrintSections.get(i).getPrintItems(), (i == (mPrintSections.size()-1)));
        }
    }

    private void print(Connection printerConnection, List<PrintItem> printItems, boolean finalSection) throws ConnectionException {
        if (printItems.size() > 0) {
            if (printItems.get(0).getImage() == null) {
                printText(printerConnection, printItems, finalSection);
            } else {
                printImage(printerConnection, printItems);
            }
        }
    }

    private void printText(Connection printerConnection, List<PrintItem> printItems, boolean finalSection) throws ConnectionException {
        //try {

            mStringBuilder.append(ZebraPrinter.initialiseLabel(finalSection));

            for (int i = 0; i < printItems.size(); i++) {
                if (printItems.get(i).getImage() == null) {
                    mStringBuilder.append(
                            ZebraPrinter.addField(printItems.get(i).getLeft(),
                                    printItems.get(i).getTop(),
                                    printItems.get(i).getFont(),
                                    printItems.get(i).getText()));
                }
            }

            mStringBuilder.append(ZebraPrinter.finaliseLabel());

            String printerCommandText = mStringBuilder.toString().
                    replace(ZebraPrinter.LABEL_LENGTH_PLACE_HOLDER,
                            Integer.toString(printItems.get(printItems.size() - 1).getTop() + (finalSection ? 80 : 30))); //80 for section56

            printerConnection.write(printerCommandText.getBytes());

//        } catch (ConnectionException e) {
//            return e.getMessage();
//        }
//
//        return "";
    }

    private String printImage(Connection printerConnection, List<PrintItem> printItems) {

        String error = "";

        for (int i = 0; i < printItems.size(); i++) {
            try {
                //Image print height
                //printerConnection.write(ZebraPrinter.setLabelLength(120)); //120 for section56
                printerConnection.write(ZebraPrinter.setLabelLength(printItems.get(i).getImageHeight())); //120 for section56
            } catch (ConnectionException e) {
                e.printStackTrace();
            }

            int height = printItems.get(i).getImageHeight();
            int top = printItems.get(i).getTop();

            error = ZebraPrinter.printImage(
                    printerConnection,
                    printItems.get(i).getImage(),
                    printItems.get(i).getLeft(),
                    printItems.get(i).getTop(),
                    printItems.get(i).getImageWidth(),
                    printItems.get(i).getImageHeight());
        }

        return error;
    }

    private int centreLabel(String text, String font) {
        double textWidth = text.length() * getFontWidth(font) + (text.length() - 1) * getFontGapWidth(font);
        double left = (mSlipDotWidth - textWidth) / 2;
        return (int)left;
    }

    private int maxCharsPerLineByOffset(String font, int leftOffset){

        int maxCharsPerLine = getFontMaxCharsPerLine(font);

        float lineRatio = (float)((float)(ZebraPrinter.mSlipDotWidth - ((leftOffset == -1) ? 0 : leftOffset)) / (float) mSlipDotWidth);

        return (int)(maxCharsPerLine * lineRatio);
    }

    private static int getFontMaxCharsPerLine(String font) {
        switch (font) {
            case FONT_TYPE_D:
                return 10;
            case FONT_TYPE_D_X2:
                return 19;//18
            case FONT_TYPE_F:
                return 30;
            case FONT_TYPE_0_A:
                return 60;//50
            case FONT_TYPE_0_B:
                return 90;
            case FONT_TYPE_0_C:
                return 53;
            case FONT_TYPE_0_C_M:
                return 40;
            case FONT_TYPE_0_C_L:
                return 33;
            case FONT_TYPE_A:
                return 70;
            case FONT_TYPE_A_X2:
                return 70;
            case FONT_TYPE_A_X3:
                return 60;
            default:
                return -1;
        }
    }

    private static int getFontLineHeight(String font) {
        switch (font) {
            case FONT_TYPE_A:
                return 15;
            case FONT_TYPE_A_X2:
                return 18;
            case FONT_TYPE_A_X3:
                return 30;
            case FONT_TYPE_D:
                return 20;
            case FONT_TYPE_D_X2:
                return 40;
            case FONT_TYPE_F:
                return 40;
            case FONT_TYPE_0_A:
                return 30;
            case FONT_TYPE_0_B:
                return 25;
            case FONT_TYPE_0_C:
                return 30;
            case FONT_TYPE_0_C_M:
                return 31;
            case FONT_TYPE_0_C_L:
                return 32;
            case IMAGE:
                return 40;
            default:
                return -1;
        }
    }

    private static double getFontWidth(String font) {
        switch (font) {
            case FONT_TYPE_A:
                return 5;
            case FONT_TYPE_A_X2:
                return 10;
            case FONT_TYPE_A_X3:
                return 15;
            case FONT_TYPE_B:
                return 7;
            case FONT_TYPE_D:
                return 10;
            case FONT_TYPE_D_X2:
                return 20;
            case FONT_TYPE_E:
                return 15;
            case FONT_TYPE_E_X2:
                return 30;
            case FONT_TYPE_F:
                return 13;
            case FONT_TYPE_H:
                return 13;
            case FONT_TYPE_H_X2:
                return 26;
            case FONT_TYPE_0_A: //changes this from 20 to new value
                return 20;
            case FONT_TYPE_0_B:
                return 6.5;
            case FONT_TYPE_0_C:
                return 16;
            case FONT_TYPE_0_C_M:
                return 17;
            case FONT_TYPE_0_C_L:
                return 18;
            default:
                return -1;
        }
    }

    private static int getFontGapWidth(String font) {
        switch (font) {
            case FONT_TYPE_A:
                return 1;
            case FONT_TYPE_A_X2:
                return 2;
            case FONT_TYPE_A_X3:
                return 3;
            case FONT_TYPE_B:
                return 2;
            case FONT_TYPE_D:
                return 2;
            case FONT_TYPE_D_X2:
                return 4;
            case FONT_TYPE_E:
                return 5;
            case FONT_TYPE_E_X2:
                return 10;
            case FONT_TYPE_F:
                return 3;
            case FONT_TYPE_H:
                return 6;
            case FONT_TYPE_H_X2:
                return 12;
            case FONT_TYPE_0_A:
                return 2; //??This is a guess
            case FONT_TYPE_0_B:
                return 1;
            case FONT_TYPE_0_C:
                return 2;
            case FONT_TYPE_0_C_L:
                return 4;
            default:
                return -1;
        }
    }
}
