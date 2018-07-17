package za.co.kapsch.iticket;

import android.app.Activity;

import com.zebra.sdk.comm.ConnectionException;

import java.util.Calendar;
import java.util.Date;

import za.co.kapsch.iticket.Models.CourtDetailModel;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.iticket.Models.OffenderModel;
import za.co.kapsch.iticket.Models.InfringementChargeModel;
import za.co.kapsch.iticket.Models.InfringementModel;
import za.co.kapsch.iticket.Models.TicketModel;
import za.co.kapsch.iticket.Models.VehicleModel;
import za.co.kapsch.shared.Models.UserModel;

/**
 * Created by csenekal on 2016-10-04.
 */
public class TestBench {
    private Activity mActivity;

    public TestBench(Activity activity){
        mActivity = activity;
    }

    public void printTest(String btMacAddress) throws ConnectionException {

        TicketModel ticketModel = new TicketModel();

        ticketModel.setUser(new UserModel());
        ticketModel.getUser().setFirstName("JOHAN");
        ticketModel.getUser().setLastName("WENTZEL");
        ticketModel.getUser().setInfrastructureNumber("19517DY");

        UserModel user = getUser();
        ticketModel.getUser().setSignature(user.getSignature());

        ticketModel.setOffender(new OffenderModel());
        ticketModel.getOffender().setLastName("Tester");
        ticketModel.getOffender().setFirstName("Test");
        ticketModel.getOffender().setIdNumber("7104264321234");
        ticketModel.getOffender().setGender("Male");
        ticketModel.getOffender().setMobileNumber("987654321");
        //ticketModel.getOffender().setResidentialAddress("25 DataServiceAddressModel Line One asdfakjh jh jadh kjhakjh, DataServiceAddressModel Line Two, 7550 adsf sadf asdfsdaf asdfasdfasdf");
        //ticketModel.getOffender().setBusinessAddress("25 Business Line One, Business Line Two, 7550");

        ticketModel.setVehicle(new VehicleModel());
        ticketModel.getVehicle().setLicenceNumber("CY 123 456");
        ticketModel.getVehicle().setMake("Toyota");

        ticketModel.setInfringement(new InfringementModel());
        ticketModel.getInfringement().setTicketNumber("92/01613/760/004078");
        ticketModel.getInfringement().setOffenceDate(new Date());
        ticketModel.getInfringement().setLocationDescription("Speed street, Bellville, Cape Town");
        ticketModel.getInfringement().setPayDate(new Date());
        ticketModel.getInfringement().setIssueDate(new Date());

        InfringementChargeModel[] infringementCharges = ticketModel.getInfringement().getInfringementCharges();
        InfringementChargeModel infringementCharge1 = new InfringementChargeModel();
        infringementCharge1.setChargeCode("88600");
        infringementCharge1.setFineAmount(2500);
        infringementCharge1.setDescription("No operating license/permit");
        infringementCharge1.setRegulation("contravened Art./Sect. 50 Wet/Act 5/2009");
        infringementCharges[0] = infringementCharge1;

        InfringementChargeModel infringementCharge2 = new InfringementChargeModel();
        infringementCharge2.setChargeCode("89216");
        infringementCharge2.setFineAmount(1000);
        infringementCharge2.setDescription("Duplicate operating license");
        infringementCharge2.setRegulation("contravened Reg. 23(b) Wet/Act 5 van/of 2009");
        infringementCharges[1] = infringementCharge2;

        CourtDetailModel court = new CourtDetailModel();
        Calendar calendar = Calendar.getInstance();
        calendar.add(Calendar.MONTH, 15);
        court.setDate(calendar.getTime());
        court.setName("Stellenbosch Municiple court papagaai street");
        court.setRoom("D");
        ticketModel.setCourt(court);

        DistrictModel district = new DistrictModel();
        district.setBranchName("Stellenbosch");
        ticketModel.setDistrict(district);

//        PrintHandWrittenSlip printSection56Slip = new PrintHandWrittenSlip(btMacAddress, mActivity, this);
//        try {
//            printSection56Slip.print(ticketModel);
//        }catch (Exception e){
//
//        }

    }

    private UserModel getUser(){
        return null;
    }
}
