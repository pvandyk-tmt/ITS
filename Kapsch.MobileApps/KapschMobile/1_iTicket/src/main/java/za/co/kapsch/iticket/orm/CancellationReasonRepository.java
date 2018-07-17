package za.co.kapsch.iticket.orm;

import android.app.Activity;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Models.CancellationReasonModel;

/**
 * Created by csenekal on 2016-10-06.
 */
public class CancellationReasonRepository {
    public static List<CancellationReasonModel> getCancellationReasons(Activity activity) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(activity);

        try {
            final Dao<CancellationReasonModel, Integer> cancellationReasonDao = ormDbHelper.getCancellationReasonDao();

            return cancellationReasonDao.queryForAll();
        } finally {
            ormDbHelper.close();
        }
    }
}
