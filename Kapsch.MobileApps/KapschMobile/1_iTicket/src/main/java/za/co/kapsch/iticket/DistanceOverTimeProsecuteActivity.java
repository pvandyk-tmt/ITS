package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Base64;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import za.co.kapsch.iticket.Models.SectionModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

public class DistanceOverTimeProsecuteActivity extends AppCompatActivity {

    private static final String START_IMAGE = "START_IMAGE";
    private static final String END_IMAGE = "END_IMAGE";
    private static final String PLATE_IMAGE = "PLATE_IMAGE";

    private ImageView mImageView;
    private TextView mSpeedTextView;
    private TextView mRegistrationNumberTextView;
    private TextView mZoneTextView;
    private TextView mVehicleClassTextView;

    private SectionModel mSectionModel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_distance_over_time_prosecute);

        mImageView = (ImageView) findViewById (R.id.imageView);
        mSpeedTextView = (TextView) findViewById (R.id.speedTextView);
        mRegistrationNumberTextView = (TextView) findViewById (R.id.registrationTextView);
        mZoneTextView = (TextView) findViewById (R.id.zoneTextView);
        mVehicleClassTextView = (TextView) findViewById (R.id.vehicleClassTextView);

        Intent intent = getIntent();
        mSectionModel = intent.getParcelableExtra(Constants.SECTION_MODEL);

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        try {
            mImageView.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                switch ((String) view.getTag()) {
                    case START_IMAGE:
                        mImageView.setImageBitmap((Utilities.byteArrayToBitmap(Base64.decode(mSectionModel.getSectionPointEnd().getImage(), Base64.DEFAULT))));
                        mImageView.setTag(END_IMAGE);
                        break;
                    case END_IMAGE:
                        //mImageView.setImageBitmap((Utilities.byteArrayToBitmap(Base64.decode(mSectionModel.getSectionPointStart().getImage(), Base64.DEFAULT))));
                        //mImageView.setTag(START_IMAGE);
                        MessageManager.showMessage("Not Implemented", ErrorSeverity.None);
                        break;
                        case PLATE_IMAGE:
//                            mImageView.setImageBitmap((Utilities.byteArrayToBitmap(Base64.decode(mSectionModel.getSectionPointStart().getImage(), Base64.DEFAULT))));
//                            break;
                }
                }
            });
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }

        populateFields();
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

    private void populateFields(){

        if (mSectionModel.getSectionPointStart() == null){
            MessageManager.showMessage("DistanceOverTimeProsecute::populateFields() SectionPointA is null", ErrorSeverity.None);
            return;
        }

        mImageView.setImageBitmap((Utilities.byteArrayToBitmap(Base64.decode(mSectionModel.getSectionPointStart().getImage(), Base64.DEFAULT))));
        mImageView.setTag(START_IMAGE);
        mSpeedTextView.setText(String.format("%s %.2f", getString(R.string.activity_distance_over_time_speed), mSectionModel.getAverageSpeed()));
        mRegistrationNumberTextView.setText(String.format("%s %s", getString(R.string.activity_distance_over_time_license), mSectionModel.getVln()));
        mZoneTextView.setText(String.format("%s %s", getString(R.string.activity_distance_over_time_zone), Integer.toString(mSectionModel.getZone())));
        mVehicleClassTextView.setText(String.format("%s %s", getString(R.string.activity_distance_over_time_vehicle_class), mSectionModel.getSectionPointEnd().getClassification().mClassification.name()));

//        mSpeedTextView.setText(String.format("%s: %.2f", getString(R.string.activity_distance_over_time_speed), mSectionModel.getAverageSpeed()));
//        mRegistrationNumberTextView.setText(String.format("%s: %s", getString(R.string.activity_distance_over_time_license), mSectionModel.getVln()));
//        mZoneTextView.setText(String.format("%s: %s", getString(R.string.activity_distance_over_time_zone), Integer.toString(mSectionModel.getZone())));
//        mVehicleClassTextView.setText(String.format("%s: %s", getString(R.string.activity_distance_over_time_vehicle_class), mSectionModel.getSectionPointEnd().getClassification().mClassification.name()));
    }

    public void procesuteSection56(View view){
        Intent intent = new Intent();
        intent.putExtra(Constants.DISTANCE_OVER_TIME_RESULT, mSectionModel);
        setResult(RESULT_OK, intent);
        finish();
    }
}
