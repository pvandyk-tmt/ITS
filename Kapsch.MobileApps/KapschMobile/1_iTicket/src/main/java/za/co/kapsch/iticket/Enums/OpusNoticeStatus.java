package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2017/02/03.
 */
public enum OpusNoticeStatus {

    Available,
    Issued;

    public static OpusNoticeStatus fromInteger(int x) {
        switch(x) {
            case 0 : return Available;
            case 1 : return Issued;
        }
        return null;
    }

    public static int toInteger(OpusNoticeStatus opusNoticeStatus) {
        switch(opusNoticeStatus) {
            case Available : return 0;
            case Issued : return 1;
        }
        return -1;
    }
}
