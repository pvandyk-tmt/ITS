package za.co.kapsch.iticket.Models;

import android.app.Activity;

import java.sql.SQLException;
import java.util.Date;

import za.co.kapsch.iticket.App;
import za.co.kapsch.iticket.Constants;
import za.co.kapsch.iticket.Enums.EvidenceType;
import za.co.kapsch.iticket.Enums.IdentificationType;
import za.co.kapsch.iticket.orm.HandWrittenRepository;
import za.co.kapsch.shared.MessageManager;
import za.co.kapsch.shared.Models.EndPointConfigModel;
import za.co.kapsch.shared.Models.SessionModel;
import za.co.kapsch.shared.Utilities;
import za.co.kapsch.iticket.orm.ChargeInfoRepository;
import za.co.kapsch.iticket.orm.EvidenceRepository;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.Models.DistrictModel;
import za.co.kapsch.shared.Models.UserModel;

/**
 * Created by CSenekal on 2017/04/05.
 */
public class HandWrittenToTicketModel {

    public TicketModel handWrittenToTicket(HandWrittenModel handWrittenModel){

        try{
            offenceDateSanityCheck(handWrittenModel.getOffenceDate(), handWrittenModel.getIssueDate());

            TicketModel ticket = new TicketModel();

            ticket.setOffender(new OffenderModel());
            ticket.getOffender().setLastName(handWrittenModel.getSurname());
            ticket.getOffender().setFirstName(handWrittenModel.getFirstName());
            ticket.getOffender().setInitials(handWrittenModel.getInitials());
            ticket.getOffender().setIdNumber(handWrittenModel.getIdentificationNumber());
            ticket.getOffender().setIdType(handWrittenModel.getIdentificationTypeId());
            ticket.getOffender().setCountryId(handWrittenModel.getIdentificationCountryID());

            ticket.getOffender().setPhysicalStreet1(handWrittenModel.getPhysicalStreet1());
            ticket.getOffender().setPhysicalStreet2(handWrittenModel.getPhysicalStreet2());
            ticket.getOffender().setPhysicalSuburb(handWrittenModel.getPhysicalSuburb());
            ticket.getOffender().setPhysicalTown(handWrittenModel.getPhysicalTown());
            ticket.getOffender().setPhysicalCode(handWrittenModel.getPhysicalCode());

            //ticket.getOffender().setEmployer(handWrittenModel.get);
            ticket.getOffender().setMobileNumber (handWrittenModel.getTelephone());
            ticket.getOffender().setGender(handWrittenModel.getGender());
            ticket.getOffender().setOccupation(handWrittenModel.getOccupation());

            EvidenceModel driversLicencePhoto  = EvidenceRepository.getEvidence(handWrittenModel.getTicketNumber(), EvidenceType.OffenderPhoto);
            if (driversLicencePhoto != null) {
                ticket.getOffender().setPhoto(Utilities.byteArrayToBitmap(driversLicencePhoto.getEvidence()));
            }

            EvidenceModel personSignature  = EvidenceRepository.getEvidence(handWrittenModel.getTicketNumber(), EvidenceType.PersonSignature);
            if (personSignature != null) {
                ticket.getOffender().setSignature(personSignature.getEvidence());
            }

            ticket.setVehicle(new VehicleModel());
            ticket.getVehicle().setLicenceNumber(handWrittenModel.getVehicleRegistrationMain());
            ticket.getVehicle().setMake(handWrittenModel.getVehicleMakeMain());
            ticket.getVehicle().setType(handWrittenModel.getVehicleTypeMain());

            ticket.setInfringement(new InfringementModel());
            ticket.getInfringement().setCancelled(handWrittenModel.isIsCancelled());
            ticket.getInfringement().setCancelledReason(handWrittenModel.getCancelReason());
            ticket.getInfringement().setPayDate(handWrittenModel.getPaymentDate());
            ticket.getInfringement().setTicketNumber(handWrittenModel.getTicketNumber());
            ticket.getInfringement().setExternalToken(handWrittenModel.getExternalToken());
            ticket.getInfringement().setExternalTokenReference(handWrittenModel.getExternalTokenReference());
            ticket.getInfringement().setOffenceDate(handWrittenModel.getOffenceDate());
            ticket.getInfringement().setLocationDescription(handWrittenModel.getOffenceLocationStreet());
            ticket.getInfringement().setLocationSuburb(handWrittenModel.getOffenceLocationSuburb());
            ticket.getInfringement().setLocationTown(handWrittenModel.getOffenceLocationTown());
            ticket.getInfringement().setIssueDate(handWrittenModel.getIssueDate());
            ticket.getInfringement().setNotes(handWrittenModel.getNotes());

            ticket.getInfringement().getInfringementCharges()[0] = infringementChargeFromChargeCode(handWrittenModel.getChargeCode1(),
                    handWrittenModel.getSpeed() == null ? null : handWrittenModel.getSpeed().toString(),
                    handWrittenModel.getVehicleRegistrationMain(),
                    handWrittenModel.getAmount1() == null ? 0 : handWrittenModel.getAmount1(),
                    handWrittenModel.getVehicleMakeMain(),
                    handWrittenModel.getVehicleModelMain());

            ticket.getInfringement().getInfringementCharges()[1] = infringementChargeFromChargeCode(handWrittenModel.getChargeCode2(),
                    handWrittenModel.getSpeed() == null ? null : handWrittenModel.getSpeed().toString(),
                    handWrittenModel.getVehicleRegistrationMain(),
                    handWrittenModel.getAmount2() == null ? 0 : handWrittenModel.getAmount2(),
                    handWrittenModel.getVehicleMakeMain(),
                    handWrittenModel.getVehicleModelMain());

            ticket.getInfringement().getInfringementCharges()[2] = infringementChargeFromChargeCode(handWrittenModel.getChargeCode3(),
                    handWrittenModel.getSpeed() == null ? null : handWrittenModel.getSpeed().toString(),
                    handWrittenModel.getVehicleRegistrationMain(),
                    handWrittenModel.getAmount3() == null ? 0 : handWrittenModel.getAmount3(),
                    handWrittenModel.getVehicleMakeMain(),
                    handWrittenModel.getVehicleModelMain());

            if (ticket.getInfringement().getInfringementCharges()[2] != null) {
                ticket.getInfringement().getInfringementCharges()[2].setIsAlternative((handWrittenModel.getHasAlternativeCharge() == null || handWrittenModel.getHasAlternativeCharge() == 0) ? false : true);
            }

            if (ticket.getInfringement().getInfringementCharges()[0] != null){
                ticket.getInfringement().getInfringementCharges()[0].setUserCapturedDescription(handWrittenModel.getChargeDescription1());
            }

            if (ticket.getInfringement().getInfringementCharges()[1] != null){
                ticket.getInfringement().getInfringementCharges()[1].setUserCapturedDescription(handWrittenModel.getChargeDescription2());
            }

            if (ticket.getInfringement().getInfringementCharges()[2] != null){
                ticket.getInfringement().getInfringementCharges()[2].setUserCapturedDescription(handWrittenModel.getChargeDescription3());
            }

            UserModel user = null;

            ticket.getOffender().setPostalPoBox(handWrittenModel.getPostalPoBox());
            ticket.getOffender().setPostalStreet(handWrittenModel.getPostalStreet());
            ticket.getOffender().setPostalSuburb(handWrittenModel.getPostalSuburb());
            ticket.getOffender().setPostalTown(handWrittenModel.getPostalTown());
            ticket.getOffender().setPhysicalCode(handWrittenModel.getPostalCode());

            ticket.setCourtInfo(new CourtInfoModel());
            ticket.getCourtInfo().setCourt(new CourtModel());
            ticket.getCourtInfo().getCourt().setName(handWrittenModel.getCourtName());
            ticket.getCourtInfo().setCourtRoom(new CourtRoomModel());
            ticket.getCourtInfo().getCourtRoom().setRoomNumber(handWrittenModel.getCourtRoom());
            ticket.getCourtInfo().setCourtDate(new CourtDateModel());
            ticket.getCourtInfo().getCourtDate().setDate(handWrittenModel.getCourtDate());

            if (handWrittenModel.getOfficerName() != null) {
                user = new UserModel();
                String officerDetail[] = handWrittenModel.getOfficerName().split("\\|", -1);
                user.setFirstName(officerDetail[0]);
                user.setLastName(officerDetail[1]);
                user.setInfrastructureNumber(officerDetail[2]);
            }

            EvidenceModel officerSignature  = EvidenceRepository.getEvidence(handWrittenModel.getTicketNumber(), EvidenceType.OfficerSignature);
            if (officerSignature != null) {
                user.setSignature(officerSignature.getEvidence());
            }

            ticket.setUser(user);

            ticket.setDistrict(SessionModel.getInstance().getDistrict());

            ticket.getOffender().setCertificateNumber(handWrittenModel.getDriverLicenceCertificateNo());

            //Lookup IdType from database
            //ticket.getOffender().setIdType(handWrittenModel.getIdentificationTypeId());

            ticket.getVehicle().setExpireDate(handWrittenModel.getVehicleLicenceExpiryDate());
            ticket.getVehicle().setModel(handWrittenModel.getVehicleModelMain());
            ticket.getVehicle().setColour(handWrittenModel.getVehicleColour());
            ticket.getVehicle().setRegisterNumber(handWrittenModel.getVehicleRegisterNumber());
            ticket.getVehicle().setEngineNumber(handWrittenModel.getVehicleEngineNumber());
            ticket.getVehicle().setVehicleIdentificationNumber(handWrittenModel.getVehicleChassisNumber());

            ticket.getInfringement().setLongitude(handWrittenModel.getOffenceLocationLongitude());
            ticket.getInfringement().setLatitude(handWrittenModel.getOffenceLocationLatitude());

            return ticket;

        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "ReprintActivity::section56ToTicket()"), ErrorSeverity.Medium);
        }

        return null;
    }

    private InfringementChargeModel infringementChargeFromChargeCode(String chargeCode, String speed, String licenceNumber, double fineAmount, String vehicleMake, String vehicleModel){

        try {
            if (chargeCode == null) return null;

            ChargeInfoModel chargeBookModel = ChargeInfoRepository.getCharge(chargeCode);
            InfringementChargeModel infringementCharge = new InfringementChargeModel();

            infringementCharge.setDescription(chargeBookModel.getDescription());
            String printDescription = replacePlaceHoldersInChargeDescription(
                    chargeBookModel.getPrintDescription(),
                    speed == null ? "0" : speed,
                    Integer.toString(chargeBookModel.getZone()),
                    licenceNumber,
                    vehicleMake,
                    vehicleModel);

            infringementCharge.setPrintDescription(printDescription);

            infringementCharge.setRegulation(chargeBookModel.getRegulationDescription());

            infringementCharge.setSpeed(speed == null ? 0 : Integer.parseInt(speed));
            infringementCharge.setId(chargeBookModel.getId());
            infringementCharge.setChargeCode(chargeBookModel.getCode());
            infringementCharge.setFineAmount(fineAmount);
            infringementCharge.setZone(chargeBookModel.getZone());
            return infringementCharge;

        } catch (SQLException e) {
            MessageManager.showMessage(Utilities.exceptionMessage(e, "InfringementChargeFromChargeCode"), ErrorSeverity.Medium);
        }

        return null;
    }

    private String replacePlaceHoldersInChargeDescription(String chargeDescription, String speed, String zone, String licenceNumber, String vehicleMake, String vehicleModel){

        String description = chargeDescription;

        if (chargeDescription.contains(Constants.SPEED_PLACE_HOLDER)){
            description = description.replace(Constants.SPEED_PLACE_HOLDER, speed);
        }

        if (chargeDescription.contains(Constants.VEHREG_PLACE_HOLDER)){
            description = description.replace(Constants.VEHREG_PLACE_HOLDER, licenceNumber);
        }

        if (chargeDescription.contains(Constants.VEHMAKE_PLACE_HOLDER)){
            description = description.replace(Constants.VEHMAKE_PLACE_HOLDER, vehicleMake);
        }


        if (chargeDescription.contains(Constants.VEHMODEL_PLACE_HOLDER)){
            description = description.replace(Constants.VEHMODEL_PLACE_HOLDER, vehicleModel);
        }


        if (chargeDescription.contains(Constants.ZONE_PLACE_HOLDER)){
            description = description.replace(Constants.ZONE_PLACE_HOLDER, zone);
        }

        return description;
    }

    public void offenceDateSanityCheck(Date offenceDate, Date issueDate){

        try {
            if (EndPointConfigModel.getInstance().getITSGateway().equals("192.168.0.33:60002")) {

                if (issueDate == null) {
                    MessageManager.showMessage("ISSUE DATE IS NULL", ErrorSeverity.None);
                }

                if (offenceDate == null) {
                    MessageManager.showMessage("OFFENCE DATE IS NULL", ErrorSeverity.None);
                }

                if (HandWrittenRepository.offenceDateCount(offenceDate) > 1) {
                    MessageManager.showMessage("OFFENCE DATE ALREADY EXIST", ErrorSeverity.None);
                }
            }
        }catch (Exception e){
            MessageManager.showMessage(Utilities.exceptionMessage(e, "offenceDateSanityCheck"), ErrorSeverity.High);
        }
    }
}
