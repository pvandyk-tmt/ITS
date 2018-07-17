package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Models.CancellationReasonModel;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.orm.CancellationReasonRepository;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

public class CancellationReasonActivity extends AppCompatActivity {

    private static final int MinimumValidReasonLength = 5;
    private CancellationReasonModel mCancellationReason;
    private TextView mOtherReasonTextView;
    private EditText mOtherReasonEditText;
    private Button mOkButton;
    private TicketModel mTicket;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_cancellation_reason);

        Intent intent = getIntent();
        mTicket = intent.getParcelableExtra(Constants.TICKET_MODEL);

        mOkButton = (Button) findViewById(R.id.okButton);
        mOtherReasonTextView = (TextView) findViewById (R.id.provideReasonTextView);
        mOtherReasonEditText = (EditText) findViewById (R.id.provideReasonEditText);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        Spinner cancellationReasonSpinner = (Spinner) findViewById(R.id.cancellationReasonSpinner);

        List<CancellationReasonModel> cancellationReasonList = getCancellationReason();

        if (cancellationReasonList != null) {
            if (cancellationReasonList.size() > 0) {
                ArrayAdapter adapter = new ArrayAdapter<CancellationReasonModel>(this, R.layout.spinner_item, getCancellationReason());
                cancellationReasonSpinner.setAdapter(adapter);
            }
        }

        cancellationReasonSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                mCancellationReason = (CancellationReasonModel) parent.getItemAtPosition(position);
                validateUserInterface();
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });
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

    private List<CancellationReasonModel> getCancellationReason() {
        try {
            return CancellationReasonRepository.getCancellationReasons(this);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getCancellationReason()"), ErrorSeverity.Low);
            return null;
        }
    }

    private void otherReasonFields(boolean show){
        mOtherReasonTextView.setVisibility(show ? View.VISIBLE : View.INVISIBLE);
        mOtherReasonEditText.setVisibility(show ? View.VISIBLE : View.INVISIBLE);
    }

    private void validateUserInterface(){

        if (mCancellationReason != null){
            mOkButton.setEnabled(true);
        }

        if (mCancellationReason.getReason().equals(getResources().getString(R.string.activity_cancellation_reason_other_text))){
            otherReasonFields(true);
            return;
        }

        otherReasonFields(false);
    }

    private void returnCancellationReason(boolean cancelWizard){
        Intent intent = new Intent();
        //intent.putExtra(Constants.WIZARD_CANCELLATION, cancelWizard);
        setResult(RESULT_OK, intent);
        finish();
    }

    public void okButtonClick(View view){

        if (mOtherReasonEditText.getText().toString().equals(Constants.EMPTY_STRING) == false) {
            mCancellationReason.setId(0);
            mCancellationReason.setReason(mOtherReasonEditText.getText().toString());
        }

        if (mCancellationReason.getReason().toString().length() <= MinimumValidReasonLength ){
            MessageManager.showMessage(getResources().getString(R.string.activity_cancellation_reason_provide_valid_reason_message), ErrorSeverity.None);
            return;
        }

        if (mTicket.getDocumentType() == DocumentType.RoadSideDriver) {
            HandWrittenModel handWritten = new HandWrittenModel();
            handWritten.setHandWritten(mTicket, Utilities.getDeviceId(), true, mCancellationReason.getReason());

            if (mTicket.getPersisted() == false) {
                if (saveHandWrittenModel(handWritten) == true) {
                    deleteEvidence(mTicket.getInfringement().getTicketNumber());
                    mTicket.setPersisted(true);
                }
            }
        }

        returnCancellationReason(true);
    }

    private boolean saveHandWrittenModel(HandWrittenModel handWritten){
        try {
            HandWrittenRepository.create(handWritten);
            return true;
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "CancellationReasonActivity::saveHandWrittenModel"), ErrorSeverity.High);
            return false;
        } catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "CancellationReasonActivity::saveHandWrittenModel"), ErrorSeverity.High);
            return false;
        }
    }

    private boolean deleteEvidence(String ticketNumber){

        try {
            List<EvidenceModel> evidenceList = EvidenceRepository.getEvidenceByTicketNumber(ticketNumber);
            for(EvidenceModel evidence : evidenceList){
                EvidenceRepository.delete(evidence);
            }
            return true;
        }catch(Exception e){
            return false;
        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }

//    private void setOpusNoticeNumberToIssued(String noticeNumber){
//        try {
//            if (ConfigurationModel.TICKET_PROCESSING_SYSTEM == TicketProcessingSystem.TmtCentral) return;
//
//            if (OpusNoticeNumberRepository.updateStatusToIssued(noticeNumber) == false){
//                MessageManager.showMessage(this, "setOpusNoticeNumberToIssued()-failed to set opus notice number to issued.", ErrorSeverity.High);
//            }
//        } catch (SQLException e) {
//            MessageManager.showMessage(this, Utilities.exceptionMessage(e, "setOpusNoticeNumberToIssued()" ), ErrorSeverity.High);
//        }
//    }
}
