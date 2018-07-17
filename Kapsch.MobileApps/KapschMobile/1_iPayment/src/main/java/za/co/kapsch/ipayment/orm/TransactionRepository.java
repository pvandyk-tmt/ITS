package za.co.kapsch.ipayment.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.Date;
import java.util.List;

import za.co.kapsch.ipayment.Enums.PaymentTransactionStatus;
import za.co.kapsch.ipayment.General.App;
import za.co.kapsch.ipayment.Models.TransactionItemModel;
import za.co.kapsch.ipayment.Models.TransactionModel;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/09/12.
 */
public class TransactionRepository {

    public static final String RECEIPT_TIME_STAMP_COLUMN = "ReceiptTimeStamp";
    public static final String RECEIPT_COLUMN = "Receipt";
    public static final String TRANSACTION_TOKEN_COLUMN = "TransactionToken";
    public static final String TRANSACTION_ID_COLUMN = "TransactionID";
    public static final String TRANSACTION_STATUS_COLUMN = "Status";

    public static void create(TransactionModel transaction) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
        SQLiteDatabase db = ormDbHelper.getWritableDatabase();
        final Dao<TransactionModel, Integer> transactionDao = ormDbHelper.getTransactionDao();
        final Dao<TransactionItemModel, Integer> transactionItemDao = ormDbHelper.getTransactionItemDao();

        try {

            db.beginTransaction();

            if (transactionDao.create(transaction) <= 0) {
                throw new SQLException("Failed to create receipt.");
            }

            TransactionModel localTransaction =  transactionDao.queryForFirst(
                    transactionDao.queryBuilder()
                            .where()
                            .eq(RECEIPT_COLUMN, transaction.getReceipt()).prepare());

            for (TransactionItemModel transactionItem : transaction.getTransactionItems()) {
                transactionItem.setTransactionID(localTransaction.getID());
                if (transactionItemDao.create(transactionItem) <= 0) {
                    throw new SQLException("Failed to create receipt item.");
                }
            }

            db.setTransactionSuccessful();

        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static int getTransactionCountPerDate(Date date) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TransactionModel, Integer> transactionDao = ormDbHelper.getTransactionDao();

            date.setHours(00);
            date.setMinutes(00);
            date.setSeconds(00);

            final List<TransactionModel> transactionList = transactionDao.query(transactionDao.queryBuilder()
                    .where().ge(RECEIPT_TIME_STAMP_COLUMN, date).prepare());

            return transactionList.size();

        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "TransactionRepository::getTransactionCountPerDate()"), ErrorSeverity.High);
            return -1;
        }finally{
            ormDbHelper.close();
        }
    }

    public static TransactionModel getTransaction(String transactionToken) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TransactionModel, Integer> transactionDao = ormDbHelper.getTransactionDao();
            final Dao<TransactionItemModel, Integer> transactionItemDao = ormDbHelper.getTransactionItemDao();

            TransactionModel transaction = transactionDao.queryForFirst(transactionDao.queryBuilder()
                    .where()
                    .eq(TRANSACTION_TOKEN_COLUMN, transactionToken).prepare());

            if (transaction != null) {
                List<TransactionItemModel> transactionItems = transactionItemDao.query(transactionItemDao.queryBuilder()
                        .where().eq(TRANSACTION_ID_COLUMN, transaction.getID()).prepare());

                transaction.setTransactionItems(transactionItems);
            }

            return transaction;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static TransactionModel getTransactionByReceipt(String receiptNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TransactionModel, Integer> transactionDao = ormDbHelper.getTransactionDao();
            final Dao<TransactionItemModel, Integer> transactionItemDao = ormDbHelper.getTransactionItemDao();

            TransactionModel transaction = transactionDao.queryForFirst(transactionDao.queryBuilder()
                    .where()
                    .eq(RECEIPT_COLUMN, receiptNumber).prepare());

            if (transaction != null) {
                List<TransactionItemModel> transactionItems = transactionItemDao.query(transactionItemDao.queryBuilder()
                        .where().eq(TRANSACTION_ID_COLUMN, transaction.getID()).prepare());

                transaction.setTransactionItems(transactionItems);
            }

            return transaction;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void updateTransaction(TransactionModel transaction) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
        SQLiteDatabase db = ormDbHelper.getWritableDatabase();
        final Dao<TransactionModel, Integer> transactionDao = ormDbHelper.getTransactionDao();
        final Dao<TransactionItemModel, Integer> transactionItemDao = ormDbHelper.getTransactionItemDao();

        try {

            db.beginTransaction();

            if (transactionDao.update(transaction) <= 0) {
                throw new SQLException("Failed to update receipt.");
            }

            for (TransactionItemModel transactionItem : transaction.getTransactionItems()) {
                if (transactionItemDao.update(transactionItem) <= 0) {
                    throw new SQLException("Failed to update receipt item.");
                }
            }

            db.setTransactionSuccessful();

        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<TransactionModel> getByDate(Date date) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TransactionModel, Integer> transactionDao = ormDbHelper.getTransactionDao();
            final Dao<TransactionItemModel, Integer> transactionItemDao = ormDbHelper.getTransactionItemDao();

            Date upperDate = Utilities.getCalendarDate(date).getTime();
            upperDate.setHours(23);
            upperDate.setMinutes(59);
            upperDate.setSeconds(59);

            date.setHours(0);
            date.setMinutes(0);
            date.setSeconds(0);

            final List<TransactionModel> transactionList = transactionDao.query(transactionDao.queryBuilder()
                    .where().between(RECEIPT_TIME_STAMP_COLUMN, date, upperDate).and()
                    .eq(TRANSACTION_STATUS_COLUMN, PaymentTransactionStatus.Settled).prepare());

            for (TransactionModel transaction : transactionList) {
                if (transaction != null) {
                    List<TransactionItemModel> transactionItems = transactionItemDao.query(transactionItemDao.queryBuilder()
                            .where()
                            .eq(TRANSACTION_ID_COLUMN, transaction.getID()).prepare());

                    transaction.setTransactionItems(transactionItems);
                }
            }

            return transactionList;
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getByDate"), ErrorSeverity.None);
            return null;
        }finally {
            ormDbHelper.close();
        }
    }
}
