package za.co.kapsch.iticket;

import java.util.Calendar;
import java.util.List;

import android.app.Activity;
import android.app.AlertDialog;
import android.bluetooth.BluetoothAdapter;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.location.LocationManager;
import android.provider.Settings;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.InputType;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.Toast;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Models.CourtDateModel;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.iticket.Services.LocationReceiver;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.iticket.orm.TicketNumberRepository;
import za.co.kapsch.shared.Enums.Environment;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CourtInfoModel;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.iticket.Models.InfringementChargeModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.PaymentOption;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.orm.CourtsInfoRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;


public class MainActivity extends AppCompatActivity {

    private String mPassword;

    private static String DATA_SYNC = "data";
    private static String COURT = "court";
    private static int CHARGE_PAGE_INDEX = 7;

    private UserModel mUser;
    private DistrictModel mDistrict;
    private CourtInfoModel mCourtInfo;
    private MobileDeviceModel mMobileDevice;

    Intent mServiceIntent;
    private ImageButton mLocationButton;
    private ImageButton mBlueToothButton;
    private boolean mDataSynchronised;
    private ImageButton mAdminButton;
    private LocationReceiver mLocationReceiver;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        try {
            //SystemClock.sleep(1000);//This is only needed if the splash screen flashes passed to quickly
            //Splash screen is set in AndroidManifest.xml
            //set the application theme after splash screen
            setTheme(R.style.AppTheme);
            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_main);


//            String test = "[{\"Header\": \"HEADER ONE\",\"Details\": \"Lusaka Magistrate Court\",\"Address\": \"John MBita Road\"},{\"Header\": \"HEADER TWO\",\"Details\": \"ACC NAME: RTSA, BRANCH: Lusaka, ACC NO: 876 908 676 909\",\"Address\": \" John MBita Road\"},{\"Header\": \"HEADER THREE\",\"Details\": \"On line payment\",\"Address\": \"http://www.paysite.com\"}]";
//
//            Gson gson = new Gson();
//            List<PaymentOption> paymentOptions = gson.fromJson(test, new TypeToken<List<PaymentOption>>() {}.getType());
//
//            MessageManager.showMessage(paymentOptions.get(0).getAddress(), ErrorSeverity.High);

            mAdminButton = (ImageButton) findViewById(R.id.adminButton);
            mLocationButton = (ImageButton) findViewById(R.id.locationButton);
            mBlueToothButton = (ImageButton) findViewById(R.id.blueToothButton);

            mDataSynchronised = false;

            Utilities.setActionBar(
                    this,
                    R.layout.custom_action_bar,
                    R.id.subText,
                    String.format("iTicket version %s-[%d]", BuildConfig.VERSION_NAME, BuildConfig.VERSION_CODE));

            if (savedInstanceState != null) {
                mUser = savedInstanceState.getParcelable(za.co.kapsch.shared.Constants.USER);
                mCourtInfo = savedInstanceState.getParcelable(COURT);
                mDataSynchronised = savedInstanceState.getBoolean(DATA_SYNC);
            }

            ConfigItemModel.getInstance().refreshConfigurationValues();

            showAdminButtonForStaging();

            mUser = getUser();
            mDistrict = getDistrict();
            mMobileDevice = getMobileDevice();
            SessionModel.getInstance().setMobileDevice(mMobileDevice);
            EndPointConfigModel.getInstance().setCoreGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.CORE_END_POINT));
            EndPointConfigModel.getInstance().setITSGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.ITS_END_POINT));
            EndPointConfigModel.getInstance().setEVRGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.EVR_END_POINT));

            Utilities.setPrinterMacAddress(this);

            if ((mUser == null) || (mDistrict == null) || (mMobileDevice == null)) {
                finish();
                return;
            }

            validateUserInterface();
            initialise();
            registerLocationReceiver();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::onCreate()"), ErrorSeverity.High);
        }
    }

    private UserModel getUser(){

//        mUser = new UserModel();
//        mUser.setId(1);
//        mUser.setFirstName("Test");
//        mUser.setLastName("Tester");
//        mUser.setInfrastructureNumber("02321");
//        mUser.setUserName("pvandyk");
//        mUser.setPassword("pieterd#1");
//
//        SessionModel.getInstance().setUserId(1);
//        SessionModel.getInstance().setUserName("pvandyk");
//        SessionModel.getInstance().setPassword("pieterd#1");
//        SessionModel.getInstance().setDistrictID(1);
//        return true;

        return Utilities.getUser(this);
    }

    private DistrictModel getDistrict(){

        return Utilities.getDistrict(this);
    }

    private MobileDeviceModel getMobileDevice(){

        return Utilities.getMobileDevice(this);
    }

    private void registerLocationReceiver(){
        mLocationReceiver = new LocationReceiver();
        IntentFilter intentFilter = new IntentFilter(za.co.kapsch.shared.Constants.LOCATION_ACTION);
        //LocalBroadcastManager.getInstance(App.getContext()).registerReceiver(mLocationReceiver.getReceiver(), intentFilter);
        registerReceiver(mLocationReceiver.getReceiver(), intentFilter);

//        mServiceIntent = new Intent(this, za.co.kapsch.shared.Services.LocationService.class);
//        mServiceIntent.putExtra(za.co.kapsch.shared.Constants.GPS_INTERVAL, ConfigItemModel.getInstance().getGpsInterval());
//        startService(mServiceIntent);
    }

    public CourtInfoModel getCourtInfo(){
        return mCourtInfo;
    }

    public void setCourtInfo(CourtInfoModel courtInfo){
        mCourtInfo = courtInfo;
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        // Save the user's current game state
        savedInstanceState.putBoolean(DATA_SYNC, mDataSynchronised);
        savedInstanceState.putParcelable(za.co.kapsch.shared.Constants.USER, mUser);
        savedInstanceState.putParcelable(COURT, mCourtInfo);
        //savedInstanceState.putParcelable(VEHICLE, mVehicleInfo);

        // Always call the superclass so it can save the view hierarchy state
        super.onSaveInstanceState(savedInstanceState);
    }

    private void showAdminButtonForStaging(){

        if (EndPointConfigModel.mEnvironment == Environment.Production){
            LinearLayout.LayoutParams params = (LinearLayout.LayoutParams) mAdminButton.getLayoutParams();
            params.height = 0;
            params.width = 0;
            mAdminButton.setLayoutParams(params);
            mAdminButton.setVisibility(View.INVISIBLE);
        }else{
            LinearLayout.LayoutParams params = (LinearLayout.LayoutParams) mAdminButton.getLayoutParams();
            params.height = 150;
            params.width = 150;
            mAdminButton.setLayoutParams(params);
            mAdminButton.setVisibility(View.VISIBLE);
        }
    }

    private boolean isLocationManagerEnabled() {
        return ((LocationManager) getSystemService(Context.LOCATION_SERVICE)).isProviderEnabled(LocationManager.GPS_PROVIDER);
    }

    private boolean isBlueToothEnabled() {
        try {
            BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
            return mBluetoothAdapter.isEnabled();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::isBlueToothEnabled()"), ErrorSeverity.High);
            return false;
        }
    }

    public void showAboutActivity(View view){
        Intent intent = new Intent(this, AboutActivity.class);
        startActivity(intent);
    }

//    public void showNatisSapsActivity(View view){
//        Intent intent = new Intent(this, NatisVosiRequestActivity.class);
//        startActivity(intent);
//    }

    public void loginToAdminActivity(View view){
        mPassword = Constants.EMPTY_STRING;
        displayPasswordDialog();
    }

    public String test = null;

    public void dotServiceActivity(View view){
        Intent intent = new Intent(this, DistanceOverTimeActivity.class);

        intent.putExtra(za.co.kapsch.shared.Constants.USER, mUser);
        intent.putExtra(Constants.COURT_INFO, mCourtInfo);
        intent.putExtra(za.co.kapsch.shared.Constants.DISTRICT, mDistrict);

        startActivity(intent);
    }

    public void displayPasswordDialog(){
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Enter password");

        final EditText input = new EditText(this);
        input.setInputType(InputType.TYPE_CLASS_TEXT | InputType.TYPE_TEXT_VARIATION_PASSWORD);
        builder.setView(input);

        builder.setPositiveButton("OK", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                  showAdminActivity();
            }
        });

        builder.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.cancel();
            }
        });

        builder.show();
    }

    public void showAdminActivity(){
        Intent intent = new Intent(this, AdminActivity.class);
        startActivity(intent);
    }

    private InfringementChargeModel InfringementChargeFromOffenceCode(ChargeInfoModel chargeCode){

        InfringementChargeModel infringementCharge = new InfringementChargeModel();
        infringementCharge.setDescription(chargeCode.getDescription());
        infringementCharge.setSpeed(chargeCode.getSpeed());
        infringementCharge.setId(chargeCode.getId());
        infringementCharge.setChargeCode(chargeCode.getCode());
        infringementCharge.setFineAmount(chargeCode.getFineAmount());
        infringementCharge.setZone(chargeCode.getZone());

        return infringementCharge;
    }

    public void startICamActivity(View view) {

        if (isUserLoggedIn() == false) return;

        if (isCourtSelected() == false) return;

        if (isPrinterConfigured() == false) return;

        if (isDistrictConfigured() == false) return;

        Intent intent = new Intent(this, ICamActivity.class);
        intent.putExtra(za.co.kapsch.shared.Constants.USER, mUser);
        intent.putExtra(Constants.COURT_INFO, mCourtInfo);
        intent.putExtra(za.co.kapsch.shared.Constants.DISTRICT, mDistrict);
        startActivityForResult(intent, Constants.ICAM_REQUEST_CODE);
    }

    private void startWizardActivity(TicketModel ticket, int startPageIndex){
        Intent intent = new Intent(this, WizardActivity.class);
        intent.putExtra(Constants.TICKET_MODEL, ticket);
        intent.putExtra(Constants.WIZARD_START_PAGE_INDEX, startPageIndex);
        startActivityForResult(intent, Constants.TICKET_WIZARD_REQUEST_CODE);
    }

    private void startSection56TicketReprintActivity() {
        Intent intent = new Intent(this, ReprintActivity.class);
        intent.putExtra(Constants.DOCUMENT_TYPE, DocumentType.RoadSideDriver.getNumValue());
        startActivity(intent);
    }

    private void startEndOfDayReportActivity() {
        Intent intent = new Intent(this, EndOfDayReportActivity.class);
        startActivity(intent);
    }

    private void startDataSynchActivity() {
        Intent intent = new Intent(this, DataSynchronisationActivity.class);
        startActivityForResult(intent, Constants.DATA_SYNC_REQUEST_CODE);
    }

    public void findPrinters(View view) {
        Intent intent = new Intent(this, PrinterListActivity.class);
        startActivity(intent);
    }

    public void startCourtActivity() {
        Intent intent = new Intent(this, CourtActivity.class);
        intent.putExtra(Constants.COURT, mCourtInfo);
        startActivityForResult(intent, Constants.COURT_REQUEST_CODE);
    }

    public void startCourtDelimitationActivity() {
        DelimitationDescriptor delimitationDescriptor =
                new DelimitationDescriptor(
                        App.getContext().getResources().getString(R.string.court_info_title),
                        App.getContext().getResources().getString(R.string.court_info_court),
                        App.getContext().getResources().getString(R.string.court_info_court_room),
                        null,//App.getContext().getResources().getString(R.string.court_info_court_date),
                        null,
                        null,
                        new CourtsInfoRepository(),
                        mCourtInfo == null ? new CourtInfoModel() : mCourtInfo,
                        true);

        Intent intent = new Intent(this, DelimitationSelectionActivity.class);
        intent.putExtra(Constants.DELIMITATION_DATA, delimitationDescriptor);
        startActivityForResult(intent, Constants.COURT_REQUEST_CODE);
    }

    public void setCourt(View view) {
        startCourtDelimitationActivity();
    }

    public void reprintSection56Ticket(View view){
        if (isPrinterConfigured() == true){
            startSection56TicketReprintActivity();
        }
    }

    public void syncToBackend(View view){
        startDataSynchActivity();
    }

    public void endOfDayReport(View view){
        startEndOfDayReportActivity();
    }

    public void startHandWrittenWizard(View view) {

        if (isUserLoggedIn() == false) return;

        if (isCourtSelected() == false) return;

        if (isPrinterConfigured() == false) return;

        if (isDistrictConfigured() == false) return;

        TicketModel ticket = getNewTicket(DocumentType.RoadSideDriver, mUser, mCourtInfo, mDistrict);
        if (ticket == null) return;

        startWizardActivity(ticket, -1);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {

        if (resultCode == Activity.RESULT_OK) {
            switch(requestCode) {

                case Constants.COURT_REQUEST_CODE:
                    mCourtInfo = data.getParcelableExtra(Constants.DELIMITATION);
                    CourtDateModel courtDate = new CourtDateModel();
                    courtDate.setCourtId(mCourtInfo.getCourt().getId());
                    courtDate.setCourtRoomId(mCourtInfo.getCourtRoom().getId());
                    courtDate.setDate(Utilities.addDaysToDate(Calendar.getInstance().getTime(), 14));
                    mCourtInfo.setCourtDate(courtDate);
                    break;
                case Constants.DATA_SYNC_REQUEST_CODE:
                    initialise();
                    break;
                case Constants.TICKET_WIZARD_REQUEST_CODE:
                    try {

                        TicketModel ticketModel = null;

                        if (data != null) {
                            ticketModel = data.getParcelableExtra(Constants.TICKET_MODEL);
                        }

                        if (ticketModel != null) {
                            ticketModel.getInfringement().getInfringementCharges()[0] = null;
                            ticketModel.getInfringement().getInfringementCharges()[1] = null;
                            ticketModel.getInfringement().getInfringementCharges()[2] = null;

                            TicketNumberModel ticketNumber = TicketNumberRepository.getNextTicketNumber(ticketModel.getDocumentType());

                            if (ticketNumber != null) {
                                ticketModel.getInfringement().setTicketNumber(ticketNumber.getNumberValue());
                                ticketModel.getInfringement().setExternalToken(ticketNumber.getExternalToken());
                                ticketModel.getInfringement().setExternalTokenReference(ticketNumber.getExternalTokenReference());
                                ticketModel.setPersisted(false);
                                startWizardActivity(ticketModel, CHARGE_PAGE_INDEX);
                            }else{
                                MessageManager.showMessage(getString(R.string.no_more_tickets_available), ErrorSeverity.None);
                            }
                        }
                    }catch (Exception e){
                        MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::onActivityResult()"), ErrorSeverity.High);
                    }
               }
        }else{
            switch(requestCode) {
                case Constants.DATA_SYNC_REQUEST_CODE:
                    initialise();
                    break;
            }
        }

        validateUserInterface();
    }

    public void initialise(){

        if (mCourtInfo == null) {
            if (hasCourts() == true) {
                startCourtDelimitationActivity();
                return;
            }
        }

        if (mDataSynchronised == false) {
            startDataSynchActivity();
        }

        mDataSynchronised = true;
    }

    private boolean hasCourts(){
        try {
            return (CourtsInfoRepository.hasCourts());
        } catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e,"MainActivity::hasCourts()"), ErrorSeverity.High);
            return false;
        }
    }

    private TicketModel getNewTicket(DocumentType documentType, UserModel user, CourtInfoModel courtInfo, DistrictModel district) {

        return TicketModel.getNewTicket(documentType, user, courtInfo, district, false);
    }

    private void validateUserInterface(){
        boolean locationManagerEnabled = isLocationManagerEnabled();
        mLocationButton.setImageResource(locationManagerEnabled ?  R.drawable.gps_on : R.drawable.gps_off);

        boolean blueToothEnabled = isBlueToothEnabled();
        mBlueToothButton.setImageResource(blueToothEnabled ?  R.drawable.bluetooth_on : R.drawable.bluetooth_off);
    }

    private boolean isDistrictConfigured() {
        if (mDistrict == null) {
            MessageManager.showMessage(getResources().getString(R.string.activity_main_district_not_configured), ErrorSeverity.None);
            return false;
        }
        return true;
    }

    private boolean isCourtSelected(){

         if (mCourtInfo == null) {
            MessageManager.showMessage(getResources().getString(R.string.activity_main_court_not_selected), ErrorSeverity.None);
            return false;
        }

        return true;
    }

    private boolean isUserLoggedIn(){
        if (mUser == null){
            MessageManager.showMessage(getResources().getString(R.string.activity_main_user_not_logged_in), ErrorSeverity.None);
            return false;
        }
        return true;
    }

    private boolean isPrinterConfigured(){
        if (Utilities.printerConfiguredEx() == false){
            MessageManager.showMessage(getResources().getString(R.string.activity_main_printer_not_configured), ErrorSeverity.None);
            return false;
        }
        return true;
    }

    public void copyDatabase(View view){
        if (Utilities.copyDatabaseFile("/data/data/za.co.kapsch.iticket/databases/iTicket.db") == true){
            MessageManager.showMessage(getResources().getString(R.string.database_backed_up_message), ErrorSeverity.None);
        }
    }

    public void locationButtonClick(View view){
        Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
        startActivityForResult(intent, Constants.LOCATION_REQUEST_CODE);
    }

    public void blueToothButtonClick(View view){
        Intent intent = new Intent(Settings.ACTION_BLUETOOTH_SETTINGS);
        startActivityForResult(intent, Constants.BLUETOOTH_REQUEST_CODE);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();

        if (mServiceIntent != null) {
            stopService(mServiceIntent);
        }

        if (mLocationReceiver != null) {
            //LocalBroadcastManager.getInstance(this).unregisterReceiver(mLocationReceiver.getReceiver());
            unregisterReceiver(mLocationReceiver.getReceiver());
        }
   }
}
