package za.co.kapsch.ivehicletest;


import android.content.DialogInterface;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.Date;
import java.util.List;

import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.General.DataSynchronisation;
import za.co.kapsch.ivehicletest.Interfaces.IFinalWizardPage;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultsModel;
import za.co.kapsch.ivehicletest.Printer.InspectionResultsSlip;
import za.co.kapsch.ivehicletest.orm.EvidenceRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultRepository;
import za.co.kapsch.ivehicletest.orm.VehicleInspectionResultsRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;


/**
 * A simple {@link Fragment} subclass.
 */
public class VehicleInspectionReviewFragment extends WizardPage implements DialogInterface.OnClickListener, IFinalWizardPage, IMessageCallBack, IAsyncProcessCallBack {

    private static int INSPECTION_FAILURE_WEIGHT = 100;
    private ListView mListView;
    private TextView mFinalResultTextView;
    private DataSynchronisation mDataSynchronisation;
    private VehicleInspectionResultsModel mVehicleInspectionResults;
    private List<VehicleInspectionResultModel> mVehicleInspectionResultList;

    AdapterView.OnItemClickListener mOnItemClickListener  = new AdapterView.OnItemClickListener()
    {
        @Override
        public void onItemClick(AdapterView<?> parent, final View view, int position, long id)
        {
            try {
                view.setSelected(true);

                if (wizardActivity().wizardLocked() == false) {
                    wizardActivity().goToPage(position - 1);
                }

            }catch (Exception e){
                String error = e.getMessage();
            }
        }
    };

    public VehicleInspectionReviewFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment

        View rootView =  inflater.inflate(R.layout.fragment_vehicle_inspection_review, container, false);

        mListView = (ListView)rootView.findViewById(R.id.listView);
        mListView.setOnItemClickListener(mOnItemClickListener);

        mFinalResultTextView = (TextView) rootView.findViewById(R.id.finalResultTextView);

        mVehicleInspectionResultList = wizardActivity().getVehicleInspectionResultList();
        setupScreen(mVehicleInspectionResultList);

        return rootView;
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    public void setupScreen(List<VehicleInspectionResultModel> vehicleInspectionResultList){

        if (vehicleInspectionResultList.size() < 1) return;

        VehicleInspectionListAdapter adapter = new VehicleInspectionListAdapter(this.getActivity(), vehicleInspectionResultList);
        mListView.setAdapter(adapter);

        boolean isPassed = calculateIsPassedFromQuestionWeight(vehicleInspectionResultList);
        mFinalResultTextView.setText(
                String.format("%s : %s",
                        getResources().getString(R.string.final_result),
                        isPassed ?  getResources().getString(R.string.pass) : getResources().getString(R.string.fail)));
    }

    private VehicleInspectionResultsModel getVehicleInspectionResults(long bookingID, Date testStartTime, Date testEndTime, List<VehicleInspectionResultModel> vehicleInspectionResultList){

        if (mVehicleInspectionResults == null) {
            mVehicleInspectionResults = new VehicleInspectionResultsModel();
        }

        mVehicleInspectionResults.setCredentialID(SessionModel.getInstance().getUser().getCredentialID());
        mVehicleInspectionResults.setTestStartTime(testStartTime);
        mVehicleInspectionResults.setTestEndTime(testEndTime);
        mVehicleInspectionResults.setBookingID(bookingID);
        mVehicleInspectionResults.setIsPassed(calculateIsPassedFromQuestionWeight(vehicleInspectionResultList));
        mVehicleInspectionResults.setVehicleInspectionResultList(vehicleInspectionResultList);

        return mVehicleInspectionResults;
    }

    public void message(String message, boolean append)
    {
        if (message.equals(Constants.FINISHED_MESSAGE) == false) {
            MessageManager.showMessage(message, ErrorSeverity.None);
        }
    }

    @Override
    public boolean onFinish() {

        if (mVehicleInspectionResultList != null){
            if (saveVehicleInspectionResult() == true) {
                printVehicleInspectionSlip();
            }
        }

        return true;
    }

    private boolean calculateIsPassedFromQuestionWeight(List<VehicleInspectionResultModel>  vehicleInspectionResultList){

        double result = 0;

        for(VehicleInspectionResultModel vehicleInspectionResult : vehicleInspectionResultList){
            if (vehicleInspectionResult.getIsPassed() == 0) {
                result = result + vehicleInspectionResult.getWeight();
            }
        }

        return result < INSPECTION_FAILURE_WEIGHT;
    }

    private boolean saveVehicleInspectionResult(){

        try {

            VehicleInspectionResultsRepository.createOrUpdate(
                    getVehicleInspectionResults(
                            mVehicleInspectionResultList.get(0).getBookingID(),
                            wizardActivity().getWizardStartTime(),
                            wizardActivity().getWizardEndTime(),
                            mVehicleInspectionResultList));

            EvidenceRepository.setEvidenceToSubmit(mVehicleInspectionResultList.get(0).getBookingID());

            wizardActivity().enableNextButton(false);
            return true;

        }catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "saveVehicleInspectionResult::saveVehicleInspectionResult() 1"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return false;
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::saveVehicleInspectionResult() 2"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return false;
        }
    }

    private boolean printVehicleInspectionSlip() {

        try {
            if (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false)
            {
                InspectionResultsSlip InspectionResultsSlip = new InspectionResultsSlip(SessionModel.getInstance().getPrinterMacAddress(), this.getContext(), this);
                InspectionResultsSlip.print(
                        false,
                        wizardActivity().getBookingReferenceNumber(),
                        wizardActivity().getVehicleInspectionQuery().getSiteName(),
                        wizardActivity().getVehicleInspectionQuery().getTestCategory() == null ? Constants.EMPTY_STRING : wizardActivity().getVehicleInspectionQuery().getTestCategory(),
                        wizardActivity().getVehicleInspectionQuery().getBarcodeData(),
                        mVehicleInspectionResults.getIsPassed() == true ? getResources().getString(R.string.pass) : getResources().getString(R.string.fail),
                        mVehicleInspectionResultList);
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::printVehicleInspectionSlip()"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return false;
        }

        return true;
    }

    private void processUpdates() {

        try {
            if (mDataSynchronisation == null) {
                mDataSynchronisation = new DataSynchronisation(this, true);
            }
            mDataSynchronisation.processUpdates(true);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::processUpdates()"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {

            if (asyncResultModel == null) {
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_ASYNC_PROCESS_PRINT:

                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            Utilities.displayDecisionMessage(getResources().getString(R.string.did_the_slip_print_successfully), this.getContext(), this);
                            break;
                        case FAILED:
                            break;
                    }

                    break;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
            return;
        }
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {

        try {
            if (which == Constants.YES) {
                processUpdates();
                wizardActivity().finish();
            }else{
                wizardActivity().enableNextButton(true);
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleInspectionReviewFragment::onClick()"), ErrorSeverity.Medium);
            wizardActivity().enableNextButton(true);
        }
    }
}
