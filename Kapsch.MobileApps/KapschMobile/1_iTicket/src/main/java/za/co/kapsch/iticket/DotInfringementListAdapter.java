package za.co.kapsch.iticket;

import android.app.Activity;
import android.util.Base64;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.Models.SectionModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/05/23.
 */
public class DotInfringementListAdapter extends BaseAdapter {

    private int counter;
    private TextView mCounterTextView;
    //private TextView mTimeTextView;
    //private TextView mZoneTextView;
    private TextView mSpeedTextView;
    private TextView mRegistrationNumberTextView;
    private ImageView mExitImageView;
    //private ImageView mEntryImageView;
    private LinearLayout mBackgroundLinearLayout;

    private ViewParent mViewParent;
    private final Activity mActivity;

    private List<SectionModel> mSectionList;

    private TextView mNoticeNumberTextView;

    public <T> DotInfringementListAdapter(Activity activity, ViewParent viewParent, List<SectionModel> dotInfringementList) {
        super();
        mActivity = activity;
        mViewParent = viewParent;
        mSectionList = dotInfringementList;
    }

    public View getView(int position, View rowView, ViewGroup parent) {

        LayoutInflater inflater = mActivity.getLayoutInflater();

        if (rowView == null) {

            rowView = inflater.inflate(R.layout.dot_infringement_list_item, null);

            mBackgroundLinearLayout = (LinearLayout) rowView.findViewById(R.id.backgroundLinearLayout);
            //mTimeTextView = (TextView) rowView.findViewById(R.id.timeTextView);
            //mZoneTextView = (TextView) rowView.findViewById(R.id.zoneTextView);
            mSpeedTextView = (TextView) rowView.findViewById(R.id.speedTextView);
            mRegistrationNumberTextView = (TextView) rowView.findViewById(R.id.registrationTextView);
            mExitImageView = (ImageView) rowView.findViewById(R.id.exitImage);
            //mEntryImageView = (ImageView) rowView.findViewById(R.id.entryImage);
            //mCounterTextView = (TextView) rowView.findViewById(R.id.counterTextView);

            rowView.setTag(R.id.backgroundLinearLayout, mBackgroundLinearLayout);
            //rowView.setTag(R.id.timeTextView, mTimeTextView);
            //rowView.setTag(R.id.zoneTextView, mZoneTextView);
            rowView.setTag(R.id.speedTextView, mSpeedTextView);
            rowView.setTag(R.id.registrationTextView, mRegistrationNumberTextView);
            rowView.setTag(R.id.exitImage, mExitImageView);
            //rowView.setTag(R.id.entryImage, mEntryImageView);
            //rowView.setTag(R.id.counterTextView, mCounterTextView);

//            try {
//                mExitImageView.setOnTouchListener(new OnSwipeTouchListener(this.mActivity, mViewParent) {
//
//                    public boolean onSwipeTop() {
//                        MessageManager.showMessage("onSwipeTop", ErrorSeverity.None);
//                        return false;
//                    }
//
//                    public boolean onSwipeRight() {
//                        MessageManager.showMessage("onSwipeRight", ErrorSeverity.None);
//                        return true;
//                    }
//
//                    public boolean onSwipeLeft() {
//                        MessageManager.showMessage("onSwipeLeft", ErrorSeverity.None);
//                        return false;
//                    }
//
//                    public boolean onSwipeBottom() {
//                        MessageManager.showMessage("onSwipeBottom", ErrorSeverity.None);
//                        return false;
//                    }
//                });
//            }catch (Exception e){
//                MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
//            }



        } else {
            mBackgroundLinearLayout = (LinearLayout) rowView.getTag(R.id.backgroundLinearLayout);
            //mTimeTextView = (TextView) rowView.getTag(R.id.timeTextView);
            //mZoneTextView = (TextView) rowView.getTag(R.id.zoneTextView);
            mSpeedTextView = (TextView) rowView.getTag(R.id.speedTextView);
            mRegistrationNumberTextView = (TextView) rowView.getTag(R.id.registrationTextView);
            mExitImageView = (ImageView) rowView.getTag(R.id.exitImage);
            //mEntryImageView = (ImageView) rowView.getTag(R.id.entryImage);
            //mCounterTextView = (TextView) rowView.getTag(R.id.counterTextView);
        }

        setValues(position);

        if (position % 2 == 0) {
            mBackgroundLinearLayout.setBackgroundColor(App.getContext().getResources().getColor(R.color.colorBannerBackGround));
        } else {
            mBackgroundLinearLayout.setBackgroundColor(App.getContext().getResources().getColor(R.color.pressedColor));
        }

        return rowView;
    }

    private void setValues(int position) {

        mExitImageView.setImageBitmap(Utilities.byteArrayToBitmap(Base64.decode(mSectionList.get(position).getSectionPointEnd().getImage(), Base64.DEFAULT)));
        //mEntryImageView.setImageBitmap(Utilities.byteArrayToBitmap(Base64.decode(mSectionList.get(position).getSectionPointStart().getImage(), Base64.DEFAULT)));
        //mTimeTextView.setText(Utilities.timeToString(mSectionList.get(position).getSectionPointEnd().getEventDateTime()));
        //mZoneTextView.setText(mSectionList.get(position).getZone().toString());
        mSpeedTextView.setText(String.format("%.2f", mSectionList.get(position).getAverageSpeed()));
        mRegistrationNumberTextView.setText(mSectionList.get(position).getVln());
        //mCounterTextView.setText(Integer.toString(position));
    }

    @Override
    public int getCount() {

        if (mSectionList == null) return 0;

        return mSectionList.size();
    }


    @Override
    public Object getItem(int position) {

        if (mSectionList != null) {
            return mSectionList.get(position);
        }

        return null;
    }

    @Override
    public long getItemId(int position) {

        return 0;
    }

    @Override
    public int getViewTypeCount() {

        return getCount();
    }

    @Override
    public int getItemViewType(int position) {

        return position;
    }
}
