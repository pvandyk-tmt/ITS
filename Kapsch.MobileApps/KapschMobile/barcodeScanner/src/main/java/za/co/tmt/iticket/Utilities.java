package za.co.tmt.iticket;

/**
 * Created by csenekal on 2016-07-17.
 */
public class Utilities {

    public static String exceptionMessage(Exception e, String additionInfo) {
        String cause;

        try {
            cause = e.getCause().getCause().getMessage();
        } catch (Exception ex) {
            cause = Constants.EMPTY_STRING;
        }

        if (e.getMessage() == null) {
            return String.format("%s: %s: %s", additionInfo, cause, e.toString());
        } else
            return String.format("%s: %s: %s", additionInfo, cause, e.getMessage());
    }
}
