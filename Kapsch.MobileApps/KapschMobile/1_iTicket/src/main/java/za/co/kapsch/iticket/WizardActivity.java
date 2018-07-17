package za.co.kapsch.iticket;

import android.app.Activity;
import android.content.Intent;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.view.animation.AnimationUtils;
import android.widget.Button;
import android.widget.TextView;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.Enums.AddressType;
import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Interfaces.IFinalWizardPage;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Utilities;

public class WizardActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private TicketModel mTicket;
    private int mStartPageIndex;
    private Type mCancellationActivityType;
    private List<Fragment> mWizardPages = new ArrayList<Fragment>();
    private boolean mWizardLocked;
    private boolean mIssueAdditionalNotice;

    private TextView mNotificationTextView;
    private ArrayList<FineModel> mOutstandingPersonViolations;
    private ArrayList<FineModel> mOutstandingVehicleViolations;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_wizard);

        Intent intent = getIntent();
        mTicket = intent.getParcelableExtra(Constants.TICKET_MODEL);
        mStartPageIndex = intent.getIntExtra(Constants.WIZARD_START_PAGE_INDEX, -1);
        mNotificationTextView = (TextView) findViewById(R.id.notificationTextView);
        Utilities.hideView(mNotificationTextView);

//        if (intent.getBooleanExtra(Constants.TOGGLE_WIFI, false) == true){
//            Utilities.wifiOff();
//        }

        registerWizardPages(mTicket.getDocumentType());

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        //if  wizardExtention exists
        loadWizardExtention(new WizardExtentionFragment());

        if (currentPageIndex() == -1) {
            if (mStartPageIndex == -1) {
                next(findViewById(R.id.nextButton));
            }else{
                loadWizardPage(mStartPageIndex);
            }
        }

        mWizardLocked = false;
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
    public void onBackPressed(){

        if (mTicket.isLocallyGeneratedTicket() == true) {
            if (mWizardLocked == false) {
                startCancellationReasonActivity();
            }
        }else{
            finish();
        }
    }

    public void enableNextButton(boolean value) {
        nextButton().setEnabled(value);
    }

    public void enableBackButton(boolean value) {
        backButton().setEnabled(value);
    }

    public boolean wizardLocked() { return mWizardLocked; }

    public void setWizardLocked(boolean wizardLocked)
    {
        mWizardLocked = wizardLocked;
    }

    public void setIssueAdditionalNotice(boolean issueAdditionalNotice)
    {
        mIssueAdditionalNotice = issueAdditionalNotice;
    }

    public void next(View view) {
        try {
            if (isWizardFinished()) {
                wizardFinished();
                issueAdditionalNotice();
                finish();
                return;
            }

            int index = currentPageIndex();
            index = (index == mWizardPages.size() - 1) ? index : index + 1;
            loadWizardPage(index);
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardActivity::next()"), ErrorSeverity.High);
        }
    }

    public void back(View view) {
        int index = currentPageIndex();
        index = index <= 0 ? index : index - 1;
        loadWizardPage(index);
    }

    private void issueAdditionalNotice(){

        if (mIssueAdditionalNotice) {
            Intent intent = new Intent();
            intent.putExtra(Constants.TICKET_MODEL, mTicket);
            setResult(RESULT_OK, intent);
            return;
        }

        setResult(RESULT_OK);
    }

    public void wizardFinished() {
        ((IFinalWizardPage) currentPage()).onFinish();
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == Activity.RESULT_OK) {
            if (requestCode == Constants.WIZARD_CANCELLATION_REQUEST_CODE) {
                finish();
            }
        }
    }

    public TicketModel getTicketModel() {
        return mTicket;
    }

    //add wizard pages here, wizard pages are of type Fragment
    private void registerWizardPages(DocumentType documentType) {

        mWizardPages.add(setPageIndex(new DriversLicenceFragment(), mWizardPages.size(), null, -1));
        mWizardPages.add(setPageIndex(new DriversLicenceFragmentEx(), mWizardPages.size(), null, -1));
        mWizardPages.add(setPageIndex(new VehicleLicenceFragment(), mWizardPages.size(), null, -1));
        mWizardPages.add(setPageIndex(new VehicleLicenceFragmentEx(), mWizardPages.size(), null, -1));

        mWizardPages.add(setPageIndex(new AddressCaptureFragmentEx(), mWizardPages.size(),Constants.ADDRESS_TYPE, AddressType.toInteger(AddressType.Physical)));
        mWizardPages.add(setPageIndex(new AddressCaptureFragmentEx(), mWizardPages.size(), Constants.ADDRESS_TYPE, AddressType.toInteger(AddressType.Postal)));

        if (mTicket.isLocallyGeneratedTicket() == true) {
            mWizardPages.add(setPageIndex(new AddressCaptureFragmentEx(), mWizardPages.size(), Constants.ADDRESS_TYPE, AddressType.toInteger(AddressType.Offence)));
            mWizardPages.add(setPageIndex(new ChargeFragment(), mWizardPages.size(), null, -1));
        }

        mWizardPages.add(setPageIndex(new SignatureCaptureFragment(), mWizardPages.size(), null, -1));
        mWizardPages.add(setPageIndex(new PrintFragment(), mWizardPages.size(), null, -1));

        //this activity must return a boolean true or false in the onActivityResult method
        mCancellationActivityType = CancellationReasonActivity.class;
    }

    private Fragment newFragment(AddressType addressType){

        AddressCaptureFragmentEx addressCaptureFragmentEx = new AddressCaptureFragmentEx();
        Bundle args = new Bundle();
        args.putInt(Constants.ADDRESS_TYPE, AddressType.toInteger(addressType));
        addressCaptureFragmentEx.setArguments(args);

        return addressCaptureFragmentEx;
    }

    private void loadWizardPage(int index) {
        try {
            FragmentManager fragmentManager = getSupportFragmentManager();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();

            if (mWizardPages.get(index).isAdded()) {
                fragmentTransaction.add(R.id.fragment_container, mWizardPages.get(index));
            } else {
                fragmentTransaction.replace(R.id.fragment_container, mWizardPages.get(index));
            }

            fragmentTransaction.commit();
            fragmentManager.executePendingTransactions();
            validateButtons();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardActivity::loadWizardPage()"), ErrorSeverity.High);
        }
    }

    private void loadWizardExtention(Fragment wizardExtentionFragment) {
        try {
            FragmentManager fragmentManager = getSupportFragmentManager();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
            fragmentTransaction.replace(R.id.clientLinearLayout, wizardExtentionFragment);
            fragmentTransaction.commit();
            fragmentManager.executePendingTransactions();
            validateButtons();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardActivity::loadWizardExtention()"), ErrorSeverity.High);
        }
    }

    private Fragment setPageIndex(Fragment fragment, int index, String additionalIfo, int additionalInfoValue) {
        Bundle bundle = new Bundle();
        bundle.putInt(Constants.INDEX_TEXT, index);
        if(additionalIfo != null){
            bundle.putInt(additionalIfo, additionalInfoValue);
        }
        fragment.setArguments(bundle);
        return fragment;
    }

    private Fragment currentPage() {
        FragmentManager fragmentManager = getSupportFragmentManager();
        return fragmentManager.findFragmentById(R.id.fragment_container);
    }

    private int currentPageIndex() {
        Fragment fragment = currentPage();
        return fragment != null ? fragment.getArguments().getInt(Constants.INDEX_TEXT) : -1;
    }

    private void validateButtons() {
        if (wizardLocked() == true) return;

        backButton().setEnabled(currentPageIndex() != 0);

        nextButton().setText((currentPageIndex() == mWizardPages.size() - 1) ?
                getResources().getString(R.string.finish_Button_Text) :
                getResources().getString(R.string.next_Button_Text));
    }

    private void startCancellationReasonActivity() {
        Intent intent = new Intent(this, (Class<?>) mCancellationActivityType);
        intent.putExtra(Constants.TICKET_MODEL, mTicket);
        startActivityForResult(intent, Constants.WIZARD_CANCELLATION_REQUEST_CODE);
    }

    private void startOutstandingViolationsActivity(ArrayList<FineModel> outstandingPersonViolations, ArrayList<FineModel> outstandingVehicleViolations) {
        try {
            Intent intent = new Intent(this, OutstandingViolationsActivity.class);
            intent.putExtra(Constants.OUTSTANDING_PERSON_VIOLATIONS, outstandingPersonViolations);
            intent.putExtra(Constants.OUTSTANDING_VEHICLE_VIOLATIONS, outstandingVehicleViolations);
            startActivity(intent);
        }catch (Exception e){
            MessageManager.showMessage(e.getMessage(), ErrorSeverity.High);
        }
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        savedInstanceState.putParcelable("TicketModel", mTicket);
        super.onSaveInstanceState(savedInstanceState);
    }

    @Override
    public void onRestoreInstanceState(Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);
        mTicket = savedInstanceState.getParcelable("TicketModel");
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }

    private boolean isWizardFinished() {
        return currentPageIndex() == mWizardPages.size() - 1;
    }

    private Button nextButton() {
        return (Button) findViewById(R.id.nextButton);
    }

    private Button backButton() {
        return (Button) findViewById(R.id.backButton);
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try {
            if (asyncResultModel == null) return;

            if (asyncResultModel.getObject() == null)  return;

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_ID:
                    mOutstandingPersonViolations = (ArrayList<FineModel>) asyncResultModel.getObject();
                    break;

                case Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_VLN:
                    mOutstandingVehicleViolations = (ArrayList<FineModel>) asyncResultModel.getObject();
                    break;

                default:
                    return;
            }

            violationNotification();

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardActivity::finishedCallBack()"), ErrorSeverity.None);
        }
    }

    public void resetOutstandingViolations(){
        mOutstandingPersonViolations = null;
        mOutstandingVehicleViolations = null;
    }

    private void violationNotification(){

        if (mOutstandingPersonViolations != null && mOutstandingVehicleViolations != null) {
            if (mOutstandingPersonViolations.size() > 0 || mOutstandingVehicleViolations.size() > 0)
            {
                Utilities.showView(mNotificationTextView);
                mNotificationTextView.startAnimation(AnimationUtils.loadAnimation(WizardActivity.this, android.R.anim.slide_in_left));
            }else{
                Utilities.hideView(mNotificationTextView);
            }
        }else{
            Utilities.hideView(mNotificationTextView);
        }
    }

    public void notificationClick(View view){

//        String idNumber = mTicket.getOffender().getIdNumber();
//
//        while(true){
//            boolean valueFound = false;
//            for (OutstandingViolationModel outstandingViolation : mOutstandingVehicleViolations){
//                if (outstandingViolation.getOffenderIDNumber().equals(idNumber)) {
//                    mOutstandingVehicleViolations.remove(outstandingViolation);
//                    valueFound = true;
//                    break;
//                }
//            }
//
//            if (valueFound == false) {
//                break;
//            }
//        }

        startOutstandingViolationsActivity(mOutstandingPersonViolations, mOutstandingVehicleViolations);
    }
}
