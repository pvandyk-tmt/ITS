package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-11-30.
 */

public class FieldDeviceDlcLicModel {

    public String FieldDeviceDeviceId;

    public String TechnovolveDeviceId;

    @SerializedName("DrcSerializerRsaLic")
    public String DlcSerializerRsaLic;

    public String getFieldDeviceDeviceId() {
        return FieldDeviceDeviceId;
    }

    public void setFieldDeviceDeviceId(String fieldDeviceDeviceId) {
        FieldDeviceDeviceId = fieldDeviceDeviceId;
    }

    public String getTechnovolveDeviceId() {
        return TechnovolveDeviceId;
    }

    public void setTechnovolveDeviceId(String technovolveDeviceId) {
        TechnovolveDeviceId = technovolveDeviceId;
    }

    public String getDlcSerializerRsaLic() {
        return DlcSerializerRsaLic;
    }

    public void setDrcSerializerRsaLic(String dlcSerializerRsaLic) {
        DlcSerializerRsaLic = dlcSerializerRsaLic;
    }
}
