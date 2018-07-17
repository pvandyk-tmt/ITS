package za.co.kapsch.iticket;

import android.content.DialogInterface;
import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;

import za.co.kapsch.shared.Utilities;

public class AdminActivity extends AppCompatActivity implements  DialogInterface.OnClickListener {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_admin);
    }

    public void overwriteDatabaseClick(View view){
        //Utilities.displayDecisionMessage("Are you sure you want to overwrite the database?", this);
    }

    @Override
    public void onClick(DialogInterface dialog, int which) {

        if (which == Constants.YES) {
            Utilities.overwriteDatabaseFile(App.getContext().getPackageName(), "iTicket.db");
        } else {

        }
    }


}
