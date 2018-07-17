package za.co.kapsch.iticket;

import android.content.Intent;
import android.os.Parcelable;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Adapter;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Spinner;
import android.widget.TextView;

import com.toptoche.searchablespinnerlibrary.SearchableSpinner;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Interfaces.IDelimitation;
import za.co.kapsch.iticket.Interfaces.IDelimitationRepository;
import za.co.kapsch.iticket.Models.DelimitationResultModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

public class DelimitationSelectionActivity extends AppCompatActivity {

    private Parcelable mSelectedLevelOneObject;
    private Parcelable mSelectedLevelTwoObject;
    private Parcelable mSelectedLevelThreeObject;
    private Parcelable mSelectedLevelFourObject;
    private Parcelable mSelectedLevelFiveObject;
    private DelimitationResultModel mDelimitationResultModel;

    private TextView mTextViewOne;
    private Spinner mSpinnerOne;

    private TextView mTextViewTwo;
    private Spinner mSpinnerTwo;

    private TextView mTextViewThree;
    private Spinner mSpinnerThree;

    private TextView mTextViewFour;
    private Spinner mSpinnerFour;

    private TextView mTextViewFive;
    private Spinner mSpinnerFive;

    private DelimitationDescriptor mDelimitationDescriptor;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_delimitation_selection);

        mTextViewOne = (TextView)findViewById(R.id.textView1);
        mSpinnerOne = (SearchableSpinner)findViewById(R.id.spinner1);
        mTextViewTwo = (TextView)findViewById(R.id.textView2);
        mSpinnerTwo = (SearchableSpinner)findViewById(R.id.spinner2);
        mTextViewThree = (TextView)findViewById(R.id.textView3);
        mSpinnerThree = (SearchableSpinner)findViewById(R.id.spinner3);
        mTextViewFour = (TextView)findViewById(R.id.textView4);
        mSpinnerFour = (SearchableSpinner)findViewById(R.id.spinner4);
        mTextViewFive = (TextView)findViewById(R.id.textView5);
        mSpinnerFive = (SearchableSpinner)findViewById(R.id.spinner5);

        Intent intent = getIntent();
        mDelimitationDescriptor = intent.getParcelableExtra(Constants.DELIMITATION_DATA);
        mDelimitationResultModel = mDelimitationDescriptor.getDelimitationResult();

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        mSpinnerOne.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                try{
                     mSelectedLevelOneObject = (Parcelable)parent.getItemAtPosition(position);
                    if (mDelimitationDescriptor.isCascade() == true) {
                        mSelectedLevelTwoObject = null;
                        mSelectedLevelThreeObject = null;
                        mSelectedLevelFourObject = null;
                        mSelectedLevelFiveObject = null;
                    }

                     if (levelTwoUsed()) {
                         List<IDelimitation> list = null;
                         if (mDelimitationDescriptor.isCascade()) {
                             list = getDelimitationRepository().getSecondLevel(((IDelimitation) mSelectedLevelOneObject).getId());
                         }else{
                             Adapter adapter = mSpinnerTwo.getAdapter();
                             if (adapter == null) {
                                 list = getDelimitationRepository().getSecondLevel(-1);
                             }
                         }
                         if (list != null) {
                             populateSpinner(mSpinnerTwo, list);
                             setSpinnerValue(mSpinnerTwo, (IDelimitation) mDelimitationResultModel.getLevelTwoObject());
                             mDelimitationResultModel.setLevelTwoObject(null);
                         }
                     }
                }catch(SQLException e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "DelimitationSelectionActivity:mSpinnerOne.setOnItemSelectedListener()"), ErrorSeverity.High);
                }
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        mSpinnerTwo.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                try{
                    mSelectedLevelTwoObject = (Parcelable)parent.getItemAtPosition(position);
                    if (mDelimitationDescriptor.isCascade() == true) {
                        mSelectedLevelThreeObject = null;
                        mSelectedLevelFourObject = null;
                        mSelectedLevelFiveObject = null;
                    }

                    if (levelThreeUsed()) {
                        List<IDelimitation> list = null;
                        if (mDelimitationDescriptor.isCascade()) {
                            list = getDelimitationRepository().getThirdLevel(((IDelimitation) mSelectedLevelTwoObject).getId());
                        }else{
                            Adapter adapter = mSpinnerThree.getAdapter();
                            if (adapter == null) {
                                list = getDelimitationRepository().getThirdLevel(-1);
                            }
                        }
                        if (list != null) {
                            populateSpinner(mSpinnerThree, list);
                            setSpinnerValue(mSpinnerThree, (IDelimitation) mDelimitationResultModel.getLevelThreeObject());
                            mDelimitationResultModel.setLevelThreeObject(null);
                        }
                    }
                }catch(SQLException e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "DelimitationSelectionActivity:mSpinnerTwo.setOnItemSelectedListener()"), ErrorSeverity.High);
                }
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        mSpinnerThree.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                try{
                    mSelectedLevelThreeObject = (Parcelable)parent.getItemAtPosition(position);
                    if (mDelimitationDescriptor.isCascade() == true) {
                        mSelectedLevelFourObject = null;
                        mSelectedLevelFiveObject = null;
                    }

                    if (levelFourUsed()) {
                        List<IDelimitation> list = null;
                        if (mDelimitationDescriptor.isCascade()) {
                            list = getDelimitationRepository().getThirdLevel(((IDelimitation)mSelectedLevelThreeObject).getId());
                        }else {
                            Adapter adapter = mSpinnerFour.getAdapter();
                            if (adapter == null) {
                                list = getDelimitationRepository().getThirdLevel(-1);
                            }
                        }
                        if (list != null) {
                            populateSpinner(mSpinnerFour, list);
                            setSpinnerValue(mSpinnerFour, (IDelimitation) mDelimitationResultModel.getLevelFourObject());
                            mDelimitationResultModel.setLevelFourObject(null);
                        }
                    }
                }catch(SQLException e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "DelimitationSelectionActivity:mSpinnerThree.setOnItemSelectedListener()"), ErrorSeverity.High);
                }
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        mSpinnerFour.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                try{
                    mSelectedLevelFourObject = (Parcelable)parent.getItemAtPosition(position);
                    if (mDelimitationDescriptor.isCascade() == true) {
                        mSelectedLevelFiveObject = null;
                    }

                    if (levelFourUsed()) {
                        List<IDelimitation> list = null;
                        if (mDelimitationDescriptor.isCascade()) {
                            list = getDelimitationRepository().getFourthLevel(((IDelimitation) mSelectedLevelFourObject).getId());
                        }else{
                            Adapter adapter = mSpinnerFive.getAdapter();
                            if (adapter == null) {
                                list = getDelimitationRepository().getFourthLevel(-1);
                            }
                        }
                        if (list != null) {
                            populateSpinner(mSpinnerFive, list);
                            setSpinnerValue(mSpinnerFive, (IDelimitation) mDelimitationResultModel.getLevelFiveObject());
                            mDelimitationResultModel.setLevelFiveObject(null);
                        }
                    }
                }catch(SQLException e){
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "DelimitationSelectionActivity:mSpinnerFour.setOnItemSelectedListener()"), ErrorSeverity.High);
                }
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        mSpinnerFive.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                mSelectedLevelFiveObject = (Parcelable)parent.getItemAtPosition(position);
            }

            public void onNothingSelected(AdapterView<?> parent) {

            }
        });

        try{
            populateSpinner(mSpinnerOne, (List<IDelimitation>)getDelimitationRepository().getFirstLevel());
            setSpinnerValue(mSpinnerOne, (IDelimitation) mDelimitationResultModel.getLevelOneObject());
        }catch(SQLException e){
            e.printStackTrace();
        }

        setupScreen();
    }

    private void setupScreen(){

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                mDelimitationDescriptor.getTitle()));

        mTextViewOne.setText(mDelimitationDescriptor.getLevelOneLabelText());
        mTextViewTwo.setText(mDelimitationDescriptor.getLevelTwoLabelText());
        mTextViewThree.setText(mDelimitationDescriptor.getLevelThreeLabelText());
        mTextViewFour.setText(mDelimitationDescriptor.getLevelFourLabelText());
        mTextViewFive.setText(mDelimitationDescriptor.getLevelFiveLabelText());

        mTextViewOne.setVisibility(levelOneUsed() ? View.VISIBLE : View.INVISIBLE);
        mTextViewTwo.setVisibility(levelTwoUsed() ? View.VISIBLE : View.INVISIBLE);
        mTextViewThree.setVisibility(levelThreeUsed() ? View.VISIBLE : View.INVISIBLE);
        mTextViewFour.setVisibility(levelFourUsed() ? View.VISIBLE : View.INVISIBLE);
        mTextViewFive.setVisibility(levelFiveUsed() ? View.VISIBLE : View.INVISIBLE);

        mSpinnerOne.setVisibility(levelOneUsed() ? View.VISIBLE : View.INVISIBLE);
        mSpinnerTwo.setVisibility(levelTwoUsed() ? View.VISIBLE : View.INVISIBLE);
        mSpinnerThree.setVisibility(levelThreeUsed() ? View.VISIBLE : View.INVISIBLE);
        mSpinnerFour.setVisibility(levelFourUsed() ? View.VISIBLE : View.INVISIBLE);
        mSpinnerFive.setVisibility(levelFiveUsed() ? View.VISIBLE : View.INVISIBLE);

//        if (mDelimitationDescriptor.getLevelThreeVisibility() == false){
//            mTextViewThree.setVisibility(View.INVISIBLE);
//            mSpinnerThree.setVisibility(View.INVISIBLE);
//        }
    }

    private void setSpinnerValue(Spinner spinner, IDelimitation delimitation){

        if(delimitation == null) return;

        int position = Utilities.indexOf(spinner.getAdapter(), delimitation.toString());

        if (position == -1) position = 0;

        spinner.setSelection(position);
    }


    private boolean levelOneUsed(){
        return (mDelimitationDescriptor.getLevelOneLabelText() != null);
    }

    private boolean levelTwoUsed(){
        return (mDelimitationDescriptor.getLevelTwoLabelText() != null);
    }

    private boolean levelThreeUsed(){
        return (mDelimitationDescriptor.getLevelThreeLabelText() != null);
    }

    private boolean levelFourUsed(){
        return (mDelimitationDescriptor.getLevelFourLabelText() != null);
    }

    private boolean levelFiveUsed(){
        return (mDelimitationDescriptor.getLevelFiveLabelText() != null);
    }

    private <T> void populateSpinner(Spinner spinner, List<T> list){
        //if (list.size() < 1) return;
        ArrayAdapter adapter = null;
        adapter = new ArrayAdapter<T>(this, R.layout.spinner_item, list);
        spinner.setAdapter(adapter);
    }

    private IDelimitationRepository getDelimitationRepository(){
        return mDelimitationDescriptor.getDelimitationRepository();
    }

    public void save(View view){
        try{
            mDelimitationDescriptor.getDelimitationResult().setLevelOneObject((Parcelable) mSelectedLevelOneObject);
            mDelimitationDescriptor.getDelimitationResult().setLevelTwoObject((Parcelable) mSelectedLevelTwoObject);
            mDelimitationDescriptor.getDelimitationResult().setLevelThreeObject((Parcelable) mSelectedLevelThreeObject);
            mDelimitationDescriptor.getDelimitationResult().setLevelFourObject((Parcelable) mSelectedLevelFourObject);
            mDelimitationDescriptor.getDelimitationResult().setLevelFiveObject((Parcelable) mSelectedLevelFiveObject);

            returnDelimitationData();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DelimitationSelectionActivity::save()"), ErrorSeverity.High);
        }
   }

    public void returnDelimitationData(){

        if (levelOneUsed() == true){
            if (mSelectedLevelOneObject == null) return;
        }

        if (levelTwoUsed() == true){
            if (mSelectedLevelTwoObject == null) return;
        }

        if (levelThreeUsed() == true){
            if (mSelectedLevelThreeObject == null) return;
        }

        if (levelFourUsed() == true){
            if (mSelectedLevelFourObject == null) return;
        }

        if (levelFiveUsed() == true){
            if (mSelectedLevelFiveObject == null) return;
        }

        Intent intent = new Intent();
        intent.putExtra(Constants.DELIMITATION, mDelimitationDescriptor.getDelimitationResult());
        setResult(RESULT_OK, intent);
        finish();
    }

//    public void returnDelimitationData(){
//
//        if (mDelimitationResultModel instanceof CourtInfoModel){
//            if (((CourtInfoModel)mDelimitationResultModel).getCourtDate() == null){
//                return;
//            }
//
//            if (((CourtInfoModel)mDelimitationResultModel).getCourtDate().getDate().after((Calendar.getInstance().getTime())) == false){
//                MessageManager.showMessage(getResources().getString(R.string.activity_court_delimitation_court_date_invalid_message), ErrorSeverity.None);
//                return;
//            }
//        }else if (mDelimitationResultModel instanceof OpusVehicleInfoModel){
//            if ((mSelectedLevelOneObject == null) || (mSelectedLevelTwoObject == null) || (mSelectedLevelThreeObject == null)){
//                return;
//            }
//        }
//
//        Intent intent = new Intent();
//        intent.putExtra(Constants.DELIMITATION, mDelimitationDescriptor.getDelimitationResult());
//        setResult(RESULT_OK, intent);
//        finish();
//    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onBackPressed(){
//        if (mDelimitationResultModel instanceof OpusVehicleInfoModel){
            super.onBackPressed();
//        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
