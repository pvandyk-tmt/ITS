package za.co.kapsch.iticket;


import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.TextView;

import za.co.kapsch.iticket.Enums.AddressType;
import za.co.kapsch.iticket.Google.GoogleAddressRefactor;
import za.co.kapsch.iticket.Google.GoogleGeoCodeReponse;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.iticket.Models.InfringementModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Utilities;


/**
 * A simple {@link Fragment} subclass.
 */
public class AddressCaptureFragmentEx extends Fragment implements IAsyncProcessCallBack {

    private static final String pipe = "|";
    private OffenderModel mOffender;
    private InfringementModel mInfringement;
    private EditText mAddressLineOneEditText;
    private EditText mAddressLineTwoEditText;
    private EditText mSuburbEditText;
    private EditText mCityEditText;
    private EditText mCodeEditText;
    private ImageButton mSearchButton;
    private String[] mAddressList;
    private AddressType mAddressType;
    private TextView mAddressLineOneTextView;
    private TextView mCodeTextView;
    private TextView mLineTwoTextView;
    private LinearLayout mLineOnelinearLayout;

    TextWatcher mTextWatcher = new TextWatcher() {
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
    };

    public AddressCaptureFragmentEx() {
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View rootView = inflater.inflate(R.layout.fragment_address_capture_layout_ex, container, false);

        mAddressLineOneEditText = (EditText)rootView.findViewById(R.id.addressLineOneEditText);
        mAddressLineTwoEditText = (EditText)rootView.findViewById(R.id.addressLineTwoEditText);
        mSuburbEditText = (EditText)rootView.findViewById(R.id.suburbEditText);
        mCityEditText = (EditText)rootView.findViewById(R.id.cityEditText);
        mCodeEditText = (EditText)rootView.findViewById(R.id.codeEditText);
        mSearchButton = (ImageButton) rootView.findViewById(R.id.searchButton);
        mOffender = wizardActivity().getTicketModel().getOffender();
        mInfringement = wizardActivity().getTicketModel().getInfringement();
        mAddressType = AddressType.fromInteger(getArguments().getInt(Constants.ADDRESS_TYPE, -1));
        mAddressLineOneTextView = (TextView)rootView.findViewById(R.id.addressLineOneTextView);
        mCodeTextView = (TextView)rootView.findViewById(R.id.codeTextView);
        mLineTwoTextView = (TextView)rootView.findViewById(R.id.lineTwoTextView);
        mLineOnelinearLayout = (LinearLayout)rootView.findViewById(R.id.lineOnelinearLayout);

        if(mAddressType == AddressType.Offence){
            mAddressLineOneEditText.setVisibility(View.INVISIBLE);
            mCodeEditText.setVisibility(View.INVISIBLE);
            mAddressLineOneTextView.setVisibility(View.INVISIBLE);
            mCodeTextView.setVisibility(View.INVISIBLE);
            mLineTwoTextView.setText(getResources().getString(R.string.text_fragment_address_capture_ex_address_description));
            ViewGroup.LayoutParams params = mLineOnelinearLayout.getLayoutParams();
            params.height = 0;
        }

        getActivity().setTitle(String.format("%1$s - %2$s", getResources().getString(R.string.app_name), getSubHeading()));

        mAddressLineOneEditText.addTextChangedListener(mTextWatcher);
        mAddressLineTwoEditText.addTextChangedListener(mTextWatcher);
        mSuburbEditText.addTextChangedListener(mTextWatcher);
        mCityEditText.addTextChangedListener(mTextWatcher);

        mSearchButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                searchAddress();
            }
        });

        return rootView;
    }

    private String getSubHeading(){
        switch (mAddressType) {
            case Physical: return getResources().getString(R.string.text_fragment_address_capture_ex_address_home_address);
            case Business : return getResources().getString(R.string.text_fragment_address_capture_ex_address_business_address);
            case Postal: return getResources().getString(R.string.text_fragment_address_capture_ex_address_postal_address);
            case Offence: return getResources().getString(R.string.text_fragment_address_capture_ex_address_offence_address);
            default: return null;
        }
    }

    public void searchAddress(){
        String searchString  = mAddressLineOneEditText.getVisibility() == View.VISIBLE ?
                mAddressLineOneEditText.getText().toString() :
                mAddressLineTwoEditText.getText().toString();

        try {
            wizardActivity().setWizardLocked(true);
            wizardActivity().enableNextButton(false);
            wizardActivity().enableBackButton(false);
            DataServiceRequest.googleAddressSearchRequest(this, this.getActivity(), searchString);
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "AddressCaptureFragmentEx::searchAddress()"), ErrorSeverity.High);
            wizardActivity().setWizardLocked(false);
            wizardActivity().enableNextButton(true);
            wizardActivity().enableBackButton(true);
        }
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {

        if (requestCode == Constants.ADDRESS_LIST_REQUEST_CODE) {
            if(resultCode == Activity.RESULT_OK) {
                String address =  data.getStringExtra(Constants.ADDRESS_SEARCH_RESULT);
                GoogleAddressRefactor googleAddressRefactor = new GoogleAddressRefactor();
                googleAddressRefactor.refactor(address, mAddressType);
                switch (mAddressType) {
                    case Offence:
                        mInfringement.setLocationDescription(googleAddressRefactor.getDescription());
                        mInfringement.setLocationSuburb(googleAddressRefactor.getSuburb());
                        mInfringement.setLocationTown(googleAddressRefactor.getTown());
                        break;
                    case Postal:
                        mOffender.setPostalPoBox(googleAddressRefactor.getLineOne());
                        mOffender.setPostalStreet(googleAddressRefactor.getLineTwo());
                        mOffender.setPostalSuburb(googleAddressRefactor.getSuburb());
                        mOffender.setPostalTown(googleAddressRefactor.getTown());
                        mOffender.setPostalCode( googleAddressRefactor.getCode());
                        break;
                    case Physical:
                        mOffender.setPhysicalStreet1(googleAddressRefactor.getLineOne());
                        mOffender.setPhysicalStreet2(googleAddressRefactor.getLineTwo());
                        mOffender.setPhysicalSuburb(googleAddressRefactor.getSuburb());
                        mOffender.setPhysicalTown(googleAddressRefactor.getTown());
                        mOffender.setPhysicalCode(googleAddressRefactor.getCode());
                        break;
                }
            }
        }
    }

    private void startAddressListActivity(String[] addressList) {
        Intent intent = new Intent(this.getActivity(), AddressListActivity.class);
        intent.putExtra(Constants.ADDRESS_LIST_EXTRA, addressList);
        startActivityForResult(intent, Constants.ADDRESS_LIST_REQUEST_CODE);
    }

    private void readFields(){

        switch (mAddressType){
            case Postal:
                mOffender.setPostalPoBox(mAddressLineOneEditText.getText().toString());
                mOffender.setPostalStreet(mAddressLineTwoEditText.getText().toString());
                mOffender.setPostalSuburb(mSuburbEditText.getText().toString());
                mOffender.setPostalTown(mCityEditText.getText().toString());
                mOffender.setPostalCode(mCodeEditText.getText().toString());
                break;
            case Physical:
                mOffender.setPhysicalStreet1(mAddressLineOneEditText.getText().toString());
                mOffender.setPhysicalStreet2(mAddressLineTwoEditText.getText().toString());
                mOffender.setPhysicalSuburb(mSuburbEditText.getText().toString());
                mOffender.setPhysicalTown(mCityEditText.getText().toString());
                mOffender.setPhysicalCode(mCodeEditText.getText().toString());
                break;
            case Offence:
                mInfringement.setLocationDescription(mAddressLineTwoEditText.getText().toString());
                mInfringement.setLocationSuburb(mSuburbEditText.getText().toString());
                mInfringement.setLocationTown(mCityEditText.getText().toString());

                if (wizardActivity().getTicketModel().getICamProsecution() == true){
                    if (TextUtils.isEmpty(SessionModel.getInstance().getOffenceLocation()) == false) {
                        SessionModel.getInstance().setOffenceLocation(
                                pipeFields(mAddressLineTwoEditText.getText().toString(),
                                        mSuburbEditText.getText().toString(),
                                        mCityEditText.getText().toString()));
                    }
                }

                break;
        }
    }

    private void populateFields(String line1, String line2, String suburb, String town, String code){

        mAddressLineOneEditText.setText(line1);
        mAddressLineTwoEditText.setText(line2);
        mSuburbEditText.setText(suburb);
        mCityEditText.setText(town);
        mCodeEditText.setText(code);
    }

    private void populateFields() {

        if (TextUtils.isEmpty(mInfringement.getLocationDescription()) &&
            TextUtils.isEmpty(mInfringement.getLocationSuburb()) &&
            TextUtils.isEmpty(mInfringement.getLocationTown())){

            if (wizardActivity().getTicketModel().getICamProsecution() == true){
                if (TextUtils.isEmpty(SessionModel.getInstance().getOffenceLocation()) == false){

                    String[] offenceLocationArray = SessionModel.getInstance().getOffenceLocation().split("\\|", -1);

                    mInfringement.setLocationDescription(offenceLocationArray.length > 0 ? offenceLocationArray[0] : null);
                    mInfringement.setLocationSuburb(offenceLocationArray.length > 1 ? offenceLocationArray[1] : null);
                    mInfringement.setLocationTown(offenceLocationArray.length > 2 ? offenceLocationArray[2] : null);

                    mInfringement.setLatitude(SessionModel.getInstance().getLatitude());
                    mInfringement.setLongitude(SessionModel.getInstance().getLongitude());
                }
            }

            String address = SessionModel.getInstance().getCurrentGpsAddress();
            if (address != null) {
                GoogleAddressRefactor googleAddressRefactor = new GoogleAddressRefactor();
                googleAddressRefactor.refactor(address, mAddressType);

                mInfringement.setLocationDescription(googleAddressRefactor.getDescription());
                mInfringement.setLocationSuburb(googleAddressRefactor.getSuburb());
                mInfringement.setLocationTown(googleAddressRefactor.getTown());

                mInfringement.setLatitude(SessionModel.getInstance().getLatitude());
                mInfringement.setLongitude(SessionModel.getInstance().getLongitude());
            }
        }

        mAddressLineTwoEditText.setText(mInfringement.getLocationDescription());
        mSuburbEditText.setText(mInfringement.getLocationSuburb());
        mCityEditText.setText(mInfringement.getLocationTown());
    }

//    private void validateUserInterface(){
//        if (mAddressType.equals(AddressType.Offence)){
//            wizardActivity().enableNextButton(mAddressLineTwoEditText.getText().toString().isEmpty() == false);
//        }else {
//            wizardActivity().enableNextButton(true);
//        }
//    }

    private void validateUserInterface(){
        switch (mAddressType){
            case Offence:
                wizardActivity().enableNextButton(
                        (TextUtils.isEmpty(Utilities.trimString(mAddressLineTwoEditText.getText().toString())) == false) &&
                                (TextUtils.isEmpty(Utilities.trimString(mSuburbEditText.getText().toString())) == false) &&
                                (TextUtils.isEmpty(Utilities.trimString(mCityEditText.getText().toString())) == false));
                break;
            case Physical:
                if ((TextUtils.isEmpty(mOffender.getIdNumber()) == false) ||
                        (TextUtils.isEmpty(mOffender.getInitials()) == false) ||
                        (TextUtils.isEmpty(mOffender.getFirstName()) == false) ||
                        (TextUtils.isEmpty(mOffender.getLastName()) == false)){

                    wizardActivity().enableNextButton(
                            (TextUtils.isEmpty(Utilities.trimString(mAddressLineOneEditText.getText().toString())) == false) &&
                                    (TextUtils.isEmpty(Utilities.trimString(mAddressLineTwoEditText.getText().toString())) == false) &&
                                    (TextUtils.isEmpty(Utilities.trimString(mSuburbEditText.getText().toString())) == false) &&
                                    (TextUtils.isEmpty(Utilities.trimString(mCityEditText.getText().toString())) == false));
                }
                break;
            default:
                wizardActivity().enableNextButton(true);
                break;
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try {
            if (asyncResultModel == null) {
                return;
            }

            if (asyncResultModel.getObject() == null) {
                return;
            }

            GoogleGeoCodeReponse response = (GoogleGeoCodeReponse) asyncResultModel.getObject();

            mAddressList = new String[response.results.length];

            for (int i = 0; i < response.results.length; i++) {
                mAddressList[i] = response.results[i].formatted_address;
            }

            if (mAddressList.length > 0) {
                startAddressListActivity(mAddressList);
            } else {
                MessageManager.showMessage(getResources().getString(R.string.message_no_results_found_for_search_criteria), ErrorSeverity.None);
            }
        }finally {
            wizardActivity().setWizardLocked(false);
            validateUserInterface();
            wizardActivity().enableBackButton(true);
        }
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    private void saveData(){
        readFields();
        wizardActivity().getTicketModel().setOffender(mOffender);
    }

    private String pipeFields(String description, String suburb, String city){

        return String.format("%s|%s|%s",
                TextUtils.isEmpty(description) ? Constants.EMPTY_STRING: description,
                TextUtils.isEmpty(suburb) ? Constants.EMPTY_STRING: suburb,
                TextUtils.isEmpty(city) ? Constants.EMPTY_STRING: city);
    }

    @Override
    public void onStart()
    {
        super.onStart();

        switch (mAddressType) {
            case Physical :
                populateFields(
                        mOffender.getPhysicalStreet1(),
                        mOffender.getPhysicalStreet2(),
                        mOffender.getPhysicalSuburb(),
                        mOffender.getPhysicalTown(),
                        mOffender.getPhysicalCode());
                break;
            case Postal :
                populateFields(
                    mOffender.getPostalPoBox(),
                    mOffender.getPostalStreet(),
                    mOffender.getPostalSuburb(),
                    mOffender.getPostalTown(),
                    mOffender.getPostalCode());
                break;
            case Offence : populateFields();
                break;
        }
        validateUserInterface();
    }

    public void onStop() {
        super.onStop();
        saveData();
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
