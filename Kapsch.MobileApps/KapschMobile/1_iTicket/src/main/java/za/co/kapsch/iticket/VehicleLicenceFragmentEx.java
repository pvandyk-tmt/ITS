package za.co.kapsch.iticket;


import android.app.DatePickerDialog;
import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Models.InfringementChargeModel;
import za.co.kapsch.iticket.Models.InfringementModel;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.EnatisVehicle;
import za.co.kapsch.iticket.Models.EnatisVehicleResponse;
import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.iticket.Models.VosiResponse;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Utilities;

import static android.app.AlertDialog.THEME_HOLO_LIGHT;


/**
 * A simple {@link Fragment} subclass.
 */
public class VehicleLicenceFragmentEx extends Fragment {//} implements IAsyncProcessCallBack {

    private VehicleModel mVehicleModel;

    private EditText mLicenceNumberEditText;
    private EditText mMakeEditText;
    private EditText mModelEditText;

    public VehicleLicenceFragmentEx() {
        // Required empty public constructor
    }

    private TextWatcher validationTextWatcher =  new TextWatcher() {
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

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView = inflater.inflate(R.layout.fragment_vehicle_licence_layout_ex, container, false);

        mLicenceNumberEditText = (EditText) rootView.findViewById(R.id.licenceNoEditText);
        mMakeEditText = (EditText) rootView.findViewById(R.id.makeEditText);
        mModelEditText = (EditText) rootView.findViewById(R.id.modelEditText);
        mVehicleModel = wizardActivity().getTicketModel().getVehicle();

        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) mMakeEditText.getLayoutParams();
        layoutParams.width = 360;
        mMakeEditText.setLayoutParams(layoutParams);
        mModelEditText.setLayoutParams(layoutParams);

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.fragment_vehicle_title_b)));

        mMakeEditText.addTextChangedListener(validationTextWatcher);
        mModelEditText.addTextChangedListener(validationTextWatcher);

        mLicenceNumberEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                mLicenceNumberEditText.setTextColor(Color.BLACK);
                validateUserInterface();
            }
        });

        mLicenceNumberEditText.setOnFocusChangeListener(new View.OnFocusChangeListener() {

            @Override
            public void onFocusChange(View v, boolean hasFocus) {
            }
        });

        return rootView;
    }

    private void write(){

        if (mVehicleModel == null){
            return;
        }

        mLicenceNumberEditText.setText(mVehicleModel.getLicenceNumber());

        mMakeEditText.setText(mVehicleModel.getMake());
        mModelEditText.setText(mVehicleModel.getModel());
    }

    private void read(){

        if (mVehicleModel == null) {
            mVehicleModel = new VehicleModel();
            wizardActivity().getTicketModel().setVehicle(mVehicleModel);
        }

        mVehicleModel.setLicenceNumber(mLicenceNumberEditText.getText().toString());
        mVehicleModel.setMake(mMakeEditText.getText().toString());
        mVehicleModel.setModel(mModelEditText.getText().toString());

        replacePrintDescriptionPlaceHolders();

        if (wizardActivity().getTicketModel().isLocallyGeneratedTicket() == true) {
            outstandingViolationsSearch();
        }
    }

    private void validateUserInterface(){
        //if wizardLocked is false it means person id search process is pending or busy we have to wait for it to complete before moving to next wizard page.
        wizardActivity().enableNextButton(wizardActivity().wizardLocked() == false);
        wizardActivity().enableBackButton(wizardActivity().wizardLocked() == false);

        if (wizardActivity().wizardLocked() == true) return;

        wizardActivity().enableNextButton(
                TextUtils.isEmpty(mLicenceNumberEditText.getText()) == false &&
                TextUtils.isEmpty(mMakeEditText.getText()) == false &&
                TextUtils.isEmpty(mModelEditText.getText()) == false);
    }

    private void outstandingViolationsSearch(){

        try {

            String idNumber = wizardActivity().getTicketModel().getOffender().getIdNumber();
            String vehicleLicencePlate = wizardActivity().getTicketModel().getVehicle().getLicenceNumber();

            if (TextUtils.isEmpty(idNumber) && TextUtils.isEmpty(vehicleLicencePlate)){
                return;
            }

            wizardActivity().resetOutstandingViolations();
            DataServiceRequest.finesRequest(wizardActivity(), wizardActivity(), SearchFinesCriteriaType.ID, idNumber, false);
            DataServiceRequest.finesRequest(wizardActivity(), wizardActivity(), SearchFinesCriteriaType.VLN, vehicleLicencePlate, false);

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleLicenceFragmentEx::outstandingViolationsSearch()"), ErrorSeverity.Medium);
        }
    }

    public void replacePrintDescriptionPlaceHolders(){

        InfringementModel infringement = wizardActivity().getTicketModel().getInfringement();
        InfringementChargeModel[] infringementCharges = infringement.getInfringementCharges();

        for (InfringementChargeModel infringementCharge: infringementCharges) {

            if (infringementCharge != null){

                List<String> placeHolders = Utilities.getRegexMatches(infringementCharge.getPrintDescription(), Constants.REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN);

                for(String placeHolder : placeHolders) {

                    if (placeHolder.equals(Constants.VEHMAKE_PLACE_HOLDER)) {
                        String vehicleMake = wizardActivity().getTicketModel().getVehicle().getMake();
                        infringementCharge.setPrintDescription(infringementCharge.getPrintDescription().replace(placeHolder, vehicleMake));
                    }

                    if (placeHolder.equals(Constants.VEHMODEL_PLACE_HOLDER)) {
                        String vehicleModel = wizardActivity().getTicketModel().getVehicle().getModel();
                        infringementCharge.setPrintDescription(infringementCharge.getPrintDescription().replace(placeHolder, vehicleModel));
                    }
                }
            }
        }
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    @Override
    public void onStart()
    {
        super.onStart();
        write();
        validateUserInterface();
    }

    public void onStop()
    {
        super.onStop();
        read();
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
