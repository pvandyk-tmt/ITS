package za.co.kapsch.ivehicletest.Printer;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.text.TextUtils;

import java.util.Calendar;
import java.util.List;
import java.util.concurrent.ExecutionException;

import za.co.kapsch.ivehicletest.Enums.QuestionType;
import za.co.kapsch.ivehicletest.General.Constants;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionResultModel;
import za.co.kapsch.ivehicletest.R;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Printer.ZebraPrinter;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

import static za.co.kapsch.shared.Utilities.dateTimeToString;

/**
 * Created by csenekal on 2017/09/17.
 */
public class InspectionResultsSlip {

    private Context mContext;
    private ZebraPrinter mZebraPrinter;
    private IAsyncProcessCallBack mAsyncProcessCallBack;
    private final int mHeaderLeftOffsetA = 140;
    private final int mHeaderLeftOffsetB = 190;
    private final int mHeaderLeftOffsetC = 155;

    private final int mColumnA = 40;
    private final int mColumnB = 300;
    private final int mColumnWidthA = 20;
    private final int mColumnWidthB = 36;
    private final int mRtsaLogoOffset = 165;
    private final int mImsLogoOffset = 155;
    private final int mBarcodeOffset = 79;

    public InspectionResultsSlip(String macAddress, Context context, IAsyncProcessCallBack asyncProcessCallBack){
        mContext = context;
        mAsyncProcessCallBack = asyncProcessCallBack;
        mZebraPrinter = new ZebraPrinter(macAddress);
    }

    public String print(
            boolean waitForTask,
            String bookingReferenceNumber,
            String site,
            String testCategory,
            String barcodeData,
            String finalResult,
            List<VehicleInspectionResultModel> vehicleInspectionResultList){

        if (waitForTask == true) {
            try {
                new Task().execute(bookingReferenceNumber, site, testCategory, barcodeData, finalResult, vehicleInspectionResultList).get();
            } catch (InterruptedException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            } catch (ExecutionException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            }
            return null;
        }

        new Task().execute(bookingReferenceNumber, site, testCategory, barcodeData, finalResult, vehicleInspectionResultList);
        return null;
    }

    private class Task extends AsyncTask<Object, AsyncResultModel, Object> {
        @Override
        protected Object doInBackground(Object... params) {

            try{
                buildPrintJob((String)params[0], (String)params[1], (String)params[2], (String)params[3], (String)params[4], (List<VehicleInspectionResultModel>) params[5]);
                printJob();
            }catch (Exception e){
                publishProgress(new AsyncResultModel(DataService.FAILED, null, Utilities.exceptionMessage(e, "InspectionResultsSlip::Task::doInBackground()"), Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
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

    private void buildPrintJob(String bookingReferenceNumber, String siteName, String testCategory, String barcodeData, String finalResult, List<VehicleInspectionResultModel> vehicleInspectionResults){

        int sectionIndex = 0;
        mZebraPrinter.setTopOffset(0);
        addBlankLine(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addRTSALogo(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addHeaderDetails(sectionIndex, bookingReferenceNumber, siteName, testCategory);

        if (barcodeData != null) {
            mZebraPrinter.setTopOffset(0);
            sectionIndex++;
            addBarcode128Ex(barcodeData, sectionIndex);

            mZebraPrinter.setTopOffset(0);
            sectionIndex++;
            mZebraPrinter.addTextItem(
                    barcodeData,
                    ZebraPrinter.FONT_TYPE_0_C,
                    ZebraPrinter.getLeftOffsetToCenterTextEx(ZebraPrinter.FONT_TYPE_0_C_MAX_LINE_CHARS, barcodeData),
                    false,
                    sectionIndex);
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addSeperator(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        for (int index = 0; index < vehicleInspectionResults.size(); index++) {
            addTextDetails(vehicleInspectionResults.get(index), sectionIndex);
        }

        addFinalResult(finalResult, sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addOfficerDetails(sectionIndex);
        addBlankLine(sectionIndex);

        byte[] officerSignature = SessionModel.getInstance().getUser().getSignature();
        if (officerSignature != null) {
            mZebraPrinter.setTopOffset(0);
            sectionIndex++;
            addOfficerSignature(officerSignature, sectionIndex);
        }

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addOfficerSignatureText(ZebraPrinter.FONT_TYPE_0_C, mColumnA, sectionIndex);
        addSeperator(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addIMSLogo(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addBlankLine(sectionIndex);
    }

    private void printJob() throws Exception {
        mZebraPrinter.print();
    }

    private void addHeaderDetails(int sectionIndex, String bookingReferenceNumber, String siteName, String testCategory)
    {
        mZebraPrinter.addTextItem(
                testCategory,
                ZebraPrinter.FONT_TYPE_0_C_L,
                ZebraPrinter.getLeftOffsetToCenterTextEx(ZebraPrinter.FONT_TYPE_0_C_L_MAX_LINE_CHARS, testCategory),
                true,
                sectionIndex);

        addBlankLine(sectionIndex);

        mZebraPrinter.addTextItem(
                dateTimeToString(Calendar.getInstance().getTime()),
                ZebraPrinter.FONT_TYPE_0_C,
                mHeaderLeftOffsetB,
                true,
                sectionIndex);

        addBlankLine(sectionIndex);

        mZebraPrinter.addTextItem(
                String.format("%s %s", mContext.getResources().getString(R.string.reference_no), bookingReferenceNumber),
                ZebraPrinter.FONT_TYPE_0_C,
                mColumnA,
                true,
                sectionIndex);

        mZebraPrinter.addTextItem(
                mContext.getResources().getString(R.string.site_name),
                ZebraPrinter.FONT_TYPE_0_C,
                mColumnA,
                true,
                sectionIndex);

        List<String> siteNames = Utilities.wrapText(siteName, mColumnWidthB);

        if (siteNames != null) {
            for (int index = 0; index < siteNames.size(); index++) {
                mZebraPrinter.addTextItem(
                        siteNames.get(index),
                        ZebraPrinter.FONT_TYPE_0_C,
                        mHeaderLeftOffsetC,
                        ((index == 0) == false),
                        sectionIndex);
            }
        }

        addBlankLine(sectionIndex);
    }

    private void addTextDetails(VehicleInspectionResultModel vehicleInspectionResult, int sectionIndex) {

        List<String> questionLines = Utilities.wrapText(vehicleInspectionResult.getQuestion(), mColumnWidthA);
        if (questionLines != null) {
            for (String questionLine : questionLines) {
                mZebraPrinter.addTextItem(questionLine, ZebraPrinter.FONT_TYPE_0_C, mColumnA, true, sectionIndex);
            }
        }

        List<String> compareValueLines = Utilities.wrapText(vehicleInspectionResult.getCompareValue(), mColumnWidthA);
        if (compareValueLines != null) {
            for (String compareValue : compareValueLines) {
                mZebraPrinter.addTextItem(compareValue, ZebraPrinter.FONT_TYPE_0_C, mColumnA, true, sectionIndex);
            }
        }

        String answer = Constants.EMPTY_STRING;
        if (TextUtils.isEmpty(vehicleInspectionResult.getAnswer()) == true) {
            answer = vehicleInspectionResult.getMultipleChoiceText();
        } else{
            answer = String.format("%s - %s", vehicleInspectionResult.getAnswer(),
                    TextUtils.isEmpty(vehicleInspectionResult.getMultipleChoiceText()) == true ?
                            vehicleInspectionResult.getPassFailedText() :
                            vehicleInspectionResult.getMultipleChoiceText());
        }

        List<String> answerLines = Utilities.wrapText(answer, mColumnWidthA);
        if (answerLines != null) {
            for (int index = 0; index < answerLines.size(); index++) {
                List<String> wrappedAnswerLines = Utilities.wrapText(answerLines.get(index), mColumnWidthA);
                if (wrappedAnswerLines != null) {
                    for (String wrappedAnswerLine : wrappedAnswerLines) {
                        mZebraPrinter.addTextItem(wrappedAnswerLine, ZebraPrinter.FONT_TYPE_0_C, mColumnB, index != 0, sectionIndex);
                    }
                }
                //mZebraPrinter.addTextItem(answerLines.get(index), ZebraPrinter.FONT_TYPE_0_C, mColumnB, false, sectionIndex);
            }
        }

        String[] carriageReturnedComments = vehicleInspectionResult.getComments().split(za.co.kapsch.shared.Constants.NEW_LINE);
        for (String comment : carriageReturnedComments) {
            List<String> comments = Utilities.wrapText(comment, mColumnWidthA);
            if (comments != null) {
                for (int index = 0; index < comments.size(); index++) {
                    mZebraPrinter.addTextItem(comments.get(index), ZebraPrinter.FONT_TYPE_0_C, mColumnB, true, sectionIndex);
                }
            }
        }

        addBlankLine(sectionIndex);
    }

    private void addFinalResult(String finalResult, int sectionIndex) {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.final_result), ZebraPrinter.FONT_TYPE_0_C, mColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(finalResult, ZebraPrinter.FONT_TYPE_0_C, mColumnB, false, sectionIndex);
    }

    private void addOfficerDetails(int sectionIndex){

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_official_details), ZebraPrinter.FONT_TYPE_0_C, mColumnA, true, sectionIndex);
        addSeperator(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_official_name), ZebraPrinter.FONT_TYPE_0_C, mColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(
                String.format("%1$s %2$s", SessionModel.getInstance().getUser().getFirstName(), SessionModel.getInstance().getUser().getLastName()),
                ZebraPrinter.FONT_TYPE_0_C,
                mColumnB,
                false,
                sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_official_code), ZebraPrinter.FONT_TYPE_0_C, mColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(SessionModel.getInstance().getUser().getInfrastructureNumber(), ZebraPrinter.FONT_TYPE_0_C, mColumnB, false, sectionIndex);
    }

    private void addSeperator(int secionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.slip_seperator), ZebraPrinter.FONT_TYPE_D, -1, true, secionIndex);
    }

    private void addBlankLine(int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.printer_blank_line), ZebraPrinter.FONT_TYPE_0_B, -1, true, sectionIndex);
    }

    private void addOfficerSignature(byte[] signature, int sectionIndex){

        if (signature == null) return;

        Bitmap bitmap = BitmapFactory.decodeByteArray(signature , 0, signature.length);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth()/3, bitmap.getHeight()/3, mColumnA, sectionIndex);
    }

    private void addOfficerSignatureText(String detailFont, int detailCaptionOffset, int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.officer_signature), detailFont, detailCaptionOffset, true, sectionIndex);
    }

    private void addRTSALogo(int sectionIndex){
        Bitmap bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.rtsa_smallest);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mRtsaLogoOffset, sectionIndex);
    }

    private void addIMSLogo(int sectionIndex){
        Bitmap bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.ims_logo_2018_small);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mImsLogoOffset, sectionIndex);
    }

    private void addBarcode128(String receiptNumber, int sectionIndex){

        Bitmap bitmap = Utilities.getBarcode128(receiptNumber);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mBarcodeOffset, sectionIndex);
    }

    private void addBarcode128Ex(String receiptNumber, int sectionIndex){

        Bitmap bitmap = Utilities.getBarcode128Ex(receiptNumber, 412, 80);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mBarcodeOffset, sectionIndex);
    }
}
