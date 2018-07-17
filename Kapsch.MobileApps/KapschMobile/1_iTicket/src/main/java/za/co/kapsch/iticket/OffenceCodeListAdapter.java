package za.co.kapsch.iticket;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.Models.ChargeInfoModel;

/**
 * Created by csenekal on 2017-01-03.
 */
public class OffenceCodeListAdapter extends ArrayAdapter<ChargeInfoModel> {

    private final Activity mActivity;
    private TextView mCodeTextView;
    private TextView mVehicleTypeTextView;
    private final List<ChargeInfoModel> mOffenceCodeList;

    public OffenceCodeListAdapter(Activity activity, List<ChargeInfoModel> offenceCodeList) {
        super(activity, R.layout.offence_code_list_item, offenceCodeList);
        // TODO Auto-generated constructor stub

        mActivity = activity;
        mOffenceCodeList = offenceCodeList;
    }

    public View getView(int position, View rowView, ViewGroup parent) {

        LayoutInflater inflater = mActivity.getLayoutInflater();

        if(rowView == null){

            rowView=inflater.inflate(R.layout.offence_code_list_item, null);

            mCodeTextView = (TextView) rowView.findViewById(R.id.codeTextView);
            mVehicleTypeTextView = (TextView) rowView.findViewById(R.id.vehicleTypeTextView);
        }

        mCodeTextView.setText(mOffenceCodeList.get(position).getCode());
        mVehicleTypeTextView.setText(mOffenceCodeList.get(position).getVehicleType());

        return rowView;
    };
}
