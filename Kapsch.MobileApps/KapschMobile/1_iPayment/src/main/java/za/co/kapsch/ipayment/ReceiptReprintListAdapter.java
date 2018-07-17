package za.co.kapsch.ipayment;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.CheckBox;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.ipayment.General.App;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/09/29.
 */
public class ReceiptReprintListAdapter extends BaseAdapter {

    private List<TransactionModel> mTransactionList;

    private Activity mActivity;
    private CheckBox mCheckBox;
    private TextView mTicketNumberTextView;
    private TextView mPersonVlnTextView;
    private TextView mDateTextView;
    private TextView mAmountTextView;
    private LinearLayout mItemLinearLayout;

    public <T> ReceiptReprintListAdapter(Activity activity, List<TransactionModel> transactionList){
        super();
        mActivity = activity;
        mTransactionList = transactionList;
    }

    @Override
    public int getCount() {

        if (mTransactionList != null) {
            return mTransactionList.size();
        }

        return 0;
    }

    @Override
    public Object getItem(int position) {

        if (mTransactionList != null) {
            return mTransactionList.get(position);
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
    public View getView(int position, View rowView, ViewGroup parent) {
        LayoutInflater inflater = mActivity.getLayoutInflater();

        if(rowView == null){

            rowView=inflater.inflate(R.layout.search_receipt_list_item, null);

            mItemLinearLayout = (LinearLayout) rowView.findViewById(R.id.itemLinearLayout);
            mCheckBox = (CheckBox) rowView.findViewById(R.id.checkBox);
            mTicketNumberTextView = (TextView) rowView.findViewById(R.id.ticketNumberTextView);
            mPersonVlnTextView = (TextView) rowView.findViewById(R.id.personVlnTextView);
            //mPersonTextView = (TextView) rowView.findViewById(R.id.personTextView);
            //mVlnTextView = (TextView) rowView.findViewById(R.id.vlnTextView);
            mDateTextView = (TextView) rowView.findViewById(R.id.dateTextView);
            mAmountTextView = (TextView) rowView.findViewById(R.id.amountTextView);

            rowView.setTag(R.id.itemLinearLayout, mItemLinearLayout);
            rowView.setTag(R.id.checkBox, mCheckBox);
            rowView.setTag(R.id.ticketNumberTextView, mTicketNumberTextView);
            rowView.setTag(R.id.personVlnTextView, mPersonVlnTextView);
            //rowView.setTag(R.id.personTextView, mPersonTextView);
            //rowView.setTag(R.id.vlnTextView, mVlnTextView);
            rowView.setTag(R.id.dateTextView, mDateTextView);
            rowView.setTag(R.id.amountTextView, mAmountTextView);

        }else{
            mItemLinearLayout = (LinearLayout) rowView.getTag(R.id.itemLinearLayout);
            mCheckBox = (CheckBox) rowView.getTag(R.id.checkBox);
            mTicketNumberTextView = (TextView) rowView.getTag(R.id.ticketNumberTextView);
            mPersonVlnTextView = (TextView) rowView.getTag(R.id.personVlnTextView);
            //mPersonTextView= (TextView) rowView.getTag(R.id.personTextView);
            //mVlnTextView = (TextView) rowView.getTag(R.id.vlnTextView);
            mDateTextView = (TextView) rowView.getTag(R.id.dateTextView);
            mAmountTextView = (TextView) rowView.getTag(R.id.amountTextView);
        }

        if (Utilities.isEven(position)) {
            mItemLinearLayout.setBackgroundColor(App.getContext().getResources().getColor(R.color.listAlternatingColor));
        }

        mTicketNumberTextView.setText(mTransactionList == null ? Constants.EMPTY_STRING : mTransactionList.get(position).getReceipt());

        mPersonVlnTextView.setText(mTransactionList == null ? Constants.EMPTY_STRING :
                String.format("%s %s", mTransactionList.get(position).getCustomerFirstName(), mTransactionList.get(position).getCustomerLastName()));
        mDateTextView.setText(mTransactionList == null ? Constants.EMPTY_STRING : Utilities.dateTimeToString(mTransactionList.get(position).getReceiptTimeStamp()));

        mAmountTextView.setText(mTransactionList == null ? Constants.EMPTY_STRING : Double.toString(mTransactionList.get(position).getConfirmedAmount()));

        return rowView;
    }
}
