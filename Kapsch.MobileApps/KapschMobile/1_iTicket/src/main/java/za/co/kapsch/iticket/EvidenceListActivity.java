package za.co.kapsch.iticket;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;

import java.sql.SQLException;
import java.util.List;

import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Enums.MediaPlayerMode;
import za.co.kapsch.iticket.Models.EvidenceModel;
import za.co.kapsch.iticket.orm.EvidenceRepository;

public class EvidenceListActivity extends AppCompatActivity{

    private String mTicketNumber;
    private ListView mEvidenceListView;
    private EvidenceModel mSelectedEvidence;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_evidence_list);

        mEvidenceListView = (ListView) findViewById(R.id.evidenceListView);

        Intent intent = getIntent();
        mTicketNumber = intent.getStringExtra(Constants.TICKET_NUMBER);

        setTitle(String.format("%1$s - %2$s",
                getResources().getString(R.string.app_name),
                getResources().getString(R.string.activity_evidence_title)));

        populateEvidenceLists();

        mEvidenceListView.setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {
            view.setSelected(true);
            mSelectedEvidence = (EvidenceModel) mEvidenceListView.getItemAtPosition(position);

            if (mSelectedEvidence.getEvidenceType() == EvidenceType.VoiceRecording) {
                playSound(mSelectedEvidence.getEvidence());
            }else {
                displayImage(mSelectedEvidence.getEvidence());
            }

            return true;
            }
        });
    }

    private void populateEvidenceLists(){
         try {
             List<EvidenceModel> evidenceList = EvidenceRepository.getEvidenceByTicketNumber(mTicketNumber);
            PopulateEvidenceListView(evidenceList);
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    private void PopulateEvidenceListView(List<EvidenceModel> evidenceList){
        if (evidenceList.size() < 1) return;
        EvidenceListAdapter arrayAdapter = new EvidenceListAdapter(this, evidenceList);
        mEvidenceListView.setAdapter(arrayAdapter);
    }

    private void playSound(byte[] soundData){
        Intent intent = new Intent(this, MediaManagerActivity.class);
        intent.putExtra(Constants.AUDIO_FILE_DATA, soundData);
        intent.putExtra(Constants.MEDIA_PLAYER_TYPE, MediaPlayerMode.toInteger(MediaPlayerMode.Player));
        startActivity(intent);
    }

    private void displayImage(byte[] imageData){
        Intent intent = new Intent(this, MediaManagerActivity.class);
        intent.putExtra(Constants.IMAGE_FILE_DATA, imageData);
        intent.putExtra(Constants.MEDIA_PLAYER_TYPE, MediaPlayerMode.toInteger(MediaPlayerMode.ImageViewer));
        startActivity(intent);
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
