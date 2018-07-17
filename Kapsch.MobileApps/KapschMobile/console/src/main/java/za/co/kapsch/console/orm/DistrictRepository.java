package za.co.kapsch.console.orm;

import android.app.Application;
import android.database.sqlite.SQLiteDatabase;
import android.text.TextUtils;

import com.j256.ormlite.dao.Dao;

import org.w3c.dom.Text;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2016-09-13.
 */
public class DistrictRepository {

    private static String ID_COLUMN = "ID";

    public static void cleanInsert(List<DistrictModel> districtList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            OrmDbHelper.deleteAll(districtDao);

            for (DistrictModel district: districtList){
                districtDao.create(district);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static void create(DistrictModel district) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            //there can only be one district
            final List<DistrictModel> districts = districtDao.queryForAll();
            districtDao.delete(districts);

            districtDao.create(district);
        } finally {
            ormDbHelper.close();
        }
    }

    public static void update(DistrictModel district) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();
            districtDao.update(district);
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean delete(DistrictModel district) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            return (districtDao.delete(district) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static void deleteAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            final List<DistrictModel> districts = districtDao.queryForAll();

            districtDao.delete(districts);
        } finally {
            ormDbHelper.close();
        }
    }

//    public static DistrictModel getDistrict()  throws SQLException {
//
//        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());
//
//        final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();
//
//        List<DistrictModel> districtList = districtDao.queryForAll();
//
//        if (districtList.size() > 0) {
//            return districtList.get(0);
//        }
//
//        return null;
//    }

    public static DistrictModel getDistrict(long districtID) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            final DistrictModel district = districtDao.queryForFirst(districtDao.queryBuilder()
                    .where()
                    .eq(ID_COLUMN, districtID).prepare());

            return district;
        }
        finally {
            ormDbHelper.close();
        }
    }

//    public static Long getDistrictID(){
//
//        try {
//            DistrictModel district = DistrictRepository.getDistrict();
//            if (district != null) {
//                return district.getID();
//            }
//        }catch (SQLException e){
//            MessageManager.showMessage(Utilities.exceptionMessage(e, "DistrictRepository::getDistrictID()"), ErrorSeverity.High);
//        }
//
//        return (long)-1;
//    }

    public static boolean hasDistricts() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            final List<DistrictModel> districts = districtDao.queryForAll();

            return districts.size() > 0;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static List<DistrictModel> getDistricts() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            return districtDao.queryForAll();

        } finally {
            ormDbHelper.close();
        }
    }

    public static List<DistrictModel> getDistricts(String IDs) throws SQLException {

        if (TextUtils.isEmpty(IDs)){
            return null;
        }

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<DistrictModel, Integer> districtDao = ormDbHelper.getDistrictDao();

            final List<DistrictModel> districts = districtDao.query(districtDao.queryBuilder()
                    .where()
                    .in(ID_COLUMN, IDs).prepare());

            return districts;
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "DistictRepository::getDistricts()"), ErrorSeverity.High);
            return null;
        }
        finally {
            ormDbHelper.close();
        }
    }
}
