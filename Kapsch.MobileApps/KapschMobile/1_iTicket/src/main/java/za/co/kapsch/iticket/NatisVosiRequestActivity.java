package za.co.kapsch.iticket;

import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Spannable;
import android.text.SpannableStringBuilder;
import android.text.TextUtils;
import android.text.style.StyleSpan;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.EnatisVehicle;
import za.co.kapsch.iticket.Models.EnatisVehicleResponse;
import za.co.kapsch.iticket.Models.VosiResponse;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Utilities;

public class NatisVosiRequestActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private boolean mBusy;
    private TextView mResultTitleTextView;
    private Button mNatisButton;
    private Button mSapsButton;
    private TextView mVosiEnatisTextView;
    private EditText mLicenceNumberEditText;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_natis_vosi_request);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.vosi_enatis_query_heading)));

        mBusy = false;
        mResultTitleTextView = (TextView) findViewById(R.id.resultTitleTextView);
        mNatisButton = (Button) findViewById(R.id.natisButton);
        mSapsButton = (Button) findViewById(R.id.sapsButton);
        mVosiEnatisTextView = (TextView) findViewById(R.id.vosiEnatisTextView);
        mLicenceNumberEditText = (EditText) findViewById(R.id.licenceNoEditText);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);
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

    public void sapsRequest(View view){
        mBusy = true;
        mSapsButton.setEnabled(false);
        mNatisButton.setEnabled(false);
        mVosiEnatisTextView.setText(Constants.EMPTY_STRING);
        DataServiceRequest.vosiRequest(this, this, mLicenceNumberEditText.getText().toString());
    }

    public void enatisRequest(View view){
        mBusy = true;
        mNatisButton.setEnabled(false);
        mSapsButton.setEnabled(false);
        mVosiEnatisTextView.setText(Constants.EMPTY_STRING);
        DataServiceRequest.eNatisVehicleRequest(this, this, mLicenceNumberEditText.getText().toString());
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try {
            if (asyncResultModel == null){
                return;
            }

            if (asyncResultModel.getObject() == null) {
                MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
                return;
            }

            switch (asyncResultModel.getProcessId()) {
                case Constants.PROCESS_ID_VOSI_LOOKUP:
                    VosiResponse vosiResponse = (VosiResponse) asyncResultModel.getObject();
                    mVosiEnatisTextView.setText(vosiResponse.getMessage());
                    mResultTitleTextView.setText(getResources().getString(R.string.saps_query_text));
                    break;
                case Constants.PROCESS_ID_ENATIS_VEHICLE_LOOKUP:
                    EnatisVehicleResponse enatisVehicleResponse = (EnatisVehicleResponse) asyncResultModel.getObject();
                    mVosiEnatisTextView.setText(validateEnatisVehicleResponse(enatisVehicleResponse));
                    mResultTitleTextView.setText(getResources().getString(R.string.enatis_query_text));
                    break;
            }

            if (TextUtils.isEmpty(mVosiEnatisTextView.getText())) {
                mResultTitleTextView.setText(getResources().getString(R.string.saps_enatis_try_again_text));
            }


        }catch (IllegalStateException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "NatisVosiRequestActivity::finishedCallBack()"), ErrorSeverity.Medium);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "NatisVosiRequestActivity::finishedCallBack()"), ErrorSeverity.Medium);
        }
        finally {
            mNatisButton.setEnabled(true);
            mSapsButton.setEnabled(true);
            mBusy = false;
        }
    }

    private SpannableStringBuilder validateEnatisVehicleResponse(EnatisVehicleResponse enatisVehicleResponse){

        EnatisVehicle enatisVehicle = null;

        if (enatisVehicleResponse.Errors.length == 0) {
            if (enatisVehicleResponse.Vehicles.length > 0) {
                enatisVehicle = enatisVehicleResponse.Vehicles[0];
                return setEnatisText(enatisVehicle);
            }
        }

        return null;
    }

    private SpannableStringBuilder setEnatisText(EnatisVehicle enatisVehicle){

        SpannableStringBuilder stringBuilder = new SpannableStringBuilder();

        appendText(stringBuilder, getString(R.string.Vln), enatisVehicle.getLicenceDiscNumber());
        appendText(stringBuilder, getString(R.string.LicenceDiscNumber), enatisVehicle.getVln());
        appendText(stringBuilder, getString(R.string.Gvm), enatisVehicle.getGvm());
        appendText(stringBuilder, getString(R.string.Make), enatisVehicle.getMake());
        appendText(stringBuilder, getString(R.string.MakeCode), enatisVehicle.getMakeCode());
        appendText(stringBuilder, getString(R.string.Model), enatisVehicle.getModel());
        appendText(stringBuilder, getString(R.string.ModelCode), enatisVehicle.getModelCode());
        appendText(stringBuilder, getString(R.string.ColorCode), enatisVehicle.getColorCode());
        appendText(stringBuilder, getString(R.string.Color), enatisVehicle.getColor());
        appendText(stringBuilder, getString(R.string.Category), enatisVehicle.getCategory());
        appendText(stringBuilder, getString(R.string.Description), enatisVehicle.getDescription());
        appendText(stringBuilder, getString(R.string.VehicleState), enatisVehicle.getVehicleState());
        appendText(stringBuilder, getString(R.string.RwState), enatisVehicle.getRwState());
        appendText(stringBuilder, getString(R.string.Vin), enatisVehicle.getVin());
        appendText(stringBuilder, getString(R.string.EngineNumber), enatisVehicle.getEngineNumber());
        appendText(stringBuilder, getString(R.string.RegistrationNumber), enatisVehicle.getRegistrationNumber());
        appendText(stringBuilder, getString(R.string.CategoryCode), enatisVehicle.getCategoryCode());
        appendText(stringBuilder, getString(R.string.DescriptionCode), enatisVehicle.getDescriptionCode());
        appendText(stringBuilder, getString(R.string.RwStateCode), enatisVehicle.getRwStateCode());

        if (enatisVehicle.getOwner() != null) {
            appendText(stringBuilder, getString(R.string.Owner), enatisVehicle.getOwner().getOwner());
            appendText(stringBuilder, getString(R.string.IdDocumentTypeCode), enatisVehicle.getOwner().getIdDocumentTypeCode());
            appendText(stringBuilder, getString(R.string.IdNumber), enatisVehicle.getOwner().getIdNumber());
            appendText(stringBuilder, getString(R.string.HomePhoneNumber), enatisVehicle.getOwner().getHomePhoneNumber());
            appendText(stringBuilder, getString(R.string.WorkPhoneNumber), enatisVehicle.getOwner().getWorkPhoneNumber());
            appendText(stringBuilder, getString(R.string.FaxNumber), enatisVehicle.getOwner().getFaxNumber());
            appendText(stringBuilder, getString(R.string.CellularNumber), enatisVehicle.getOwner().getCellularNumber());
            appendText(stringBuilder, getString(R.string.EmailAddresss), enatisVehicle.getOwner().getEmailAddresss());
            appendText(stringBuilder, getString(R.string.LastName), enatisVehicle.getOwner().getLastName());
            appendText(stringBuilder, getString(R.string.FirstName), enatisVehicle.getOwner().getFirstName());
            appendText(stringBuilder, getString(R.string.Initials), enatisVehicle.getOwner().getInitials());
        }

        return stringBuilder;
    }

    private SpannableStringBuilder makeParitalTextBold(String text, int boldStart, int boldLength){
        final SpannableStringBuilder spannableStringBuilder = new SpannableStringBuilder(text);
        final StyleSpan styleSpan = new StyleSpan(android.graphics.Typeface.BOLD); // Span to make text bold
        spannableStringBuilder.setSpan(styleSpan, boldStart, boldLength, Spannable.SPAN_INCLUSIVE_INCLUSIVE); // make first 4 characters Bold
        return spannableStringBuilder;
    }

    private void appendText(SpannableStringBuilder stringBuilder, String caption, String value){
        if (TextUtils.isEmpty(value) == false) {
            stringBuilder.append(makeParitalTextBold(String.format("%s %s\n", caption, value), 0, caption.length()));
        }
    }

    private String validateValue(String value){
        return TextUtils.isEmpty(value) ? getString(R.string.not_available_message) : value;
    }

    @Override
    public void onBackPressed(){
        if (mBusy == false) {
            super.onBackPressed();
        }
   }
}
