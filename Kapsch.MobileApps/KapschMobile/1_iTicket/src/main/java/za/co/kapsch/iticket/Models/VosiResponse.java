package za.co.kapsch.iticket.Models;

/**
 * Created by csenekal on 2017/03/07.
 */
public class VosiResponse {

    public String status;
    public String errorCode;
    public String message;
    public String caseNumber;
    public String caseStation;
    public String dateStolen;
    public String engine;
    public String vin;
    public String makeDescription;
    public String reason;
    public String message1;
    public String message2;
    public String error;

    public String getStatus() {
        return status;
    }

    public String getErrorCode() {
        return errorCode;
    }

    public String getMessage() {
        return message;
    }

    public String getCaseNumber() {
        return caseNumber;
    }

    public String getCaseStation() {
        return caseStation;
    }

    public String getDateStolen() {
        return dateStolen;
    }

    public String getEngine() {
        return engine;
    }

    public String getVin() {
        return vin;
    }

    public String getMakeDescription() {
        return makeDescription;
    }

    public String getReason() {
        return reason;
    }

    public String getMessage1() {
        return message1;
    }

    public String getMessage2() {
        return message2;
    }

    public String getError() {
        return error;
    }
}
