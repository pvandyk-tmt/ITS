package za.co.kapsch.iticket;


import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Base64;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.Date;

import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.iticket.orm.ConfigItemRepository;
import za.co.kapsch.iticket.orm.DeviceItemRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.technovolve.dlcserializerrsa.DrivingLicenseCard;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Constants;
import za.co.kapsch.iticket.Models.VehicleClassModel;

/**
 * A simple {@link Fragment} subclass.
 */
public class DriversLicenceFragment extends Fragment {

    private TableLayout mVehicleLicInfoTableLayout;
    public static final String DLC_SERIALIZER_RSA_LIC = "dlcSerializerRsaLic";

    private OffenderModel mOffender;

    private Button mScanBarcodeButton;
    private TextView mNameTextView;
    private TextView mGenderTextView;
    private TextView mIdNumberTextView;
    private TextView mLicenceNoTextView;
    private TextView mDateOfBirthTextView;
    private TextView mValidTextView;
    private TextView mIssuedTextView;
    private TextView mRestrictionsTextView;
    private ImageView mDriverImageView;

    private Date mExpiryDate;

    public DriversLicenceFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View rootView = inflater.inflate(R.layout.fragment_drivers_licence_layout, container, false);

        mScanBarcodeButton = (Button) rootView.findViewById(R.id.scanBarcodeButton);
        mNameTextView = (TextView) rootView.findViewById(R.id.nameTextView);
        mGenderTextView = (TextView) rootView.findViewById(R.id.genderTextView);
        mIdNumberTextView = (TextView) rootView.findViewById(R.id.idNumberTextView);
        mLicenceNoTextView = (TextView) rootView.findViewById(R.id.licenceNoTextView);//licenceNoTextView
        mDateOfBirthTextView = (TextView) rootView.findViewById(R.id.dateOfBirthTextView);
        mValidTextView = (TextView) rootView.findViewById(R.id.validTextView);
        mIssuedTextView = (TextView) rootView.findViewById(R.id.issuedTextView);
        mRestrictionsTextView = (TextView) rootView.findViewById(R.id.restrictionsTextView);
        mDriverImageView = (ImageView) rootView.findViewById(R.id.driverImageView);
        mVehicleLicInfoTableLayout = (TableLayout) rootView.findViewById(R.id.vehicleLicInfoTable);
        mOffender = wizardActivity().getTicketModel().getOffender();
        mScanBarcodeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                scanBarcode();
            }
        });

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.fragment_drivers_title_a)));

        return rootView;
    }

    private void scanBarcode() {
        za.co.kapsch.shared.Utilities.startBarcodeScanActivity(this, Constants.SCAN_REQUEST_CODE);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == Constants.SCAN_REQUEST_CODE) {
            if(resultCode == Activity.RESULT_OK) {
                 read(data.getByteArrayExtra(Constants.BARCODE_SCAN_RESULT));
            }
        }


        if (mOffender != null) {
            write(mOffender);
        }
    }

    private void read(byte[] rawData){

        try {
            if (rawData == null) return;

            String idNumber = Utilities.byteArrayToString(rawData);

            if (idNumber.length() > 20){
                MessageManager.showMessage(getResources().getString(R.string.id_number_cannot_be_longer_than_20_characters), ErrorSeverity.High);
                return;
            }

            if (mOffender == null) {
                mOffender = new OffenderModel();
            }

            mOffender.setIdNumber(idNumber);
            wizardActivity().getTicketModel().setOffender(mOffender);


//            if (rawData.length < 20){
//                Utilities.displayOkMessage(Utilities.byteArrayToString(rawData), this.getActivity());
//
//            }else {

                //TODO Query bankend for driver information
                //DrivingLicenseCard driverLicenceCard = tryReadDriversLicenceCardData(rawData);
                //if (driverLicenceCard == null) return;

                //if (mOffender == null) {
                ///    mOffender = new OffenderModel();
                //}

                //mOffender.setData(driverLicenceCard);
 //           }
        } catch (Exception e) {
            MessageManager.showMessage(e.getMessage(), ErrorSeverity.Medium);
        }

    }

    private void write(OffenderModel offenderModel){

        if (offenderModel == null) return;

        mNameTextView.setText(offenderModel.getInitials() + " " + offenderModel.getLastName());
        mGenderTextView.setText(offenderModel.getGender());
        mIdNumberTextView.setText(offenderModel.getIdNumber());
        mLicenceNoTextView.setText(offenderModel.getCertificateNumber());
        mDateOfBirthTextView.setText(Utilities.dateToString(offenderModel.getDateOfBirth()));
        mValidTextView.setText(Utilities.dateToString(offenderModel.getValidFromDate()) + " - " + Utilities.dateToString(offenderModel.getValidUntilDate()));
        //mIssuedTextView.setText(offenderModel.getIdCountry());
        mRestrictionsTextView.setText(offenderModel.getDriverRestrictions());
        mExpiryDate = offenderModel.getValidUntilDate();
        mDriverImageView.setImageBitmap(offenderModel.getPhoto());

        while(mVehicleLicInfoTableLayout.getChildCount() != 1) {
            mVehicleLicInfoTableLayout.removeViewAt(1);
        }

        VehicleClassModel[] vehicleClasses = offenderModel.getVehicleClasses();

        if (vehicleClasses != null){
            for(int i = 0; i < vehicleClasses.length; i++) {
                addVehicleLicInfoTableRow(
                        vehicleClasses[i].getCode(),
                        vehicleClasses[i].getVehicleRestriction(),
                        Utilities.dateToString(vehicleClasses[i].getFirstIssueDate()));
            }
        }

        validateUserInterface();
    }

    private DrivingLicenseCard tryReadDriversLicenceCardData(byte[] rawData){
        DrivingLicenseCard drivingLicenseCard = null;
        try {
            String dlcSerializerRsaLic = ConfigItemRepository.getConfigItem(DLC_SERIALIZER_RSA_LIC).getValue();

            if (dlcSerializerRsaLic != null) {
                DrivingLicenseCard.activate("za.co.kapsch.iticket", Base64.decode(dlcSerializerRsaLic, Base64.DEFAULT));

                if (DrivingLicenseCard.isActive()) {
                    drivingLicenseCard = DrivingLicenseCard.deserialize(rawData);
                }
            }else{
                MessageManager.showMessage("DLC Serializer licence not installed, please do an update.", ErrorSeverity.Medium);
            }
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "SessionModel::getDlcSerializerRsaLic()"), ErrorSeverity.Medium);
            return null;
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DriversLicenceFragment::tryReadDriversLicenceCardData()"), ErrorSeverity.Medium);
            return null;
        }

        return drivingLicenseCard;
    }

     private void addVehicleLicInfoTableRow(String code, String description, String firstIssue) {

        TableRow tableRow = new TableRow(this.getActivity());
        tableRow.setLayoutParams(new TableRow.LayoutParams(TableRow.LayoutParams.MATCH_PARENT, TableRow.LayoutParams.WRAP_CONTENT));

        tableRow.addView(getTableRowItem(code));
        tableRow.addView(getTableRowItem(description));
        tableRow.addView(getTableRowItem(firstIssue));

        mVehicleLicInfoTableLayout.addView(tableRow);
    }

    private View getTableRowItem(String value){
        TextView textView = new TextView(this.getActivity());
        textView.setText(value);
        TableRow.LayoutParams layoutParams = new TableRow.LayoutParams(TableRow.LayoutParams.WRAP_CONTENT, TableRow.LayoutParams.WRAP_CONTENT);
        layoutParams.gravity = Gravity.CENTER_HORIZONTAL;
        textView.setLayoutParams(layoutParams);

        return textView;
    }

    private void validateUserInterface(){
        try{
            wizardActivity().enableNextButton(true);
            Date currentDate = new Date();
            if (currentDate.after(mExpiryDate)) {
                mValidTextView.setTextColor(Color.RED);
            }else {
                mValidTextView.setTextColor(Color.GRAY);
            }
            wizardActivity().enableNextButton(true);
        }catch(Exception e) {
            e.printStackTrace();
            MessageManager.showMessage(getResources().getString(R.string.message_date_parse_error), ErrorSeverity.Low);
        }
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }

    @Override
    public void onStart()
    {
        super.onStart();
        if (mOffender != null) {
            write(mOffender);
        }
        validateUserInterface();
    }

    @Override
    public void onStop()
    {
        super.onStop();
        if (mOffender == null) return;
        wizardActivity().getTicketModel().setOffender(mOffender);
    }
}
