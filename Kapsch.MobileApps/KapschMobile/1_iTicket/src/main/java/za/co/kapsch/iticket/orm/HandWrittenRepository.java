package za.co.kapsch.iticket.orm;

import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.GenericRawResults;
import com.j256.ormlite.stmt.QueryBuilder;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Enums.TicketStatus;
import za.co.kapsch.iticket.Models.HandWrittenModel;
import za.co.kapsch.iticket.Models.TicketNumberModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/08/02.
 */
public class HandWrittenRepository {

    public static final String ID_COLUMN = "ID";
    public static final String UPLOADED_COLUMN = "Uploaded";
    public static final String TICKET_NUMBER_COLUMN = "TicketNumber";
    public static final String ISSUE_DATE_COLUMN = "IssueDate";
    public static final String OFFENCE_DATE_COLUMN = "OffenceDate";
    public static final String CANCELLED_COLUMN = "IsCancelled";
    public static final String PRINTED_COLUMN = "Printed";
    public static final String COMPLETED_COLUMN = "Completed";

    private static final String NUMBER_VALUE_COLUMN = "NumberValue";

    public static int create(HandWrittenModel handWrittenModel) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
        final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            int rowsCreated = handWrittenDao.create(handWrittenModel);

            final Dao<TicketNumberModel, Integer> ticketNumberDao = ormDbHelper.getTicketNumberDao();

            TicketNumberModel ticketNumber =
                    ticketNumberDao.queryForFirst(
                            ticketNumberDao.queryBuilder()
                                    .where()
                                    .eq(NUMBER_VALUE_COLUMN, handWrittenModel.getTicketNumber()).prepare());

            //ticketNumber will be null if the ticket was not generated on the device i.e. iCam logged ticket
            if (ticketNumber != null) {
                ticketNumber.setStatus(TicketStatus.Issued);

                if (ticketNumberDao.update(ticketNumber) <= 0) {
                    throw new SQLException("Failed to set ticket number as used.");
                }
            }

            if (rowsCreated == 1) {
                QueryBuilder queryBuilder = handWrittenDao.queryBuilder();

                queryBuilder.selectRaw(ID_COLUMN).where().in(TICKET_NUMBER_COLUMN, handWrittenModel.getTicketNumber());

                GenericRawResults<String[]> results = handWrittenDao.queryRaw(queryBuilder.prepareStatementString());

                String[] values = results.getFirstResult();

                if (values.length == 0) return -1;

                db.setTransactionSuccessful();

                return Integer.parseInt(values[0]);
            }

        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }

        return -1;
    }

    public static boolean update(HandWrittenModel handWrittenModel) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            return (handWrittenDao.update(handWrittenModel) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean delete(HandWrittenModel handWrittenModel) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            return (handWrittenDao.delete(handWrittenModel) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            return handWrittenDao.queryForAll();

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getNonCancelled() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where()
                    .eq(CANCELLED_COLUMN, false).prepare());

            return handWrittenList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getAllNotCancelled() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            return handWrittenDao.queryForAll();

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getByDate(Date date) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            Date upperDate = Utilities.getCalendarDate(date).getTime();
            upperDate.setHours(23);
            upperDate.setMinutes(59);
            upperDate.setSeconds(59);

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where().between(ISSUE_DATE_COLUMN, date, upperDate).prepare());

            return handWrittenList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean offenceDateExists(Date date) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where().eq(OFFENCE_DATE_COLUMN, date).prepare());

            return handWrittenList.size() > 0;
        }finally {
            ormDbHelper.close();
        }
    }

    public static int offenceDateCount(Date date) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where().eq(OFFENCE_DATE_COLUMN, date).prepare());

            return handWrittenList.size();
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getUnSyncedTickets() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where()
                    .eq(CANCELLED_COLUMN, true).and().eq(UPLOADED_COLUMN, false)
                    .or()
                    .eq(PRINTED_COLUMN, true).and().eq(UPLOADED_COLUMN, false).prepare());

            return handWrittenList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getNotPrinted() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where()
                    .eq(PRINTED_COLUMN, false).and().eq(CANCELLED_COLUMN, false).prepare());

            return handWrittenList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<String> getNotPrintedTicketNumbers() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            SQLiteDatabase db = ormDbHelper.getWritableDatabase();
            String query = "SELECT TicketNumber FROM HandWritten WHERE Uploaded = 0";
            Cursor cursor = db.rawQuery(query, null);

            List<String> ticketNumberList = new ArrayList<>();

            cursor.moveToFirst();

            while ( !cursor.isAfterLast()) {
                ticketNumberList.add(cursor.getString(cursor.getColumnIndex("TicketNumber")));
                cursor.moveToNext();
            }

            return ticketNumberList;

        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean setUploadedStatus(String noticeNumber, boolean value) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            HandWrittenModel handWritten = getNotice(noticeNumber);

            if (handWritten == null) return false;

            handWritten.setUploaded(value);

            return update(handWritten);
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getUploadedIncomplete() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> HandWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = HandWrittenDao.query(HandWrittenDao.queryBuilder()
                    .where()
                    .eq(UPLOADED_COLUMN, true)
                    .and()
                    .eq(COMPLETED_COLUMN, false).prepare());

            return handWrittenList;
        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean isCompleted(String ticketNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final HandWrittenModel handWritten = handWrittenDao.queryForFirst(handWrittenDao.queryBuilder()
                    .where()
                    .eq(TICKET_NUMBER_COLUMN, ticketNumber)
                    .and()
                    .eq(COMPLETED_COLUMN, true).prepare());

            return (handWritten != null);
        }finally {
            ormDbHelper.close();
        }
    }

    public static HandWrittenModel getNotice(String ticketNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final HandWrittenModel handWritten = handWrittenDao.queryForFirst(handWrittenDao.queryBuilder()
                    .where()
                    .eq(TICKET_NUMBER_COLUMN, ticketNumber).prepare());

            return handWritten;
        }finally {
            ormDbHelper.close();
        }
    }

    public static boolean setComplete(HandWrittenModel handWritten) throws SQLException {

        if (handWritten.isUploaded() == false){
            throw new SQLException("Uploaded must TRUE before Completed can be set TRUE.");
        }

        return update(handWritten);
    }

    public static HandWrittenModel getByTicketNumber(String ticketNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where()
                    .eq(TICKET_NUMBER_COLUMN, ticketNumber).prepare());

            return handWrittenList.get(0);
        }finally {
            ormDbHelper.close();
        }
    }

    public static int getId(String ticketNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            QueryBuilder queryBuilder = handWrittenDao.queryBuilder();

            queryBuilder.selectRaw(ID_COLUMN).where().in(TICKET_NUMBER_COLUMN, ticketNumber);

            GenericRawResults<String[]> results = handWrittenDao.queryRaw(queryBuilder.prepareStatementString());

            String[] values = results.getFirstResult();

            if (values.length == 0) return -1;

            return Integer.parseInt(values[0]);

        } finally {
            ormDbHelper.close();
        }
    }

    public static List<HandWrittenModel> getTicketsByDateRange(Date startDate, Date endDate) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<HandWrittenModel, Integer> handWrittenDao = ormDbHelper.getHandWrittenDao();

            endDate.setHours(23);
            endDate.setMinutes(59);
            endDate.setSeconds(59);

            final List<HandWrittenModel> handWrittenList = handWrittenDao.query(handWrittenDao.queryBuilder()
                    .where()
                    .ge(OFFENCE_DATE_COLUMN, startDate)
                    .and()
                    .le(OFFENCE_DATE_COLUMN, endDate).prepare());

            return handWrittenList;

        }finally {

            ormDbHelper.close();
        }
    }
}
