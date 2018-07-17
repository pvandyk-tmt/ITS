package za.co.kapsch.iticket;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.media.MediaPlayer;
import android.media.MediaRecorder;
import android.net.Uri;
import android.os.Handler;
import android.os.SystemClock;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.SeekBar;
import android.widget.TextView;

import java.io.File;
import java.io.IOException;

import za.co.kapsch.iticket.Enums.MediaPlayerMode;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Utilities;

public class MediaManagerActivity extends AppCompatActivity {

    private Handler updatehandler = new Handler();

    private static final int MAX_DURATION = 60000;
    private MediaRecorder mMediaRecorder = null;

    private MediaPlayer mMediaPlayer;
    private long mStartTime;

    private MediaPlayerMode mMediaPlayerMode;

    private TextView mProgressView;
    private TextView mRemainingTextView;
    private SeekBar mSeekBar;

    private byte[] mAudioFile;

    private ImageView mImageView;
    private LinearLayout mImageLinearLayout;
    private ImageButton mRecordPlayButton;
    private Button mOkButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_media_manager);

        mProgressView = (TextView)findViewById(R.id.progressTextView);
        mRemainingTextView = (TextView)findViewById(R.id.remainingTextView);

        mRecordPlayButton = (ImageButton)findViewById(R.id.recordPlayButton);
        mOkButton = (Button)findViewById(R.id.okButton);
        mImageLinearLayout = (LinearLayout)findViewById(R.id.imageLinearLayout);
        mImageView = (ImageView)findViewById(R.id.imageView);
        mSeekBar = (SeekBar)  findViewById(R.id.seekBar);

        Intent intent = getIntent();
        mMediaPlayerMode =  MediaPlayerMode.fromInteger(intent.getIntExtra(Constants.MEDIA_PLAYER_TYPE, -1));

        switch (mMediaPlayerMode) {
            case Player:
                mAudioFile = intent.getByteArrayExtra(Constants.AUDIO_FILE_DATA);
                CreateMediaPlayer();
                break;
            case Recorder:
                break;
            case ImageViewer:
                byte[] image = intent.getByteArrayExtra(Constants.IMAGE_FILE_DATA);
                Bitmap bitmap = BitmapFactory.decodeByteArray(image, 0, image.length);
                mImageView.setImageBitmap(bitmap);
                break;
        }

        setupScreen();
    }

    private void CreateMediaPlayer(){
        try {

            if (mMediaPlayer != null) {
                mMediaPlayer.stop();
                mMediaPlayer.release();
            }

            File file = Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME);
            if (file == null) return;
            Utilities.writeBufferToFile(mAudioFile, file.getAbsolutePath());
            Uri uri = Uri.fromFile(Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME));
            mMediaPlayer = MediaPlayer.create(this, uri);

            mSeekBar.setMax(mMediaPlayer.getDuration());
            mMediaPlayer.setOnCompletionListener(new MediaPlayer.OnCompletionListener() {
                @Override
                public void onCompletion(MediaPlayer mp) {
                    CreateMediaPlayer();
                }
            });

        } catch (IOException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "AudioPlay:AudioPlay() 1"), ErrorSeverity.Medium);
            //Toast.makeText(App.getContext(), Utilities.exceptionMessage(e, "AudioPlay:AudioPlay() 1"), Toast.LENGTH_LONG ).show();
        } catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "AudioPlay:AudioPlay() 2"), ErrorSeverity.Medium);
            //Toast.makeText(App.getContext(), Utilities.exceptionMessage(e, "AudioPlay:AudioPlay() 2"), Toast.LENGTH_LONG ).show();
        }
    }

    private void setupScreen(){
        mProgressView.setText("0:00:000");

        switch (mMediaPlayerMode) {
            case Recorder:
                mRemainingTextView.setText(getFormattedText(MAX_DURATION));
                mRecordPlayButton.setImageResource(R.drawable.record_on);
                mOkButton.setVisibility(View.INVISIBLE);
                break;
            case Player:
                mRemainingTextView.setText(getFormattedText(mMediaPlayer.getDuration()));
                mRecordPlayButton.setImageResource(R.drawable.play);
                mOkButton.setVisibility(View.INVISIBLE);
                break;
            case ImageViewer:
                mImageLinearLayout.setLayoutParams(
                        new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.MATCH_PARENT,
                                ViewGroup.LayoutParams.MATCH_PARENT,
                                0.0f));
                break;
        }
    }

    public void startButtonClick(View view) {
        try {
            switch (mMediaPlayerMode) {
                case Recorder:
                    startRecording();
                    mOkButton.setVisibility(View.INVISIBLE);
                    break;
                case Player:
                    startAudioPlayBack();
                    break;
            }
        } catch (IllegalStateException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "startButtonClick(View view) 1"), ErrorSeverity.Medium);
            //Toast.makeText(App.getContext(), Utilities.exceptionMessage(e, "startButtonClick(View view) 1"), Toast.LENGTH_LONG).show();
        } catch (IOException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "startButtonClick(View view) 2"), ErrorSeverity.Medium);
            //Toast.makeText(App.getContext(), Utilities.exceptionMessage(e, "startButtonClick(View view) 2"), Toast.LENGTH_LONG).show();
        }
    }

    public void stopButtonClick(View view) {
        try {
            switch (mMediaPlayerMode) {
                case Recorder:
                    stopRecording();
                    mOkButton.setVisibility(View.VISIBLE);
                    break;
                case Player:
                    stopAudioPlayBack();
                    break;
            }
        } catch (IllegalStateException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "stopButtonClick(View view) 1"), ErrorSeverity.Medium);
            //Toast.makeText(App.getContext(), Utilities.exceptionMessage(e, "stopButtonClick(View view) 1"), Toast.LENGTH_LONG).show();
        }
    }

    public void okButtonClick(View view) {

        final File evidenceFile = Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME);

        if (evidenceFile.exists() == false){
            onBackPressed();
            return;
        }

        Intent intent = new Intent();
        setResult(RESULT_OK, intent);
        finish();
    }

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {

            long progress = 0;
            long remaining = 0;

            if (mMediaPlayerMode == MediaPlayerMode.Recorder) {
                progress = SystemClock.uptimeMillis() - mStartTime;
                remaining = MAX_DURATION - progress;
            }
            else if (mMediaPlayerMode == MediaPlayerMode.Player){
                progress = mMediaPlayer.getCurrentPosition();
                remaining = mMediaPlayer.getDuration() - progress;
            }

            mProgressView.setText(getFormattedText(progress));
            mRemainingTextView.setText(getFormattedText(remaining));

            mSeekBar.setProgress((int) progress);
            updatehandler.postDelayed(this, 0);
        }
    };

    private String getFormattedText(long duration){
        int secs = (int) (duration / 1000);
        int milliseconds = (int) (duration % 1000);
        return String.format("0:%02d:%03d", secs, milliseconds);
    }

    private void startRecording() throws IOException, IllegalStateException {
        try {
            if (mMediaRecorder == null) {
                mMediaRecorder = new MediaRecorder();
                mMediaRecorder.setAudioSource(MediaRecorder.AudioSource.MIC);
                mMediaRecorder.setOutputFormat(MediaRecorder.OutputFormat.MPEG_4);
                mMediaRecorder.setAudioEncoder(MediaRecorder.AudioEncoder.AMR_NB);
                mMediaRecorder.setOutputFile(Utilities.getTicketFile(Constants.TEMP_VOICE_EVIDENCE_FILENAME).getAbsolutePath());
                mMediaRecorder.setOnErrorListener(errorListener);
                mMediaRecorder.setOnInfoListener(infoListener);
                mMediaRecorder.setMaxDuration(MAX_DURATION);
                mSeekBar.setMax(MAX_DURATION);
                mMediaRecorder.prepare();
                mMediaRecorder.start();
                mStartTime = SystemClock.uptimeMillis();
                runnable.run();
            }
        }finally {

        }
    }

    private void stopRecording(){
        if (mMediaRecorder != null) {
            try {
                mMediaRecorder.stop();
                mMediaRecorder.reset();
                mMediaRecorder.release();
                mMediaRecorder = null;
                updatehandler.removeCallbacks(runnable);
            } finally {

            }
        }
    }

    @Override
    public void onBackPressed() {
        switch (mMediaPlayerMode) {
            case Recorder:
                stopRecording();
                break;
            case Player:
                stopAudioPlayBack();
                break;
        }

        super.onBackPressed();
        finish();
    }

    private void startAudioPlayBack(){

        if (mMediaPlayer == null){
            CreateMediaPlayer();
        }
        mMediaPlayer.start();
        runnable.run();
    }

    private void stopAudioPlayBack(){
        if (mMediaPlayer != null) {
            mMediaPlayer.stop();
            mMediaPlayer.release();
            mMediaPlayer = null;
            updatehandler.removeCallbacks(runnable);
        }
    }

    private MediaRecorder.OnErrorListener errorListener = new MediaRecorder.OnErrorListener() {
        @Override
        public void onError(MediaRecorder mr, int what, int extra) {
            switch (what) {
                case MediaRecorder.MEDIA_RECORDER_ERROR_UNKNOWN:
                    break;
                case MediaRecorder.MEDIA_ERROR_SERVER_DIED:
                    break;
            }
        }
    };

    private MediaRecorder.OnInfoListener infoListener = new MediaRecorder.OnInfoListener() {
        @Override
        public void onInfo(MediaRecorder mr, int what, int extra) {
            switch (what) {
                case MediaRecorder.MEDIA_RECORDER_INFO_UNKNOWN:
                    break;
                case MediaRecorder.MEDIA_RECORDER_INFO_MAX_DURATION_REACHED:
                    stopRecording();
                    MessageManager.showMessage(getResources().getString(R.string.activity_media_manager_maximum_duration_reached), ErrorSeverity.None);
                    //Toast.makeText(App.getContext(), getResources().getString(R.string.activity_media_manager_maximum_duration_reached), Toast.LENGTH_LONG).show();
                    break;
                case MediaRecorder.MEDIA_RECORDER_INFO_MAX_FILESIZE_REACHED:
                    break;
            }
        }
    };

    @Override
    public void onDestroy(){
        super.onDestroy();
    }
}
