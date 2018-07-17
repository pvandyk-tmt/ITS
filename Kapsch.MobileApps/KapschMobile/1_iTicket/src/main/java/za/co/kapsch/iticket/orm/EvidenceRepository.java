package za.co.kapsch.iticket.orm;

import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Models.EvidenceModel;

/**
 * Created by csenekal on 2016-10-20.
 */
public class EvidenceRepository {

    public static final String UPLOADED_COLUMN = "Uploaded";
    public static final String TICKET_NO_COLUMN = "TicketNumber";
    public static final String EVIDENCE_TYPE_COLUMN = "EvidenceType";

    public static void create(EvidenceModel evidence) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();
            evidenceDao.create(evidence);
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean update(EvidenceModel evidence) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            return (evidenceDao.update(evidence) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean delete(EvidenceModel evidence) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            return (evidenceDao.delete(evidence) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<EvidenceModel> getUnSyncedEvidence() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

        final List<EvidenceModel> evidenceList = evidenceDao.query(evidenceDao.queryBuilder()
                .where()
                .eq(UPLOADED_COLUMN, false).prepare());

        return evidenceList;
        } finally {
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

    public static List<EvidenceModel> getUnSyncedEvidenceEx() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            List<String> ticketNumberList = getNotPrintedTicketNumbers();

            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            final List<EvidenceModel> evidenceList = evidenceDao.query(evidenceDao.queryBuilder()
                    .where()
                    .notIn(TICKET_NO_COLUMN, ticketNumberList)
                    .and()
                    .eq(UPLOADED_COLUMN, false).prepare());

            return evidenceList;
        } finally {
            ormDbHelper.close();
        }
    }

    public static List<EvidenceModel> getEvidenceByTicketNumber(String ticketNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            final List<EvidenceModel> evidenceList = evidenceDao.query(evidenceDao.queryBuilder()
                    .where()
                    .eq(TICKET_NO_COLUMN, ticketNumber).prepare());

            return evidenceList;
        } finally {
            ormDbHelper.close();
        }
    }

    public static EvidenceModel getEvidence(String ticketNumber, EvidenceType evidenceType) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            final EvidenceModel evidenceModel = evidenceDao.queryForFirst(evidenceDao.queryBuilder()
                    .where()
                    .eq(EVIDENCE_TYPE_COLUMN, evidenceType)
                    .and()
                    .eq(TICKET_NO_COLUMN, ticketNumber).prepare());

            return evidenceModel;

        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean isEvidenceUploaded(String ticketNumber) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            final List<EvidenceModel> evidenceList = evidenceDao.query(evidenceDao.queryBuilder()
                    .where()
                    .ne(EVIDENCE_TYPE_COLUMN, EvidenceType.OfficerSignature)
                    .and()
                    .ne(EVIDENCE_TYPE_COLUMN, EvidenceType.OffenderPhoto)
                    .and()
                    .ne(EVIDENCE_TYPE_COLUMN, EvidenceType.PersonSignature)
                    .and()
                    .eq(TICKET_NO_COLUMN, ticketNumber).prepare());
                    //.eq(UPLOADED_COLUMN, true).prepare());

            return (evidenceList.size() == 0);
        } finally {
            ormDbHelper.close();
        }
    }

}
