package za.co.kapsch.iticket;


import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.TicketStatusType;
import za.co.kapsch.iticket.Enums.ViolationCategory;
import za.co.kapsch.iticket.Models.FineToTicketModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.shared.Models.FineEvidenceModel;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.Printer.HandWrittenSlip;
import za.co.kapsch.iticket.Printer.OutstandingViolationsSlip;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;


/**
 * A simple {@link Fragment} subclass.
 */
public class OutstandingViolationsFragment extends Fragment implements IAsyncProcessCallBack {

    private Button mPrintButton;
    private TextView mTicketsTextView;
    private ListView mTicketsListView;

    private FineModel mFine;
    private byte[] mOfficerSignature;
    private byte[] mOffenderSignature;
    private List<FineModel> mOutstandingViolations;
    private ViolationCategory mViolationCategory;

    AdapterView.OnItemClickListener mOnClickListener = new AdapterView.OnItemClickListener()
    {
        @Override
        public void onItemClick(AdapterView<?> parent, final View view, int position, long id)
        {
            try {
                view.setSelected(true);
                mFine = (FineModel) mTicketsListView.getItemAtPosition(position);
            }catch (Exception e){
                String error = e.getMessage();
                MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::mListView.setOnItemClickListener()"), ErrorSeverity.High);
            }
        }
    };

//    View.OnClickListener mPrintOnClickListener = new View.OnClickListener() {
//        @Override
//        public void onClick(View view) {
//            printOutstandingViolations();
//        }
//    };

    public OutstandingViolationsFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_outstanding_violations, container, false);
        setHasOptionsMenu(true);

        //mPrintButton = (Button)view.findViewById(R.id.printButton);
        mTicketsListView = (ListView)view.findViewById(R.id.ticketsListView);
        mTicketsTextView = (TextView)view.findViewById(R.id.ticketsTextView);
        mTicketsListView.setOnItemClickListener(mOnClickListener);

        //mPrintButton.setOnClickListener(mPrintOnClickListener);

        populateList();

        return view;
    }

    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {

        inflater.inflate(R.menu.oustanding_violations_menu, menu);
        super.onCreateOptionsMenu(menu, inflater);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {

        switch(item.getItemId()) {

            case R.id.printTicketItem:

                if (mFine.getTransactionToken() == null){
                    MessageManager.showMessage(getString(R.string.transaction_token_is_absent), ErrorSeverity.None);
                    return true;
                }

                TicketModel ticket = FineToTicketModel.getTicketModel(mFine, DocumentType.RoadSideDriver, null, null);

                if (evaluateTicketState(ticket) == TicketStatusType.Complete) {
                    printTicket(ticket);
                }else {
                    startWizardActivity(ticket, -1);
                }

                break;
            case R.id.printTicketListItem:
                printOutstandingViolations();
                break;
            default:
                return super.onOptionsItemSelected(item);
        }

        return true;
    }

    private void getTicketEvidence(){

        List<FineEvidenceModel> fineEvidenceModels = mFine.getFineEvidenceModels();

        if (fineEvidenceModels != null) {

            if (fineEvidenceModels.size() > 0) {
                if (fineEvidenceModels.get(0) != null) {
                    getEvidence(mFine.getFineEvidenceModels().get(0).getID());
                }
            }

            if (fineEvidenceModels.size() > 1) {
                if (fineEvidenceModels.get(1) != null) {
                    getEvidence(mFine.getFineEvidenceModels().get(1).getID());
                }
            }
        }
    }

    private void populateList(){

        try {
            mOutstandingViolations = null;
            mViolationCategory = ViolationCategory.fromInteger((int) getArguments().get(Constants.VEHICLE_CATEGORY));

            switch (mViolationCategory) {
                case Person:
                    mOutstandingViolations = ((OutstandingViolationsActivity) getActivity()).getOutstandingPersonViolationList();
                    break;
                case Vehicle:
                    mOutstandingViolations = ((OutstandingViolationsActivity) getActivity()).getOutstandingVehicleViolationList();
                    break;
            }

            mTicketsTextView.setText(String.format("%s: %s", getString(R.string.outstanding_violations), Integer.toString(mOutstandingViolations.size())));

            OutstandingViolationListAdapter adapter = new OutstandingViolationListAdapter(this.getActivity(), mOutstandingViolations);
            mTicketsListView.setAdapter(adapter);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e,""), ErrorSeverity.High);
        }
    }

    private void printOutstandingViolations() {

        try {
            if (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false) {
                OutstandingViolationsSlip outstandingViolationsSlip = new OutstandingViolationsSlip(SessionModel.getInstance().getPrinterMacAddress(), this.getActivity(), this);
                outstandingViolationsSlip.print(false, mOutstandingViolations, mViolationCategory);
            }else{
                MessageManager.showMessage(getResources().getString(R.string.printer_is_not_configured), ErrorSeverity.None);
                return;
            }
        }catch(Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "OutstandingViolationsFragment::printOutstandingViolations()"), ErrorSeverity.High);
            return;
        }
    }

    private void startWizardActivity(TicketModel ticket, int startPageIndex){

        Intent intent = new Intent(this.getActivity(), WizardActivity.class);
        intent.putExtra(Constants.TICKET_MODEL, ticket);
        intent.putExtra(Constants.WIZARD_START_PAGE_INDEX, startPageIndex);
        startActivityForResult(intent, Constants.TICKET_WIZARD_REQUEST_CODE);
    }

    public boolean getEvidence(long evidenceId){
        DataServiceRequest.getEvidenceRequest(this, this.getActivity(), evidenceId, Constants.PROCESS_ID_GET_OFFICER_SIGNATURE_EVIDENCE);
        return true;
    }

    private void printTicket(TicketModel ticket) {

        try {
            if (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false) {
                HandWrittenSlip handWrittenSlip = new HandWrittenSlip(SessionModel.getInstance().getPrinterMacAddress(), this.getActivity(), this);
                handWrittenSlip.print(false, ticket);
            }else{
                MessageManager.showMessage(getResources().getString(R.string.printer_is_not_configured), ErrorSeverity.None);
                return;
            }
        }catch(Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "printTicket()"), ErrorSeverity.High);
            return;
        }
    }

    private TicketStatusType evaluateTicketState(TicketModel ticket){

        try {

            OffenderModel offender = ticket.getOffender();

            if (offender == null){
                return TicketStatusType.InComplete;
            }

            if (TextUtils.isEmpty(offender.getIdNumber()) ||
                TextUtils.isEmpty(offender.getFirstName()) ||
                TextUtils.isEmpty(offender.getLastName())){

                return TicketStatusType.InComplete;
            }

            return TicketStatusType.Complete;

        }catch(Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "OutstandingViolationsFragment::evaluateTicketState()"), ErrorSeverity.High);
            return null;
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage( asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try{
            if (asyncResultModel == null){
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_ASYNC_PROCESS_PRINT:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            break;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                            break;
                    }
                    break;

//                case Constants.PROCESS_ID_GET_OFFICER_SIGNATURE_EVIDENCE:
//                    switch (asyncResultModel.getProcessResult()) {
//                        case SUCCESS:
//                            byte[] mOfficerSignature = (byte[])asyncResultModel.getObject();
//                            return;
//                        case FAILED:
//                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
//                            break;
//                    }
//                    break;
//
//                case Constants.PROCESS_ID_GET_OFFENDER_SIGNATURE_EVIDENCE:
//                    switch (asyncResultModel.getProcessResult()) {
//                        case SUCCESS:
//                            byte[] mOffenderSignature = (byte[])asyncResultModel.getObject();
//                            return;
//                        case FAILED:
//                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
//                            break;
//                    }
//                    break;

            }
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            return;
        }
    }
}
