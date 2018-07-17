package za.co.kapsch.console;

import android.os.Bundle;
import android.os.Message;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.RelativeLayout;

import com.zebra.sdk.printer.discovery.DiscoveredPrinter;
import com.zebra.sdk.printer.discovery.DiscoveryHandler;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.PrinterModel;
import za.co.kapsch.shared.Printer.ZebraBlueToothDiscovery;
import za.co.kapsch.shared.Utilities;

public class PrinterListActivity extends AppCompatActivity implements DiscoveryHandler {

    private RelativeLayout mRelativeLayout;
    private static String FRIENDLY_NAME = "FRIENDLY_NAME";
    private static String MAC_ADDRESS = "MAC_ADDRESS";
    private List<PrinterModel> mPrinterList = new ArrayList<PrinterModel>();

    private PrinterModel mSelectedListItem;
    private Button mSaveButton;
    private ListView mListView;
    private EditText mFriendlyName;
    private EditText mMacAddress;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_printer_list);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.address)));

        mRelativeLayout = (RelativeLayout) findViewById(R.id.mainLayout);
        mSaveButton = (Button) findViewById(R.id.saveButton);
        mListView = (ListView) findViewById(R.id.printersListView);
        mFriendlyName = (EditText) findViewById(R.id.currentPrinterFriedlyNameEditText);
        mMacAddress = (EditText) findViewById(R.id.currentPrinterMacAddressEditText);

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                view.setSelected(true);
                mSelectedListItem = (PrinterModel) mListView.getItemAtPosition(position);

                if (mSelectedListItem == null) {
                    return;
                }

                mFriendlyName.setText(mSelectedListItem.getFriendlyName());
                mMacAddress.setText(mSelectedListItem.getMacAddress());
            }
        });

        mListView.setOnTouchListener(new View.OnTouchListener() {

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
                            mListView.setSelector(android.R.color.transparent);
                            mSelectedListItem = null;
                            mFriendlyName.setText(Constants.EMPTY_STRING);
                            mMacAddress.setText(Constants.EMPTY_STRING);
                        }
                        break;
                    default:
                        break;
                }
                return false;
            }
        });

        mMacAddress.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {

            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {

            }

            @Override
            public void afterTextChanged(Editable s) {
                validateUserInterface();
            }
        });

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        LayoutInflater inflater = getLayoutInflater();
        ViewGroup header = (ViewGroup)inflater.inflate(R.layout.printer_list_header, mListView, false);
        mListView.addHeaderView(header);

        mSaveButton.setEnabled(false);
        RetrieveCurrentPrinter();
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


    private void PopulatePrinterListView(List<PrinterModel> printerList) {
        if (printerList.size() < 1) return;
        ArrayAdapter<PrinterModel> arrayAdapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, printerList);
        mListView.setAdapter(arrayAdapter);
    }

    private void RetrieveCurrentPrinter() {
        String[] details = Utilities.getPrinterDetails();

        if (details.length == 2) {
            mFriendlyName.setText(details[0]);
            mMacAddress.setText(details[1]);
        }
    }

    private void validateUserInterface(){
        mSaveButton.setEnabled(
                (mMacAddress.getText().toString().isEmpty() == false) &&
                (mFriendlyName.getText().toString().isEmpty() == false));
    }

    public void searchPrinters(View view) {
        try {
            mPrinterList.clear();
            //mListView.setAdapter(null);
            mSaveButton.setEnabled(false);

            Utilities.busyProgressBarEx(this, true);
            new ZebraBlueToothDiscovery(this);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    public void savePrinter(View view) {

        if ((mFriendlyName.getText().toString().isEmpty()) || (mMacAddress.getText().toString().isEmpty())){
            return;
        }

        if (Utilities.SavePrinterDetails(mFriendlyName.getText().toString(), mMacAddress.getText().toString()) == false) {
            MessageManager.showMessage(getResources().getString(R.string.save_failed), ErrorSeverity.None);
        }else{
            MessageManager.showMessage(getResources().getString(R.string.printer_saved), ErrorSeverity.None);
        }
    }

    @Override
    public void foundPrinter(DiscoveredPrinter discoveredPrinter) {
        MessageManager.showMessage(discoveredPrinter.address, ErrorSeverity.None);
        Map<String, String> printer = discoveredPrinter.getDiscoveryDataMap();
        mPrinterList.add(new PrinterModel(printer.get(FRIENDLY_NAME), printer.get(MAC_ADDRESS)));
    }

    @Override
    public void discoveryFinished() {
        Utilities.busyProgressBarEx(this, false);
        MessageManager.showMessage("discoveryFinished", ErrorSeverity.None);
        if (mPrinterList.size() > 0) {
            PopulatePrinterListView(mPrinterList);
        }
    }

    @Override
    public void discoveryError(String error) {
        try {
            MessageManager.showMessage(error, ErrorSeverity.High);
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "discoveryError"), ErrorSeverity.High);
        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}