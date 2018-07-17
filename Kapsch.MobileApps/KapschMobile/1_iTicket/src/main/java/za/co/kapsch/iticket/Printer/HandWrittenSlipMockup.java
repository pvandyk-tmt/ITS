package za.co.kapsch.iticket.Printer;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;

import java.util.concurrent.ExecutionException;

import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.R;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Printer.ZebraPrinter;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

/**
 * Created by CSenekal on 2018/03/05.
 */

public class HandWrittenSlipMockup {

    private Context mContext;
    private ZebraPrinter mZebraPrinter;
    private IAsyncProcessCallBack mAsyncProcessCallBack;
    private final int mHeaderLeftOffsetA = 30;
    private final int mHeaderLeftOffsetB = mHeaderLeftOffsetA + 2;
    private final int mColumnAOffset = 20;
    private final int mRtsaLogoOffset = 150;
    private final int mQRCodeOffset = 180;
    private final int mColumnBOffset = mColumnAOffset + 220;

    public HandWrittenSlipMockup(String macAddress, Context context, IAsyncProcessCallBack asyncProcessCallBack){
        mContext = context;
        mAsyncProcessCallBack = asyncProcessCallBack;
        mZebraPrinter = new ZebraPrinter(macAddress);
    }

    public String print(boolean waitForTask, TicketModel ticket){

        if (waitForTask == true) {
            try {
                new HandWrittenSlipMockup.Task().execute(ticket).get();
            } catch (InterruptedException e) {
                return Utilities.exceptionMessage(e, "HandWrittenSlipMockup::print()");
            } catch (ExecutionException e) {
                return Utilities.exceptionMessage(e, "HandWrittenSlipMockup::print()");
            }
            return null;
        }

        new HandWrittenSlipMockup.Task().execute(ticket);
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

                mZebraPrinter.setTopOffset(0);
                sectionIndex++;
                addAccusedSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, sectionIndex);
                addFooter(ZebraPrinter.FONT_TYPE_0_A, sectionIndex);

                mZebraPrinter.setTopOffset(0);
                sectionIndex++;
                addBlankLine(sectionIndex);

                return;
            }
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addAccusedSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, sectionIndex);
        addFooter(ZebraPrinter.FONT_TYPE_0_C, 5);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addBlankLine(sectionIndex);
    }

    private void printJob() throws Exception {
        mZebraPrinter.print();
    }

    private void addSeperator(int secionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_seperator), ZebraPrinter.FONT_TYPE_D, -1, true, secionIndex);
    }

    private void addHeaderDetails(int sectionIndex)
    {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.warning_of_intended_prosection), ZebraPrinter.FONT_TYPE_0_C_L, mHeaderLeftOffsetA, true, sectionIndex);
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
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.nrc_number), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem("134625/10/1", ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.last_name), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem("CHISENGA", ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.first_name), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem("LESA", ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.address_h), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem("609 Zambezi Road", ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex, false, true);
        mZebraPrinter.addTextItem("Roma", ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex, false);
        mZebraPrinter.addTextItem("Roma", ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex, false);
        mZebraPrinter.addTextItem("Lusaka", ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex, false);
        mZebraPrinter.addTextItem("10101", ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, true, sectionIndex, false);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.vehicle_details), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        addSeperator(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.registration_number), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getVehicle().getLicenceNumber(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.text_fragment_vehicle_Licence_Make), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getVehicle().getMake(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.text_fragment_vehicle_Licence_Model), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getVehicle().getModel(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_details), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);
        addSeperator(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.you_are_hereby_warned_that_it_is_intended), ZebraPrinter.FONT_TYPE_0_C, headingOffset, true, sectionIndex);

        mZebraPrinter.addTextItem("• Exceeding the Speed Limit", ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
        mZebraPrinter.addTextItem("• Reckless Driving", ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
        mZebraPrinter.addTextItem("• Dangerous Driving", ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
        mZebraPrinter.addTextItem("• Careless Driving", ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
        mZebraPrinter.addTextItem("• Failing to obey traffic signs/signal", ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);
        mZebraPrinter.addTextItem("• Obstructing a Road", ZebraPrinter.FONT_TYPE_0_C, headingOffset+20, true, sectionIndex);

        addSeperator(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.charge_1), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getChargeCode(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.description), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[0].getDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.amount), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ChargeAmount(ticket.getInfringement().getInfringementCharges()[0].getFineAmount()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        addSeperator(sectionIndex);

        if (ticket.getInfringement().getInfringementCharges()[1] != null) {
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getIsAlternative() ? "ALt Charge:" : "Charge 2:", ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getChargeCode(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.description), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.regulation), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[1].getRegulation(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.amount), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ChargeAmount(ticket.getInfringement().getInfringementCharges()[1].getFineAmount()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            addSeperator(sectionIndex);
        }

        if (ticket.getInfringement().getInfringementCharges()[2] != null) {
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getIsAlternative() ? "ALt Charge:" : "Charge 3:", ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getChargeCode(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.description), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getDescription(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.regulation), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getInfringementCharges()[2].getRegulation(), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.amount), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ChargeAmount(ticket.getInfringement().getInfringementCharges()[2].getFineAmount()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

            addSeperator(sectionIndex);
        }

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_date), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.dateTimeToString( ticket.getInfringement().getOffenceDate()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.issue_date), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.dateTimeToString( ticket.getInfringement().getIssueDate()), ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_location), ZebraPrinter.FONT_TYPE_0_C, detailCaptionOffset, true, sectionIndex);

        //"Corner of Mukwa Drive and Kafue Road, Lusaka, Zambia. P.O Box 320414"
        mZebraPrinter.addTextItem("Corner of Mukwa Drive and Kafue Road", ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, false, sectionIndex);
        mZebraPrinter.addTextItem("Kabulonga", ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, true, sectionIndex, false);
        mZebraPrinter.addTextItem("Lusaka", ZebraPrinter.FONT_TYPE_0_C, detailValueOffset, true, sectionIndex, false);

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

    private String ChargeAmount(double fineAmount){
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

    private void addFooter(String detailFont, int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);

        mZebraPrinter.addTextItem(ConfigItemModel.getInstance().getPaymentDetailsHeading(), detailFont, mColumnAOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(ConfigItemModel.getInstance().getPaymentDetailsInfo(), detailFont, mColumnAOffset, true, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);
    }

    private void addQrCode(String externalTokenNumber, int sectionIndex){

        Bitmap bitmap = Utilities.getQrCode(externalTokenNumber);

        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mQRCodeOffset, sectionIndex);
    }

    private void addRTSALogo(int sectionIndex){
        Bitmap bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.rtsa_small);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth()/2, bitmap.getHeight()/2, mRtsaLogoOffset, sectionIndex);
    }

    private void addBlankLine(int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_blank_line), ZebraPrinter.FONT_TYPE_0_B, -1, true, sectionIndex);
    }
}