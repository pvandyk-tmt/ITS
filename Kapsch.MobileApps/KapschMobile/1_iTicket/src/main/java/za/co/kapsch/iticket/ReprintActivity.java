package za.co.kapsch.iticket;

import android.app.DatePickerDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
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

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Printer.HandWrittenSlip;
import za.co.kapsch.iticket.Printer.HandWrittenSlipMockup;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.HandWrittenToTicketModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

import static android.app.AlertDialog.THEME_HOLO_LIGHT;

public class ReprintActivity extends AppCompatActivity implements DialogInterface.OnClickListener, IAsyncProcessCallBack{

    private DocumentType mDocumentType;

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
    private HandWrittenModel mHandWritten;

    private static final String SpeedPlaceHolder = "[SPEED]";

    private DatePickerDialog.OnDateSetListener datePickerListner = new DatePickerDialog.OnDateSetListener() {
        @Override
        public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
            mYear = year;
            mMonth = monthOfYear;
            mDay = dayOfMonth;
            mSelectedDate = Utilities.getDate(year, monthOfYear, dayOfMonth, 0, 0, 0);

            try {
                if (mDocumentType == DocumentType.RoadSideDriver) {
                    List<HandWrittenModel> handWrittenList = HandWrittenRepository.getByDate(mSelectedDate);
                    populateListView(handWrittenList);
                }
            } catch (SQLException e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::onDateSet()"), ErrorSeverity.Medium);
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_reprint);

        Intent intent = getIntent();
        mDocumentType = DocumentType.RoadSideDriver.fromInteger(intent.getIntExtra(Constants.DOCUMENT_TYPE, -1));

        mListView = (ListView)findViewById(R.id.listView);

        Calendar calendar = Calendar.getInstance();
        mYear = calendar.get(Calendar.YEAR);
        mMonth = calendar.get(Calendar.MONTH);
        mDay = calendar.get(Calendar.DAY_OF_MONTH);

        mIdNumber = (TextView)findViewById(R.id.idNumber);
        mName = (TextView)findViewById(R.id.name);
        mSurname = (TextView)findViewById(R.id.surname);
        mPrintButton = (Button)findViewById(R.id.printButton);
        mOffenceDate = (TextView)findViewById(R.id.offenceDate);
        mNotPrintedTextView = (TextView)findViewById(R.id.notPrintedTextView);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        LayoutInflater inflater = getLayoutInflater();
        ViewGroup header = (ViewGroup)inflater.inflate(R.layout.reprint_header, mListView, false);
        mListView.addHeaderView(header);

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener()
        {
            @Override
            public void onItemClick(AdapterView<?> parent, final View view, int position, long id)
            {
                try {
                    view.setSelected(true);

                    if (mDocumentType == DocumentType.RoadSideDriver){
                        mHandWritten = (HandWrittenModel) mListView.getItemAtPosition(position);
                        mIdNumber.setText(mHandWritten.getIdentificationNumber());
                        mName.setText(mHandWritten.getFirstName());
                        mSurname.setText(mHandWritten.getSurname());
                        mOffenceDate.setText(Utilities.dateTimeToString(mHandWritten.getOffenceDate()));
                    }
                }catch (Exception e){
                    String error = e.getMessage();
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::mListView.setOnItemClickListener()"), ErrorSeverity.High);
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
//                   case MotionEvent.ACTION_UP:
//                       if (isOnClick) {
//                           return false;
//                       }
                        break;
                    case MotionEvent.ACTION_MOVE:
                        if (isOnClick && (Math.abs(mDownX - event.getX()) > SCROLL_THRESHOLD ||  Math.abs(mDownY - event.getY()) > SCROLL_THRESHOLD)) {
                            //isOnClick = false;
                            mListView.setSelector(android.R.color.transparent);
//                            mSection56ModelItem = null;
//                            mSection341ModelItem = null;
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

        switch(mDocumentType) {
            case RoadSideDriver:
                populateListView();
                break;
        }
    }

    public Button getPrintButton(){
        return mPrintButton;
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

    private void populateListView(){
        try {
            List<HandWrittenModel> handWrittenList = HandWrittenRepository.getNonCancelled();
            populateListView(handWrittenList);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "Populate56ListView()"), ErrorSeverity.Low);
        }
    }

    private void populateListView(List<HandWrittenModel> handWrittenList){
        if (handWrittenList.size() < 1) return;
        ReprintListAdapter adapter = new ReprintListAdapter(this, handWrittenList);
        mListView.setAdapter(adapter);
    }

    private void setEvidence(TicketModel ticket, String ticketNumber, EvidenceType evidenceType){
        try{
            EvidenceModel evidence  = EvidenceRepository.getEvidence(ticketNumber, evidenceType);
            if (evidence != null) {
                switch (evidenceType) {
                    case PersonSignature:
                        ticket.getOffender().setSignature(evidence.getEvidence());
                        break;
                    case OfficerSignature:
                        ticket.getUser().setSignature(evidence.getEvidence());
                        break;
                    case OffenderPhoto:
                        ticket.getOffender().setPhoto(Utilities.byteArrayToBitmap(evidence.getEvidence()));
                }
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::getEvidence()"), ErrorSeverity.Medium);
        }
    }

    public void printTicketClick(View view) {

        TicketModel ticket = null;

        if ((mDocumentType == DocumentType.RoadSideDriver)) {
            ticket = new HandWrittenToTicketModel().handWrittenToTicket(mHandWritten); //section56ToTicket(mSection56ModelItem);
            printTicket(ticket);
        }
    }

//    private void printTicket(TicketModel ticket){
//
//        try {
//            String[] details = Utilities.getPrinterDetails();
//            if (details.length != 2) {
//                Utilities.displayOkMessage("Printer details not found, Please register a printer.", this);
//                return;
//            }
//
//            ticket.setDocumentType(mDocumentType);
//
//            if (mDocumentType == mDocumentType.RoadSide) {
//                mPrintButton.setEnabled(false);
//
//                HandWrittenSlip handWrittenSlip = new HandWrittenSlip(details[1], this, this);
//                handWrittenSlip.print(false, ticket);
//            }
//
//        } catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "printTicket()"), ErrorSeverity.High);
//            return;
//        }
//    }

    private void printTicket(TicketModel ticket) {

        try {
            if (TextUtils.isEmpty(SessionModel.getInstance().getPrinterMacAddress()) == false) {

                ticket.setDocumentType(mDocumentType);
                if (mDocumentType == mDocumentType.RoadSideDriver) {
                    mPrintButton.setEnabled(false);

                    HandWrittenSlip handWrittenSlip = new HandWrittenSlip(SessionModel.getInstance().getPrinterMacAddress(), this, this);
                    //HandWrittenSlipMockup handWrittenSlip = new HandWrittenSlipMockup(SessionModel.getInstance().getPrinterMacAddress(), this, this);

                    handWrittenSlip.print(false, ticket);
                }
            }else{
                MessageManager.showMessage(getResources().getString(R.string.printer_is_not_configured), ErrorSeverity.None);
                return;
            }
        }catch(Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "printTicket()"), ErrorSeverity.High);
            return;
        }
    }

    public void showCalendarDialog(View view){
        Dialog calendarDialog = new DatePickerDialog(this, THEME_HOLO_LIGHT, datePickerListner, mYear, mMonth, mDay);
        calendarDialog.show();
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {

        if (mDocumentType == DocumentType.RoadSideDriver){
            if (which == Constants.YES) {
                if (mHandWritten != null) {
                    mHandWritten.setPrinted(true);
                    boolean rowsUpdated = updateHandWritten(mHandWritten);
                    if (rowsUpdated == false) {
                        Utilities.displayOkMessage("Database update failed, please reprint.", this);
                    }
                } else {
                    MessageManager.showMessage("ReprintActivity::onClick() - mSection56Model is null", ErrorSeverity.Medium);
                }
            }
//        }else if ((mTicketType == TicketType.Section341) || (mTicketType == TicketType.OpusSection341)){
//            if (which == Constants.YES) {
//                if (mSection341ModelItem != null) {
//                    mSection341ModelItem.setPrinted(true);
//                    boolean rowsUpdated = updateTicketModel(mSection341ModelItem);
//                    if (rowsUpdated == false) {
//                        Utilities.displayOkMessage("Database update failed, please reprint.", this);
//                    }
//                } else {
//                    MessageManager.showMessage("ReprintActivity::onClick() - mSection341Model is null", ErrorSeverity.Medium);
//                }
//            }
        }

        switch(mDocumentType) {
            case RoadSideDriver: populateListView();
                break;
        }
    }

    private boolean updateHandWritten(HandWrittenModel handWritten){
        try {
            return HandWrittenRepository.update(handWritten);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::updateHandWritten"), ErrorSeverity.High);
        } catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::updateHandWritten"), ErrorSeverity.High);
        }

        return false;
    }
//
//    private boolean updateTicketModel(Section341Model section341Model){
//        try {
//            return Section341Repository.update(section341Model);
//        } catch (SQLException e) {
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::UpdateTicketModel(341) 1"), ErrorSeverity.High);
//        } catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::UpdateTicketModel(341) 2"), ErrorSeverity.High);
//        }
//
//        return false;
//    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.Medium);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        Utilities.displayDecisionMessage("Did the slip print successfully?", this);
        mPrintButton.setEnabled(true);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
