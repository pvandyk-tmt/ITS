package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Models.CountryModel;

/**
 * Created by CSenekal on 2018/04/20.
 */

public class CountryRepository {

    public static final String ID_COLUMN = "ID";

    public static void cleanInsert(List<CountryModel> countryList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<CountryModel, Integer> countryDao = ormDbHelper.getCountryDao();

            OrmDbHelper.deleteAll(countryDao);

            for (CountryModel country: countryList){
                countryDao.create(country);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<CountryModel> getCountryList() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CountryModel, Integer> countryDao = ormDbHelper.getCountryDao();

            return countryDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }

    public static CountryModel getCountry(long ID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<CountryModel, Integer> countryDao = ormDbHelper.getCountryDao();

            final CountryModel country = countryDao.queryForFirst(countryDao.queryBuilder()
                    .where()
                    .eq(ID_COLUMN, ID).prepare());

            return country;

        } finally {
            ormDbHelper.close();
        }
    }
}
