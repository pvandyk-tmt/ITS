package za.co.kapsch.shared;

import android.text.TextUtils;
import android.widget.Toast;

import za.co.kapsch.shared.Enums.ErrorSeverity;

/**
 * Created by csenekal on 2016-11-07.
 */
public class MessageManager {
    public static void showMessage(String error, ErrorSeverity severity) {

        if (TextUtils.isEmpty(error) == true){
            return;
        }
        
        if ((severity == ErrorSeverity.High) || (severity == ErrorSeverity.Medium) || (severity == ErrorSeverity.None)) {
            Toast.makeText(LibApp.getContext(), error, Toast.LENGTH_LONG).show();
        }
    }
}
