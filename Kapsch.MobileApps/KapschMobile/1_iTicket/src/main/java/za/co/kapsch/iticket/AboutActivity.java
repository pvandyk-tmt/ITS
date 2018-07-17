package za.co.kapsch.iticket;

import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.TextView;

import za.co.kapsch.iticket.orm.OrmDbHelper;

public class AboutActivity extends AppCompatActivity {

    private TextView mApplicationVersionTextView;
    private TextView mCodeVersionTextView;
    private TextView mDatabaseVersionTextView;
    //private TextView mNewFeaturesTextView;
    //private TextView mBugFixesTextView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_about);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.activity_about_title)));

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        mApplicationVersionTextView = (TextView) findViewById(R.id.applicationVersionTextView);
        mCodeVersionTextView = (TextView) findViewById(R.id.codeVersionTextView);
        mDatabaseVersionTextView = (TextView) findViewById(R.id.databaseVersionTextView);
        //mNewFeaturesTextView = (TextView) findViewById(R.id.newFeaturesTextView);
        //mBugFixesTextView = (TextView) findViewById(R.id.bugFixesTextView);

        mApplicationVersionTextView.setText(BuildConfig.VERSION_NAME);
        mCodeVersionTextView.setText(Integer.toString(BuildConfig.VERSION_CODE));
        mDatabaseVersionTextView.setText(Integer.toString(OrmDbHelper.DATABASE_VERSION));
        //mNewFeaturesTextView.setText(newFeatures());
        //mBugFixesTextView.setText(bugFixes());
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
        }

        return super.onOptionsItemSelected(item);
    }

//    private String newFeatures(){
//
//        StringBuilder stringBuilder = new StringBuilder();
//
//        stringBuilder.append(getResources().getString(R.string.release_notes_new_features));
//
//        return stringBuilder.toString();
//    }
//
//    private String bugFixes(){
//
//        StringBuilder stringBuilder = new StringBuilder();
//
//        stringBuilder.append(getResources().getString(R.string.release_notes_bug_fixes));
//
//        return stringBuilder.toString();
//    }
}
