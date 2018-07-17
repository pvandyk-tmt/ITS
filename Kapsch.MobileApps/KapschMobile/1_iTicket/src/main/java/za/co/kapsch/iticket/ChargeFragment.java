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
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.InfringementChargeModel;
import za.co.kapsch.iticket.Models.InfringementModel;
import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.iticket.orm.ChargeInfoRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;


/**
 * A simple {@link Fragment} subclass.
 */
public class ChargeFragment extends Fragment {

    public ChargeFragment() {
        // Required empty public constructor
    }

    //private OffenderModel mDriver;
    private InfringementModel mInfringement;

    //private CheckBox mAlternativeCheckBox;
    private EditText mCodeOneEditText;
    private TextView mOffenceDateOneTextView;
    private TextView mOffenceOneTextView;
    private EditText mDescriptionOneEditText;
    //private TextView mRegulationOneTextView;
    private EditText mAmountOneEditText;

    private EditText mCodeTwoEditText;
    private TextView mOffenceDateTwoTextView;
    private TextView mOffenceTwoTextView;
    private EditText mDescriptionTwoEditText;
    //private TextView mRegulationTwoTextView;
    private EditText mAmountTwoEditText;

    private EditText mCodeThreeEditText;
    private TextView mOffenceDateThreeTextView;
    private TextView mOffenceThreeTextView;
    private EditText mDescriptionThreeEditText;
    //private TextView mRegulationThreeTextView;
    private EditText mAmountThreeEditText;

    private ImageButton mSearchChargeCodeOneButton;
    private ImageButton mSearchChargeCodeTwoButton;
    private ImageButton mSearchChargeCodeThreeButton;
    private ImageButton mQuickSearchChargeCodeOneButton;
    private ImageButton mQuickSearchChargeCodeTwoButton;
    private ImageButton mQuickSearchChargeCodeThreeButton;

    private EditText mNotesEditText;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView =   inflater.inflate(R.layout.fragment_charge_layout, container, false);

//        mAlternativeCheckBox = (CheckBox) rootView.findViewById(R.id.alternativeCheckBox);
//        mAlternativeCheckBox.setVisibility(View.INVISIBLE);

        mCodeOneEditText = (EditText) rootView.findViewById(R.id.codeOneEditText);
        mOffenceDateOneTextView = (TextView) rootView.findViewById(R.id.offenceDateOneTextView);
        mOffenceOneTextView = (TextView) rootView.findViewById(R.id.offenceOneTextView);
        mDescriptionOneEditText = (EditText) rootView.findViewById(R.id.descriptionOneEditText);
        //mRegulationOneTextView = (TextView) rootView.findViewById(R.id.offenceOneTextView);
        mAmountOneEditText = (EditText) rootView.findViewById(R.id.amountOneEditText);

        mCodeTwoEditText = (EditText) rootView.findViewById(R.id.codeTwoEditText);
        mOffenceDateTwoTextView = (TextView) rootView.findViewById(R.id.offenceDateTwoTextView);
        mOffenceTwoTextView = (TextView) rootView.findViewById(R.id.offenceTwoTextView);
        mDescriptionTwoEditText = (EditText) rootView.findViewById(R.id.descriptionTwoEditText);
        //mRegulationTwoTextView = (TextView) rootView.findViewById(R.id.regulationTwoTextView);
        mAmountTwoEditText = (EditText) rootView.findViewById(R.id.amountTwoEditText);

        mCodeThreeEditText = (EditText) rootView.findViewById(R.id.codeThreeEditText);
        mOffenceDateThreeTextView = (TextView) rootView.findViewById(R.id.offenceDateThreeTextView);
        mOffenceThreeTextView = (TextView) rootView.findViewById(R.id.offenceThreeTextView);
        mDescriptionThreeEditText = (EditText) rootView.findViewById(R.id.descriptionThreeEditText);
        //mRegulationThreeTextView = (TextView) rootView.findViewById(R.id.regulationThreeTextView);
        mAmountThreeEditText = (EditText) rootView.findViewById(R.id.amountThreeEditText);

        mSearchChargeCodeOneButton = (ImageButton) rootView.findViewById(R.id.searchOneButton);
        mSearchChargeCodeTwoButton = (ImageButton) rootView.findViewById(R.id.searchTwoButton);
        mSearchChargeCodeThreeButton = (ImageButton) rootView.findViewById(R.id.searchThreeButton);

        mQuickSearchChargeCodeOneButton = (ImageButton) rootView.findViewById(R.id.quickSearchOneButton);
        mQuickSearchChargeCodeTwoButton = (ImageButton) rootView.findViewById(R.id.quickSearchTwoButton);
        mQuickSearchChargeCodeThreeButton = (ImageButton) rootView.findViewById(R.id.quickSearchThreeButton);

        mNotesEditText = (EditText) rootView.findViewById(R.id.notesEditText);

        mInfringement = wizardActivity().getTicketModel().getInfringement();

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.fragment_charge_title)));

//        mAlternativeCheckBox.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                setAlternativeCharge();
//            }
//        });

        mSearchChargeCodeOneButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                searchChargeOne();
            }
        });

        mSearchChargeCodeTwoButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                searchChargeTwo();
            }
        });

        mSearchChargeCodeThreeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                searchChargeThree();
            }
        });

        mQuickSearchChargeCodeOneButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                quickSearchChargeOne();
            }
        });

        mQuickSearchChargeCodeTwoButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                quickSearchChargeTwo();
            }
        });

        mQuickSearchChargeCodeThreeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                quickSearchChargeThree();
            }
        });

        mAmountThreeEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {

                try {
                    Double amountOne = (double) 0;
                    Double amountThree = (double) 0;

                    if (TextUtils.isEmpty(mAmountOneEditText.getText()) == false) {
                        if (mAmountOneEditText.getText().toString().equals(Constants.NAG)) {
                            //amountOne = (double) ConfigItemModel.getInstance().getNagAmount();
                        } else if (mAmountOneEditText.getText().toString().equals(getResources().getString(R.string.text_place_holder))==false) {
                            amountOne = Double.parseDouble(mAmountOneEditText.getText().toString());
                        }
                    }

                    if (TextUtils.isEmpty(mAmountThreeEditText.getText()) == false) {
                        if (mAmountThreeEditText.getText().toString().equals(Constants.NAG)) {
                            //amountThree = (double) ConfigItemModel.getInstance().getNagAmount();
                        } else if (mAmountThreeEditText.getText().toString().equals(getResources().getString(R.string.text_place_holder))==false) {
                            amountThree = Double.parseDouble(mAmountThreeEditText.getText().toString());
                        }
                    }

//                    if (amountThree > amountOne) {
//                        mAlternativeCheckBox.setChecked(false);
//                    }
//
//                    if (amountOne > 0) {
//                        mAlternativeCheckBox.setVisibility(amountOne >= amountThree ? View.VISIBLE : View.INVISIBLE);
//                    }
                }catch (Exception e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "ChargeFragment::afterTextChanged"), ErrorSeverity.High);
                }
            }
        });

       return rootView;
    }

    public void searchChargeOne(){
        startChargeSearchActivity(Constants.CHARGE_ONE_REQUEST);
    }

    public void searchChargeTwo(){
        startChargeSearchActivity(Constants.CHARGE_TWO_REQUEST);
    }

    public void searchChargeThree(){
        startChargeSearchActivity(Constants.CHARGE_THREE_REQUEST);
    }

    public void quickSearchChargeOne(){

        ChargeInfoModel chargeBookModel = null;
        try {
            chargeBookModel = ChargeInfoRepository.getCharge(mCodeOneEditText.getText().toString());
        } catch (SQLException e) {
            MessageManager.showMessage(e.getMessage(), ErrorSeverity.Medium);
        }
        if (chargeBookModel != null) {
            mInfringement.getInfringementCharges()[0] = InfringementChargeFromChargeBook(chargeBookModel);
            String chargePrintDescription = chargePlaceHolderManager(mInfringement.getInfringementCharges()[0]);
            startChargePlaceHolderActivity(chargePrintDescription, Constants.PLACEHOLDER_REPLACED_CHARGE_DESC_ONE);
            mInfringement.getInfringementCharges()[0].setPrintDescription(chargePrintDescription);
            populateChargeOneFields(mInfringement);
            validateUserInterface();
        }
        else{
            MessageManager.showMessage(getResources().getString(R.string.message_no_results_found_for_search_criteria), ErrorSeverity.None);
        }
    }

//    public void setAlternativeCharge(){
//        if (mInfringement.getInfringementCharges()[2] != null) {
//            mInfringement.getInfringementCharges()[2].setIsAlternative(mAlternativeCheckBox.isChecked());
//        }
//    }

    public void quickSearchChargeTwo(){
        ChargeInfoModel chargeBookModel = null;
        try {
            chargeBookModel = ChargeInfoRepository.getCharge(mCodeTwoEditText.getText().toString());
        } catch (SQLException e) {
            MessageManager.showMessage(e.getMessage(), ErrorSeverity.Medium);
        }
        if (chargeBookModel != null) {
            mInfringement.getInfringementCharges()[1] = InfringementChargeFromChargeBook(chargeBookModel);
            String chargePrintDescription = chargePlaceHolderManager(mInfringement.getInfringementCharges()[1]);
            startChargePlaceHolderActivity(chargePrintDescription, Constants.PLACEHOLDER_REPLACED_CHARGE_DESC_TWO);
            mInfringement.getInfringementCharges()[1].setDescription(chargePrintDescription);
            populateChargeTwoFields(mInfringement);
            validateUserInterface();
        }
        else{
            mInfringement.getInfringementCharges()[1] = null;
            populateChargeTwoFields(mInfringement);
            MessageManager.showMessage(getResources().getString(R.string.message_no_results_found_for_search_criteria), ErrorSeverity.None);
        }
    }

    public void quickSearchChargeThree(){
        ChargeInfoModel chargeBookModel = null;
        try {
            chargeBookModel = ChargeInfoRepository.getCharge(mCodeThreeEditText.getText().toString());
        } catch (SQLException e) {
            MessageManager.showMessage(e.getMessage(), ErrorSeverity.Medium);
        }
        if (chargeBookModel != null) {
            mInfringement.getInfringementCharges()[2] = InfringementChargeFromChargeBook(chargeBookModel);
            String chargePrintDescription = chargePlaceHolderManager(mInfringement.getInfringementCharges()[2]);
            startChargePlaceHolderActivity(chargePrintDescription, Constants.PLACEHOLDER_REPLACED_CHARGE_DESC_THREE);
            mInfringement.getInfringementCharges()[2].setDescription(chargePrintDescription);
            populateChargeThreeFields(mInfringement);
            validateUserInterface();
        }
        else{
            mInfringement.getInfringementCharges()[2] = null;
            populateChargeThreeFields(mInfringement);
            MessageManager.showMessage(getResources().getString(R.string.message_no_results_found_for_search_criteria), ErrorSeverity.None);
        }
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        try {
            String vehicleLicenceNumber;
            ChargeInfoModel chargeBookModel = null;

            if (resultCode == Activity.RESULT_OK) {
                switch (requestCode) {

                    case Constants.CHARGE_ONE_REQUEST:
                        chargeBookModel = data.getParcelableExtra(Constants.CHARGE_QUERY_RESULT);
                        mInfringement.getInfringementCharges()[0] = InfringementChargeFromChargeBook(chargeBookModel);
                        if (TextUtils.isEmpty(chargeBookModel.getVehicleRegistrationNumber()) == false) {
                            wizardActivity().getTicketModel().getVehicle().setLicenceNumber(chargeBookModel.getVehicleRegistrationNumber());
                        }
                        break;

                    case Constants.CHARGE_TWO_REQUEST:
                        chargeBookModel = data.getParcelableExtra(Constants.CHARGE_QUERY_RESULT);
                        mInfringement.getInfringementCharges()[1] = InfringementChargeFromChargeBook(chargeBookModel);

                        if (TextUtils.isEmpty(chargeBookModel.getVehicleRegistrationNumber()) == false) {
                            wizardActivity().getTicketModel().getVehicle().setLicenceNumber(chargeBookModel.getVehicleRegistrationNumber());
                        }
                        break;

                    case Constants.CHARGE_THREE_REQUEST:
                        chargeBookModel = data.getParcelableExtra(Constants.CHARGE_QUERY_RESULT);
                        mInfringement.getInfringementCharges()[2] = InfringementChargeFromChargeBook(chargeBookModel);

                        if (TextUtils.isEmpty(chargeBookModel.getVehicleRegistrationNumber()) == false) {
                            wizardActivity().getTicketModel().getVehicle().setLicenceNumber(chargeBookModel.getVehicleRegistrationNumber());
                        }
                        break;

                    case Constants.PLACEHOLDER_REPLACED_CHARGE_DESC_ONE:
                        mInfringement.getInfringementCharges()[0].setSpeed(data.getIntExtra(Constants.CHARGE_SPEED, 0));
                        //mInfringement.getInfringementCharges()[0].setDescription(data.getStringExtra(Constants.CHARGE_DESC));
                        mInfringement.getInfringementCharges()[0].setPrintDescription(data.getStringExtra(Constants.CHARGE_PRINT_DESC));

                        vehicleLicenceNumber = data.getStringExtra(Constants.CHARGE_VEHICLE_LICENCE_NUMBER);
                        if (TextUtils.isEmpty(vehicleLicenceNumber) == false) {
                            wizardActivity().getTicketModel().getVehicle().setLicenceNumber(vehicleLicenceNumber);
                        }
                       break;

                    case Constants.PLACEHOLDER_REPLACED_CHARGE_DESC_TWO:
                        mInfringement.getInfringementCharges()[1].setSpeed(data.getIntExtra(Constants.CHARGE_SPEED, 0));
                        //mInfringement.getInfringementCharges()[1].setDescription(data.getStringExtra(Constants.CHARGE_DESC));
                        mInfringement.getInfringementCharges()[1].setPrintDescription(data.getStringExtra(Constants.CHARGE_PRINT_DESC));

                        vehicleLicenceNumber = data.getStringExtra(Constants.CHARGE_VEHICLE_LICENCE_NUMBER);
                        if (TextUtils.isEmpty(vehicleLicenceNumber) == false) {
                            wizardActivity().getTicketModel().getVehicle().setLicenceNumber(vehicleLicenceNumber);
                        }
                        break;

                    case Constants.PLACEHOLDER_REPLACED_CHARGE_DESC_THREE:
                        mInfringement.getInfringementCharges()[2].setSpeed(data.getIntExtra(Constants.CHARGE_SPEED, 0));
                        //mInfringement.getInfringementCharges()[2].setDescription(data.getStringExtra(Constants.CHARGE_DESC));
                        mInfringement.getInfringementCharges()[2].setPrintDescription(data.getStringExtra(Constants.CHARGE_PRINT_DESC));

                        vehicleLicenceNumber = data.getStringExtra(Constants.CHARGE_VEHICLE_LICENCE_NUMBER);
                        if (TextUtils.isEmpty(vehicleLicenceNumber) == false) {
                            wizardActivity().getTicketModel().getVehicle().setLicenceNumber(vehicleLicenceNumber);
                        }
                        break;
                }
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ChargeFragment::onActivityResult()"), ErrorSeverity.High);
        }
        finally {
            mSearchChargeCodeOneButton.setEnabled(true);
            mSearchChargeCodeTwoButton.setEnabled(true);
            mSearchChargeCodeThreeButton.setEnabled(true);
        }
    }

    private void populateChargeOneFields(InfringementModel infringement){

        if (infringement.getInfringementCharges()[0] != null) {
            mCodeOneEditText.setText(infringement.getInfringementCharges()[0].getChargeCode());
            mOffenceDateOneTextView.setText(Utilities.dateToString(infringement.getOffenceDate()));
            mOffenceOneTextView.setText(infringement.getInfringementCharges()[0].getDescription());
            mDescriptionOneEditText.setText(infringement.getInfringementCharges()[0].getUserCapturedDescription());
            //mRegulationOneTextView.setText(infringement.getInfringementCharges()[0].getRegulation());
            mAmountOneEditText.setText(ChargeAmount(infringement.getInfringementCharges()[0].getFineAmount()));

            if (infringement.getInfringementCharges()[0].getFineAmount() == 0){
                MessageManager.showMessage(getResources().getString(R.string.fragment_charge_amount_invalid), ErrorSeverity.None);
            }
        }
    }

    private void populateChargeTwoFields(InfringementModel infringement){

        if (infringement.getInfringementCharges()[1] != null) {
            mCodeTwoEditText.setText(infringement.getInfringementCharges()[1].getChargeCode());
            mOffenceDateTwoTextView.setText(Utilities.dateToString(infringement.getOffenceDate()));
            mOffenceTwoTextView.setText(infringement.getInfringementCharges()[1].getDescription());
            mDescriptionTwoEditText.setText(infringement.getInfringementCharges()[1].getUserCapturedDescription());
            //mRegulationTwoTextView.setText(infringement.getInfringementCharges()[1].getRegulation());
            mAmountTwoEditText.setText(ChargeAmount(infringement.getInfringementCharges()[1].getFineAmount()));

            if (infringement.getInfringementCharges()[1].getFineAmount() == 0){
                MessageManager.showMessage(getResources().getString(R.string.fragment_charge_amount_invalid), ErrorSeverity.None);
            }
        }else{
            mCodeTwoEditText.setText(Constants.EMPTY_STRING);
            mOffenceDateTwoTextView.setText(Constants.EMPTY_STRING);
            mOffenceTwoTextView.setText(Constants.EMPTY_STRING);
            //mRegulationTwoTextView.setText(Constants.EMPTY_STRING);
            mAmountTwoEditText.setText(Constants.EMPTY_STRING);
        }
    }

    private void populateChargeThreeFields(InfringementModel infringement){

        if (infringement.getInfringementCharges()[2] != null) {
            mCodeThreeEditText.setText(infringement.getInfringementCharges()[2].getChargeCode());
            mOffenceDateThreeTextView.setText(Utilities.dateToString(infringement.getOffenceDate()));
            mOffenceThreeTextView.setText(infringement.getInfringementCharges()[2].getDescription());
            mDescriptionThreeEditText.setText(infringement.getInfringementCharges()[2].getUserCapturedDescription());
            //mRegulationThreeTextView.setText(infringement.getInfringementCharges()[2].getRegulation());
            mAmountThreeEditText.setText(ChargeAmount(infringement.getInfringementCharges()[2].getFineAmount()));
            //mAlternativeCheckBox.setChecked(infringement.getInfringementCharges()[2].getIsAlternative());

            if (infringement.getInfringementCharges()[2].getFineAmount() == 0){
                MessageManager.showMessage(getResources().getString(R.string.fragment_charge_amount_invalid), ErrorSeverity.None);
            }
        }else{
            mCodeThreeEditText.setText(Constants.EMPTY_STRING);
            mOffenceDateThreeTextView.setText(Constants.EMPTY_STRING);
            mOffenceThreeTextView.setText(Constants.EMPTY_STRING);
            //mRegulationThreeTextView.setText(Constants.EMPTY_STRING);
            mAmountThreeEditText.setText(Constants.EMPTY_STRING);
        }
    }

    private String ChargeAmount(double fineAmount){
        try {
            return fineAmount == ConfigItemModel.getInstance().getNagAmount() ? Constants.NAG : String.valueOf((int) fineAmount);
        }catch (Exception e){
            return fineAmount == ConfigItemModel.getInstance().getNagAmount() ? Constants.NAG : String.valueOf(fineAmount);
        }
    }

    private InfringementChargeModel InfringementChargeFromChargeBook(ChargeInfoModel chargeBookModel){

        InfringementChargeModel infringementCharge = new InfringementChargeModel();
        infringementCharge.setDescription(chargeBookModel.getDescription());
        infringementCharge.setPrintDescription(chargeBookModel.getPrintDescription());
        infringementCharge.setRegulation(chargeBookModel.getRegulationDescription());
        infringementCharge.setId(chargeBookModel.getId());
        infringementCharge.setChargeCode(chargeBookModel.getCode());
        //infringementCharge.setRegulation(chargeBookModel.getRegulation());
        infringementCharge.setFineAmount(chargeBookModel.getFineAmount());
        infringementCharge.setZone(chargeBookModel.getZone());
        infringementCharge.setOffenceSet(chargeBookModel.getOffenceSet());
        //infringementCharge.setIsByLaw(chargeBookModel.getIsByLaw());
        infringementCharge.setSpeed(chargeBookModel.getSpeed());
        //infringementCharge.setAlternativeOffenceCodeId(chargeBookModel.getAlternativeOffenceCodeId());

        return infringementCharge;
    }

    private void startChargeSearchActivity(int requestCode) {
        mSearchChargeCodeOneButton.setEnabled(false);
        mSearchChargeCodeTwoButton.setEnabled(false);
        mSearchChargeCodeThreeButton.setEnabled(false);

        Intent intent = new Intent(this.getActivity(), ChargeSearchActivity.class);
        intent.putExtra(Constants.TICKET_MODEL, wizardActivity().getTicketModel());
        startActivityForResult(intent, requestCode);
    }

    private void validateUserInterface(){

        try {

            mAmountOneEditText.setEnabled(mAmountOneEditText.getText().toString().equals(Constants.NAG) == false);
            mAmountTwoEditText.setEnabled(mAmountTwoEditText.getText().toString().equals(Constants.NAG) == false);
            mAmountThreeEditText.setEnabled(mAmountThreeEditText.getText().toString().equals(Constants.NAG) == false);

//            if (mInfringement.getInfringementCharges()[2] != null){
//                if (Utilities.stringToDouble(mAmountThreeEditText.getText().toString()) <= Utilities.stringToDouble(mAmountOneEditText.getText().toString())) {
//                    mAlternativeCheckBox.setVisibility(View.VISIBLE);
//                } else {
//                    mAlternativeCheckBox.setVisibility(View.INVISIBLE);
//                }
//            }

            wizardActivity().enableNextButton(mInfringement.getInfringementCharges()[0] != null);

            VehicleModel vehicle = wizardActivity().getTicketModel().getVehicle();
            boolean vehilceInfoEmpty = vehicle.getMake().isEmpty();

            InfringementChargeModel[] infringementChargeModel = mInfringement.getInfringementCharges();
            if (infringementChargeModel[0] != null) {
                if (infringementChargeModel[0].getIsByLaw() == false) {
                    wizardActivity().enableNextButton(vehilceInfoEmpty == false);
                    if (vehilceInfoEmpty) {
                        Utilities.displayOkMessage(getResources().getString(R.string.fragment_charge_please_capture_vehicle_info_message), getActivity());
                    }
                    return;
                }
            }

            if (infringementChargeModel[1] != null) {
                if (infringementChargeModel[1].getIsByLaw() == false) {
                    wizardActivity().enableNextButton(vehilceInfoEmpty == false);
                    if (vehilceInfoEmpty) {
                        Utilities.displayOkMessage(getResources().getString(R.string.fragment_charge_please_capture_vehicle_info_message), getActivity());
                    }
                    return;
                }
            }

            if (infringementChargeModel[2] != null) {
                if (infringementChargeModel[2].getIsByLaw() == false) {
                    wizardActivity().enableNextButton(vehilceInfoEmpty == false);
                    if (vehilceInfoEmpty) {
                        Utilities.displayOkMessage(getResources().getString(R.string.fragment_charge_please_capture_vehicle_info_message), getActivity());
                    }
                    return;
                }
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ChargeFragment::validateUserInterface()"), ErrorSeverity.High);
        }
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    @Override
    public void onStart()
    {
        super.onStart();
        populateChargeOneFields(mInfringement);
        populateChargeTwoFields(mInfringement);
        populateChargeThreeFields(mInfringement);
        mNotesEditText.setText(mInfringement.getNotes());
        validateUserInterface();
    }

    public void onStop() {

        if (TextUtils.isEmpty(mAmountOneEditText.getText()) == false){
            if (mInfringement.getInfringementCharges()[0] != null) {
                mInfringement.getInfringementCharges()[0].setFineAmount(
                        mAmountOneEditText.getText().toString().equals(Constants.NAG) ?
                                ConfigItemModel.getInstance().getNagAmount() :
                                Double.parseDouble(mAmountOneEditText.getText().toString()));

                mInfringement.getInfringementCharges()[0].setUserCapturedDescription(mDescriptionOneEditText.getText().toString());
            }
        }

        if (TextUtils.isEmpty(mAmountTwoEditText.getText()) == false) {
            if (mInfringement.getInfringementCharges()[1] != null) {
                mInfringement.getInfringementCharges()[1].setFineAmount(
                        mAmountTwoEditText.getText().toString().equals(Constants.NAG) ?
                                ConfigItemModel.getInstance().getNagAmount() :
                                Double.parseDouble(mAmountTwoEditText.getText().toString()));

                mInfringement.getInfringementCharges()[1].setUserCapturedDescription(mDescriptionTwoEditText.getText().toString());
            }
        }

        if (TextUtils.isEmpty(mAmountThreeEditText.getText()) == false) {
            if (mInfringement.getInfringementCharges()[2] != null) {
                mInfringement.getInfringementCharges()[2].setFineAmount(
                        mAmountThreeEditText.getText().toString().equals(Constants.NAG) ?
                                ConfigItemModel.getInstance().getNagAmount() :
                                Double.parseDouble(mAmountThreeEditText.getText().toString()));

                mInfringement.getInfringementCharges()[2].setUserCapturedDescription(mDescriptionThreeEditText.getText().toString());
            }
        }

        mInfringement.setNotes(mNotesEditText.getText().toString());

        super.onStop();
    }

    public String chargePlaceHolderManager(InfringementChargeModel infringementCharge){

        List<String> placeHolders = Utilities.getRegexMatches(infringementCharge.getPrintDescription(), Constants.REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN);

        for(String placeHolder : placeHolders){

            if (placeHolder.equals(Constants.VEHREG_PLACE_HOLDER)){
                String licenceNumber = wizardActivity().getTicketModel().getVehicle().getLicenceNumber();
                infringementCharge.setPrintDescription(infringementCharge.getPrintDescription().replace(placeHolder, licenceNumber));
            }

            if (placeHolder.equals(Constants.ZONE_PLACE_HOLDER)){
                infringementCharge.setPrintDescription(infringementCharge.getPrintDescription().replace(placeHolder, Integer.toString(infringementCharge.getZone())));
            }

            if (placeHolder.equals(Constants.VEHMAKE_PLACE_HOLDER)){
                String vehicleMake = wizardActivity().getTicketModel().getVehicle().getMake();
                infringementCharge.setPrintDescription(infringementCharge.getPrintDescription().replace(placeHolder, vehicleMake));
            }

            if (placeHolder.equals(Constants.VEHMODEL_PLACE_HOLDER)){
                String vehicleModel = wizardActivity().getTicketModel().getVehicle().getMake();
                infringementCharge.setPrintDescription(infringementCharge.getPrintDescription().replace(placeHolder, vehicleModel));
            }
        }

        return infringementCharge.getPrintDescription();
    }

    private void startChargePlaceHolderActivity(String chargePrintDescription, int activityId) {

        if (Utilities.getRegexMatches(chargePrintDescription, Constants.REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN).size() == 0) return;

        Intent intent = new Intent(this.getActivity(), ChargePlaceHoldersActivity.class);
        intent.putExtra(Constants.CHARGE_PRINT_DESC, chargePrintDescription);
        intent.putExtra(Constants.TICKET_MODEL, wizardActivity().getTicketModel());
        startActivityForResult(intent, activityId);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
