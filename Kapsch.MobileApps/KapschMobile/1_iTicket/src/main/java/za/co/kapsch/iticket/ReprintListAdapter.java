package za.co.kapsch.iticket;

import android.app.Activity;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ListView;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-11-30.
 */
public class ReprintListAdapter extends BaseAdapter {

    private static int textColor = -1979711488;
    public  List<HandWrittenModel> mHandWrittenList;
    Activity mActivity;
    TextView mTicketNumberTextView;
    TextView mIssueDateTextView;

    public <T> ReprintListAdapter(Activity activity, List<HandWrittenModel> handWrittenList){
        super();
        mActivity = activity;
        mHandWrittenList = handWrittenList;
    }

    @Override
    public int getCount() {

        if (mHandWrittenList != null) {
            return mHandWrittenList.size();
        }

        return 0;
    }

    @Override
    public Object getItem(int position) {

        if (mHandWrittenList != null) {
            return mHandWrittenList.get(position);
        }

        return null;
    }

    @Override
    public int getViewTypeCount() {

        return getCount();
    }

    @Override
    public int getItemViewType(int position) {

        return position;
    }

    @Override
    public long getItemId(int position) {
        return 0;
    }

    @Override
    public View getView(int position, View rowView, final ViewGroup parent) {

        LayoutInflater inflater = mActivity.getLayoutInflater();

        if(rowView == null){

            rowView=inflater.inflate(R.layout.reprint_list_item, null);

            mTicketNumberTextView=(TextView) rowView.findViewById(R.id.ticketNumber);
            mIssueDateTextView=(TextView) rowView.findViewById(R.id.issueDate);

            rowView.setTag(R.id.ticketNumber, mTicketNumberTextView);
            rowView.setTag(R.id.issueDate, mIssueDateTextView);

        }else{
            mTicketNumberTextView=(TextView) rowView.getTag(R.id.ticketNumber);
            mIssueDateTextView=(TextView) rowView.getTag(R.id.issueDate);
        }

        mTicketNumberTextView.setText(mHandWrittenList == null ? Constants.EMPTY_STRING : Utilities.formatReferenceNumber(mHandWrittenList.get(position).getTicketNumber()));
        mIssueDateTextView.setText(Utilities.dateToString(mHandWrittenList == null ? null : mHandWrittenList.get(position).getIssueDate()));

        if (mHandWrittenList != null) {
            mTicketNumberTextView.setTextColor(mHandWrittenList.get(position).isPrinted() ? Color.BLACK : Color.RED);
            mIssueDateTextView.setTextColor(mHandWrittenList.get(position).isPrinted() ? Color.BLACK : Color.RED);
        }

        if (mHandWrittenList != null){
            mTicketNumberTextView.setTextColor(mHandWrittenList.get(position).isPrinted() ? Color.BLACK : Color.RED);
            mIssueDateTextView.setTextColor(mHandWrittenList.get(position).isPrinted() ? Color.BLACK : Color.RED);
        }

        final int listPosition = position;
        rowView.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View arg0)
            {
                //Highlight item and call parent onclick event.
                ((ListView)parent).setItemChecked(listPosition+1, true);
                ((ListView)parent).performItemClick(arg0, listPosition+1, getItemId(listPosition+1));
            }
        });

        return rowView;
    }
}