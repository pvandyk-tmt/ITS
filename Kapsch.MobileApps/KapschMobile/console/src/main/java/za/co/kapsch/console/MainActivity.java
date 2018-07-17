package za.co.kapsch.console;

import android.app.Activity;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AppCompatActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.LinearLayout;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ApplicationType;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.ClientAreaBuilder;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.General.LoginDialog;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.console.orm.MobileDeviceRepository;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Interfaces.ILoginDialogCallBack;
import za.co.kapsch.console.Models.MobileDeviceApplicationModel;
import za.co.kapsch.console.orm.DistrictRepository;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.console.Services.LocationReceiver;
import za.co.kapsch.console.orm.MobileDeviceApplicationRepository;
import za.co.kapsch.console.orm.UserRepository;
import za.co.kapsch.shared.orm.ConfigItemRepository;
//import za.co.kapsch.shared.Services.

public class MainActivity extends AppCompatActivity implements ILoginDialogCallBack {

    private UserModel mUser;
    private DistrictModel mDistrict;
    private MobileDeviceModel mMobileDevice;
    private Intent mServiceIntent;
    private LocationReceiver mLocationReceiver;
    private LinearLayout mClienAreaLinearLayout;
    private List<MobileDeviceApplicationModel> mMobileDeviceApplicationList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        setTheme(R.style.AppTheme);
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        startLocationService();
        mClienAreaLinearLayout = (LinearLayout)findViewById(R.id.clientAreaLinearLayout);
        mClienAreaLinearLayout.setBackground(getResources().getDrawable(R.drawable.rtsa));

        updateEndPoints();

        if (validateDeviceRegistration() == false){
            showConfigActivity();
        }else {
            run(false);
        }

        if (isPrinterConfigured() == false){
            showPrinterActivity();
        }
    }

    private void updateEndPoints(){
        try {
            EndPointConfigModel.getInstance().setCoreGateway(ConfigItemRepository.getConfigItem("CORE_GATEWAY").getValue());
            EndPointConfigModel.getInstance().setITSGateway(ConfigItemRepository.getConfigItem("ITS_GATEWAY").getValue());
            EndPointConfigModel.getInstance().setEVRGateway(ConfigItemRepository.getConfigItem("EVR_GATEWAY").getValue());
        }catch (SQLException e){
            za.co.kapsch.shared.MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigurationActivity::updateEndPoints()"), za.co.kapsch.shared.Enums.ErrorSeverity.High);
        }
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
            //case R.id.logoutItem:
                //your action
            //    break;
            case R.id.aboutItem:
                showAboutActivity();
                break;
            case R.id.configItem:
                showConfigActivity();
                break;
            case R.id.printerItem:
                showPrinterActivity();
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

    public void showConfigActivity(){
        Intent intent = new Intent(this, ConfigurationActivity.class);
        startActivityForResult(intent, Constants.CONFIG_REQUEST_CODE);
    }

    public void showPrinterActivity(){
        Intent intent = new Intent(this, PrinterListActivity.class);
        startActivity(intent);
    }

    private boolean isPrinterConfigured(){
        if (Utilities.printerConfigured() == false){
            za.co.kapsch.shared.MessageManager.showMessage(getResources().getString(R.string.this_option_requires_a_printer_to_be_configured), za.co.kapsch.shared.Enums.ErrorSeverity.None);
            return false;
        }
        return true;
    }

    private void startLocationService(){
        mLocationReceiver = new LocationReceiver();
        IntentFilter intentFilter = new IntentFilter(za.co.kapsch.shared.Constants.LOCATION_ACTION);
        //LocalBroadcastManager.getInstance(App.getContext()).registerReceiver(mLocationReceiver.getReceiver(), intentFilter);
        registerReceiver(mLocationReceiver.getReceiver(), intentFilter);

        mServiceIntent = new Intent(this, za.co.kapsch.shared.Services.LocationService.class);
        mServiceIntent.putExtra(za.co.kapsch.shared.Constants.GPS_INTERVAL, ConfigItemModel.getInstance().getGpsInterval());
        startService(mServiceIntent);
    }

    @Override
    public void onStart(){
        super.onStart();
    }

    private void buildClientArea(){

        try {
            //mDistrict = DistrictRepository.getDistrict(mUser.);
            mMobileDevice = MobileDeviceRepository.getMobileDevice();
            ClientAreaBuilder clientAreaBuilder = new ClientAreaBuilder(this, mClienAreaLinearLayout, mUser, mDistrict, mMobileDevice);

            mMobileDeviceApplicationList = MobileDeviceApplicationRepository.getApplications(ApplicationType.InConsole);

            clientAreaBuilder.run(mMobileDeviceApplicationList);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::buildClientArea()"), ErrorSeverity.High);
        }
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {

        switch(requestCode) {

            case Constants.CONFIG_REQUEST_CODE:
                if (validateDeviceRegistration() == false){
                    run(false);
                }
                break;

            case Constants.LOGIN_REQUEST_CODE:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        mUser = data.getParcelableExtra(za.co.kapsch.shared.Constants.USER);
                        SessionModel.getInstance().setUserName(mUser.getUserName());
                        SessionModel.getInstance().setPassword(mUser.getPassword());
                        run(false);
                        break;
                    case Activity.RESULT_CANCELED:
                        break;
                }
                break;

            case Constants.SIGNATURE_REQUEST_CODE:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        byte[] signature = data.getByteArrayExtra(Constants.SIGNATURE);
                        mUser.setSignature(signature);
                        mUser.setSignatureConfirmed(mUser.getSignature() != null);
                        updateUser(mUser);
                        run(false);
                        break;
                    case Activity.RESULT_CANCELED:
                        break;
                }
                break;

            case Constants.DISTRICT_REQUEST_CODE:
                switch (resultCode){
                    case Activity.RESULT_OK:
                         mDistrict = data.getParcelableExtra(Constants.DISTRICT_RESULT);
                         run(false);
                         break;
                    case Activity.RESULT_CANCELED:
                        break;
                }
                break;

            case Constants.DATA_SYNC_REQUEST_CODE:
                switch (resultCode) {
                    case Activity.RESULT_OK:
                        run(true);
                        break;
                    case Activity.RESULT_CANCELED:
                        run(true);
                        break;
                }
                break;
        }
    }

    private void run(boolean synchronisationComplete){

        if (synchronisationComplete == false) {

            if (validateDeviceRegistration() == false) {
                startDataSynchActivity(true);
                return;
            }

            if (mUser == null) {
                startLoginActivity();
                return;
            }

            if (mUser.isSignatureConfirmed() == false) {
                startSignatureActivity();
                return;
            }

            if (mDistrict == null){
                startDistrictListActivity();
                return;
            }

            startDataSynchActivity(false);
            return;
        }

        if (mUser == null){
            startLoginActivity();
            return;
        }

        buildClientArea();
    }

    private boolean validateDeviceRegistration(){
        try {
            //return ((DistrictRepository.getDistrictID() != -1) && (UserRepository.hasUsers() == true));
            return ((DistrictRepository.hasDistricts()) && (UserRepository.hasUsers() == true) && (MobileDeviceRepository.hasMobileDevice() == true));
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::validateDeviceRegistration()"), ErrorSeverity.High);
            return false;
        }
    }

    @Override
    public void loginResult(UserModel user) {

        MessageManager.showMessage(user.getPassword(), ErrorSeverity.None);
    }

    private void startLoginActivity() {
        Intent intent = new Intent(this, LoginActivity.class);
        intent.putExtra(Constants.VALIDATE_LOCALLY, true);
        startActivityForResult(intent, Constants.LOGIN_REQUEST_CODE);
    }

    private void startDataSynchActivity(boolean registerDevice) {
        try {
            Intent intent = new Intent(this, DataSynchronisationActivity.class);
            intent.putExtra(Constants.REGISTER_DEVICE, registerDevice);
            startActivityForResult(intent, Constants.DATA_SYNC_REQUEST_CODE);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    private void startDistrictListActivity() {
        Intent intent = new Intent(this, DistrictListActivity.class);
        startActivityForResult(intent, Constants.DISTRICT_REQUEST_CODE);
    }

    private void startSignatureActivity() {
        try {
            Intent intent = new Intent(this, SignatureActivity.class);
            intent.putExtra(Constants.SIGNATURE, mUser.getSignature());
            startActivityForResult(intent, Constants.SIGNATURE_REQUEST_CODE);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    private boolean updateUser(UserModel user) {
        try {
            UserRepository.updateUser(user);
            return true;
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::updateUser()"), ErrorSeverity.High);
            return false;
        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();

        if (mServiceIntent != null) {
            stopService(mServiceIntent);
        }

        if (mLocationReceiver != null) {
            LocalBroadcastManager.getInstance(this).unregisterReceiver(mLocationReceiver.getReceiver());
        }
    }
}


