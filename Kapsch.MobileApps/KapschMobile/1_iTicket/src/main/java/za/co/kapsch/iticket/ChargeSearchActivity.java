package za.co.kapsch.iticket;

import android.app.Activity;
import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;

import za.co.kapsch.iticket.Enums.ChargeQueryType;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.TicketModel;

public class ChargeSearchActivity extends AppCompatActivity {

    private TicketModel mTicket;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_charge_search);

        Intent intent = getIntent();
        mTicket = intent.getParcelableExtra(Constants.TICKET_MODEL);

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

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (resultCode == Activity.RESULT_OK) {
            ChargeInfoModel chargeBookModel;
            chargeBookModel = data.getParcelableExtra(Constants.CHARGE_QUERY_RESULT);
            returnCharge(Constants.CHARGE_QUERY_RESULT, chargeBookModel);
        }else if (resultCode == Activity.RESULT_CANCELED){
            finish();
        }
    }

    public void searchNoDriversLicenceCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.DriversLicence);
    }

    public void searchNoVehicleLicenceCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.VehicleLicence);
    }

    public void searchStopSignCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.StopSign);
    }

    public void searchTrafficSignCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.TrafficSign);
    }

    public void searchSeatBeltCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.Seatbelt);
    }

    public void searchMobilePhoneCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.Cellular);
    }

    public void searchTyreCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.Tyre);
    }

    public void searchRoadWorthyCharges(View view){
        startChargeSearchActivityEx(ChargeQueryType.Roadworthy);
    }

    public void searchChargesByCode(View view){
        startChargeSearchActivityEx(ChargeQueryType.Code);
    }

    public void searchChargesByDescription(){
        startChargeSearchActivityEx(ChargeQueryType.Description);
    }

    public void searchChargesByFavourite(View view){
        startChargeSearchActivityEx(ChargeQueryType.Favourites);
    }

    private void getAllCharges(){
        startChargeSearchActivityEx(ChargeQueryType.All);
    }

    private void startChargeSearchActivityEx(ChargeQueryType chargeQueryType) {
        Intent intent = new Intent(this, ChargeSearchActivityEx.class);
        intent.putExtra(Constants.TICKET_MODEL, mTicket);
        intent.putExtra(Constants.CHARGE_QUERY_TYPE, ChargeQueryType.toInteger(chargeQueryType));
        startActivityForResult(intent, Constants.CHARGE_REQUEST);
    }

    public void returnCharge(String name, ChargeInfoModel chargeBookModel){
        Intent intent = new Intent();
        intent.putExtra(name, chargeBookModel);
        setResult(RESULT_OK, intent);
        finish();
    }

    @Override
    public void onStart(){
        super.onStart();
        //getAllCharges();
        searchChargesByDescription();
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
