package za.co.kapsch.console;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.shared.Constants;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.ConfigItemModel;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.shared.orm.ConfigItemRepository;

public class ConfigurationActivity extends AppCompatActivity {

    private Button mButton;
    private ListView mListView;
    private EditText mDescEditText;
    private EditText mValueEditText;
    private ConfigItemModel mConfigItem;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_configuration);

        mButton = (Button)findViewById(R.id.saveButton);
        mDescEditText = (EditText)findViewById(R.id.descEditText);
        mValueEditText = (EditText)findViewById(R.id.valueEditText);
        mListView = (ListView)findViewById(R.id.listView);

        LayoutInflater inflater = getLayoutInflater();
        ViewGroup header = (ViewGroup)inflater.inflate(R.layout.configuration_header, mListView, false);
        mListView.addHeaderView(header);

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, final View view, int position, long id) {
                try {
                    view.setSelected(true);
                    mConfigItem = (ConfigItemModel) mListView.getItemAtPosition(position);
                    if (mConfigItem == null) return;
                    mDescEditText.setText(mConfigItem.getDescription());
                    mValueEditText.setText(mConfigItem.getValue());
                } catch (Exception e) {
                    MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigurationActivity::setOnItemClickListener()"), ErrorSeverity.High);
                }
            }
        });

        mListView.setOnTouchListener(new View.OnTouchListener() {
            private float mDownX;
            private float mDownY;
            private final float SCROLL_THRESHOLD = 10;
            private boolean isOnClick;

            @Override
            public boolean onTouch(View view, MotionEvent event) {

                switch (event.getAction() & MotionEvent.ACTION_MASK) {

                    case MotionEvent.ACTION_DOWN:
                        mDownX = event.getX();
                        mDownY = event.getY();
                        isOnClick = true;
                        break;
                    case MotionEvent.ACTION_CANCEL:
//                   case MotionEvent.ACTION_UP:
//                       if (isOnClick) {
//                           return false;
//                       }
                        break;
                    case MotionEvent.ACTION_MOVE:
                        if (isOnClick && (Math.abs(mDownX - event.getX()) > SCROLL_THRESHOLD ||  Math.abs(mDownY - event.getY()) > SCROLL_THRESHOLD)) {
                            //isOnClick = false;
                            mListView.setSelector(android.R.color.transparent);
                            mConfigItem = null;
                            mDescEditText.setText(Constants.EMPTY_STRING);
                            mValueEditText.setText(Constants.EMPTY_STRING);
                        }
                        break;
                    default:
                        break;
                }
                return false;
            }
        });

        populateListView();
    }

    private void populateListView(){
        try {
            List<ConfigItemModel> mConfigItemList = ConfigItemRepository.getAll();
            if (mConfigItemList.size() < 1) return;
            ConfigurationListAdapter adapter = new ConfigurationListAdapter(this, mConfigItemList);
            mListView.setAdapter(adapter);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    public void saveClick(View view){
        try{
            if (mConfigItem != null) {
                mConfigItem.setValue(mValueEditText.getText().toString());
                ConfigItemRepository.update(mConfigItem);
                populateListView();
                updateEndPoints();
            }
        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigurationActivity::saveClick()"), ErrorSeverity.High);
        }
    }

    private void updateEndPoints(){
       try {
          EndPointConfigModel.getInstance().setCoreGateway(ConfigItemRepository.getConfigItem("CORE_GATEWAY").getValue());
          EndPointConfigModel.getInstance().setITSGateway(ConfigItemRepository.getConfigItem("ITS_GATEWAY").getValue());
          EndPointConfigModel.getInstance().setEVRGateway(ConfigItemRepository.getConfigItem("EVR_GATEWAY").getValue());
       }catch (SQLException e){
          MessageManager.showMessage(Utilities.exceptionMessage(e, "ConfigurationActivity::updateEndPoints()"), ErrorSeverity.High);
       }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
