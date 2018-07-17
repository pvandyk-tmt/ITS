using Kapsch.Core.Data;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.ITS.Gateway.Models.Adjudicate;
using Kapsch.ITS.Repositories;
using Kapsch.ThirdParty.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Adjudicate")]
    [UsageLog]
    public class AdjudicateController : BaseController
    {
        [Route("Case/Unlock")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult UnlockCase(string ticketNumber)
        {
            using (var dbContext = new DataContext())
            {
                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.caseUnlock(ticketNumber))
                {
                    return this.BadRequest(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Case/First")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult FirstCase(string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var ticketCount = 0;
                var rejectReasons = new List<AdjudicateRepository.sRejectReasons>();
                var caseInfo = new AdjudicateRepository.sCaseInfo();
                var remoteCaseInfo = new AdjudicateRepository.sCaseInfo();
                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.caseFirst(ref rejectReasons, out ticketCount, out caseInfo, out remoteCaseInfo, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            TicketNo = caseInfo.mTicketNo,
                            Notification = caseInfo.mNotification,
                            VehicleRegNo = caseInfo.mVehicleRegNo,
                            VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                            VehicleMake = caseInfo.mVehicleMake,
                            VehicleModel = caseInfo.mVehicleModel,
                            VehicleColour = caseInfo.mVehicleColour,
                            VehicleType = caseInfo.mVehicleType,
                            VehicleLicenseExpire = caseInfo.mVehicleLicenseExpire,
                            OffenceSet = caseInfo.mOffenceSet,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            OffenceDirectionLane = caseInfo.mOffenceDirectionLane,
                            OffenceCode = caseInfo.mOffenceCode,
                            OffenceNotes = caseInfo.mOffenceNotes,
                            OffenceAdditionalsXml = caseInfo.mOffenceAdditionalsXml,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            ImageNP = caseInfo.mImageNP ,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImageNP = remoteCaseInfo.mImageNP
                        },
                        TicketCount = ticketCount,
                        RejectReasons = rejectReasons.Select(f => new RejectReasonModel { ID = f.mID, Description = f.mDescription }).ToList()
                    });
            }
        }

        [Route("Case/Additionals")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(OffenceCodeModel))]
        public IHttpActionResult CaseAdditionals(int offenceSet)
        {
            using (var dbContext = new DataContext())
            {               
                var offenceCodes = new List<AdjudicateRepository.sOffenseCode>();

                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.caseAdditionals(offenceSet, out offenceCodes))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok(
                    offenceCodes.Select(f => 
                        new OffenceCodeModel 
                        { 
                            Amount = f.mAmount, 
                            Code = f.mCode, 
                            Description = f.mDescription 
                        })
                        .ToList());
            }
        }

        [Route("Case/Reject")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult RejectCase(string ticketNumber, int reasonID, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var ticketCount = 0;
                var caseInfo = new AdjudicateRepository.sCaseInfo();
                caseInfo.mTicketNo = ticketNumber;

                var remoteCaseInfo = new AdjudicateRepository.sCaseInfo();
                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.caseReject(ref caseInfo, ref remoteCaseInfo, reasonID, out ticketCount, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            TicketNo = caseInfo.mTicketNo,
                            Notification = caseInfo.mNotification,
                            VehicleRegNo = caseInfo.mVehicleRegNo,
                            VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                            VehicleMake = caseInfo.mVehicleMake,
                            VehicleModel = caseInfo.mVehicleModel,
                            VehicleColour = caseInfo.mVehicleColour,
                            VehicleType = caseInfo.mVehicleType,
                            VehicleLicenseExpire = caseInfo.mVehicleLicenseExpire,
                            OffenceSet = caseInfo.mOffenceSet,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            OffenceDirectionLane = caseInfo.mOffenceDirectionLane,
                            OffenceCode = caseInfo.mOffenceCode,
                            OffenceNotes = caseInfo.mOffenceNotes,
                            OffenceAdditionalsXml = caseInfo.mOffenceAdditionalsXml,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            ImageNP = caseInfo.mImageNP,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImageNP = remoteCaseInfo.mImageNP
                        },
                        TicketCount = ticketCount
                    });
            }
        }

        [Route("Case/Accept")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult AcceptCase([FromBody] CaseModel model, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var ticketCount = 0;
                var caseInfo = new AdjudicateRepository.sCaseInfo();
                caseInfo.mTicketNo = model.TicketNo;
                caseInfo.mOffenceAdditionalsXml = model.OffenceAdditionalsXml;
                caseInfo.mOffenceSet = model.OffenceSet;
                caseInfo.mOffenceNotes = model.OffenceNotes;

                var remoteCaseInfo = new AdjudicateRepository.sCaseInfo();
                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                if (!adjudicateRepository.caseAdjudicate(ref caseInfo, ref remoteCaseInfo, out ticketCount, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                var register = dbContext.Registers.SingleOrDefault(f => f.ReferenceNumber == caseInfo.mTicketNo);
                if (register == null)
                {
                    return this.BadRequestEx(
                        Error.PopulateUnexpectedException(
                            new Exception(string.Format("Case ({0}) not added to Registers table.", caseInfo.mTicketNo))));
                }
                var referenceNumber = dbContext.GeneratedReferenceNumbers.SingleOrDefault(f => f.ReferenceNumber == caseInfo.mTicketNo);
                if (referenceNumber == null)
                {
                    return this.BadRequestEx(
                        Error.PopulateUnexpectedException(
                            new Exception(string.Format("Case ({0}) not added to GeneratedReferenceNumbers table.", caseInfo.mTicketNo))));
                }

                var paymentProvider = ProviderFactory.Get();
                var transactionIDModel =
                        paymentProvider.RegisterTransaction(
                            new ThirdParty.Payment.Models.TransactionModel
                            {
                                CompanyRef = referenceNumber.ReferenceNumber,
                                CompanyAccRef = string.Empty,
                                Amount = register.OutstandingAmount.HasValue ? register.OutstandingAmount.Value : 0,
                                UserID = SessionModel.UserName,
                                ServiceDescription = "Reserved Payment Token",
                                ServiceType = 6067
                            });


                referenceNumber.ExternalToken = transactionIDModel.TransactionToken;
                referenceNumber.ExternalReference = transactionIDModel.TransactionReference;

                dbContext.SaveChanges();

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            TicketNo = caseInfo.mTicketNo,
                            Notification = caseInfo.mNotification,
                            VehicleRegNo = caseInfo.mVehicleRegNo,
                            VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                            VehicleMake = caseInfo.mVehicleMake,
                            VehicleModel = caseInfo.mVehicleModel,
                            VehicleColour = caseInfo.mVehicleColour,
                            VehicleType = caseInfo.mVehicleType,
                            VehicleLicenseExpire = caseInfo.mVehicleLicenseExpire,
                            OffenceSet = caseInfo.mOffenceSet,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            OffenceDirectionLane = caseInfo.mOffenceDirectionLane,
                            OffenceCode = caseInfo.mOffenceCode,
                            OffenceNotes = caseInfo.mOffenceNotes,
                            OffenceAdditionalsXml = caseInfo.mOffenceAdditionalsXml,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            ImageNP = caseInfo.mImageNP,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImageNP = remoteCaseInfo.mImageNP
                        },
                        TicketCount = ticketCount
                    });
            }
        }

        [Route("Fishpond")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(FishpondCaseModel))]
        public IHttpActionResult FirstFishpondCase()
        {
            using (var dbContext = new DataContext())
            {
               
                var rejectReasons = new List<AdjudicateRepository.sRejectReasons>();
                var fishpondInfoCases = new List<AdjudicateRepository.cFishpondInfo>();

                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.fishpondGetCases(ref rejectReasons, ref fishpondInfoCases))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok(
                    new FirstFishpondCaseModel
                    {
                        Cases = fishpondInfoCases.Select(f => 
                            new FishpondCaseModel 
                            {
                                LockedBy = f.pLockedBy,
                                RejectBy = f.pRejectBy,
                                RejectReason = f.pRejectReason,
                                TicketDate = f.pTicketDate,
                                TicketNo = f.pTicketNo,
                                TimesRejected = f.pTimesRejected,
                                VehicleMake = f.pVehicleMake,
                                VehicleModel = f.pVehicleModel,
                                VehicleRegistration = f.pVehicleRegistration,
                                VerifyDate = f.pVerifyDate
                            })
                            .ToList(),
                        RejectReasons = rejectReasons.Select(f => new RejectReasonModel { ID = f.mID, Description = f.mDescription }).ToList()
                    });
            }
        }


        [Route("Fishpond")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(CaseModel))]
        public IHttpActionResult FishpondCase(string ticketNumber, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var caseInfo = new AdjudicateRepository.sCaseInfo();
                var remoteCaseInfo = new AdjudicateRepository.sCaseInfo();

                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.fishpondGetCase(ticketNumber, ref caseInfo, ref remoteCaseInfo, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok(
                    new CaseModel
                    {
                        TicketNo = caseInfo.mTicketNo,
                        Notification = caseInfo.mNotification,
                        VehicleRegNo = caseInfo.mVehicleRegNo,
                        VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                        VehicleMake = caseInfo.mVehicleMake,
                        VehicleModel = caseInfo.mVehicleModel,
                        VehicleColour = caseInfo.mVehicleColour,
                        VehicleType = caseInfo.mVehicleType,
                        VehicleLicenseExpire = caseInfo.mVehicleLicenseExpire,
                        OffenceSet = caseInfo.mOffenceSet,
                        OffenceDate = caseInfo.mOffenceDate,
                        OffenceSpeed = caseInfo.mOffenceSpeed,
                        OffenceZone = caseInfo.mOffenceZone,
                        OffenceDirectionLane = caseInfo.mOffenceDirectionLane,
                        OffenceCode = caseInfo.mOffenceCode,
                        OffenceNotes = caseInfo.mOffenceNotes,
                        OffenceAdditionalsXml = caseInfo.mOffenceAdditionalsXml,
                        Image1 = caseInfo.mImage1,
                        Image2 = caseInfo.mImage2,
                        ImageNP = caseInfo.mImageNP,
                        RemoteImage1 = remoteCaseInfo.mImage1,
                        RemoteImage2 = remoteCaseInfo.mImage2,
                        RemoteImageNP = remoteCaseInfo.mImageNP
                    });
            }
        }

        [Route("Fishpond/AllCases")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(FirstFishpondCaseModel))]
        public IHttpActionResult FishpondCases()
        {
            using (var dbContext = new DataContext())
            {

                var rejectReasons = new List<AdjudicateRepository.sRejectReasons>();
                var fishpondInfoCases = new List<AdjudicateRepository.cFishpondInfo>();

                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.fishpondGetCases(ref rejectReasons, ref fishpondInfoCases))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }
                //
                return Ok(
                    new FirstFishpondCaseModel
                    {
                        Cases = fishpondInfoCases.Select(f =>
                            new FishpondCaseModel
                            {
                                LockedBy = f.pLockedBy,
                                RejectBy = f.pRejectBy,
                                RejectReason = f.pRejectReason,
                                TicketDate = f.pTicketDate,
                                TicketNo = f.pTicketNo,
                                TimesRejected = f.pTimesRejected,
                                VehicleMake = f.pVehicleMake,
                                VehicleModel = f.pVehicleModel,
                                VehicleRegistration = f.pVehicleRegistration,
                                VerifyDate = f.pVerifyDate
                            })
                            .ToList(),
                        RejectReasons = rejectReasons.Select(f => new RejectReasonModel { ID = f.mID, Description = f.mDescription }).ToList()
                    });
            }
        }

        [Route("Fishpond/Accept")]
        [HttpPost]
        [SessionAuthorize]
        public IHttpActionResult AcceptFishpondCase([FromBody] CaseModel model, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var caseInfo = new AdjudicateRepository.sCaseInfo();
                caseInfo.mTicketNo = model.TicketNo;
                caseInfo.mOffenceAdditionalsXml = model.OffenceAdditionalsXml;
                caseInfo.mOffenceSet = model.OffenceSet;
                caseInfo.mOffenceNotes = model.OffenceNotes;

                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.fishpondCaseAccept(caseInfo, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Fishpond/Reject")]
        [HttpPost]
        [SessionAuthorize]
        public IHttpActionResult RejectFishpondCase(string ticketNumber, int reasonID, string notes, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.fishpondCaseReject(ticketNumber, reasonID, notes, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Fishpond/Unlock")]
        [HttpPost]
        [SessionAuthorize]
        public IHttpActionResult UnlockFishpondCase(string ticketNumber)
        {
            using (var dbContext = new DataContext())
            {
                var adjudicateRepository = new AdjudicateRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!adjudicateRepository.fishpondUnlock(ticketNumber))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(adjudicateRepository.Error));
                }

                return Ok();
            }
        }
    }
}
