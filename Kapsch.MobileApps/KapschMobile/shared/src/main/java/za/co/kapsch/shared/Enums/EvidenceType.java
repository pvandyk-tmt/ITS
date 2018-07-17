package za.co.kapsch.shared.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-10-20.
 */
public enum EvidenceType {

    @SerializedName("1")
    OfficerSignature(1),

    @SerializedName("2")
    PersonSignature(2),

    @SerializedName("3")
    VehiclePhoto(3),

    @SerializedName("4")
    LicenceDisk(4),

    @SerializedName("5")
    DriversLicence(5),

    @SerializedName("6")
    OffenderPhoto(6),

    @SerializedName("7")
    Other(7),

    @SerializedName("8")
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
