package za.co.kapsch.iticket.Models;

import android.os.Parcel;
import android.os.Parcelable;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.Enums.TicketType;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.iticket.R;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.iticket.orm.TicketNumberRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.UserModel;

/**
 * Created by csenekal on 2016-07-20.
 */
public class TicketModel implements Parcelable {

    private boolean mLocallyGeneratedTicket;
    private DocumentType mDocumentType;
    private UserModel mUser;
    private DistrictModel mDistrict;
    private CourtDetailModel mCourt;
    private CourtInfoModel mCourtInfo;
    private OffenderModel mOffender;
    private VehicleModel mVehicle;
    private InfringementModel mInfringement;
    private List<EvidenceModel> mEvidenceList;
    private boolean mPersisted;
    private String mNatisMessages;
    private String mVosiMessages;
    private boolean mICamProsecution;

    public TicketModel(){}

    public boolean isLocallyGeneratedTicket(){
        return mLocallyGeneratedTicket;
    }

    public DocumentType getDocumentType() {
        return mDocumentType;
    }

    public void setDocumentType(DocumentType documentType) {
        mDocumentType = documentType;
    }

    public UserModel getUser() {  return mUser; }

    public void setUser(UserModel user){
        mUser = user;
    }

    public CourtDetailModel getCourt() {
        return mCourt;
    }

    public void setCourt(CourtDetailModel court) {
        this.mCourt = court;
    }

    public CourtInfoModel getCourtInfo() {
        return mCourtInfo;
    }

    public void setCourtInfo(CourtInfoModel courtInfo) {
        mCourtInfo = courtInfo;
    }

    public OffenderModel getOffender() {
        return mOffender;
    }

    public DistrictModel getDistrict() {
        return mDistrict;
    }

    public void setDistrict(DistrictModel district) {
        mDistrict = district;
    }

    public void setOffender(OffenderModel offender) { mOffender = offender; }

    public VehicleModel getVehicle() {
        return mVehicle;
    }

    public void setVehicle(VehicleModel vehicle) {
        mVehicle = vehicle;
    }

    public InfringementModel
    getInfringement() { return mInfringement; }

    public void setInfringement(InfringementModel infringement) { mInfringement = infringement; }

    public List<EvidenceModel> getEvidenceList() {
        return mEvidenceList;
    }

    public String getNatisMessages() {
        return mNatisMessages;
    }

    public void setNatisMessages(String natisMessages) {
        mNatisMessages = natisMessages;
    }

    public String getVosiMessages() {
        return mVosiMessages;
    }

    public void setVosiMessages(String vosiMessages) {
        mVosiMessages = vosiMessages;
    }

    public void setICamProsecution(boolean iCamProsecution){ mICamProsecution = iCamProsecution; }

    public boolean getICamProsecution(){ return mICamProsecution; }

    public static TicketModel getNewTicket(DocumentType documentType, UserModel user, CourtInfoModel courtInfo, DistrictModel district, boolean iCamProsecution) {

        try {

            TicketNumberModel ticketNumber = TicketNumberRepository.getNextTicketNumber(documentType);

            if (ticketNumber == null){
                MessageManager.showMessage(
                        String.format(App.getContext().getString(R.string.no_more_tickets_available),
                                DocumentType.toString(documentType)) , ErrorSeverity.None);
                return null;
            }

            String numberValue = ticketNumber.getNumberValue();
            String externalToken = ticketNumber.getExternalToken();
            String externalTokenReference = ticketNumber.getExternalTokenReference();

            TicketModel ticket = new TicketModel();
            ticket.mLocallyGeneratedTicket = true;

            InfringementModel infringement = new InfringementModel();
            infringement.setTicketNumber(numberValue);
            infringement.setExternalToken(externalToken);
            infringement.setExternalTokenReference(externalTokenReference);
            infringement.setOffenceDate(Utilities.getDateAddMinutes(ConfigItemModel.getInstance().getOffenceMinutesFromNow()));
            ticket.setInfringement(infringement);

            ticket.setUser(user);
            ticket.setCourtInfo(courtInfo);
            ticket.setDistrict(district);
            ticket.setDocumentType(documentType);
            ticket.setICamProsecution(iCamProsecution);
            if (iCamProsecution == true) {
                SessionModel.getInstance().setOffenceLocation(null);
            }

            return ticket;

        } catch (Exception e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketModel::getNewTicket()"), ErrorSeverity.Medium);
            return null;
        }
    }

    public void addEvidence(EvidenceModel evidence) {
        if (mEvidenceList == null){
            mEvidenceList = new ArrayList<>();
        }
        mEvidenceList.add(evidence);
    }

    public boolean getPersisted() { return mPersisted; }

    public void setPersisted(boolean persisted) { mPersisted = persisted; }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        try {
            out.writeValue(mDocumentType);
            out.writeParcelable(mUser, flags);
            out.writeParcelable(mDistrict, flags);
            out.writeParcelable(mCourt, flags);
            out.writeParcelable(mCourtInfo, flags);
            out.writeParcelable(mOffender, flags);
            out.writeParcelable(mVehicle, flags);
            out.writeParcelable(mInfringement, flags);

            if (mEvidenceList == null) {
                out.writeInt(0);
            } else {
                out.writeInt(mEvidenceList.size());
                for (EvidenceModel evidence : mEvidenceList) {
                    out.writeParcelable(evidence, flags);
                }
            }

            out.writeByte((byte) (mPersisted ? 1 : 0));
            out.writeString(mNatisMessages);
            out.writeString(mVosiMessages);
            out.writeByte((byte) (mICamProsecution ? 1 : 0));
            out.writeByte((byte) (mLocallyGeneratedTicket ? 1 : 0));

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketModel::writeToParcel()"), ErrorSeverity.High);
        }
    }

    public static final Parcelable.Creator<TicketModel> CREATOR = new Parcelable.Creator<TicketModel>() {

        public TicketModel createFromParcel(Parcel in) {
            return new TicketModel(in);
        }

        public TicketModel[] newArray(int size) {
            return new TicketModel[size];
        }
    };

    private TicketModel(Parcel in) {

        try {
            mDocumentType = (DocumentType)in.readValue(TicketType.class.getClassLoader());
            mUser = in.readParcelable(UserModel.class.getClassLoader());
            mDistrict = in.readParcelable(DistrictModel.class.getClassLoader());
            mCourt = in.readParcelable(CourtDetailModel.class.getClassLoader());
            mCourtInfo = in.readParcelable(CourtInfoModel.class.getClassLoader());
            mOffender = in.readParcelable(OffenderModel.class.getClassLoader());
            mVehicle = in.readParcelable(VehicleModel.class.getClassLoader());
            mInfringement = in.readParcelable(InfringementModel.class.getClassLoader());

            int evidenceCount = in.readInt();
            if (evidenceCount > 0) {
                mEvidenceList = new ArrayList<>();
                for (int i = 0; i < evidenceCount; i++) {
                    EvidenceModel evidenceModel =  in.readParcelable(EvidenceModel.class.getClassLoader());
                    mEvidenceList.add(evidenceModel);
                }
            }

            mPersisted = in.readByte() != 0;
            mNatisMessages = in.readString();
            mVosiMessages = in.readString();
            mICamProsecution = in.readByte() != 0;
            mLocallyGeneratedTicket = in.readByte() != 0;
        }
        catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "TicketModel::TicketModel(Parcel in)"), ErrorSeverity.High);
        }
    }
}
