package za.co.kapsch.shared.Enums;

/**
 * Created by CSenekal on 2018/04/17.
 */

public enum RegisterStatus {

    Open(0),
    Cancelled(1),
    Paid(2);

    private final int code;

    RegisterStatus(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}