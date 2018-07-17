package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.iCam.ICamInfringement;
import za.co.kapsch.iticket.orm.ChargeInfoRepository;

public class ICamOffenceCodeListActivity extends AppCompatActivity {

    private ListView mListView;
    private ICamInfringement mICamInfringement;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_icam_offence_code_list);
        mListView = (ListView)findViewById(R.id.listView);

        Intent intent = getIntent();
        mICamInfringement = intent.getParcelableExtra(Constants.ICAM_INFRINGEMENT);

        mListView.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {
                view.setSelected(true);
                ChargeInfoModel offenceCode = (ChargeInfoModel) mListView.getItemAtPosition(position);
                offenceCode.setDescription(substituteDiscriptionPlaceHolders(offenceCode.getDescription(), mICamInfringement.getSpeed(), mICamInfringement.getZone()));
                returnOffenceCodeModel(offenceCode);
                return true;
            }
        });
    }

    private String substituteDiscriptionPlaceHolders(String description, String speed, String zone){

        description = description.replace(Constants.SPEED_PLACE_HOLDER, speed);
        description = description.replace(Constants.ZONE_PLACE_HOLDER, zone);

       return description;
    }

    private void populateListView(List<ChargeInfoModel> offenceCodeList){
        if (offenceCodeList.size() < 1) return;
        OffenceCodeListAdapter offenceCodeListAdapter = new OffenceCodeListAdapter(this, offenceCodeList);
        mListView.setAdapter(offenceCodeListAdapter);
    }

    public void returnOffenceCodeModel(ChargeInfoModel offenceCode){
        Intent intent = new Intent();
        intent.putExtra(Constants.CHARGE_CODE, offenceCode);
        setResult(RESULT_OK, intent);
        finish();
    }

    @Override
    public void onStart()
    {
        super.onStart();

        try {
            List<ChargeInfoModel> chargeInfoModelList = ChargeInfoRepository.getChargeByZoneAndSpeed(
                    Integer.parseInt(mICamInfringement.getZone()),
                    Integer.parseInt(mICamInfringement.getSpeed()));

            populateListView(chargeInfoModelList);

        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
