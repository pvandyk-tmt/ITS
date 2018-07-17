package za.co.kapsch.console.Google;

import java.util.ArrayList;
import java.util.List;

import za.co.kapsch.console.Enums.AddressType;

/**
 * Created by CSenekal on 2017/02/10.
 */
public class GoogleAddressRefactor {

    //"Eversdal Rd, Cape Town, South Africa"
    //"First Floor, Building E, Plattekloof Office Park, Bloulelie Street, Plattekloof, 7500"
    //"First Floor, Building E, Plattekloof Office Park, Bloulelie Street, Plattekloof"
    private static final String CAPE_TOWN = "CAPE TOWN";
    private static final String SOUTH_AFRICA = "SOUTH AFRICA";
    private static final String[] STREET_REFERENCES = {"STREET", "ROAD", "RD", "WAY"};

    private String mDescription;
    private String mLineOne;
    private String mLineTwo;
    private String mSuburb;
    private String mTown;
    private String mCode;
    private String mCity;
    private String mCountry;

    List<String> mAddressList = new ArrayList<>();

    public String getCode() {
        return mCode;
    }

    public String getDescription() {
        return mDescription;
    }

    public String getSuburb() {
        return mSuburb;
    }

    public String getTown() {
        return mTown;
    }

    public String getLineOne() {
        return mLineOne;
    }

    public String getLineTwo() {
        return mLineTwo;
    }

    public String getCountry() {
        return mCountry;
    }

    public String getCity() {
        return mCity;
    }

    public void refactor(String formattedAddress, AddressType addressType) {

        String[] addressLines = formattedAddress.split(",");

        for (String addressLine : addressLines) {
                mAddressList.add(addressLine.trim());
        }

        setAddressCountry();
        setAddressCity();
        setAddressCode();

        if (mAddressList.size() == 1) {

            switch (addressType) {
                case Offence:
                    mDescription = mAddressList.get(0);
                    break;
                default:
                    mLineOne = mAddressList.get(0);
                    break;
            }
        } else if (mAddressList.size() == 2) {

            mSuburb = mAddressList.get(1);
            switch (addressType) {
                case Offence:
                    mDescription = mAddressList.get(0);
                    break;
                default:
                    mLineOne = mAddressList.get(0);
                    break;
            }
        } else if (mAddressList.size() == 3) {
            mSuburb = mAddressList.get(1);
            mTown = mAddressList.get(2);
            switch (addressType) {
                case Offence:
                    mDescription = mAddressList.get(0);
                    break;
                default:
                    mLineOne = mAddressList.get(0);
                    break;
            }
        } else if (mAddressList.size() == 4) {
            mSuburb = mAddressList.get(2);
            mTown = mAddressList.get(3);
            switch (addressType) {
                case Offence:
                    mDescription = String.format("%s, %s", mAddressList.get(0), mAddressList.get(1));
                    break;
                default:
                    mLineOne = mAddressList.get(0);
                    mLineTwo = mAddressList.get(1);
                    break;
            }
        } else if (mAddressList.size() == 5) {
            mSuburb = mAddressList.get(3);
            mTown = mAddressList.get(4);
            switch (addressType) {
                case Offence:
                    mDescription = String.format("%s, %s, %s", mAddressList.get(0), mAddressList.get(1), mAddressList.get(2));
                    break;
                default:
                    mLineOne = String.format("%s, %s", mAddressList.get(0), mAddressList.get(1));
                    mLineTwo = mAddressList.get(2);
                    break;
            }
        } else if (mAddressList.size() == 6) {
            mSuburb = mAddressList.get(4);
            mTown = mAddressList.get(5);
            switch (addressType) {
                case Offence:
                    mDescription = String.format("%s, %s, %s, %s", mAddressList.get(0), mAddressList.get(1), mAddressList.get(2), mAddressList.get(3));
                    break;
                default:
                    mLineOne = String.format("%s, %s, %s", mAddressList.get(0), mAddressList.get(1), mAddressList.get(2));
                    mLineTwo = mAddressList.get(3);
                    break;
            }
        }

        if (mTown == null){
            mTown = mCity;
        }
    }

    private void setAddressCountry(){
        int index = -1;
        for (int i = 0; i < mAddressList.size(); i++) {
            if (mAddressList.get(i).toUpperCase().contains(SOUTH_AFRICA)) {
                mCountry = mAddressList.get(i);
                index = i;
                break;
            }
        }
        if (index != -1) {
            mAddressList.remove(index);
        }
    }

    private void setAddressCity(){
        int index = -1;
        for (int i = 0; i < mAddressList.size(); i++) {
            if (mAddressList.get(i).toUpperCase().contains(CAPE_TOWN)) {
                mCity = mAddressList.get(i);
                index = i;
                break;
            }
        }
        if (index != -1) {
            mAddressList.remove(index);
        }
    }

    private void setAddressCode() {

        int index = -1;

        for (int i = 0; i < mAddressList.size(); i++) {
            try {
                Integer.parseInt(mAddressList.get(i));
                index = i;
            } catch (Exception e) {
                //ignore
            }
        }

        if (index != -1){
            mCode = mAddressList.get(index);
            mAddressList.remove(index);
        }
    }
}
