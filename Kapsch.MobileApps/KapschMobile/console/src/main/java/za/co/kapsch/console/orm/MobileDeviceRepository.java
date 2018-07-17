package za.co.kapsch.console.orm;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Models.MobileDeviceModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/09/11.
 */
public class MobileDeviceRepository {

    public static void create(MobileDeviceModel mobileDevice) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceModel, Integer> mobileDeviceDao = ormDbHelper.getMobileDeviceDao();

            //there can only be one mobile device
            final List<MobileDeviceModel> mobileDeviceList = mobileDeviceDao.queryForAll();
            mobileDeviceDao.delete(mobileDeviceList);

            mobileDeviceDao.create(mobileDevice);
        } finally {
            ormDbHelper.close();
        }
    }

    public static boolean delete(MobileDeviceModel mobileDevice) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceModel, Integer> mobileDeviceDao = ormDbHelper.getMobileDeviceDao();

            return (mobileDeviceDao.delete(mobileDevice) > 0);
        } finally {
            ormDbHelper.close();
        }
    }

    public static void deleteAll() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<MobileDeviceModel, Integer> mobileDeviceDao = ormDbHelper.getMobileDeviceDao();

            final List<MobileDeviceModel> mobileDeviceList = mobileDeviceDao.queryForAll();

            mobileDeviceDao.delete(mobileDeviceList);
        } finally {
            ormDbHelper.close();
        }
    }

    public static MobileDeviceModel getMobileDevice() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        final Dao<MobileDeviceModel, Integer> mobileDeviceDao = ormDbHelper.getMobileDeviceDao();

        List<MobileDeviceModel> mobileDeviceList = mobileDeviceDao.queryForAll();

        if (mobileDeviceList.size() > 0) {
            return mobileDeviceList.get(0);
        }

        return null;
    }

    public static boolean hasMobileDevice() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        final Dao<MobileDeviceModel, Integer> mobileDeviceDao = ormDbHelper.getMobileDeviceDao();

        List<MobileDeviceModel> mobileDeviceList = mobileDeviceDao.queryForAll();

        return (mobileDeviceList.size() > 0);
    }

    public static Long getID(){

        try {
            MobileDeviceModel mobileDevice = MobileDeviceRepository.getMobileDevice();
            if (mobileDevice != null) {
                return mobileDevice.getID();
            }
        }catch (SQLException e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "MobileDeviceRepository::getID()"), ErrorSeverity.High);
        }

        return (long)-1;
    }
}
