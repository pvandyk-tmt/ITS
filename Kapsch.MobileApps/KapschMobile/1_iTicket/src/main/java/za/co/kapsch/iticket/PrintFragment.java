package za.co.kapsch.iticket;


import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CheckBox;
import android.widget.ImageButton;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Interfaces.IFinalWizardPage;
import za.co.kapsch.iticket.Printer.HandWrittenSlip;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Interfaces.IMessageCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;


/**
 * A simple {@link Fragment} subclass.
 */
public class PrintFragment extends Fragment implements IFinalWizardPage, DialogInterface.OnClickListener, IAsyncProcessCallBack, IMessageCallBack {

    private int mHandWrittenId;
    private HandWrittenModel mHandWritten;

    private EvidenceModel mCurrentEvidence;

    private ImageButton mPaymentButton;
    private ImageButton mPrintButton;
    private CheckBox mIssueAdditionalNoticeCheckBox;

    private DialogInterface.OnClickListener mAlertDialogSlipOnClickListener = new DialogInterface.OnClickListener() {
        @Override
        public void onClick(DialogInterface dialog, int which) {
            try {
                if (which == Constants.YES) {
                    boolean rowsUpdated = false;
                    TicketModel ticket = wizardActivity().getTicketModel();
                    if (ticket.getDocumentType() == DocumentType.RoadSideDriver) {
                        if (mHandWritten != null) {
                            mHandWritten.setID(mHandWrittenId);
                            mHandWritten.setPrinted(true);
                            rowsUpdated = updateHandWrittenModel(mHandWritten);
                        } else {
                            wizardActivity().enableNextButton(true);
                            MessageManager.showMessage("PrintFragment::onClick() - mHandWritten is null", ErrorSeverity.Medium);
                        }
                    }
                    if (rowsUpdated == false) {
                        Utilities.displayOkMessage("Database update failed, please reprint.", getActivity());
                    }

                    ticket.getInfringement().setIssueDateLocked(true);
                    uploadTicket();

                } else {
                    mIssueAdditionalNoticeCheckBox.setVisibility(View.VISIBLE);
                    wizardActivity().enableNextButton(true);
                }
            }catch (Exception e){
                MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::onClick()"), ErrorSeverity.Medium);
                wizardActivity().enableNextButton(true);
            }
        }
    };


    public PrintFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        View rootView = inflater.inflate(R.layout.fragment_print, container, false);

        mPaymentButton = (ImageButton) rootView.findViewById(R.id.paymentButton);
        mPaymentButton.setVisibility(View.INVISIBLE);
        mPaymentButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                paymentButtonClick();
            }
        });

        mPrintButton = (ImageButton) rootView.findViewById(R.id.printNoticeButton);
        mPrintButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                printButtonClick();
            }
        });

        mIssueAdditionalNoticeCheckBox = (CheckBox) rootView.findViewById(R.id.issueAdditionalNoticeCheckBox);
        mIssueAdditionalNoticeCheckBox.setVisibility(View.INVISIBLE);

        mPrintButton.setOnLongClickListener(new View.OnLongClickListener() {
            @Override
            public boolean onLongClick(View view) {
                findPrintersActivity();
                return true;
            }
        });

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.fragment_print_section)));

        wizardActivity().enableNextButton(false);

        return rootView;
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    public void onFinish(){
        wizardActivity().setIssueAdditionalNotice(mIssueAdditionalNoticeCheckBox.isChecked());
    }

    private void printButtonClick(){

        TicketModel ticket = wizardActivity().getTicketModel();

        if (ticket.isLocallyGeneratedTicket() == true) {
            ticket.getInfringement().setIssueDate(new Date());
            ticket.getInfringement().setPayDate(Utilities.addDaysToDate(ticket.getCourtInfo().getCourtDate().getDate(), ConfigItemModel.getInstance().getPayDateFromCourtDate()));
        }else if (ticket.getInfringement().getIssueDate() == null){
            ticket.getInfringement().setIssueDate(new Date());
        }

        if (saveHandWritten(ticket) == true) {
            printTicket(ticket);
        }
    }

    private boolean saveHandWritten(TicketModel ticket){

        mHandWritten = new HandWrittenModel();

        String error = mHandWritten.setHandWritten(ticket, Utilities.getDeviceId(), false, null);
        if (error != null){
            MessageManager.showMessage(String.format("set ticket failed: %s", error), ErrorSeverity.High);
            return false;
        }

        if (ticket.getPersisted() == false){
            mHandWrittenId = saveHandWritten(mHandWritten);
            if (mHandWrittenId == -1){
                MessageManager.showMessage("Failed to save ticket.", ErrorSeverity.High);
                return false;
            }

            mHandWritten.setID(mHandWrittenId);
            ticket.setPersisted(true);
        }

        return true;
    }

//    private void printTicket(TicketModel ticket){
//        try{
//            String[] details = Utilities.getPrinterDetails();
//            if (details.length != 2){
//                Utilities.displayOkMessage("Printer details not found, Please register a printer.", getActivity());
//                return;
//            }
//
//            HandWrittenSlip handWrittenSlip = new HandWrittenSlip(details[1], this.getActivity(), this);
//            handWrittenSlip.print(false, ticket);
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "printTicket()"), ErrorSeverity.High);
//            return;
//        }
//    }

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

    private void uploadTicket(){
        try {
            DataSynchronisation dataSynchronisation = new DataSynchronisation(this, false);
            dataSynchronisation.processUpdates();
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::uploadTicket():"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
        }
    }

    private int saveHandWritten(HandWrittenModel handWritten){
        try {

            //Start HandWrittenModel Sanity Checks: (anomolies were notices in the database ie duplicate OffenceDate and null IssueDate)
            if (EndPointConfigModel.getInstance().getITSGateway().equals("192.168.0.33:60002")) {

                if (handWritten.getIssueDate() == null){
                    MessageManager.showMessage("ISSUE DATE ALREADY EXIST", ErrorSeverity.None);
                    Utilities.displayOkMessage("ISSUE DATE ALREADY EXIST", this.getActivity());
                }

                if (handWritten.getOffenceDate() == null){
                    MessageManager.showMessage("ISSUE DATE ALREADY EXIST", ErrorSeverity.None);
                    Utilities.displayOkMessage("ISSUE DATE ALREADY EXIST", this.getActivity());
                }


                if (HandWrittenRepository.offenceDateExists(handWritten.getOffenceDate()) == true) {
                    MessageManager.showMessage("OFFENCE DATE ALREADY EXIST", ErrorSeverity.None);
                    Utilities.displayOkMessage("OFFENCE DATE ALREADY EXIST", this.getActivity());
                }

            }
            //End HandWrittenModel Sanity Checks

            int id = HandWrittenRepository.create(handWritten);
            if (id != -1){
                wizardActivity().setWizardLocked(true);

                List<EvidenceModel> evidenceList = wizardActivity().getTicketModel().getEvidenceList();
                if (evidenceList == null){
                    evidenceList = new ArrayList<>();
                }
                moveEvidenceToEvidenceList(evidenceList);

                if (evidenceList != null) {
                    for (EvidenceModel evidence : evidenceList) {
                        EvidenceRepository.create(evidence);
                    }
                }
            }
            wizardActivity().enableBackButton(false);
            return id;
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::saveHandWritten():"), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
        } catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::saveHandWritten(): "), ErrorSeverity.High);
            wizardActivity().enableNextButton(true);
        }

        return -1;
    }

    private void moveEvidenceToEvidenceList(List<EvidenceModel> evidenceList){

        if (wizardActivity().getTicketModel().getOffender() != null) {

            byte[] evidence = wizardActivity().getTicketModel().getOffender().getSignature();
            if (evidence != null) {
                evidenceList.add(EvidenceModel.getEvidence(
                        EvidenceType.PersonSignature,
                        evidence,
                        wizardActivity().getTicketModel().getInfringement().getTicketNumber()));
            }

            evidence = wizardActivity().getTicketModel().getUser().getSignature();
            if (evidence != null) {
                evidenceList.add(EvidenceModel.getEvidence(
                        EvidenceType.OfficerSignature,
                        evidence,
                        wizardActivity().getTicketModel().getInfringement().getTicketNumber()));
            }

            Bitmap driverPhoto = wizardActivity().getTicketModel().getOffender().getPhoto();
            if (driverPhoto != null) {
                evidence = Utilities.bitmapToJPGBytes(driverPhoto);
                if (evidence != null) {
                    evidenceList.add(EvidenceModel.getEvidence(
                            EvidenceType.OffenderPhoto,
                            evidence,
                            wizardActivity().getTicketModel().getInfringement().getTicketNumber()));
                }
            }
        }
    }

    private boolean updateHandWrittenModel(HandWrittenModel handWritten){
        try {
            return HandWrittenRepository.update(handWritten);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::updateHandWrittenModel 1"), ErrorSeverity.High);
        } catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::updateHandWrittenModel 2"), ErrorSeverity.High);
        }

        return false;
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

                    Utilities.displayDecisionMessage("Did the slip print successfully?", this.getContext(), mAlertDialogSlipOnClickListener);
                    break;

                }
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            return;
        }
    }

    public void message(String message, boolean appendText){

        if (message.equals(Constants.FINISHED_MESSAGE)||message.equals(Constants.FAILED_MESSAGE)) {
            wizardActivity().enableNextButton(true);
        }

//        if (Utilities.validateUserAccess(SessionModel.getInstance().getUser(), "za.co.kapsch.ipayment") == true) {
//            if (message.equals(Constants.FINISHED_MESSAGE)) {
//                mPaymentButton.setVisibility(View.VISIBLE);
//            }
//        }

        if (message.equals(Constants.FINISHED_MESSAGE)||message.equals(Constants.FAILED_MESSAGE)) {
            wizardActivity().enableNextButton(true);
            mIssueAdditionalNoticeCheckBox.setVisibility(View.VISIBLE);
        }

        MessageManager.showMessage(message, ErrorSeverity.High);
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {

        MessageManager.showMessage("Print error please investigate", ErrorSeverity.None);
//        try {
//            if (which == Constants.YES) {
//                boolean rowsUpdated = false;
//                TicketModel ticket = wizardActivity().getTicketModel();
//                if (ticket.getDocumentType() == DocumentType.RoadSideDriver) {
//                    if (mHandWritten != null) {
//                        mHandWritten.setID(mHandWrittenId);
//                        mHandWritten.setPrinted(true);
//                        rowsUpdated = updateHandWrittenModel(mHandWritten);
//                    } else {
//                        wizardActivity().enableNextButton(true);
//                        MessageManager.showMessage("PrintFragment::onClick() - mSection56Model is null", ErrorSeverity.Medium);
//                    }
//                }
//                if (rowsUpdated == false) {
//                    Utilities.displayOkMessage("Database update failed, please reprint.", getActivity());
//                }
//
//                ticket.getInfringement().setIssueDateLocked(true);
//                uploadTicket();
//
//            } else {
//                mIssueAdditionalNoticeCheckBox.setVisibility(View.VISIBLE);
//                wizardActivity().enableNextButton(true);
//            }
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "PrintFragment::onClick()"), ErrorSeverity.Medium);
//            wizardActivity().enableNextButton(true);
//        }
    }

    public void findPrintersActivity() {

        Intent intent = new Intent(this.getActivity(), PrinterListActivity.class);
        startActivity(intent);
    }

    public void paymentButtonClick(){

        if (Utilities.validateUserAccess(SessionModel.getInstance().getUser(), "za.co.kapsch.ipayment") == false) {
            return;
        }

        startFineSearchInfoValidation(
            wizardActivity().getTicketModel().getVehicle().getLicenceNumber(),
            wizardActivity().getTicketModel().getOffender().getIdNumber());
    }

    private void startFineSearchInfoValidation(String vln, String idNumber){

        Intent intent = new Intent(this.getContext(), FineSearchInfoValidationActivity.class);
        intent.putExtra(za.co.kapsch.shared.Constants.VLN, vln);
        intent.putExtra(za.co.kapsch.shared.Constants.ID_NUMBER, idNumber);
        intent.putExtra(za.co.kapsch.shared.Constants.CORE_END_POINT, EndPointConfigModel.getInstance().getCoreGateway());
        intent.putExtra(za.co.kapsch.shared.Constants.ITS_END_POINT, EndPointConfigModel.getInstance().getITSGateway());
        intent.putExtra(za.co.kapsch.shared.Constants.EVR_END_POINT, EndPointConfigModel.getInstance().getEVRGateway());
        intent.putExtra(za.co.kapsch.shared.Constants.PRINTER_MAC_ADDRESS, SessionModel.getInstance().getPrinterMacAddress());

        startActivity(intent);
    }
}
