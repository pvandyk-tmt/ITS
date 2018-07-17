package za.co.kapsch.ivehicletest;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;

import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

public class MainActivity extends AppCompatActivity {

    private UserModel mUser;
    private DistrictModel mDistrict;
    private MobileDeviceModel mMobileDevice;
    private String mPrinterMacAddress;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        if (validateIntent() == false) {
            finish();
            return;
        }

//        UserModel user = new UserModel();
//        user.setCredentialID(1);
//        SessionModel.getInstance().setUserId(1);
//        SessionModel.getInstance().setUserName("Super User");
//        SessionModel.getInstance().setPassword("Q!w2e3r4t5");
//        SessionModel.getInstance().setUser(user);

        startDataSynchActivity();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the main_menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main_menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch(item.getItemId()) {
            case R.id.aboutItem:
                showAboutActivity();
                break;
            default:
                return super.onOptionsItemSelected(item);
        }

        return true;
    }

    public void showAboutActivity(){
        Intent intent = new Intent(this, AboutActivity.class);
        startActivity(intent);
    }

    private boolean validateIntent(){

        mUser = Utilities.getUser(this);
        mDistrict = Utilities.getDistrict(this);
        mMobileDevice = Utilities.getMobileDevice(this);
        EndPointConfigModel.getInstance().setCoreGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.CORE_END_POINT));
        EndPointConfigModel.getInstance().setITSGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.ITS_END_POINT));
        EndPointConfigModel.getInstance().setEVRGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.EVR_END_POINT));
        Utilities.setPrinterMacAddress(this);

        if (mUser == null || mDistrict == null ||  mMobileDevice == null) {
            return false;
        }

        return true;
    }

    public void startVehicleInspectionActivity(View view) {
        Intent intent = new Intent(this, VehicleInspectionInitiateActivity.class);
        startActivity(intent);
    }

    private void startDataSynchActivity() {
        Intent intent = new Intent(this, DataSynchronisationActivity.class);
        startActivityForResult(intent, Constants.DATA_SYNC_REQUEST_CODE);
    }

    public void synchroniseDataClick(View view){
        startDataSynchActivity();
    }
}
