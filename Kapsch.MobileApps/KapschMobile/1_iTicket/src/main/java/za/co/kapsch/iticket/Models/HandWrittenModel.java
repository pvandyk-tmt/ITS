package za.co.kapsch.iticket.Models;

import com.google.gson.annotations.SerializedName;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

import java.util.Calendar;
import java.util.Date;

import za.co.kapsch.iticket.BuildConfig;
import za.co.kapsch.iticket.Enums.AddressType;
import za.co.kapsch.iticket.Enums.IdentificationType;
import za.co.kapsch.shared.Utilities;

/**
 * Created by CSenekal on 2017/07/26.
 */
@DatabaseTable(tableName = "HandWritten")
public class HandWrittenModel {

    @DatabaseField(columnName = "ID", generatedId = true)
    private transient Long mID;
    @DatabaseField(columnName = "Uploaded")
    private transient boolean mUploaded;
    @DatabaseField(columnName = "Completed")
    private transient boolean mCompleted;
    @DatabaseField(columnName = "UploadDateTime")
    private transient Date mUploadDateTime;
    @DatabaseField(columnName = "SavedDateTime")
    private transient Date mSavedDateTime;
    @DatabaseField(columnName = "Printed")
    private transient boolean mPrinted;
    @DatabaseField(columnName = "OffenceSetID")
    private transient long mOffenceSetID;
    @DatabaseField(columnName = "OfficerName")
    private transient String mOfficerName;

    @SerializedName("TicketNumber")
    @DatabaseField(columnName = "TicketNumber")
    private String mTicketNumber;

    @SerializedName("PersonInfoID")
    @DatabaseField(columnName = "PersonInfoID")
    private Long mPersonInfoID;

    @SerializedName("Title")
    @DatabaseField(columnName = "Title")
    private String mTitle;

    @SerializedName("FirstName")
    @DatabaseField(columnName = "FirstName")
    private String mFirstName;

    @SerializedName("MiddleNames")
    @DatabaseField(columnName = "MiddleNames")
    private String mMiddleNames;

    @SerializedName("Surname")
    @DatabaseField(columnName = "Surname")
    private String mSurname;

    @SerializedName("Initials")
    @DatabaseField(columnName = "Initials")
    private String mInitials;

    @SerializedName("IdentificationNumber")
    @DatabaseField(columnName = "IdentificationNumber")
    private String mIdentificationNumber;

    @SerializedName("IdentificationTypeID")
    @DatabaseField(columnName = "IdentificationTypeID")
    private Long mIdentificationTypeID;

    @SerializedName("IdentificationCountryID")
    @DatabaseField(columnName = "IdentificationCountryID")
    private int mIdentificationCountryID;

    @SerializedName("CitizenTypeID")
    @DatabaseField(columnName = "CitizenTypeID")
    private Long mCitizenTypeID;

    @SerializedName("Gender")
    @DatabaseField(columnName = "Gender")
    private String mGender;

    @SerializedName("Age")
    @DatabaseField(columnName = "Age")
    private Integer mAge;

    @SerializedName("BirthDateTime")
    @DatabaseField(columnName = "BirthDate")
    private Date mBirthDate;

    @SerializedName("Occupation")
    @DatabaseField(columnName = "Occupation")
    private String mOccupation;

    @SerializedName("Telephone")
    @DatabaseField(columnName = "Telephone")
    private String mTelephone;

    @SerializedName("MobileNumber")
    @DatabaseField(columnName = "MobileNumber")
    private String mMobileNumber;

    @SerializedName("Fax")
    @DatabaseField(columnName = "Fax")
    private String mFax;

    @SerializedName("Email")
    @DatabaseField(columnName = "Email")
    private String mEmail;

    @SerializedName("Company")
    @DatabaseField(columnName = "Company")
    private String mCompany;

    @SerializedName("BusinessTelephone")
    @DatabaseField(columnName = "BusinessTelephone")
    private String mBusinessTelephone;

    @SerializedName("PhysicalAddressInfoID")
    @DatabaseField(columnName = "PhysicalAddressInfoID")
    private Long mPhysicalAddressInfoID;

    @SerializedName("PhysicalAddressTypeID")
    @DatabaseField(columnName = "PhysicalAddressTypeID")
    private Long mPhysicalAddressTypeID;

    @SerializedName("PhysicalStreet1")
    @DatabaseField(columnName = "PhysicalStreet1")
    private String mPhysicalStreet1;

    @SerializedName("PhysicalStreet2")
    @DatabaseField(columnName = "PhysicalStreet2")
    private String mPhysicalStreet2;

    @SerializedName("PhysicalSuburb")
    @DatabaseField(columnName = "PhysicalSuburb")
    private String mPhysicalSuburb;

    @SerializedName("PhysicalTown")
    @DatabaseField(columnName = "PhysicalTown")
    private String mPhysicalTown;

    @SerializedName("PhysicalCode")
    @DatabaseField(columnName = "PhysicalCode")
    private String mPhysicalCode;

    @SerializedName("PostalAddressInfoID")
    @DatabaseField(columnName = "PostalAddressInfoID")
    private Long mPostalAddressInfoID;

    @SerializedName("PostalAddressTypeID")
    @DatabaseField(columnName = "PostalAddressTypeID")
    private Long mPostalAddressTypeID;

    @SerializedName("PostalPoBox")
    @DatabaseField(columnName = "PostalPoBox")
    private String mPostalPoBox;

    @SerializedName("PostalStreet")
    @DatabaseField(columnName = "PostalStreet")
    private String mPostalStreet;

    @SerializedName("PostalSuburb")
    @DatabaseField(columnName = "PostalSuburb")
    private String mPostalSuburb;

    @SerializedName("PostalTown")
    @DatabaseField(columnName = "PostalTown")
    private String mPostalTown;

    @SerializedName("PostalCode")
    @DatabaseField(columnName = "PostalCode")
    private String mPostalCode;

    @SerializedName("OffenceLocationStreet")
    @DatabaseField(columnName = "OffenceLocationStreet")
    private String mOffenceLocationStreet;

    @SerializedName("OffenceLocationSuburb")
    @DatabaseField(columnName = "OffenceLocationSuburb")
    private String mOffenceLocationSuburb;

    @SerializedName("OffenceLocationTown")
    @DatabaseField(columnName = "OffenceLocationTown")
    private String mOffenceLocationTown;

    @SerializedName("OffenceLocationLatitude")
    @DatabaseField(columnName = "OffenceLocationLatitude")
    private double mOffenceLocationLatitude;

    @SerializedName("OffenceLocationLongitude")
    @DatabaseField(columnName = "OffenceLocationLongitude")
    private double mOffenceLocationLongitude;

    @SerializedName("VehicleRegistrationMain")
    @DatabaseField(columnName = "VehicleRegistrationMain")
    private String mVehicleRegistrationMain;

    @SerializedName("VehicleRegistrationNo2")
    @DatabaseField(columnName = "VehicleRegistrationNo2")
    private String mVehicleRegistrationNo2;

    @SerializedName("VehicleRegistrationNo3")
    @DatabaseField(columnName = "VehicleRegistrationNo3")
    private String mVehicleRegistrationNo3;

    @SerializedName("VehicleMakeMain")
    @DatabaseField(columnName = "VehicleMakeMain")
    private String mVehicleMakeMain;

    @SerializedName("VehicleModelMain")
    @DatabaseField(columnName = "VehicleModelMain")
    private String mVehicleModelMain;

    @SerializedName("VehicleTypeMain")
    @DatabaseField(columnName = "VehicleTypeMain")
    private String mVehicleTypeMain;

    @SerializedName("VehicleLicenceExpiryDateTime")
    @DatabaseField(columnName = "VehicleLicenceExpiryDate")
    private String mVehicleLicenceExpiryDate;

    @SerializedName("VehicleColour")
    @DatabaseField(columnName = "VehicleColour")
    private String mVehicleColour;

    @SerializedName("VehicleRegisterNumber")
    @DatabaseField(columnName = "VehicleRegisterNumber")
    private String mVehicleRegisterNumber;

    @SerializedName("VehicleEngineNumber")
    @DatabaseField(columnName = "VehicleEngineNumber")
    private String mVehicleEngineNumber;

    @SerializedName("VehicleChassisNumber")
    @DatabaseField(columnName = "VehicleChassisNumber")
    private String mVehicleChassisNumber;

    @SerializedName("Gaurdian")
    @DatabaseField(columnName = "Gaurdian")
    private String mGaurdian;

    @SerializedName("Direction")
    @DatabaseField(columnName = "Direction")
    private String mDirection;

    @SerializedName("MeterNumber")
    @DatabaseField(columnName = "MeterNumber")
    private String mMeterNumber;

    @SerializedName("CaseNumber")
    @DatabaseField(columnName = "CaseNumber")
    private String mCaseNumber;

    @SerializedName("CcNumber")
    @DatabaseField(columnName = "CcNumber")
    private String mCcNumber;

    @SerializedName("ChargeCode1")
    @DatabaseField(columnName = "ChargeCode1")
    private String mChargeCode1;

    @SerializedName("ChargeCode1ID")
    @DatabaseField(columnName = "ChargeCode1ID")
    private Long mChargeCode1ID;

    @SerializedName("ChargeDescription1")
    @DatabaseField(columnName = "ChargeDescription1")
    private String mChargeDescription1;

    @SerializedName("Amount1")
    @DatabaseField(columnName = "Amount1")
    private Double mAmount1;

    @SerializedName("ChargeCode2")
    @DatabaseField(columnName = "ChargeCode2")
    private String mChargeCode2;

    @SerializedName("ChargeCode2ID")
    @DatabaseField(columnName = "ChargeCode2ID")
    private Long mChargeCode2ID;

    @SerializedName("ChargeDescription2")
    @DatabaseField(columnName = "ChargeDescription2")
    private String mChargeDescription2;

    @SerializedName("Amount2")
    @DatabaseField(columnName = "Amount2")
    private Double mAmount2;

    @SerializedName("ChargeCode3")
    @DatabaseField(columnName = "ChargeCode3")
    private String mChargeCode3;

    @SerializedName("ChargeCode3ID")
    @DatabaseField(columnName = "ChargeCode3ID")
    private Long mChargeCode3ID;

    @SerializedName("ChargeDescription3")
    @DatabaseField(columnName = "ChargeDescription3")
    private String mChargeDescription3;

    @SerializedName("Amount3")
    @DatabaseField(columnName = "Amount3")
    private Double mAmount3;

    @SerializedName("HasAlternativeCharge")
    @DatabaseField(columnName = "HasAlternativeCharge")
    private Integer mHasAlternativeCharge;

    @SerializedName("OffenceDateTime")
    @DatabaseField(columnName = "OffenceDate")
    private Date mOffenceDate;

    @SerializedName("IssueDateTime")
    @DatabaseField(columnName = "IssueDate")
    private Date mIssueDate;

    @SerializedName("CourtDateTime")
    @DatabaseField(columnName = "CourtDate")
    private Date mCourtDate;

    @SerializedName("CourtName")
    @DatabaseField(columnName = "CourtName")
    private String mCourtName;

    @SerializedName("CourtRoom")
    @DatabaseField(columnName = "CourtRoom")
    private String mCourtRoom;

    @SerializedName("DistrictName")
    @DatabaseField(columnName = "DistrictName")
    private String mDistrictName;

    @SerializedName("PaymentPlace")
    @DatabaseField(columnName = "PaymentPlace")
    private String mPaymentPlace;

    @SerializedName("PaymentDateTime")
    @DatabaseField(columnName = "PaymentDate")
    private Date mPaymentDate;

    @SerializedName("OfficerCredentialID")
    @DatabaseField(columnName = "OfficerCredentialID")
    private Long mOfficerCredentialID;

    @SerializedName("CapturedDateTime")
    @DatabaseField(columnName = "CapturedDate")
    private Date mCapturedDate;

    @SerializedName("CapturedCredentialID")
    @DatabaseField(columnName = "CapturedCredentialID")
    private Long mCapturedCredentialID;

    @SerializedName("LicenceCode")
    @DatabaseField(columnName = "LicenceCode")
    private String mLicenceCode;

    @SerializedName("LicenceType")
    @DatabaseField(columnName = "LicenceType")
    private String mLicenceType;

    @SerializedName("DriverLicenceCertificateNo")
    @DatabaseField(columnName = "DriverLicenceCertificateNo")
    private String mDriverLicenceCertificateNo;

    @SerializedName("ModifiedDateTime")
    @DatabaseField(columnName = "ModifiedDate")
    private Date mModifiedDate;

    @SerializedName("ModifiedCredentialID")
    @DatabaseField(columnName = "ModifiedCredentialID")
    private Long mModifiedCredentialID;

    @SerializedName("Speed")
    @DatabaseField(columnName = "Speed")
    private Double mSpeed;

    @SerializedName("MassPercentage")
    @DatabaseField(columnName = "MassPercentage")
    private String mMassPercentage;

    @SerializedName("IsCancelled")
    @DatabaseField(columnName = "IsCancelled")
    private boolean mIsCancelled;

    @SerializedName("CancelReason")
    @DatabaseField(columnName = "CancelledReason")
    private String mCancelReason;

    @SerializedName("SendToCourtRole")
    @DatabaseField(columnName = "SendToCourtRole")
    private String mSendToCourtRole;

    @SerializedName("Notes")
    @DatabaseField(columnName = "Notes")
    private String mNotes;

    @SerializedName("ApplicationAndVersion")
    @DatabaseField(columnName = "ApplicationAndVersion")
    private String mApplicationAndVersion;

    @SerializedName("DeviceID")
    @DatabaseField(columnName = "DeviceID")
    private String mDeviceID;

    @SerializedName("CameraID")
    @DatabaseField(columnName = "CameraID")
    private String mCameraID;

    @SerializedName("EventID")
    @DatabaseField(columnName = "EventID")
    private String mEventID;

    @SerializedName("InfringementLocationCode")
    @DatabaseField(columnName = "InfringementLocationCode")
    public String mInfringementLocationCode;

    @SerializedName("ExternalToken")
    @DatabaseField(columnName = "ExternalToken")
    public String mExternalToken;

    @SerializedName("ExternalReference")
    @DatabaseField(columnName = "ExternalTokenReference")
    public String mExternalTokenReference;

    public long getID() {
        return mID;
    }

    public void setID(long ID) {
        mID = ID;
    }

    public boolean isUploaded() {
        return mUploaded;
    }

    public void setUploaded(boolean uploaded) {
        mUploaded = uploaded;
    }

    public boolean isCompleted() {
        return mCompleted;
    }

    public void setCompleted(boolean completed) {
        mCompleted = completed;
    }

    public Date getUploadDateTime() {
        return mUploadDateTime;
    }

    public void setUploadDateTime(Date uploadDateTime) {
        mUploadDateTime = uploadDateTime;
    }

    public Date getSavedDateTime() {
        return mSavedDateTime;
    }

    public void setSavedDateTime(Date savedDateTime) {
        mSavedDateTime = savedDateTime;
    }

    public boolean isPrinted() {
        return mPrinted;
    }

    public void setPrinted(boolean printed) {
        mPrinted = printed;
    }

    public void setOfficerName(String officerName) { mOfficerName = officerName; }

    public String getOfficerName() { return mOfficerName; }

    public long getOffenceSetID(){ return mOffenceSetID; }

    public String getTicketNumber() {
        return mTicketNumber;
    }

    public void setTicketNumber(String ticketNumber) {
        mTicketNumber = ticketNumber;
    }

    public Long getPersonInfoID() {
        return mPersonInfoID;
    }

    public void setPersonInfoID(Long personInfoID) {
        mPersonInfoID = personInfoID;
    }

    public String getTitle() {
        return mTitle;
    }

    public void setTitle(String title) {
        mTitle = title;
    }

    public String getFirstName() {
        return mFirstName;
    }

    public void setFirstName(String firstName) {
        mFirstName = firstName;
    }

    public String getMiddleNames() {
        return mMiddleNames;
    }

    public void setMiddleNames(String middleNames) {
        mMiddleNames = middleNames;
    }

    public String getSurname() {
        return mSurname;
    }

    public void setSurname(String surname) {
        mSurname = surname;
    }

    public String getInitials() {
        return mInitials;
    }

    public void setInitials(String initials) {
        mInitials = initials;
    }

    public String getIdentificationNumber() {
        return mIdentificationNumber;
    }

    public void setIdentificationNumber(String identificationNumber) {
        mIdentificationNumber = identificationNumber;
    }

    public Long getIdentificationTypeId() {
        return mIdentificationTypeID;
    }

    public void setIdentificationTypeId(Long identificationTypeID) {
        mIdentificationTypeID = identificationTypeID;
    }

    public int getIdentificationCountryID() {
        return mIdentificationCountryID;
    }

    public void setIdentificationCountryID(int identificationCountryID) {
        mIdentificationCountryID = identificationCountryID;
    }

    public Long getCitizenTypeID() {
        return mCitizenTypeID;
    }

    public void setCitizenTypeID(Long citizenTypeID) {
        mCitizenTypeID = citizenTypeID;
    }

    public String getGender() {
        return mGender;
    }

    public void setGender(String gender) {
        mGender = gender;
    }

    public Integer getAge() {
        return mAge;
    }

    public void setAge(Integer age) {
        mAge = age;
    }

    public Date getBirthDate() {
        return mBirthDate;
    }

    public void setBirthDate(Date birthDate) {
        mBirthDate = birthDate;
    }

    public String getOccupation() {
        return mOccupation;
    }

    public void setOccupation(String occupation) {
        mOccupation = occupation;
    }

    public String getTelephone() {
        return mTelephone;
    }

    public void setTelephone(String telephone) {
        mTelephone = telephone;
    }

    public String getMobileNumber() {
        return mMobileNumber;
    }

    public void setMobileNumber(String mobileNumber) {
        mMobileNumber = mobileNumber;
    }

    public String getFax() {
        return mFax;
    }

    public void setFax(String fax) {
        mFax = fax;
    }

    public String getEmail() {
        return mEmail;
    }

    public void setEmail(String email) {
        mEmail = email;
    }

    public String getCompany() {
        return mCompany;
    }

    public void setCompany(String company) {
        mCompany = company;
    }

    public String getBusinessTelephone() {
        return mBusinessTelephone;
    }

    public void setBusinessTelephone(String businessTelephone) {
        mBusinessTelephone = businessTelephone;
    }

    public Long getPhysicalAddressInfoID() {
        return mPhysicalAddressInfoID;
    }

    public void setPhysicalAddressInfoID(Long physicalAddressInfoID) {
        mPhysicalAddressInfoID = physicalAddressInfoID;
    }

    public Long getPhysicalAddressTypeID() {
        return mPhysicalAddressTypeID;
    }

    public void setPhysicalAddressTypeID(Long physicalAddressTypeID) {
        mPhysicalAddressTypeID = physicalAddressTypeID;
    }

    public String getPhysicalStreet1() {
        return mPhysicalStreet1;
    }

    public void setPhysicalStreet1(String physicalStreet1) {
        mPhysicalStreet1 = physicalStreet1;
    }

    public String getPhysicalStreet2() {
        return mPhysicalStreet2;
    }

    public void setPhysicalStreet2(String physicalStreet2) {
        mPhysicalStreet2 = physicalStreet2;
    }

    public String getPhysicalSuburb() {
        return mPhysicalSuburb;
    }

    public void setPhysicalSuburb(String physicalSuburb) {
        mPhysicalSuburb = physicalSuburb;
    }

    public String getPhysicalTown() {
        return mPhysicalTown;
    }

    public void setPhysicalTown(String physicalTown) {
        mPhysicalTown = physicalTown;
    }

    public String getPhysicalCode() {
        return mPhysicalCode;
    }

    public void setPhysicalCode(String physicalCode) {
        mPhysicalCode = physicalCode;
    }

    public Long getPostalAddressID() {
        return mPostalAddressInfoID;
    }

    public void setPostalAddressID(Long postalAddressInfoID) {
        mPostalAddressInfoID = postalAddressInfoID;
    }

    public Long getPostalAddressTypeID() {
        return mPostalAddressTypeID;
    }

    public void setPostallAddressTypeID(Long postalAddressTypeID) {
        mPostalAddressTypeID = postalAddressTypeID;
    }

    public String getPostalPoBox() {
        return mPostalPoBox;
    }

    public void setPostalPoBox(String postalPoBox) {
        mPostalPoBox = postalPoBox;
    }

    public String getPostalStreet() {
        return mPostalStreet;
    }

    public void setPostalStreet(String postalStreet) {
        mPostalStreet = postalStreet;
    }

    public String getPostalSuburb() {
        return mPostalSuburb;
    }

    public void setPostalSuburb(String postalSuburb) {
        mPostalSuburb = postalSuburb;
    }

    public String getPostalTown() {
        return mPostalTown;
    }

    public void setPostalTown(String postalTown) {
        mPostalTown = postalTown;
    }

    public String getPostalCode() {
        return mPostalCode;
    }

    public void setPostalCode(String postalCode) {
        mPostalCode = postalCode;
    }

    public String getOffenceLocationStreet() {
        return mOffenceLocationStreet;
    }

    public void setOffenceLocationStreet(String offenceLocationStreet) {
        mOffenceLocationStreet = offenceLocationStreet;
    }

    public String getOffenceLocationSuburb() {
        return mOffenceLocationSuburb;
    }

    public void setOffenceLocationSuburb(String offenceLocationSuburb) {
        mOffenceLocationSuburb = offenceLocationSuburb;
    }

    public String getOffenceLocationTown() {
        return mOffenceLocationTown;
    }

    public void setOffenceLocationTown(String offenceLocationTown) {
        mOffenceLocationTown = offenceLocationTown;
    }

    public double getOffenceLocationLatitude() {
        return mOffenceLocationLatitude;
    }

    public void setOffenceLocationLatitude(double offenceLocationLatitude) {
        mOffenceLocationLatitude = offenceLocationLatitude;
    }

    public double getOffenceLocationLongitude() {
        return mOffenceLocationLongitude;
    }

    public void setOffenceLocationLongitude(double offenceLocationLongitude) {
        mOffenceLocationLongitude = offenceLocationLongitude;
    }

    public String getVehicleRegistrationMain() {
        return mVehicleRegistrationMain;
    }

    public void setVehicleRegistrationMain(String vehicleRegistrationMain) {
        mVehicleRegistrationMain = vehicleRegistrationMain;
    }

    public String getVehicleRegistrationNo2() {
        return mVehicleRegistrationNo2;
    }

    public void setVehicleRegistrationNo2(String vehicleRegistrationNo2) {
        mVehicleRegistrationNo2 = vehicleRegistrationNo2;
    }

    public String getVehicleRegistrationNo3() {
        return mVehicleRegistrationNo3;
    }

    public void setVehicleRegistrationNo3(String vehicleRegistrationNo3) {
        mVehicleRegistrationNo3 = vehicleRegistrationNo3;
    }

    public String getVehicleMakeMain() {
        return mVehicleMakeMain;
    }

    public void setVehicleMakeMain(String vehicleMakeMain) {
        mVehicleMakeMain = vehicleMakeMain;
    }

    public String getVehicleModelMain() {
        return mVehicleModelMain;
    }

    public void setVehicleModelMain(String vehicleModelMain) {
        mVehicleModelMain = vehicleModelMain;
    }

    public String getVehicleTypeMain() {
        return mVehicleTypeMain;
    }

    public void setVehicleTypeMain(String vehicleTypeMain) {
        mVehicleTypeMain = vehicleTypeMain;
    }

    public String getVehicleLicenceExpiryDate() {
        return mVehicleLicenceExpiryDate;
    }

    public void setVehicleLicenceExpiryDate(String vehicleLicenceExpiryDate) {
        mVehicleLicenceExpiryDate = vehicleLicenceExpiryDate;
    }

    public String getVehicleColour() {
        return mVehicleColour;
    }

    public void setVehicleColour(String vehicleColour) {
        mVehicleColour = vehicleColour;
    }

    public String getVehicleRegisterNumber() {
        return mVehicleRegisterNumber;
    }

    public void setVehicleRegisterNumber(String vehicleRegisterNumber) {
        mVehicleRegisterNumber = vehicleRegisterNumber;
    }

    public String getVehicleEngineNumber() {
        return mVehicleEngineNumber;
    }

    public void setVehicleEngineNumber(String vehicleEngineNumber) {
        mVehicleEngineNumber = vehicleEngineNumber;
    }

    public String getVehicleChassisNumber() {
        return mVehicleChassisNumber;
    }

    public void setVehicleChassisNumber(String vehicleChassisNumber) {
        mVehicleChassisNumber = vehicleChassisNumber;
    }

    public String getGaurdian() {
        return mGaurdian;
    }

    public void setGaurdian(String gaurdian) {
        mGaurdian = gaurdian;
    }

    public String getDirection() {
        return mDirection;
    }

    public void setDirection(String direction) {
        mDirection = direction;
    }

    public String getMeterNumber() {
        return mMeterNumber;
    }

    public void setMeterNumber(String meterNumber) {
        mMeterNumber = meterNumber;
    }

    public String getCaseNumber() {
        return mCaseNumber;
    }

    public void setCaseNumber(String caseNumber) {
        mCaseNumber = caseNumber;
    }

    public String getCcNumber() {
        return mCcNumber;
    }

    public void setCcNumber(String ccNumber) {
        mCcNumber = ccNumber;
    }

    public String getChargeCode1() {
        return mChargeCode1;
    }

    public String getChargeDescription1() { return mChargeDescription1; }

    public void setChargeCode1(String chargeCode1) {
        mChargeCode1 = chargeCode1;
    }

    public Long getChargeCode1ID() {
        return mChargeCode1ID;
    }

    public void setChargeCode1ID(Long chargeCode1ID) {
        mChargeCode1ID = chargeCode1ID;
    }

    public Double getAmount1() {
        return mAmount1;
    }

    public void setAmount1(Double amount1) {
        mAmount1 = amount1;
    }

    public String getChargeCode2() {
        return mChargeCode2;
    }

    public String getChargeDescription2() { return mChargeDescription2; }

    public void setChargeCode2(String chargeCode2) {
        mChargeCode2 = chargeCode2;
    }

    public Long getChargeCode2ID() {
        return mChargeCode2ID;
    }

    public void setChargeCode2ID(Long chargeCode2ID) {
        mChargeCode2ID = chargeCode2ID;
    }

    public Double getAmount2() {
        return mAmount2;
    }

    public void setAmount2(Double amount2) {
        mAmount2 = amount2;
    }

    public String getChargeCode3() {
        return mChargeCode3;
    }

    public String getChargeDescription3() { return mChargeDescription3; }

    public void setChargeCode3(String chargeCode3) {
        mChargeCode3 = chargeCode3;
    }

    public Long getChargeCode3ID() {
        return mChargeCode3ID;
    }

    public void setChargeCode3ID(Long chargeCode3ID) {
        mChargeCode3ID = chargeCode3ID;
    }

    public Double getAmount3() {
        return mAmount3;
    }

    public void setAmount3(Double amount3) {
        mAmount3 = amount3;
    }

    public Integer getHasAlternativeCharge() {
        return mHasAlternativeCharge;
    }

    public void setHasAlternativeCharge(Integer hasAlternativeCharge) {
        mHasAlternativeCharge = hasAlternativeCharge;
    }

    public Date getOffenceDate() {
        return mOffenceDate;
    }

    public void setOffenceDate(Date offenceDate) {
        mOffenceDate = offenceDate;
    }

    public Date getIssueDate() {
        return mIssueDate;
    }

    public void setIssueDate(Date issueDate) {
        mIssueDate = issueDate;
    }

    public Date getCourtDate() {
        return mCourtDate;
    }

    public void setCourtDate(Date courtDate) {
        mCourtDate = courtDate;
    }

    public String getCourtName() {
        return mCourtName;
    }

    public void setCourtName(String courtName) {
        mCourtName = courtName;
    }

    public String getCourtRoom() {
        return mCourtRoom;
    }

    public void setCourtRoom(String courtRoom) {
        mCourtRoom = courtRoom;
    }

    public String getDistrictName() {
        return mDistrictName;
    }

    public void setDistrictName(String districtName) {
        mDistrictName = districtName;
    }

    public String getPaymentPlace() {
        return mPaymentPlace;
    }

    public void setPaymentPlace(String paymentPlace) {
        mPaymentPlace = paymentPlace;
    }

    public Date getPaymentDate() {
        return mPaymentDate;
    }

    public void setPaymentDate(Date paymentDate) {
        mPaymentDate = paymentDate;
    }

    public Long getOfficerCredentialID() {
        return mOfficerCredentialID;
    }

    public void setOfficerCredentialID(Long officerCredentialID) {
        mOfficerCredentialID = officerCredentialID;
    }

    public Date getCapturedDate() {
        return mCapturedDate;
    }

    public void setCapturedDate(Date capturedDate) {
        mCapturedDate = capturedDate;
    }

    public Long getCapturedCredentialID() {
        return mCapturedCredentialID;
    }

    public void setCapturedCredentialID(Long capturedCredentialID) {
        mCapturedCredentialID = capturedCredentialID;
    }

    public String getLicenceCode() {
        return mLicenceCode;
    }

    public void setLicenceCode(String licenceCode) {
        mLicenceCode = licenceCode;
    }

    public String getLicenceType() {
        return mLicenceType;
    }

    public void setLicenceType(String licenceType) {
        mLicenceType = licenceType;
    }

    public String getDriverLicenceCertificateNo() {
        return mDriverLicenceCertificateNo;
    }

    public void setDriverLicenceCertificateNo(String driverLicenceCertificateNo) {
        mDriverLicenceCertificateNo = driverLicenceCertificateNo;
    }

    public Date getModifiedDate() {
        return mModifiedDate;
    }

    public void setModifiedDate(Date modifiedDate) {
        mModifiedDate = modifiedDate;
    }

    public Long getModifiedCredentialID() {
        return mModifiedCredentialID;
    }

    public void setModifiedCredentialID(Long modifiedCredentialID) {
        mModifiedCredentialID = modifiedCredentialID;
    }

    public Double getSpeed() {
        return mSpeed;
    }

    public void setSpeed(Double speed) {
        mSpeed = speed;
    }

    public String getMassPercentage() {
        return mMassPercentage;
    }

    public void setMassPercentage(String massPercentage) {
        mMassPercentage = massPercentage;
    }

    public boolean isIsCancelled() {
        return mIsCancelled;
    }

    public void setIsCancelled(boolean isCancelled) {
        mIsCancelled = isCancelled;
    }

    public String getCancelReason() {
        return mCancelReason;
    }

    public void setCancelReason(String cancelReason) {
        mCancelReason = cancelReason;
    }

    public String getSendToCourtRole() {
        return mSendToCourtRole;
    }

    public void setSendToCourtRole(String sendToCourtRole) {
        mSendToCourtRole = sendToCourtRole;
    }

    public String getNotes() {
        return mNotes;
    }

    public void setNotes(String notes) {
        mNotes = notes;
    }

    public String getApplicationAndVersion() {
        return mApplicationAndVersion;
    }

    public void setApplicationAndVersion(String applicationAndVersion) {
        mApplicationAndVersion = applicationAndVersion;
    }

    public void setDeviceID(String machineID){
        mDeviceID = machineID;
    }

    public String getDeviceID() {
        return mDeviceID;
    }

    public void setCameraID(String cameraID){
        mCameraID = cameraID;
    }

    public String getCameraID() {
        return mCameraID;
    }

    public void seEventID(String eventID){
        mEventID = eventID;
    }

    public String getEventID() {
        return mEventID;
    }

    public void setInfringementLocationCode(String infringementLocationCode)
    {
        mInfringementLocationCode = infringementLocationCode;
    }

    public String getInfringementLocationCode(){
        return mInfringementLocationCode;
    }

    public void setExternalToken(String externalToken){
        mExternalToken = externalToken;
    }

    public String getExternalToken() {
        return mExternalToken;
    }

    public void setExternalTokenReference(String externalTokenReference){
        mExternalTokenReference = externalTokenReference;
    }

    public String getExternalTokenReference() {
        return mExternalTokenReference;
    }


    public String setHandWritten(TicketModel ticket, String deviceId, boolean cancelled, String cancelReason){

        try {
            mIsCancelled = cancelled;
            mCancelReason = cancelReason;
            mSavedDateTime = Calendar.getInstance().getTime();
            mPrinted = false;

            if (ticket.isLocallyGeneratedTicket() == true) {
                mOffenceSetID = (ticket.getInfringement().getInfringementCharges()[0] == null) ? 0 : ticket.getInfringement().getInfringementCharges()[0].getOffenceSet();
            }
            mTicketNumber = ticket.getInfringement().getTicketNumber();
            mExternalToken = ticket.getInfringement().getExternalToken();
            mExternalTokenReference = ticket.getInfringement().getExternalTokenReference();

            if (ticket.getOffender() != null) {
                mSurname = ticket.getOffender().getLastName();
                mFirstName = ticket.getOffender().getFirstName();
                mInitials = ticket.getOffender().getInitials();
                mIdentificationNumber = ticket.getOffender().getIdNumber();
                mGaurdian = ticket.getOffender().getGuardian();
                mGender = ticket.getOffender().getGender();
                mAge = ticket.getOffender().getAge();
                mBirthDate = ticket.getOffender().getDateOfBirth();
                mOccupation = ticket.getOffender().getOccupation();
                mIssueDate = ticket.getInfringement().getIssueDate();
                mMobileNumber = ticket.getOffender().getMobileNumber();
                mCompany = ticket.getOffender().getEmployer();
                mPhysicalAddressTypeID = (long)AddressType.toInteger(AddressType.Physical);
                mPhysicalStreet1 =  ticket.getOffender().getPhysicalStreet1();
                mPhysicalStreet2 = ticket.getOffender().getPhysicalStreet2();
                mPhysicalSuburb = ticket.getOffender().getPhysicalSuburb();
                mPhysicalTown = ticket.getOffender().getPhysicalTown();
                mPhysicalCode = ticket.getOffender().getPhysicalCode();
                mPostalAddressTypeID = (long)AddressType.toInteger(AddressType.Postal);
                mPostalPoBox = ticket.getOffender().getPostalPoBox();
                mPostalStreet = ticket.getOffender().getPostalStreet();
                mPostalSuburb = ticket.getOffender().getPostalSuburb();
                mPostalTown = ticket.getOffender().getPostalTown();
                mPostalCode = ticket.getOffender().getPostalCode();
            }

            if (ticket.getVehicle() != null) {
                mVehicleRegistrationMain = ticket.getVehicle().getLicenceNumber();
                mVehicleMakeMain = ticket.getVehicle().getMake();
                mVehicleModelMain = ticket.getVehicle().getModel();
                mVehicleTypeMain = ticket.getVehicle().getType();
                mVehicleLicenceExpiryDate = ticket.getVehicle().getExpireDate();
                mVehicleColour = ticket.getVehicle().getColour();
                mVehicleRegisterNumber = ticket.getVehicle().getRegisterNumber();
                mVehicleEngineNumber = ticket.getVehicle().getEngineNumber();
                mVehicleChassisNumber = ticket.getVehicle().getVehicleIdentificationNumber();
            }

            if (ticket.getInfringement() != null){

                mAmount1 = (ticket.getInfringement().getInfringementCharges()[0] == null) ? null :  ticket.getInfringement().getInfringementCharges()[0].getFineAmount();
                mChargeCode1 = (ticket.getInfringement().getInfringementCharges()[0] == null) ? null : ticket.getInfringement().getInfringementCharges()[0].getChargeCode();
                mChargeCode1ID = (ticket.getInfringement().getInfringementCharges()[0] == null) ? null : ticket.getInfringement().getInfringementCharges()[0].getId();
                mChargeDescription1 = (ticket.getInfringement().getInfringementCharges()[0] == null) ? null : ticket.getInfringement().getInfringementCharges()[0].getUserCapturedDescription();

                mAmount2 = (ticket.getInfringement().getInfringementCharges()[1] == null) ? null :  ticket.getInfringement().getInfringementCharges()[1].getFineAmount();
                mChargeCode2 = (ticket.getInfringement().getInfringementCharges()[1] == null) ? null : ticket.getInfringement().getInfringementCharges()[1].getChargeCode();
                mChargeCode2ID = (ticket.getInfringement().getInfringementCharges()[1] == null) ? null : ticket.getInfringement().getInfringementCharges()[1].getId();
                mChargeDescription2 = (ticket.getInfringement().getInfringementCharges()[1] == null) ? null : ticket.getInfringement().getInfringementCharges()[1].getUserCapturedDescription();

                mAmount3 = (ticket.getInfringement().getInfringementCharges()[2] == null) ? null :  ticket.getInfringement().getInfringementCharges()[2].getFineAmount();
                mChargeCode3 = (ticket.getInfringement().getInfringementCharges()[2] == null) ? null : ticket.getInfringement().getInfringementCharges()[2].getChargeCode();
                mChargeCode3ID = (ticket.getInfringement().getInfringementCharges()[2] == null) ? null : ticket.getInfringement().getInfringementCharges()[2].getId();
                mChargeDescription3 = (ticket.getInfringement().getInfringementCharges()[2] == null) ? null : ticket.getInfringement().getInfringementCharges()[2].getUserCapturedDescription();

                mHasAlternativeCharge = alternativeCharge(ticket.getInfringement());
                mNotes = ticket.getInfringement().getNotes();

                mPaymentDate = ticket.getInfringement().getPayDate();
                mOffenceDate = ticket.getInfringement().getOffenceDate();
                mCapturedDate = new Date();

                mOffenceLocationStreet = ticket.getInfringement().getLocationDescription();
                mOffenceLocationSuburb = ticket.getInfringement().getLocationSuburb();
                mOffenceLocationTown = ticket.getInfringement().getLocationTown();
            }

            if (ticket.isLocallyGeneratedTicket() == true) {
                mCourtDate = ticket.getCourtInfo().getCourtDate().getDate();
                mCourtName = ticket.getCourtInfo().getCourt().getName();
                mCourtRoom = ticket.getCourtInfo().getCourtRoom().getRoomNumber();
            }

            mOfficerCredentialID = ticket.getUser().getCredentialID();

            mOfficerName = String.format("%s|%s|%s",
                    ticket.getUser().getFirstName(),
                    ticket.getUser().getLastName(),
                    ticket.getUser().getInfrastructureNumber());

            if (ticket.getOffender() != null) {
                mIdentificationTypeID = ticket.getOffender().getIdType();
                mIdentificationCountryID = ticket.getOffender().getCountryId();
            }

            mSpeed = findSpeed(ticket.getInfringement());
            mDistrictName = ticket.getDistrict().getBranchName();

            mOffenceLocationLatitude = ticket.getInfringement().getLatitude();
            mOffenceLocationLongitude = ticket.getInfringement().getLongitude();

            mApplicationAndVersion = String.format("%s-v%s", BuildConfig.APPLICATION_ID, BuildConfig.VERSION_NAME);

            mDeviceID = deviceId;
            mCameraID = ticket.getInfringement().getCameraID();
            mEventID = ticket.getInfringement().getEventID();
            mInfringementLocationCode = ticket.getInfringement().getLocationCode();

            return null;

        }catch (Exception e){
            return Utilities.exceptionMessage(e, "HandWritten.setHandWritten(): ");
        }
    }

    private Integer alternativeCharge(InfringementModel infringement){

        InfringementChargeModel[] infringementChargeList = infringement.getInfringementCharges();
        if (infringementChargeList[2] != null){
            return (infringementChargeList[2].getIsAlternative() == true) ? 1 : 0;
        }
        return null;
    }

    private Double findSpeed(InfringementModel infringement)
    {
        if (infringement.getSpeed() != 0){
            return infringement.getSpeed();
        }

        if (infringement.getInfringementCharges() == null) return null;

        for (InfringementChargeModel infringementCharge: infringement.getInfringementCharges())
        {
            if (infringementCharge != null)
            {
                if (infringementCharge.getSpeed() > 0)
                {
                    return infringementCharge.getSpeed();
                }
            }
        }

        return null;
    }
}
