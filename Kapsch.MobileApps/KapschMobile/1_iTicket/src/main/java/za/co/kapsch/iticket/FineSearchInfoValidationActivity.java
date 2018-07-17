package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.EditText;

import java.util.ArrayList;

import za.co.kapsch.shared.*;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.shared.Models.SessionModel;

public class FineSearchInfoValidationActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private EditText mIdEditText;
    private EditText mVlnEditText;

    private ArrayList<FineModel> mOutstandingPersonViolations;
    private ArrayList<FineModel> mOutstandingVehicleViolations;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_fine_search_info_validation);

        mIdEditText = (EditText)findViewById(R.id.idNumberEditText);
        mVlnEditText = (EditText)findViewById(R.id.vlnEditText);

        String vln = getIntent().getStringExtra(za.co.kapsch.shared.Constants.VLN);
        String idNumber = getIntent().getStringExtra(za.co.kapsch.shared.Constants.ID_NUMBER);

        mIdEditText.setText(idNumber);
        mVlnEditText.setText(vln);
    }

    public void proceedClick(View view){

//        Utilities.startPaymentApplication(
//            SessionModel.getInstance().getUser(),
//            SessionModel.getInstance().getDistrict(),
//            SessionModel.getInstance().getMobileDevice(),
//            SearchFinesCriteriaType.Unknown,
//            EndPointConfigModel.getInstance().getCoreGateway(),
//            EndPointConfigModel.getInstance().getITSGateway(),
//            EndPointConfigModel.getInstance().getEVRGateway(),
//            SessionModel.getInstance().getPrinterMacAddress(),
//            mVlnEditText.getText().toString(),
//            mIdEditText.getText().toString());
        outstandingViolationsSearch();
    }

    public void resetOutstandingViolations(){
        mOutstandingPersonViolations = null;
        mOutstandingVehicleViolations = null;
    }

    private void outstandingViolationsSearch(){

        try {

            if (TextUtils.isEmpty(mIdEditText.getText().toString()) && TextUtils.isEmpty(mVlnEditText.getText().toString())){
                return;
            }

            resetOutstandingViolations();
            DataServiceRequest.finesRequest(this, this, SearchFinesCriteriaType.ID, mIdEditText.getText().toString(), true);
            DataServiceRequest.finesRequest(this, this, SearchFinesCriteriaType.VLN, mVlnEditText.getText().toString(), true);

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "personInfoSearch()"), ErrorSeverity.Medium);
        }
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

    private void violationNotification(){

        if (mOutstandingPersonViolations != null && mOutstandingVehicleViolations != null) {
            if (mOutstandingPersonViolations.size() > 0 || mOutstandingVehicleViolations.size() > 0) {
                startOutstandingViolationsActivity(mOutstandingPersonViolations, mOutstandingVehicleViolations);
            }
        }
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
}
