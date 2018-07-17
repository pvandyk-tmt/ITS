package za.co.kapsch.console.General;

import android.app.Application;
import android.widget.Toast;

import za.co.kapsch.console.Enums.ErrorSeverity;

/**
 * Created by csenekal on 2016-11-07.
 */
public class MessageManager {
    public static void showMessage(String error, ErrorSeverity severity) {
        if ((severity == ErrorSeverity.High) || (severity == ErrorSeverity.Medium) || (severity == ErrorSeverity.None)) {
            Toast.makeText(App.getContext(), error, Toast.LENGTH_LONG).show();
        }
    }
}
