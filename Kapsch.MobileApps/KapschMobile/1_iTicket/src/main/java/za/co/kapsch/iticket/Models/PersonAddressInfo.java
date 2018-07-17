package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2016-10-04.
 */
public class PersonAddressInfo {

    @SerializedName("Person")
    private PersonModel PersonModel;
    @SerializedName("PostalAddress")
    private DataServiceAddressModel postalDataServiceAddressModel;
    @SerializedName("PhysicalAddress")
    private DataServiceAddressModel physicalDataServiceAddressModel;
    private String Message;

    public PersonModel getPersonModel() {
        return PersonModel;
    }

    public void setPersonModel(PersonModel personModel) {
        PersonModel = personModel;
    }

    public DataServiceAddressModel getPostalDataServiceAddressModel() {
        return postalDataServiceAddressModel;
    }

    public void setPostalDataServiceAddressModel(DataServiceAddressModel postalDataServiceAddressModel) {
        this.postalDataServiceAddressModel = postalDataServiceAddressModel;
    }

    public DataServiceAddressModel getPhysicalDataServiceAddressModel() {
        return physicalDataServiceAddressModel;
    }

    public void setPhysicalDataServiceAddressModel(DataServiceAddressModel physicalDataServiceAddressModel) {
        this.physicalDataServiceAddressModel = physicalDataServiceAddressModel;
    }

    public String getMessage() {
        return Message;
    }

    public void setMessage(String message) {
        Message = message;
    }
}
