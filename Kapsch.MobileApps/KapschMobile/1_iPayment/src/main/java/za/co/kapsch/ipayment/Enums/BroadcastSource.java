package za.co.kapsch.ipayment.Enums;

/**
 * Created by CSenekal on 2017/09/13.
 */
public enum BroadcastSource {

    Timeout(0),
    TransactionService(1),
    Query(3);

    private final int code;

    BroadcastSource(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }

    public static BroadcastSource fromCode(int code) throws Exception{

        switch (code){
            case 0: return Timeout;
            case 1: return TransactionService;
            case 3: return Query;
            default: throw new Exception("Invalid value");
        }
    }
}
