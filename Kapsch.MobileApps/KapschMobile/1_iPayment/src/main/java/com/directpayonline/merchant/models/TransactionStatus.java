package com.directpayonline.merchant.models;

/**
 * Created by CSenekal on 2017/12/18.
 */

public class TransactionStatus {

    public static final String TRANSACTION_PAID = "000";
    public static final String AUTHORIZED = "001";
    public static final String OVERPAY_UNDERPAY = "002";
    public static final String REQUEST_MISSING_COMPANY_TOKEN = "801";
    public static final String COMPANY_TOKEN_DOES_NOT_EXIST = "802";
    public static final String NO_REQUEST_OR_ERROR_REQUEST_TYPE_NAME = "803";
    public static final String ERROR_IN_XML = "804";
    public static final String NOT_PAID_YET = "900";
    public static final String DECLINED = "901";
    public static final String TRANSACTION_PASSED_PTL = "903";
    public static final String TRANSACTION_CANDELLED = "904";
    public static final String REQUEST_MISSING_TRANSACTION_LEVEL_MADATORY_FIELDS = "950";
    public static final String DATA_MISMATCH_IN_ON_OF_THE_FIELDS = "951";
}
