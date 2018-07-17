package za.co.kapsch.iticket;

import android.support.design.widget.TabLayout;
import android.support.v4.app.FragmentActivity;
import android.support.v4.view.ViewPager;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.Enums.ViolationCategory;
import za.co.kapsch.iticket.Models.SwipeAdapterTabTitle;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

public class OutstandingViolationsActivity extends FragmentActivity implements IAsyncProcessCallBack {

    private ViewPager mViewPager;
    private TabLayout mTabLayout;

//    private FineModel mFineModel;
    ArrayList<FineModel> mOutstandingPersonViolationList;
    ArrayList<FineModel> mOutstandingVehicleViolationList;

//    AdapterView.OnItemClickListener mOnClickListener = new AdapterView.OnItemClickListener()
//    {
//        @Override
//        public void onItemClick(AdapterView<?> parent, final View view, int position, long id)
//        {
//            try {
//                view.setSelected(true);
//                mFineModel = (FineModel)mViewPager.
//                    //mHandWritten = (HandWrittenModel) mListView.getItemAtPosition(position);
//            }catch (Exception e){
//                String error = e.getMessage();
//                MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::mListView.setOnItemClickListener()"), ErrorSeverity.High);
//            }
//        }
//    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_outstanding_violations);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.outstanding_violations)));

        mViewPager = (ViewPager)findViewById(R.id.view_pager);
        mTabLayout = (TabLayout)findViewById(R.id.tabLayout);

        try{
            mOutstandingPersonViolationList = (ArrayList<FineModel>) getIntent().getSerializableExtra(Constants.OUTSTANDING_PERSON_VIOLATIONS);
            mOutstandingVehicleViolationList = (ArrayList<FineModel>) getIntent().getSerializableExtra(Constants.OUTSTANDING_VEHICLE_VIOLATIONS);
        }catch (Exception e){
            MessageManager.showMessage(e.getMessage(), ErrorSeverity.High);
        }

        OutstandingViolationsSwipeAdapter outstandingViolationsSwipeAdapter = new OutstandingViolationsSwipeAdapter(getSupportFragmentManager(), getSwipeAdapterTabTitle());
        mViewPager.setAdapter(outstandingViolationsSwipeAdapter);

        mTabLayout.setupWithViewPager(mViewPager);
        mTabLayout.setOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                mViewPager.setCurrentItem(tab.getPosition());
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {

            }

            @Override
            public void onTabReselected(TabLayout.Tab tab) {

            }

        });
    }

    public List<FineModel> getOutstandingPersonViolationList(){
        return mOutstandingPersonViolationList;
    }

    public List<FineModel> getOutstandingVehicleViolationList(){
        return mOutstandingVehicleViolationList;
    }

    public SwipeAdapterTabTitle[] getSwipeAdapterTabTitle(){

        if ((mOutstandingPersonViolationList.size() > 0) && (mOutstandingVehicleViolationList.size() > 0)){

            SwipeAdapterTabTitle[] swipeAdapterTabTitles =
                    {
                            getSwipeAdapterTabTitle(App.getContext().getResources().getString(R.string.person_ex), ViolationCategory.toInteger(ViolationCategory.Person)),
                            getSwipeAdapterTabTitle(App.getContext().getResources().getString(R.string.vehicle), ViolationCategory.toInteger(ViolationCategory.Vehicle))
                    };

            return swipeAdapterTabTitles;
        }

        if ((mOutstandingPersonViolationList.size() > 0) && (mOutstandingVehicleViolationList.size() == 0)){

            SwipeAdapterTabTitle[] swipeAdapterTabTitles =
                    {
                            getSwipeAdapterTabTitle(App.getContext().getResources().getString(R.string.person_ex), ViolationCategory.toInteger(ViolationCategory.Person)),
                    };

            return swipeAdapterTabTitles;
        }

        if ((mOutstandingPersonViolationList.size() == 0) && (mOutstandingVehicleViolationList.size() > 0)){

            SwipeAdapterTabTitle[] swipeAdapterTabTitles =
                    {
                            getSwipeAdapterTabTitle(App.getContext().getResources().getString(R.string.vehicle), ViolationCategory.toInteger(ViolationCategory.Vehicle))
                    };

            return swipeAdapterTabTitles;
        }

        return null;
    }

    public SwipeAdapterTabTitle getSwipeAdapterTabTitle(String tabTitle, int tabIdentifier){

        SwipeAdapterTabTitle swipeAdapterTabTitle = new SwipeAdapterTabTitle();
        swipeAdapterTabTitle.setTabTitle(tabTitle);
        swipeAdapterTabTitle.setTabIdentifier(tabIdentifier);

        return swipeAdapterTabTitle;
    }

    public boolean getEvidence(long evidenceId){
        DataServiceRequest.getEvidenceRequest(this, this, evidenceId, Constants.PROCESS_ID_GET_OFFICER_SIGNATURE_EVIDENCE);
        return true;
    }

    public void printTicketClick(View view){

    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try {

            if (asyncResultModel == null) {
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_GET_OFFICER_SIGNATURE_EVIDENCE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            byte[] signature = (byte[])asyncResultModel.getObject();
                            return;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_GET_OFFENDER_SIGNATURE_EVIDENCE:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            byte[] signature = (byte[])asyncResultModel.getObject();
                            return;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.None);
                            break;
                }
                break;
            }
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            return;
        }
    }
}

