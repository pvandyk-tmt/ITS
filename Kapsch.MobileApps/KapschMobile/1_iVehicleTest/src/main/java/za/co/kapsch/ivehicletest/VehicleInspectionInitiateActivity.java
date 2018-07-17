package za.co.kapsch.ivehicletest;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Parcelable;
import android.provider.MediaStore;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.EditText;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;

import java.io.IOException;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.General.DataServiceRequest;
import za.co.kapsch.ivehicletest.Models.EvidenceModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionAnswerModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQueryModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQuestionModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.Printer.InspectionResultsSlip;
import za.co.kapsch.ivehicletest.orm.EvidenceRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.shared.LibApp.getContext;
import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

public class VehicleInspectionInitiateActivity extends AppCompatActivity implements IAsyncProcessCallBack{

    private EditText mBookingReferenceEditText;
    private VehicleInspectionQueryModel mVehicleInspectionQuery;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_vehicle_inspection_initiate);

        mBookingReferenceEditText = (EditText) findViewById(R.id.bookingReferenceEditText);

        setTitle(getResources().getString(R.string.vehicle_inspection));
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {

    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try{
            if (asyncResultModel == null){
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_GET_VEHICLE_INSPECTION:

                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mVehicleInspectionQuery = (VehicleInspectionQueryModel)asyncResultModel.getObject();

                            if(mVehicleInspectionQuery.getVehicleInspectionQuestionList().size() == 0){
                                MessageManager.showMessage(getResources().getString(R.string.question_list_is_empty), ErrorSeverity.High);
                                return;
                            }
                            startWizard(mVehicleInspectionQuery);
                            return;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
                            mBookingReferenceEditText.setEnabled(true);
                            break;
                    }
                    break;
            }
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            return;
        }finally {
            mBookingReferenceEditText.setEnabled(true);
        }
    }

    private void startWizard(VehicleInspectionQueryModel vehicleInspectionQuery){

        if (vehicleInspectionQuery.isPhotoRequired() == true){
            vehicleInspectionQuery.getVehicleInspectionQuestionList().add(
                    0,
                    getVehicleInspectionQuestion(
                            vehicleInspectionQuery.getVehicleInspectionQuestionList().get(0).getTestQuestionID()));
        }

        startWizardActivity(-1, vehicleInspectionQuery, mBookingReferenceEditText.getText().toString());
    }


    private void printTestSlip(){

        try {

            VehicleInspectionResultsModel vehicleInspectionResults = new VehicleInspectionResultsModel();//gson.fromJson(VehicleResultJson, new TypeToken<VehicleInspectionResultsModel>(){}.getType());

            List<VehicleInspectionResultModel> vehicleInspectionResultList = new ArrayList<>();

            vehicleInspectionResultList.add(getVehicleInspectionResult("ENG", "VIN111", "Pass", 1, "", "VIN111", "pass"));
            vehicleInspectionResultList.add(getVehicleInspectionResult("Number Plate", "CY111", "Pass", 1, "", "CY111", "pass"));
            vehicleInspectionResultList.add(getVehicleInspectionResult("VIN/CHASSIS", "VIN111", "Change", 1, "", "VIN111", "Change"));
            vehicleInspectionResultList.add(getVehicleInspectionResult("Colour", "RED", "Pass", 1, "", "RED", "Pass"));
            vehicleInspectionResultList.add(getVehicleInspectionResult("Brake", "", "Pass", 1, "", "", "Pass"));
            vehicleInspectionResultList.add(getVehicleInspectionResult("Passengers", "25", "Pass", 1, "Many more passengers many many more", "5", "Pass"));


             if (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false)
            {
                InspectionResultsSlip InspectionResultsSlip = new InspectionResultsSlip(SessionModel.getInstance().getPrinterMacAddress(), this, this);
                InspectionResultsSlip.print(
                        false,
                        "CE111",
                        "Durbanville Home",
                        "",
                        "CY111",
                        "Pass",
                        vehicleInspectionResultList);
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::printVehicleInspectionSlip()"), ErrorSeverity.High);
        }
    }

    public VehicleInspectionResultModel getVehicleInspectionResult(String question, String answer, String multiChoiceAnswer, int isPassed, String comments, String compareValue, String passFailedText){

        VehicleInspectionResultModel vehicleInspectionResult = new VehicleInspectionResultModel();

        vehicleInspectionResult.setQuestion(question);
        vehicleInspectionResult.setMultipleChoiceAnswer(multiChoiceAnswer);
        vehicleInspectionResult.setIsPassed(isPassed);
        vehicleInspectionResult.setAnswer(answer);
        vehicleInspectionResult.setComments(comments);
        vehicleInspectionResult.setCompareValue(compareValue);
        vehicleInspectionResult.setPassFailedText(passFailedText);

        return vehicleInspectionResult;
     }

    public void queryButtonClick(View view){

//        printTestSlip();

        mBookingReferenceEditText.setEnabled(false);

        if (TextUtils.isEmpty(mBookingReferenceEditText.getText().toString()) == true){
            MessageManager.showMessage(getResources().getString(R.string.please_enter_a_booking_reference), ErrorSeverity.None);
            mBookingReferenceEditText.setEnabled(true);
            return;
        }

        DataServiceRequest.vehicleInspectionRequest(this, this, mBookingReferenceEditText.getText().toString());
        Utilities.hideKeyboard(this);
    }

    private void startWizardActivity(int startPageIndex, VehicleInspectionQueryModel vehicleInspectionQuery /*, List<VehicleInspectionQuestionModel> vehicleInspectionQuestionList*/, String bookingReferenceNumber){
        try {
            Intent intent = new Intent(this, WizardActivity.class);
            intent.putExtra(Constants.LOAD_WIZARDPAGE_DYNAMICALLY, true);
            intent.putExtra(Constants.BOOKING_REFERENCE_NUMBER, bookingReferenceNumber);
            //intent.putParcelableArrayListExtra(Constants.VEHICLE_INSPECTION_QUESTION_LIST, (ArrayList<? extends Parcelable>) vehicleInspectionQuestionList);
            intent.putExtra(Constants.VEHICLE_INSPECTION_QUERY, vehicleInspectionQuery);
            intent.putExtra(Constants.WIZARD_START_PAGE_INDEX, startPageIndex);
            Utilities.logUserActivity("Vehicle Inspection","Initiated-Vehicle Inspection");
            startActivityForResult(intent, 1);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    private VehicleInspectionQuestionModel getVehicleInspectionQuestion(long nextQuestionID){

        VehicleInspectionQuestionModel vehicleInspectionQuestion = new VehicleInspectionQuestionModel();
        vehicleInspectionQuestion.setTestQuestionDescription(getResources().getString(R.string.capture_vehicle_photos));
        vehicleInspectionQuestion.setBookingID(mVehicleInspectionQuery.getBookingID());

        List<VehicleInspectionAnswerModel> vehicleInspectionAnswerList = new ArrayList<>();
        vehicleInspectionAnswerList.add(getVehicleInspectionPhotoAnswer(nextQuestionID));

        vehicleInspectionQuestion.setVehicleInspectionAnswerList(vehicleInspectionAnswerList);
        vehicleInspectionQuestion.setPhotoRequired(true);

        return vehicleInspectionQuestion;
    }

    private VehicleInspectionAnswerModel getVehicleInspectionPhotoAnswer(long nextQuestionID){

        VehicleInspectionAnswerModel vehicleInspectionAnswer = new VehicleInspectionAnswerModel();
        vehicleInspectionAnswer.setNextQuestionID(nextQuestionID);
        vehicleInspectionAnswer.setTestQuestionAnswerID((long)0);

        return vehicleInspectionAnswer;
    }

    @Override
    public void onStart(){
        super.onStart();
        mBookingReferenceEditText.setText(Constants.EMPTY_STRING);
    }
}
