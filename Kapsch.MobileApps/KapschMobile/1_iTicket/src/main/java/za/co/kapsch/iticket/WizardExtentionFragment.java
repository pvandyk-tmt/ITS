package za.co.kapsch.iticket;


import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;

import java.io.IOException;
import java.util.List;

import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Enums.MediaPlayerMode;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;


/**
 * A simple {@link Fragment} subclass.
 */
public class WizardExtentionFragment extends Fragment {

    private WizardActivity mWizardActivity;
    private ImageButton mCameraButton;
    private ImageButton mAudioButton;

    public WizardExtentionFragment() {
        // Required empty public constructor
    }

    private View.OnLongClickListener mOnLongClickListener = new View.OnLongClickListener() {
        @Override
        public boolean onLongClick(View view) {
            try {
                List<EvidenceModel> evidenceList = EvidenceRepository.getEvidenceByTicketNumber(mWizardActivity.getTicketModel().getInfringement().getTicketNumber());
                if (evidenceList == null) {
                    MessageManager.showMessage("This ticket does not have evidence", ErrorSeverity.None);
                    return true;
                }

                Intent intent = new Intent(mWizardActivity, EvidenceListActivity.class);
                intent.putExtra(Constants.TICKET_NUMBER, mWizardActivity.getTicketModel().getInfringement().getTicketNumber());
                mWizardActivity.startActivity(intent);
                return true;
            } catch (Exception e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "photoButton.setOnLongClickListener()"), ErrorSeverity.Medium);
                return true;
            }
        }
    };

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView =  inflater.inflate(R.layout.fragment_wizard_extention, container, false);

        mCameraButton = (ImageButton) rootView.findViewById(R.id.cameraButton);
        mAudioButton = (ImageButton) rootView.findViewById(R.id.audioButton);
        mWizardActivity = (WizardActivity)this.getActivity();

        mCameraButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                try {
                    Uri uri = Uri.fromFile(Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME));
                    Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
                    intent.putExtra(MediaStore.EXTRA_OUTPUT, uri);
                    startActivityForResult(intent, Constants.REQUEST_TAKE_PHOTO);
                } catch (Exception e) {
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardExtentionFragment:mCameraButton():onCLick()"), ErrorSeverity.Medium);
                }
            }
        });

        mAudioButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                try {
                    Intent intent = new Intent(mWizardActivity, MediaManagerActivity.class);
                    intent.putExtra(Constants.MEDIA_PLAYER_TYPE, MediaPlayerMode.toInteger(MediaPlayerMode.Recorder));
                    startActivityForResult(intent, Constants.REQUEST_RECORD_AUDIO);
                }catch (Exception e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardExtentionFragment:mAudioButton():onCLick()"), ErrorSeverity.Medium);
                }
            }
        });

        mCameraButton.setOnLongClickListener(mOnLongClickListener);
        mAudioButton.setOnLongClickListener(mOnLongClickListener);

        return rootView;
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == Activity.RESULT_OK) {
            if (requestCode == Constants.REQUEST_RECORD_AUDIO) {
                try{
                    byte[] voiceEvidence = Utilities.getFileData(Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME));

                    EvidenceRepository.create(EvidenceModel.getEvidence(
                            EvidenceType.VoiceRecording,
                            voiceEvidence,
                            mWizardActivity.getTicketModel().getInfringement().getTicketNumber()));

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

                    EvidenceRepository.create(EvidenceModel.getEvidence(
                            EvidenceType.Other,
                            pictureEvidenceJpeg,
                            mWizardActivity.getTicketModel().getInfringement().getTicketNumber()));

                    if (Utilities.getTicketFile(Constants.TEMP_PICTURE_EVIDENCE_FILENAME).delete() == false) {
                        MessageManager.showMessage("Failed to delete photo evidence file", ErrorSeverity.High);
                    }
                } catch (Exception e) {
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketWizardExtension callBackMethod() 2"), ErrorSeverity.High);
                }
            }
        }
    }
}
