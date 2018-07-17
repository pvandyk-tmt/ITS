package za.co.kapsch.iticket.orm;

import android.database.sqlite.SQLiteDatabase;

import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.stmt.QueryBuilder;
import com.j256.ormlite.stmt.Where;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Enums.SqlJunction;
import za.co.kapsch.iticket.Models.ChargeInfoModel;

/**
 * Created by csenekal on 2016-09-07.
 */
public class ChargeInfoRepository {

    private static final String DESC_COLUMN = "Description";
    private static final String CODE_COLUMN = "Code";
    private static final String ISFAVOURITE_COLUMN = "IsFavourite";
    private static final String ZONE_COLUMN = "Zone";
    private static final String MIN_SPEED_COLUMN = "MinSpeed";
    private static final String MAX_SPEED_COLUMN = "MaxSpeed";
    private static final String VEHICLE_TYPE_COLUMN = "VehicleType";

    public static ChargeInfoModel getCharge(String chargeCode) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

            return chargeBookDao.queryForFirst(chargeBookDao.queryBuilder()
                    .where().eq(CODE_COLUMN, chargeCode).prepare());

        } finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getCharges(String code, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

            if (zone == null) {
                return chargeBookDao.query(chargeBookDao.queryBuilder()
                        .where().eq(CODE_COLUMN, code).prepare());
            }

            return chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .eq(CODE_COLUMN, code)
                    .and()
                    .eq(ZONE_COLUMN, zone).prepare());

        } finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getAllCharges() throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

                return chargeBookDao.queryForAll();

        } finally {
            ormDbHelper.close();
        }
    }


    public static void cleanInsert(List<ChargeInfoModel> offenceCodeList) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        SQLiteDatabase db = ormDbHelper.getWritableDatabase();

        try {
            db.beginTransaction();

            final Dao<ChargeInfoModel, Integer> offenceCodeDao = ormDbHelper.getChargeBookDao();

            OrmDbHelper.deleteAll(offenceCodeDao);

            for (ChargeInfoModel offenceCode: offenceCodeList){
                offenceCodeDao.create(offenceCode);
            }

            db.setTransactionSuccessful();
        } finally {
            db.endTransaction();
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesWithoutJunction(String partialDesc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

            return chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .like(DESC_COLUMN, "%" + partialDesc + "%").prepare());
          }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesWithOrJunction(String[] partialDesc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            return chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .like(DESC_COLUMN, "%" + partialDesc[0] + "%")
                    .or()
                    .like(DESC_COLUMN, "%" + partialDesc[1] + "%").prepare());
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesWithOrJunctionEx(String[] partialDesc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            return chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .like(DESC_COLUMN, "%" + partialDesc[0] + "%")
                    .or()
                    .like(DESC_COLUMN, "%" + partialDesc[1] + "%")
                    .or()
                    .like(DESC_COLUMN, "%" + partialDesc[2] + "%").prepare());
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesWithAndJunction(String[] partialDesc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            return chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .like(DESC_COLUMN, "%" + partialDesc[0] + "%")
                    .and()
                    .like(DESC_COLUMN, "%" + partialDesc[1] + "%").prepare());
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesWithAndJunctionEx(String[] partialDesc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            return chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .like(DESC_COLUMN, "%" + partialDesc[0] + "%")
                    .and()
                    .like(DESC_COLUMN, "%" + partialDesc[1] + "%")
                    .and()
                    .like(DESC_COLUMN, "%" + partialDesc[2] + "%").prepare());
        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesWithBothJunctionEx(String[] partialDesc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            QueryBuilder<ChargeInfoModel, Integer> queryBuilder = chargeBookDao.queryBuilder();
            Where<ChargeInfoModel, Integer> where = queryBuilder.where();

            where.like(DESC_COLUMN, "%" + partialDesc[0] + "%")
                    .and().or(where.like(DESC_COLUMN, "%" + partialDesc[1] + "%"),
                    where.like(DESC_COLUMN, "%" + partialDesc[2] + "%"));

            return queryBuilder.query();

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesByDesc(String desc, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            QueryBuilder<ChargeInfoModel, Integer> queryBuilder = chargeBookDao.queryBuilder();
            Where<ChargeInfoModel, Integer> where = queryBuilder.where();

            if (zone == null) {
               where.like(DESC_COLUMN, "%" + desc + "%");
            }else {
               where.like(DESC_COLUMN, "%" + desc + "%")
                        .and().eq(ZONE_COLUMN, zone);
            }

            return queryBuilder.query();

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargesByCode(String code, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            QueryBuilder<ChargeInfoModel, Integer> queryBuilder = chargeBookDao.queryBuilder();
            Where<ChargeInfoModel, Integer> where = queryBuilder.where();

            where.like(CODE_COLUMN, "%" + code + "%");

            return queryBuilder.query();

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargeByCodeAndZone(String code, String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();
            QueryBuilder<ChargeInfoModel, Integer> queryBuilder = chargeBookDao.queryBuilder();
            Where<ChargeInfoModel, Integer> where = queryBuilder.where();

            where.eq(CODE_COLUMN, code)
                    .and()
                    .eq(ZONE_COLUMN, zone);

            return queryBuilder.query();

        }finally {
            ormDbHelper.close();
        }
    }

    public static List<ChargeInfoModel> getChargeByZoneAndSpeed(int zone, int speed) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

            QueryBuilder<ChargeInfoModel, Integer> queryBuilder = chargeBookDao.queryBuilder();

            queryBuilder
                    .where()
                    //.eq(Constants.TABLE_CHARGE_BOOK_VEHICLE_TYPE_COLUMN, "Light - Urban")
                    //.and()
                    .eq(ZONE_COLUMN, zone)
                    .and()
                    .le(MIN_SPEED_COLUMN, speed)
                    .and()
                    .gt(MAX_SPEED_COLUMN, speed);

            return queryBuilder.query();

        }finally {
            ormDbHelper.close();
        }
    }


    public static List<ChargeInfoModel> getAllCharges(String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

            return chargeBookDao.query(chargeBookDao.queryBuilder()
                            .where().eq(ZONE_COLUMN, 80).prepare());

        }finally {
            ormDbHelper.close();
        }
    }
    public static List<ChargeInfoModel> getFavouriteCharges(String zone) throws SQLException {

        final OrmDbHelper ormDbHelper = new OrmDbHelper(App.getContext());

        try {
            final Dao<ChargeInfoModel, Integer> chargeBookDao = ormDbHelper.getChargeBookDao();

            final List<ChargeInfoModel> offenceCodes = chargeBookDao.query(chargeBookDao.queryBuilder()
                    .where()
                    .eq(ISFAVOURITE_COLUMN, true).prepare());

            return offenceCodes;
        }
        finally {
            ormDbHelper.close();
        }
    }

    private static SqlJunction SqlJunction(String value){
        if (value.contains(Constants.OR) && value.contains(Constants.AND))return SqlJunction.Both;
        if (value.contains(Constants.OR)) return SqlJunction.Or;
        if (value.contains(Constants.AND)) return SqlJunction.And;
        return SqlJunction.None;
    }
}
