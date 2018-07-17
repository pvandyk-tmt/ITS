package za.co.kapsch.ipayment;

import android.app.DatePickerDialog;
import android.app.Dialog;
import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.ListView;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import za.co.kapsch.ipayment.Enums.BroadcastSource;
import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;
import za.co.kapsch.ipayment.General.DataServiceRequest;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.ipayment.Printer.Receipt;
import za.co.kapsch.ipayment.orm.TransactionRepository;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Utilities;

import static android.app.AlertDialog.THEME_HOLO_LIGHT;

public class ReceiptReprintActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private int mYear = 0;
    private int mMonth = 0;
    private int mDay = 0;

    private TextView mIdNumber;
    private TextView mName;
    private TextView mSurname;
    private TextView mOffenceDate;
    private Button mPrintButton;

    private Date mSelectedDate;

    private ListView mListView;
    private TextView mNotPrintedTextView;
    private TransactionModel mTransaction;

    private DatePickerDialog.OnDateSetListener datePickerListner = new DatePickerDialog.OnDateSetListener() {
        @Override
        public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
            mYear = year;
            mMonth = monthOfYear;
            mDay = dayOfMonth;
            mSelectedDate = Utilities.getDate(year, monthOfYear, dayOfMonth, 0, 0, 0);

            try {
                    List<TransactionModel> transactionList = TransactionRepository.getByDate(mSelectedDate);
                    populateListView(transactionList);

            } catch (Exception e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::onDateSet()"), ErrorSeverity.Medium);
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_receipt_reprint);

        Intent intent = getIntent();

        mListView = (ListView) findViewById(R.id.listView);

        Calendar calendar = Calendar.getInstance();
        mYear = calendar.get(Calendar.YEAR);
        mMonth = calendar.get(Calendar.MONTH);
        mDay = calendar.get(Calendar.DAY_OF_MONTH);

//        mIdNumber = (TextView) findViewById(R.id.idNumber);
//        mName = (TextView) findViewById(R.id.name);
//        mSurname = (TextView) findViewById(R.id.surname);
//        mPrintButton = (Button) findViewById(R.id.printButton);
//        mOffenceDate = (TextView) findViewById(R.id.offenceDate);
        //mNotPrintedTextView = (TextView) findViewById(R.id.notPrintedTextView);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        LayoutInflater inflater = getLayoutInflater();
        ViewGroup header = (ViewGroup) inflater.inflate(R.layout.receipt_reprint_header, mListView, false);
        mListView.addHeaderView(header);

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {
                try {
                    view.setSelected(true);

                    mTransaction = (TransactionModel) mListView.getItemAtPosition(position);
                    mIdNumber.setText(mTransaction.getCustomerIDNumber());
                    mName.setText(mTransaction.getCustomerFirstName());
                    mSurname.setText(mTransaction.getCustomerLastName());
                    mOffenceDate.setText(Utilities.dateTimeToString(mTransaction.getReceiptTimeStamp()));
                } catch (Exception e) {
                    String error = e.getMessage();
                }
            }
        });

        mListView.setOnTouchListener(new View.OnTouchListener() {

            private float mDownX;
            private float mDownY;
            private final float SCROLL_THRESHOLD = 10;
            private boolean isOnClick;

            @Override
            public boolean onTouch(View view, MotionEvent event) {

                switch (event.getAction() & MotionEvent.ACTION_MASK) {

                    case MotionEvent.ACTION_DOWN:
                        mDownX = event.getX();
                        mDownY = event.getY();
                        isOnClick = true;
                        break;
                    case MotionEvent.ACTION_CANCEL:
                        break;
                    case MotionEvent.ACTION_MOVE:
                        if (isOnClick && (Math.abs(mDownX - event.getX()) > SCROLL_THRESHOLD || Math.abs(mDownY - event.getY()) > SCROLL_THRESHOLD)) {
                            mListView.setSelector(android.R.color.transparent);
                            mIdNumber.setText(Constants.EMPTY_STRING);
                            mName.setText(Constants.EMPTY_STRING);
                            mSurname.setText(Constants.EMPTY_STRING);
                            mOffenceDate.setText(Constants.EMPTY_STRING);
                        }
                        break;
                    default:
                        break;
                }
                return false;
            }
        });

        populateListView();
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

    public void showCalendarDialog(View view){
        Dialog calendarDialog = new DatePickerDialog(this, THEME_HOLO_LIGHT, datePickerListner, mYear, mMonth, mDay);
        calendarDialog.show();
    }

    public void printReceipt(View view){

        String[] details = Utilities.getPrinterDetails();
        if (details.length != 2){

            Utilities.displayOkMessage("Printer details not found, Please register a printer.", this);
            return;
        }

        Receipt receipt = new Receipt(details[1], this, this);
        MessageManager.showMessage(getResources().getString(R.string.printing_receipt), ErrorSeverity.None);
        receipt.print(false, mTransaction);
    }

    private void populateListView(){
        try {
            List<TransactionModel> transaction = TransactionRepository.getByDate(Calendar.getInstance().getTime());
            populateListView(transaction);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PopulateListView()"), ErrorSeverity.Low);
        }
    }

    private void populateListView(List<TransactionModel> transactionList){

        if (transactionList.size() < 1) return;
        ReceiptReprintListAdapter adapter = new ReceiptReprintListAdapter(this, transactionList);
        mListView.setAdapter(adapter);
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {

            if (asyncResultModel == null) {
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case za.co.kapsch.ipayment.General.Constants.PROCESS_ID_ASYNC_PROCESS_PRINT:
                    break;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "finishedCallBack() - PROCESS_ID: %d"), ErrorSeverity.High);
            return;
        }
    }
}
