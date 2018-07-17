package za.co.kapsch.ipayment;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;

import com.directpayonline.merchant.models.Receipt;

import za.co.kapsch.ipayment.General.App;
import za.co.kapsch.ipayment.General.DataSynchronisation;
import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.Enums.PaymentContext;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

public class MainActivity extends AppCompatActivity implements IMessageCallBack {

    static final String INTENT_EXTRA_TOKEN = "intentToken"; //Transaction Token
    static final String INTENT_EXTRA_TOKEN_REF = "intentTokenRef";//Reference for the transaction token (above) -> received when creating the toke
    static final String INTENT_EXTRA_COMPANY_TOKEN = "IntentCompanyToken";  //Static compay token provided by DPO (Available from you Merchant Portal)
    static final String INTENT_EXTRA_AMOUNT = "IntentTokenAmount"; //amount of the desirec transaction
    static final String INTENT_EXTRA_AMOUNT_CURRENCY = "IntentTokenAmountCurr"; //Currency of the transaction
    static final String INTENT_EXTRA_COUNTRY = "intentCountryDefault";  //The Default country of the merchant (required for Mobile Money Payments)
    static final String INTENT_EXTRA_EXTERNAL_B_RECEIVER_INTENT_FILTER = "intentExternalFilter"; //The intent filter for the Broadcast Receiver that will wait for the response from the Dumapay App once a transaction is complete

    private PaymentContext mPaymentContext;
    private MobileDeviceModel mMobileDevice;
    private UserModel mUser;
    private DistrictModel mDistrict;

    private String mVLN;
    private String mIdNumber;
    private BroadcastReceiver mDumaPayReceiver;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ConfigItemModel.getInstance().refreshConfigurationValues();

        if (validateIntent() == false) {
            finish();
            return;
        }

        try {
            //new DataSynchronisation(this, false).centralConfigItems();
            new DataSynchronisation(this, false).processUpdates();
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::onCreate"), ErrorSeverity.High);
        }

        switch (mPaymentContext) {
            case TrafficFines:
                startSearchFinesActivity(mVLN, mIdNumber);
            default:
                break;
        }

        mDumaPayReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {

                MessageManager.showMessage("DumaPay response", ErrorSeverity.None);

                try {
                    Receipt receipt = (Receipt) intent.getParcelableExtra("receipt");
                    String resultCode = receipt.getResult();

                    if (resultCode.equals("000") == false) {
                        MessageManager.showMessage(receipt.getResultExplanation(), ErrorSeverity.None);
                    }

                }catch (Exception e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
                }
            }
        };
    }

    public void message(String message, boolean append){
        MessageManager.showMessage(message, ErrorSeverity.None);
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
            case R.id.aboutMenuItem:
                showAboutActivity();
                break;
            case R.id.printerMenuItem:
                showPrinterListActivity();
                break;
            case R.id.reprintReceiptMenuItem:
                showSlipReprintActivity();
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

    public void showPrinterListActivity() {
        Intent intent = new Intent(App.getContext(), PrinterListActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        App.getContext().startActivity(intent);
    }

    public void showSlipReprintActivity() {

        if (validatePrinter() == false) {
            return;
        }

        Intent intent = new Intent(App.getContext(), ReceiptReprintActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        App.getContext().startActivity(intent);
    }

    public void trafficFinesButtonClick(View view){

        if (validatePrinter() == false) {
            return;
        }

        startSearchFinesActivity(mVLN, mIdNumber);
    }

    //DumaPay test code

    public void dumaPayTestClick(View view) {

        registerReceiver();

        startDumaPayApplication();
    }



    private void registerReceiver() {
        IntentFilter intentFilter = new IntentFilter("za.co.kapsch.ipayment.DUMAPAY");
        registerReceiver(mDumaPayReceiver, intentFilter);
    }

    public void startDumaPayApplication(){

        Intent launchIntent = getPackageManager().getLaunchIntentForPackage("com.directpayonline.merchant");
        launchIntent.putExtra(INTENT_EXTRA_TOKEN, "BA71AED3-9202-4B75-A325-8273B86D44BB");
        launchIntent.putExtra(INTENT_EXTRA_TOKEN_REF,"840BA71A");
        launchIntent.putExtra(INTENT_EXTRA_COMPANY_TOKEN,"D7E04568-FECD-4CDD-84BF-E5716FE77F62");
        launchIntent.putExtra(INTENT_EXTRA_AMOUNT,"4");
        launchIntent.putExtra(INTENT_EXTRA_AMOUNT_CURRENCY,"ZMW");
        launchIntent.putExtra(INTENT_EXTRA_COUNTRY,"ZAM");
        launchIntent.putExtra(INTENT_EXTRA_EXTERNAL_B_RECEIVER_INTENT_FILTER, "za.co.kapsch.ipayment.DUMAPAY");//"za.co.kapsch.payment.DUMAPAY");

        if (launchIntent != null) {
            startActivity(launchIntent);//null pointer check in case package name was not found
        }
    }

//    public void startDumaPayApplication(){
//
//        Intent launchIntent = getPackageManager().getLaunchIntentForPackage("com.directpayonline.merchant");
//        launchIntent.putExtra(INTENT_EXTRA_TOKEN, mExternalTokenEditText.getText().toString());
//        launchIntent.putExtra(INTENT_EXTRA_TOKEN_REF, mTokenREfEditText.getText().toString());
//        launchIntent.putExtra(INTENT_EXTRA_COMPANY_TOKEN, mCompanyTokenEditText.getText().toString());
//        launchIntent.putExtra(INTENT_EXTRA_AMOUNT, mAmountEditText.getText().toString());
//        launchIntent.putExtra(INTENT_EXTRA_AMOUNT_CURRENCY, mCurrencyEditText.getText().toString());
//        launchIntent.putExtra(INTENT_EXTRA_COUNTRY, mCountryEditText.getText().toString());
//        launchIntent.putExtra(INTENT_EXTRA_EXTERNAL_B_RECEIVER_INTENT_FILTER, "za.co.kapsch.dumapaytestapp.DUMAPAY");
//
//        if (launchIntent != null) {
//            startActivity(launchIntent);//null pointer check in case package name was not found
//        }
//    }

    //

    private boolean validatePrinter(){

        String[] details = Utilities.getPrinterDetails();
        if (details.length != 2){
            MessageManager.showMessage(getResources().getString(R.string.this_option_requires_a_printer_to_be_configured), ErrorSeverity.None);
            return false;
        }

        return true;
    }

    private boolean validateIntent(){

        mUser = Utilities.getUser(this);
        mDistrict = Utilities.getDistrict(this);
        mMobileDevice = Utilities.getMobileDevice(this);
        mPaymentContext = Utilities.getPaymentContext(this);
        EndPointConfigModel.getInstance().setCoreGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.CORE_END_POINT));
        EndPointConfigModel.getInstance().setITSGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.ITS_END_POINT));
        EndPointConfigModel.getInstance().setEVRGateway(Utilities.getEndPoint(this, za.co.kapsch.shared.Constants.EVR_END_POINT));
        Utilities.setPrinterMacAddress(this);

        mVLN = getIntent().getStringExtra(Constants.VLN);
        mIdNumber = getIntent().getStringExtra(Constants.ID_NUMBER);

        if (mUser == null ||
            mDistrict == null ||
            mMobileDevice == null ||
            mPaymentContext == null) {
            return false;
        }

        return true;
    }

    private void startSearchFinesActivity(String vln, String mIdNumber){

        Intent intent = new Intent(this, SearchFinesActivity.class);

        if (vln != null) {
            intent.putExtra(Constants.VLN, vln);
        }

        if (mIdNumber != null) {
            intent.putExtra(Constants.ID_NUMBER, mIdNumber);
        }

        startActivity(intent);
    }
}
