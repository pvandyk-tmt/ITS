package za.co.kapsch.iticket;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Handler;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import za.co.kapsch.iticket.Enums.DistanceOverTimeServiceTypeEnum;
import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.ConfigItemModel;
import za.co.kapsch.iticket.Models.CourtInfoModel;
import za.co.kapsch.iticket.Models.DistanceOverTimeSectionConfigurationModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.iticket.Models.InfringementModel;
import za.co.kapsch.iticket.Models.SectionModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.WebAccess.DistanceOverTimeInfringementService;
import za.co.kapsch.iticket.orm.TicketNumberRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

public class DistanceOverTimeActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private static final int KEEP_ALIVE_INTERVAL = 30000;
    private static int MAX_PORT_NUMBER = 65535;
    private static final Pattern IP_ADDRESS
            = Pattern.compile(
            "((25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9])\\.(25[0-5]|2[0-4]"
                    + "[0-9]|[0-1][0-9]{2}|[1-9][0-9]|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]"
                    + "[0-9]{2}|[1-9][0-9]|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}"
                    + "|[1-9][0-9]|[0-9]))");

    private static final int mListSize = 100000;
    private static final int mLookupListSize = 100000;

    private UserModel mUser;
    private CourtInfoModel mCourtInfo;
    private DistrictModel mDistrict;

    private ListView mListView;
    private Intent mServiceIntent;
    private List<SectionModel> mSectionModelList;
    private List<SectionModel> mLookupSectionList;
    private DotInfringementListAdapter mDotInfringementListAdapter;
    private LinearLayout mServiceRequestLayout;
    private TextView mIpAddressAEditText;
    private TextView mIpAddressBEditText;
    private TextView mPortAEditText;
    private TextView mPortBEditText;
    private Button mRequestServiceButton;
    private boolean mActivityStarted;

    private Handler handler = new Handler();

    private TextWatcher textWatcher = new TextWatcher() {

        @Override public void onTextChanged(CharSequence s, int start, int before, int count) {}
        @Override public void beforeTextChanged(CharSequence s,int start,int count,int after) {}

        @Override
        public void afterTextChanged(Editable s) {
            validateUserInterface();
        }
    };

    private BroadcastReceiver mDistanceOverTimeInfringementReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            try {
                switch (intent.getAction()) {
                    case Constants.DOT_INFRINGEMENT_ACTION:
                        SectionModel sectionModel = intent.getParcelableExtra(Constants.DOT_INFRINGEMENT_RESULT);

                        while (mSectionModelList.size() >= mListSize) {
                            mSectionModelList.remove(0);
                        }

                        while (mLookupSectionList.size() >= mLookupListSize) {
                            mLookupSectionList.remove(0);
                        }

                        mSectionModelList.add(sectionModel);
                        mLookupSectionList.add(sectionModel);

                        if (mActivityStarted == true) {
                            Utilities.playNotificationTone();
                        }
                        populateListView(mSectionModelList);
                        break;
                    default:
                        break;
                }
            } catch (Exception e){
                MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
            } finally {
                Utilities.busyProgressBarEx(getThis(), false);
            }
        }
    };

    public DistanceOverTimeActivity getThis(){
        return this;
    }

//    private BroadcastReceiver mDistanceOverTimeInfringementRestartReceiver = new BroadcastReceiver() {
//        @Override
//        public void onReceive(Context context, Intent intent) {
//            switch(intent.getAction()) {
//                case Constants.DOT_INFRINGEMENT_RESTART_ACTION :
//
//                    //stopService(mServiceIntent);
//                    //mServiceIntent = new Intent(App.getContext(), DistanceOverTimeInfringementService.class);
//                    //startService(mServiceIntent);
//
//                    break;
//                default:
//                    break;
//            }
//        }
//    };

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {
            //TODO send keep alive signal();
            handler.postDelayed(this, KEEP_ALIVE_INTERVAL);
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_distance_over_time);

        Intent intent = getIntent();
        mUser = intent.getParcelableExtra(za.co.kapsch.shared.Constants.USER);
        mCourtInfo = intent.getParcelableExtra(Constants.COURT_INFO);
        mDistrict = intent.getParcelableExtra(za.co.kapsch.shared.Constants.DISTRICT);

        mServiceRequestLayout = (LinearLayout)findViewById(R.id.serviceRequestLayout);

        mIpAddressAEditText = (EditText)findViewById(R.id.ipAddressAEditText);
        mIpAddressBEditText = (EditText)findViewById(R.id.ipAddressBEditText);
        mPortAEditText = (EditText)findViewById(R.id.portAEditText);
        mPortBEditText = (EditText)findViewById(R.id.portBEditText);
        mRequestServiceButton = (Button)findViewById(R.id.requestServiceButton);
        mRequestServiceButton.setEnabled(false);

        mListView = (ListView)findViewById(R.id.listView);
        mSectionModelList = new ArrayList<>();
        mLookupSectionList = new ArrayList<>();

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.activity_distance_over_time_title)));

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                startDistanceOverTimeProsecuteActivity(mSectionModelList.get(position));
            }
        });

        mIpAddressAEditText.addTextChangedListener(textWatcher);
        mIpAddressBEditText.addTextChangedListener(textWatcher);
        mPortAEditText.addTextChangedListener(textWatcher);
        mPortBEditText.addTextChangedListener(textWatcher);

        //start timer
        handler.postDelayed(runnable, KEEP_ALIVE_INTERVAL);

//        mListView.setOnTouchListener(new OnSwipeTouchListener(this) {
//            public boolean onSwipeTop() {
//                //MessageManager.showMessage("onSwipeTop", ErrorSeverity.None);
//                return false;
//            }
//            public boolean onSwipeRight() {
//                //MessageManager.showMessage("onSwipeRight", ErrorSeverity.None);
//                return false;
//            }
//            public boolean onSwipeLeft() {
//                //MessageManager.showMessage("onSwipeLeft", ErrorSeverity.None);
//                return false;
//            }
//            public boolean onSwipeBottom() {
//                //MessageManager.showMessage("onSwipeBottom", ErrorSeverity.None);
//                return false;
//            }
//
//        });

    }

    public void requestService(View view){

        startDotService();

        DistanceOverTimeSectionConfigurationModel distanceOverTimeSectionConfiguration = new DistanceOverTimeSectionConfigurationModel();

        distanceOverTimeSectionConfiguration.setServiceType(DistanceOverTimeServiceTypeEnum.File);
        distanceOverTimeSectionConfiguration.setPollMs(10000);
        distanceOverTimeSectionConfiguration.setUniqueIdentifier(Utilities.getDeviceId());

//        distanceOverTimeSectionConfiguration.setPointAIpAddress(mIpAddressAEditText.getText().toString());
//        distanceOverTimeSectionConfiguration.setPointBIpAddress(mIpAddressBEditText.getText().toString());
//        distanceOverTimeSectionConfiguration.setPointAPort(Integer.parseInt(mPortAEditText.getText().toString()));
//        distanceOverTimeSectionConfiguration.setPointBPort(Integer.parseInt(mPortBEditText.getText().toString()));

        DataServiceRequest.distanceOverTimeServiceRequest(this, this, distanceOverTimeSectionConfiguration);

    }

    public void startDotService(){
        IntentFilter intentFilter = new IntentFilter(Constants.DOT_INFRINGEMENT_ACTION);
        LocalBroadcastManager.getInstance(App.getContext()).registerReceiver(mDistanceOverTimeInfringementReceiver, intentFilter);

        //IntentFilter intentRestartFilter = new IntentFilter(Constants.DOT_INFRINGEMENT_RESTART_ACTION);
        //LocalBroadcastManager.getInstance(App.getContext()).registerReceiver(mDistanceOverTimeInfringementRestartReceiver, intentRestartFilter);

        mServiceIntent = new Intent(this, DistanceOverTimeInfringementService.class);
        startService(mServiceIntent);

        Utilities.busyProgressBarEx(this, true);
    }

    private void populateListView(List<SectionModel> sectionModelList){

        try {
            if (mListView.getCount() == 0) {
                Utilities.hideLinearLayout(mServiceRequestLayout);
                Utilities.hideKeyboard(this);
            }

            while (sectionModelList.size() > 5) {//ConfigItemModel.getInstance().getDistanceOverTimeListSize()){
                sectionModelList.remove(0);
            }

            if (sectionModelList.size() < 1) return;

            if (mDotInfringementListAdapter == null) {
                mDotInfringementListAdapter = new DotInfringementListAdapter(this, mListView, sectionModelList);
                mListView.setAdapter(mDotInfringementListAdapter);
            } else {
                mListView.invalidate();
                mDotInfringementListAdapter.notifyDataSetChanged();
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    private void startWizardActivity(SectionModel section){
        TicketModel ticket = getNewTicket(DocumentType.RoadSideDriver);
        if (ticket == null) return;

        Intent intent = new Intent(this, WizardActivity.class);
        intent.putExtra(Constants.TICKET_MODEL, ticket);
        startActivity(intent);
    }

    private void startDistanceOverTimeProsecuteActivity(SectionModel section){
        try {
            Intent intent = new Intent(this, DistanceOverTimeProsecuteActivity.class);
            intent.putExtra(Constants.SECTION_MODEL, section);
            startActivityForResult(intent, Constants.DISTANCE_OVER_TIME_REQUEST_CODE);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

//    private void hideLinearLayout(LinearLayout linearLayout){
//
//        try {
//            LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
//            layoutParams.height = 0;
//            layoutParams.width = 0;
//            linearLayout.setLayoutParams(layoutParams);
//        }catch (Exception e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "DistanceOverTimeActivity::hideLinearLayout()"), ErrorSeverity.High);
//        }
//    }

//    private void showLinearLayout(LinearLayout linearLayout){
//
//        LinearLayout.LayoutParams layoutParams = (LinearLayout.LayoutParams) linearLayout.getLayoutParams();
//        layoutParams.height = ViewGroup.LayoutParams.WRAP_CONTENT;
//        layoutParams.width = ViewGroup.LayoutParams.MATCH_PARENT;
//        linearLayout.setLayoutParams(layoutParams);
//    }

    private void validateUserInterface(){

        if (TextUtils.isEmpty(mPortAEditText.getText()) ||
            TextUtils.isEmpty(mPortBEditText.getText())){
            mRequestServiceButton.setEnabled(false);
            return;
        }

        Matcher ipAMatcherA = IP_ADDRESS.matcher(mIpAddressAEditText.getText().toString());
        Matcher ipAMatcherB = IP_ADDRESS.matcher(mIpAddressBEditText.getText().toString());

        mRequestServiceButton.setEnabled(
                ipAMatcherA.matches() &&
                ipAMatcherB.matches() &&
                Integer.parseInt(mPortAEditText.getText().toString()) <= MAX_PORT_NUMBER &&
                Integer.parseInt(mPortBEditText.getText().toString()) <= MAX_PORT_NUMBER &&
                Integer.parseInt(mPortAEditText.getText().toString()) >= 0 &&
                Integer.parseInt(mPortAEditText.getText().toString()) >= 0);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {

        if (resultCode == Activity.RESULT_OK) {
            SectionModel section;
            section = data.getParcelableExtra(Constants.DISTANCE_OVER_TIME_RESULT);

            startWizardActivity(section);
        }
    }

    @Override
    public void onStart()
    {
        super.onStart();
        mActivityStarted = true;
        validateUserInterface();

//        if (mCameraConfigSubmitted == true) {
//            startDotService();
//        }
    }

    public void onStop() {
        super.onStop();

        mActivityStarted = false;

//        if (mServiceIntent != null) {
//            stopService(mServiceIntent);
//        }

//        if (mDistanceOverTimeInfringementReceiver != null) {
//            LocalBroadcastManager.getInstance(this).unregisterReceiver(mDistanceOverTimeInfringementReceiver);
//        }

//        if (mDistanceOverTimeInfringementRestartReceiver != null) {
//            LocalBroadcastManager.getInstance(this).unregisterReceiver(mDistanceOverTimeInfringementRestartReceiver);
//        }
    }

    @Override
    public void onDestroy(){

        if (mServiceIntent != null) {
            stopService(mServiceIntent);
        }

        if (mDistanceOverTimeInfringementReceiver != null) {
            LocalBroadcastManager.getInstance(this).unregisterReceiver(mDistanceOverTimeInfringementReceiver);
        }

        super.onDestroy();
    }

    private TicketModel getNewTicket(DocumentType documentType) {

        try {

            TicketNumberModel ticketNumber = TicketNumberRepository.getNextTicketNumber(documentType);
            String numberValue = ticketNumber.getNumberValue();
            String externalToken = ticketNumber.getExternalToken();
            String externalTokenReference  = ticketNumber.getExternalTokenReference();

            if (ticketNumber == null) {
                MessageManager.showMessage(
                        String.format(
                                getString(R.string.no_more_tickets_available),
                                DocumentType.toString(documentType)),
                        ErrorSeverity.None);
                return null;
            }

            TicketModel ticket = new TicketModel();

            InfringementModel infringement = new InfringementModel();
            infringement.setTicketNumber(numberValue);
            infringement.setExternalToken(externalToken);
            infringement.setExternalTokenReference(externalTokenReference);
            infringement.setOffenceDate(Utilities.getDateAddMinutes(ConfigItemModel.getInstance().getOffenceMinutesFromNow()));
            ticket.setInfringement(infringement);

            ticket.setUser(mUser);
            ticket.setCourtInfo(mCourtInfo);
            ticket.setDistrict(mDistrict);
            ticket.setDocumentType(documentType);

            return ticket;

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::getNewTicket()"), ErrorSeverity.Medium);
            return null;
        }
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
        try {

            if (asyncResultModel == null){
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_DISTANCE_OVER_TIME_SERVICE_REQUEST:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            //displayMessage(this.getResources().getString(R.string.data_service_registration_completed), true);
                            return;
                        case FAILED:
                            //displayMessage(asyncResultModel.getMessage(), true);
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
