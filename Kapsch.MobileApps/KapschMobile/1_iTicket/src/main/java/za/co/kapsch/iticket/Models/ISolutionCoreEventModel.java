package za.co.kapsch.iticket.Models;

import java.sql.Date;

/**
 * Created by csenekal on 2017-01-13.
 */
public class ISolutionCoreEventModel {

    private String EventID;
    private String SolutionID;
    private String EventType; //'Location'
    private boolean RemoteAccess;
    private String VLNs;
    private String GPSLatitude;
    private String GPSLongitude;
    private Date Timestamp;
    private int VLNXRectangleCoordinates;
    private int VLNYRectangleCoordinates;
    private int VLNRectangleWidth;
    private int VLNRectangleHeight;
    private String InstanceID;
    private String Direction;
    private String DeviceID;
    private Date CreatedTimeStamp;
    private String Blob;
}
