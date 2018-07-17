package za.co.kapsch.iticket.Gestures;

import android.content.Context;
import android.view.GestureDetector;
import android.view.GestureDetector.SimpleOnGestureListener;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewParent;

/**
 * Created by CSenekal on 2017/05/30.
 */
public class OnSwipeTouchListener implements View.OnTouchListener {

    private ViewParent mParent;
    private GestureDetector mGestureDetector = null;

    public OnSwipeTouchListener (Context ctx, ViewParent parent){
        mParent = parent;
        mGestureDetector = new GestureDetector(ctx, new GestureListener());
    }

    @Override
    public boolean onTouch(View view, MotionEvent motionEvent) {

        mParent.requestDisallowInterceptTouchEvent(true);

//        switch (motionEvent.getAction() & MotionEvent.ACTION_MASK) {
//            case MotionEvent.ACTION_UP:
//                mParent.requestDisallowInterceptTouchEvent(false);
//                break;
//        }

        //return mGestureDetector.onTouchEvent(motionEvent);


        ///

        boolean handled = mGestureDetector.onTouchEvent(motionEvent);

        if (handled == false){
            mParent.requestDisallowInterceptTouchEvent(false);
        }

        return handled;
    }

    private final class GestureListener extends SimpleOnGestureListener {

        private static final int SWIPE_THRESHOLD = 100;
        private static final int SWIPE_VELOCITY_THRESHOLD = 100;

        @Override
        public boolean onDown(MotionEvent e) {
            return true;
        }

        @Override
        public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {



            boolean result = false;
            try {
                float diffY = e2.getY() - e1.getY();
                float diffX = e2.getX() - e1.getX();
                if (Math.abs(diffX) > Math.abs(diffY)) {
                    if (Math.abs(diffX) > SWIPE_THRESHOLD && Math.abs(velocityX) > SWIPE_VELOCITY_THRESHOLD) {
                        if (diffX > 0) {
                            return onSwipeRight();
                        } else {
                            return onSwipeLeft();
                        }
                        //result = true;
                    }
                }
                else if (Math.abs(diffY) > SWIPE_THRESHOLD && Math.abs(velocityY) > SWIPE_VELOCITY_THRESHOLD) {
                    if (diffY > 0) {
                        return onSwipeBottom();
                    } else {
                        return onSwipeTop();
                    }
                    //result = true;
                }
            } catch (Exception exception) {
                exception.printStackTrace();
            }
            return result;
        }


    }

    public boolean onSwipeRight() {
        return true;
    }

    public boolean onSwipeLeft() {
        return false;
    }

    public boolean onSwipeTop() {
        return false;
    }

    public boolean onSwipeBottom() {
        return false;
    }
}