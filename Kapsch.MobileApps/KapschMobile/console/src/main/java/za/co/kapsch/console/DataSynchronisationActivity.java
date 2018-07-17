package za.co.kapsch.console;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import java.sql.SQLException;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.General.ConfigItemSynchroniser;
import za.co.kapsch.console.General.DataServiceRequest;
import za.co.kapsch.console.General.DataSynchronisation;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.console.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.console.orm.DistrictRepository;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

public class DataSynchronisationActivity extends AppCompatActivity implements IAsyncProcessCallBack, IMessageCallBack {

    private static final int IDLE = 1;
    private static final int BUSY = 2;

    private UserModel mUser;
    private int mCurrentMode;
    private Button mUpdateButton;
    private boolean mRegisterDevice;
    private StringBuilder mStringBuilder;
    private TextView mInformationTextView;

    private DataSynchronisation mDataSynchronisation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_data_synchronisation);

        mStringBuilder = new StringBuilder();
        mInformationTextView = (TextView) findViewById(R.id.informationTextView);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        mUpdateButton = (Button) findViewById(R.id.updateButton);

        mRegisterDevice = getIntent().getBooleanExtra(za.co.kapsch.console.General.Constants.REGISTER_DEVICE, false);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.data_updates)));

        if (mRegisterDevice == true){
            registerDevice();
        }else{
            processUpdates();
        }

        mInformationTextView.setOnLongClickListener(new View.OnLongClickListener() {
            @Override
            public boolean onLongClick(View view) {
                mDataSynchronisation.installApks();
                return true;
            }
        });
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onBackPressed() {
        if (mCurrentMode == IDLE) {
            super.onBackPressed();
        }

    }

    public void processUpdates(View view) {
        displayMessage(Constants.EMPTY_STRING, false);
        processUpdates();
    }

    private void registerDevice(){

        try {
            if (mUser == null) {
                startLoginActivity();
                return;
            }

            SessionModel.getInstance().setUserId(mUser.getId());
            SessionModel.getInstance().setUserName(mUser.getUserName());
            SessionModel.getInstance().setPassword(mUser.getPassword());
            SessionModel.getInstance().setUser(mUser);

            mCurrentMode = BUSY;
            mUpdateButton.setEnabled(false);
            displayMessage(Constants.EMPTY_STRING, false);

//            if (DistrictRepository.getDistrict() == null) {
//                startDistrictListActivity();
//                return;
//            }

            displayMessage(Utilities.getString(R.string.validating_device_registration), true);
            DataServiceRequest.registerDevice(this, this);

//        } catch (SQLException e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "DataSynchronisationActivity::registerDevice()"), ErrorSeverity.High);
//            mCurrentMode = IDLE;
//            mUpdateButton.setEnabled(true);
        } catch (Exception e) {
            displayMessage(Utilities.exceptionMessage(e, "DataSynchronisationActivity::registerDevice()"), true);
            mCurrentMode = IDLE;
            mUpdateButton.setEnabled(true);
        }
    }

    private void processUpdates() {
        mCurrentMode = BUSY;
        mUpdateButton.setEnabled(false);

        try {
            if (mDataSynchronisation == null) {
                mDataSynchronisation = new DataSynchronisation(this);
            }
            mDataSynchronisation.processUpdates();
        }catch (Exception e){
            displayMessage(Utilities.exceptionMessage(e, null), true);
            mUpdateButton.setText(getResources().getString(R.string.query_updates));
            mUpdateButton.setEnabled(true);
            mCurrentMode = IDLE;
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {

            if (asyncResultModel == null){
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_REGISTER_DEVICE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            MobileDeviceModel mobileDevice = (MobileDeviceModel)asyncResultModel.getObject();
                            displayMessage(new ConfigItemSynchroniser().insertMobileDevice(mobileDevice), true);
                            processUpdates();
                            return;
                        case FAILED:
                            displayMessage(asyncResultModel.getMessage(), true);
                            break;
                    }
                    break;
            }

            mUpdateButton.setEnabled(true);
            mCurrentMode = IDLE;
        }catch (Exception e) {
            displayMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), true);
            mUpdateButton.setEnabled(true);
            mCurrentMode = IDLE;
            return;
        }
    }

    public void message(String message, boolean appendText){

        switch (message) {
            case Constants.SYNCHRONISATION_REQUIRED:
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.update));
                displayMessage(getResources().getString(R.string.synchronization_required), appendText);
                mCurrentMode = IDLE;
                return;
            case Constants.FINISHED_MESSAGE:
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.query_updates));
                displayMessage(getResources().getString(R.string.data_is_up_to_date), appendText);
                mDataSynchronisation = null;
                mCurrentMode = IDLE;
                finish();
                return;
            case Constants.FAILED_MESSAGE:
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.query_updates));
                mCurrentMode = IDLE;
                return;
            default:
        }

        displayMessage(message, appendText);
    }

    private void displayMessage(String message, boolean appendText){

        if (appendText == false) {
            mStringBuilder = new StringBuilder();
            mInformationTextView.setText(Constants.EMPTY_STRING);
        }

        mStringBuilder.append(mInformationTextView.getText().length() == 0 ? message : String.format("\r\n%s", message));

        mInformationTextView.setText(mStringBuilder.toString());
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {

        switch(requestCode) {
            case Constants.DISTRICT_REQUEST_CODE:

                switch (resultCode){
                    case Activity.RESULT_OK:
                        try {
                            DistrictModel district = data.getParcelableExtra(Constants.DISTRICT_RESULT);
                            DistrictRepository.create(district);
                            registerDevice();
                            break;
                        } catch (SQLException e) {
                            MessageManager.showMessage(Utilities.exceptionMessage(e, "DataSynchronisationActivity:: onActivityResult()"), ErrorSeverity.High);
                        }
                        break;
                    case Activity.RESULT_CANCELED:
                        mCurrentMode = IDLE;
                        break;
                    case Constants.ACTIVTIY_FAILED_WITH_INVALID_LOGIN_CREDENTIALS:
                        startLoginActivity();
                        break;
                }
                break;

            case Constants.LOGIN_REQUEST_CODE:

                switch (resultCode) {
                    case Activity.RESULT_OK:
                    case Constants.LOGIN_REQUEST_CODE:
                        mUser = data.getParcelableExtra(za.co.kapsch.shared.Constants.USER);
                        registerDevice();
                        break;
                    case Activity.RESULT_CANCELED:
                        break;
                }
            default:
                break;
        }

//        if (resultCode == Activity.RESULT_OK) {
//            switch (requestCode) {
//                case Constants.DISTRICT_REQUEST_CODE:
//                    try {
//                        DistrictModel district = data.getParcelableExtra(Constants.DISTRICT_RESULT);
//                        DistrictRepository.create(district);
//                        registerDevice();
//                        break;
//                    } catch (SQLException e) {
//                        MessageManager.showMessage(Utilities.exceptionMessage(e, "DataSynchronisationActivity:: onActivityResult()"), ErrorSeverity.High);
//                    }
//                    break;
//                case Constants.LOGIN_REQUEST_CODE:
//                    mUser = data.getParcelableExtra(za.co.kapsch.shared.Constants.USER);
//                    registerDevice();
//                    break;
//                default:
//                    break;
//            }
//        }
    }

    private void startDistrictListActivity() {
        Intent intent = new Intent(this, DistrictListActivity.class);
        startActivityForResult(intent, Constants.DISTRICT_REQUEST_CODE);
    }

    private void startLoginActivity() {
        Intent intent = new Intent(this, LoginActivity.class);
        intent.putExtra(Constants.VALIDATE_LOCALLY, false);
        startActivityForResult(intent, Constants.LOGIN_REQUEST_CODE);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
