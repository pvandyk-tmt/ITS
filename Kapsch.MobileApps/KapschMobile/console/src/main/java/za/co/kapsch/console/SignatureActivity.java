package za.co.kapsch.console;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Interfaces.ICallBack;
import za.co.kapsch.console.General.CaptureSignatureView;

public class SignatureActivity extends AppCompatActivity implements ICallBack {

    private Button mSaveSignatureButton;
    private byte[] mSignature;
    private CaptureSignatureView mCaptureSignatureView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_signature);

        Intent intent = getIntent();
        mSignature = intent.getByteArrayExtra(Constants.SIGNATURE);

        LinearLayout mContent = (LinearLayout) findViewById(R.id.signatureLinearLayout);
        mCaptureSignatureView = new CaptureSignatureView(this, null, this);
        mSaveSignatureButton = (Button) findViewById(R.id.saveSignatureButton);
        mContent.addView(mCaptureSignatureView, LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.officer_signature)));

        mCaptureSignatureView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_UP) {
                    //validateUserInteface();
                }
                return false;
            }
        });
    }

//    private void validateUserInteface(){
//        byte[] signature = mCaptureSignatureView.getBytes();
//        mSaveSignatureButton.setEnabled(signature != null);
//    }

    public void callBackMethod(){
        drawSignature();
    }

    public void clearSignature(View view){
        try {
            mCaptureSignatureView.clearCanvas();
            //validateUserInteface();
        }
        catch (Exception e){
            Utilities.displayOkMessage(e.getMessage(), this);
        }
    }

    public void returnSignature(View view){

        byte[] signature = mCaptureSignatureView.getBytes();
        if (signature == null){
            MessageManager.showMessage(getResources().getString(R.string.please_capture_a_signature), ErrorSeverity.None);
            return;
        }

        Intent intent = new Intent();
        intent.putExtra(Constants.SIGNATURE, signature);
        setResult(RESULT_OK, intent);
        finish();
    }

    @Override
    public void onBackPressed(){

    }

    private void drawSignature(){
        mCaptureSignatureView.setBitmap(mSignature);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
