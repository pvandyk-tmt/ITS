package za.co.kapsch.console.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.SystemFunctionModel;
import za.co.kapsch.shared.Models.UserModel;

import static za.co.kapsch.console.orm.DistrictRepository.getDistricts;

/**
 * Created by csenekal on 2016-09-15.
 */
public class UserRepository {

    public static final String USERNAME_COLUMN = "UserName";
    public static final String PASSWORD_COLUMN = "Password";
    public static final String USER_ID_COLUMN = "UserID";

    public static UserModel getUser(String username, String password) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<UserModel, Integer> userDao = ormDbHelper.getUserDao();

            final UserModel user = userDao.queryForFirst(userDao.queryBuilder()
                    .where()
                    .eq(USERNAME_COLUMN, username)
                    .and()
                    .eq(PASSWORD_COLUMN, password).prepare());

            if (user != null) {
                user.setSystemFunctions(SystemFunctionRepository.getSystemFunctions(user.getSystemFunctionIDs()));
                List<DistrictModel> districtList = DistrictRepository.getDistricts(user.getDistrictIDs());
                if (districtList != null) {
                    user.setDistricts(districtList);
                }
            }

            return user;
        }
          finally {
            ormDbHelper.close();
        }
    }

    public static boolean hasUsers() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<UserModel, Integer> userDao = ormDbHelper.getUserDao();

            final List<UserModel> users = userDao.queryForAll();

            for (UserModel user : users) {
                if ((user.getUserName().equals(ConfigItemModel.getInstance().getTmtApiUser())) &&
                    (user.getPassword().equals(ConfigItemModel.getInstance().getTmtApiUser())) &&
                    (user.getInfrastructureNumber().equals(ConfigItemModel.getInstance().getTmtApiUser()))){
                    users.remove(user);
                }
            }

            return users.size() > 0;
        }
        finally {
            ormDbHelper.close();
        }
    }

    public static void cleanInsert(List<UserModel> userList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<UserModel, Integer> userDao = ormDbHelper.getUserDao();

            ormDbHelper.deleteAll(userDao);

            for (UserModel user : userList) {
                user.setSystemFunctionIDs();
                user.setDistrictIDs();
                userDao.create(user);
            }

            db.setTransactionSuccessful();
        }
        catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "UserRepository::cleanInsert()"), ErrorSeverity.High);
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static void updateUser(UserModel user) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<UserModel, Integer> userDao = ormDbHelper.getUserDao();
            userDao.update(user);
        }
        finally {
            ormDbHelper.close();
        }
    }
}
