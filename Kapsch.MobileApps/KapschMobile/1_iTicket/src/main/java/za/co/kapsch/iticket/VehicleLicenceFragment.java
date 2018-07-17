package za.co.kapsch.iticket;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

import java.util.Date;

import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.Constants;

/**
 * A simple {@link Fragment} subclass.
 */
public class VehicleLicenceFragment extends Fragment {

    private VehicleModel mVehicle;

    private Button mScanBarcodeButton;
    private TextView mLicenceNoTextView;
    private TextView mDiskNoTextView;
    private TextView mDescriptionTextView;
    private TextView mMakeTextView;
    private TextView mSeriesTextView;
    private TextView mColourTextView;
    private TextView mVinTextView;
    private TextView mEngineNoTextView;
    private TextView mExpiryDateTextView;

    public VehicleLicenceFragment() {
        // Required empty public constructor
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View rootView = inflater.inflate(R.layout.fragment_vehicle_licence_layout, container, false);

        mScanBarcodeButton = (Button) rootView.findViewById(R.id.scanBarcodeButton);
        mLicenceNoTextView = (TextView) rootView.findViewById(R.id.licenceNoTextView);
        mDiskNoTextView = (TextView) rootView.findViewById(R.id.diskNoTextView);
        mDescriptionTextView = (TextView) rootView.findViewById(R.id.descriptionTextView);
        mMakeTextView = (TextView) rootView.findViewById(R.id.makeTextView);
        mSeriesTextView = (TextView) rootView.findViewById(R.id.seriesTextView);
        mColourTextView = (TextView) rootView.findViewById(R.id.colourTextView);
        mVinTextView = (TextView) rootView.findViewById(R.id.vinTextView);
        mEngineNoTextView = (TextView) rootView.findViewById(R.id.engineNoTextView);
        mExpiryDateTextView = (TextView) rootView.findViewById(R.id.expiryDateTextView);
        mVehicle = wizardActivity().getTicketModel().getVehicle();
        mScanBarcodeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                scanBarcode();
            }
        });

        getActivity().setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.fragment_vehicle_title_a)));

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
    }

    private void write(VehicleModel vehicleModel){

        if (vehicleModel == null) return;

        mLicenceNoTextView.setText(vehicleModel.getLicenceNumber());
        mDiskNoTextView.setText(vehicleModel.getDiscNumber());
        mDescriptionTextView.setText(vehicleModel.getDescription());
        mMakeTextView.setText(vehicleModel.getMake());
        mSeriesTextView.setText(vehicleModel.getModel());
        mColourTextView.setText(vehicleModel.getColour());
        mVinTextView.setText(vehicleModel.getVehicleIdentificationNumber());
        mEngineNoTextView.setText(vehicleModel.getEngineNumber());
        mExpiryDateTextView.setText(vehicleModel.getExpireDate());

        validateUserInterface();
    }

    public void read(byte[] data){
        try {
            if (data == null) return;

            String licenceNumber = Utilities.byteArrayToString(data);

            if (licenceNumber.length() > 20){
                MessageManager.showMessage(getResources().getString(R.string.invalid_barcode), ErrorSeverity.High);
                return;
            }

            if (mVehicle == null) {
               mVehicle = new VehicleModel();
            }

            mVehicle.setLicenceNumber(licenceNumber);
            write(mVehicle);

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleLicenceFragment::setBarcodeData()"), ErrorSeverity.Medium);
        }
    }

//    public void read(byte[] data){
//        try {
//            if (data == null) return;
//
//            if (data.length < 20){
//                Utilities.displayOkMessage(Utilities.byteArrayToString(data), this.getActivity());
//            }else {
//                if (tryReadVehicleLicenceDiscData(data) == false) return;
//
//                String vehicleInfo = Utilities.byteArrayToString(data);
//
//                if (mVehicle == null) {
//                    mVehicle = new VehicleModel();
//                }
//
//                mVehicle.setData(vehicleInfo);
//                write(mVehicle);
//            }
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "VehicleLicenceFragment::setBarcodeData()"), ErrorSeverity.Medium);
//        }
//    }

    private boolean tryReadVehicleLicenceDiscData(byte[] rawData){
        if (rawData.length < 100) return false;

//        String data = Utilities.byteArrayToString(rawData);
//        String[] fields = data.split("%");
//
//        if (fields.length != Constants.VEHICLE_DISK_DATA_FIELD_COUNT){
//            MessageManager.showMessage(getResources().getString(R.string.message_not_valid_vehicle_disc_barcode), ErrorSeverity.Medium);
//            return false;
//        }

        return true;
    }

    public VehicleModel getVehicleModel(){
        return mVehicle;
    }

    @Override
    public void onStart()
    {
        super.onStart();
        write(mVehicle);
        validateUserInterface();
    }

    @Override
    public void onStop(){
        super.onStop();

        //TODO edited data needs to be entered here
        //mVehicleModel.setColour("black");

        wizardActivity().getTicketModel().setVehicle(mVehicle);
    }

    private void validateUserInterface(){
        try{
            Date expiryDate = Utilities.stringToDate(mExpiryDateTextView.getText().toString(), Constants.VEHICLE_DISC_DATE_FORMAT);

            Date currentDate = new Date();
            if (currentDate.after(expiryDate)) {
                mExpiryDateTextView.setTextColor(Color.RED);
            }else {
                mExpiryDateTextView.setTextColor(Color.GRAY);
            }

        }catch(Exception e) {
            MessageManager.showMessage(getResources().getString(R.string.message_date_parse_error), ErrorSeverity.Low);
        }
        wizardActivity().enableNextButton(true);
    }

    private WizardActivity wizardActivity(){
        return (WizardActivity) getActivity();
    }
}
