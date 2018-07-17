package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2018/04/17.
 */

public enum ViolationCategory {

    Person(1),
    Vehicle(2);

    private int mNumValue;

    ViolationCategory(int numValue){
        mNumValue = numValue;
    }

    public static ViolationCategory fromInteger(int x) {

        switch(x) {
            case 1 : return Person;
            case 2 : return Vehicle;
        }

        return null;
    }

    public static int toInteger(ViolationCategory violationCategory) {

        switch(violationCategory) {
            case Person : return 1;
            case Vehicle : return 2;
        }
        return -1;
    }
}
