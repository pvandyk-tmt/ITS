package za.co.kapsch.iticket.Models;

/**
 * Created by csenekal on 2016-10-04.
 */
public class DataServiceAddressModel {

    private int Id;
    private String Town;
    private String Suburb;
    private String Street;
    private String PoBox;
    private String Code;
    private String Residual;
    private int Score;

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        Id = id;
    }

    public String getTown() {
        return Town;
    }

    public void setTown(String town) {
        Town = town;
    }

    public String getSuburb() {
        return Suburb;
    }

    public void setSuburb(String suburb) {
        Suburb = suburb;
    }

    public String getStreet() {
        return Street;
    }

    public void setStreet(String street) {
        Street = street;
    }

    public String getPoBox() {
        return PoBox;
    }

    public void setPoBox(String poBox) {
        PoBox = poBox;
    }

    public String getCode() {
        return Code;
    }

    public void setCode(String code) {
        Code = code;
    }

    public String getResidual() {
        return Residual;
    }

    public void setResidual(String residual) {
        Residual = residual;
    }

    public int getScore() {
        return Score;
    }

    public void setScore(int score) {
        Score = score;
    }
}
