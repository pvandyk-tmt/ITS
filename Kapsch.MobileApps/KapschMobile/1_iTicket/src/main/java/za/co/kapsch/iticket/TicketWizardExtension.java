package za.co.kapsch.iticket;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.os.Parcel;
import android.os.Parcelable;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Interfaces.IWizardExtension;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-10-25.
 */
public class TicketWizardExtension implements IWizardExtension, Parcelable {

    private List<View> mViewList;

    public List<View> getViewList() {
        if (mViewList == null) {
            mViewList = new ArrayList<>();
            mViewList.add(getPhotoButton());
            mViewList.add(getAudioButton());
        }

        return mViewList;
    }

    public TicketWizardExtension(){}

    private View.OnLongClickListener mOnLongClickListener = new View.OnLongClickListener() {
        @Override
        public boolean onLongClick(View view) {
//        try {
//            Activity activity = getActivity(view);
//            if (((WizardActivity) activity).getTicketModel().getEvidenceList() == null){
//                MessageManager.showMessage("This ticket does not have evidence", ErrorSeverity.None);
                return true;
//            }
//
//            Intent intent = new Intent(activity, EvidenceListActivity.class);
//            intent.putExtra(Constants.TICKET_MODEL, ((WizardActivity) activity).getTicketModel());
//            activity.startActivity(intent);
//            return true;
//        } catch (Exception e) {
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "photoButton.setOnLongClickListener()"), ErrorSeverity.Medium);
//            return true;
//        }
        }
    };

    private ImageButton getPhotoButton(){
        ImageButton photoButton = new ImageButton(App.getContext());
        photoButton.setImageResource(R.drawable.camera_on);
        photoButton.setLayoutParams(new LinearLayout.LayoutParams(65, ViewGroup.LayoutParams.MATCH_PARENT, 1.0f));
        photoButton.setScaleType(ImageView.ScaleType.FIT_XY);
        ((ViewGroup.MarginLayoutParams) photoButton.getLayoutParams()).rightMargin = 15;
        photoButton.setBackgroundColor(Color.TRANSPARENT);

        photoButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                MessageManager.showMessage("Not implemented.",  ErrorSeverity.None);
//                try {
//                    Activity activity = getActivity(view);
//                    Uri uri = Uri.fromFile(Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME));
//                    Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
//                    intent.putExtra(MediaStore.EXTRA_OUTPUT, uri);
//                    activity.startActivityForResult(intent, Constants.REQUEST_TAKE_PHOTO);
//                } catch (Exception e) {
//                    MessageManager.showMessage(Utilities.exceptionMessage(e, "getAudioButton():onCLick()"), ErrorSeverity.Medium);
//                }
            }
        });

        //photoButton.setOnLongClickListener(mOnLongClickListener);

        return photoButton;
    }

    private ImageButton getAudioButton(){

        ImageButton audioButton = new ImageButton(App.getContext());
        audioButton.setImageResource(R.drawable.voicerecording_on);
        audioButton.setScaleType(ImageView.ScaleType.FIT_XY);
        audioButton.setLayoutParams(new LinearLayout.LayoutParams(65, ViewGroup.LayoutParams.MATCH_PARENT, 1.0f));
        ((ViewGroup.MarginLayoutParams) audioButton.getLayoutParams()).leftMargin = 15;
        //((ViewGroup.MarginLayoutParams) audioButton.getLayoutParams()).topMargin = 3;
        //((ViewGroup.MarginLayoutParams) audioButton.getLayoutParams()).bottomMargin = 3;
        audioButton.setBackgroundColor(Color.TRANSPARENT);

        audioButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                MessageManager.showMessage("Not implemented.",  ErrorSeverity.None);
//                try {
//                    Activity activity = getActivity(view);
//                    Intent intent = new Intent(activity, MediaManagerActivity.class);
//                    intent.putExtra(Constants.MEDIA_PLAYER_TYPE, MediaPlayerMode.toInteger(MediaPlayerMode.Recorder));
//                    activity.startActivityForResult(intent, Constants.REQUEST_RECORD_AUDIO);
//                }catch (Exception e){
//                    MessageManager.showMessage(Utilities.exceptionMessage(e, "getPhotoButton():onCLick()"), ErrorSeverity.Medium);
//                }
            }
        });

        //audioButton.setOnLongClickListener(mOnLongClickListener);

        return audioButton;
    }

    private Activity getActivity(View view){
        ViewParent parent = view.getParent();
        return (Activity) ((LinearLayout)parent).getContext();
    }

    private TicketModel getTicket(){
        Activity activity = getActivity(getViewList().get(0));
        return ((WizardActivity) getActivity(getViewList().get(0))).getTicketModel();
    }

    public void callBackMethod(int requestCode, Object object){

        if (requestCode == Constants.REQUEST_RECORD_AUDIO) {
            try{

                byte[] voiceEvidence = Utilities.getFileData(Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME));

                TicketModel ticket = getTicket();

                EvidenceRepository.create(EvidenceModel.getEvidence(
                        EvidenceType.VoiceRecording,
                        voiceEvidence,
                        ticket.getInfringement().getTicketNumber()));

//                ticket.addEvidence(
//                        EvidenceModel.getEvidence(
//                            EvidenceType.Voice,
//                            voiceEvidence,
//                            ticket.getInfringement().getTicketNumber()));

                if (Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME).delete() == false) {
                    MessageManager.showMessage("Failed to delete voice evidence file", ErrorSeverity.Medium);
                }

            } catch (IOException e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketWizardExtension callBackMethod() 1"), ErrorSeverity.Medium);
            } catch (Exception e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketWizardExtension callBackMethod() 1"), ErrorSeverity.High);
            }
        }else if (requestCode == Constants.REQUEST_TAKE_PHOTO) {

            try {
                byte[] pictureEvidence = Utilities.getFileData(Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME));
                Bitmap bitmap = BitmapFactory.decodeByteArray(pictureEvidence, 0, pictureEvidence.length);
                bitmap = Utilities.getResizedBitmap(bitmap, Constants.EVIDENCE_BITMAP_MAX_SIZE);
                byte[] pictureEvidenceJpeg = Utilities.bitmapToJPGBytes(bitmap);

                TicketModel ticket = getTicket();

                EvidenceRepository.create(EvidenceModel.getEvidence(
                        EvidenceType.Other,
                        pictureEvidenceJpeg,
                        ticket.getInfringement().getTicketNumber()));

//                ticket.addEvidence(
//                        EvidenceModel.getEvidence(
//                            EvidenceType.Picture,
//                            pictureEvidenceJpeg,
//                            ticket.getInfringement().getTicketNumber()));

                if (Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME).delete() == false) {
                    MessageManager.showMessage("Failed to delete photo evidence file", ErrorSeverity.High);
                }

            } catch (Exception e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketWizardExtension callBackMethod() 2"), ErrorSeverity.High);
            }
        }
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
    }

    public static final Parcelable.Creator<TicketWizardExtension> CREATOR = new Parcelable.Creator<TicketWizardExtension>() {
        public TicketWizardExtension createFromParcel(Parcel in) {
            return new TicketWizardExtension(in);
        }

        public TicketWizardExtension[] newArray(int size) {
            return new TicketWizardExtension[size];
        }
    };

    private TicketWizardExtension(Parcel in){
    }
}

