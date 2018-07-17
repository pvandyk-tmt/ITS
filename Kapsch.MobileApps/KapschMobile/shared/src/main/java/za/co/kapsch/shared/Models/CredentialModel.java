package za.co.kapsch.shared.Models;

/**
 * Created by csenekal on 2016-09-08.
 */
public class CredentialModel {

    private String UserName;
    private String Password;

    public CredentialModel(String userName, String password){
        UserName = userName;
        Password = password;
    }

    public String getUserName() {
        return UserName;
    }

    public void setUserName(String userName) {
        UserName = userName;
    }

    public String getPassword() {
        return Password;
    }

    public void setPassword(String password) {
        Password = password;
    }
}
