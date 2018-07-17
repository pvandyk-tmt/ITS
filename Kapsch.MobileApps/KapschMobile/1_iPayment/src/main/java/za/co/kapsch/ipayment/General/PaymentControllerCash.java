package za.co.kapsch.ipayment.General;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

import java.util.Calendar;

import za.co.kapsch.ipayment.Enums.BroadcastSource;
import za.co.kapsch.ipayment.Enums.PaymentMethod;
import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;
import za.co.kapsch.ipayment.Enums.TerminalType;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2017/09/16.
 */
public class PaymentControllerCash implements IAsyncProcessCallBack {

    public static final int FAILED = 1;
    public static final int SUCCESS = 2;

    private Activity mActivity;
    private TransactionModel mTransaction;

    public PaymentControllerCash(TransactionModel transaction, Activity activity){
        mActivity = activity;
        mTransaction = transaction;
    }


//    private void run() {
//        mTransaction.setPaymentSource(PaymentMethod.Cash);
//        DataServiceRequest.transactionSettleRequest(this, mActivity, mTransaction);
//    }

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

                case Constants.PROCESS_ID_SETTLE_TRANSACTION:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            //printSlip();
                            return;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
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
