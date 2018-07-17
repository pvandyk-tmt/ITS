package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2017/01/19.
 */
public enum TicketProcessingSystem {
    SyntellOpus(0),
    TmtCentral(1),
    KapschITS(2);

    private int mNumValue;

    TicketProcessingSystem(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }

    public static TicketProcessingSystem fromInteger(int x) {

        switch(x) {
            case 0 : return SyntellOpus;
            case 1 : return TmtCentral;
            case 2 : return KapschITS;
        }
        return null;
    }

    public static int toInteger(TicketProcessingSystem ticketProcessingSystem) {

        switch(ticketProcessingSystem) {
            case SyntellOpus: return 0;
            case TmtCentral : return 1;
            case KapschITS : return 2;
        }
        return -1;
    }
}
