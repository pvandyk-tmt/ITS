package za.co.kapsch.iticket.Models;

/**
 * Created by csenekal on 2017/03/09.
 */
public class EnatisVehicleResponse {

    public String Source;
    public NatisErrors[] Errors;
    public EnatisVehicle[] Vehicles;

    public String getSource() {
        return Source;
    }

    public NatisErrors[] getErrors() {
        return Errors;
    }

    public EnatisVehicle[] getVehicles() {
        return Vehicles;
    }
}
