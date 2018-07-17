package za.co.kapsch.ipayment;

import android.app.Activity;
import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.MenuItem;
import android.view.View;
import android.widget.EditText;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.ipayment.General.Constants;
import za.co.kapsch.ipayment.General.DataServiceRequest;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Utilities;

public class SearchFinesActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    public static final int FAILED = 1;
    public static final int SUCCESS = 2;
    private List<FineModel> mPersonFineList;
    private List<FineModel> mVehicleFineList;
    private EditText mSearchValueEditText;
    private SearchFinesCriteriaType mSearchFinesCriteriaType;

    private static final Object mlockPersonFineList = new Object();
    private static final Object mlockVehicleFineList = new Object();
    private List<SearchFinesCriteriaType> mSearchFinesCriteriaTypeList;
    private static final Object mlockSearchFinesCriteriaTypeList = new Object();

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_search_fines);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.search_fines)));


        mSearchFinesCriteriaTypeList = new ArrayList<>();

        mSearchValueEditText = (EditText)findViewById(R.id.searchValueEditText);

        //String refNumber = getIntent().getStringExtra(za.co.kapsch.shared.Constants.REF_NUMBER);
        String licenceNumber = getIntent().getStringExtra(za.co.kapsch.shared.Constants.VLN);
        String idNumber = getIntent().getStringExtra(za.co.kapsch.shared.Constants.ID_NUMBER);

        //fineNumberQuery(refNumber);
        vehicleQuery(licenceNumber);
        idNumberQuery(idNumber);
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

    public void scanBarcodeClick(View view) {
        za.co.kapsch.shared.Utilities.startBarcodeScanActivity(this, za.co.kapsch.shared.Constants.SCAN_REQUEST_CODE);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == za.co.kapsch.shared.Constants.SCAN_REQUEST_CODE) {
            if(resultCode == Activity.RESULT_OK) {
                String value = Utilities.byteArrayToString(data.getByteArrayExtra(za.co.kapsch.shared.Constants.BARCODE_SCAN_RESULT));
                mSearchValueEditText.setText(value);
            }
        }
    }

//    private void queryScanResult(String scanResult){

//        if (scanResult.length() == 16){
//            fineNumberQuery(scanResult);
//            return;
//        }
//
//        if (scanResult.length() == 36){
//            transactionTokenQuery(scanResult);
//            return;
//        }
//
//        if (scanResult.length() == 13){
//            idNumberQuery(scanResult);
//            return;
//        }
//
//        if (scanResult.length() == 10){
//            vehicleQuery(scanResult);
//            return;
//        }

//        MessageManager.showMessage(getResources().getString(R.string.invalid_value), ErrorSeverity.None);
//    }

    public void fineNumberQueryClick(View view){
        fineNumberQuery(mSearchValueEditText.getText().toString());
    }

    public void idNumberQueryClick(View view){
        idNumberQuery(mSearchValueEditText.getText().toString());
    }

    public void vehicleQueryClick(View view){
        vehicleQuery(mSearchValueEditText.getText().toString());
    }

    public void transactionTokenQueryClick(View view){
        vehicleQuery(mSearchValueEditText.getText().toString());
    }

    private  void fineNumberQuery(String refNumber){
        if (refNumber == null) return;
        clearLists();
        mSearchValueEditText.setText(refNumber);
        mSearchFinesCriteriaType = SearchFinesCriteriaType.RefNumber;
        mSearchFinesCriteriaTypeList.add(SearchFinesCriteriaType.RefNumber);
        DataServiceRequest.finesRequest(this, this, mSearchFinesCriteriaType, refNumber);
    }

    public void vehicleQuery(String licenceNumber){
        if (TextUtils.isEmpty(licenceNumber) == true) return;
        clearLists();
        mSearchValueEditText.setText(licenceNumber);
        mSearchFinesCriteriaType = SearchFinesCriteriaType.VLN;
        mSearchFinesCriteriaTypeList.add(SearchFinesCriteriaType.VLN);
        DataServiceRequest.finesRequest(this, this, mSearchFinesCriteriaType, licenceNumber);
    }

    public void idNumberQuery(String idNumber){
        if (TextUtils.isEmpty(idNumber) == true) return;
        clearLists();
        mSearchValueEditText.setText(idNumber);
        mSearchFinesCriteriaType = SearchFinesCriteriaType.ID;
        mSearchFinesCriteriaTypeList.add(SearchFinesCriteriaType.ID);
        DataServiceRequest.finesRequest(this, this, mSearchFinesCriteriaType,  idNumber);
    }

    public void transactionTokenQuery(String transactionToken){
        if (transactionToken == null) return;
        clearLists();
        mSearchValueEditText.setText(transactionToken);
        mSearchFinesCriteriaType = SearchFinesCriteriaType.TransactionToken;
        mSearchFinesCriteriaTypeList.add(SearchFinesCriteriaType.TransactionToken);
        DataServiceRequest.finesRequest(this, this, mSearchFinesCriteriaType,  transactionToken);
    }

    private void clearLists(){

        if (mPersonFineList != null) {
            mPersonFineList.clear();
        }

        if (mVehicleFineList != null) {
            mVehicleFineList.clear();
        }
    }

    private void startFineResultsActivity(){

        synchronized (mlockSearchFinesCriteriaTypeList) {
            if (mSearchFinesCriteriaTypeList.size() > 0) return;
        }

        normaliseFineLists();

        if (isListNullOrEmpty(mPersonFineList) && isListNullOrEmpty(mVehicleFineList)){
            MessageManager.showMessage(getResources().getString(R.string.no_fines_found), ErrorSeverity.None);
            return;
        }

        try{
            Intent intent = new Intent(this, FineResultsActivity.class);
            intent.putParcelableArrayListExtra(Constants.ID_NUMBER_FINE_LIST, (ArrayList<FineModel>)mPersonFineList);
            intent.putParcelableArrayListExtra(Constants.VEHICLE_FINE_LIST, (ArrayList<FineModel>)mVehicleFineList);
            startActivity(intent);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "FineResultsActivity::startSearchFinesActivity()"), ErrorSeverity.High);
        }
    }

    private void normaliseFineLists() {

        synchronized (mlockVehicleFineList){
            synchronized (mlockPersonFineList){
                if (isListNullOrEmpty(mPersonFineList) == true ||
                        isListNullOrEmpty(mVehicleFineList) == true) {
                    return;
                }

                List<FineModel> removeList = new ArrayList<>();

                for (FineModel idNumberFine : mPersonFineList) {
                    for (FineModel vehicleFine : mVehicleFineList) {
                        if (idNumberFine.getOffenderIDNumber().equals(vehicleFine.getOffenderIDNumber())) {
                            //mVehicleFineList.remove(vehicleFine);
                            removeList.add(vehicleFine);
                        }
                    }
                }

                mVehicleFineList.removeAll(removeList);
            }
        }
    }

    private boolean isListNullOrEmpty(List<FineModel> list){

        if(list == null) return true;

        if (list.size() == 0) return true;

        return false;
    }

    @Override
    public void onStart(){
        super.onStart();
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {

            if (asyncResultModel == null){
                MessageManager.showMessage("asyncResultModel == null", ErrorSeverity.High);
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_ID:
                    personFineListQueryResult(asyncResultModel, SearchFinesCriteriaType.ID);
                    break;

                case Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_REF_NUMBER:
                    personFineListQueryResult(asyncResultModel, SearchFinesCriteriaType.RefNumber);
                    break;

                case Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_TOKEN:
                    personFineListQueryResult(asyncResultModel, SearchFinesCriteriaType.TransactionToken);
                    break;

                case Constants.PROCESS_ID_QUERY_OUTSTANDING_FINES_BY_VLN:
                    vehicleFineListQueryResult(asyncResultModel, SearchFinesCriteriaType.VLN);
                    break;
            }

            startFineResultsActivity();

        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "finishedCallBack() - PROCESS_ID: %d"), ErrorSeverity.High);
            return;
        }
    }

    private void personFineListQueryResult(AsyncResultModel asyncResultModel, SearchFinesCriteriaType searchFinesCriteriaType){

        switch (asyncResultModel.getProcessResult()) {
            case SUCCESS:
                addToPersonFineList((List<FineModel>) asyncResultModel.getObject(), searchFinesCriteriaType);
                break;
            case FAILED:
                removeFromSearchFinesCriteriaTypeList(searchFinesCriteriaType);
                MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                break;
        }
    }

    private void vehicleFineListQueryResult(AsyncResultModel asyncResultModel, SearchFinesCriteriaType searchFinesCriteriaType){

        switch (asyncResultModel.getProcessResult()) {
            case SUCCESS:
                addToVehicleFineList((List<FineModel>) asyncResultModel.getObject(), searchFinesCriteriaType);
                break;
            case FAILED:
                removeFromSearchFinesCriteriaTypeList(searchFinesCriteriaType);
                MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                break;
        }
    }

    private void addToPersonFineList(List<FineModel> fineList, SearchFinesCriteriaType mSearchFinesCriteriaType){

        if (fineList != null){
            synchronized (mlockPersonFineList){
                for(FineModel fine: fineList){
                    if (mPersonFineList == null){
                        mPersonFineList = new ArrayList<>();
                    }
                    mPersonFineList.add(fine);
                }
            }
        }

        removeFromSearchFinesCriteriaTypeList(mSearchFinesCriteriaType);
    }

    private void addToVehicleFineList(List<FineModel> fineList, SearchFinesCriteriaType mSearchFinesCriteriaType){

        if (fineList != null){
            synchronized (mlockVehicleFineList){
                for(FineModel fine: fineList){
                    if (mVehicleFineList == null){
                        mVehicleFineList = new ArrayList<>();
                    }
                    mVehicleFineList.add(fine);
                }
            }
        }

        removeFromSearchFinesCriteriaTypeList(mSearchFinesCriteriaType);
    }

    private void removeFromSearchFinesCriteriaTypeList(SearchFinesCriteriaType mSearchFinesCriteriaType){

        synchronized (mlockSearchFinesCriteriaTypeList){
            int index  = mSearchFinesCriteriaTypeList.indexOf(mSearchFinesCriteriaType);
            if (index != -1) {
                mSearchFinesCriteriaTypeList.remove(index);
            }
        }
    }
}

