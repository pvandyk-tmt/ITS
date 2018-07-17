package za.co.kapsch.iticket.Models;


import android.graphics.Bitmap;
import android.os.Parcel;
import android.os.Parcelable;

import java.util.Calendar;
import java.util.Date;

import za.co.kapsch.iticket.Enums.IdentificationType;
import za.co.technovolve.dlcserializerrsa.DrivingLicenseCard;

/**
 * Created by csenekal on 2016-08-02.
 */
public class OffenderModel implements Parcelable {

    private String mInitials;
    private String mFirstName;
    private String mLastName;
    private String mGender;
    private String mMobileNumber;
    private Date mDateOfBirth;
    private String mIdNumber;
    private Long mIdTypeId;
    private int mCountryId;
    private String mEmployer;
    private String mOccupation;
    private String mEmail;

    private String mPhysicalStreet1;
    private String mPhysicalStreet2;
    private String mPhysicalSuburb;
    private String mPhysicalTown;
    private String mPhysicalCode;

    private String mPostalPoBox;
    private String mPostalStreet;
    private String mPostalSuburb;
    private String mPostalTown;
    private String mPostalCode;

    private String mGuardian;
    private String mPdpNumber;
    private byte[] mSignature;
    private Bitmap mPhoto;

    public String mCertificateNumber;
    public Date mValidFromDate;
    public Date mValidUntilDate;
    public String mDriverRestrictions;

    public VehicleClassModel[] mVehicleClasses;

    public OffenderModel(){}

    public String getInitials() {
        return mInitials;
    }

    public void setInitials(String mInitials) {
        this.mInitials = mInitials;
    }

    public String getFirstName() {
        return mFirstName;
    }

    public void setFirstName(String firstName) {
        mFirstName = firstName;
    }

    public String getLastName() {
        return mLastName;
    }

    public void setLastName(String lastName) {
        mLastName = lastName;
    }

    public String getGender() {
        return mGender;
    }

    public void setGender(String gender) {
        mGender = gender;
    }

    public String getMobileNumber() {
        return mMobileNumber;
    }

    public void setMobileNumber(String mobileNumber) {
        this.mMobileNumber = mobileNumber;
    }

    public Date getDateOfBirth() {
        return mDateOfBirth;
    }

    public void setDateOfBirth(Date dateOfBirth) {
        this.mDateOfBirth = dateOfBirth;
    }

    public String getIdNumber() {
        return mIdNumber;
    }

    public void setIdNumber(String idNumber) {
        this.mIdNumber = idNumber;
    }

//    public IdentificationType getIdType() {
//        return mIdTypeId;
//    }

    public Long getIdType() {
        return mIdTypeId;
    }

    public void setIdType(Long idTypeId) {
        mIdTypeId = idTypeId;
    }

    public int getCountryId() {
        return mCountryId;
    }

    public void setCountryId(int countryId) {
        mCountryId = countryId;
    }

    public String getEmployer() {
        return mEmployer;
    }

    public void setEmployer(String employer) {
        this.mEmployer = employer;
    }

    public String getOccupation() {
        return mOccupation;
    }

    public void setOccupation(String occupation) {
        this.mOccupation = occupation;
    }

    public String getEmail() {
        return mEmail;
    }

    public void setEmail(String email) {
        this.mEmail = email;
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

    public String getGuardian() {
        return mGuardian;
    }

    public void setGuardian(String guardian) {
        this.mGuardian = guardian;
    }

    public String getPdpNumber() {
        return mPdpNumber;
    }

    public void setPdpNumber(String pdpNumber) {
        this.mPdpNumber = pdpNumber;
    }

    public byte[] getSignature() {
        return mSignature;
    }

    public void setSignature(byte[] signature) {
        mSignature = signature;
    }

    public Bitmap getPhoto() {
        return mPhoto;
    }

    public void setPhoto(Bitmap photo) {
        this.mPhoto = photo;
    }

    public String getCertificateNumber() {
        return mCertificateNumber;
    }

    public void setCertificateNumber(String certificateNumber) {
        this.mCertificateNumber = certificateNumber;
    }

    public Date getValidFromDate() {
        return mValidFromDate;
    }

    public void setValidFromDate(Date validFromDate) {
        this.mValidFromDate = validFromDate;
    }

    public Date getValidUntilDate() {
        return mValidUntilDate;
    }

    public void setValidUntilDate(Date validUntilDate) {
        this.mValidUntilDate = validUntilDate;
    }

    public String getDriverRestrictions() {
        return mDriverRestrictions;
    }

    public void setDriverRestrictions(String driverRestrictions) {
        this.mDriverRestrictions = driverRestrictions;
    }

    public VehicleClassModel[] getVehicleClasses() {
        return mVehicleClasses;
    }

    public void setVehicleClasses(VehicleClassModel[] mVehicleClasses) {
        this.mVehicleClasses = mVehicleClasses;
    }

    @Override
    public int describeContents() {
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags) {
        out.writeString(mInitials);
        out.writeString(mFirstName);
        out.writeString(mLastName);
        out.writeString(mGender);
        out.writeString(mMobileNumber);
        out.writeLong(mDateOfBirth == null ? -1 : mDateOfBirth.getTime());
        out.writeString(mIdNumber);
        out.writeLong(mIdTypeId);
        out.writeInt(mCountryId);
        out.writeString(mEmployer);
        out.writeString(mOccupation);
        out.writeString(mEmail);
        out.writeString(mPhysicalStreet1);
        out.writeString(mPhysicalStreet2);
        out.writeString(mPhysicalSuburb);
        out.writeString(mPhysicalTown);
        out.writeString(mPhysicalCode);
        out.writeString(mPostalPoBox);
        out.writeString(mPostalStreet);
        out.writeString(mPostalSuburb);
        out.writeString(mPostalTown);
        out.writeString(mPostalCode);
        out.writeString(mGuardian);
        out.writeString(mPdpNumber);
        WriteByteArray(out, mSignature);
        out.writeParcelable(mPhoto, flags);

        out.writeString(mCertificateNumber);
        out.writeLong(mValidFromDate == null ? -1 : mValidFromDate.getTime());
        out.writeLong(mValidUntilDate == null ? -1 : mValidUntilDate.getTime());
        out.writeString(mDriverRestrictions);

        if (mVehicleClasses != null) {
            out.writeInt(mVehicleClasses.length);
            for (int i = 0; i < mVehicleClasses.length; i++) {
                out.writeParcelable(mVehicleClasses[i], flags);
            }
        }else {
            out.writeInt(0);
        }
    }

    public static final Parcelable.Creator<OffenderModel> CREATOR = new Parcelable.Creator<OffenderModel>() {
        public OffenderModel createFromParcel(Parcel in) {
            return new OffenderModel(in);
        }

        public OffenderModel[] newArray(int size) {
            return new OffenderModel[size];
        }
    };

    public int getAge(){
        try
        {
            if (mIdNumber.length() >= 6)
            {
                int year = Integer.parseInt(mIdNumber.substring(0, 2));
                int month = Integer.parseInt(mIdNumber.substring(2, 4));
                int day = Integer.parseInt(mIdNumber.substring(4, 6).replaceAll("^0*", ""));

                if (month > 12) return 0;
                if (day > 31) return 0;

                Date now = new Date();

                if (2000 + year > now.getYear())
                {
                    year += 1900;
                }
                else
                {
                    year += 2000;
                }

                Calendar calendar = Calendar.getInstance();
                calendar.set(year, month, day, 0, 0);
                Date dob = calendar.getTime();

                Date temp = dob;
                int age = -1;

                while (temp.before(new Date())) {

                    calendar.setTime(temp);
                    calendar.add(Calendar.YEAR, 1);

                    temp = calendar.getTime();
                    age++;
                }
                return age;
            }
        }
        catch(Exception e)
        {
            return 0;
        }

        return 0;
    }

    private OffenderModel(Parcel in) {
        mInitials = in.readString();
        mFirstName = in.readString();
        mLastName = in.readString();
        mGender = in.readString();
        mMobileNumber = in.readString();
        long tmpDateOfBirth = in.readLong();
        mDateOfBirth = tmpDateOfBirth == -1 ? null : new Date(tmpDateOfBirth);
        mIdNumber = in.readString();
        mIdTypeId = in.readLong();
        mCountryId = in.readInt();
        mEmployer = in.readString();
        mOccupation = in.readString();
        mEmail = in.readString();
        mPhysicalStreet1 = in.readString();
        mPhysicalStreet2 = in.readString();
        mPhysicalSuburb = in.readString();
        mPhysicalTown = in.readString();
        mPhysicalCode = in.readString();
        mPostalPoBox = in.readString();
        mPostalStreet = in.readString();
        mPostalSuburb = in.readString();
        mPostalTown = in.readString();
        mPostalCode = in.readString();
        mGuardian = in.readString();
        mPdpNumber = in.readString();
        ReadByteArray(in, mSignature);
        mPhoto = in.readParcelable(Bitmap.class.getClassLoader());
        mCertificateNumber = in.readString();

        long tmpValidFromDate = in.readLong();
        mValidFromDate =  tmpValidFromDate == -1 ? null : new Date(tmpValidFromDate);

        long tmpValidUntilDate = in.readLong();
        mValidUntilDate =  tmpValidUntilDate == -1 ? null : new Date(tmpValidUntilDate);

        mDriverRestrictions = in.readString();

        int vehicleClassesCount = in.readInt();
        mVehicleClasses = new VehicleClassModel[vehicleClassesCount];
        for(int i = 0; i  < vehicleClassesCount; i++){
            mVehicleClasses[i] = in.readParcelable(VehicleClassModel.class.getClassLoader());
        }
    }

    private void WriteByteArray(Parcel out, byte[] value){
        if (value != null) {
            out.writeInt(value.length);
            if (value != null) out.writeByteArray(value);
        }
        else{
            out.writeInt(0);
        }
    }

    private void ReadByteArray(Parcel in, byte[] value) {
        int signatureLength = in.readInt();
        if (signatureLength > 0) {
            value = new byte[signatureLength];
            in.readByteArray(value);
        }
    }

    private String driverRestrictions(String[] restrictions){

        if (restrictions.length == 0) return "0";

        String value = restrictions[0];
        for (int i = 1; i < restrictions.length; i++){
            value += "," + restrictions[i];
        }

        return value;
    }
}