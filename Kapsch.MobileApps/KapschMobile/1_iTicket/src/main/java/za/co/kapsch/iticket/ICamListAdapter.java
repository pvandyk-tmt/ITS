package za.co.kapsch.iticket;

import android.app.Activity;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.ListItemHolders.ICamViewHolder;
import za.co.kapsch.iticket.iCam.ICamInfringement;
import za.co.kapsch.iticket.iCam.ICamEvent;
import za.co.kapsch.iticket.iCam.ICamVlns;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-12-15.
 */
public class ICamListAdapter extends ArrayAdapter<ICamEvent> {

    private final Activity mActivity;
    private final List<ICamEvent> mICamEventList;

    public ICamListAdapter(Activity activity, List<ICamEvent> iCamEventList) {
        super(activity, R.layout.icam_list_item, iCamEventList);
        // TODO Auto-generated constructor stub

        mActivity = activity;
        mICamEventList = iCamEventList;
    }

    public View getView(int position, View rowView, ViewGroup parent) {

        try {
            ICamViewHolder iCamViewHolder = null;

            LayoutInflater inflater = mActivity.getLayoutInflater();

            if (rowView == null) {

                rowView = inflater.inflate(R.layout.icam_list_item, null);

                iCamViewHolder = new ICamViewHolder();
                iCamViewHolder.mTypeTextView = (TextView) rowView.findViewById(R.id.typeTextView);
                iCamViewHolder.mInfoTextView = (TextView) rowView.findViewById(R.id.infoTextView);
                iCamViewHolder.mImageView = (ImageView) rowView.findViewById(R.id.itemImage);
                iCamViewHolder.mTimeTextView = (TextView) rowView.findViewById(R.id.timeTextView);
                iCamViewHolder.mReasonTextView = (TextView) rowView.findViewById(R.id.reasonTextView);
                rowView.setTag(iCamViewHolder);

            } else {
                iCamViewHolder = (ICamViewHolder) rowView.getTag();
            }

            ICamVlns iCamVlns = mICamEventList.get(position).getICamVlns();
            ICamInfringement iCamInfringement = mICamEventList.get(position).getICamInfringement();

            if (iCamVlns != null) {

                iCamViewHolder.mTypeTextView.setText("VOSI");
                iCamViewHolder.mInfoTextView.setText(iCamVlns.getPlate());
                iCamViewHolder.mTypeTextView.setBackgroundColor(App.getContext().getResources().getColor(R.color.vosiColor));
                iCamViewHolder.mImageView.setImageBitmap(iCamVlns.getThumbImage());
                iCamViewHolder.mTimeTextView.setText(iCamVlns.getTime());
                iCamViewHolder.mReasonTextView.setText(iCamVlns.getVosiReason());

            } else if (iCamInfringement != null) {

                if (TextUtils.isEmpty(iCamInfringement.getType()) == false) {
                    iCamViewHolder.mTypeTextView.setText(iCamInfringement.getType().toUpperCase());
                }else{
                    iCamViewHolder.mTypeTextView.setText("SPEED");
                }

                iCamViewHolder.mInfoTextView.setText(iCamInfringement.getIcamVlns() == null ? Constants.EMPTY_STRING : iCamInfringement.getIcamVlns().getPlate());
                iCamViewHolder.mTypeTextView.setBackgroundColor(App.getContext().getResources().getColor(R.color.speedColor));
                iCamViewHolder.mImageView.setImageBitmap(iCamInfringement.getThumbImage());
                iCamViewHolder.mTimeTextView.setText(iCamInfringement.getTime());
                String reason = String.format(App.getContext().getResources().getString(R.string.zone_speed), iCamInfringement.getZone(), iCamInfringement.getSpeed());
                iCamViewHolder.mReasonTextView.setText(reason);
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
        return rowView;
    };

    private void setType(ICamViewHolder iCamViewHolder, ICamInfringement iCamInfringement){
        //iCamInfringement.
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
