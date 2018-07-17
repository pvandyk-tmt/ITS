package za.co.kapsch.iticket;

import android.graphics.Color;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.IdentificationType;
import za.co.kapsch.iticket.Enums.OffenderIdType;
import za.co.kapsch.iticket.Models.CountryModel;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;
import za.co.kapsch.iticket.orm.CountryRepository;
import za.co.kapsch.iticket.orm.IdentificationTypeRepository;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.iticket.Models.PersonAddressInfo;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Utilities;

/**
 * A simple {@link Fragment} subclass.
 */
public class DriversLicenceFragmentEx extends Fragment implements IAsyncProcessCallBack {

    public static final int ID_TYPE_ID_INDEX = 1; //spinner index coincides with id_type_id
    public static final int COUNTRY_ID_INDEX = 196 - 1; // //spinner index is one less than country_id

    private OffenderModel mOffender;
    private EditText mIdNumberEditText;
    private EditText mInitialsEditText;
    private EditText mFirstNameEditText;
    private EditText mLastNameEditText;
    private EditText mEmailNoEditText;
    private EditText mOccupationEditText;
    private EditText mMobileNumberEditText;
    private EditText mGaurdianEditText;
    private Spinner mGenderSpinner;
    private DocumentType mDocumentType;
    private TextView mIdNumberMandatoryTextView;
    private TextView mFirstNameMandatoryTextView;
    private TextView mLastNameMandatoryTextView;
    private TextView mInitialsMandatoryTextView;
    private Spinner mIdTypeSpinner;
    private Spinner mCountrySpinner;

    private TextWatcher textWatcher = new TextWatcher() {
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

    public DriversLicenceFragmentEx() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView = inflater.inflate(R.layout.fragment_drivers_licence_layout_ex, container, false);

        mIdNumberEditText = (EditText) rootView.findViewById(R.id.idNumberEditText);
        mInitialsEditText = (EditText) rootView.findViewById(R.id.initialsEditText);
        mFirstNameEditText = (EditText) rootView.findViewById(R.id.firstNameEditText);
        mLastNameEditText = (EditText) rootView.findViewById(R.id.lastNameEditText);
        mEmailNoEditText = (EditText) rootView.findViewById(R.id.emailEditText);
        mOccupationEditText = (EditText) rootView.findViewById(R.id.occupationEditText);
        mMobileNumberEditText = (EditText) rootView.findViewById(R.id.mobileNumberEditText);
        mGaurdianEditText = (EditText) rootView.findViewById(R.id.gaurdianEditText);
        mGenderSpinner = (Spinner) rootView.findViewById(R.id.genderSpinner);
        mOffender = wizardActivity().getTicketModel().getOffender();
        mIdTypeSpinner = (Spinner) rootView.findViewById(R.id.idTypeSpinner);
        mCountrySpinner = (Spinner) rootView.findViewById(R.id.countrySpinner);

        mIdNumberMandatoryTextView = (TextView) rootView.findViewById(R.id.idMandatoryTextView);
        mFirstNameMandatoryTextView = (TextView) rootView.findViewById(R.id.firstNameMandatoryTextView);
        mLastNameMandatoryTextView = (TextView) rootView.findViewById(R.id.lastNameMandatoryTextView);
        mInitialsMandatoryTextView = (TextView) rootView.findViewById(R.id.initialsMandatoryTextView);

        mDocumentType = wizardActivity().getTicketModel().getDocumentType();

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                R.string.fragment_drivers_person_details_title));

        wizardActivity().enableNextButton(true);

        mIdNumberEditText.setOnFocusChangeListener(new View.OnFocusChangeListener() {

            @Override
            public void onFocusChange(View v, boolean hasFocus) {
                if (!hasFocus) {
                   validateUserInterface();
                }
            }
        });

        mIdNumberEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                if (mIdNumberEditText.hasFocus()) {
                    wizardActivity().setWizardLocked(false);
                    wizardActivity().enableNextButton(false);
                    wizardActivity().enableBackButton(false);
                }
            }
        });

        populateIdTypeSpinner();
        populateCountrySpinner();

        mInitialsEditText.addTextChangedListener(textWatcher);

        mFirstNameEditText.addTextChangedListener(textWatcher);

        mLastNameEditText.addTextChangedListener(textWatcher);

        mEmailNoEditText.addTextChangedListener(textWatcher);

        mIdTypeSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                validateUserInterface();
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        validateUserInterface();

        return rootView;
    }

    private void populateIdTypeSpinner(){

        try {
            mIdTypeSpinner.setAdapter(new ArrayAdapter<>(this.getActivity(), R.layout.spinner_item, IdentificationTypeRepository.getIdentificationTypeList()));
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DriversLicenceFragment::populateIdTypeSpinner()"), ErrorSeverity.High);
        }
    }

    private void populateCountrySpinner(){

        try {
            mCountrySpinner.setAdapter(new ArrayAdapter<>(this.getActivity(), R.layout.spinner_item, CountryRepository.getCountryList()));
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DriversLicenceFragment::populateCountrySpinner()"), ErrorSeverity.High);
        }
    }

    private void write(OffenderModel offender){

        try {
            if (offender == null) {
                mIdTypeSpinner.setSelection(ID_TYPE_ID_INDEX); //default spinner to NATIONAL REGISTRATION CARD (NRC)
                mCountrySpinner.setSelection(COUNTRY_ID_INDEX);//default spinner to Zambia
                validateUserInterface();
                return;
            }

            mIdNumberEditText.setText(offender.getIdNumber());
            mInitialsEditText.setText(offender.getInitials());
            mFirstNameEditText.setText(offender.getFirstName());
            mLastNameEditText.setText(offender.getLastName());
            mEmailNoEditText.setText(offender.getEmail());
            mOccupationEditText.setText(offender.getOccupation());
            mGaurdianEditText.setText(offender.getGuardian());
            if (offender.getMobileNumber() != null) {
                mMobileNumberEditText.setText(offender.getMobileNumber());
            }
            mGenderSpinner.setSelection(((ArrayAdapter)mGenderSpinner.getAdapter()).getPosition(offender.getGender()));

            if (TextUtils.isEmpty(mIdNumberEditText.getText()) == false){
                if (TextUtils.isEmpty(offender.getPhysicalStreet1())){
                    //mIdNumberEdited = true;
                    validateUserInterface();
                    //personInfoSearch();
                }
            }

            if (offender.getIdType() == null) {
                IdentificationTypeModel identificationType = IdentificationTypeRepository.getIdentificationType(0);
                mIdTypeSpinner.setSelection(((ArrayAdapter) mIdTypeSpinner.getAdapter()).getPosition(identificationType));
            } else {
                IdentificationTypeModel identificationType = IdentificationTypeRepository.getIdentificationType(offender.getIdType());
                mIdTypeSpinner.setSelection(((ArrayAdapter) mIdTypeSpinner.getAdapter()).getPosition(identificationType));
            }

            if (offender.getCountryId() == 0){
                CountryModel country = CountryRepository.getCountry(0);
                mCountrySpinner.setSelection(((ArrayAdapter) mCountrySpinner.getAdapter()).getPosition(country));
            } else {
                CountryModel country = CountryRepository.getCountry(offender.getCountryId());
                mIdTypeSpinner.setSelection(((ArrayAdapter) mIdTypeSpinner.getAdapter()).getPosition(country));
            }

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DriversLicenceFragemntEx::write()"), ErrorSeverity.High);
        }

        validateUserInterface();
    }

    private void read(){

        createDriver();

        mOffender.setIdNumber(mIdNumberEditText.getText().toString());
        mOffender.setInitials(mInitialsEditText.getText().toString());
        mOffender.setFirstName(mFirstNameEditText.getText().toString());
        mOffender.setLastName(mLastNameEditText.getText().toString());
        mOffender.setEmail(mEmailNoEditText.getText().toString());
        mOffender.setOccupation(mOccupationEditText.getText().toString());

        mOffender.setMobileNumber(Utilities.rectifyTelephoneNumber(mMobileNumberEditText.getText().toString()));

        mOffender.setGender(mGenderSpinner.getSelectedItem().toString());
        mOffender.setIdType(((IdentificationTypeModel) mIdTypeSpinner.getSelectedItem()).getID());
        mOffender.setCountryId(((CountryModel) mCountrySpinner.getSelectedItem()).getID());
        mOffender.setGuardian(mGaurdianEditText.getText().toString());
    }

    private void createDriver(){
        if (mOffender == null) {
            mOffender = new OffenderModel();
            wizardActivity().getTicketModel().setOffender(mOffender);
        }
    }

    private void personInfoSearch(){

        try {
            String idNumber = mIdNumberEditText.getText().toString();

            if (idNumber.equals(Constants.EMPTY_STRING)){
                //mIdNumberEdited = false;
                validateUserInterface();
                return;
            }

            DataServiceRequest.personInfoRequest(this, this.getActivity(), idNumber);

        }catch (Exception e){
            //mIdNumberEdited = false;
            validateUserInterface();
            MessageManager.showMessage(Utilities.exceptionMessage(e, "personInfoSearch()"), ErrorSeverity.Medium);
        }
    }

    private void validateUserInterface(){

        setManditoryFields();

//        if ((mIdTypeSpinner.getSelectedItem()).equals(IdentificationType.ZambianID) &&
//                Utilities.validateSaIdNumber(mIdNumberEditText.getText().toString()) == false){
//            mIdNumberEdited = false;
//            MessageManager.showMessage(getResources().getString(R.string.fragment_drivers_Licence_not_valid_rsa_id), ErrorSeverity.None);
//            wizardActivity().enableNextButton(false);
//            return;
//        }else if ((mIdTypeSpinner.getSelectedItem()).equals(OffenderIdType.RSA) && (mIdNumberEdited == true)){
//            personInfoSearch();
//            return;
//        } else {
//        mIdNumberEdited = false;
//        }


//        wizardActivity().setWizardLocked(mIdNumberEdited == true);
//
//        if (mIdNumberEditText.hasFocus() == true){
//            wizardActivity().enableNextButton(mIdNumberEdited == false);
//            wizardActivity().enableBackButton(mIdNumberEdited == false);
//            return;
//        }

        if (mDocumentType == DocumentType.RoadSideDriver){
            validateRoadSide();
        }
    }

    private void validateRoadSide(){

        boolean isEmailValid = isEmailValid();

                wizardActivity().enableNextButton(
                //mIdNumberEdited == false &&
                (offenderIdNumberMandatory() ? (TextUtils.isEmpty(mIdNumberEditText.getText()) == false) : true) &&
                TextUtils.isEmpty(mInitialsEditText.getText())==false &&
                TextUtils.isEmpty(mFirstNameEditText.getText())==false &&
                TextUtils.isEmpty(mLastNameEditText.getText())==false &&
                isEmailValid == true );

       // wizardActivity().enableBackButton(mIdNumberEdited == false);
    }

//    private void validateSection341(){
//
//        if (TextUtils.isEmpty(mIdNumberEditText.getText()) == false){
//            wizardActivity().enableNextButton(mIdNumberEdited == false);
//        }else{
//            mIdTypeSpinner.setSelection(((ArrayAdapter) mIdTypeSpinner.getAdapter()).getPosition(OffenderIdType.NoID));
//            wizardActivity().enableNextButton(mIdNumberEdited == false);
//        }
//
//        wizardActivity().enableBackButton(mIdNumberEdited == false);
//    }

    private boolean isEmailValid(){

        if (TextUtils.isEmpty(mEmailNoEditText.getText().toString()) == true){
            return true;
        }

        boolean isEmailValid = Utilities.isValidEmail(mEmailNoEditText.getText().toString());

        mEmailNoEditText.setTextColor(isEmailValid ? Color.BLACK : Color.RED);

        return isEmailValid;
    }

    private void setManditoryFields(){

        if (mDocumentType == DocumentType.RoadSideDriver) {
            mIdNumberMandatoryTextView.setVisibility(offenderIdNumberMandatory() ?  View.VISIBLE : View.INVISIBLE);
            mFirstNameMandatoryTextView.setVisibility(View.VISIBLE);
            mLastNameMandatoryTextView.setVisibility(View.VISIBLE);
            mInitialsMandatoryTextView.setVisibility(View.VISIBLE);
        }
    }

    private boolean offenderIdNumberMandatory(){

        IdentificationTypeModel identificationType = (IdentificationTypeModel)mIdTypeSpinner.getSelectedItem();

        if (identificationType == null) return false;

        if (mDocumentType == DocumentType.RoadSideDriver) {
            return (identificationType.getID() != Constants.IDENTIFICATION_TYPE_UNKNOWN_ID);
        }

        return false;
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    @Override
    public void onStart()
    {
        super.onStart();
        write(mOffender);
    }

    public void onStop()
    {
        super.onStop();
        read();
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {
            if (asyncResultModel == null) return;

            if (asyncResultModel.getObject() == null) {
                 MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
                 if (mOffender != null){
                     mOffender.setMobileNumber(Constants.EMPTY_STRING);
                     mOffender.setPhysicalStreet1(Constants.EMPTY_STRING);
                     mOffender.setPhysicalStreet2(Constants.EMPTY_STRING);
                     mOffender.setPhysicalSuburb(Constants.EMPTY_STRING);
                     mOffender.setPhysicalTown(Constants.EMPTY_STRING);
                     mOffender.setPhysicalCode(Constants.EMPTY_STRING);
                }
                return;
            }

            createDriver();

            PersonAddressInfo personAddressInfo = (PersonAddressInfo) asyncResultModel.getObject();

            mOffender.setMobileNumber(Utilities.rectifyTelephoneNumber(personAddressInfo.getPersonModel().getTelephone()));

            String physicalAddress = mOffender.getPhysicalStreet1();

            if (physicalAddress != null) {
                if (physicalAddress.equals(Constants.EMPTY_STRING) == false) {
                    return;
                }
            }

            Utilities.removeNonLetters(personAddressInfo.getPhysicalDataServiceAddressModel().getResidual());
            mOffender.setPhysicalStreet1(personAddressInfo.getPhysicalDataServiceAddressModel().getStreet());
            mOffender.setPhysicalSuburb(personAddressInfo.getPhysicalDataServiceAddressModel().getSuburb());
            mOffender.setPhysicalTown(personAddressInfo.getPhysicalDataServiceAddressModel().getTown());
            mOffender.setPhysicalCode(personAddressInfo.getPhysicalDataServiceAddressModel().getCode());
        }catch (IllegalStateException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DriversLicenceFragmentEx::personInfoSearch()"), ErrorSeverity.Medium);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DriversLicenceFragmentEx::personInfoSearch()"), ErrorSeverity.Medium);
        }
        finally {
            //mIdNumberEdited = false;
            validateUserInterface();
        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
