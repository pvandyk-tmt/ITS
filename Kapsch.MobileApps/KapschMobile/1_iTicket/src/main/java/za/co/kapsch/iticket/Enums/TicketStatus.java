package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2017/08/02.
 */
public enum TicketStatus {
    Available,
    Issued;

    public static TicketStatus fromInteger(int x) {
        switch(x) {
            case 0 : return Available;
            case 1 : return Issued;
        }
        return null;
    }

    public static int toInteger(TicketStatus ticketStatus) {
        switch(ticketStatus) {
            case Available : return 0;
            case Issued : return 1;
        }
        return -1;
    }
}

