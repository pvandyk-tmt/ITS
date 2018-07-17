using Kapsch.Core.Data;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.ITS.Gateway.Models.Verify;
using Kapsch.ITS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Verify")]
    [UsageLog]
    public class VerifyController : BaseController
    {
        [Route("Address")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(IList<AddressInfoModel>))]
        public IHttpActionResult GetAdditionalAddress(int personalID)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var addresses = verifyRepository.GetAdditionalAddress(personalID);

                return Ok(addresses.Select(f =>
                    new AddressInfoModel
                    {
                        AddressDate = f.mAddressDate,
                        AddressTypeID = f.AddressTypeID,
                        Code = f.mCode,
                        CompanyName = f.mCompanyName,
                        Date = f.mDate,
                        IDNumber = f.mIDNumber,
                        Key = f.mKey,
                        PersonID = f.mPersonID,
                        POBox = f.mPOBox,
                        Residual = f.mResidual,
                        ResidualScore = f.mResidualScore,
                        Source = f.mSource,
                        Street = f.mStreet,
                        Suburb = f.mSuburb,
                        Town = f.mTown,
                        UserDetails = f.mUserDetails
                    })
                    .ToList());
            }
        }

        [Route("Address/Check")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(CaseModel))]
        public IHttpActionResult CheckAddress([FromBody] CaseModel model, bool isSummons)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var caseInfo = new VerifyRepository.sCaseInfo();

                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();


                if (!verifyRepository.checkAddress(ref caseInfo, isSummons))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }


                return Ok(
                    new CaseModel
                    {
                        Image1 = caseInfo.mImage1,
                        Image1ID = caseInfo.mImage1ID,
                        Image2 = caseInfo.mImage2,
                        Image2ID = caseInfo.mImage2ID,
                        Image3 = caseInfo.mImage3,
                        Image3ID = caseInfo.mImage3ID,
                        Image4 = caseInfo.mImage4,
                        Image4ID = caseInfo.mImage4ID,
                        ImageNP = caseInfo.mImageNP,
                        NatisPhysical = caseInfo.mNatisPhysical.ToModel(),
                        NatisPostal = caseInfo.mNatisPostal.ToModel(),
                        OffenceDate = caseInfo.mOffenceDate,
                        OffenceNewNotes = caseInfo.mOffenceNewNotes,
                        OffenceOldNotes = caseInfo.mOffenceOldNotes,
                        OnlyOneImage = caseInfo.mOnlyOneImage,
                        PersonID = caseInfo.mPersonID,
                        PersonKey = caseInfo.mPersonKey,
                        PersonMiddleNames = caseInfo.mPersonMiddleNames,
                        PersonName = caseInfo.mPersonName,
                        PersonPhysicalAddressKey = caseInfo.mPersonPhysicalAddressKey,
                        PersonPostalAddressKey = caseInfo.mPersonPostalAddressKey,
                        PersonSurname = caseInfo.mPersonSurname,
                        PersonTelephone = caseInfo.mPersonTelephone,
                        PrintImageNo = caseInfo.mPrintImageNo,
                        SystemPhysical = caseInfo.mSystemPhysical.ToModel(),
                        SystemPostal = caseInfo.mSystemPostal.ToModel(),
                        IsNumberPlateCentralCaptured = verifyRepository.pNPCapturedOnCentral,
                        NumberPlateCentralPath = verifyRepository.pNPCapturePath
                    });
            }
        }

        [Route("PostalCode")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(IList<PostalCodeModel>))]
        public IHttpActionResult GetPostalCode(string city, string suburb, string code, bool isPhysical)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var postalCodes = new List<VerifyRepository.cPostalCode>();

                if (!verifyRepository.getPostalCodes(city, suburb, code, isPhysical, postalCodes))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(postalCodes.Select(f => new PostalCodeModel { City = f.pCity, Code = f.pCode, Suburb = f.pSuburb }).ToList());
            }
        }

        [Route("Case/First")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult GetFirstCase()
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var rejectReasons = new List<VerifyRepository.sRejectReasons>();
                var captureTypes = new List<VerifyRepository.sCaptureTypes>();
                var caseInfo = new VerifyRepository.sCaseInfo();
                var count = 0;

                if (!verifyRepository.caseFirst(ref rejectReasons, ref captureTypes, out count, ref caseInfo))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            Image1 = caseInfo.mImage1,
                            Image1ID = caseInfo.mImage1ID,
                            Image2 = caseInfo.mImage2,
                            Image2ID = caseInfo.mImage2ID,
                            Image3 = caseInfo.mImage3,
                            Image3ID = caseInfo.mImage3ID,
                            Image4 = caseInfo.mImage4,
                            Image4ID = caseInfo.mImage4ID,
                            ImageNP = caseInfo.mImageNP,
                            NatisPhysical = caseInfo.mNatisPhysical.ToModel(),
                            NatisPostal = caseInfo.mNatisPostal.ToModel(),
                            OffenceDate = caseInfo.mOffenceDate,
                            OffenceNewNotes = caseInfo.mOffenceNewNotes,
                            OffenceOldNotes = caseInfo.mOffenceOldNotes,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PersonID = caseInfo.mPersonID,
                            PersonKey = caseInfo.mPersonKey,
                            PersonMiddleNames = caseInfo.mPersonMiddleNames,
                            PersonName = caseInfo.mPersonName,
                            PersonPhysicalAddressKey = caseInfo.mPersonPhysicalAddressKey,
                            PersonPostalAddressKey = caseInfo.mPersonPostalAddressKey,
                            PersonSurname = caseInfo.mPersonSurname,
                            PersonTelephone = caseInfo.mPersonTelephone,
                            PrintImageNo = caseInfo.mPrintImageNo,
                            SystemPhysical = caseInfo.mSystemPhysical.ToModel(),
                            SystemPostal = caseInfo.mSystemPostal.ToModel(),
                            TicketNo = caseInfo.mTicketNo,
                            UseGismoAddress = caseInfo.mUseGismoAddress,
                            VehicleCaptureType = caseInfo.mVehicleCaptureType,
                            VehicleColour = caseInfo.mVehicleColour,
                            VehicleMake = caseInfo.mVehicleMake,
                            VehicleModel = caseInfo.mVehicleModel,
                            VehicleRegNo = caseInfo.mVehicleRegNo,
                            VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                            VehicleType = caseInfo.mVehicleType,
                            IsNumberPlateCentralCaptured = verifyRepository.pNPCapturedOnCentral,
                            NumberPlateCentralPath = verifyRepository.pNPCapturePath
                        },
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Description = f.mDescription }).ToList(),
                        Count = count,
                        RejectReasons = rejectReasons.Select(f => new RejectReasonModel { ID = f.mID, Description = f.mDescription }).ToList()
                    });
            }
        }

        [Route("Case/Unlock")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult UnlockCase(string ticketNumber)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                if (!verifyRepository.caseUnlock(ticketNumber))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Summons/Unlock")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult UnlockSummons()
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                if (!verifyRepository.summonsUnlock())
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Summons/Lock")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(CaseModel))]
        public IHttpActionResult LockSummons([FromBody] CaseModel model)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var caseInfo = new VerifyRepository.sCaseInfo();

                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();


                if (!verifyRepository.summonsLock(ref caseInfo))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new CaseModel
                    {
                        Image1 = caseInfo.mImage1,
                        Image1ID = caseInfo.mImage1ID,
                        Image2 = caseInfo.mImage2,
                        Image2ID = caseInfo.mImage2ID,
                        Image3 = caseInfo.mImage3,
                        Image3ID = caseInfo.mImage3ID,
                        Image4 = caseInfo.mImage4,
                        Image4ID = caseInfo.mImage4ID,
                        ImageNP = caseInfo.mImageNP,
                        NatisPhysical = caseInfo.mNatisPhysical.ToModel(),
                        NatisPostal = caseInfo.mNatisPostal.ToModel(),
                        OffenceDate = caseInfo.mOffenceDate,
                        OffenceNewNotes = caseInfo.mOffenceNewNotes,
                        OffenceOldNotes = caseInfo.mOffenceOldNotes,
                        OnlyOneImage = caseInfo.mOnlyOneImage,
                        PersonID = caseInfo.mPersonID,
                        PersonKey = caseInfo.mPersonKey,
                        PersonMiddleNames = caseInfo.mPersonMiddleNames,
                        PersonName = caseInfo.mPersonName,
                        PersonPhysicalAddressKey = caseInfo.mPersonPhysicalAddressKey,
                        PersonPostalAddressKey = caseInfo.mPersonPostalAddressKey,
                        PersonSurname = caseInfo.mPersonSurname,
                        PersonTelephone = caseInfo.mPersonTelephone,
                        PrintImageNo = caseInfo.mPrintImageNo,
                        SystemPhysical = caseInfo.mSystemPhysical.ToModel(),
                        SystemPostal = caseInfo.mSystemPostal.ToModel(),
                        IsNumberPlateCentralCaptured = verifyRepository.pNPCapturedOnCentral,
                        NumberPlateCentralPath = verifyRepository.pNPCapturePath

                    });
            }
        }

        [Route("Case/Accept")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult AcceptCase(CaseModel model, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, int printImageID, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var captureTypes = new List<VerifyRepository.sCaptureTypes>();
                var count = 0;
                var caseInfo = new VerifyRepository.sCaseInfo();
                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();


                if (!verifyRepository.caseAccept(ref caseInfo, addressChanged, personChanged, typeID, typeAmount, ref captureTypes, out count, printImageID))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            Image1 = caseInfo.mImage1,
                            Image1ID = caseInfo.mImage1ID,
                            Image2 = caseInfo.mImage2,
                            Image2ID = caseInfo.mImage2ID,
                            Image3 = caseInfo.mImage3,
                            Image3ID = caseInfo.mImage3ID,
                            Image4 = caseInfo.mImage4,
                            Image4ID = caseInfo.mImage4ID,
                            ImageNP = caseInfo.mImageNP,
                            NatisPhysical = caseInfo.mNatisPhysical.ToModel(),
                            NatisPostal = caseInfo.mNatisPostal.ToModel(),
                            OffenceDate = caseInfo.mOffenceDate,
                            OffenceNewNotes = caseInfo.mOffenceNewNotes,
                            OffenceOldNotes = caseInfo.mOffenceOldNotes,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PersonID = caseInfo.mPersonID,
                            PersonKey = caseInfo.mPersonKey,
                            PersonMiddleNames = caseInfo.mPersonMiddleNames,
                            PersonName = caseInfo.mPersonName,
                            PersonPhysicalAddressKey = caseInfo.mPersonPhysicalAddressKey,
                            PersonPostalAddressKey = caseInfo.mPersonPostalAddressKey,
                            PersonSurname = caseInfo.mPersonSurname,
                            PersonTelephone = caseInfo.mPersonTelephone,
                            PrintImageNo = caseInfo.mPrintImageNo,
                            SystemPhysical = caseInfo.mSystemPhysical.ToModel(),
                            SystemPostal = caseInfo.mSystemPostal.ToModel(),
                            TicketNo = caseInfo.mTicketNo,
                            UseGismoAddress = caseInfo.mUseGismoAddress,
                            VehicleCaptureType = caseInfo.mVehicleCaptureType,
                            VehicleColour = caseInfo.mVehicleColour,
                            VehicleMake = caseInfo.mVehicleMake,
                            VehicleModel = caseInfo.mVehicleModel,
                            VehicleRegNo = caseInfo.mVehicleRegNo,
                            VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                            VehicleType = caseInfo.mVehicleType,
                            IsNumberPlateCentralCaptured = verifyRepository.pNPCapturedOnCentral,
                            NumberPlateCentralPath = verifyRepository.pNPCapturePath

                        },
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Description = f.mDescription }).ToList(),
                        Count = count
                    });
            }
        }

        [Route("Case/Reject")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult RejectCase(CaseModel model, int reasonID, bool registrationNumberChanged, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var captureTypes = new List<VerifyRepository.sCaptureTypes>();
                var count = 0;
                var caseInfo = new VerifyRepository.sCaseInfo();
                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();


                if (!verifyRepository.caseReject(ref caseInfo, reasonID, registrationNumberChanged, addressChanged, personChanged, typeID, typeAmount, ref captureTypes, out count))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            Image1 = caseInfo.mImage1,
                            Image1ID = caseInfo.mImage1ID,
                            Image2 = caseInfo.mImage2,
                            Image2ID = caseInfo.mImage2ID,
                            Image3 = caseInfo.mImage3,
                            Image3ID = caseInfo.mImage3ID,
                            Image4 = caseInfo.mImage4,
                            Image4ID = caseInfo.mImage4ID,
                            ImageNP = caseInfo.mImageNP,
                            NatisPhysical = caseInfo.mNatisPhysical.ToModel(),
                            NatisPostal = caseInfo.mNatisPostal.ToModel(),
                            OffenceDate = caseInfo.mOffenceDate,
                            OffenceNewNotes = caseInfo.mOffenceNewNotes,
                            OffenceOldNotes = caseInfo.mOffenceOldNotes,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PersonID = caseInfo.mPersonID,
                            PersonKey = caseInfo.mPersonKey,
                            PersonMiddleNames = caseInfo.mPersonMiddleNames,
                            PersonName = caseInfo.mPersonName,
                            PersonPhysicalAddressKey = caseInfo.mPersonPhysicalAddressKey,
                            PersonPostalAddressKey = caseInfo.mPersonPostalAddressKey,
                            PersonSurname = caseInfo.mPersonSurname,
                            PersonTelephone = caseInfo.mPersonTelephone,
                            PrintImageNo = caseInfo.mPrintImageNo,
                            SystemPhysical = caseInfo.mSystemPhysical.ToModel(),
                            SystemPostal = caseInfo.mSystemPostal.ToModel(),
                            TicketNo = caseInfo.mTicketNo,
                            UseGismoAddress = caseInfo.mUseGismoAddress,
                            VehicleCaptureType = caseInfo.mVehicleCaptureType,
                            VehicleColour = caseInfo.mVehicleColour,
                            VehicleMake = caseInfo.mVehicleMake,
                            VehicleModel = caseInfo.mVehicleModel,
                            VehicleRegNo = caseInfo.mVehicleRegNo,
                            VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                            VehicleType = caseInfo.mVehicleType,
                            IsNumberPlateCentralCaptured = verifyRepository.pNPCapturedOnCentral,
                            NumberPlateCentralPath = verifyRepository.pNPCapturePath

                        },
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Description = f.mDescription }).ToList(),
                        Count = count
                    });
            }
        }

        [Route("Fishpond/Accept")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult AcceptFishpondCase(CaseModel model, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, int printImageID, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var caseInfo = new VerifyRepository.sCaseInfo();
                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();

                if (!verifyRepository.fishpondCaseAccept(caseInfo, addressChanged, personChanged, typeID, typeAmount, printImageID))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Fishpond/Reject")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult RejectFishpondCase(CaseModel model, int reasonID, bool registrationNumberChanged, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var caseInfo = new VerifyRepository.sCaseInfo();
                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();

                if (!verifyRepository.fishpondCaseReject(caseInfo, reasonID, registrationNumberChanged, addressChanged, personChanged, typeID, typeAmount))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Summons/Accept")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(CaseModel))]
        public IHttpActionResult AcceptSummons([FromBody] CaseModel model, bool addressChanged, bool personChanged, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                var caseInfo = new VerifyRepository.sCaseInfo();
                caseInfo.mImage1 = model.Image1;
                caseInfo.mImage1ID = model.Image1ID;
                caseInfo.mImage2 = model.Image2;
                caseInfo.mImage2ID = model.Image2ID;
                caseInfo.mImage3 = model.Image3;
                caseInfo.mImage3ID = model.Image3ID;
                caseInfo.mImage4 = model.Image4;
                caseInfo.mImage4ID = model.Image4ID;
                caseInfo.mImageNP = model.ImageNP;
                caseInfo.mNatisPhysical = model.NatisPhysical.ToEntity();
                caseInfo.mNatisPostal = model.NatisPostal.ToEntity();
                caseInfo.mOffenceDate = model.OffenceDate;
                caseInfo.mOffenceNewNotes = model.OffenceNewNotes;
                caseInfo.mOffenceOldNotes = model.OffenceOldNotes;
                caseInfo.mOnlyOneImage = model.OnlyOneImage;
                caseInfo.mPersonID = model.PersonID;
                caseInfo.mPersonKey = model.PersonKey;
                caseInfo.mPersonMiddleNames = model.PersonMiddleNames;
                caseInfo.mPersonName = model.PersonName;
                caseInfo.mPersonPhysicalAddressKey = model.PersonPhysicalAddressKey;
                caseInfo.mPersonPostalAddressKey = model.PersonPostalAddressKey;
                caseInfo.mPersonSurname = model.PersonSurname;
                caseInfo.mPersonTelephone = model.PersonTelephone;
                caseInfo.mPrintImageNo = model.PrintImageNo;
                caseInfo.mSystemPhysical = model.SystemPhysical.ToEntity();
                caseInfo.mSystemPostal = model.SystemPostal.ToEntity();

                if (!verifyRepository.summonsAccept(ref caseInfo, addressChanged, personChanged))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new CaseModel
                    {
                        Image1 = caseInfo.mImage1,
                        Image1ID = caseInfo.mImage1ID,
                        Image2 = caseInfo.mImage2,
                        Image2ID = caseInfo.mImage2ID,
                        Image3 = caseInfo.mImage3,
                        Image3ID = caseInfo.mImage3ID,
                        Image4 = caseInfo.mImage4,
                        Image4ID = caseInfo.mImage4ID,
                        ImageNP = caseInfo.mImageNP,
                        NatisPhysical = caseInfo.mNatisPhysical.ToModel(),
                        NatisPostal = caseInfo.mNatisPostal.ToModel(),
                        OffenceDate = caseInfo.mOffenceDate,
                        OffenceNewNotes = caseInfo.mOffenceNewNotes,
                        OffenceOldNotes = caseInfo.mOffenceOldNotes,
                        OnlyOneImage = caseInfo.mOnlyOneImage,
                        PersonID = caseInfo.mPersonID,
                        PersonKey = caseInfo.mPersonKey,
                        PersonMiddleNames = caseInfo.mPersonMiddleNames,
                        PersonName = caseInfo.mPersonName,
                        PersonPhysicalAddressKey = caseInfo.mPersonPhysicalAddressKey,
                        PersonPostalAddressKey = caseInfo.mPersonPostalAddressKey,
                        PersonSurname = caseInfo.mPersonSurname,
                        PersonTelephone = caseInfo.mPersonTelephone,
                        PrintImageNo = caseInfo.mPrintImageNo,
                        SystemPhysical = caseInfo.mSystemPhysical.ToModel(),
                        SystemPostal = caseInfo.mSystemPostal.ToModel(),
                        TicketNo = caseInfo.mTicketNo,
                        UseGismoAddress = caseInfo.mUseGismoAddress,
                        VehicleCaptureType = caseInfo.mVehicleCaptureType,
                        VehicleColour = caseInfo.mVehicleColour,
                        VehicleMake = caseInfo.mVehicleMake,
                        VehicleModel = caseInfo.mVehicleModel,
                        VehicleRegNo = caseInfo.mVehicleRegNo,
                        VehicleRegNoConfirmed = caseInfo.mVehicleRegNoConfirmed,
                        VehicleType = caseInfo.mVehicleType,
                        IsNumberPlateCentralCaptured = verifyRepository.pNPCapturedOnCentral,
                        NumberPlateCentralPath = verifyRepository.pNPCapturePath

                    });
            }
        }

        [Route("Verify/Fishpond")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(FirstFishpondCaseModel))]
        public IHttpActionResult GetFirstFishpondCase()
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var rejectReasons = new List<VerifyRepository.sRejectReasons>();
                var cases = new List<VerifyRepository.cFishpondInfo>();

                if (!verifyRepository.fishpondGetCases(ref rejectReasons, ref cases))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new FirstFishpondCaseModel
                    {
                        Cases = cases.Select(f => 
                            new FishpondInfoModel
                            {
                                // TODO
                            })
                            .ToList(),
                        RejectReasons = rejectReasons.Select(f => new RejectReasonModel { ID = f.mID, Description = f.mDescription }).ToList()
                    });
            }
        }

        [Route("Verify/Fishpond")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(FishpondCaseModel))]
        public IHttpActionResult GetFishpondCase(string ticketNumber)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var captureTypes = new List<VerifyRepository.sCaptureTypes>();
                var @case = new VerifyRepository.sCaseInfo();

                if (!verifyRepository.fishpondGetCase(ticketNumber, ref @case, ref captureTypes))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok(
                    new FishpondCaseModel
                    {
                        // TODO
                    });
            }
        }

        [Route("Verify/Fishpond/Unlock")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult UnlockFishpondCase(string ticketNumber)
        {
            using (var dbContext = new DataContext())
            {
                var verifyRepository = new VerifyRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                var captureTypes = new List<VerifyRepository.sCaptureTypes>();
                
                if (!verifyRepository.fishpondUnlock(ticketNumber))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(verifyRepository.Error));
                }

                return Ok();
            }
        }
    }

    public static class Extensions
    {
        public static AddressInfoModel ToModel(this VerifyRepository.sAddressInfo addressInfo)
        {
            return
                new AddressInfoModel
                {
                    AddressDate = addressInfo.mAddressDate,
                    AddressTypeID = addressInfo.AddressTypeID,
                    Code = addressInfo.mCode,
                    CompanyName = addressInfo.mCompanyName,
                    Date = addressInfo.mDate,
                    IDNumber = addressInfo.mIDNumber,
                    Key = addressInfo.mKey,
                    PersonID = addressInfo.mPersonID,
                    POBox = addressInfo.mPOBox,
                    Residual = addressInfo.mResidual,
                    ResidualScore = addressInfo.mResidualScore,
                    Source = addressInfo.mSource,
                    Street = addressInfo.mStreet,
                    Suburb = addressInfo.mSuburb,
                    Town = addressInfo.mTown,
                    UserDetails = addressInfo.mUserDetails
                };
        }

        public static VerifyRepository.sAddressInfo ToEntity(this AddressInfoModel NatisPostal)
        {
            return 
                new VerifyRepository.sAddressInfo
                {
                    mAddressDate = NatisPostal.AddressDate,
                    AddressTypeID = NatisPostal.AddressTypeID,
                    mCode = NatisPostal.Code,
                    mCompanyName = NatisPostal.CompanyName,
                    mDate = NatisPostal.Date,
                    mIDNumber = NatisPostal.IDNumber,
                    mKey = NatisPostal.Key,
                    mPersonID = NatisPostal.PersonID,
                    mPOBox = NatisPostal.POBox,
                    mResidual = NatisPostal.Residual,
                    mResidualScore = NatisPostal.ResidualScore,
                    mSource = NatisPostal.Source,
                    mStreet = NatisPostal.Street,
                    mSuburb = NatisPostal.Suburb,
                    mTown = NatisPostal.Town,
                    mUserDetails = NatisPostal.UserDetails
                };
        }
    }
}