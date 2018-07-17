package za.co.kapsch.iticket.Printer;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;

import java.util.List;
import java.util.concurrent.ExecutionException;

import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Enums.ViolationCategory;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.iticket.R;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.PaymentOption;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Printer.ZebraPrinter;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

/**
 * Created by CSenekal on 2018/04/19.
 */

public class OutstandingViolationsSlip {

    private Context mContext;
    private ZebraPrinter mZebraPrinter;
    private IAsyncProcessCallBack mAsyncProcessCallBack;
    private final int mHeaderLeftOffsetA = 60;
    private final int mColumnAOffset = 20;
    private final int mRtsaLogoOffset = 50;
    private final int mColumnBOffset = mColumnAOffset + 300;

    public OutstandingViolationsSlip(String macAddress, Context context, IAsyncProcessCallBack asyncProcessCallBack){
        mContext = context;
        mAsyncProcessCallBack = asyncProcessCallBack;
        mZebraPrinter = new ZebraPrinter(macAddress);
    }

    public String print(boolean waitForTask, List<FineModel> outstandingViolations, ViolationCategory violationCategory){

        if (waitForTask == true) {
            try {
                new OutstandingViolationsSlip.Task().execute(outstandingViolations, violationCategory).get();
            } catch (InterruptedException e) {
                return Utilities.exceptionMessage(e, "OutstandingViolationsSlip::print()");
            } catch (ExecutionException e) {
                return Utilities.exceptionMessage(e, "OutstandingViolationsSlip::print()");
            }
            return null;
        }

        new OutstandingViolationsSlip.Task().execute(outstandingViolations, violationCategory);
        return null;
    }

    private class Task extends AsyncTask<Object, AsyncResultModel, Object> {
        @Override
        protected Object doInBackground(Object... params) {

            try {
                buildPrintJob((List<FineModel>) params[0], (ViolationCategory) params[1]);
                printJob();
            } catch (Exception e) {
                publishProgress(new AsyncResultModel(DataService.FAILED, null, Utilities.exceptionMessage(e, "HandWrittenSlip::Task::doInBackground()"), Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
            }

            return null;
        }


        // onPostExecute displays the results of the AsyncTask.
        @Override
        protected void onPostExecute(Object result) {
            super.onPostExecute(result);
            if (mContext != null) {
                Utilities.busyProgressBarEx((Activity) mContext, false);
                mAsyncProcessCallBack.finishedCallBack(new AsyncResultModel(DataService.SUCCESS, null, null, Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
            }
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            if (mContext != null) {
                Utilities.busyProgressBarEx((Activity) mContext, true);
            }
        }

        @Override
        protected void onProgressUpdate(AsyncResultModel... values) {
            super.onProgressUpdate(values);
            mAsyncProcessCallBack.progressCallBack(values[0]);
        }
    }

    private void printJob() throws Exception {
        mZebraPrinter.print();
    }

    private void buildPrintJob(List<FineModel> outstandingViolations, ViolationCategory violationCategory) {

        int sectionIndex = 0;
        mZebraPrinter.setTopOffset(0);
        addBlankLine(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addRTSALogo(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;

        switch (violationCategory) {
            case Person:
                addHeaderDetails(sectionIndex, outstandingViolations.get(0).getOffenderIDNumber(), violationCategory);
                break;
            case Vehicle:
                addHeaderDetails(sectionIndex, outstandingViolations.get(0).getVLN(), violationCategory);
                break;
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        for (FineModel outstandingViolation: outstandingViolations) {
            addDetails(sectionIndex, outstandingViolation);
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addFooter(SessionModel.getInstance().getDistrict().getPaymentOptions(), ZebraPrinter.FONT_TYPE_0_A, sectionIndex);

        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);
    }

    private void addHeaderDetails(int sectionIndex, String identifier, ViolationCategory violationCategory)
    {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.outstanding_offences_for), ZebraPrinter.FONT_TYPE_0_C_L, mHeaderLeftOffsetA, true, sectionIndex);
        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(
                violationCategory == ViolationCategory.Person ?
                        mContext.getResources().getString(R.string.id_number) :
                        mContext.getResources().getString(R.string.registration_number),
                ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(identifier, ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);
        addSeperator(sectionIndex);
    }

    private void addDetails(int sectionIndex, FineModel outstandingViolation) {

        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.notice_number_uppercase), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(outstandingViolation.getReferenceNumber(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);
        addSeperator(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.offence_date_ex), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.dateTimeToString(outstandingViolation.getOffenceDate()), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.total_amount), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(
                String.format(
                        "%s %.2f",
                        mContext.getResources().getString(R.string.zambia_currency),
                        outstandingViolation.getOutstandingAmount()),
                ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);
    }

    private void addRTSALogo(int sectionIndex){
        Bitmap bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.zambia_police_rtsa_logo_small);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth()/2, bitmap.getHeight()/2, mRtsaLogoOffset, sectionIndex);
    }

    private void addBlankLine(int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_blank_line), ZebraPrinter.FONT_TYPE_0_B, -1, true, sectionIndex);
    }

    private void addSeperator(int secionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_seperator), ZebraPrinter.FONT_TYPE_D, -1, true, secionIndex);
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

//    private void addFooter(String detailFont, int sectionIndex){
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);
//        mZebraPrinter.addTextItem(ConfigItemModel.getInstance().getPaymentDetailsHeading(), detailFont, mColumnAOffset, true, sectionIndex);
//        mZebraPrinter.addTextItem(ConfigItemModel.getInstance().getPaymentDetailsInfo(), detailFont, mColumnAOffset, true, sectionIndex);
//        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_line), ZebraPrinter.FONT_TYPE_D, -1, true, sectionIndex);
//    }
}