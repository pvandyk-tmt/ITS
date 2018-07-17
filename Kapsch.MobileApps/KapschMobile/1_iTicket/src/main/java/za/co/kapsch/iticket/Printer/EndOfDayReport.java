package za.co.kapsch.iticket.Printer;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.AsyncTask;
import android.text.TextUtils;

import java.util.List;
import java.util.concurrent.ExecutionException;

import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Models.InfringementChargeModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.iticket.R;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Printer.ZebraPrinter;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.WebAccess.DataService;

/**
 * Created by csenekal on 2018/06/01.
 */

public class EndOfDayReport {

    private Context mContext;
    private ZebraPrinter mZebraPrinter;
    private IAsyncProcessCallBack mAsyncProcessCallBack;
    private final int mHeaderALeftOffset = 165;
    private final int mHeaderBLeftOffset = 141;
    private final int mColumnAOffset = 0;
    private final int mColumnBOffset = mColumnAOffset + 120;
    private final String mChargeSeperator = "-------------------------";

    public EndOfDayReport(String macAddress, Context context, IAsyncProcessCallBack asyncProcessCallBack){
        mContext = context;
        mAsyncProcessCallBack = asyncProcessCallBack;
        mZebraPrinter = new ZebraPrinter(macAddress);
    }

    public void print(List<TicketModel> ticketList, String fromDate, String toDate) throws Exception {
        buildPrintJob(ticketList, fromDate, toDate);
        mZebraPrinter.print();
    }

    public String print(boolean waitForTask, List<TicketModel> ticketList, String fromDate, String toDate){

        if (waitForTask == true) {
            try {
                new Task().execute(ticketList, fromDate, toDate).get();
            } catch (InterruptedException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            } catch (ExecutionException e) {
                return Utilities.exceptionMessage(e, "PrintSection56Slip::print()");
            }
            return null;
        }

        new Task().execute(ticketList, fromDate, toDate);
        return null;
    }

    private class Task extends AsyncTask<Object, AsyncResultModel, Object> {
        @Override
        protected Object doInBackground(Object... params) {

            try{
                buildPrintJob((List<TicketModel>) params[0], (String)params[1], (String)params[2]);
                mZebraPrinter.print();
            }catch (Exception e){
                publishProgress(new AsyncResultModel(DataService.FAILED, null, Utilities.exceptionMessage(e, "EndOfDayReport::Task::doInBackground()"), Constants.PROCESS_ID_ASYNC_PROCESS_PRINT));
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

    private void buildPrintJob(List<TicketModel> ticketList, String fromDate, String toDate) {

        int sectionIndex = 0;

        mZebraPrinter.setTopOffset(0);
        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report), ZebraPrinter.FONT_TYPE_0_C, mHeaderALeftOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(String.format("%s - %s", fromDate, toDate), ZebraPrinter.FONT_TYPE_0_C, mHeaderBLeftOffset, true, sectionIndex);
        mZebraPrinter.addTextItem(mChargeSeperator, ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);

        for (TicketModel ticket : ticketList) {

            mZebraPrinter.addTextItem(ticket.getInfringement().getTicketNumber(), ZebraPrinter.FONT_TYPE_0_C, mHeaderALeftOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(mChargeSeperator, ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report_idNumber), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(Utilities.nullOrEmptyString(ticket.getOffender().getIdNumber(), true), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report_adapter_name), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(getOffenderName(ticket), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report_offence_date), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(Utilities.dateTimeToString(ticket.getInfringement().getOffenceDate()), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report_vehicle), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(getVehicleInfo(ticket), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report_location), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(getOffenceLocation(ticket), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

            InfringementChargeModel[] charges = ticket.getInfringement().getInfringementCharges();
            if (charges[0] != null) {
                addBlankLine(sectionIndex);
                mZebraPrinter.addTextItem(getChargeInfo(charges[0]), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            }

            if (charges[1] != null) {
                addBlankLine(sectionIndex);
                mZebraPrinter.addTextItem(getChargeInfo(charges[1]), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            }

            if (charges[2] != null) {
                addBlankLine(sectionIndex);
                mZebraPrinter.addTextItem(getChargeInfo(charges[2]), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            }

            addBlankLine(sectionIndex);
            mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.end_of_day_report_notes), ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, true, sectionIndex);
            mZebraPrinter.addTextItem(ticket.getInfringement().getNotes(), ZebraPrinter.FONT_TYPE_0_C, mColumnBOffset, false, sectionIndex);

            addBlankLine(sectionIndex);
            mZebraPrinter.addTextItem(mChargeSeperator, ZebraPrinter.FONT_TYPE_0_C, mColumnAOffset, false, sectionIndex);
        }

        addBlankLine(sectionIndex);
        addBlankLine(sectionIndex);
    }

    private String getVehicleInfo(TicketModel ticket){

        VehicleModel vehicle = ticket.getVehicle();
        return Utilities.nullOrEmptyString(
                Utilities.trimString(String.format("%s, %s %s %s",
                        Utilities.nullOrEmptyString(vehicle.getLicenceNumber(), false),
                        Utilities.nullOrEmptyString(vehicle.getColour(), false),
                        Utilities.nullOrEmptyString(vehicle.getMake(), false),
                        Utilities.nullOrEmptyString(vehicle.getType(), false))), true);
    }

    private String getOffenceLocation(TicketModel ticket){

        return String.format(
                "%s, %s",
                Utilities.nullOrEmptyString(ticket.getInfringement().getLocationDescription(), false),
                Utilities.nullOrEmptyString(ticket.getInfringement().getLocationSuburb(), false));
    }

    private String getOffenderName(TicketModel ticket){

        return Utilities.nullOrEmptyString(
                Utilities.trimString(String.format("%s %s",
                        Utilities.nullOrEmptyString(ticket.getOffender().getFirstName(), false),
                        Utilities.nullOrEmptyString(ticket.getOffender().getLastName(), false))), true);
    }

    private String getChargeInfo(InfringementChargeModel infringementCharge){

        return Utilities.nullOrEmptyString(
                Utilities.trimString(String.format("#%s %s",
                        Utilities.nullOrEmptyString(infringementCharge.getChargeCode(), false),
                        Utilities.nullOrEmptyString(infringementCharge.getDescription(), false))), true);
    }

    private void addBlankLine(int sectionIndex){

        mZebraPrinter.addTextItem(mContext.getResources().getString(R.string.section_slip_blank_line), ZebraPrinter.FONT_TYPE_0_C, -1, true, sectionIndex);
    }
}
