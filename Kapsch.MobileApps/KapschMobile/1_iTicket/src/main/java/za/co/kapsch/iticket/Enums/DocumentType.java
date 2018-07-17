package za.co.kapsch.iticket.Enums;

/**
 * Created by CSenekal on 2017/08/02.
 */
public enum DocumentType {

    RoadSideDriver(1),
    RoadSideNoDriver(2),
    ElectronicallyPosted(3);

    private int mNumValue;

    DocumentType(int numValue){
        mNumValue = numValue;
    }

    public int getNumValue(){
        return mNumValue;
    }

    public static String toString(DocumentType documentType) {

        switch(documentType) {
            case RoadSideDriver : return "RoadSideDriver";
            case RoadSideNoDriver : return "RoadSideNoDriver";
            case ElectronicallyPosted : return "ElectronicallyPosted";
        }

        return null;
    }

    public static DocumentType fromInteger(int x) {

        switch(x) {
            case 1 : return RoadSideDriver;
            case 2 : return RoadSideNoDriver;
            case 3 : return ElectronicallyPosted;
       }

       return null;
    }

    public static int toInteger(DocumentType documentType) {

        switch(documentType) {
            case RoadSideDriver: return 1;
            case RoadSideNoDriver: return 2;
            case ElectronicallyPosted : return 3;
        }

        return -1;
    }
}
