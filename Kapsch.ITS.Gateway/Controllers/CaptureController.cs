using Kapsch.Core.Data;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.ITS.Gateway.Models.Capture;
using Kapsch.ITS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Capture")]
    [UsageLog]
    public class CaptureController : BaseController
    {
        [Route("Session")]
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(SessionsModel))]
        public IHttpActionResult Sessions()
        {
            using (var dbContext = new DataContext())
            {
                var sessions = new List<CaptureRepository.sSessionInfo>();
                string[] headings = null;

                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);
                if (!captureRepository.Sessions(out headings, ref sessions))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok(
                    new SessionsModel
                    {
                        Headings = headings.ToList(),
                        Sessions = sessions.Select(f =>
                            new SessionModel
                            {
                                LocationCode = f.mLocationCode,
                                CameraSessionID = f.mCamSession,
                                MachineID = f.mMachineId,
                                NothingDoneCol = f.mNothingDoneCol,
                                NothingDone = f.mNothingDone,
                                CameraDate = f.mCamDate,
                                CamDateCol = f.mCamDateCol,
                                Columns = f.mColumns
                            })
                            .ToList()
                    });
            }
        }

        [Route("Case/Accept")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(NewCaseModel))]
        public IHttpActionResult acceptCase([FromBody] AcceptCaseModel model, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var captureTypes = new List<CaptureRepository.sCaptureTypes>();
                var caseInfo = new CaptureRepository.sCaseInfo()
                {
                    mVehicleRegNo = model.CaseInfo.VehicleRegisterNumber,
                    mVehicleType = model.CaseInfo.VehicleType,
                    mOffenceDate = model.CaseInfo.OffenceDate,
                    mOffencePlace = model.CaseInfo.OffencePlace,
                    mPrevRejectID = model.CaseInfo.PreviousRejectID,
                    mOffenceSpeed = model.CaseInfo.OffenceSpeed,
                    mOffenceZone = model.CaseInfo.OffenceZone,
                    mImage1 = model.CaseInfo.Image1,
                    mImage2 = model.CaseInfo.Image2,
                    mImage3 = model.CaseInfo.Image3,
                    mImage4 = model.CaseInfo.Image4,
                    mImageNP = model.CaseInfo.ImageNP,
                    mImage1ID = model.CaseInfo.Image1ID,
                    mImage2ID = model.CaseInfo.Image2ID,
                    mImage3ID = model.CaseInfo.Image3ID,
                    mImage4ID = model.CaseInfo.Image4ID,
                    mOnlyOneImage = model.CaseInfo.OnlyOneImage,
                    mPrintImageNo = model.CaseInfo.PrintImageNumber,
                };

                var captureType =
                    new CaptureRepository.sCaptureTypes()
                    {
                        mID = model.CaptureType.ID,
                        mCode = model.CaptureType.Code,
                        mType = model.CaptureType.Type,
                        mAmount = model.CaptureType.Amount,
                        mBeskrywing = model.CaptureType.Beskrywing,
                        mDescription = model.CaptureType.Description
                    };
                var session =
                    new CaptureRepository.sSessionInfo()
                    {
                        mLocationCode = model.Session.LocationCode,
                        mCamDate = model.Session.CameraDate,
                        mCamSession = model.Session.CameraSessionID,
                        mMachineId = model.Session.MachineID,
                        mNothingDoneCol = model.Session.NothingDoneCol,
                        mNothingDone = model.Session.NothingDone,
                        mCamDateCol = model.Session.CamDateCol,
                        mColumns = model.Session.Columns
                    };

                var remoteCaseInfo = new CaptureRepository.sCaseInfo();

                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.CaseAccept(session, model.OffenceSetID, model.FileNumber, model.NextFileNumber, model.OfficerID, model.SheetNumber, model.HasSheetNumberChanged, ref caseInfo, ref remoteCaseInfo, captureType, ref captureTypes, model.PrintImageID, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok(
                    new NewCaseModel
                    {
                        Case = new CaseModel
                        {
                            VehicleRegisterNumber = caseInfo.mVehicleRegNo,
                            VehicleType = caseInfo.mVehicleType,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffencePlace = caseInfo.mOffencePlace,
                            PreviousRejectID = caseInfo.mPrevRejectID,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            Image3 = caseInfo.mImage3,
                            Image4 = caseInfo.mImage4,
                            ImageNP = caseInfo.mImageNP,
                            Image1ID = caseInfo.mImage1ID,
                            Image2ID = caseInfo.mImage2ID,
                            Image3ID = caseInfo.mImage3ID,
                            Image4ID = caseInfo.mImage4ID,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PrintImageNumber = caseInfo.mPrintImageNo,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImage3 = remoteCaseInfo.mImage3,
                            RemoteImage4 = remoteCaseInfo.mImage4,
                            RemoteImageNP = remoteCaseInfo.mImageNP,
                            RemoteImage1ID = remoteCaseInfo.mImage1ID,
                            RemoteImage2ID = remoteCaseInfo.mImage2ID,
                            RemoteImage3ID = remoteCaseInfo.mImage3ID,
                            RemoteImage4ID = remoteCaseInfo.mImage4ID,
                            RemotePrintImageNumber = remoteCaseInfo.mPrintImageNo,
                            IsNumberPlateCentralCaptured = captureRepository.NumberPlateCapturedOnCentral,
                            NumberPlateCentralPath = captureRepository.NumberPlateCapturePath
                        },
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Code = f.mCode, Description = f.mDescription, Beskrywing = f.mBeskrywing, Type = f.mType }).ToList(),
                    });
            }
        }

        [Route("Case/Reject")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(NewCaseModel))]
        public IHttpActionResult RejectCase([FromBody] RejectCaseModel model, string computerName)
        {
            using (var dbContext = new DataContext())
            {
                var captureTypes = new List<CaptureRepository.sCaptureTypes>();
                var caseInfo = new CaptureRepository.sCaseInfo()
                {
                    mVehicleRegNo = model.CaseInfo.VehicleRegisterNumber,
                    mVehicleType = model.CaseInfo.VehicleType,
                    mOffenceDate = model.CaseInfo.OffenceDate,
                    mOffencePlace = model.CaseInfo.OffencePlace,
                    mPrevRejectID = model.CaseInfo.PreviousRejectID,
                    mOffenceSpeed = model.CaseInfo.OffenceSpeed,
                    mOffenceZone = model.CaseInfo.OffenceZone,
                    mImage1 = model.CaseInfo.Image1,
                    mImage2 = model.CaseInfo.Image2,
                    mImage3 = model.CaseInfo.Image3,
                    mImage4 = model.CaseInfo.Image4,
                    mImageNP = model.CaseInfo.ImageNP,
                    mImage1ID = model.CaseInfo.Image1ID,
                    mImage2ID = model.CaseInfo.Image2ID,
                    mImage3ID = model.CaseInfo.Image3ID,
                    mImage4ID = model.CaseInfo.Image4ID,
                    mOnlyOneImage = model.CaseInfo.OnlyOneImage,
                    mPrintImageNo = model.CaseInfo.PrintImageNumber
                };
                var session =
                    new CaptureRepository.sSessionInfo()
                    {
                        mLocationCode = model.Session.LocationCode,
                        mCamDate = model.Session.CameraDate,
                        mCamSession = model.Session.CameraSessionID,
                        mMachineId = model.Session.MachineID,
                        mNothingDoneCol = model.Session.NothingDoneCol,
                        mNothingDone = model.Session.NothingDone,
                        mCamDateCol = model.Session.CamDateCol,
                        mColumns = model.Session.Columns
                    };

                var remoteCaseInfo = new CaptureRepository.sCaseInfo();
                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.CaseReject(session, model.RejectReasonID, model.OffenceSetID, model.FileNumber, model.NextFileNumber, model.OfficerID, model.SheetNumber, model.HasSheetNumberChanged, ref caseInfo, ref remoteCaseInfo, ref captureTypes, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok(
                    new NewCaseModel
                    {
                        Case = new CaseModel
                        {
                            VehicleRegisterNumber = caseInfo.mVehicleRegNo,
                            VehicleType = caseInfo.mVehicleType,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffencePlace = caseInfo.mOffencePlace,
                            PreviousRejectID = caseInfo.mPrevRejectID,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            Image3 = caseInfo.mImage3,
                            Image4 = caseInfo.mImage4,
                            ImageNP = caseInfo.mImageNP,
                            Image1ID = caseInfo.mImage1ID,
                            Image2ID = caseInfo.mImage2ID,
                            Image3ID = caseInfo.mImage3ID,
                            Image4ID = caseInfo.mImage4ID,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PrintImageNumber = caseInfo.mPrintImageNo,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImage3 = remoteCaseInfo.mImage3,
                            RemoteImage4 = remoteCaseInfo.mImage4,
                            RemoteImageNP = remoteCaseInfo.mImageNP,
                            RemoteImage1ID = remoteCaseInfo.mImage1ID,
                            RemoteImage2ID = remoteCaseInfo.mImage2ID,
                            RemoteImage3ID = remoteCaseInfo.mImage3ID,
                            RemoteImage4ID = remoteCaseInfo.mImage4ID,
                            RemotePrintImageNumber = remoteCaseInfo.mPrintImageNo,
                            IsNumberPlateCentralCaptured = captureRepository.NumberPlateCapturedOnCentral,
                            NumberPlateCentralPath = captureRepository.NumberPlateCapturePath
                        },
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Code = f.mCode, Description = f.mDescription, Beskrywing = f.mBeskrywing, Type = f.mType }).ToList(),
                    });
            }
        }

        [Route("Case/First")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(FirstCaseModel))]
        public IHttpActionResult FirstCase(bool onlyNew, string computerName, [FromBody] SessionModel model)
        {
            using (var dbContext = new DataContext())
            {
                var fileNumbers = new List<int>();
                var officerInfos = new List<CaptureRepository.sOfficerInfo>();
                var rejectReasons = new List<CaptureRepository.sRejectReasons>();
                var captureTypes = new List<CaptureRepository.sCaptureTypes>();
                var caseInfo = new CaptureRepository.sCaseInfo();
                var remoteCaseInfo = new CaptureRepository.sCaseInfo();

                int offenceSet;
                int ticketIndex;

                var session =
                    new CaptureRepository.sSessionInfo()
                    {
                        mLocationCode = model.LocationCode,
                        mCamDate = model.CameraDate,
                        mCamSession = model.CameraSessionID,
                        mMachineId = model.MachineID,
                        mNothingDoneCol = model.NothingDoneCol,
                        mNothingDone = model.NothingDone,
                        mCamDateCol = model.CamDateCol,
                        mColumns = model.Columns
                    };

                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.CaseFirst(session, onlyNew, ref rejectReasons, ref captureTypes, ref fileNumbers, ref officerInfos, ref caseInfo, ref remoteCaseInfo, out offenceSet, out ticketIndex, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok(
                    new FirstCaseModel
                    {
                        Case = new CaseModel
                        {
                            VehicleRegisterNumber = caseInfo.mVehicleRegNo,
                            VehicleType = caseInfo.mVehicleType,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffencePlace = caseInfo.mOffencePlace,
                            PreviousRejectID = caseInfo.mPrevRejectID,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            Image3 = caseInfo.mImage3,
                            Image4 = caseInfo.mImage4,
                            ImageNP = caseInfo.mImageNP,
                            Image1ID = caseInfo.mImage1ID,
                            Image2ID = caseInfo.mImage2ID,
                            Image3ID = caseInfo.mImage3ID,
                            Image4ID = caseInfo.mImage4ID,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImage3 = remoteCaseInfo.mImage3,
                            RemoteImage4 = remoteCaseInfo.mImage4,
                            RemoteImageNP = remoteCaseInfo.mImageNP,
                            RemoteImage1ID = remoteCaseInfo.mImage1ID,
                            RemoteImage2ID = remoteCaseInfo.mImage2ID,
                            RemoteImage3ID = remoteCaseInfo.mImage3ID,
                            RemoteImage4ID = remoteCaseInfo.mImage4ID,
                            RemotePrintImageNumber = remoteCaseInfo.mPrintImageNo,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PrintImageNumber = caseInfo.mPrintImageNo,
                            IsNumberPlateCentralCaptured = captureRepository.NumberPlateCapturedOnCentral,
                            NumberPlateCentralPath = captureRepository.NumberPlateCapturePath
                        },
                        FileNumbers = fileNumbers,
                        Officers = officerInfos.Select(f => new OfficerModel { ID = f.mID, ExternalID = f.mExternID, Name = f.mName, Surname = f.mSurname }).ToList(),
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Code = f.mCode, Description = f.mDescription, Beskrywing = f.mBeskrywing, Type = f.mType }).ToList(),
                        OffenceSet = offenceSet,
                        StartIndex = ticketIndex,
                        RejectReasons = rejectReasons.Select(f => new RejectReasonModel { ID = f.mID, Description = f.mDescription }).ToList()
                    });
            }
        }

        [Route("Case/Previous")]
        [HttpPost]
        [SessionAuthorize]
        [ResponseType(typeof(NewCaseModel))]
        public IHttpActionResult PreviousCase(int offenceSetID, int fileNumber, string computerName, [FromBody] SessionModel model)
        {
            using (var dbContext = new DataContext())
            {
                var captureTypes = new List<CaptureRepository.sCaptureTypes>();
                var caseInfo = new CaptureRepository.sCaseInfo();
                var session =
                    new CaptureRepository.sSessionInfo()
                    {
                        mLocationCode = model.LocationCode,
                        mCamDate = model.CameraDate,
                        mCamSession = model.CameraSessionID,
                        mMachineId = model.MachineID,
                        mNothingDoneCol = model.NothingDoneCol,
                        mNothingDone = model.NothingDone,
                        mCamDateCol = model.CamDateCol,
                        mColumns = model.Columns
                    };

                var remoteCaseInfo = new CaptureRepository.sCaseInfo();
                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.CasePrevious(session, offenceSetID, fileNumber, ref caseInfo, ref remoteCaseInfo, ref captureTypes, computerName))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok(
                    new NewCaseModel
                    {
                        Case = new CaseModel
                        {
                            VehicleRegisterNumber = caseInfo.mVehicleRegNo,
                            VehicleType = caseInfo.mVehicleType,
                            OffenceDate = caseInfo.mOffenceDate,
                            OffencePlace = caseInfo.mOffencePlace,
                            PreviousRejectID = caseInfo.mPrevRejectID,
                            OffenceSpeed = caseInfo.mOffenceSpeed,
                            OffenceZone = caseInfo.mOffenceZone,
                            Image1 = caseInfo.mImage1,
                            Image2 = caseInfo.mImage2,
                            Image3 = caseInfo.mImage3,
                            Image4 = caseInfo.mImage4,
                            ImageNP = caseInfo.mImageNP,
                            Image1ID = caseInfo.mImage1ID,
                            Image2ID = caseInfo.mImage2ID,
                            Image3ID = caseInfo.mImage3ID,
                            Image4ID = caseInfo.mImage4ID,
                            OnlyOneImage = caseInfo.mOnlyOneImage,
                            PrintImageNumber = caseInfo.mPrintImageNo,
                            RemoteImage1 = remoteCaseInfo.mImage1,
                            RemoteImage2 = remoteCaseInfo.mImage2,
                            RemoteImage3 = remoteCaseInfo.mImage3,
                            RemoteImage4 = remoteCaseInfo.mImage4,
                            RemoteImageNP = remoteCaseInfo.mImageNP,
                            RemoteImage1ID = remoteCaseInfo.mImage1ID,
                            RemoteImage2ID = remoteCaseInfo.mImage2ID,
                            RemoteImage3ID = remoteCaseInfo.mImage3ID,
                            RemoteImage4ID = remoteCaseInfo.mImage4ID,
                            RemotePrintImageNumber = remoteCaseInfo.mPrintImageNo,
                            IsNumberPlateCentralCaptured = captureRepository.NumberPlateCapturedOnCentral,
                            NumberPlateCentralPath = captureRepository.NumberPlateCapturePath
                        },
                        CaptureTypes = captureTypes.Select(f => new CaptureTypeModel { ID = f.mID, Amount = f.mAmount, Code = f.mCode, Description = f.mDescription, Beskrywing = f.mBeskrywing, Type = f.mType }).ToList(),
                    });
            }
        }

        [Route("Case/Submit")]
        [HttpPut]
        [SessionAuthorize]
        [ResponseType(typeof(SubmitCaseModel))]
        public IHttpActionResult SubmitCase(string cameraDate, string cameraSessionID, string locationCode)
        {
            using (var dbContext = new DataContext())
            {
                int captureTotal;
                int rejectTotal;

                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.CasesSubmit(cameraDate, cameraSessionID, locationCode, out captureTotal, out rejectTotal))
                {
                    return this.BadRequest(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok(
                    new SubmitCaseModel
                    {
                        CaptureTotal = captureTotal,
                        RejectTotal = rejectTotal
                    });
            }
        }

        [Route("Case/Unlock")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult UnlockCase(string cameraDate, string cameraSessionID, string locationCode)
        {
            using (var dbContext = new DataContext())
            {
                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.CaseUnlock(cameraDate, cameraSessionID, locationCode))
                {
                    return this.BadRequest(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok();
            }
        }

        [Route("Case/SaveNP")]
        [HttpPut]
        [SessionAuthorize]
        public IHttpActionResult SaveNPImage (string mimeType, string mimeDataPath, string fileName, int evidenceFileNumber)
        {
            using (var dbContext = new DataContext())
            {
                var captureRepository = new CaptureRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!captureRepository.NPImageSave(mimeType, mimeDataPath, fileName, evidenceFileNumber))
                {
                    return this.BadRequest(Error.PopulateMethodFailed(captureRepository.Error));
                }

                return Ok();
            }
        }
    }
}
