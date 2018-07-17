package za.co.kapsch.ivehicletest;

import android.app.Activity;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.Parcelable;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.Interfaces.IFinalWizardPage;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQueryModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQuestionModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
//import za.co.kapsch.shared.Stopwatch;
import za.co.kapsch.shared.Utilities;

public class WizardActivity extends AppCompatActivity {

    private List<VehicleInspectionResultModel> mVehicleInspectionResultList;
    private VehicleInspectionQueryModel mVehicleInspectionQuery;
    //private List<VehicleInspectionQuestionModel> mVehicleInspectionQuestionList;
    private int mStartPageIndex;
    private Type mCancellationActivityType;
    private List<Fragment> mWizardPages = new ArrayList<Fragment>();
    private boolean mWizardLocked;
    private boolean mLoadPagesDynamically;
    private boolean mLastWizardPageReached;
    private int mLastPageIndex;
    private Date mWizardStartTime;
    private Date mWizardEndTime;
    private String mBookingReferenceNumber;

    private boolean mHasFinalOperation;
    private String mFinalOperationButtonText;

    private DialogInterface.OnClickListener mAlertDialogCancelWizardOnClickListener = new DialogInterface.OnClickListener() {
        @Override
        public void onClick(DialogInterface dialog, int which) {
            if (which == Constants.YES) {
                finish();
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_wizard);

        Intent intent = getIntent();
        mLastPageIndex = -1;
        mStartPageIndex = intent.getIntExtra(Constants.WIZARD_START_PAGE_INDEX, -1);
        mLoadPagesDynamically = intent.getBooleanExtra(Constants.LOAD_WIZARDPAGE_DYNAMICALLY, false);
        //mVehicleInspectionQuestionList = intent.getParcelableArrayListExtra(Constants.VEHICLE_INSPECTION_QUESTION_LIST);
        mVehicleInspectionQuery = intent.getParcelableExtra(Constants.VEHICLE_INSPECTION_QUERY);
        mBookingReferenceNumber = intent.getStringExtra(Constants.BOOKING_REFERENCE_NUMBER);

        mHasFinalOperation = intent.getBooleanExtra(Constants.HAS_FINAL_OPRATION, false);
        mFinalOperationButtonText = intent.getStringExtra(Constants.FINAL_OPERATION_TEXT);

        registerWizardPages(mVehicleInspectionQuery.getVehicleInspectionQuestionList());

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        if (currentPageIndex() == -1) {
            mWizardStartTime = Calendar.getInstance().getTime();
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

        if (mWizardLocked == false) {
            //startCancellationReasonActivity();
            Utilities.displayDecisionMessage(
                    getResources().getString(R.string.are_you_sure_you_want_to_cancel_the_test),
                    this,
                    mAlertDialogCancelWizardOnClickListener);
        }
    }

    //add wizard pages here, wizard pages are of type Fragment
    private void registerWizardPages(List<VehicleInspectionQuestionModel> vehicleInspectionQuestionList) {

        if (mLoadPagesDynamically == true){
            mWizardPages.add(setPageIndex(new VehicleInspectionFragment(), mWizardPages.size(), vehicleInspectionQuestionList.get(0)));
        }else {
            for (VehicleInspectionQuestionModel vehicleInspectionQuestion : vehicleInspectionQuestionList) {
                mWizardPages.add(setPageIndex(new WizardPage(), mWizardPages.size(), vehicleInspectionQuestion));
            }

            mWizardPages.add(setPageIndex(new VehicleInspectionReviewFragment(), mWizardPages.size(), null));
        }

        //this activity must return a boolean true or false in the onActivityResult method
        mCancellationActivityType = CancellationReasonActivity.class;
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

    public void next(View view) {

        try {

            if (currentPage() != null) {

                if (((WizardPage)currentPage()).validateNextPage() == false){
                    return;
                }

                if (mLastWizardPageReached == false) {
                    WizardPage wizardPage = (WizardPage) currentPage();
                    if (wizardPage.getNextPageID() != 0) {
                        addDynamicWizardPage(wizardPage.getNextPageID());
                    } else if (currentPageIndex() != mLastPageIndex){
                        mWizardPages.add(setPageIndex(new VehicleInspectionReviewFragment(), mWizardPages.size(), null));
                        mLastPageIndex = mWizardPages.size()-1;
                    }
                }
            }

            //next was clicked on the page before the last page
            if (((mLastPageIndex-1) == currentPageIndex()) && (currentPageIndex() != -1)){
                mLastWizardPageReached = true;
            }

            if (isWizardFinished()) {
                wizardFinished();
                //if (wizardFinished() == true) {
                //    finish();
                //}
                return;
            }

            int index = currentPageIndex();
            index = (index == mWizardPages.size() - 1) ? index : index + 1;
            loadWizardPage(index);
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardActivity::next()"), ErrorSeverity.High);
        }
    }

    public void goToPage(int index) {
        try {
            loadWizardPage(index);
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "WizardActivity::next()"), ErrorSeverity.High);
        }
    }

    public void addDynamicWizardPage(long nextQuestionID) throws Exception{

        for(VehicleInspectionQuestionModel vehicleInspectionQuestion : mVehicleInspectionQuery.getVehicleInspectionQuestionList()){
            if (vehicleInspectionQuestion.getTestQuestionID() == nextQuestionID){

                for(Fragment fragment : mWizardPages){
                    if (((VehicleInspectionFragment)fragment).getVehicleInspectionQuestion().getTestQuestionID() == vehicleInspectionQuestion.getTestQuestionID()){
                        return;
                    }
                }

                mWizardPages.add(setPageIndex(new VehicleInspectionFragment(), mWizardPages.size(), vehicleInspectionQuestion));
                return;
            }
        }

        throw new Exception("WizardActivity::addDynamicWizardPage() - Question for next wizard page does not exit");
    }

    public void back(View view) {
        int index = currentPageIndex();
        index = index <= 0 ? index : index - 1;
        loadWizardPage(index);
    }

    public boolean wizardFinished() {
        mWizardEndTime = Calendar.getInstance().getTime();
        return ((IFinalWizardPage) currentPage()).onFinish();
    }

    public void finishWizard(){
        finish();
    }

    //The time it took the user to complete the wizard
    public Date getWizardStartTime(){
        return mWizardStartTime;
    }

    public Date getWizardEndTime(){
        return mWizardEndTime;
    }

    public String getBookingReferenceNumber() {
        return mBookingReferenceNumber;
    }

//    public void onActivityResult(int requestCode, int resultCode, Intent data) {
//        super.onActivityResult(requestCode, resultCode, data);
//
//        if (resultCode == Activity.RESULT_OK) {
//            if (requestCode == Constants.WIZARD_CANCELLATION_REQUEST_CODE) {
//                ((WizardPage)currentPage()).beforeCancel();
//                finish();
//            }
//        }
//    }

    public List<VehicleInspectionResultModel> getVehicleInspectionResultList() {

        if (mVehicleInspectionResultList == null){
            mVehicleInspectionResultList = new ArrayList<>();
        }

        return mVehicleInspectionResultList;
    }

    public List<VehicleInspectionQuestionModel> getVehicleInspectionList() {
        return mVehicleInspectionQuery.getVehicleInspectionQuestionList();
    }

    public VehicleInspectionQueryModel getVehicleInspectionQuery() {
        return mVehicleInspectionQuery;
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

    private <T> Fragment setPageIndex(Fragment fragment, int index, T additionalInformation) {

        Bundle bundle = new Bundle();
        bundle.putInt(Constants.INDEX_TEXT, index);

        bundle.putParcelable(Constants.ADDITIONAL_WIZARD_INFORMATION, (Parcelable) additionalInformation);

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

        if (mLoadPagesDynamically == true) {
            if (mWizardPages.size() > 1) {
                nextButton().setText(mLastPageIndex == currentPageIndex() ?
                        getResources().getString(R.string.finish) :
                        getResources().getString(R.string.next));
            }
        }else{
            nextButton().setText(mLastPageIndex == currentPageIndex() ?
                    getResources().getString(R.string.finish) :
                    getResources().getString(R.string.next));
        }
    }

//    private void startCancellationReasonActivity() {
//        try {
//            Intent intent = new Intent(this, (Class<?>) mCancellationActivityType);
//            startActivityForResult(intent, Constants.WIZARD_CANCELLATION_REQUEST_CODE);
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
//        }
//    }

    public boolean lastPageReached(){
        return mLastWizardPageReached;
    }

    public void lastPage(){
        loadWizardPage(mWizardPages.size()-1);
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        super.onSaveInstanceState(savedInstanceState);
    }

    @Override
    public void onRestoreInstanceState(Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }

    private boolean isWizardFinished() {
        return (currentPageIndex() == mLastPageIndex) && (mLastPageIndex != -1);
    }

    private Button nextButton() {
        return (Button) findViewById(R.id.nextButton);
    }

    private Button backButton() {
        return (Button) findViewById(R.id.backButton);
    }
}
