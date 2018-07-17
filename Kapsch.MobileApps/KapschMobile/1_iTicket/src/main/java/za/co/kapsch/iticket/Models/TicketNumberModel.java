package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.TicketStatus;

/**
 * Created by csenekal on 2016-09-19.
 */
@DatabaseTable(tableName = "TicketNumber")
public class TicketNumberModel {

    @SerializedName("ID")
    @DatabaseField(columnName = "ID", generatedId = true)
    private int mID;

    @SerializedName("NumberValue")
    @DatabaseField(columnName = "NumberValue")
    private String mNumberValue;

    @SerializedName("ExternalToken")
    @DatabaseField(columnName = "ExternalToken")
    private String mExternalToken;

    @SerializedName("ExternalReference")
    @DatabaseField(columnName = "ExternalTokenReference")
    public String mExternalTokenReference;

    @DatabaseField(columnName = "DocumentType")
    private DocumentType mDocumentType;

    @DatabaseField(columnName = "Status")
    private TicketStatus mStatus;

    public int getID() {
        return mID;
    }

    public void setID(int ID) {
        mID = ID;
    }

    public String getNumberValue() {
        return mNumberValue;
    }

    public void setNumberValue(String numberValue) {
        this.mNumberValue = numberValue;
    }

    public String getExternalToken() {
        return mExternalToken;
    }

    public void setExternalToken(String externalToken) {
        mExternalToken = externalToken;
    }

    public String getExternalTokenReference() {
        return mExternalTokenReference;
    }

    public void seExternalTokenReference(String externalTokenReference){
        mExternalTokenReference = externalTokenReference;
    }

    public DocumentType getDocumentType() {
        return mDocumentType;
    }

    public void setDocumentType(DocumentType documentType) {
        mDocumentType = documentType;
    }

    public TicketStatus getStatus() {
        return mStatus;
    }

    public void setStatus(TicketStatus status) {
        mStatus = status;
    }
}
