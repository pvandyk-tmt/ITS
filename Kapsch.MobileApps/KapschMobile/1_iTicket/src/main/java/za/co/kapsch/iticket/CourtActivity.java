package za.co.kapsch.iticket;

import android.app.DatePickerDialog;
import android.app.Dialog;
import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.DatePicker;
import android.widget.Spinner;
import android.widget.TextView;

import java.sql.SQLException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.List;

import za.co.kapsch.iticket.Models.CourtDetailModel;
import za.co.kapsch.iticket.orm.CourtDetailRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

import static android.app.AlertDialog.*;

public class CourtActivity extends AppCompatActivity {

    private int mYear = 0;
    private int mMonth = 0;
    private int mDay = 0;
    private TextView mDistrictTextView;
    private TextView mCourtDateTextView;
    private Spinner mCourtSpinner;
    private CourtDetailModel mCourt;
    //private boolean mVerificationMode;

    private DatePickerDialog.OnDateSetListener datePickerListner
            = new DatePickerDialog.OnDateSetListener() {
        @Override
        public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
            mYear = year;
            mMonth = monthOfYear;
            mDay = dayOfMonth;

            setCourtDate();
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_court);

        mDistrictTextView = (TextView) findViewById (R.id.districtNameTextView);
        mCourtDateTextView = (TextView) findViewById(R.id.courtDateTextView);

//        try {
//            mDistrictTextView.setText(DistrictRepository.getDistrict().getName());
//        } catch (SQLException e) {
//            MessageManager.showMessage(Utilities.exceptionMessage(e,"CourtActivity::onCreate()"), ErrorSeverity.High);
//        } catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e,"CourtActivity::onCreate()"), ErrorSeverity.High);
//        }

        Intent intent = getIntent();
        CourtDetailModel court = intent.getParcelableExtra(Constants.COURT);
        //mVerificationMode = intent.getBooleanExtra(Constants.COURT_VERIFICATION, false);

        mCourtSpinner = (Spinner) findViewById(R.id.spinnerCourt);
        List<CourtDetailModel> courts = getCourts();
        if (courts.size() > 0) {
            ArrayAdapter adapter = new ArrayAdapter<CourtDetailModel>(this, R.layout.spinner_item, courts);
            mCourtSpinner.setAdapter(adapter);
        }

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.activity_court_title)));

        mCourtSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                mCourt = (CourtDetailModel) parent.getItemAtPosition(position);
                Calendar calendar = Utilities.getCalendar(mYear, mMonth, mDay, 8, 30, 0);
                mCourt.setDate(calendar.getTime());
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        setCourtDefaultDate(court);
        setSelectedCourt(court);
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

    private List<CourtDetailModel> getCourts() {
        try {
            return CourtDetailRepository.getCourts();
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "CourtActiviry::getCourts()"), ErrorSeverity.High);
            //Toast.makeText(this, e.getMessage(), Toast.LENGTH_SHORT).show();
            return null;
        }
    }

    public void saveCourt(View view){
        mCourt.setDate(Utilities.getDate(mYear, mMonth, mDay, 8, 30, 0));
        returnCourtData(mCourt);
    }

    public void returnCourtData(CourtDetailModel court){
        Intent intent = new Intent();
        intent.putExtra(Constants.COURT, court);
        setResult(RESULT_OK, intent);
        finish();
    }

    private void setCourtDate(){
        Calendar calendar = Utilities.getCalendar(mYear, mMonth, mDay, 0, 0, 0);
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.DATE_FORMAT);
        mCourtDateTextView.setText(simpleDateFormat.format(calendar.getTime()));
    }

    private void setCourtDefaultDate(CourtDetailModel court){

        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(Constants.DATE_FORMAT);

        if (court == null) {
            Calendar calendar = Utilities.getCalendarAddDays(60);
            mYear = calendar.get(Calendar.YEAR);
            mMonth = calendar.get(Calendar.MONTH);
            mDay = calendar.get(Calendar.DAY_OF_MONTH);

            mCourtDateTextView.setText(simpleDateFormat.format(calendar.getTime()));
            return;
        }

        Calendar calendar = Utilities.getCalendarDate(court.getDate());

        mYear = calendar.get(Calendar.YEAR);
        mMonth = calendar.get(Calendar.MONTH);
        mDay = calendar.get(Calendar.DAY_OF_MONTH);

        mCourtDateTextView.setText(simpleDateFormat.format(court.getDate()));
    }

    private void setSelectedCourt(CourtDetailModel court){
        if(court == null) return;

        int position = Utilities.indexOf(mCourtSpinner.getAdapter(), court.getName());

        mCourtSpinner.setSelection(position);
     }

    public void showCalendarDialog(View view){
        Dialog calendarDialog = new DatePickerDialog(this, THEME_HOLO_LIGHT, datePickerListner, mYear, mMonth, mDay);
        //Dialog calendar = new DatePickerDialog(this, R.style.datePickerTheme, datePickerListner, mYear, mMonth, mDay);

        calendarDialog.show();
    }

    @Override
    public void onBackPressed(){
        //if (mVerificationMode == true){
            super.onBackPressed();
        //}
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
