package za.co.kapsch.iticket.Models;

import java.util.List;

import za.co.kapsch.iticket.Enums.DocumentType;
import za.co.kapsch.iticket.orm.CourtsInfoRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.FineChargeModel;
import za.co.kapsch.shared.Models.FineModel;
import za.co.kapsch.shared.Models.UserModel;
import za.co.kapsch.shared.Utilities;

/**
 * Created by csenekal on 2018/06/06.
 */

public class FineToTicketModel {

    public static TicketModel getTicketModel(FineModel fine, DocumentType documentType, byte[] officerSignature, byte[] offenderSignature){

        try{

            TicketModel ticket = new TicketModel();
            ticket.setDocumentType(documentType);
            ticket.setOffender(getOffender(fine, offenderSignature));
            ticket.setVehicle(getVehicle(fine));
            ticket.setInfringement(getInfringement(fine));
            ticket.setUser(getUser(fine, officerSignature));
            ticket.setDistrict(getDistrict(fine));

            return ticket;

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "FineToTicketModel::getTicketModel()"), ErrorSeverity.Medium);
            return null;
        }
    }

    private static OffenderModel getOffender(FineModel fine, byte[] offenderSignature){

        OffenderModel offender = new OffenderModel();

        offender.setLastName(fine.getOffenderLastName());
        offender.setFirstName(fine.getOffenderFirstName());
        offender.setIdNumber(fine.getOffenderIDNumber());
        offender.setIdType(fine.getOffenderIDType());
        offender.setSignature(offenderSignature);
        offender.setPhysicalStreet1(fine.getOffenderAddressLine1());
        offender.setPhysicalStreet2(fine.getOffenderAddressLine2());
        offender.setPhysicalSuburb(fine.getOffenderAddressSuburb());
        offender.setPhysicalTown(fine.getOffenderAddressTown());
        offender.setMobileNumber(fine.getOffenderMobileNumber());

        return offender;
    }

    private static VehicleModel getVehicle(FineModel fine){

        VehicleModel vehicle = new VehicleModel();

        vehicle.setLicenceNumber(fine.getVLN());
        vehicle.setMake(fine.getVehicleMake());

        return vehicle;
    }

    private static UserModel getUser(FineModel fine, byte[] officerSignature){

        UserModel user = new UserModel();

        user.setFirstName(fine.getOfficerFirstName());
        user.setLastName(fine.getOfficerLastName());
        user.setInfrastructureNumber(fine.getExternalID());
        user.setSignature(officerSignature);

        return user;
    }

    private static DistrictModel getDistrict(FineModel fine){

        DistrictModel district = new DistrictModel();

        district.setBranchName(fine.getDistrictName());
        district.setPaymentOptions(fine.getPaymentOptions());

        return district;
    }

    private static InfringementModel getInfringement(FineModel fine){

        InfringementModel infringment = new InfringementModel();

        infringment.setTicketNumber(fine.getReferenceNumber());
        infringment.setExternalToken(fine.getTransactionToken());
        infringment.setOffenceDate(fine.getOffenceDate());

        String[] locationAddress = fine.getOffenceLocation().split("\\n", -1);
        infringment.setLocationDescription(locationAddress.length > 0 ? locationAddress[0] : null);
        infringment.setLocationSuburb(locationAddress.length > 1 ? locationAddress[1] : null);
        infringment.setLocationTown(locationAddress.length > 2 ? locationAddress[2] : null);

        infringment.setIssueDate(fine.getFirstPrintDate());

        int chargeIndex = 0;
        List<FineChargeModel> fineChargeList = fine.getFineChargeModels();
        for (FineChargeModel fineCharge: fineChargeList) {
            infringment.getInfringementCharges()[chargeIndex] = getInfringementCharge(fineCharge, fine.getOffenceSpeed());
            chargeIndex++;
        }

        return infringment;
    }

    private static InfringementChargeModel getInfringementCharge(FineChargeModel fineCharge, double speed){

        InfringementChargeModel infringementCharge = new InfringementChargeModel();

        infringementCharge.setId((long)0);
        infringementCharge.setChargeCode(fineCharge.getCode());
        infringementCharge.setDescription(fineCharge.getShortDescription());
        infringementCharge.setUserCapturedDescription(fineCharge.getSecondaryDescription());
        infringementCharge.setRegulation(fineCharge.getRegulationDescription());
        infringementCharge.setPrintDescription(fineCharge.getDescription());
        infringementCharge.setFineAmount(fineCharge.getFineAmount());
        infringementCharge.setSpeed(speed);

        return infringementCharge;
    }
}
