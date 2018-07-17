package za.co.kapsch.shared.Printer;

import android.graphics.Bitmap;

/**
 * Created by csenekal on 2016-08-26.
 */
public class PrintItem {
    private int mTop;
    private int mLeft;
    private String mText;
    private String mFont;
    private Bitmap mImage;
    private int mImageWidth;
    private int mImageHeight;

    public PrintItem(int top, int left, String text, String font, Bitmap image, int imageWidth, int imageHeight) {
        mTop = top;
        mLeft = left;
        mText = text;
        mFont = font;
        mImage = image;
        mImageWidth = imageWidth;
        mImageHeight = imageHeight;
    }

    public int getTop() {
        return mTop;
    }

    public int getLeft() {
        return mLeft;
    }

    public String getText() {
        return mText;
    }

    public String getFont() {
        return mFont;
    }

    public Bitmap getImage() {
        return mImage;
    }

    public int getImageWidth() {
        return mImageWidth;
    }

    public int getImageHeight() {
        return mImageHeight;
    }
}
