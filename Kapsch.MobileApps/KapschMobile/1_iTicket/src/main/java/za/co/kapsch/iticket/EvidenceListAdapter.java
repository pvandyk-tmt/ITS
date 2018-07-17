package za.co.kapsch.iticket;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;

import java.util.List;

import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Models.EvidenceModel;

/**
 * Created by csenekal on 2016-10-27.
 */
public class EvidenceListAdapter extends ArrayAdapter<EvidenceModel> {

    private final Activity mContext;
    private final List<EvidenceModel> mEvidenceList;

    public EvidenceListAdapter(Activity context, List<EvidenceModel> evidenceList) {
        super(context, R.layout.list_image_text, evidenceList);
        // TODO Auto-generated constructor stub

        mContext = context;
        mEvidenceList = evidenceList;
    }

    public View getView(int position, View view, ViewGroup parent) {
        LayoutInflater inflater = mContext.getLayoutInflater();
        View rowView=inflater.inflate(R.layout.list_image_text, null,true);

        ImageView imageView = (ImageView) rowView.findViewById(R.id.itemImage);

        byte[] evidence = null;
        if ((mEvidenceList.get(position).getEvidenceType() == EvidenceType.Other) ||
            (mEvidenceList.get(position).getEvidenceType() == EvidenceType.PersonSignature) ||
            (mEvidenceList.get(position).getEvidenceType() == EvidenceType.OfficerSignature) ||
            (mEvidenceList.get(position).getEvidenceType() == EvidenceType.OffenderPhoto)) {

            evidence = mEvidenceList.get(position).getEvidence();
        }

        if (evidence != null) {
            Bitmap image = BitmapFactory.decodeByteArray(evidence, 0, evidence.length);
            imageView.setImageBitmap(image);
        }
        else
        {
            imageView.setImageResource(R.drawable.voicerecording_on);
        }

        return rowView;
    };

    private String getText(EvidenceType evidenceType){
        switch (evidenceType){
            case Other: return Constants.PICTURE;
            case VoiceRecording: return Constants.VOICE;
            case OfficerSignature: return Constants.OFFICER_SIGNATURE;
            case PersonSignature: return Constants.PERSON_SIGNATURE;
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

