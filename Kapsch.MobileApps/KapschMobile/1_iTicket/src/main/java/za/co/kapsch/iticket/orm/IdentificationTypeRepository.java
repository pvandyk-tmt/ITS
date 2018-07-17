package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.CourtModel;
import za.co.kapsch.iticket.Models.IdentificationTypeModel;

/**
 * Created by CSenekal on 2018/03/09.
 */

public class IdentificationTypeRepository {

    public static final String ID_COLUMN = "ID";
    public static final String DESCRIPTION_COLUMN = "Description";

    public static void cleanInsert(List<IdentificationTypeModel> identificationTypeList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<IdentificationTypeModel, Integer> identificationTypeDao = ormDbHelper.getIdentificationTypeDao();

            OrmDbHelper.deleteAll(identificationTypeDao);

            for (IdentificationTypeModel identificationType: identificationTypeList){
                identificationTypeDao.create(identificationType);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<IdentificationTypeModel> getIdentificationTypeList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<IdentificationTypeModel, Integer> identificationTypeDao = ormDbHelper.getIdentificationTypeDao();

            return identificationTypeDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }

    public static IdentificationTypeModel getIdentificationType(long ID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<IdentificationTypeModel, Integer> identificationTypeDao = ormDbHelper.getIdentificationTypeDao();

            final IdentificationTypeModel identificationType = identificationTypeDao.queryForFirst(identificationTypeDao.queryBuilder()
                    .where()
                    .eq(ID_COLUMN, ID).prepare());

            return identificationType;

        } finally {
            ormDbHelper.close();
        }
    }

    public static IdentificationTypeModel getIdentificationType(String description) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<IdentificationTypeModel, Integer> identificationTypeDao = ormDbHelper.getIdentificationTypeDao();

            final IdentificationTypeModel identificationType = identificationTypeDao.queryForFirst(identificationTypeDao.queryBuilder()
                    .where()
                    .eq(DESCRIPTION_COLUMN, description).prepare());

            return identificationType;

        } finally {
            ormDbHelper.close();
        }
    }
}
