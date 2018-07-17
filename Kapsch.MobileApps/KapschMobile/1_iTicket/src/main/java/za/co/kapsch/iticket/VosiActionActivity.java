package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;

import java.sql.SQLException;
import java.util.Calendar;

import za.co.kapsch.iticket.Enums.AddressType;
import za.co.kapsch.iticket.Enums.VosiActionType;
import za.co.kapsch.iticket.Google.GoogleAddressRefactor;
import za.co.kapsch.iticket.Models.VosiActionCaptureModel;
import za.co.kapsch.iticket.Models.VosiActionModel;
import za.co.kapsch.iticket.iCam.ICamVlns;
import za.co.kapsch.iticket.orm.VosiActionCaptureRepository;
import za.co.kapsch.iticket.orm.VosiActionRepository;
import za.co.kapsch.shared.*;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.UserModel;

public class VosiActionActivity extends AppCompatActivity {

    private UserModel mUser;
    private DistrictModel mDistrict;
    private MobileDeviceModel mMobileDevice;
    private ICamVlns mICamVlns;
    private Spinner mVosiActionSpinner;

    private EditText mVlnEditText;
    private EditText mDescriptionEditText;
    private EditText mSuburbEditText;
    private EditText mCityEditText;
    private EditText mCommentsEditText;
    private Button mOkButton;

    private TextWatcher mTextWatcher = new TextWatcher() {
        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {

        }

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

        }

        @Override
        public void afterTextChanged(Editable s) {
            validateUserInterface();
        }
    };


    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_vosi_action);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.vosi_action)));

        mVosiActionSpinner = (Spinner) findViewById(R.id.vosiActionSpinner);
        mVlnEditText = (EditText) findViewById(R.id.vlnEditText);
        mDescriptionEditText = (EditText) findViewById(R.id.descriptionEditText);
        mSuburbEditText = (EditText) findViewById(R.id.suburbEditText);
        mCityEditText = (EditText) findViewById(R.id.cityEditText);
        mCommentsEditText = (EditText) findViewById(R.id.commentsEditText);

        mVlnEditText.addTextChangedListener(mTextWatcher);
        mDescriptionEditText.addTextChangedListener(mTextWatcher);
        mSuburbEditText.addTextChangedListener(mTextWatcher);
        mCityEditText.addTextChangedListener(mTextWatcher);
        mCommentsEditText.addTextChangedListener(mTextWatcher);

        mOkButton = (Button) findViewById(R.id.okButton);
        mOkButton.setEnabled(false);

        mICamVlns = getIntent().getParcelableExtra(Constants.ICAM_VLNS);
        mUser = Utilities.getUser(this);
        mMobileDevice = Utilities.getMobileDevice(this);

        populateIdTypeSpinner();
        populateFields(mICamVlns);
   }

    private void populateIdTypeSpinner(){

        try {
            mVosiActionSpinner.setAdapter(new ArrayAdapter<>(this, R.layout.spinner_item, VosiActionRepository.getVosiAction()));
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VosiActionActivity::populateIdTypeSpinner()"), ErrorSeverity.High);
        }
    }

    private void populateFields(ICamVlns mICamVlns){

        mVlnEditText.setText(mICamVlns.getPlate());

        if (TextUtils.isEmpty(SessionModel.getInstance().getOffenceLocation()) == false){
            String[] offenceLocationArray = SessionModel.getInstance().getOffenceLocation().split("\\|", -1);
            mDescriptionEditText.setText(offenceLocationArray.length > 0 ? offenceLocationArray[0] : null);
            mSuburbEditText.setText(offenceLocationArray.length > 1 ? offenceLocationArray[1] : null);
            mCityEditText.setText(offenceLocationArray.length > 2 ? offenceLocationArray[2] : null);
            return;
        }

        if (TextUtils.isEmpty(SessionModel.getInstance().getCurrentGpsAddress()) == false){
            String offenceGpsLocation = SessionModel.getInstance().getCurrentGpsAddress();

            if (offenceGpsLocation != null) {
                GoogleAddressRefactor googleAddressRefactor = new GoogleAddressRefactor();
                googleAddressRefactor.refactor(offenceGpsLocation, AddressType.Offence);

                mDescriptionEditText.setText(googleAddressRefactor.getDescription());
                mSuburbEditText.setText(googleAddressRefactor.getSuburb());
                mCityEditText.setText(googleAddressRefactor.getTown());
            }
        }
    }

    private void persistOffenceLocationToSession(){

       SessionModel.getInstance().setOffenceLocation(
                pipeFields(mDescriptionEditText.getText().toString(),
                        mSuburbEditText.getText().toString(),
                        mCityEditText.getText().toString()));
    }

    private String pipeFields(String description, String suburb, String city){

        return String.format("%s|%s|%s",
                TextUtils.isEmpty(description) ? Constants.EMPTY_STRING: description,
                TextUtils.isEmpty(suburb) ? Constants.EMPTY_STRING: suburb,
                TextUtils.isEmpty(city) ? Constants.EMPTY_STRING: city);
    }

    private void validateUserInterface(){

        mOkButton.setEnabled(TextUtils.isEmpty(mVlnEditText.getText()) == false &&
                             TextUtils.isEmpty(mDescriptionEditText.getText()) == false &&
                             TextUtils.isEmpty(mSuburbEditText.getText()) == false &&
                             TextUtils.isEmpty(mCityEditText.getText()) == false);
    }

    public void okButtonClick(View view){

        VosiActionCaptureModel vosiActionCapture = new VosiActionCaptureModel();
        vosiActionCapture.setVosiActionID(((VosiActionModel)mVosiActionSpinner.getSelectedItem()).getID());
        vosiActionCapture.setCapturedDateTime(Calendar.getInstance().getTime());
        vosiActionCapture.setCapturedCredentialID(mUser.getCredentialID());
        vosiActionCapture.setLocationLatitude(SessionModel.getInstance().getLatitude());
        vosiActionCapture.setLocationlongitude(SessionModel.getInstance().getLongitude());

        vosiActionCapture.setVLN(mVlnEditText.getText().toString());
        vosiActionCapture.setLocationStreet(mDescriptionEditText.getText().toString());
        vosiActionCapture.setLocationSuburb(mSuburbEditText.getText().toString());
        vosiActionCapture.setLocationTown(mCityEditText.getText().toString());

        if (TextUtils.isEmpty(mCommentsEditText.getText().toString()) == true) {
            vosiActionCapture.setComments(mICamVlns.getVosiReason());
        }else{
            vosiActionCapture.setComments(mCommentsEditText.getText().toString());
        }

        persistOffenceLocationToSession();

        saveVosiAction(vosiActionCapture);

        if ( ((VosiActionModel)mVosiActionSpinner.getSelectedItem()).getID() == VosiActionType.UnpaidInfringement.getCode()) {
            //startFineSearchInfoValidation(mICamVlns.getPlate());

                Intent intent = new Intent();
                intent.putExtra(Constants.ICAM_VLNS, vosiActionCapture.getVLN());
                intent.putExtra(Constants.VOSI_ACTION_TYPE, ((VosiActionModel)mVosiActionSpinner.getSelectedItem()).getID());
                setResult(RESULT_OK, intent);
                finish();

            //mICamVlns.getPlate()
        }
    }

//    private void startFineSearchInfoValidation(String vln){
//
//        Intent intent = new Intent(this, FineSearchInfoValidationActivity.class);
//        intent.putExtra(za.co.kapsch.shared.Constants.VLN, vln);
//        startActivity(intent);
//    }

    private boolean saveVosiAction(VosiActionCaptureModel vosiActionCapture){
        try {
            VosiActionCaptureRepository.insert(vosiActionCapture);
            return true;
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VosiActionActivity::saveVosiAction()"), ErrorSeverity.High);
            return false;
        }
    }
}
