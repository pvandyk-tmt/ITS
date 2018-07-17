package za.co.kapsch.iticket;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.Models.ChargeInfoModel;

/**
 * Created by CSenekal on 2017/01/31.
 */
public class ChargeSearchListAdapter extends BaseAdapter {

    public List<ChargeInfoModel> mOffenceCodeList;
    Activity mActivity;
    TextView mCodeTextView;
    TextView mZoneTextView;

    public <T> ChargeSearchListAdapter(Activity activity, List<ChargeInfoModel> offenceCodeList){
        super();
        mActivity = activity;
        mOffenceCodeList = offenceCodeList;
    }

    public void clearData(){
        mOffenceCodeList.clear();
    }

    @Override
    public int getCount() {
        if (mOffenceCodeList != null) {
            return mOffenceCodeList.size();
        }

        return 0;
    }

    @Override
    public Object getItem(int position) {

        if (mOffenceCodeList != null) {
            return mOffenceCodeList.get(position);
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

            rowView=inflater.inflate(R.layout.charge_search_list_item, null);

            mCodeTextView=(TextView) rowView.findViewById(R.id.code);
            mZoneTextView=(TextView) rowView.findViewById(R.id.zone);
        }

        mCodeTextView.setText(mOffenceCodeList.get(position).getCode());
        mZoneTextView.setText(Integer.toString(mOffenceCodeList.get(position).getZone()));

        return rowView;
    }

}