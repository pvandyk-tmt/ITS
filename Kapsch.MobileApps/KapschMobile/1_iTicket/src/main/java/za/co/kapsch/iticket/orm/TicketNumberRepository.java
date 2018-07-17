package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.stmt.QueryBuilder;
import com.j256.ormlite.stmt.Where;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.TicketStatus;
import za.co.kapsch.iticket.Enums.TicketType;
import za.co.kapsch.iticket.Models.TicketNumberModel;

/**
 * Created by csenekal on 2016-09-19.
 */
public class TicketNumberRepository {

    private static final String ID_COLUMN = "ID";
    private static final String DOCUMENT_TYPE_COLUMN = "DocumentType";
    private static final String STATUS_COLUMN = "Status";
    private static final String NUMBER_VALUE_COLUMN = "NumberValue";

    public static void insert(List<TicketNumberModel> ticketNumberList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<TicketNumberModel, Integer> ticketNumberDao = ormDbHelper.getTicketNumberDao();

            for (TicketNumberModel ticketNumber: ticketNumberList){
                ticketNumber.setStatus(TicketStatus.Available);
                ticketNumber.setDocumentType(DocumentType.RoadSideDriver);
                ticketNumberDao.create(ticketNumber);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static TicketNumberModel getNextTicketNumber(DocumentType documentType) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TicketNumberModel, Integer> ticketNumberDao = ormDbHelper.getTicketNumberDao();

            TicketNumberModel ticketNumber =  ticketNumberDao.queryForFirst(
                    ticketNumberDao.queryBuilder()
                            .orderBy(ID_COLUMN, true)
                            .where()
                            .eq(STATUS_COLUMN, TicketStatus.Available)
                            .and()
                            .eq(DOCUMENT_TYPE_COLUMN, documentType).prepare());

            if (ticketNumber == null){
                return null;
            }

            return ticketNumber;

        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean updateStatusToIssued(String numberValue) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TicketNumberModel, Integer> ticketNumberDao = ormDbHelper.getTicketNumberDao();

            TicketNumberModel ticketNumber =
                    ticketNumberDao.queryForFirst(
                            ticketNumberDao.queryBuilder()
                                    .where()
                                    .eq(NUMBER_VALUE_COLUMN, numberValue).prepare());

            ticketNumber.setStatus(TicketStatus.Issued);

            return (ticketNumberDao.update(ticketNumber) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static int getAvailableTicketCount(DocumentType documentType) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TicketNumberModel, Integer> ticketNumberDao = ormDbHelper.getTicketNumberDao();

            QueryBuilder<TicketNumberModel, Integer> queryBuilder = ticketNumberDao.queryBuilder();
            Where<TicketNumberModel, Integer> where = queryBuilder.where();

            return  (int)where
                        .eq(STATUS_COLUMN, TicketStatus.Available)
                        .and()
                        .eq(DOCUMENT_TYPE_COLUMN, documentType).countOf();

        } finally {
            ormDbHelper.close();
        }
    }

    public static List<TicketNumberModel> getTicketNumbers() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<TicketNumberModel, Integer> ticketNumberDao = ormDbHelper.getTicketNumberDao();

            return ticketNumberDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }
}
