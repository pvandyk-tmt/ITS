package za.co.kapsch.console;

import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.TextView;

import za.co.kapsch.console.orm.OrmDbHelper;

public class AboutActivity extends AppCompatActivity {

    private TextView mApplicationVersionTextView;
    private TextView mCodeVersionTextView;
    private TextView mDatabaseVersionTextView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_about);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.about)));

        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayHomeAsUpEnabled(true);

        mApplicationVersionTextView = (TextView) findViewById(R.id.applicationVersionTextView);
        mCodeVersionTextView = (TextView) findViewById(R.id.codeVersionTextView);
        mDatabaseVersionTextView = (TextView) findViewById(R.id.databaseVersionTextView);

        mApplicationVersionTextView.setText(BuildConfig.VERSION_NAME);
        mCodeVersionTextView.setText(Integer.toString(BuildConfig.VERSION_CODE));
        mDatabaseVersionTextView.setText(Integer.toString(OrmDbHelper.DATABASE_VERSION));
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
}
