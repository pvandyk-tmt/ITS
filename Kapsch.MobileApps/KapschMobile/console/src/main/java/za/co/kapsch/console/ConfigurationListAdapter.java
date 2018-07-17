package za.co.kapsch.console;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.ConfigItemModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/02/28.
 */
public class ConfigurationListAdapter  extends BaseAdapter {

    private final Activity mActivity;
    private TextView mDescriptionTextView;
    private TextView mValueTextView;

    private final List<ConfigItemModel> mConfigItemList;

    public <T> ConfigurationListAdapter(Activity activity, List<ConfigItemModel> configItemList){
        super();
        mActivity = activity;
        mConfigItemList = configItemList;
    }

    @Override
    public int getCount() {

        if (mConfigItemList == null) return 0;

        return mConfigItemList.size();
    }


    @Override
    public Object getItem(int position) {

        if (mConfigItemList != null) {
            return mConfigItemList.get(position);
        }

        return null;
    }

    @Override
    public long getItemId(int position) {

        if (mConfigItemList != null) {
            return mConfigItemList.get(position).getID();
        }

        return 0;
    }

    @Override
    public View getView(int position, View rowView, ViewGroup parent) {
        LayoutInflater inflater = mActivity.getLayoutInflater();

        try {
            if(rowView == null){

                rowView=inflater.inflate(R.layout.configuration_list_item, null);

                mDescriptionTextView = (TextView) rowView.findViewById(R.id.descTextView);
                mValueTextView = (TextView) rowView.findViewById(R.id.valueTextView);
            }


            mDescriptionTextView.setText(mConfigItemList == null ? null : mConfigItemList.get(position).getDescription());
            mValueTextView.setText(mConfigItemList == null ? null : mConfigItemList.get(position).getValue());
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }

        return rowView;
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
