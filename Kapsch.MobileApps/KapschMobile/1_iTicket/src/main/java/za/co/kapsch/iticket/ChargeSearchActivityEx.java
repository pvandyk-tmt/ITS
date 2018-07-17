package za.co.kapsch.iticket;

import android.app.Activity;
import android.content.Intent;
import android.os.Message;
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
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.HeaderViewListAdapter;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.TextView;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.Enums.ChargeQueryType;
import za.co.kapsch.iticket.Enums.SqlJunction;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.orm.ChargeInfoRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

public class ChargeSearchActivityEx extends AppCompatActivity {

    private LinearLayout mSearchBoxLinearLayout;
    private ChargeInfoModel mSelectedChargeBookItem;
    private String mSelectedListItem;
    private ListView mChargeCodeListView;
    private TextView mDescriptionTextView;
    private EditText mEditText;
    //private Spinner mZoneSpinner;
    private ImageButton mSearchButton;
    //private ChargeQueryType mQueryType;
    private String mSelectedZone;
    private TicketModel mTicket;
    //private ChargeSearchListAdapter mChargeSearchListAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_charge_search_ex);

        Intent intent = getIntent();
        //mQueryType = ChargeQueryType.fromInteger(intent.getIntExtra(Constants.CHARGE_QUERY_TYPE, -1));
        mTicket = intent.getParcelableExtra(Constants.TICKET_MODEL);

        mSearchBoxLinearLayout = (LinearLayout) findViewById(R.id.searchBoxLinearLayout);
        mDescriptionTextView = (TextView) findViewById(R.id.descriptionTextView);
        mChargeCodeListView = (ListView) findViewById(R.id.chargeCodeListView);
        mEditText = (EditText) findViewById(R.id.editText);
//        mZoneSpinner = (Spinner) findViewById(R.id.zoneSpinner);
        mSearchButton = (ImageButton) findViewById(R.id.searchButton);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        //mDescriptionTextView.setMovementMethod(new ScrollingMovementMethod());

        LayoutInflater inflater = getLayoutInflater();
        ViewGroup header = (ViewGroup) inflater.inflate(R.layout.charge_search_header, mChargeCodeListView, false);
        mChargeCodeListView.addHeaderView(header);

        executeQuery(ChargeQueryType.All, null, null, true);

        mChargeCodeListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                mChargeCodeListView.setSelector(R.color.selectColor);
                view.setSelected(true);
                mSelectedChargeBookItem = (ChargeInfoModel) mChargeCodeListView.getItemAtPosition(position);
                if (mSelectedChargeBookItem != null) {
                    mDescriptionTextView.setText(mSelectedChargeBookItem.getDescription());
                }
            }
        });

        mChargeCodeListView.setOnTouchListener(new View.OnTouchListener() {

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
                           mChargeCodeListView.setSelector(android.R.color.transparent);
                           mSelectedChargeBookItem = null;
                           mDescriptionTextView.setText(Constants.EMPTY_STRING);
                       }
                       break;
                     default:
                       break;
               }
               return false;
            }
        });


//        mZoneSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
//            @Override
//            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
//                mSelectedZone = (String)parent.getItemAtPosition(position);
//                executeQuery(mEditText.getText().toString(), mSelectedZone, false);
//            }
//
//            public void onNothingSelected(AdapterView<?> parent) {
//
//            }
//        });
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

    private void executeQuery(ChargeQueryType queryType, String searchValue, String zone, boolean updateZoneList){
        switch (queryType) {
//            case Code:
//                if (searchValue != null) {
//                    try {
//                        Integer.parseInt(searchValue);
//                        PopulateChargeCodeListView(searchChargesByCode(searchValue, zone), updateZoneList);
//                    } catch (Exception e) {
//                        MessageManager.showMessage(Utilities.exceptionMessage(e, "executeQuery()"), ErrorSeverity.Low);
//                    }
//                }
//                break;
            case Description:
                setSearchEnable(true);
                if (searchValue != null) {
                    PopulateChargeCodeListView(searchChargesByDesc(searchValue, zone), updateZoneList);
                }
                break;
//            case Favourites:
//                PopulateChargeCodeListView(getFavouriteCharges(zone), updateZoneList);
//                setSearchEnable(false);
//                break;
            case All:
                PopulateChargeCodeListView(getAllCharges(), updateZoneList);
                break;
//            default:
//                PopulateChargeCodeListView(getCharges(mQueryType, zone), updateZoneList);
//                setSearchEnable(false);
//                break;
        }
    }

    private void setSearchEnable(boolean enable){
        mEditText.setEnabled(enable);
        mSearchButton.setEnabled(enable);
    }

    private List<ChargeInfoModel> getCharges(ChargeQueryType chargeQueryType, String zone){
        switch (chargeQueryType){
            case DriversLicence : return getCharges(Constants.DRIVER_LICENSE_LICENCE, zone);
            case VehicleLicence : return getCharges(Constants.VEHICLE_LICENSE_LICENCE, zone);
            case StopSign : return getCharges(Constants.STOP, zone);
            case TrafficSign : return getCharges(Constants.SIGN_ZONE, zone);
            case Seatbelt : return getCharges(Constants.BELT, zone);
            case Tyre : return getCharges(Constants.TYRE, zone);
            case Cellular : return getCharges(Constants.CELL_MOBILE, zone);
            case Roadworthy : return getCharges(Constants.ROADWORTHY, zone);
            case Favourites : return getCharges(Constants.FAVOURITES, zone);
        }

        return null;
    }

    private List<ChargeInfoModel> getCharges(String searchCriteria, String zone) {
        String[] partialDescList;
        SqlJunction conditionalOperator = SqlJunction(searchCriteria);

        try {
            switch (conditionalOperator) {
                case None:
                    return ChargeInfoRepository.getChargesWithoutJunction(searchCriteria, zone);
                case And:
                    partialDescList = searchCriteria.split(Constants.AND);
                    if (partialDescList.length == 2) {
                        return ChargeInfoRepository.getChargesWithAndJunction(partialDescList, zone);
                    } else if (partialDescList.length == 3) {
                        return ChargeInfoRepository.getChargesWithAndJunctionEx(partialDescList, zone);
                    }
                    break;
                case Or:
                    partialDescList = searchCriteria.split(Constants.REG_EXPRESSION_OR);
                    if (partialDescList.length == 2) {
                        return ChargeInfoRepository.getChargesWithOrJunction(partialDescList, zone);
                    } else if (partialDescList.length == 3) {
                        return ChargeInfoRepository.getChargesWithOrJunctionEx(partialDescList, zone);
                    }
                    break;
                case Both:
                    partialDescList = searchCriteria.split(Constants.REG_EXPRESSION_AND_OR);
                    if (partialDescList.length == 3) {
                        return ChargeInfoRepository.getChargesWithBothJunctionEx(partialDescList, zone);
                    }
                    break;
            }
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getCharges()"), ErrorSeverity.Medium);
        }

        return null;
    }

    private SqlJunction SqlJunction(String value){
        if (value.contains(Constants.OR) && value.contains(Constants.AND))return SqlJunction.Both;
        if (value.contains(Constants.OR)) return SqlJunction.Or;
        if (value.contains(Constants.AND)) return SqlJunction.And;
        return SqlJunction.None;
    }

    public void searchCharges(View view) {
        String searchValue = mEditText.getText().toString();
        searchCharges(searchValue, (String)null);
    }

    public void searchCharges(String searchValue, String zone) {

        executeQuery(ChargeQueryType.Description, searchValue, null, true);
    }

    private List<ChargeInfoModel> searchChargesByCode(String code, String zone){
        try {
            return ChargeInfoRepository.getChargesByCode(code, zone);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "searchChargeByCode()"), ErrorSeverity.Medium);
            return null;
        }
    }

    private List<ChargeInfoModel> searchChargesByDesc(String desc, String zone){
        try {
            return ChargeInfoRepository.getChargesByDesc(desc, zone);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "searchChargeByDesc()"), ErrorSeverity.Medium);
            return null;
        }
    }

    private List<ChargeInfoModel> getFavouriteCharges(String zone){
        try {
            return ChargeInfoRepository.getFavouriteCharges(zone);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getFavouriteCharges()"), ErrorSeverity.Medium);
            return null;
        }
    }

    private List<ChargeInfoModel> getAllCharges(){
        try {
            return ChargeInfoRepository.getAllCharges();
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getAllCharges()"), ErrorSeverity.Medium);
            return null;
        }
    }

    private void PopulateChargeCodeListView(List<ChargeInfoModel> offenceCodeList, boolean updateZoneList){

        try {
            ChargeSearchListAdapter chargeSearchListAdapter;

            if (offenceCodeList.size() < 1) {

                HeaderViewListAdapter hlva = (HeaderViewListAdapter)mChargeCodeListView.getAdapter();
                chargeSearchListAdapter = (ChargeSearchListAdapter) hlva.getWrappedAdapter();
                chargeSearchListAdapter.clearData();
                chargeSearchListAdapter.notifyDataSetChanged();
                return;
            }

            chargeSearchListAdapter = new ChargeSearchListAdapter(this, offenceCodeList);
            mChargeCodeListView.setAdapter(chargeSearchListAdapter);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.Low);
        }

//        if (updateZoneList) {
//            populateZoneSpinner(offenceCodeList);
//        }
    }

//    private <T> void  populateZoneSpinner(List<ChargeInfoModel> offenceCodeList){
//
//        List<String> zoneList = new ArrayList<>();
//        for(ChargeInfoModel offenceCode: offenceCodeList){
//            if (isInList(Integer.toString(offenceCode.getZone()), zoneList) == false){
//
//                if(isSmallerThanValuesInList(zoneList, Integer.toString(offenceCode.getZone()))){
//                    zoneList.add(0, Integer.toString(offenceCode.getZone()));
//                }
//                else {
//                    zoneList.add(Integer.toString(offenceCode.getZone()));
//                }
//            }
//        }
//
//        if (zoneList.size() < 1) return;
//        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<>(this, R.layout.list_item, zoneList);
//        mZoneSpinner.setAdapter(arrayAdapter);
//    }

    private boolean isSmallerThanValuesInList(List<String> zoneList, String value){

        for (String zone: zoneList) {
            if (Integer.parseInt(value) > Integer.parseInt(zone)){
                return false;
            }
        }

        return true;
    }

    private boolean isInList(String value, List<String> zoneList){

        for(String zone : zoneList){
            if (zone.equals(value)) return true;
        }

        return  false;
    }

    public void okButtonClick(View view){

        if (mSelectedChargeBookItem == null){
            MessageManager.showMessage(App.getContext().getString(R.string.activity_charge_search_item_not_selected), ErrorSeverity.None);
            return;
        }

        List<String> placeHolders = Utilities.getRegexMatches(mSelectedChargeBookItem.getPrintDescription(), Constants.REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN);

        for(String placeHolder : placeHolders){

//            if (placeHolder.equals(Constants.SPEED_PLACE_HOLDER)) {
//                String speed = Integer.toString(mSelectedChargeBookItem.getSpeed());
//                if (TextUtils.isEmpty(speed) == false) {
//                    mSelectedChargeBookItem.setDescription(mSelectedChargeBookItem.getDescription().replace(placeHolder, speed));
//                }
//            }

            if (placeHolder.equals(Constants.VEHREG_PLACE_HOLDER)){
                String licenceNumber = mTicket.getVehicle().getLicenceNumber();
                if (TextUtils.isEmpty(licenceNumber) == false) {
                    mSelectedChargeBookItem.setPrintDescription(mSelectedChargeBookItem.getPrintDescription().replace(placeHolder, licenceNumber));
                }
            }

            if (placeHolder.equals(Constants.ZONE_PLACE_HOLDER)){
                mSelectedChargeBookItem.setPrintDescription(mSelectedChargeBookItem.getPrintDescription().replace(placeHolder, Integer.toString(mSelectedChargeBookItem.getZone())));
            }

            if (placeHolder.equals(Constants.VEHMAKE_PLACE_HOLDER)){
                String vehicleMake = mTicket.getVehicle().getMake();
                if (TextUtils.isEmpty(vehicleMake) == false) {
                    mSelectedChargeBookItem.setPrintDescription(mSelectedChargeBookItem.getPrintDescription().replace(placeHolder, vehicleMake));
                }
            }

            if (placeHolder.equals(Constants.VEHMODEL_PLACE_HOLDER)){
                String vehicleModel = mTicket.getVehicle().getModel();
                if (TextUtils.isEmpty(vehicleModel) == false) {
                    mSelectedChargeBookItem.setPrintDescription(mSelectedChargeBookItem.getPrintDescription().replace(placeHolder, vehicleModel));
                }
            }
        }

        if (Utilities.getRegexMatches(mSelectedChargeBookItem.getPrintDescription(), Constants.REG_EXPRESSION_CHARGE_PLACEHOLDER_PATTERN).size() > 0){
            startChargePlaceHolderActivity(mSelectedChargeBookItem.getPrintDescription());
            return;
        }

        returnCharge();
    }

    private void returnCharge(){
        if (mSelectedChargeBookItem != null) {
            Intent intent = new Intent();
            intent.putExtra(Constants.CHARGE_QUERY_RESULT, mSelectedChargeBookItem);
            setResult(RESULT_OK, intent);
            finish();
        }
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (resultCode == Activity.RESULT_OK) {
            if (requestCode == Constants.PLACEHOLDER_REPLACED_CHARGE_DESC){
//                int chargeSpeed = data.getIntExtra(Constants.CHARGE_SPEED, 0);
//                String chargeDescription = data.getStringExtra(Constants.CHARGE_DESC);

//                if (chargeDescription.contains(Constants.ZONE_PLACE_HOLDER)){
//                    mSelectedChargeBookItem.setDescription(
//                            chargeDescription.replace(
//                                    Constants.ZONE_PLACE_HOLDER,
//                                    Integer.toString(mSelectedChargeBookItem.getZone())));
//                }
//                else{
                    mSelectedChargeBookItem.setPrintDescription(data.getStringExtra(Constants.CHARGE_PRINT_DESC));
                    mSelectedChargeBookItem.setSpeed(data.getIntExtra(Constants.CHARGE_SPEED, 0));
                    mSelectedChargeBookItem.setVehicleRegistrationNumber(data.getStringExtra(Constants.CHARGE_VEHICLE_LICENCE_NUMBER));
//                }

                returnCharge();
            }
        }
    }

    private void startChargePlaceHolderActivity(String chargePrintDescription) {
        Intent intent = new Intent(this, ChargePlaceHoldersActivity.class);
        intent.putExtra(Constants.CHARGE_PRINT_DESC, chargePrintDescription);
        intent.putExtra(Constants.TICKET_MODEL, mTicket);
        startActivityForResult(intent, Constants.PLACEHOLDER_REPLACED_CHARGE_DESC);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
