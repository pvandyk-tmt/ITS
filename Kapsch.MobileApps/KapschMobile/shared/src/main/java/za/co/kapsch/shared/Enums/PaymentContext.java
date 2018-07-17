package za.co.kapsch.shared.Enums;

/**
 * Created by csenekal on 2017/09/21.
 */
public enum PaymentContext {

    Unknown(0),
    TrafficFines(1),
    VehicleNumberPlates(2),
    VehicleTesting(3),
    Tolling(4);

    private final int code;

    PaymentContext(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}
