package za.co.kapsch.ipayment;

import android.content.Intent;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.SparseBooleanArray;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;

import com.google.android.gms.appindexing.AppIndex;
import com.google.android.gms.common.api.GoogleApiClient;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import za.co.kapsch.ipayment.Enums.PaymentMethod;
import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;
import za.co.kapsch.ipayment.Enums.TerminalType;
import za.co.kapsch.ipayment.General.Constants;
import za.co.kapsch.ipayment.General.PaymentControllerDumaPay;
import za.co.kapsch.ipayment.Interfaces.IProcessResultCallback;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.ipayment.Models.TransactionItemModel;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.ipayment.orm.TransactionRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Enums.SearchFinesCriteriaType;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;

public class FineResultsActivity extends AppCompatActivity implements IProcessResultCallback {

    private LinearLayout mParentLinearLayout;
    private LinearLayout mIdNumberLinearLayout;
    private LinearLayout mVehicleLinearLayout;
    private ListView mIdNumberListView;
    private ListView mVehicleListView;
    private List<FineModel> mIdNumberFineList;
    private List<FineModel> mVehicleFineList;
    private TextView mTotalAmountTextView;
    private double mTotalAmount;
    //private CheckBox mCashPaymentCheckBox;
    private Button mProceedButton;
    private TextView mHeaderTextView;

    public void callBack(boolean processSuccessfull){

        if (processSuccessfull == true){
            finish();
        }
    }

    private AdapterView.OnItemClickListener mOnItemClickListener = new AdapterView.OnItemClickListener() {

        @Override
        public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {

            try {
                view.setSelected(true);

                ListView listView = (ListView)parent;
                SparseBooleanArray checkedItems = listView.getCheckedItemPositions();
                setCheckBoxes(listView, checkedItems);
                calculateTotalAmountEx();

                //TODO check all lists
                validateInterface();

            } catch (Exception e) {
                MessageManager.showMessage(Utilities.exceptionMessage(e, "FineResultsActivity::onItemClick()"), ErrorSeverity.High);
            }
        }
    };

    private AdapterView.OnItemLongClickListener mOnItemLongClickListener = new AdapterView.OnItemLongClickListener() {
        @Override
        public boolean onItemLongClick(AdapterView parent, View view, int position, long id) {
            //do your stuff here
            LinearLayout detailLinearLayout = (LinearLayout) ((ViewGroup) view).findViewById(R.id.detailLinearLayout);

            if (detailLinearLayout.getHeight() == 0) {
                Utilities.showLinearLayout(detailLinearLayout);
            } else {
                Utilities.hideLinearLayout(detailLinearLayout);
            }

            return true;
        }
    };
    /**
     * ATTENTION: This was auto-generated to implement the App Indexing API.
     * See https://g.co/AppIndexing/AndroidStudio for more information.
     */
    private GoogleApiClient client;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_fine_results);

        try {

            mIdNumberFineList = getIntent().getParcelableArrayListExtra(Constants.ID_NUMBER_FINE_LIST);
            mVehicleFineList  = getIntent().getParcelableArrayListExtra(Constants.VEHICLE_FINE_LIST);

            mIdNumberListView = (ListView) findViewById(R.id.idNumberListView);
            mVehicleListView = (ListView) findViewById(R.id.vehicleListView);

            mIdNumberListView.setOnItemClickListener(mOnItemClickListener);
            mVehicleListView.setOnItemClickListener(mOnItemClickListener);

            //mCashPaymentCheckBox = (CheckBox) findViewById(R.id.cashPaymentCheckBox);

            mParentLinearLayout = (LinearLayout) findViewById(R.id.parentLinearLayout);
            mIdNumberLinearLayout = (LinearLayout) findViewById(R.id.idNumberLinearLayout);
            mVehicleLinearLayout = (LinearLayout) findViewById(R.id.vehicleLinearLayout);

            mProceedButton = (Button) findViewById(R.id.proceedButton);

            mTotalAmountTextView = (TextView) findViewById(R.id.totalAmountTextView);

            setListViewHeader(mIdNumberListView, R.layout.search_fine_list_header, getPersonNameHeaderText(mIdNumberFineList));
            setListViewHeader(mVehicleListView, R.layout.search_fine_list_header, getVLNHeaderText(mVehicleFineList));

            populateListViews(mIdNumberFineList, mVehicleFineList);

            ActionBar actionBar = getSupportActionBar();
            actionBar.setDisplayHomeAsUpEnabled(true);

            setTitle(String.format("%1$s - %2$s",
                    getResources().getString(R.string.app_name),
                    getResources().getString(R.string.fine_basket)));

            validateInterface();

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "FineResultsActivity::onCreate()"), ErrorSeverity.High);
        }
        // ATTENTION: This was auto-generated to implement the App Indexing API.
        // See https://g.co/AppIndexing/AndroidStudio for more information.
        client = new GoogleApiClient.Builder(this).addApi(AppIndex.API).build();
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

    private void validateInterface(){

        if (isListNullOrEmpty(mIdNumberFineList) == true) {
            mParentLinearLayout.removeView(mIdNumberLinearLayout);
        }

        if (isListNullOrEmpty(mVehicleFineList) == true) {
            mParentLinearLayout.removeView(mVehicleLinearLayout);
        }

        boolean itemSelected;

        itemSelected = (validateListSelection(mIdNumberListView.getCheckedItemPositions()) ||
                        validateListSelection(mVehicleListView.getCheckedItemPositions()));

        mProceedButton.setEnabled(itemSelected == true);
    }

    private String getPersonNameHeaderText(List<FineModel> idNumberFineList){

        if (idNumberFineList == null) return null;

        if (idNumberFineList.size() == 0) return null;

        return String.format("%s %s %s",
                getResources().getString(R.string.person),
                idNumberFineList.get(0).getOffenderFirstName(),
                idNumberFineList.get(0).getOffenderLastName());
    }

    private String getVLNHeaderText(List<FineModel> vehicleFineListFineList){

        if (vehicleFineListFineList == null) return null;

        if (vehicleFineListFineList.size() == 0) return null;

        return String.format("%s %s",
                getResources().getString(R.string.vehicle),
                vehicleFineListFineList.get(0).getVLN());
    }

    private boolean validateListSelection(SparseBooleanArray checkedItems) {

        boolean itemSelected = false;

        for (int i = 0; i < checkedItems.size(); i++) {
            int checkedPosition = checkedItems.keyAt(i);
            itemSelected = checkedItems.get(checkedPosition);
            if (itemSelected == true) {
                break;
            }
        }

        return itemSelected;
    }

    private void setListViewHeader(ListView listView, int headerTextId,  String value){

        LayoutInflater inflater = getLayoutInflater();
        ViewGroup header = (ViewGroup) inflater.inflate(headerTextId, listView, false);

        mHeaderTextView = (TextView) header.findViewById(R.id.headerTextView);
        mHeaderTextView.setText(value);

        listView.addHeaderView(header);
        listView.setOnItemClickListener(mOnItemClickListener);
        listView.setOnItemLongClickListener(mOnItemLongClickListener);
    }

    private CheckBox getCheckBox(ListView listView, int index) {

        View view = listView.getChildAt(index);

        if (view == null) return null;

        LinearLayout itemLinearLayout = (LinearLayout) view.findViewById(R.id.itemLinearLayout);

        if (itemLinearLayout == null) return null;

        LinearLayout rowLinearLayout = (LinearLayout) itemLinearLayout.findViewById(R.id.rowLinearLayout);

        if (rowLinearLayout == null) return null;

        return (CheckBox) rowLinearLayout.findViewById(R.id.checkBox);
    }

    private void setCheckBoxes(ListView listView, SparseBooleanArray checkedItems) {

        int size = listView.getCount();

        for (int position = 1; position <= size; position++) {
            CheckBox checkBox = getCheckBox(listView, position - 1);
            if (checkBox != null) {
                boolean checked = isCheckBoxChecked(listView, position - 1, checkedItems);
                checkBox.setChecked(checked);
            }
        }
    }

    private boolean isCheckBoxChecked(ListView listView, int position, SparseBooleanArray checkedItems) {

        int size = checkedItems.size();
        FineModel fine = (FineModel) listView.getItemAtPosition(position);

        for (int i = 0; i < size; i++) {
            int checkedPosition = checkedItems.keyAt(i);
            boolean isChecked = checkedItems.get(checkedPosition);

            if (isChecked == true) {
                if (position == checkedPosition) {
                    fine.setChecked(true);
                    return true;
                }
            }
        }
        fine.setChecked(false);
        return false;
    }

    private void populateListViews(List<FineModel> idNumberfineList, List<FineModel> vehiclefineList) {

        mTotalAmount = 0;

        if (isListNullOrEmpty(idNumberfineList) == false) {
            for(FineModel fine : idNumberfineList) {
                fine.setSearchFinesCriteriaType(SearchFinesCriteriaType.ID);
            }
            FineResultsListAdapter adapter = new FineResultsListAdapter(this, idNumberfineList);
            mIdNumberListView.setAdapter(adapter);
        }

        if (isListNullOrEmpty(vehiclefineList) == false) {
            for(FineModel fine : vehiclefineList) {
                fine.setSearchFinesCriteriaType(SearchFinesCriteriaType.VLN);
            }
            FineResultsListAdapter adapter = new FineResultsListAdapter(this, vehiclefineList);
            mVehicleListView.setAdapter(adapter);
        }
    }

    public void proceedClick(View view) {

        //PaymentMethod paymentMethod = mCashPaymentCheckBox.isChecked() ? PaymentMethod.Cash : PaymentMethod.DumaPay;
        PaymentMethod paymentMethod = PaymentMethod.DumaPay;

        switch (paymentMethod) {
            case DumaPay:
                Utilities.logUserActivity("Payment","Initiated-DumaPay");
                new PaymentControllerDumaPay(getTransaction(PaymentMethod.DumaPay), this).run();
                break;
            case Cash:
                Utilities.logUserActivity("Payment","Initiated-Cash");
                new PaymentControllerDumaPay(getTransaction(PaymentMethod.Cash), this).run();
                break;
        }
    }

    private TransactionModel getTransaction(PaymentMethod paymentMethod) {

        try {
            TransactionModel transaction = new TransactionModel();
            mTotalAmount = Double.parseDouble(mTotalAmountTextView.getText().toString());
            transaction.setAmount(mTotalAmount);
            transaction.setTerminalType(TerminalType.InternalMobileDevice);
            transaction.setTerminalUUID(Utilities.getDeviceId());

            //TODO select payment method from list
            transaction.setPaymentSource(paymentMethod);
            transaction.setReceiptTimeStamp(Calendar.getInstance().getTime());
            transaction.setReceipt(receiptNumber());
            transaction.setUserID(SessionModel.getInstance().getUserId());

            transaction.setUserName(
                    String.format("%s %s(%s)",
                            SessionModel.getInstance().getUser().getFirstName(),
                            SessionModel.getInstance().getUser().getLastName(),
                            SessionModel.getInstance().getUser().getInfrastructureNumber()));

            transaction.setTransactionItems(getSelectedTransactionItems());
            return transaction;
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "FineResultsActivity::getTransactionModel()"), ErrorSeverity.High);
            return null;
        }
    }

    private List<TransactionItemModel> getSelectedTransactionItems() {

        List<TransactionItemModel> transactionItemList = new ArrayList<>();

        if (isListNullOrEmpty(mIdNumberFineList) == false) {
            for (FineModel fine : mIdNumberFineList) {
                if (fine.isChecked() == true) {
                    transactionItemList.add(getTransactionItem(fine));
                }
            }
        }

        if (isListNullOrEmpty(mVehicleFineList) == false) {
            for (FineModel fine : mVehicleFineList) {
                if (fine.isChecked() == true) {
                    transactionItemList.add(getTransactionItem(fine));
                }
            }
        }

        return transactionItemList;
    }

    private TransactionItemModel getTransactionItem(FineModel fine) {

        TransactionItemModel transactionItem = new TransactionItemModel();
        transactionItem.setTransactionToken(fine.getTransactionToken());
        transactionItem.setReferenceNumber(fine.getReferenceNumber());
        transactionItem.setAmount(fine.getOutstandingAmount());
        transactionItem.setReferenceNumber(fine.getReferenceNumber());
        transactionItem.setStatus(PaymentTransactionStatus.Added);

        return transactionItem;
    }

    private String receiptNumber() {

        long deviceInternalID = SessionModel.getInstance().getInternalDeviceID();
        int year = Utilities.getYearInCentury();
        int dayOfYear = Utilities.getDayOfYear();
        int sequence = 0;

        try {
            sequence = TransactionRepository.getTransactionCountPerDate(Calendar.getInstance().getTime()) + 1;
        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "FineResultsActivity::receiptNumber()"), ErrorSeverity.High);
        }

        return String.format("%05d-%02d-%03d-%05d", deviceInternalID, year, dayOfYear, sequence);
    }

    private void calculateTotalAmountEx() {

        try {

            double totalAmount = 0;

            if (isListNullOrEmpty(mIdNumberFineList) == false) {
                for (FineModel fine : mIdNumberFineList) {
                    if (fine.isChecked() == true) {
                        totalAmount = totalAmount + fine.getOutstandingAmount();
                    }
                }
            }

            if (isListNullOrEmpty(mVehicleFineList) == false) {
                for (FineModel fine : mVehicleFineList) {
                    if (fine.isChecked() == true) {
                        totalAmount = totalAmount + fine.getOutstandingAmount();
                    }
                }
            }

            mTotalAmountTextView.setText(Double.toString(totalAmount));

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.High);
        }
    }

    private boolean isListNullOrEmpty(List<FineModel> list){

        if(list == null) return true;

        if (list.size() == 0) return true;

        return false;
    }

    @Override
    public void onDestroy() {

        super.onDestroy();
    }
}
