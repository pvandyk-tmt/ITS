package za.co.kapsch.iticket;

import android.app.DatePickerDialog;
import android.app.Dialog;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.MenuItem;
import android.view.View;
import android.widget.DatePicker;
import android.widget.ListView;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.List;

import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.HandWrittenToTicketModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.Printer.EndOfDayReport;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

import static android.app.AlertDialog.THEME_HOLO_LIGHT;

public class EndOfDayReportActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private int mStartYear = 0;
    private int mStartMonth = 0;
    private int mStartDay = 0;
    private int mEndYear = 0;
    private int mEndMonth = 0;
    private int mEndDay = 0;
    private ListView mListView;
    private TextView mStartDateTextView;
    private TextView mEndDateTextView;
    private List<TicketModel> mTicketList;
    private EndOfDayReportListAdapter mEndOfDayReportListAdater;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_end_of_day_report);

        mListView = (ListView)findViewById(R.id.listView);
        mStartDateTextView = (TextView)findViewById(R.id.startDateTextView);
        mEndDateTextView = (TextView)findViewById(R.id.endDateTextView);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        setDefualtDate();
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
        }

        return super.onOptionsItemSelected(item);
    }

    private void setDefualtDate(){
        Calendar calendar = Calendar.getInstance();
        mStartYear = calendar.get(Calendar.YEAR);
        mStartMonth = calendar.get(Calendar.MONTH);
        mStartDay = calendar.get(Calendar.DAY_OF_MONTH);

        mEndYear = calendar.get(Calendar.YEAR);
        mEndMonth = calendar.get(Calendar.MONTH);
        mEndDay = calendar.get(Calendar.DAY_OF_MONTH);

        mStartDateTextView.setText(Utilities.dateToString(Utilities.getCalendar(mStartYear, mStartMonth, mStartDay, 0, 0, 0).getTime()));
        mEndDateTextView.setText(Utilities.dateToString(Utilities.getCalendar(mEndYear, mEndMonth, mEndDay, 0, 0, 0).getTime()));
    }

    private DatePickerDialog.OnDateSetListener startDatePickerListner = new DatePickerDialog.OnDateSetListener() {
        @Override
        public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
            mStartYear = year;
            mStartMonth = monthOfYear;
            mStartDay = dayOfMonth;

            mStartDateTextView.setText(Utilities.dateToString(Utilities.getCalendar(year, monthOfYear, dayOfMonth, 0, 0, 0).getTime()));
        }
    };

    private DatePickerDialog.OnDateSetListener endDatePickerListner = new DatePickerDialog.OnDateSetListener() {
        @Override
        public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
            mEndYear = year;
            mEndMonth = monthOfYear;
            mEndDay = dayOfMonth;

            mEndDateTextView.setText(Utilities.dateToString(Utilities.getCalendar(year, monthOfYear, dayOfMonth, 0, 0, 0).getTime()));
        }
    };

    private void populateListView(List<TicketModel> ticketList){

        if ((ticketList != null) && (ticketList.size() > 0)) {
            mEndOfDayReportListAdater = new EndOfDayReportListAdapter(this, ticketList);
            mListView.setAdapter(mEndOfDayReportListAdater);
        }else{
            mListView.setAdapter(null);
        }
    }

    public void showStartCalendarDialog(View view){
        Dialog calendarDialog = new DatePickerDialog(this, THEME_HOLO_LIGHT, startDatePickerListner, mStartYear, mStartMonth, mStartDay);
        calendarDialog.show();
    }

    public void showEndCalendarDialog(View view){
        Dialog calendarDialog = new DatePickerDialog(this, THEME_HOLO_LIGHT, endDatePickerListner, mEndYear, mEndMonth, mEndDay);
        calendarDialog.show();
    }

    public void findNotices(View view) {
        Date startDate = Utilities.getCalendar(mStartYear, mStartMonth, mStartDay, 0, 0, 0).getTime();
        Date endDate = Utilities.getCalendar(mEndYear, mEndMonth, mEndDay, 0, 0, 0).getTime();

        mTicketList = getTicketList(startDate, endDate);

        populateListView(mTicketList);
    }

    public List<TicketModel> getTicketList(Date startDate, Date endDate) {

        List<TicketModel> ticketList = new ArrayList<>();

        try {
            List<HandWrittenModel> handWrittenList = null;
            handWrittenList = HandWrittenRepository.getTicketsByDateRange(startDate, endDate);

            HandWrittenToTicketModel handWrittenToTicketModel = new HandWrittenToTicketModel();

            for(HandWrittenModel handWritten: handWrittenList){
                ticketList.add(handWrittenToTicketModel.handWrittenToTicket(handWritten));
            }

            Collections.sort(ticketList, new Comparator<TicketModel>() {
                @Override
                public int compare(TicketModel lhs, TicketModel rhs) {
                    try {
                        if ((lhs != null) && (rhs != null)) {
                            return lhs.getInfringement().getOffenceDate().compareTo(rhs.getInfringement().getOffenceDate());
                        } else {
                            return 0;
                        }
                    } catch (Exception e) {
                        //MessageManager.showMessage(Utilities.exceptionMessage(e, "EndOfDayReportActivity::getTicketList()"), ErrorSeverity.None);
                        return 0;
                    }
                }
            });

            return ticketList;

        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "EndOfDayReportActivity::getTicketList()"), ErrorSeverity.None);
        }

        return null;
    }

    public void printEndOfDayReport(View view) {

        if ((mTicketList == null) || (mTicketList.size() == 0)){
            MessageManager.showMessage(App.getContext().getResources().getString(R.string.end_of_day_report_nothing_to_print), ErrorSeverity.None);
            return;
        }

        String details = SessionModel.getInstance().getPrinterMacAddress();
        if (TextUtils.isEmpty(details)){
            MessageManager.showMessage(App.getContext().getResources().getString(R.string.end_of_day_report_print_not_found), ErrorSeverity.None);
            return;
        }
        EndOfDayReport printEndOfDayReport = new EndOfDayReport(details, this, this);

        printEndOfDayReport.print(
                false,
                mTicketList,
                mStartDateTextView.getText().toString(),
                mEndDateTextView.getText().toString());
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

    }
}
