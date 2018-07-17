package za.co.tmt.iticket;

import android.widget.Toast;

/**
 * Created by csenekal on 2016-11-07.
 */
public class ErrorManager {
    public static void showMessage(String error, ErrorSeverity severity) {
        if ((severity == ErrorSeverity.High) || (severity == ErrorSeverity.Medium) || (severity == ErrorSeverity.None)) {
            Toast.makeText( App.getContext(), error, Toast.LENGTH_LONG).show();
        }
    }
}
