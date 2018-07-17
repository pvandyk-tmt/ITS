package za.co.kapsch.iticket;

import android.app.Activity;
import android.text.Spannable;
import android.text.SpannableStringBuilder;
import android.text.TextUtils;
import android.text.style.StyleSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.Models.InfringementChargeModel;
import za.co.kapsch.iticket.Models.InfringementModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/04/02.
 */
public class EndOfDayReportListAdapter extends BaseAdapter {

    private final Activity mActivity;
    private List<TicketModel> mTicketList;

    private TextView mNoticeNumberTextView;
    private TextView mDateTimeTextView;
    private TextView mVehicleTextView;
    private TextView mLocationTextView;
    private TextView mChargeOneTextView;
    private TextView mChargeTwoTextView;
    private TextView mChargeThreeTextView;
    private TextView mNotesTextView;
    private TextView mCancelledTextView;
    private LinearLayout mChargeOneLinearLayout;
    private LinearLayout mChargeTwoLinearLayout;
    private LinearLayout mChargeThreeLinearLayout;
    private LinearLayout mNotesLinearLayout;
    private LinearLayout mDetailLinearLayout;
    private LinearLayout mCancelledLinearLayout;


    public <T> EndOfDayReportListAdapter(Activity activity, List<TicketModel> ticketList) {
        super();
        mActivity = activity;
        mTicketList = ticketList;
    }

    public View getView(int position, View rowView, ViewGroup parent) {

        LayoutInflater inflater = mActivity.getLayoutInflater();

        if(rowView == null){

            rowView=inflater.inflate(R.layout.end_of_day_report_list_item, null);

            mNoticeNumberTextView = (TextView) rowView.findViewById(R.id.noticeNumberTextView);
            mDateTimeTextView = (TextView) rowView.findViewById(R.id.dateTimeTextView);
            mVehicleTextView = (TextView) rowView.findViewById(R.id.vehicleTextView);
            mLocationTextView = (TextView) rowView.findViewById(R.id.locationTextView);
            mChargeOneTextView = (TextView) rowView.findViewById(R.id.chargeOneTextView);
            mChargeTwoTextView = (TextView) rowView.findViewById(R.id.chargeTwoTextView);
            mChargeThreeTextView = (TextView) rowView.findViewById(R.id.chargeThreeTextView);
            mNotesTextView = (TextView) rowView.findViewById(R.id.notesTextView);
            mCancelledTextView = (TextView) rowView.findViewById(R.id.cancelledTextView);
            mChargeOneLinearLayout = (LinearLayout) rowView.findViewById(R.id.chargeOneLinearLayout);
            mChargeTwoLinearLayout = (LinearLayout) rowView.findViewById(R.id.chargeTwoLinearLayout);
            mChargeThreeLinearLayout = (LinearLayout) rowView.findViewById(R.id.chargeThreeLinearLayout);
            mNotesLinearLayout = (LinearLayout) rowView.findViewById(R.id.notesLinearLayout);
            mDetailLinearLayout = (LinearLayout) rowView.findViewById(R.id.detailLinearLayout);
            mCancelledLinearLayout  = (LinearLayout) rowView.findViewById(R.id.cancelledLinearLayout);

            rowView.setTag(R.id.noticeNumberTextView, mNoticeNumberTextView);
            rowView.setTag(R.id.dateTimeTextView, mDateTimeTextView);
            rowView.setTag(R.id.vehicleTextView, mVehicleTextView);
            rowView.setTag(R.id.locationTextView, mLocationTextView);
            rowView.setTag(R.id.chargeOneTextView, mChargeOneTextView);
            rowView.setTag(R.id.chargeTwoTextView, mChargeTwoTextView);
            rowView.setTag(R.id.chargeThreeTextView, mChargeThreeTextView);
            rowView.setTag(R.id.notesTextView, mNotesTextView);
            rowView.setTag(R.id.chargeOneLinearLayout, mChargeOneLinearLayout);
            rowView.setTag(R.id.chargeTwoLinearLayout, mChargeTwoLinearLayout);
            rowView.setTag(R.id.chargeThreeLinearLayout, mChargeThreeLinearLayout);
            rowView.setTag(R.id.notesLinearLayout, mNotesLinearLayout);
            rowView.setTag(R.id.detailLinearLayout, mDetailLinearLayout);
            rowView.setTag(R.id.cancelledLinearLayout, mCancelledLinearLayout);
            rowView.setTag(R.id.cancelledTextView, mCancelledTextView);
        }else {
            mNoticeNumberTextView = (TextView) rowView.getTag(R.id.noticeNumberTextView);
            mDateTimeTextView = (TextView) rowView.getTag(R.id.dateTimeTextView);
            mVehicleTextView = (TextView) rowView.getTag(R.id.vehicleTextView);
            mLocationTextView = (TextView) rowView.getTag(R.id.locationTextView);
            mChargeOneTextView = (TextView) rowView.getTag(R.id.chargeOneTextView);
            mChargeTwoTextView = (TextView) rowView.getTag(R.id.chargeTwoTextView);
            mChargeThreeTextView = (TextView) rowView.getTag(R.id.chargeThreeTextView);
            mNotesTextView = (TextView) rowView.getTag(R.id.notesTextView);
            mChargeOneLinearLayout = (LinearLayout) rowView.getTag(R.id.chargeOneLinearLayout);
            mChargeTwoLinearLayout = (LinearLayout) rowView.getTag(R.id.chargeTwoLinearLayout);
            mChargeThreeLinearLayout = (LinearLayout) rowView.getTag(R.id.chargeThreeLinearLayout);
            mNotesLinearLayout  = (LinearLayout) rowView.getTag(R.id.notesLinearLayout);
            mDetailLinearLayout   = (LinearLayout) rowView.getTag(R.id.detailLinearLayout);
            mCancelledLinearLayout = (LinearLayout) rowView.getTag(R.id.cancelledLinearLayout);
            mCancelledTextView  = (TextView) rowView.getTag(R.id.cancelledTextView);
        }

        setValues(position);

        return rowView;
    };

    private void setValues(int position){

        try {
            TicketModel ticketModel = mTicketList.get(position);

            InfringementModel infringement = ticketModel.getInfringement();

            if (infringement.getCancelled() == true){
                Utilities.showLinearLayout(mCancelledLinearLayout);
                Utilities.hideLinearLayout(mDetailLinearLayout);
                mCancelledTextView.setText(
                        String.format("%s: %s",
                            App.getContext().getResources().getString(R.string.end_of_day_report_cancelled),
                                infringement.getCancelledReason()));
            }else{
                Utilities.showLinearLayout(mDetailLinearLayout);
                Utilities.hideLinearLayout(mCancelledLinearLayout);
            }

            InfringementChargeModel[] infringementCharges = infringement.getInfringementCharges();

            mNoticeNumberTextView.setText(infringement.getTicketNumber());

            mDateTimeTextView.setText(Utilities.dateTimeToString(infringement.getOffenceDate()));

            VehicleModel vehicle = ticketModel.getVehicle();
            mVehicleTextView.setText(String.format("%s, %s %s", vehicle.getRegisterNumber(), vehicle.getColour(), vehicle.getType()));

            mLocationTextView.setText(String.format("%s, %s", infringement.getLocationDescription(), infringement.getLocationSuburb()));

            if (infringementCharges[0] == null) {
                Utilities.hideLinearLayout(mChargeOneLinearLayout);
            } else {
                Utilities.showLinearLayout(mChargeOneLinearLayout);
                mChargeOneTextView.setText(formatChargeText(infringementCharges[0]));
            }

            if (infringementCharges[1] == null) {
                Utilities.hideLinearLayout(mChargeTwoLinearLayout);
            } else {
                Utilities.showLinearLayout(mChargeTwoLinearLayout);
                mChargeTwoTextView.setText(formatChargeText(infringementCharges[1]));
            }

            if (infringementCharges[2] == null) {
                Utilities.hideLinearLayout(mChargeThreeLinearLayout);
            } else {
                Utilities.showLinearLayout(mChargeThreeLinearLayout);
                mChargeThreeTextView.setText(formatChargeText(infringementCharges[2]));
            }

            if (TextUtils.isEmpty(infringement.getNotes())){
                Utilities.hideLinearLayout(mNotesLinearLayout);
            }else{
                Utilities.showLinearLayout(mNotesLinearLayout);
                mNotesTextView.setText(
                        makeParitalTextBold(String.format(
                                "%s %s",
                                App.getContext().getResources().getString(R.string.end_of_day_report_notes),
                                infringement.getNotes()),
                                0,
                                App.getContext().getResources().getString(R.string.end_of_day_report_notes).length()));
            }

        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "EndOfDayReportListAdapter::setValues()"), ErrorSeverity.High);
        }
    }

//    private void hideLinearLayout(LinearLayout linearLayout){
//
//        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
//        layoutParams.height = 0;
//        layoutParams.width = 0;
//        linearLayout.setLayoutParams(layoutParams);
//    }
//
//    private void showLinearLayout(LinearLayout linearLayout){
//
//        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
//        layoutParams.height = ViewGroup.LayoutParams.WRAP_CONTENT;
//        layoutParams.width = ViewGroup.LayoutParams.MATCH_PARENT;
//        linearLayout.setLayoutParams(layoutParams);
//    }

    private SpannableStringBuilder formatChargeText(InfringementChargeModel infringement){

        String altChargeText = App.getContext().getResources().getString(R.string.end_of_day_report_short_alt_charge);

        return infringement.getIsAlternative() ?
                makeParitalTextBold(
                        String.format("#%s(%s) %s",
                                infringement.getChargeCode(),
                                altChargeText,
                                infringement.getDescription()),
                        0,
                        infringement.getChargeCode().length()+1) :

                makeParitalTextBold(
                        String.format("#%s %s",
                                infringement.getChargeCode(),
                                infringement.getDescription()),
                        0,
                        infringement.getChargeCode().length()+1);
    }

    private SpannableStringBuilder makeParitalTextBold(String text, int boldStart, int boldLength){

        final SpannableStringBuilder spannableStringBuilder = new SpannableStringBuilder(text);

        final StyleSpan styleSpan = new StyleSpan(android.graphics.Typeface.BOLD); // Span to make text bold
        spannableStringBuilder.setSpan(styleSpan, boldStart, boldLength, Spannable.SPAN_INCLUSIVE_INCLUSIVE); // make first 4 characters Bold

        return spannableStringBuilder;
    }

    @Override
    public int getCount() {

        if (mTicketList == null) return 0;

        return mTicketList.size();
    }


    @Override
    public Object getItem(int position) {

        if (mTicketList != null) {
            return mTicketList.get(position);
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
