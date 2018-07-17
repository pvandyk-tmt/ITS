package za.co.kapsch.iticket.Enums;

/**
 * Created by csenekal on 2016-10-20.
 */
public enum EvidenceType {

    OfficerSignature(1),
    PersonSignature(2),
    VehiclePhoto(3),
    LicenceDisk(4),
    DriversLicence(5),
    OffenderPhoto(6),
    Other(7),
    VoiceRecording(8);

    public static String evidenceToString(EvidenceType evidenceType) {

        switch(evidenceType) {
            case OfficerSignature: return "OfficerSignature";
            case PersonSignature : return "PersonSignature";
            case VehiclePhoto : return "VehiclePhoto";
            case LicenceDisk : return "LicenceDisk";
            case DriversLicence : return "DriversLicence";
            case OffenderPhoto : return "OffenderPhoto";
            case Other : return "Other";
            case VoiceRecording : return "VoiceRecording";
            default:
                return null;
        }
    }

    private int mNumValue;

    EvidenceType(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }
}
