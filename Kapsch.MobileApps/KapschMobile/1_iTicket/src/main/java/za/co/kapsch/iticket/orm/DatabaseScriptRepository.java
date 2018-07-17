package za.co.kapsch.iticket.orm;

import android.app.Activity;

import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.GenericRawResults;
import com.j256.ormlite.stmt.QueryBuilder;

import java.sql.SQLException;

import za.co.kapsch.iticket.Models.DatabaseScriptModel;

/**
 * Created by csenekal on 2016-09-13.
 */
public class DatabaseScriptRepository {

    public static int getMaxDatabaseScriptId(Activity activity) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(activity);

        try {
            final Dao<DatabaseScriptModel, Integer> databaseScriptModelDao = ormDbHelper.getDatabaseScriptDao();

            QueryBuilder queryBuilder = databaseScriptModelDao.queryBuilder();

            queryBuilder.selectRaw("MAX(id)");

            GenericRawResults<String[]> results = databaseScriptModelDao.queryRaw(queryBuilder.prepareStatementString());

            String[] values = results.getFirstResult();

            return Integer.parseInt(values[0]);

        } finally {
            ormDbHelper.close();
        }
    }
}
