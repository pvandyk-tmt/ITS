package za.co.kapsch.ivehicletest.Models;

import com.google.gson.annotations.SerializedName;

import java.util.List;

/**
 * Created by csenekal on 2017/11/28.
 */

public class VehicleLookUpsModel {

    @SerializedName("vehicleMakes")
    private List<VehicleMakeModel> mVehicleMakes;

    @SerializedName("vehicleModels")
    private List<VehicleMakeModelModel> mVehicleModels;

    @SerializedName("vehicleModelNumbers")
    private List<VehicleModelNumberModel> mVehicleModelNumbers;

    public List<VehicleMakeModel> getVehicleMakes() {
        return mVehicleMakes;
    }

    public List<VehicleMakeModelModel> getVehicleModels() {
        return mVehicleModels;
    }

    public List<VehicleModelNumberModel> getVehicleModelNumbers() {
        return mVehicleModelNumbers;
    }
}
