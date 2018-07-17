package za.co.kapsch.ipayment.General;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.support.v4.content.LocalBroadcastManager;

import java.sql.SQLException;

import za.co.kapsch.ipayment.Enums.BroadcastSource;
import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;
import za.co.kapsch.ipayment.Interfaces.IProcessResultCallback;
import za.co.kapsch.ipayment.Models.TransactionItemModel;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.ipayment.Printer.Receipt;
import za.co.kapsch.ipayment.R;
import za.co.kapsch.ipayment.WebAccess.PaymentTransactionService;
import za.co.kapsch.ipayment.orm.TransactionRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/09/15.
 */
public class PaymentControllerDumaPay implements IAsyncProcessCallBack {

    private Intent mServiceIntent;
    public static final int FAILED = 1;
    public static final int SUCCESS = 2;

    static final String INTENT_EXTRA_SERVICE_ID = "19521";
    static final String INTENT_EXTRA_SERVICE_DESC = "Transportation";

    static final String INTENT_EXTRA_TOKEN = "intentToken"; //Transaction Token
    static final String INTENT_EXTRA_TOKEN_REF = "intentTokenRef";//Reference for the transaction token (above) -> received when creating the toke
    static final String INTENT_EXTRA_COMPANY_TOKEN = "IntentCompanyToken";  //Static compay token provided by DPO (Available from you Merchant Portal)
    static final String INTENT_EXTRA_AMOUNT = "IntentTokenAmount"; //amount of the desirec transaction
    static final String INTENT_EXTRA_AMOUNT_CURRENCY = "IntentTokenAmountCurr"; //Currency of the transaction
    static final String INTENT_EXTRA_COUNTRY = "intentCountryDefault";  //The Default country of the merchant (required for Mobile Money Payments)
    static final String INTENT_EXTRA_EXTERNAL_B_RECEIVER_INTENT_FILTER = "intentExternalFilter"; //The intent filter for the Broadcast Receiver that will wait for the response from the Dumapay App once a transaction is complete

    public static final String TRANSACTION_PAID = "000";
    public static final String AUTHORIZED = "001";
    public static final String OVERPAY_UNDERPAY = "002";
    public static final String REQUEST_MISSING_COMPANY_TOKEN = "801";
    public static final String COMPANY_TOKEN_DOES_NOT_EXIST = "802";
    public static final String NO_REQUEST_OR_ERROR_REQUEST_TYPE_NAME = "803";
    public static final String ERROR_IN_XML = "804";
    public static final String NOT_PAID_YET = "900";
    public static final String DECLINED = "901";
    public static final String TRANSACTION_PASSED_PTL = "903";
    public static final String TRANSACTION_CANDELLED = "904";
    public static final String REQUEST_MISSING_TRANSACTION_LEVEL_MADATORY_FIELDS = "950";
    public static final String DATA_MISMATCH_IN_ON_OF_THE_FIELDS = "951";

    private Activity mActivity;
    private TransactionModel mTransaction;

    private BroadcastReceiver mDumaPayReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {

            try {
                com.directpayonline.merchant.models.Receipt receipt = intent.getParcelableExtra("receipt");
                String resultCode = receipt.getResult();

                if (resultCode.equals(TRANSACTION_PAID) == false) {
                    MessageManager.showMessage(receipt.getResultExplanation(), ErrorSeverity.None);
                    deleteTransaction();
                    return;
                }

                //receipt.getResult();
                //TODO add payment reference number to Transaction and update transaction.
                mTransaction.setAmount(Double.parseDouble(receipt.getTransactionAmount()));
                printSlip(mTransaction);

            }catch (Exception e){
                MessageManager.showMessage(Utilities.exceptionMessage(e, "BroadcastReceiver::onReceive()"), ErrorSeverity.None);
            }
        }
    };

    private void deleteTransaction(){
        DataServiceRequest.transactionDeleteRequest(this, mActivity, mTransaction.getReceipt());
    }

    private void registerDumaPayReceiver() {
        IntentFilter intentFilter = new IntentFilter("za.co.kapsch.ipayment.DUMAPAY");
        mActivity.registerReceiver(mDumaPayReceiver, intentFilter);
    }

    public void startDumaPayApplication(TransactionModel transaction){

        Intent launchIntent = mActivity.getPackageManager().getLaunchIntentForPackage("com.directpayonline.merchant");

        if (launchIntent != null){
            launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            launchIntent.putExtra(INTENT_EXTRA_AMOUNT, String.valueOf(transaction.getAmount()));
            launchIntent.putExtra(INTENT_EXTRA_AMOUNT_CURRENCY,"ZMW");
            launchIntent.putExtra(INTENT_EXTRA_COMPANY_TOKEN,"D7E04568-FECD-4CDD-84BF-E5716FE77F62");

            launchIntent.putExtra(INTENT_EXTRA_TOKEN, transaction.getTransactionToken());//"BA71AED3-9202-4B75-A325-8273B86D44BB");
            launchIntent.putExtra(INTENT_EXTRA_TOKEN_REF, "840BA71A");
            launchIntent.putExtra(INTENT_EXTRA_COUNTRY,"ZAM");

            launchIntent.putExtra(INTENT_EXTRA_EXTERNAL_B_RECEIVER_INTENT_FILTER, "za.co.kapsch.ipayment.DUMAPAY");

            mActivity.startActivity(launchIntent);//null pointer check in case package name was not found
        }
    }

//    public void startDumaPayApplication(){
//
//        Intent launchIntent = getPackageManager().getLaunchIntentForPackage("com.directpayonline.merchant");
//        launchIntent.putExtra(INTENT_EXTRA_TOKEN, "BA71AED3-9202-4B75-A325-8273B86D44BB");
//        launchIntent.putExtra(INTENT_EXTRA_TOKEN_REF,"840BA71A");
//        launchIntent.putExtra(INTENT_EXTRA_COMPANY_TOKEN,"D7E04568-FECD-4CDD-84BF-E5716FE77F62");
//        launchIntent.putExtra(INTENT_EXTRA_AMOUNT,"4");
//        launchIntent.putExtra(INTENT_EXTRA_AMOUNT_CURRENCY,"ZMW");
//        launchIntent.putExtra(INTENT_EXTRA_COUNTRY,"ZAM");
//        launchIntent.putExtra(INTENT_EXTRA_EXTERNAL_B_RECEIVER_INTENT_FILTER, "za.co.kapsch.ipayment.DUMAPAY");//"za.co.kapsch.payment.DUMAPAY");
//
//        if (launchIntent != null) {
//            startActivity(launchIntent);//null pointer check in case package name was not found
//        }
//    }

    private BroadcastReceiver mPaymentConfirmationReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {

            switch (intent.getAction()) {
                case Constants.TRANSACTION_CONFIRMATION_ACTION:

                    TransactionModel transaction = null;
                    BroadcastSource broadcastSource = null;
                    PaymentTransactionStatus transactionStatus = null;

                    try {
                        double amount = intent.getDoubleExtra(Constants.TRANSACTION_AMOUNT, 0);
                        String transactionToken = intent.getStringExtra(Constants.TRANSACTION_TOKEN);
                        transactionStatus = PaymentTransactionStatus.fromCode(intent.getIntExtra(Constants.TRANSACTION_STATUS, 0));
                        broadcastSource = BroadcastSource.fromCode(intent.getIntExtra(Constants.BROADCAST_SOURCE, 0));
                        transaction = updateTransaction(broadcastSource, transactionToken, transactionStatus, amount);

                        if (transaction == null){
                            return;
                        }

                    } catch (Exception e) {
                        MessageManager.showMessage(Utilities.exceptionMessage(e, "mPaymentConfirmationReceiver"), ErrorSeverity.High);
                    } finally {
                        stopService();
                        Utilities.busyProgressBarEx(mActivity, false);
                    }

                    if (transactionStatus != null) {
                        if (transactionStatus == PaymentTransactionStatus.Settled) {
                            printSlip(transaction);
                            return;
                        }
                    }

                    if (broadcastSource == BroadcastSource.Timeout) {
                        queryTransaction(mTransaction.getReceipt());
                    }

                    break;
                default:
                    break;
            }
        }
    };

    public PaymentControllerDumaPay(TransactionModel transaction, Activity activity){
        mActivity = activity;
        mTransaction = transaction;
        registerDumaPayReceiver();
    }

    public void run(){
        registerTransaction();
        DataServiceRequest.transactionRegisterRequest(this, mActivity, mTransaction);
    }

    private void registerTransaction() {

        try {
            TransactionRepository.create(mTransaction);
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PaymentControllerDumaPay::registerTransaction()"), ErrorSeverity.High);
        }
    }

    private TransactionModel updateTransaction(TransactionModel transaction) {

        try {
            TransactionModel localTransaction = TransactionRepository.getTransactionByReceipt(transaction.getReceipt());

            if (localTransaction != null) {
                transaction.setID(localTransaction.getID());

                for(TransactionItemModel transactionItem : transaction.getTransactionItems()) {
                    for (TransactionItemModel localTransactionItem : localTransaction.getTransactionItems()) {
                        if (transactionItem.getTransactionToken().equals(localTransactionItem.getTransactionToken())){
                            transactionItem.setID(localTransactionItem.getID());
                        }
                    }
                }

                TransactionRepository.updateTransaction(transaction);

                return transaction;
            } else {
                MessageManager.showMessage("updateTransaction: Transaction does not exit", ErrorSeverity.None);
            }
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PaymentControllerDumaPay::updateTransaction"), ErrorSeverity.High);
        }

        return null;
    }

    private TransactionModel updateTransaction(BroadcastSource broadcastSource, String transactionToken, PaymentTransactionStatus transactionStatus, double amount) {
        try {
            TransactionModel transaction = TransactionRepository.getTransaction(transactionToken);

            transaction.setConfirmationSource(broadcastSource);
            transaction.setStatus(transactionStatus);
            transaction.setConfirmedTransactionToken(transactionToken);
            transaction.setConfirmedAmount(amount);

            TransactionRepository.updateTransaction(transaction);

            return transaction;
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "PaymentControllerDumaPay::updateTransaction"), ErrorSeverity.High);
        }

        return null;
    }

    private void startPaymentTransactionService(String transactionToken) {

        IntentFilter intentFilter = new IntentFilter(Constants.TRANSACTION_CONFIRMATION_ACTION);
        LocalBroadcastManager.getInstance(App.getContext()).registerReceiver(mPaymentConfirmationReceiver, intentFilter);

        mServiceIntent = new Intent(mActivity, PaymentTransactionService.class);
        mServiceIntent.putExtra("TransactionToken", transactionToken);
        mActivity.startService(mServiceIntent);

        Utilities.busyProgressBarEx(mActivity, true);
    }

    private void queryTransaction(String receiptOrTransactionTokenNumber) {
        DataServiceRequest.queryTransactionRequest(this, mActivity, receiptOrTransactionTokenNumber);
    }

    public void stopService() {

        if (mServiceIntent != null) {
            mActivity.stopService(mServiceIntent);
            mServiceIntent = null;
        }

        if (mPaymentConfirmationReceiver != null) {
            LocalBroadcastManager.getInstance(mActivity).unregisterReceiver(mPaymentConfirmationReceiver);
            mPaymentConfirmationReceiver = null;
        }
    }

    private void printSlip(TransactionModel mTransaction){

        String[] details = Utilities.getPrinterDetails();
        if (details.length != 2){

            Utilities.displayOkMessage("Printer details not found, Please register a printer.", mActivity);
            return;
        }

        Receipt receipt = new Receipt(details[1], mActivity, this);
        MessageManager.showMessage(mActivity.getResources().getString(R.string.printing_receipt), ErrorSeverity.None);
        receipt.print(false, mTransaction);
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

                case Constants.PROCESS_ID_REGISTER_TRANSACTION:

                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mTransaction.setTransactionToken((String) asyncResultModel.getObject());
                            mTransaction = updateTransaction(mTransaction);
                            startDumaPayApplication(mTransaction);
                            //startPaymentTransactionService(mTransaction.getTransactionToken());
                            return;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_QUERY_TRANSACTION:

                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            mTransaction = (TransactionModel) asyncResultModel.getObject();
                            if (mTransaction.getStatus() == PaymentTransactionStatus.Settled || mTransaction.getStatus() == PaymentTransactionStatus.Processed) {
                                mTransaction.setConfirmationSource(BroadcastSource.Query);
                                TransactionModel transaction = updateTransaction(mTransaction);
                                printSlip(transaction);
                            }else{
                                MessageManager.showMessage("Payment failed", ErrorSeverity.High);
                                //TODO confirm cancellation of transaction
                                //DataServiceRequest.transactionDeleteRequest(this, mActivity, mTransaction);
                            }
                            return;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);

                            //TODO confirm cancellation of transaction
                            DataServiceRequest.transactionDeleteRequest(this, mActivity, mTransaction.getReceipt());
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DELETE_TRANSACTION:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            MessageManager.showMessage("Transaction Cancelled", ErrorSeverity.None);
                            return;
                        case FAILED:
                            MessageManager.showMessage("Cancel Transaction Failed", ErrorSeverity.High);
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_ASYNC_PROCESS_PRINT:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            MessageManager.showMessage("Payment completed sussessfully", ErrorSeverity.None);
                            ((IProcessResultCallback)mActivity).callBack(true);
                            return;
                        case FAILED:
                            MessageManager.showMessage("Failed to print slip", ErrorSeverity.High);
                            break;
                    }
                    break;
            }

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "finishedCallBack() - PROCESS_ID: %d"), ErrorSeverity.High);
            return;
        }
    }

}
