package za.co.kapsch.iticket;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.LinearLayout;

import za.co.kapsch.iticket.Interfaces.ICallBack;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.shared.Utilities;

/**
 * A simple {@link Fragment} subclass.
 */
public class SignatureCaptureFragment extends Fragment implements ICallBack {

    private byte[] mSignature;
    private Button mClearSignatureButton;
    private CaptureSignatureView mCaptureSignatureView;

    public SignatureCaptureFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView =  inflater.inflate(R.layout.fragment_signature_capture, container, false);

        LinearLayout mContent = (LinearLayout) rootView.findViewById(R.id.signatureLinearLayout);
        mCaptureSignatureView = new CaptureSignatureView(this.getActivity(), null, this);
        mContent.addView(mCaptureSignatureView, LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);

        OffenderModel driver = wizardActivity().getTicketModel().getOffender();

        if (driver != null)
        {
            mSignature = driver.getSignature();
        }

        mClearSignatureButton = (Button) rootView.findViewById(R.id.clearFragmentSignatureButton);
        mClearSignatureButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                clearFragmentSignature();
            }
        });

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.fragment_signature_title)));

        mCaptureSignatureView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_UP) {
                    //validateUserInteface();
                }
                return false;
            }
        });

//        new Handler().postDelayed(new Runnable() {
//            @Override
//            public void run() {
//                validateUserInteface();
//            }
//        },0);

        return rootView;
    }

    private void validateUserInteface(){
        wizardActivity().enableNextButton(true);
    }

    public void callBackMethod(){
        drawSignature();
    }

    public void clearFragmentSignature(){
        try {
            mCaptureSignatureView.clearCanvas();
            //validateUserInteface();
        }
        catch (Exception e){
            Utilities.displayOkMessage(e.getMessage(), getActivity());
        }
    }

    public void drawSignature(){
        mCaptureSignatureView.setBitmap(mSignature);
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    @Override
    public void onStart() {
        super.onStart();
        validateUserInteface();
    }

    public void onStop() {
        OffenderModel driver = wizardActivity().getTicketModel().getOffender();

        driver.setSignature(mCaptureSignatureView.getBytes());
        super.onStop();
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
