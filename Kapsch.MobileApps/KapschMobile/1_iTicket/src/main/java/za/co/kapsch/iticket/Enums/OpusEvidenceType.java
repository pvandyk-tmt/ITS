package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2017/02/14.
 */
public enum OpusEvidenceType {

    OfficerSignature(1),
    OffenderSignature(2),
    VehiclePhoto(3),
    LicenceDisk(4),
    DriversLicence(5),
    OffenderPhoto(6),
    Other(7);

    private int mNumValue;

    OpusEvidenceType(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }
}
