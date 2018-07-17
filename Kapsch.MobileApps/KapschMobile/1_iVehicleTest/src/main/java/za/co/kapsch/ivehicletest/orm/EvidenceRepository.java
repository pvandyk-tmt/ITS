package za.co.kapsch.ivehicletest.orm;

import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.ivehicletest.App;
import za.co.kapsch.ivehicletest.Enums.InspectionEvidenceType;
import za.co.kapsch.ivehicletest.Models.EvidenceModel;

/**
 * Created by csenekal on 2016-10-20.
 */
public class EvidenceRepository {

    public static final String UPLOADED_COLUMN = "Uploaded";
    public static final String BOOKING_ID_COLUMN = "BookingID";
    public static final String EVIDENCE_TYPE_COLUMN = "EvidenceType";
    public static final String SUBMIT_COLUMN = "Submit";

    public static void create(EvidenceModel evidence) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();
            evidenceDao.createOrUpdate(evidence);
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

    public static List<EvidenceModel> getEvidence(long bookingID, InspectionEvidenceType evidenceType) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            final List<EvidenceModel> evidenceList = evidenceDao.query(evidenceDao.queryBuilder()
                    .where()
                    .eq(EVIDENCE_TYPE_COLUMN, evidenceType)
                    .and()
                    .eq(BOOKING_ID_COLUMN, bookingID).prepare());

            return evidenceList;

        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean isEvidenceUploaded(String bookingID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<EvidenceModel, Integer> evidenceDao = ormDbHelper.getEvidenceDao();

            final List<EvidenceModel> evidenceList = evidenceDao.query(evidenceDao.queryBuilder()
                    .where()
                    .ne(EVIDENCE_TYPE_COLUMN, InspectionEvidenceType.VehiclePhoto)
                    .and()
                    .eq(BOOKING_ID_COLUMN, bookingID).prepare());

            return (evidenceList.size() == 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static void setEvidenceToSubmit(long bookingID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            SQLiteDatabase db = ormDbHelper.getWritableDatabase();
            String query = String.format("Update Evidence set Submit = 1 WHERE BookingID = %d", bookingID);
            db.execSQL(query);
        }finally {
            ormDbHelper.close();
        }
    }

    public static void deleteCancelledEvidence() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            SQLiteDatabase db = ormDbHelper.getWritableDatabase();
            String query = "Delete from Evidence WHERE Submit = 0";
            db.execSQL(query);
        }finally {
            ormDbHelper.close();
        }
    }
}
