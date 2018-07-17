package za.co.kapsch.ivehicletest;

import android.app.Activity;
import android.graphics.Color;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/12/07.
 */

public class VehicleInspectionListAdapter extends BaseAdapter {

    Activity mActivity;
    public List<VehicleInspectionResultModel> mVehicleInspectionResultList;
    private TextView mQuestionTextView;
    private TextView mAnswerTextView;
    private TextView mMultipleChoiceResultTextView;
    private TextView mCommentTextView;
    private final int mColumnWidthA = 20;


    public <T> VehicleInspectionListAdapter(Activity activity, List<VehicleInspectionResultModel> vehicleInspectionResultList){
        super();
        mActivity = activity;
        mVehicleInspectionResultList = vehicleInspectionResultList;
    }

    @Override
    public int getCount() {

        if (mVehicleInspectionResultList != null) {
            return mVehicleInspectionResultList.size();
        }

        return 0;
    }

    @Override
    public Object getItem(int position) {

        if (mVehicleInspectionResultList != null) {
            return mVehicleInspectionResultList.get(position);
        }

        return null;
    }

    @Override
    public long getItemId(int position) {
        return 0;
    }

    @Override
    public View getView(int position, View rowView, final ViewGroup parent) {

        LayoutInflater inflater = mActivity.getLayoutInflater();

        if(rowView == null){

            rowView=inflater.inflate(R.layout.vehicle_inspection_item, null);

            mQuestionTextView = (TextView) rowView.findViewById(R.id.reviewQuestionTextView);
            mAnswerTextView = (TextView) rowView.findViewById(R.id.reviewAnswerTextView);
            mMultipleChoiceResultTextView = (TextView) rowView.findViewById(R.id.reviewMultipleChoiceResultTextView);
            mCommentTextView = (TextView) rowView.findViewById(R.id.commentTextView);

            rowView.setTag(R.id.reviewQuestionTextView, mQuestionTextView);
            rowView.setTag(R.id.reviewAnswerTextView, mAnswerTextView);
            rowView.setTag(R.id.reviewMultipleChoiceResultTextView, mMultipleChoiceResultTextView);
            rowView.setTag(R.id.commentTextView, mCommentTextView);
        }else{
            mQuestionTextView = (TextView) rowView.getTag(R.id.reviewQuestionTextView);
            mAnswerTextView = (TextView) rowView.getTag(R.id.reviewAnswerTextView);
            mMultipleChoiceResultTextView = (TextView) rowView.getTag(R.id.reviewMultipleChoiceResultTextView);
            mCommentTextView = (TextView) rowView.getTag(R.id.commentTextView);
        }

        mQuestionTextView.setText(mVehicleInspectionResultList.get(position).getQuestion());

//        if ((TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getAnswer()) == false)){
//             mAnswerTextView.setText(mVehicleInspectionResultList.get(position).getAnswer());
//        }

//        if ((TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getAnswer()) == true) &&
//            (TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getCompareValue()) == false)){
             mAnswerTextView.setText(mVehicleInspectionResultList.get(position).getCompareValue());
//        }

//        if ((TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getAnswer()) == true) &&
//            (TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getCompareValue()) == true)){
//             mAnswerTextView.setText(mVehicleInspectionResultList.get(position).getComments());
//        }

        String multiChoiceAnswer = Constants.EMPTY_STRING;
        if (TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getAnswer()) == true) {
            multiChoiceAnswer = mVehicleInspectionResultList.get(position).getMultipleChoiceText();
        } else{
            //multiChoiceAnswer = String.format("%s - %s", mVehicleInspectionResultList.get(position).getAnswer(), mVehicleInspectionResultList.get(position).getMultipleChoiceText());
            multiChoiceAnswer = String.format("%s - %s", mVehicleInspectionResultList.get(position).getAnswer(),
                                    TextUtils.isEmpty(mVehicleInspectionResultList.get(position).getMultipleChoiceText()) == true ?
                                            mVehicleInspectionResultList.get(position).getPassFailedText() :
                                            mVehicleInspectionResultList.get(position).getMultipleChoiceText());
        }

        mMultipleChoiceResultTextView.setText(multiChoiceAnswer);
        mMultipleChoiceResultTextView.setTextColor(mVehicleInspectionResultList.get(position).getDisplayColour());

        String comments = mVehicleInspectionResultList.get(position).getComments();

        if (TextUtils.isEmpty(comments)) {
            Utilities.hideView(mCommentTextView);
        }else {
            Utilities.showView(mCommentTextView);
            mCommentTextView.setText(comments);
        }

        if (Utilities.isEven(position)) {
            rowView.setBackgroundColor(App.getContext().getResources().getColor(R.color.listAlternatingColor));
        }else{
            rowView.setBackgroundColor(App.getContext().getResources().getColor(R.color.colorWhite));
        }

        final int listPosition = position;
        rowView.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View arg0)
            {
                //Highlight item and call parent onclick event.
                ((ListView)parent).setItemChecked(listPosition+1, true);
                ((ListView)parent).performItemClick(arg0, listPosition+1,  getItemId(listPosition+1));
            }
        });

        return rowView;
    }
}
