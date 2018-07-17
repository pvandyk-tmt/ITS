package za.co.kapsch.console;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.LinearLayout;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.console.General.App;
import za.co.kapsch.console.General.ConfigItemSynchroniser;
import za.co.kapsch.console.General.DataServiceRequest;
import za.co.kapsch.shared.Enums.Environment;
import za.co.kapsch.console.Enums.ErrorSeverity;
import za.co.kapsch.console.General.Constants;
import za.co.kapsch.console.General.MessageManager;
import za.co.kapsch.shared.Interfaces.IAsyncProcessCallBack;
import za.co.kapsch.shared.Models.AsyncResultModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.console.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.UserModel;

import za.co.kapsch.console.orm.UserRepository;

import static za.co.kapsch.shared.WebAccess.DataService.FAILED;
import static za.co.kapsch.shared.WebAccess.DataService.SUCCESS;

public class LoginActivity extends AppCompatActivity implements IAsyncProcessCallBack{

    private boolean mValidateLocally;
    private EditText mUsernameEditText;
    private EditText mPasswordEditText;
    private ConfigItemSynchroniser mConfigItemSynchroniser;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        setTheme(R.style.AppTheme);
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        Intent intent = getIntent();
        mValidateLocally = intent.getBooleanExtra(Constants.VALIDATE_LOCALLY, false);

        mUsernameEditText = (EditText) findViewById(R.id.usernameEditText);
        mPasswordEditText = (EditText) findViewById(R.id.paswordEditText);
        //mBackgroundLinearLayout = (LinearLayout) findViewById(R.id.backgroundLinearLayout);

        setTitle(String.format("%1$s - %2$s",
                 getResources().getString(R.string.app_name),
                 getResources().getString(R.string.login)));

    }

    @Override
    public void onBackPressed(){
        //super.onBackPressed();
        //returnLoginCancelled();
    }

    @Override
    public void progressCallBack(AsyncResultModel asyncResultModel) {
        za.co.kapsch.shared.MessageManager.showMessage( asyncResultModel.getMessage(), za.co.kapsch.shared.Enums.ErrorSeverity.None);
    }

    @Override
    public void finishedCallBack(AsyncResultModel asyncResultModel) {

        try{
            if (asyncResultModel == null){
                return;
            }

            switch (asyncResultModel.getProcessId()) {

                case Constants.PROCESS_ID_GET_DEVICE_CONFIG_ITEM:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            getConfigItemSynchroniser().setConfigItemList((List<ConfigItemModel>)asyncResultModel.getObject());
                            DataServiceRequest.usersRequest(this, this);
                            return;
                        case FAILED:
                            if (asyncResultModel.getMessage().contains("Credential is not found.")){
                                MessageManager.showMessage(getResources().getString(R.string.invalid_login_credentials), ErrorSeverity.None);
                            }else if (asyncResultModel.getMessage().contains("Password is incorrect.")){
                                MessageManager.showMessage(getResources().getString(R.string.invalid_login_credentials), ErrorSeverity.None);
                            }else{
                                MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                            }
                            clearUserInterface();
                            break;
                    }
                    break;

                case Constants.PROCESS_ID_DOWNLOAD_USERS:
                    switch (asyncResultModel.getProcessResult()) {
                        case SUCCESS:
                            List<UserModel> userList = (List<UserModel>)asyncResultModel.getObject();
                            if (userList.size() == 0){
                                MessageManager.showMessage(getResources().getString(R.string.user_list_is_empty), ErrorSeverity.None);
                                return;
                            }
                            MessageManager.showMessage(mConfigItemSynchroniser.insertOfficers(userList), ErrorSeverity.None);
                            retryLocalLogin();
                            break;
                        case FAILED:
                            MessageManager.showMessage(asyncResultModel.getMessage(), ErrorSeverity.High);
                            break;
                    }
                    break;

            }
        }catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, String.format("finishedCallBack() - PROCESS_ID: %d", asyncResultModel.getProcessId())), ErrorSeverity.High);
            return;
        }
    }

    public void processUpdates() throws Exception {

        if (centralConfigItems() == true) return;
        if (updateUsers() == true) return;
    }

    public boolean centralConfigItems(){

        if (mConfigItemSynchroniser == null){
            DataServiceRequest.configItemRequest(this, this);
            return true;
        }

        return false;
    }

    private boolean updateUsers() throws SQLException {

        if (mConfigItemSynchroniser.doUpdateOfficers()) {
            MessageManager.showMessage(Utilities.getString(R.string.downloading_users), ErrorSeverity.None);
            DataServiceRequest.usersRequest(this, this);
            return true;
        }else {
            retryLocalLogin();
        }

        return false;
    }

    private ConfigItemSynchroniser getConfigItemSynchroniser(){

        mConfigItemSynchroniser = mConfigItemSynchroniser == null ? new ConfigItemSynchroniser() : mConfigItemSynchroniser;
        return mConfigItemSynchroniser;
    }

    public void login(View view){

        boolean success = initialLocalLogin();

        if (success == true){
            return;
        }

        getUserListFromServer();
    }

    public boolean initialLocalLogin(){

        SessionModel.getInstance().clearSession();

        UserModel user = getUser();

        if (user == null){
            return false;
        }

        SessionModel.getInstance().setUser(user);
        Utilities.logUserActivity("login", "successful");

        clearUserInterface();
        returnUserData(user);
        return true;
    }

    public void retryLocalLogin(){

        UserModel user = getUser();

        if (user == null){
            MessageManager.showMessage(getResources().getString(R.string.invalid_login_credentials), ErrorSeverity.None);
            Utilities.logUserActivity("login", String.format("failed-%s", mUsernameEditText.getText().toString()));
            clearUserInterface();
            return;
        }

        SessionModel.getInstance().setUser(user);
        Utilities.logUserActivity("login", "successful");

        clearUserInterface();
        returnUserData(user);
        return;
    }

    private void getUserListFromServer(){

        try {
            SessionModel.getInstance().setUserName(mUsernameEditText.getText().toString());
            SessionModel.getInstance().setPassword(mPasswordEditText.getText().toString());
            processUpdates();
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "getUserListFromServer()"), ErrorSeverity.High);
        }
    }

    public void returnUserData(UserModel user){
        Intent intent = new Intent();
        intent.putExtra(za.co.kapsch.shared.Constants.USER, user);
        setResult(RESULT_OK, intent);
        finish();
    }

    private UserModel getUser(){
        try{
            if (mValidateLocally == true) {
                return UserRepository.getUser(mUsernameEditText.getText().toString(), Utilities.computeSHA256Hash(mPasswordEditText.getText().toString()).toUpperCase());
            }else{
                UserModel user = new UserModel();
                user.setUserName(mUsernameEditText.getText().toString());
                user.setPassword(mPasswordEditText.getText().toString());
                return user;
            }
        }
        catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "LoginActivity::getUser()"), ErrorSeverity.High);
            return null;
        }
    }

    private void clearUserInterface(){
        mUsernameEditText.setText(Constants.EMPTY_STRING);
        mPasswordEditText.setText(Constants.EMPTY_STRING);
        mUsernameEditText.requestFocus();
    }
}
