package za.co.kapsch.console;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;

import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.orm.DistrictRepository;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.WebAccess.DataService;
import za.co.kapsch.console.General.DataServiceRequest;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.PaginationListModel;


public class DistrictListActivity extends AppCompatActivity { //implements IAsyncProcessCallBack {

    private ListView mDistrictListView;
    private DistrictModel mSelectedDistrict;
    //private DataService mDataService;
    private Button mOkButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_district_list);
        mDistrictListView = (ListView) findViewById(R.id.districtListView);
        mOkButton = (Button) findViewById(R.id.okButton);
       // mDataService = new DataService(this);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.districts)));

        mDistrictListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                view.setSelected(true);
                mSelectedDistrict = (DistrictModel) mDistrictListView.getItemAtPosition(position);
            }
        });

        populateDistrictList();
    }

    private void populateDistrictList() {
        //DataServiceRequest.districtsRequest(this, this);
        try {
            List<DistrictModel> districts = SessionModel.getInstance().getUser().getDistricts();

            if (districts == null) {
                MessageManager.showMessage(getString(R.string.district_list_is_empty), ErrorSeverity.High);
                finish();
            }

            PopulateDistrictListView(districts);
            //PopulateDistrictListView(DistrictRepository.getDistricts());
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "populateDistrictList()"), ErrorSeverity.High);
        }
    }

    private void PopulateDistrictListView(List<DistrictModel> districtList){

        if (districtList == null) return;

        if (districtList.size() < 1) return;

        mOkButton.setText(this.getResources().getString(R.string.ok));
        ArrayAdapter arrayAdapter = new ArrayAdapter<>(this, R.layout.list_item, districtList);
        mDistrictListView.setAdapter(arrayAdapter);
    }

    public void okButtonClick(View view){

        try {
            if (mOkButton.getText().equals(this.getResources().getString(R.string.search))) {
                populateDistrictList();
                return;
            }

            returnDistrict();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DistrictListActivity():okButtonClick()"), ErrorSeverity.High);
        }
    }

    private void returnDistrict(){

        if (mSelectedDistrict == null){
            MessageManager.showMessage(getResources().getString(R.string.select_a_district_from_the_list), ErrorSeverity.None);
            return;
        }

        if (mSelectedDistrict != null) {
            Intent intent = new Intent();
            intent.putExtra(Constants.DISTRICT_RESULT, mSelectedDistrict);
            setResult(RESULT_OK, intent);
            finish();
        }
    }

    private void returnDistrictFailed(){
        Intent intent = new Intent();
        setResult(Constants.ACTIVTIY_FAILED_WITH_INVALID_LOGIN_CREDENTIALS, intent);
        finish();
    }

//    @Override
//    public void progressCallBack(AsyncResultModel asyncResultModel) {
//        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
//    }
//
//    @Override
//    public void finishedCallBack(AsyncResultModel asyncResultModel) {
//
//        mOkButton.setText(this.getResources().getString(R.string.search));
//
//        if (asyncResultModel == null){
//            return;
//        }
//
//        if (asyncResultModel.getProcessResult() == DataService.FAILED){
//            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.Medium);
//            returnDistrictFailed();
//            return;
//        }
//
//        if (asyncResultModel.getObject() == null){
//            return;
//        }
//
//        PopulateDistrictListView(((PaginationListModel)asyncResultModel.getObject()).getModels());
//    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}