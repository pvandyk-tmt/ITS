package za.co.kapsch.iticket.Enums;

/**
 * Created by csenekal on 2016-10-31.
 */
public enum MediaPlayerMode {
    Recorder,
    Player,
    ImageViewer;

    public static MediaPlayerMode fromInteger(int x) {
        switch(x) {
            case 0 : return Recorder;
            case 1 : return Player;
            case 2 : return ImageViewer;
        }
        return null;
    }

    public static int toInteger(MediaPlayerMode MediaPlayerMode) {
        switch(MediaPlayerMode) {
            case Recorder : return 0;
            case Player : return 1;
            case ImageViewer: return 2;
        }
        return -1;
    }
}
