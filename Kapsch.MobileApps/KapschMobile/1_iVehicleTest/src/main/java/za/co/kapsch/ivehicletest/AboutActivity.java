package za.co.kapsch.ivehicletest;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Camera;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.Result;

import java.util.ArrayList;
import java.util.List;

import me.dm7.barcodescanner.zxing.ZXingScannerView;

import za.co.kapsch.ivehicletest.orm.OrmDbHelper;

public class AboutActivity extends AppCompatActivity implements ZXingScannerView.ResultHandler  {

    private ZXingScannerView mScannerView;

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

    public void onClick(View v) {

        mScannerView = new ZXingScannerView(this);
        setContentView(mScannerView);
        mScannerView.setResultHandler(this);
        mScannerView.startCamera();
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


    @Override
    protected void onPause() {
        super.onPause();

        if (mScannerView != null) {
            mScannerView.stopCamera();
            Intent i=new Intent(this,MainActivity.class);
            startActivity(i);
        }
    }

    @Override
    public void handleResult(Result result) {

        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle("Scan result");
        builder.setMessage(result.getText());
        AlertDialog alertDialog = builder.create();
        alertDialog.show();
    }



    @Override
    protected void onResume() {
        super.onResume();

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            int permissionCheck = ContextCompat.checkSelfPermission(this,
                    Manifest.permission.CAMERA);

            if (permissionCheck == PackageManager.PERMISSION_GRANTED) {
            } else {
                ActivityCompat.requestPermissions(this,
                        new String[]{Manifest.permission.CAMERA,
                                Manifest.permission.ACCESS_FINE_LOCATION,
                                Manifest.permission.ACCESS_COARSE_LOCATION}, 1);
            }
        } else {
        }
    }
}
