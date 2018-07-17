package za.co.kapsch.ipayment.Printer;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;

import java.util.concurrent.ExecutionException;

import za.co.kapsch.ipayment.Enums.PaymentMethod;
import za.co.kapsch.ipayment.General.Constants;
import za.co.kapsch.ipayment.Models.ConfigItemModel;
import za.co.kapsch.ipayment.Models.TransactionItemModel;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.ipayment.R;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Printer.ZebraPrinter;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

/**
 * Created by csenekal on 2017/09/17.
 */
public class Receipt {

    private Context mContext;
    private ZebraPrinter mZebraPrinter;
    private IAsyncProcessCallBack mAsyncProcessCallBack;
    private final int mHeaderLeftOffsetA = 190;
    private final int mRtsaLogoOffset = 155;
    private final int mBarcodeOffset = 10;

    private final int mItemColumnA = 20;
    private final int mInfoColumn = mItemColumnA + 180;
    private final int mItemColumnB = mItemColumnA + 250;
    private final int mItemColumnC = mItemColumnB + 110;
    private final int mItemColumnD = mItemColumnC + 110;

    private final int mPaymentDetailsHeaderOffset = 200;
    private final int mPaymentItemColumnA = 80;
    private final int mPaymentItemColumnB = 420;

    private final int mFooterOffset = 155;

    public Receipt(String macAddress, Context context, IAsyncProcessCallBack asyncProcessCallBack){
        mContext = context;
        mAsyncProcessCallBack = asyncProcessCallBack;
        mZebraPrinter = new ZebraPrinter(macAddress);
    }

    public String print(boolean waitForTask, TransactionModel transaction){

        if (waitForTask == true) {
            try {
                new Task().execute(transaction).get();
            } catch (InterruptedException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            } catch (ExecutionException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            }
            return null;
        }

        new Task().execute(transaction);
        return null;
    }

    private class Task extends AsyncTask<Object, AsyncResultModel, Object> {
        @Override
        protected Object doInBackground(Object... params) {

            try{
                buildPrintJob((TransactionModel) params[0]);
                printJob();
            }catch (Exception e){
                publishProgress(new AsyncResultModel(DataService.FAILED, null, Utilities.exceptionMessage(e, "Receipt::Task::doInBackground()"), Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
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

    private void buildPrintJob(TransactionModel transaction){

        int sectionIndex = 0;
        mZebraPrinter.setTopOffset(0);
        addBlankLine(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addRTSALogo(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addHeaderDetails(sectionIndex);
        addBlankLine(sectionIndex);
        //addBlankLine(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addBlankLine(sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addBarcode128(transaction.getReceipt(), sectionIndex);

        mZebraPrinter.setTopOffset(0);
        sectionIndex++;
        addTextDetails(transaction, sectionIndex);

        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);
    }

    private void printJob() throws Exception {
        mZebraPrinter.print();
    }

    private void addSeperator(int secionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.slip_seperator), ZebraPrinter.FONT_TYPE_D, -1, true, secionIndex);
    }

    private void addHeaderDetails(int sectionIndex)
    {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.ipay_receipt), ZebraPrinter.FONT_TYPE_0_C_L, mHeaderLeftOffsetA, true, sectionIndex);
    }

    private void addTextDetails(TransactionModel transaction, int sectionIndex)
    {
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.receipt_number), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(transaction.getReceipt(), ZebraPrinter.FONT_TYPE_0_C, mInfoColumn, false, sectionIndex);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.website), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(ConfigItemModel.getInstance().getReceiptWebsiteAddress(), ZebraPrinter.FONT_TYPE_0_C, mInfoColumn, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.call_centre_tel), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(ConfigItemModel.getInstance().getReceiptCallCentreNo(), ZebraPrinter.FONT_TYPE_0_C, mInfoColumn, false, sectionIndex);
        addBlankLine(sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.payment_date), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.dateToString(transaction.getReceiptTimeStamp()), ZebraPrinter.FONT_TYPE_0_C, mInfoColumn, false, sectionIndex);

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.person), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(transaction.getUserName(), ZebraPrinter.FONT_TYPE_0_C, mInfoColumn, false, sectionIndex);

        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.item_Description), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.qty), ZebraPrinter.FONT_TYPE_0_C, mItemColumnB, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.price), ZebraPrinter.FONT_TYPE_0_C, mItemColumnC, false, sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.value), ZebraPrinter.FONT_TYPE_0_C, mItemColumnD, false, sectionIndex);

        addSeperator(sectionIndex);
        for(TransactionItemModel transactionItem : transaction.getTransactionItems()){
            mZebraPrinter.addTextItem(transactionItem.getReferenceNumber(), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
            mZebraPrinter.addTextItem("  1", ZebraPrinter.FONT_TYPE_0_C, mItemColumnB, false, sectionIndex, false);
            mZebraPrinter.addTextItem(Utilities.padLeft(String.format("%.2f", transactionItem.getAmount()), 5, ' '), ZebraPrinter.FONT_TYPE_0_C, mItemColumnC, false, sectionIndex, false);
            mZebraPrinter.addTextItem(Utilities.padLeft(String.format("%.2f", transactionItem.getAmount()), 5, ' '), ZebraPrinter.FONT_TYPE_0_C, mItemColumnD, false, sectionIndex, false);
        }

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.total_amount), ZebraPrinter.FONT_TYPE_0_C, mItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(Integer.toString(transaction.getTransactionItems().size()), ZebraPrinter.FONT_TYPE_0_C, mItemColumnB, false, sectionIndex);
        mZebraPrinter.addTextItem(Utilities.padLeft(String.format("%.2f", transaction.getAmount()), 5, ' '), ZebraPrinter.FONT_TYPE_0_C, mItemColumnD, false, sectionIndex, false);

        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.payment_details), ZebraPrinter.FONT_TYPE_0_C_M, mPaymentDetailsHeaderOffset, true, sectionIndex);
        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(
                transaction.getPaymentSource() == PaymentMethod.DumaPay ?
                        mContext.getResources().getString(R.string.duma_pay) :
                        mContext.getResources().getString(R.string.cash),
                ZebraPrinter.FONT_TYPE_0_C, mPaymentItemColumnA, true, sectionIndex);
        mZebraPrinter.addTextItem(String.format("%.2f", transaction.getAmount()), ZebraPrinter.FONT_TYPE_0_C, mPaymentItemColumnB, false, sectionIndex);

        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.thank_you_for_payment), ZebraPrinter.FONT_TYPE_0_C_M, mFooterOffset, true, sectionIndex);

        addBlankLine(sectionIndex);
        addSeperator(sectionIndex);
        addBlankLine(sectionIndex);
    }

    private void addBarcode128(String receiptNumber, int sectionIndex){

        Bitmap bitmap = Utilities.getBarcode128(receiptNumber);

        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mBarcodeOffset, sectionIndex);
    }

    private void addRTSALogo(int sectionIndex){
        Bitmap bitmap = BitmapFactory.decodeResource(mContext.getResources(), R.drawable.rtsa_smallest);
        mZebraPrinter.addImageItem(bitmap, bitmap.getWidth(), bitmap.getHeight(), mRtsaLogoOffset, sectionIndex);
    }

    private void addBlankLine(int sectionIndex){
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.printer_blank_line), ZebraPrinter.FONT_TYPE_0_B, -1, true, sectionIndex);
    }
}
