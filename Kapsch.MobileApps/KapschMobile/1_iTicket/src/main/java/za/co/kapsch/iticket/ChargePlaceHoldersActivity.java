package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.InputType;
import android.text.TextWatcher;
import android.view.Gravity;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TableRow;
import android.widget.TextView;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

public class ChargePlaceHoldersActivity extends AppCompatActivity {

    private static final int ViewMinWidth = 300;
    private static final int FontSize_20 = 15;
    private static final String Colon = ":";

    private Button mOkButton;
    private int mSpeed;
    private String mVehicleLicenceNumber;
    private String mChargePrintDescription;
    private LinearLayout mLinearLayout;
    private List<String> mPlaceHolders;
    private TicketModel mTicket;

    private Map<String, EditText> mViewMap = new HashMap<String, EditText>();

    private TextWatcher mAddTextChangedListener = (new TextWatcher() {
        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {

        }

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

        }

        @Override
        public void afterTextChanged(Editable s) {
            validateUserInterface();
        }
    });

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_charge_place_holders);

        Intent intent = getIntent();
        mChargePrintDescription = intent.getStringExtra(Constants.CHARGE_PRINT_DESC);
        mTicket = intent.getParcelableExtra(Constants.TICKET_MODEL);

        mPlaceHolders = Utilities.getRegexMatches(mChargePrintDescription, Constants.REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN);

        removeAllDuplicates();

        mLinearLayout = (LinearLayout) findViewById(R.id.chargePlaceHolderlinearLayout);
        mOkButton = (Button) findViewById(R.id.okButton);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.activity_charge_place_holder_title)));

        mOkButton.setEnabled(false);

        addPromptFields();
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

    private void removeAllDuplicates(){
        while(removeDuplicates() == false);
    }

    private boolean removeDuplicates(){

        for (int i = 0; i < mPlaceHolders.size(); i++) {
            for (int j = i+1; j < mPlaceHolders.size(); j++) {
                if (mPlaceHolders.get(i).equals(mPlaceHolders.get(j))) {
                    mPlaceHolders.remove(j);
                    return false;
                }
            }
         }

        return true;
    }

    private void addPromptFields(){

        int controlNumber = 0;

        for(String placeHolder: mPlaceHolders){
            if (placeHolder.toUpperCase().toString().equals(Constants.ZONE_PLACE_HOLDER)) continue;

            controlNumber++;

            int intputType = placeHolder.toUpperCase().equals(Constants.SPEED_PLACE_HOLDER) ? InputType.TYPE_CLASS_NUMBER|InputType.TYPE_NUMBER_FLAG_DECIMAL : InputType.TYPE_CLASS_TEXT;

            mLinearLayout.addView(getLinearLayout(controlNumber, placeHolder, intputType));
        }
    }

    private LinearLayout getLinearLayout(int id, String placeHolder, int inputType){

        LinearLayout linearLayout = new LinearLayout(this);

        linearLayout.setLayoutParams(new TableRow.LayoutParams(TableRow.LayoutParams.WRAP_CONTENT, TableRow.LayoutParams.WRAP_CONTENT));
        linearLayout.setOrientation(LinearLayout.HORIZONTAL);

        linearLayout.addView(getTextView(placeHolder));

        EditText editText = getEditText(id + 1, inputType);
        mViewMap.put(placeHolder, editText);
        linearLayout.addView(editText);

        return linearLayout;
    }

    private View getTextView(String placeHolder){
        TextView textView = new TextView(this);

        String label = placeHolder.replace("[", Constants.EMPTY_STRING).replace("]", Colon);
        textView.setText(label);
        textView.setGravity(Gravity.RIGHT);
        textView.setTextSize(FontSize_20);
        TableRow.LayoutParams layoutParams = new TableRow.LayoutParams(300, TableRow.LayoutParams.WRAP_CONTENT, 1.0f);
        textView.setLayoutParams(layoutParams);

        return textView;
    }

    private EditText getEditText(int id, int inputType){

        EditText editText = new EditText(this);
        editText.setMinimumWidth(ViewMinWidth);
        editText.setTextSize(FontSize_20);
        editText.setInputType(inputType);
        TableRow.LayoutParams layoutParams = new TableRow.LayoutParams(TableRow.LayoutParams.WRAP_CONTENT, TableRow.LayoutParams.WRAP_CONTENT, 1.0f);
        editText.setLayoutParams(layoutParams);
        editText.setId(id);

        editText.addTextChangedListener(mAddTextChangedListener);

        return editText;
    }

    private boolean insertValuesForPlaceholders()
    {
        mSpeed = 0;
        mVehicleLicenceNumber = Constants.EMPTY_STRING;

        for(String placeHolder: mPlaceHolders)
        {
            EditText editText = mViewMap.get(placeHolder);

            if (editText == null) continue;

            if (placeHolder.toUpperCase().equals(Constants.SPEED_PLACE_HOLDER)){
                try {
                    mSpeed = Integer.parseInt(editText.getText().toString());
                }catch (Exception e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "insertValuesForPlaceholders()"), ErrorSeverity.Low);
                    return false;
                }
            }
            if (placeHolder.toUpperCase().equals(Constants.VEHREG_PLACE_HOLDER)){
                mVehicleLicenceNumber = editText.getText().toString();
            }

            mChargePrintDescription = mChargePrintDescription.replace(placeHolder, editText.getText().toString());
        }

        return true; //String.format("%s%s%s", mChargeDescription, Constants.Split, speed);
    }

    private void validateUserInterface(){

        boolean validated = false;

        for(String placeHolder: mPlaceHolders)
        {
            EditText editText = mViewMap.get(placeHolder);

            if (editText == null) continue;

            validated = (editText.getText().toString().equals(Constants.EMPTY_STRING) == false);

            if (validated == false) break;
        }

        mOkButton.setEnabled(validated);
    }

    public void returnChargeDescription(View view){

        boolean result = insertValuesForPlaceholders();

        if (result == true) {
            Intent intent = new Intent();
            intent.putExtra(Constants.CHARGE_SPEED, mSpeed);
            intent.putExtra(Constants.CHARGE_VEHICLE_LICENCE_NUMBER, mVehicleLicenceNumber);
            intent.putExtra(Constants.CHARGE_PRINT_DESC, mChargePrintDescription);
            setResult(RESULT_OK, intent);
            finish();
        }
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {

        for(String placeHolder: mPlaceHolders)
        {
            EditText editText = mViewMap.get(placeHolder);

            if (editText == null) continue;

            savedInstanceState.putString(placeHolder, editText.getText().toString());
        }

        super.onSaveInstanceState(savedInstanceState);
    }

    @Override
    public void onRestoreInstanceState(Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);

        for(String placeHolder: mPlaceHolders)
        {
            TextView textView = mViewMap.get(placeHolder);

            if (textView == null) continue;

            textView.setText(savedInstanceState.getString(placeHolder));
        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
