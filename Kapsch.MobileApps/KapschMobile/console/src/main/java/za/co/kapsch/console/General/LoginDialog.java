package za.co.kapsch.console.General;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.text.InputType;
import android.view.View;
import android.widget.EditText;
import android.widget.LinearLayout;

import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.Interfaces.ILoginDialogCallBack;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.console.R;
import za.co.kapsch.console.orm.UserRepository;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/06/30.
 */
public class LoginDialog {

    public static void show(Context context, final ILoginDialogCallBack loginDialogCallBack, final boolean validateLocally){

        try {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.setTitle(Utilities.getString(R.string.login));

            LinearLayout layout = new LinearLayout(context);
            layout.setOrientation(LinearLayout.VERTICAL);

            final EditText usernameEditText = new EditText(context);
            usernameEditText.setHint(Utilities.getString(R.string.username));
            layout.addView(usernameEditText);

            final EditText passwordEditText = new EditText(context);
            passwordEditText.setInputType(InputType.TYPE_TEXT_VARIATION_PASSWORD);
            passwordEditText.setHint(Utilities.getString(R.string.password));
            layout.addView(passwordEditText);

            builder.setPositiveButton(Utilities.getString(R.string.ok), new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                }
            });

            builder.setNegativeButton(Utilities.getString(R.string.cancel), new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    dialog.cancel();
                }
            });

            builder.setView(layout);

            final AlertDialog dialog = builder.create();
            dialog.show();

            dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {

                        String username = usernameEditText.getText().toString();
                        String password = passwordEditText.getText().toString();

                        if (validateLocally == true) {
                            UserModel user = UserRepository.getUser(username, password);

                            if (user != null) {
                                dialog.dismiss();
                                loginDialogCallBack.loginResult(user);
                            }
                        }else{
                            UserModel user = new UserModel();
                            user.setUserName(username);
                            user.setPassword(password);
                            dialog.dismiss();
                            loginDialogCallBack.loginResult(user);
                        }

                    } catch (Exception e) {
                        MessageManager.showMessage(Utilities.exceptionMessage(e, "MainActivity::validateUser()"), ErrorSeverity.High);
                    }
                }
            });
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, ""), ErrorSeverity.None);
        }
    }
}
