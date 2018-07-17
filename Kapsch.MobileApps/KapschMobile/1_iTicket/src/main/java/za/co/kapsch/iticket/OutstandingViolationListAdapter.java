package za.co.kapsch.iticket;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2018/04/17.
 */

public class OutstandingViolationListAdapter  extends ArrayAdapter<FineModel> {

    private final Activity mActivity;

    private TextView mNoticeNumberTextView;
    private TextView mOffenceDateTextView;
    private TextView mTotalAmountTextView;

    private final List<FineModel> mOutstandingViolationList;

    public OutstandingViolationListAdapter(Activity activity, List<FineModel> outstandingViolationList) {
        super(activity, R.layout.outstanding_violation_list_item, outstandingViolationList);
        // TODO Auto-generated constructor stub

        mActivity = activity;
        mOutstandingViolationList = outstandingViolationList;
    }

    public View getView(int position, View rowView, final ViewGroup parent) {

        LayoutInflater inflater = mActivity.getLayoutInflater();

        if(rowView == null){

            rowView=inflater.inflate(R.layout.outstanding_violation_list_item, null);

            mNoticeNumberTextView = (TextView) rowView.findViewById(R.id.noticeNumberTextView);
            mOffenceDateTextView = (TextView) rowView.findViewById(R.id.offenceDateTextView);
            mTotalAmountTextView = (TextView) rowView.findViewById(R.id.totalAmountTextView);

            rowView.setTag(R.id.noticeNumberTextView, mNoticeNumberTextView);
            rowView.setTag(R.id.offenceDateTextView, mOffenceDateTextView);
            rowView.setTag(R.id.totalAmountTextView, mTotalAmountTextView);
        }else{
            mNoticeNumberTextView = (TextView) rowView.getTag(R.id.noticeNumberTextView);
            mOffenceDateTextView = (TextView) rowView.getTag(R.id.offenceDateTextView);
            mTotalAmountTextView = (TextView) rowView.getTag(R.id.totalAmountTextView);
        }

        mNoticeNumberTextView.setText(mOutstandingViolationList.get(position).getReferenceNumber());
        mOffenceDateTextView.setText(Utilities.dateTimeToString(mOutstandingViolationList.get(position).getOffenceDate()));
        mTotalAmountTextView.setText(Double.toString(mOutstandingViolationList.get(position).getOutstandingAmount()));

//        if (Utilities.isEven(position)) {
//            rowView.setBackgroundColor(App.getContext().getResources().getColor(R.color.colorBannerBackGround));
//        }else{
//            rowView.setBackgroundColor(App.getContext().getResources().getColor(R.color.defaultColor));
//        }
//
//        final int listPosition = position;
//        rowView.setOnClickListener(new View.OnClickListener()
//        {
//            @Override
//            public void onClick(View arg0)
//            {
//                ((ListView) parent).setItemChecked(listPosition + 1, true);
//                ((ListView) parent).performItemClick(arg0, listPosition + 1, getItemId(listPosition + 1));
//            }
//        });

        return rowView;
    };
}
