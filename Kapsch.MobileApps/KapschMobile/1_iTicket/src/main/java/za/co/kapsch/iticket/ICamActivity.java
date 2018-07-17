package za.co.kapsch.iticket;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Handler;
import android.support.v4.content.LocalBroadcastManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import microsoft.aspnet.signalr.client.ConnectionState;
import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.VosiActionType;
import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.iticket.iCam.ICamVlns;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.iticket.Models.ChargeInfoModel;
import za.co.kapsch.iticket.Models.CourtInfoModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.iCam.ICamClient;
import za.co.kapsch.iticket.iCam.ICamEvent;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

import static za.co.kapsch.iticket.Constants.DATETIME_FORMAT_EX;

public class ICamActivity extends AppCompatActivity implements IAsyncProcessCallBack {

    private static final String mPort = "7001";
    private static final String mIpAddressPartial = "172.28.48.";

    private static final String CamReachable = "CAM REACHABLE";
    private static final String CamUnReachable = "CAM UNREACHABLE";

    private UserModel mUser;
    private CourtInfoModel mCourtInfo;
    private DistrictModel mDistrict;
    private int mPassageCounter;
    private TextView mPassageCounterTextView;
    private ICamListAdapter mICamListAdapter;
    private TextView mPortTextView;
    private TextView mIpAddressTextView;
    private TextView mPipeStatusTextView;
    private ListView mListView;
    private Button mConnectButton;
    private ArrayList<ICamEvent> mICamEventList;
    private Handler mHandler = new Handler();
    private boolean mReconnect;

    private Runnable mRunnableDisconnect = new Runnable() {
        @Override
        public void run() {
            disconnect();
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_icam);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.camera_prosecution)));

        Intent intent = getIntent();
        mUser = intent.getParcelableExtra(za.co.kapsch.shared.Constants.USER);
        mCourtInfo = intent.getParcelableExtra(Constants.COURT_INFO);
        mDistrict = intent.getParcelableExtra(za.co.kapsch.shared.Constants.DISTRICT);

        mPassageCounterTextView = (TextView)findViewById(R.id.passageCounterTextView);
        mListView = (ListView)findViewById(R.id.listView);
        mPortTextView = (TextView)findViewById(R.id.iCamPortEditText);
        mIpAddressTextView = (TextView)findViewById(R.id.iCamIpAddressEditText);
        mConnectButton = (Button)findViewById(R.id.connectButton);
        mPipeStatusTextView = (TextView)findViewById(R.id.pipeStatusTextView);

        mICamEventList = new ArrayList<>();

        mListView.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {
                view.setSelected(true);
                ICamEvent iCamEvent = (ICamEvent) mListView.getItemAtPosition(position);
                iCamAction(iCamEvent);
                return true;
            }
        });

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                view.setSelected(true);
                mListView.setSelector(R.color.selectColor);
            }
        });

        mPortTextView.setText(mPort);
        mIpAddressTextView.setText(mIpAddressPartial);
        mPipeStatusTextView.setText(Constants.EMPTY_STRING);
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {

        switch (asyncResultModel.getProcessResult()) {

            case ICamClient.SUCCESS:

                switch (asyncResultModel.getProcessId()) {
                    case ICamClient.REACHABLE:
                        mPipeStatusTextView.setText(CamReachable);
                        mPipeStatusTextView.setTextColor(Color.GREEN);
                        break;
                    case ICamClient.CONNECT:
                        setConnectedState(true);
                        Utilities.busyProgressBarEx(this, false);
                        break;
                    case ICamClient.PASSAGE:
                        passageCounter(false);
                        break;
                    case ICamClient.EVENT:
                        ICamEvent iCamEvent = (ICamEvent) asyncResultModel.getObject();
                        mICamEventList.add(iCamEvent);
                        populateListView(mICamEventList);
                        Utilities.playNotificationTone();
                        break;
                    case ICamClient.CLOSE_SOCKET:
                        setConnectedState(false);
                        Utilities.busyProgressBarEx(this, false);
                        if (mReconnect == true){
                            mReconnect = false;
                            connect();
                        }
                        break;
                }
                break;

            case ICamClient.FAILED:

                switch (asyncResultModel.getProcessId()) {
                    case ICamClient.REACHABLE:
                        mPipeStatusTextView.setText(CamUnReachable);
                        mPipeStatusTextView.setTextColor(Color.RED);
                        break;
                    case ICamClient.CONNECT:
                        Utilities.busyProgressBarEx(this, false);
                        break;
                    case ICamClient.CLOSE_SOCKET:
                        mPipeStatusTextView.setText(Constants.EMPTY_STRING);
                        Utilities.busyProgressBarEx(this, false);
                        break;
                    case ICamClient.EVENT:
                        break;
                    default:
                        mPipeStatusTextView.setText(Constants.EMPTY_STRING);
                        MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                        setConnectedState(false);
                        if (asyncResultModel.getMessage().contains("SocketTimeoutException") == true){
                            connectToCamera();
                        }
                        break;
                }
        }
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {
    }

    private void populateListView(List<ICamEvent> iCamEventList){
        if (iCamEventList.size() < 1) return;
        if (mICamListAdapter == null) {
            mICamListAdapter = new ICamListAdapter(this, mICamEventList);
            mListView.setAdapter(mICamListAdapter);
        }else{
            mListView.invalidate();
            mICamListAdapter.notifyDataSetChanged();
        }
    }

    private void setConnectedState(boolean connected){
        mPortTextView.setEnabled(!connected);
        mIpAddressTextView.setEnabled(!connected);

        String connectButtonText = connected ?  App.getContext().getResources().getString(R.string.activity_icam_disconnect_button_text) :
                App.getContext().getResources().getString(R.string.activity_icam_connect_button_text);

        mConnectButton.setText(connectButtonText);
        mConnectButton.setEnabled(true);
        mPipeStatusTextView.setText(Constants.EMPTY_STRING);

        if (connected == false){
            mICamListAdapter = null;
            mICamEventList.clear();
            passageCounter(true);
            mListView.setAdapter(mICamListAdapter);
        }
    }

    public void connectClick(View view){
        connectToCamera();
    }

    private void connectToCamera(){

        try {
            if (mConnectButton.getText().toString().equals(App.getContext().getResources().getString(R.string.activity_icam_connect_button_text))) {
                connect();
            }
            else if (mConnectButton.getText().toString().equals(App.getContext().getResources().getString(R.string.activity_icam_disconnect_button_text))) {
                disconnect();
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ICamActivity::connectToCamera()"), ErrorSeverity.High);
        }
    }

    private void connect(){
        mConnectButton.setEnabled(false);
        ICamClient.getInstance(this, this).run(mIpAddressTextView.getText().toString(), Integer.parseInt(mPortTextView.getText().toString()));
    }

    private void disconnect(){
        mConnectButton.setEnabled(false);
        ICamClient.getInstance(this, this).closeSocket(true);
        passageCounter(true);
    }

//    private void returnOffenceCode(ChargeInfoModel offenceCode){
//        Intent intent = new Intent();
//        intent.putExtra(Constants.CHARGE_CODE, offenceCode);
//        setResult(RESULT_OK, intent);
//        //if (mICamClient != null) {
//            ICamClient.getInstance(this, this).closeSocket(true);
//        //}
//        finish();
//    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == Activity.RESULT_OK) {
            switch (requestCode) {

                case Constants.PROCESS_ID_VOSI_ACTION:
                    String vln = data.getStringExtra(Constants.ICAM_VLNS);
                    long vosiActionType = data.getLongExtra(Constants.VOSI_ACTION_TYPE, -1);
                    if (vosiActionType == VosiActionType.UnpaidInfringement.getCode()){
                        startFineSearchInfoValidation(vln);
                    }
                    break;
                default:
                    break;
            }
        }


//        switch (requestCode) {

//            case Constants.CHARGE_REQUEST:
//                if (resultCode == Activity.RESULT_OK) {
//                    ChargeInfoModel chargeInfoModel = data.getParcelableExtra(Constants.CHARGE_CODE);
//                    returnOffenceCode(chargeInfoModel);
//                }

//            case Constants.PROCESS_ID_ISSUE_NOTICE: {
//                if (resultCode == Activity.RESULT_OK) {
//                    Utilities.wifiOn();
//                    mReconnect = true;
//                    mHandler.postDelayed(mRunnableDisconnect, 5000);
//                }
//            }
//        }
    }

    private void passageCounter(boolean resetCounter){

        if (resetCounter == true) {
            mPassageCounter = 0;
        }else{
            mPassageCounter++;
        }

        mPassageCounterTextView.setText(Integer.toString(mPassageCounter));
    }

    private void iCamAction(ICamEvent iCamEvent){

        try{
            //icam vosi action
            if (iCamEvent.getICamVlns() != null) {
                startVosiActionActivity(iCamEvent.getICamVlns());
            }

            //icam speed-infringement action
            if (iCamEvent.getICamInfringement() != null) {
                TicketModel ticket = TicketModel.getNewTicket(DocumentType.RoadSideDriver, mUser, mCourtInfo, mDistrict, true);

                if (ticket == null){
                    return;
                }

                ticket.getInfringement().setCameraID(iCamEvent.getICamInfringement().getHwid());
                ticket.getInfringement().setEventID(iCamEvent.getICamInfringement().getImageNumber());
                ticket.getInfringement().setSpeed(Integer.parseInt(iCamEvent.getICamInfringement().getSpeed()));

                ticket.getInfringement().setOffenceDate(Utilities.stringToDate(
                        String.format(
                                "%s %s",
                                iCamEvent.getICamInfringement().getDate(),
                                iCamEvent.getICamInfringement().getTime()),
                        DATETIME_FORMAT_EX));

                ticket.getInfringement().setLocationCode(iCamEvent.getICamInfringement().getLocation());

                if (ticket.getVehicle() == null) {
                    ticket.setVehicle(new VehicleModel());
                }
                ticket.getVehicle().setLicenceNumber(iCamEvent.getICamInfringement().getIcamVlns().getPlate());
                //TODO add speed charge detail if possible
                if (ticket == null) return;
                startWizardActivity(ticket);
            }

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "iCamAction"), ErrorSeverity.High);
        }
    }

    private void startWizardActivity(TicketModel ticket){
        Intent intent = new Intent(this, WizardActivity.class);
        intent.putExtra(Constants.TICKET_MODEL, ticket);
        intent.putExtra(Constants.TOGGLE_WIFI, true);
        startActivityForResult(intent, Constants.PROCESS_ID_ISSUE_NOTICE);
    }

    private void startVosiActionActivity(ICamVlns iCamVlns){
        Intent intent = new Intent(this, VosiActionActivity.class);
        intent.putExtra(Constants.ICAM_VLNS, iCamVlns);
        intent.putExtra(za.co.kapsch.shared.Constants.USER, mUser);
        intent.putExtra(za.co.kapsch.shared.Constants.DISTRICT, mDistrict);
        startActivityForResult(intent, Constants.PROCESS_ID_VOSI_ACTION);
    }

    private void startFineSearchInfoValidation(String vln){
        Intent intent = new Intent(this, FineSearchInfoValidationActivity.class);
        intent.putExtra(za.co.kapsch.shared.Constants.VLN, vln);
        startActivity(intent);
    }

    @Override
    public void onStop() {
        super.onStop();
    }

    @Override
    public void onBackPressed(){
        super.onBackPressed();
        ICamClient.getInstance(this, this).closeSocket(true);
    }

    @Override
    public void onSaveInstanceState(Bundle savedInstanceState) {
        savedInstanceState.putParcelableArrayList(Constants.ICAM_EVENT_LIST, mICamEventList);
        super.onSaveInstanceState(savedInstanceState);
    }

    @Override
    public void onRestoreInstanceState(Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);
        mICamEventList = savedInstanceState.getParcelable(Constants.ICAM_EVENT_LIST);
        populateListView(mICamEventList);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
        ICamClient.getInstance(this, this).closeSocket(true);
    }
}
