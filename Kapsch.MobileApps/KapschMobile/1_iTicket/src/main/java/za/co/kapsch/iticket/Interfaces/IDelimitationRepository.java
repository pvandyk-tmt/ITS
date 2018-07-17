package za.co.kapsch.iticket.Interfaces;

import java.sql.SQLException;

/**
 * Created by CSenekal on 2017/01/23.
 */
public interface IDelimitationRepository {

    <T> T getFirstLevel() throws SQLException;
    <T> T getSecondLevel(int firstLevelId) throws SQLException;
    <T> T getThirdLevel(int secondLevelId) throws SQLException;
    <T> T getFourthLevel(int thirdLevelId) throws SQLException;
    <T> T getFifthLevel(int fourthLevelId) throws SQLException;
}
