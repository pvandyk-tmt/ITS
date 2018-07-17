package za.co.kapsch.ivehicletest;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;

import java.util.List;

import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.ivehicletest.Models.EvidenceModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-10-27.
 */
public class EvidenceListAdapter extends ArrayAdapter<EvidenceModel> {

    private final Activity mContext;
    private ImageView mImageView;
    private final List<EvidenceModel> mEvidenceList;

    public EvidenceListAdapter(Activity context, List<EvidenceModel> evidenceList) {

        super(context, R.layout.list_item_image, evidenceList);

        mContext = context;
        mEvidenceList = evidenceList;
    }

    public View getView(int position, View view, ViewGroup parent) {

        LayoutInflater inflater = mContext.getLayoutInflater();

        if(view == null) {
            view = inflater.inflate(R.layout.list_item_image, null, true);

            mImageView = (ImageView) view.findViewById(R.id.itemImage);
            view.setTag(R.id.itemImage, mImageView);
        }else{
            mImageView = (ImageView) view.getTag(R.id.itemImage);
        }

        byte[] evidence = null;
        if (mEvidenceList.get(position).getInspectionEvidenceType() == InspectionEvidenceType.VehiclePhoto) {
            evidence = mEvidenceList.get(position).getEvidence();
        }

        if (evidence != null) {
            Bitmap image = BitmapFactory.decodeByteArray(evidence, 0, evidence.length);
            mImageView.setImageBitmap(image);
        }

        return view;
    };

    private String getText(InspectionEvidenceType evidenceType){
        switch (evidenceType){
            //case VehiclePhoto: return Constants.PICTURE;
            default: return null;
        }
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

