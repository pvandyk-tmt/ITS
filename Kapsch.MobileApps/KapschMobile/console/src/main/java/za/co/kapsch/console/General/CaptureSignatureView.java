package za.co.kapsch.console.General;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Path;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.View;

import java.io.ByteArrayOutputStream;
import java.nio.ByteBuffer;

import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Interfaces.ICallBack;

/**
 * From http://stackoverflow.com/questions/7228191/android-signature-capture - 'Pierre'
 */
public class CaptureSignatureView  extends View {

    private Bitmap mBitmap;
    private Canvas mCanvas;
    private Path mPath;
    private Paint mBitmapPaint;
    private Paint mPaint;
    private float mX;
    private float mY;
    private float mTouchTolerance = 4;
    private float mLineThickness = 4;
    private ICallBack mCallBack;

    public CaptureSignatureView(Context context, AttributeSet attr, ICallBack callBack) {
        super(context, attr);
        mPath = new Path();
        mBitmapPaint = new Paint(Paint.DITHER_FLAG);
        mPaint = new Paint();
        mPaint.setAntiAlias(true);
        mPaint.setDither(true);
        mPaint.setColor(Color.argb(255, 0, 0, 0));
        mPaint.setStyle(Paint.Style.STROKE);
        mPaint.setStrokeJoin(Paint.Join.ROUND);
        mPaint.setStrokeCap(Paint.Cap.ROUND);
        mPaint.setStrokeWidth(mLineThickness);
        mCallBack = callBack;
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
        super.onSizeChanged(w, h, oldw, oldh);
        mBitmap = Bitmap.createBitmap(w, (h > 0 ? h : ((View) this.getParent()).getHeight()), Bitmap.Config.ARGB_8888);
        mCanvas = new Canvas(mBitmap);
        mCallBack.callBackMethod();
    }

    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);
        canvas.drawColor(Color.WHITE);
        canvas.drawBitmap(mBitmap, 0, 0, mBitmapPaint);
        canvas.drawPath(mPath, mPaint);
    }

    private void TouchStart(float x, float y) {
        mPath.reset();
        mPath.moveTo(x, y);
        mX = x;
        mY = y;
    }

    private void TouchMove(float x, float y) {
        float dx = Math.abs(x - mX);
        float dy = Math.abs(y - mY);

        if (dx >= mTouchTolerance || dy >= mTouchTolerance) {
            mPath.quadTo(mX, mY, (x + mX) / 2, (y + mY) / 2);
            mX = x;
            mY = y;
        }
    }

    private void TouchUp() {
        if (!mPath.isEmpty()) {
            mPath.lineTo(mX, mY);
            mCanvas.drawPath(mPath, mPaint);
        } else {
            mCanvas.drawPoint(mX, mY, mPaint);
        }

        mPath.reset();
    }

    @Override
    public boolean onTouchEvent(MotionEvent e) {
        super.onTouchEvent(e);
        float x = e.getX();
        float y = e.getY();

        switch (e.getAction()) {
            case MotionEvent.ACTION_DOWN:
                TouchStart(x, y);
                invalidate();
                break;
            case MotionEvent.ACTION_MOVE:
                TouchMove(x, y);
                invalidate();
                break;
            case MotionEvent.ACTION_UP:
                TouchUp();
                invalidate();
                break;
        }

        return true;
    }

    public void clearCanvas() {
         mCanvas.drawColor(Color.WHITE);
        invalidate();
    }

    public byte[] getBytes() {
        try {
            Bitmap bitmap = getBitmap();

            if (bitmap == null) return null;

            //Check of blank image
            boolean blankImage = true;
            ByteBuffer buffer = ByteBuffer.allocate(bitmap.getByteCount());
            bitmap.copyPixelsToBuffer(buffer);
            byte[] imageData = buffer.array();
            for (byte pixel : imageData) {
                if (pixel != -1) {
                    blankImage = false;
                    break;
                }
            }

            if (blankImage == true) return null;
            //

            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.PNG, 90, stream);
            return stream.toByteArray();
        }catch (Exception e){
            //ignore this exception
            return null;
        }
    }

    public Bitmap getBitmap() {
        try {
            View v = (View) this.getParent();

            //if (v.getWidth() < 0 || v.getHeight() < 0) return null;

            Bitmap b = Bitmap.createBitmap(v.getWidth(), v.getHeight(), Bitmap.Config.ARGB_8888);
            Canvas c = new Canvas(b);
            v.layout(v.getLeft(), v.getTop(), v.getRight(), v.getBottom());
            v.draw(c);

            return b;
        }catch (Exception e){
            String error = Utilities.exceptionMessage(e, "CaptureSignatureView::getBitmap()");
        }

        return null;
    }

    public void setBitmap(Bitmap signature){
        if (signature == null) return;

        mCanvas.drawBitmap(signature, 0, 0, mBitmapPaint);
        invalidate();
    }

    public void setBitmap(byte[] signature){
        if (signature == null) return;

        try {
            Bitmap bitmap = BitmapFactory.decodeByteArray(signature, 0, signature.length);
            mCanvas.drawBitmap(bitmap, 0, 0, mBitmapPaint);
            invalidate();
        }catch (Exception e){
            String error = e.getMessage();
        }

    }
}