package za.co.kapsch.iticket.Printer;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;

import java.util.List;
import java.util.concurrent.ExecutionException;

import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Enums.IdentificationType;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.iticket.orm.ChargeInfoRepository;
import za.co.kapsch.iticket.orm.IdentificationTypeRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.PaymentOption;
import za.co.kapsch.shared.Printer.ZebraPrinter;
import za.co.kapsch.shared.WebAccess.DataService;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.R;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.iticket.orm.IdentificationTypeRepository.getIdentificationType;


/**
 * Created by csenekal on 2016-12-28.
 */
public class HandWrittenSlip {

    private Context mContext;
    private ZebraPrinter mZebraPrinter;
    private IAsyncProcessCallBack mAsyncProcessCallBack;
    private final int mHeaderLeftOffsetA = 30;
    private final int mHeaderLeftOffsetB = mHeaderLeftOffsetA + 2;
    private final int mColumnAOffset = 20;
    private final int mRtsaLogoOffset = 40;
    private final int mQRCodeOffset = 180;
    private final int mColumnBOffset = mColumnAOffset + 220;

    public HandWrittenSlip(String macAddress, Context context, IAsyncProcessCallBack asyncProcessCallBack){
        mContext = context;
        mAsyncProcessCallBack = asyncProcessCallBack;
        mZebraPrinter = new ZebraPrinter(macAddress);
    }

    public String print(boolean waitForTask, TicketModel ticket){

        if (waitForTask == true) {
            try {
                new Task().execute(ticket).get();
            } catch (InterruptedException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            } catch (ExecutionException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            }
            return null;
        }

        new Task().execute(ticket);
        return null;
    }

    private class Task extends AsyncTask<Object, AsyncResultModel, Object> {
        @Override
        protected Object doInBackground(Object... params) {

            try{
                buildPrintJob((TicketModel) params[0]);
                printJob();
            }catch (Exception e){
                publishProgress(new AsyncResultModel(DataService.FAILED, null, Utilities.exceptionMessage(e, "HandWrittenSlip::Task::doInBackground()"), Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
            }

            return null;
        }

        // onPostExecute displays the results of the AsyncTask.
        @Override
        protected void onPostExecute(Object result) {
            super.onPostExecute(result);
            if (mContext != null) {
                Utilities.busyProgressBarEx((Activity)mContext, false);
                mAsyncProcessCallBack.finishedCallBack(new AsyncResultModel(DataService.SUCCESS, null, null, Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
            }
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            if (mContext != null) {
                Utilities.busyProgressBarEx((Activity)mContext, true);
            }
        }

        @Override
        protected void onProgressUpdate(AsyncResultModel... values) {
            super.onProgressUpdate(values);
            mAsyncProcessCallBack.progressCallBack(values[0]);
        }
    }

    private void buildPrintJob(TicketModel ticket){

        int sectionIndex = 0;
        mZebraPrinter.setTopOffset(0);
        addBlankLine(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addRTSALogo(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addHeaderDetails(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addQrCode(ticket.getInfringement().getExternalToken(), sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addTextDetails(ticket, mColumnAOffset, mColumnAOffset, mColumnBOffset, sectionIndex);

        byte[] officerSignature = ticket.getUser().getSignature();
        if (officerSignature != null) {
            mZebraPrinter.setTopOffset(0);
            sectionIndex++;
            addOfficerSignature(officerSignature, sectionIndex);
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addOfficerSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, sectionIndex);
        addSeperator(sectionIndex);

        OffenderModel offender = ticket.getOffender();
        if (offender != null) {
            byte[] accusedSignature = ticket.getOffender().getSignature();
            if (accusedSignature != null) {
                mZebraPrinter.setTopOffset(0);
                sectionIndex++;
                addAccusedSignature(accusedSignature, sectionIndex);

//                mZebraPrinter.setTopOffset(0);
//                sectionIndex++;
//                addAccusedSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, sectionIndex);
//                addFooter(ticket.getDistrict().getPaymentOptions(), ZebraPrinter.FONT_TYPE_0_A, sectionIndex);
//
//                mZebraPrinter.setTopOffset(0);
//                sectionIndex++;
//                addBlankLine(sectionIndex);
//
//                return;
            }
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addAccusedSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, sectionIndex);
        addFooter(ticket.getDistrict().getPaymentOptions(), ZebraPrinter.FONT_TYPE_0_A, sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addBlankLine(sectionIndex);

//        mZebraPrinter.setTopOffset(0);
//        sectionIndex++;
//        addAccusedSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, sectionIndex);
//        addFooter(ticket.getDistrict().getPaymentOptions(), ZebraPrinter.FONT_TYPE_0_C, 5);
//
//        mZebraPrinter.setTopOffset(0);
//        sectionIndex++;
//        addBlankLine(sectionIndex);
    }

    private void printJob() throws Exception {
        mZebraPrinter.print();
    }

    private void addSeperator(int secionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_seperator), ZebraPrinter.FONT_TYPE_D, -1, true, secionIndex);
    }

    private void addHeaderDetails(int sectionIndex)
    {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.notice_of_intended_prosection), ZebraPrinter.FONT_TYPE_0_C_L, mHeaderLeftOffsetA, true, sectionIndex);
        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.in_pursuant_to_section_162_of_the_road_traffic_act), ZebraPrinter.FONT_TYPE_0_B, mHeaderLeftOffsetB, true, sectionIndex);
    }

    private void addTextDetails(TicketModel ticket, int headingOffset, int detailCaptionOffset, int detailValueOffset, int sectionIndex)
    {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_number), ZebraPrinter.FONT_TYPE_0_C_M, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.formatReferenceNumber(ticket.getInfringement().getTicketNumber()), ZebraPrinter.FONT_TYPE_0_C_M, detailValueOffset, false, sectionIndex);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offender_details), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        addSeperator(sectionIndex);

        //mZebraPrinter.addTextItem(getIdTypeDescription(ticket.getOffender().getIdType()), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.id_number), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getOffender().getIdNumber(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.id_type), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(getIdTypeDescription(ticket.getOffender().getIdType()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.last_name), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getOffender().getLastName(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.first_name), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getOffender().getFirstName(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.mobile), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getOffender().getMobileNumber() == null ? Constants.EMPTY_STRING : ticket.getOffender().getMobileNumber(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.address_h), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getOffender().getPhysicalStreet1(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);
        mZebraPrinter.addTextItemOmitNA(ticket.getOffender().getPhysicalStreet2(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex);
        mZebraPrinter.addTextItemOmitNA(ticket.getOffender().getPhysicalSuburb(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex);
        mZebraPrinter.addTextItemOmitNA(ticket.getOffender().getPhysicalTown(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex);
        mZebraPrinter.addTextItemOmitNA(ticket.getOffender().getPhysicalCode(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.vehicle_details), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        addSeperator(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.registration_number), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getVehicle().getLicenceNumber(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.text_fragment_vehicle_Licence_Make), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getVehicle().getMake(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_details), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        addSeperator(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.you_are_hereby_warned_that_it_is_intended), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.exceeding_the_speed_limit), ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.reckless_driving), ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.dangerous_driving), ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.careless_driving), ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.failing_to_obey_traffic_signs_signal), ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.obstructing_a_road), ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);

        addSeperator(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.act_regulation), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getRegulation(), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
        addBlankLine(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.charge_1), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getChargeCode(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addBlankLine(sectionIndex);

        //Charge description is displayed against offence and captured charge description is displayed against description
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getDescription().toUpperCase(), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getPrintDescription(), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);

        addBlankLine(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_description), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getUserCapturedDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.amount), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(chargeAmount(ticket.getInfringement().getInfringementCharges()[0].getFineAmount()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addSeperator(sectionIndex);

        if (ticket.getInfringement().getInfringementCharges()[1] != null) {

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.act_regulation), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getRegulation(), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            addBlankLine(sectionIndex);

            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getIsAlternative() ? mContext.getResources().getString(R.string.alt_charge) : mContext.getResources().getString(R.string.charge_2), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getChargeCode(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            addBlankLine(sectionIndex);

            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getDescription().toUpperCase(), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getPrintDescription(), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);

            addBlankLine(sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_description), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getUserCapturedDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.amount), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(chargeAmount(ticket.getInfringement().getInfringementCharges()[1].getFineAmount()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            addSeperator(sectionIndex);
        }

        if (ticket.getInfringement().getInfringementCharges()[2] != null) {

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.act_regulation), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getRegulation(), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            addBlankLine(sectionIndex);

            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getIsAlternative() ?  mContext.getResources().getString(R.string.alt_charge) : mContext.getResources().getString(R.string.charge_3), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getChargeCode(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            addBlankLine(sectionIndex);

            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getDescription().toUpperCase(), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getPrintDescription(), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);

            addBlankLine(sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_description), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getUserCapturedDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.amount), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(chargeAmount(ticket.getInfringement().getInfringementCharges()[2].getFineAmount()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            addSeperator(sectionIndex);
        }

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_date_ex), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.dateTimeToString( ticket.getInfringement().getOffenceDate()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.issue_date), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.dateTimeToString( ticket.getInfringement().getIssueDate()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_location), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getLocationDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItemOmitNA(ticket.getInfringement().getLocationSuburb(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, true, sectionIndex);
        mZebraPrinter.addTextItemOmitNA(ticket.getInfringement().getLocationTown(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, true, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.place_of_issue), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getDistrict().getBranchName(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_official_details), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        addSeperator(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_official_name), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(
                        String.format("%1$s %2$s", ticket.getUser().getFirstName(), ticket.getUser().getLastName()),
                        ZebraPrinter.FONT_TYPE_0_C,
                        detailValueOffset,
                        false,
                        sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_official_code), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getUser().getInfrastructureNumber(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addSeperator(sectionIndex);
    }

    private String getIdTypeDescription(long idTypeID){
        try {
            IdentificationTypeModel identificationType = IdentificationTypeRepository.getIdentificationType(idTypeID);
            return identificationType.getDescription();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "HandWrittenSlip::getIdType()"), ErrorSeverity.None);
            return IdentificationType.Unknown.toString();
        }
    }

    private String chargeAmount(double fineAmount){
        return fineAmount == ConfigItemModel.getInstance().getNagAmount() ? Constants.NAG : String.format("ZMW %.2f", fineAmount);
    }

    private void addOfficerSignatureText(String detailFont, int detailCaptionOffset, int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_signature), detailFont, detailCaptionOffset, true, sectionIndex);
    }

    private void addOfficerSignature(byte[] signature, int sectionIndex){

        if (signature == null) return;

        Bitmap bitmap = BitmapFactory.decodeByteArray(signature , 0, signature.length);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth()/3, bitmap.getHeight()/3, mColumnAOffset, sectionIndex);
    }

    private void addAccusedSignature(byte[] signature, int sectionIndex){

        if (signature == null) return;

        Bitmap bitmap = BitmapFactory.decodeByteArray(signature , 0, signature.length);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth()/3, bitmap.getHeight()/3, mColumnAOffset, sectionIndex);
    }

    private void addAccusedSignatureText(String detailFont, int detailCaptionOffset, int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section341_slip_accused_signature), detailFont, detailCaptionOffset, true, sectionIndex);
    }

    private void addFooter(List<PaymentOption> paymentOptions, String detailFont, int sectionIndex){

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.payment_options), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);

        for (PaymentOption paymentOption: paymentOptions) {

            if (paymentOption.getHeader().equals(za.co.kapsch.shared.Constants.EMPTY_STRING) == false) {
                mZebraPrinter.addTextItem(paymentOption.getHeader(), detailFont, mColumnAOffset, true, sectionIndex);
            }

            if (paymentOption.getDetails().equals(za.co.kapsch.shared.Constants.EMPTY_STRING) == false) {
                mZebraPrinter.addTextItem(paymentOption.getDetails(), detailFont, mColumnAOffset, true, sectionIndex);
            }

            if (paymentOption.getAddress().equals(za.co.kapsch.shared.Constants.EMPTY_STRING) == false) {
                mZebraPrinter.addTextItem(paymentOption.getAddress(), detailFont, mColumnAOffset, true, sectionIndex);
            }

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);
        }
    }

    private void addQrCode(String externalTokenNumber, int sectionIndex){

        Bitmap bitmap = Utilities.getQrCode(externalTokenNumber);

        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mQRCodeOffset, sectionIndex);
    }

    private void addRTSALogo(int sectionIndex){
        Bitmap bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.zambia_police_rtsa_logo_small);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth()/2, bitmap.getHeight()/2, mRtsaLogoOffset, sectionIndex);
    }

    private void addBlankLine(int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_blank_line), ZebraPrinter.FONT_TYPE_0_B, -1, true, sectionIndex);
    }

    private String getChargeDescription(String chargeCode){
        try {
            return ChargeInfoRepository.getCharge(chargeCode).getDescription();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "HandWritten::getChargeDescription"), ErrorSeverity.High);
            return null;
        }
    }
}
