package za.co.kapsch.ivehicletest;

import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.General.DataSynchronisation;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.Utilities;

public class DataSynchronisationActivity extends AppCompatActivity implements IMessageCallBack {

    private static final int IDLE = 1;
    private static final int BUSY = 2;

    private boolean mTicketNumberRangeRequested;
    private int mCurrentMode;
    private Button mUpdateButton;
    private StringBuilder mStringBuilder;
    private TextView mInformationTextView;
    private DataSynchronisation mDataSynchronisation;
    private Handler handler = new Handler();

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {
            processUpdates();
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_data_synchronisation);

        mStringBuilder = new StringBuilder();
        mInformationTextView = (TextView) findViewById(R.id.informationTextView);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        mUpdateButton = (Button) findViewById(R.id.updateButton);

        mTicketNumberRangeRequested = false;

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.data_service_title)));

        handler.postDelayed(runnable, 50);
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

    private void processUpdates() {

        mCurrentMode = BUSY;
        mUpdateButton.setEnabled(false);

        try {
            if (mDataSynchronisation == null) {
                mDataSynchronisation = new DataSynchronisation(this, false);
            }
            mDataSynchronisation.processUpdates(false);
        }catch (Exception e){
            displayMessage(Utilities.exceptionMessage(e, null), true);
            mUpdateButton.setText(getResources().getString(R.string.query_updates));
            mUpdateButton.setEnabled(true);
            mCurrentMode = IDLE;
        }
    }

     public void message(String message, boolean appendText){

         switch (message) {
            case Constants.SYNCHRONISATION_REQUIRED:
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.update));
                displayMessage(getResources().getString(R.string.synchronisation_required), appendText);
                break;
            case Constants.FINISHED_MESSAGE:
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.query_updates));
                displayMessage(getResources().getString(R.string.data_is_up_to_date), appendText);
                mDataSynchronisation = null;
                mCurrentMode = IDLE;
                finish();
                break;
            case Constants.FAILED_MESSAGE:
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.query_updates));
                mCurrentMode = IDLE;
                break;
            default:
                displayMessage(message, appendText);
                mUpdateButton.setEnabled(true);
                mUpdateButton.setText(getResources().getString(R.string.query_updates));
                mCurrentMode = IDLE;
                break;
         }
    }

    private void displayMessage(String message, boolean appendText){

        if (appendText == false) {
            mStringBuilder = new StringBuilder();
            mInformationTextView.setText(Constants.EMPTY_STRING);
        }

        mStringBuilder.append(mInformationTextView.getText().length() == 0 ? message : String.format("\r\n%s", message));

        mInformationTextView.setText(mStringBuilder.toString());
    }

//    public void onActivityResult(int requestCode, int resultCode, Intent data) {
//        if (resultCode == Activity.RESULT_OK) {
//            if (requestCode == Constants.DISTRICT_REQUEST_CODE){
//                try {
//                    DistrictModel district =  data.getParcelableExtra(Constants.DISTRICT_RESULT);
//                    DistrictRepository.create(district);
//                } catch (SQLException e) {
//                    MessageManager.showMessage(Utilities.exceptionMessage(e, "DataSynchronisationActivity:: onActivityResult()"), ErrorSeverity.High);
//                }
//                //registerDevice();
//            }
//        }
//    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
