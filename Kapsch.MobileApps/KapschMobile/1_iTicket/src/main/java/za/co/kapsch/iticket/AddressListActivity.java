package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

public class AddressListActivity extends AppCompatActivity {

    private String mSelectedListItem;
    private String[] mAddressList;
    private Button mOkButton;
    private ListView mListView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_address_list);
        Intent intent = getIntent();
        mAddressList = intent.getStringArrayExtra(Constants.ADDRESS_LIST_EXTRA);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.activity_address_list_title)));

        mOkButton = (Button) findViewById(R.id.okButton);
        mListView = (ListView) findViewById(R.id.addressListView);

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                view.setSelected(true);
                mSelectedListItem = ((TextView)view).getText().toString();
            }
        });

        mOkButton.setEnabled(false);
        PopulateAddressListView(mAddressList);
    }

    private void PopulateAddressListView(String[] addressList){
        if (addressList.length < 1) return;
        mOkButton.setEnabled(addressList.length > 0);
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, addressList);
        mListView.setAdapter(arrayAdapter);
    }

    @Override
    public void onBackPressed() {
        String data = "Operation Cancelled";
        Intent intent = new Intent();
        intent.putExtra(Constants.ADDRESS_SEARCH_RESULT, data);
        setResult(0, intent);
        finish();
    }

    public void returnAddressData(View view){
        Intent intent = new Intent();
        intent.putExtra(Constants.ADDRESS_SEARCH_RESULT, mSelectedListItem);
        setResult(RESULT_OK, intent);
        finish();
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
