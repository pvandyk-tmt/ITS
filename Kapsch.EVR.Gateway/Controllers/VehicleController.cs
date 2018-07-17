using Kapsch.Core.Data;
using Kapsch.Core.Extensions;
using Kapsch.Core.Filters;
using Kapsch.EVR.Gateway.Models.Vehicle;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.Core.Data.Enums;
using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using TMT.Build.OracleTableTypeClasses;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace Kapsch.EVR.Gateway.Controllers
{
    [RoutePrefix("api/Vehicle")]
    [SessionAuthorize]
    [UsageLog]

    public class VehicleController : BaseController
    {
        public static string DataContextConnectionString = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;

        #region LOOKUP METHOD

        [HttpGet]
        [Route("ModelNumber")]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(VehicleModelNumberModel))]
        public IHttpActionResult GetVehicleModelNumber(long id)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var entity = dbContext.VehicleModelNumbers.Include(f => f.VehicleModel).SingleOrDefault(f => f.ID == id);
                    if (entity == null)
                    {
                        return this.BadRequestEx(Error.VehicleModelNumberDoesNotExist);
                    }

                    return Ok(
                        new VehicleModelNumberModel
                        {
                            ID = entity.ID,
                            Description = entity.Description,
                            VehicleModelID = entity.VehicleModelID,
                            VehicleModelDescription = entity.VehicleModel == null ? string.Empty : entity.VehicleModel.Description,
                            ExternalCode = entity.ExternalCode,
                            ModifiedDate = entity.ModifiedDate,
                        });
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }
        }

        [HttpPut]
        [Route("ModelNumber")]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        public IHttpActionResult PutVehicleModelNumber([FromBody] VehicleModelNumberModel model)
        {
            using (var dbContext = new DataContext())
            {
                var entity = dbContext.VehicleModelNumbers
                    .FirstOrDefault(f =>
                        f.Description.ToUpper() == model.Description.ToUpper() &&
                        f.VehicleModelID == model.VehicleModelID &&
                        f.ID != model.ID);
                if (entity != null)
                {
                    return this.BadRequestEx(Error.VehicleModelNumberAlreadyExist);
                }

                entity = dbContext.VehicleModelNumbers.SingleOrDefault(f => f.ID == model.ID);
                if (entity == null)
                {
                    return this.BadRequestEx(Error.VehicleModelNumberDoesNotExist);
                }

                entity.Description = model.Description;
                entity.ExternalCode = model.ExternalCode;
                entity.VehicleModelID = model.VehicleModelID;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("ModelNumber")]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(VehicleModelNumberModel))]
        public IHttpActionResult PostVehicleModelNumber([FromBody] VehicleModelNumberModel model)
        {
            using (var dbContext = new DataContext())
            {
                var entity = dbContext.VehicleModelNumbers
                    .FirstOrDefault(f =>
                        f.Description.ToUpper() == model.Description.ToUpper() &&
                        f.VehicleModelID == model.VehicleModelID);
                if (entity != null)
                {
                    return this.BadRequestEx(Error.VehicleModelNumberAlreadyExist);
                }

                entity = new Kapsch.Core.Data.VehicleModelNumber();
                //TODO: What todo with entity.ID = 
                entity.Description = model.Description;
                entity.ExternalCode = model.ExternalCode;
                entity.VehicleModelID = model.VehicleModelID;

                dbContext.VehicleModelNumbers.Add(entity);
                dbContext.SaveChanges();
                model.ID = entity.ID;

                return Ok(model);
            }
        }

        [HttpGet]
        [Route("ModelNumber")]
        [ResponseType(typeof(List<VehicleModelNumberModel>))]
        public IHttpActionResult GetVehicleModelNumberByModelID(int vehicleModelID)
        {
            var models = new List<VehicleModelNumberModel>();

            using (var dbContext = new DataContext())
            {
                var entities = dbContext.VehicleModelNumbers
                    .AsNoTracking()
                    .Include(f => f.VehicleModel)
                    .Where(f => f.VehicleModelID == vehicleModelID)
                    .ToList();

                foreach (var entity in entities)
                {
                    var model = new VehicleModelNumberModel();
                    model.ID = entity.ID;
                    model.VehicleModelID = entity.VehicleModelID;
                    model.Description = entity.Description;
                    model.ExternalCode = entity.ExternalCode;
                    model.ModifiedDate = entity.ModifiedDate;

                    models.Add(model);
                }
            }

            return Ok(models);
        }



        [HttpGet]
        [Route("TestCategory")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.TestCategoryModel>))]
        public IHttpActionResult GetTestCategories()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.TestCategoryModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.TestCategories
                        .AsNoTracking()
                        .Where(f => f.ActiveStatusID == 1)
                        .ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.TestCategoryModel();
                        model.ID = entity.ID;
                        model.Name = entity.Name;
                        model.ActiveStatusID = entity.ActiveStatusID;
                        model.CanContinueNoTIS = entity.CanContinueNoTIS;
                        model.IsTISCheckRequired = entity.IsTisCheckRequired;
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }
        


        [HttpGet]
        [Route("Color")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleColorModel>))]
        public IHttpActionResult GetVehicleColors()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleColorModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehicleColors.AsNoTracking().ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleColorModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }
        


        [HttpGet]
        [Route("Propeller")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehiclePropellerModel>))]
        public IHttpActionResult GetPropellers()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehiclePropellerModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehiclePropellers.ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehiclePropellerModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }

        [HttpGet]
        [Route("FuelType")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleFuelTypeModel>))]
        public IHttpActionResult GetFuelType()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleFuelTypeModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehicleFuelType.ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleFuelTypeModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }


        [HttpGet]
        [Route("Make")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleMakeModel>))]
        public IHttpActionResult GetVehicleMakes()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleMakeModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehicleMakes.AsNoTracking().ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleMakeModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }

        [HttpGet]
        [Route("Make")]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(VehicleMakeModel))]
        public IHttpActionResult GetVehicleMake(long id)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var entity = dbContext.VehicleMakes.SingleOrDefault(f => f.ID == id);
                    if (entity == null)
                    {
                        return this.BadRequestEx(Error.VehicleMakeDoesNotExist);
                    }

                    return Ok(
                        new VehicleMake 
                        {
                            ID = entity.ID,
                            Description = entity.Description,
                            ExternalCode = entity.ExternalCode,
                            ModifiedDate = entity.ModifiedDate,
                        });
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }
        }


       [HttpGet]
        [Route("Sites")]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(Core.Gateway.Models.Configuration.SiteModel))]
        public IHttpActionResult GetSitesbyDistrictId(long districtId)
        {
           var models = new List<Core.Gateway.Models.Configuration.SiteModel>();
           try
            {
                using (var dbContext = new DataContext())
                {
                    var entities = dbContext.Sites
                        .AsNoTracking()                    
                        .Where(f => f.DistrictID == districtId && f.SiteTypeID == 1)
                        .ToList();

                    foreach (var entity in entities)
                    {
                        var model = new Core.Gateway.Models.Configuration.SiteModel();
                        model.ID = entity.ID;
                        model.Name = entity.Name;
                        model.SiteTypeID = entity.SiteTypeID;
                        model.DistrictID = entity.DistrictID;
                        model.DistrictName = entity.District.BranchName;

                        models.Add(model);
                    }

                     return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }
        }

        
        [HttpPut]
        [Route("Make")]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        public IHttpActionResult PutVehicleMake([FromBody] VehicleMakeModel model)
        {
            using (var dbContext = new DataContext())
            {
                var entity = dbContext.VehicleMakes.FirstOrDefault(f => f.Description.ToUpper() == model.Description.ToUpper() && f.ID != model.ID);
                if (entity != null)
                {
                    return this.BadRequestEx(Error.VehicleMakeAlreadyExist);
                }

                entity = dbContext.VehicleMakes.SingleOrDefault(f => f.ID == model.ID);
                if (entity == null)
                {
                    return this.BadRequestEx(Error.VehicleMakeDoesNotExist);
                }

                entity.Description = model.Description;
                entity.ExternalCode = model.ExternalCode;

                dbContext.SaveChanges();

                return Ok(); 
            }
        }

        [HttpPost]
        [Route("Make")]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(VehicleMakeModel))]
        public IHttpActionResult PostVehicleMake([FromBody] VehicleMakeModel model)
        {
            using (var dbContext = new DataContext())
            {

                var entity = dbContext.VehicleMakes.FirstOrDefault(f => f.Description.ToUpper() == model.Description.ToUpper());
                if (entity != null)
                {
                    return this.BadRequestEx(Error.VehicleMakeAlreadyExist);
                }

                entity = new Kapsch.Core.Data.VehicleMake();
                //TODO: What todo with entity.ID = 
                entity.Description = model.Description;
                entity.ExternalCode = model.ExternalCode;

                dbContext.VehicleMakes.Add(entity);
                dbContext.SaveChanges();
                model.ID = entity.ID;

                return Ok(model);
            }
        }

        [HttpGet]
        [Route("Model")]
        [ResponseType(typeof(List<VehicleModelModel>))]
        public IHttpActionResult GetVehicleModelsByMakeID(int vehicleMakeID)
        {
            var models = new List<VehicleModelModel>();

            using (var dbContext = new DataContext())
            {
                var entities = dbContext.VehicleModels
                    .AsNoTracking()
                    .Include(f => f.VehicleMake)
                    .Where(f => f.VehicleMakeID == vehicleMakeID)
                    .ToList();

                foreach (var entity in entities)
                {
                    var model = new VehicleModelModel();
                    model.ID = entity.ID;
                    model.VehicleMakeID = entity.VehicleMakeID;
                    model.Description = entity.Description;
                    model.ExternalCode = entity.ExternalCode;
                    model.ModifiedDate = entity.ModifiedDate;

                    models.Add(model);
                }
            }

            return Ok(models);
        }

        [HttpGet]
        [Route("Model")]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(VehicleModelModel))]
        public IHttpActionResult GetVehicleModel(long id)
        {
            try
            {
                using (var dbContext = new DataContext())
                {
                    var entity = dbContext.VehicleModels.Include(f => f.VehicleMake).SingleOrDefault(f => f.ID == id);
                    if (entity == null)
                    {
                        return this.BadRequestEx(Error.VehicleModelDoesNotExist);
                    }

                    return Ok(
                        new VehicleModelModel 
                        {
                            ID = entity.ID,
                            Description = entity.Description,
                            VehicleMakeID = entity.VehicleMakeID,
                            VehicleMakeDescription = entity.VehicleMake == null ? string.Empty : entity.VehicleMake.Description,
                            ExternalCode = entity.ExternalCode,
                            ModifiedDate = entity.ModifiedDate,
                        });
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }
        }

        [HttpPut]
        [Route("Model")]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        public IHttpActionResult PutVehicleModel([FromBody] VehicleModelModel model)
        {
            using (var dbContext = new DataContext())
            {
                var entity = dbContext.VehicleModels
                    .FirstOrDefault(f => 
                        f.Description.ToUpper() == model.Description.ToUpper() && 
                        f.VehicleMakeID == model.VehicleMakeID && 
                        f.ID != model.ID);
                if (entity != null)
                {
                    return this.BadRequestEx(Error.VehicleModelAlreadyExist);
                }

                entity = dbContext.VehicleModels.SingleOrDefault(f => f.ID == model.ID);
                if (entity == null)
                {
                    return this.BadRequestEx(Error.VehicleModelDoesNotExist);
                }

                entity.Description = model.Description;
                entity.ExternalCode = model.ExternalCode;
                entity.VehicleMakeID = model.VehicleMakeID;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("Model")]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(VehicleModelModel))]
        public IHttpActionResult PostVehicleModel([FromBody] VehicleModelModel model)
        {
            using (var dbContext = new DataContext())
            {
                var entity = dbContext.VehicleModels
                    .FirstOrDefault(f => 
                        f.Description.ToUpper() == model.Description.ToUpper() && 
                        f.VehicleMakeID == model.VehicleMakeID);
                if (entity != null)
                {
                    return this.BadRequestEx(Error.VehicleModelAlreadyExist);
                }

                entity = new Kapsch.Core.Data.VehicleModel();
                entity.Description = model.Description;
                entity.ExternalCode = model.ExternalCode;
                entity.VehicleMakeID = model.VehicleMakeID;

                dbContext.VehicleModels.Add(entity);
                dbContext.SaveChanges();
                model.ID = entity.ID;

                return Ok(model);
            }
        }

        [HttpGet]
        [Route("LookUp")]
        [ResponseType(typeof(List<VehicleModelModel>))]
        public IHttpActionResult GetVehicleLookUps(bool includeMake, bool includeModel, bool includeModelNumber)
        {
            var vehicleLookUps = new VehicleLookUpsModel();
            var vehicleModelNumbers = new List<VehicleModelNumberModel>();
            var vehicleMakes = new List<Models.Vehicle.VehicleMakeModel>();
            var vehicleModels = new List<VehicleModelModel>();

            using (var dbContext = new DataContext())
            {
                if (includeMake)
                {
                    var entities = dbContext.VehicleMakes
                        .ToList();

                    foreach (var entity in entities)
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleMakeModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        vehicleMakes.Add(model);
                    }

                    vehicleLookUps.vehicleMakes = vehicleMakes;
                }

                if (includeModel)
                {
                    foreach (var entity in dbContext.VehicleModels.AsNoTracking().ToList())
                    {
                        var model = new VehicleModelModel();
                        model.ID = entity.ID;
                        model.VehicleMakeID = entity.VehicleMakeID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        vehicleModels.Add(model);
                    }

                    vehicleLookUps.vehicleModels = vehicleModels;
                }

                if (includeModelNumber)
                {
                    var entities = dbContext.VehicleModelNumbers
                        .AsNoTracking()
                        .ToList();

                    foreach (var entity in entities)
                    {
                        var model = new VehicleModelNumberModel();
                        model.ID = entity.ID;
                        model.VehicleModelID = entity.VehicleModelID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        vehicleModelNumbers.Add(model);
                    }
                }
            }

            vehicleLookUps.vehicleMakes = vehicleMakes;
            vehicleLookUps.vehicleModels = vehicleModels;
            vehicleLookUps.vehicleModelNumbers = vehicleModelNumbers;

            return Ok(vehicleLookUps);
        }

        [HttpGet]
        [Route("Category")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleCategoryModel>))]
        public IHttpActionResult GetVehicleCategories()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleCategoryModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehicleCategories.AsNoTracking().ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleCategoryModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;
                        model.ExternalCode = entity.ExternalCode;
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }

        [HttpGet]
        [Route("VehicleTypes")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleTypeModel>))]
        public IHttpActionResult GetVehicleTypes()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleTypeModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehicleTypes.AsNoTracking().ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleTypeModel();
                        model.ID = entity.ID;
                        model.Description = entity.Description;                        
                        model.ModifiedDate = entity.ModifiedDate;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }

        #endregion



        #region QuestionAnswerResults

        [HttpPost]
        [Route("Question")]
        [ResponseType(typeof(BookingQuestionAnswerModel))]
        public IHttpActionResult GetQuestionAnswers(string bookingRef)
        {
            var model = new BookingQuestionAnswerModel();            

            try
            {
                using (var dbContext = new DataContext())
                {
                    var connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;

                    using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand())
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        command.Parameters.Add("p_booking_reference", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2).Value = bookingRef;
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("o_questions", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("o_answers", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("o_booking_detail", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("o_is_successful", Oracle.ManagedDataAccess.Client.OracleDbType.Int32)).Direction = ParameterDirection.Output;
                        

                        DataSet dsData = new DataSet();

                        try
                        {
                            dsData = ExecuteQuery(command, "TIS.VEHICLE_TESTING.get_test_questions_answers", connection);

                            if (dsData.Tables.Count > 0)
                            {

                                model.Questions = new List<QuestionModel>();

                                if (dsData.Tables[0].Rows.Count > 0)
                                {

                                    foreach (DataRow bookingDetail in dsData.Tables[2].Rows)
                                    {   
                                        model.BookingID = bookingDetail["BOOKING_ID"] == DBNull.Value ? 0 : Convert.ToInt32(bookingDetail["BOOKING_ID"]);
                                        model.TestTypeID = bookingDetail["TEST_TYPE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(bookingDetail["TEST_TYPE_ID"]);
                                        model.SiteName = bookingDetail["SITE_NAME"].ToString();
                                        model.SiteID = bookingDetail["SITE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(bookingDetail["SITE_ID"]);
                                        model.TestTypeDescription = bookingDetail["TEST_DESCRIPTION"].ToString();
                                        model.TestCategory = bookingDetail["TEST_CATEGORY"].ToString();
                                        model.BarcodeData = bookingDetail["BARCODE"].ToString();
                                        model.IsPhotoRequired = Convert.ToInt32(bookingDetail["IS_PHOTO_REQUIRED"]) == 0 ? false : true;
                                    }

                                    //if (!(command.Parameters["o_is_successful"].Value is DBNull))
                                    //{
                                    //    model.IsSuccessfull = Convert.ToInt32(command.Parameters["o_is_successful"].Value);                                        
                                    //}


                                    foreach (DataRow questionRow in dsData.Tables[0].Rows)
                                    {
                                        var question = new QuestionModel();

                                        question.ComparedValue = questionRow["COMPAREDVALUE"].ToString();
                                        question.Criteria = questionRow["CRITERIA"].ToString();
                                        question.HasComment = Convert.ToInt32(questionRow["HASCOMMENT"]) == 0 ? false : true;
                                        question.IsCompared = Convert.ToInt32(questionRow["ISCOMPARED"]) == 0 ? false : true;
                                        question.IsDoubleEntry = Convert.ToInt32(questionRow["ISDOUBLEENTRY"]) == 0 ? false : true;
                                        question.IsReadOnly = Convert.ToInt32(questionRow["ISREADONLY"]) == 0 ? false : true;
                                        question.QuestionTypeID = questionRow["QUESTIONTYPEID"] == DBNull.Value ? 0 : Convert.ToInt32(questionRow["QUESTIONTYPEID"]);
                                        question.TestQuestionID = questionRow["TESTQUESTIONID"] == DBNull.Value ? 0 : Convert.ToInt32(questionRow["TESTQUESTIONID"]);                                        
                                        question.ValuePattern = questionRow["VALUEPATTERN"].ToString();
                                        question.TestQuestionDescription = questionRow["TESTQUESTIONDESCRIPTION"].ToString();
                                        question.CorrectQuestionAnswerID = questionRow["CORRECTQUESTIONANSWERID"].ToString();
                                        question.Weight = questionRow["WEIGHT"] == DBNull.Value ? 0 : Convert.ToInt32(questionRow["WEIGHT"]);

                                        foreach (DataRow answerRow in dsData.Tables[1].Select("TESTQUESTIONID=" + questionRow["TESTQUESTIONID"].ToString()))
                                        {
                                            var answer = new AnswerModel();
                                            answer.NextQuestionID = answerRow["NEXTQUESTIONID"] == DBNull.Value ? 0 : Convert.ToInt32(answerRow["NEXTQUESTIONID"]);
                                            answer.QuestionAnswerDescription = answerRow["QUESTIONANSWERDESCRIPTION"].ToString();
                                            answer.RelationshipID = answerRow["RELATIONSHIPID"] == DBNull.Value ? 0 : Convert.ToInt32(answerRow["RELATIONSHIPID"]);
                                            answer.TestQuestionAnswerID = answerRow["TESTQUESTIONANSWERID"] == DBNull.Value ? 0 : Convert.ToInt32(answerRow["TESTQUESTIONANSWERID"]);
                                            answer.TestQuestionID = answerRow["TESTQUESTIONID"] == DBNull.Value ? 0 : Convert.ToInt32(answerRow["TESTQUESTIONID"]);
                                            answer.DisplayColour = answerRow["DISPLAYCOLOUR"].ToString();
                                            answer.IsCommentRequired = answerRow["ISCOMMENTREQUIRED"] == DBNull.Value ? false : Convert.ToInt32(answerRow["ISCOMMENTREQUIRED"]) == 0 ? false : true;
                                            question.Answers.Add(answer);
                                        }

                                        model.Questions.Add(question);
                                    }
                                }

                            }
                            else
                            {
                                throw new Exception("No booking data found for booking reference - " + bookingRef);
                            }
                        }
                        catch (Exception ex)
                        {
                            return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }

            return Ok(model);
        }



        [HttpPost]
        [Route("Result")]
        [ResponseType(typeof(QuestionAnswerResultModel))]
        public IHttpActionResult SaveTestResults(QuestionAnswerResultModel model)
        {
            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                {
                    var command = new Oracle.DataAccess.Client.OracleCommand();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var resultsArray = model.questionAnswerResults.Select(f =>
                        new TMT.Build.OracleTableTypeClasses.QuestionAnswerResult
                        {
                            Comments = f.Comments,
                            ID = f.ID,
                            TestQuestionsAnswersID = f.TestQuestionsAnswersID,
                            TestQuestionsAnswersIDRelID = f.TestQuestionsAnswersIDRelID,
                            TestQuestionsID = f.TestQuestionsID,
                            TestTypeID = f.TestTypeID,
                            TextAnswer = f.TextAnswer,
                            VehicleTestBookingID = f.VehicleTestBookingID,
                            IsPassed = f.IsPassed
                        })
                        .ToArray();

                    //ORACLE DATA TYPE INPUT
                    Oracle.DataAccess.Client.OracleParameter par = new Oracle.DataAccess.Client.OracleParameter("P_TEST_RESULT_TYPE", Oracle.DataAccess.Client.OracleDbType.Array);
                    par.Direction = ParameterDirection.Input;
                    par.Value = resultsArray;
                    par.UdtTypeName = "TIS.TABLE_VEHICLE_TEST_RESULT_TYPE";
                    command.Parameters.Add(par);

                    command.Parameters.Add("P_INSPECTOR_CREDENTIAL_ID", Oracle.DataAccess.Client.OracleDbType.Int32).Value = model.inspectorID;
                    command.Parameters.Add("P_OVERALL_IS_PASSED", Oracle.DataAccess.Client.OracleDbType.Int32).Value = model.isPassed == true ? 1 : 0;
                    command.Parameters.Add("P_TEST_STARTED_AT", Oracle.DataAccess.Client.OracleDbType.Date).Value = model.TestStartTime;
                    command.Parameters.Add("P_TEST_ENDED_AT", Oracle.DataAccess.Client.OracleDbType.Date).Value = model.TestEndTime;
                    

                    try
                    {
                        ExecuteNonQuery(command, "TIS.VEHICLE_TESTING.SUBMIT_TEST_RESULTS", connection);
                        model.isSaved = true;
                    }
                    catch (Exception ex)
                    {
                        model.isSaved = false;
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }

            return Ok(model);
        }
        #endregion

        

        #region VehicleAndBookings

        [HttpPost]
        [Route("Booking")]
        [ResponseType(typeof(VehicleBookingRecordModel))]
        public IHttpActionResult Booking(VehicleBookingRecordModel model)
        {
            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                {
                    var command = new Oracle.DataAccess.Client.OracleCommand();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var vehicleDetails = new TMT.Build.OracleTableTypeClasses.VehicleTestBooking();
                    vehicleDetails.ColourID = model.VehicleDetails.ColourID;
                    vehicleDetails.EngineNumber = model.VehicleDetails.EngineNumber;
                    vehicleDetails.GVM = model.VehicleDetails.GVM;

                    vehicleDetails.InsuranceExpiryDate = model.VehicleDetails.InsuranceExpiryDate == "" ? (DateTime?)null : DateTime.ParseExact(model.VehicleDetails.InsuranceExpiryDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    vehicleDetails.LicenceExpiryDate = model.VehicleDetails.LicenceExpiryDate == "" ? (DateTime?)null : DateTime.ParseExact(model.VehicleDetails.LicenceExpiryDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    vehicleDetails.RoadworthyExpiryDate = model.VehicleDetails.RoadworthyExpiryDate  == "" ? (DateTime?)null : DateTime.ParseExact(model.VehicleDetails.RoadworthyExpiryDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    vehicleDetails.NetWeight = model.VehicleDetails.NetWeight;
                    vehicleDetails.PropelledByID = model.VehicleDetails.PropelledByID;
                    vehicleDetails.FuelTypeID = model.VehicleDetails.FuelTypeID;
                    vehicleDetails.RegistrationStatusID = model.VehicleDetails.RegistrationStatusID;
                    
                    vehicleDetails.VehicleCategoryID = model.VehicleDetails.VehicleCategoryID;
                    vehicleDetails.VehicleMakeID = model.VehicleDetails.VehicleMakeID;
                    vehicleDetails.VehicleModelID = model.VehicleDetails.VehicleModelID;
                    vehicleDetails.VehicleModelNumberID = model.VehicleDetails.VehicleModelNumberID;
                    vehicleDetails.VehicleTypeID = model.VehicleDetails.VehicleTypeID;
                    vehicleDetails.VIN = model.VehicleDetails.VIN;
                    vehicleDetails.VLN = model.VehicleDetails.VLN;
                    vehicleDetails.YearOfMake = model.VehicleDetails.YearOfMake;
                    vehicleDetails.SeatingCapacity = model.VehicleDetails.SeatingCapacity;

                    Oracle.DataAccess.Client.OracleParameter par = new Oracle.DataAccess.Client.OracleParameter("P_VEHICLE_DETAIL_TYPE", Oracle.DataAccess.Client.OracleDbType.Object);
                    par.Direction = ParameterDirection.Input;
                    par.Value = vehicleDetails;
                    par.UdtTypeName = "TIS.VEHICLE_DETAIL_TYPE";
                    command.Parameters.Add(par);

                    OracleParameter pP_TEST_CATEGORY_ID = new OracleParameter();
                    pP_TEST_CATEGORY_ID.ParameterName = "P_TEST_CATEGORY_ID";
                    pP_TEST_CATEGORY_ID.OracleDbType = OracleDbType.Int32;
                    pP_TEST_CATEGORY_ID.Size = 8;
                    pP_TEST_CATEGORY_ID.Direction = ParameterDirection.Input;
                    pP_TEST_CATEGORY_ID.Value = model.TestCategoryID;
                    command.Parameters.Add(pP_TEST_CATEGORY_ID);

                    OracleParameter pP_CREDENTIAL_ID = new OracleParameter();
                    pP_CREDENTIAL_ID.ParameterName = "P_CREDENTIAL_ID";
                    pP_CREDENTIAL_ID.OracleDbType = OracleDbType.Int32;
                    pP_CREDENTIAL_ID.Size = 8;
                    pP_CREDENTIAL_ID.Direction = ParameterDirection.Input;
                    pP_CREDENTIAL_ID.Value = model.CredentialID;
                    command.Parameters.Add(pP_CREDENTIAL_ID);

                    OracleParameter pP_Booking_Reference = new OracleParameter();
                    pP_Booking_Reference.ParameterName = "P_BOOKING_REFERENCE";
                    pP_Booking_Reference.OracleDbType = OracleDbType.Varchar2;
                    //pP_Booking_Reference.Size = 50;
                    pP_Booking_Reference.Direction = ParameterDirection.Input;
                    pP_Booking_Reference.Value = model.BookingReference;
                    command.Parameters.Add(pP_Booking_Reference);

                    OracleParameter pP_SITE_ID = new OracleParameter();
                    pP_SITE_ID.ParameterName = "P_SITE_ID";
                    pP_SITE_ID.OracleDbType = OracleDbType.Int32;
                    pP_SITE_ID.Size = 8;
                    pP_SITE_ID.Direction = ParameterDirection.Input;
                    pP_SITE_ID.Value = model.SiteId;
                    command.Parameters.Add(pP_SITE_ID);

                    OracleParameter pO_SUCCESSFUL_INDICATOR = new OracleParameter();
                    pO_SUCCESSFUL_INDICATOR.ParameterName = "O_SUCCESSFUL_INDICATOR";
                    pO_SUCCESSFUL_INDICATOR.OracleDbType = OracleDbType.Int32;
                    pO_SUCCESSFUL_INDICATOR.Size = 8;
                    pO_SUCCESSFUL_INDICATOR.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pO_SUCCESSFUL_INDICATOR);


                    OracleParameter pMessage = new OracleParameter();
                    pMessage.ParameterName = "O_MESSAGE";
                    pMessage.OracleDbType = OracleDbType.Varchar2;
                    pMessage.Size = 200;
                    pMessage.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pMessage);



                    //try
                    //{
                    ExecuteNonQuery(command, "TIS.VEHICLE_TESTING.CREATE_TEST_BOOKING", connection);

                    if (!(command.Parameters["O_SUCCESSFUL_INDICATOR"].Value is DBNull))
                    {
                        int success = int.Parse(command.Parameters["O_SUCCESSFUL_INDICATOR"].Value.ToString());

                        model.IsSuccessfull = Convert.ToDecimal(success);
                    }

                    if (!(command.Parameters["O_MESSAGE"].Value is DBNull))
                    {
                        var message = command.Parameters["O_MESSAGE"].Value;
                        model.Message = message.ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
            }

            return Ok(model);
        }



        [HttpGet]
        [Route("Bookings")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestResultModel>))]
        public IHttpActionResult GetBookings()
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestBookingModel>();

                using (var dbContext = new DataContext())
                {
                    foreach (var entity in dbContext.VehicleTestBookings.AsNoTracking().ToList())
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestBookingModel();
                        model.ID = entity.ID;
                        model.VehicleDetailID = entity.VehicleDetailID;
                        model.BookingReference = entity.BookingReference;
                        model.TestDate = entity.TestDate;
                        model.IsPassed = entity.IsPassed;
                        model.TestTypeID = entity.TestTypeID;
                        model.CapturedCredentialID = entity.CapturedCredentialID;
                        model.CapturedDate = entity.CapturedDate;
                        model.SiteID = entity.SiteID;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }            
        }

        

        [HttpGet]
        [Route("TestResult")]
        [ResponseType(typeof(List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestResultModel>))]
        public IHttpActionResult GetTestResult(int VehicleTestBookingID)
        {
            try
            {
                var models = new List<Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestResultModel>();

                using (var dbContext = new DataContext())
                {
                    var entities = dbContext.VehicleTestResults
                        .AsNoTracking()
                        .Where(f => f.VehicleTestBookingID == VehicleTestBookingID)
                        .ToList();

                    foreach (var entity in entities)
                    {
                        var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestResultModel();
                        model.ID = entity.ID;
                        model.Comments = entity.Comments;
                        model.TestQuestionsAnswersID = entity.TestQuestionsAnswersID;
                        model.TestQuestionsAnswersRelID = entity.TestQuestionsAnswersRelID;
                        model.TestQuestionsID = entity.TestQuestionsID;
                        model.TestTypeID = entity.TestTypeID;
                        model.VehicleTestBookingID = entity.VehicleTestBookingID;

                        models.Add(model);
                    }

                    return Ok(models);
                }
            }
            catch (Exception ex)
            {
                return Ok(string.Format("{0} {1}", ex.Message, ex.InnerException));
            }
        }


        //[HttpGet]
        //[Route("VehicleDetails")]
        //[ResponseType(typeof(Kapsch.EVR.Gateway.Models.Vehicle.VehicleModel)]
        //public IHttpActionResult GetVehicleDetailsByID(int VehicleID)
        //{
        //    var model = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleModel();

        //    try
        //    {
        //        using (var dbContext = new DataContext())
        //        {
        //            var connection = (Oracle.ManagedDataAccess.Client.OracleConnection)dbContext.Database.Connection;

        //            using (var command = new Oracle.ManagedDataAccess.Client.OracleCommand())
        //            {
        //                if (connection.State != ConnectionState.Open)
        //                {
        //                    connection.Open();
        //                }

        //                command.Parameters.Add("p_booking_reference", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2).Value = VehicleID;
        //                command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("o_questions", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor)).Direction = ParameterDirection.Output;                        
        //                command.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("o_is_successful", Oracle.ManagedDataAccess.Client.OracleDbType.Int32)).Direction = ParameterDirection.Output;

        //                DataSet dsData = new DataSet();

        //                try
        //                {
        //                    dsData = ExecuteQuery(command, "TIS.VEHICLE_TESTING.get_test_questions_answers", connection);

        //                    if (dsData.Tables.Count > 0)
        //                    {
        //                        if (dsData.Tables[0].Rows.Count > 0)
        //                        {
        //                            foreach (DataRow entity in dsData.Tables[0].Rows)
        //                            {
        //                                model.VehicleCategoryId = entity[0].ToString();

                                        
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("No vehicle data found for vehicle ID - " + VehicleID);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
        //    }

        //    return Ok(model);
        //}


        [HttpPost]
        [Route("Vehicle")]
        [SessionAuthorize]
        [UsageLog]
        [ValidationActionFilter]
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult CreateVehicle(Vehicle model)
        {
            using (var dbContext = new DataContext())
            {
                var vehicle = new Vehicle();
                vehicle.VehicleIDNumber = model.VehicleIDNumber;
                vehicle.EngineNumber = model.EngineNumber;
                vehicle.VehicleCategoryId = model.VehicleCategoryId;
                vehicle.VehicleTypeId = model.VehicleTypeId;
                vehicle.VehicleMakeId = model.VehicleMakeId;
                vehicle.VehicleModelId = model.VehicleModelId;
                vehicle.VehicleModelNumberId = model.VehicleModelNumberId;
                vehicle.ColourId = model.ColourId;
                vehicle.YearOfMake = model.YearOfMake;
                vehicle.VLN = model.VLN;
                vehicle.NetWeight = model.NetWeight;
                vehicle.ProplelledById = model.ProplelledById;
                vehicle.RegistrationStatusId = model.RegistrationStatusId;
                vehicle.LicenceExpiryDate = model.LicenceExpiryDate;
                vehicle.RoadworthyExpiryDate = model.RoadworthyExpiryDate;
                vehicle.InsuranceExpiryDate = model.InsuranceExpiryDate;
                vehicle.CapturedCredentialId = model.CapturedCredentialId;

                dbContext.Vehicles.Add(vehicle);
                dbContext.SaveChanges();

                model.ID = vehicle.ID;
                model.EngineNumber = vehicle.EngineNumber;

                return Ok(model);
            }
        }


        #endregion


        [HttpPost]
        [Route("PaginationList")]
        [ResponseType(typeof(PaginationListModel<Models.Vehicle.VehicleModel>))]
        public IHttpActionResult GetVehiclePaginationList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Vehicle>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .Vehicles
                    .Include(f => f.TestBookings)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<Models.Vehicle.VehicleModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<Models.Vehicle.VehicleModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Vehicle>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<Models.Vehicle.VehicleModel>();

                paginationList.Models = entities.Select(f =>
                    new Models.Vehicle.VehicleModel
                    {
                        ID = f.ID,
                        VIN = f.VehicleIDNumber,
                        EngineNumber = f.EngineNumber,
                        VehicleCategoryId = f.VehicleCategoryId,
                        VehicleTypeId = f.VehicleTypeId,
                        VehicleMakeId = f.VehicleMakeId,
                        VehicleModelId = f.VehicleModelId,
                        VehicleModelNumberId = f.VehicleModelNumberId,
                        VehicleColourId = f.ColourId,
                        YearOfMake = f.YearOfMake,
                        VLN = f.VLN,
                        NetWeight = f.NetWeight,
                        GVM = f.GVM,
                        PropelledById = f.ProplelledById,
                        SeatingCapacity = f.SeatingCapacity,
                        FuelTypeId = f.FuelTypeId,
                        RegistrationStatusId = f.RegistrationStatusId,
                        LicenceExpiryDate = f.LicenceExpiryDate ,
                        RoadworthyExpiryDate = f.RoadworthyExpiryDate,
                        InsuranceExpiryDate = f.InsuranceExpiryDate,
                        CapturedDate = f.CapturedDate,
                        CapturedCredentialId = f.CapturedCredentialId,
                        TestBookings = f.TestBookings.Select(g => new VehicleBookingModel {  BookingReference = g.BookingReference }).ToList()
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }
        


        [HttpPost]
        [Route("BookingsPaginationList")]
        [ResponseType(typeof(PaginationListModel<Models.Vehicle.VehicleTestBookingModel>))]
        public IHttpActionResult GetBookingsPaginationList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Kapsch.Core.Data.VehicleTestBooking>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .VehicleTestBookings
                    .Include(f => f.VehicleDetailID)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<Models.Vehicle.VehicleTestBookingModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<Models.Vehicle.VehicleTestBookingModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;

                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Core.Data.VehicleTestBooking>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<Models.Vehicle.VehicleTestBookingModel>();

                paginationList.Models = entities.Select(f =>
                    new Models.Vehicle.VehicleTestBookingModel
                    {
                        ID = f.ID,
                        VehicleDetailID = f.VehicleDetailID,
                        BookingReference = f.BookingReference,
                        TestDate = f.TestDate,
                        IsPassed = f.IsPassed,
                        TestTypeID = f.TestTypeID,
                        CapturedCredentialID = f.CapturedCredentialID,
                        CapturedDate = f.CapturedDate,
                        SiteID = f.SiteID
                    })
                    .ToList();

                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }



        [HttpPost]
        [Route("TestResultsPaginationList")]
        [ResponseType(typeof(PaginationListModel<Models.Vehicle.TestResultRecordModel>))]
        public IHttpActionResult GetTestResultsPaginationList(int VehicleTestBookingID,
            [FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            Oracle.DataAccess.Client.OracleDataReader reader;
            var paginationList = new PaginationListModel<Models.Vehicle.TestResultRecordModel>();

            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                {
                    var command = new Oracle.DataAccess.Client.OracleCommand();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    OracleParameter p_booking_id = new OracleParameter();
                    p_booking_id.ParameterName = "p_booking_id";
                    p_booking_id.OracleDbType = OracleDbType.Int32;
                    p_booking_id.Size = 8;
                    p_booking_id.Value = VehicleTestBookingID;
                    p_booking_id.Direction = ParameterDirection.Input;
                    command.Parameters.Add(p_booking_id);

                    OracleParameter pResults = new OracleParameter();
                    pResults.ParameterName = "o_result";
                    pResults.OracleDbType = OracleDbType.RefCursor;
                    pResults.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pResults);

                    var testResultRecords = new List<TestResultRecordModel>();


                    try
                    {
                        reader = ExecuteUnManagedReader(command, "TIS.VEHICLE_TESTING.GET_RESULTS", connection);

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var testRecordResult = new TestResultRecordModel();

                                testRecordResult.Comments = reader["COMMENTS"].ToString();
                                testRecordResult.TestQuestionDescription = reader["TEST_QUESTION_DESCRIPTION"].ToString();
                                testRecordResult.TestQuestionResult = reader["TEST_QUESTION_RESULT"].ToString();
                                testRecordResult.TestTypeDescription = reader["TEST_TYPE_DESCRIPTION"].ToString();
                                testRecordResult.TextAnswer = reader["TEXT_ANSWER"].ToString();
                                
                                testResultRecords.Add(testRecordResult);
                            }

                            paginationList.Models = testResultRecords;
                            paginationList.TotalCount = testResultRecords.Count();

                        }
                    }
                    catch (Exception ex)
                    {
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }

            return Ok(paginationList);
        }

        [HttpGet]
        [Route("Test")]
        [ResponseType(typeof(Models.Vehicle.VehicleTestModel))]
        public IHttpActionResult GetVehicleTest(int vehicleBookingID)
        {
            using (var dbContext = new DataContext())
            {
                // dbContext.Database.Log = s => Debug.WriteLine(s);
                var vehicleBooking = dbContext.VehicleTestBookings
                    .AsNoTracking()
                    .Include(f => f.CapturedCredential)
                    .Include(f => f.CapturedCredential.User)
                    .Include(f => f.Site)
                    .Include(f => f.Vehicle.VehicleMake)
                    .Include(f => f.Vehicle.VehicleModel)
                    .Include(f => f.Vehicle.VehicleModelNumber)
                    .Include(f => f.Vehicle.VehicleColor)
                    .FirstOrDefault(f => f.ID == vehicleBookingID);

                var vehicleCategoryType = dbContext.VehicleCategoryTestTypes
                    .Include(f => f.TestCategory)
                    .Include(f => f.TestType)
                    .Include(f => f.VehicleCategory)
                    .First(f =>
                        f.TestTypeID == vehicleBooking.TestTypeID &&
                        f.VehicleCategoryID == vehicleBooking.Vehicle.VehicleCategoryId);

                var model = new Models.Vehicle.VehicleTestModel();
                model.BookingID = vehicleBooking.ID;
                model.BookingReference = vehicleBooking.BookingReference;
                model.ColourID = vehicleBooking.Vehicle.VehicleColor.ID;
                model.ColourName = vehicleBooking.Vehicle.VehicleColor.Description;
                model.EndedTimestamp = vehicleBooking.EndedTimestamp;
                model.EngineNumber = vehicleBooking.Vehicle.EngineNumber;
                model.GVM = vehicleBooking.Vehicle.GVM;
                model.InsuranceExpiryDate = vehicleBooking.Vehicle.InsuranceExpiryDate;
                model.LicenceExpiryDate = vehicleBooking.Vehicle.LicenceExpiryDate;
                model.NetWeight = vehicleBooking.Vehicle.NetWeight;
                model.RoadworthyExpiryDate = vehicleBooking.Vehicle.RoadworthyExpiryDate;
                model.SiteID = (int)vehicleBooking.SiteID;
                model.SiteName = vehicleBooking.Site.Name;
                model.StartedTimestamp = vehicleBooking.StartedTimestamp;
                model.VehicleCategoryID = vehicleCategoryType.VehicleCategoryID;
                model.VehicleCategoryName = vehicleCategoryType.VehicleCategory.Description;
                model.VehicleMakeID = vehicleBooking.Vehicle.VehicleMakeId;
                model.VehicleMakeName = vehicleBooking.Vehicle.VehicleMake.Description;
                model.VehicleModelID = vehicleBooking.Vehicle.VehicleModelId;
                model.VehicleModelName = vehicleBooking.Vehicle.VehicleModel.Description;
                model.VehicleModelNumberID = vehicleBooking.Vehicle.VehicleModelNumberId;
                model.VehicleModelNumberName = vehicleBooking.Vehicle.VehicleModelNumber.Description;
                model.VehicleTypeID = vehicleBooking.Vehicle.VehicleTypeId;
                model.HasPassed = vehicleBooking.IsPassed == 1;
                model.VIN = vehicleBooking.Vehicle.VehicleIDNumber;
                model.VLN = vehicleBooking.Vehicle.VLN;
                model.YearOfMake = vehicleBooking.Vehicle.YearOfMake;
                model.TestCategoryID = vehicleCategoryType.TestCategoryID;
                model.TestCategoryName = vehicleCategoryType.TestCategory.Name;
                model.UserFullName = vehicleBooking.CapturedCredential.User == null ? string.Empty : string.Format("{0} {1}", vehicleBooking.CapturedCredential.User.FirstName, vehicleBooking.CapturedCredential.User.LastName);

                var vehicleTestResults = dbContext.VehicleTestResults
                    .AsNoTracking()
                    .Include(f => f.VehicleTestQuestion)
                    .Include(f => f.VehicleTestQuestionAnswer)
                    .Where(f => f.VehicleTestBookingID == vehicleBooking.ID)
                    .OrderBy(f => f.ID)
                    .ToList();

                model.TestAnswers =
                    vehicleTestResults.Select(f =>
                        new VehicleTestAnswerModel()
                        {
                            ID = f.ID,
                            Comments = f.Comments,
                            Question = f.VehicleTestQuestion.Description,
                            QuestionType = f.TestTypeID == 1 ? "Text" : f.TestTypeID == 2 ? "Multiple Choice" : "Text / Multiple Choice",
                            Answer = f.TestQuestionsAnswersID.HasValue ? f.VehicleTestQuestionAnswer.Description : f.TextAnswer
                        })
                        .ToList();

                return Ok(model);
            }             
        }

        [HttpPost]
        [Route("TestResults")]
        [ResponseType(typeof(List<Models.Vehicle.TestResultRecordModel>))]
        public IHttpActionResult GetTestResults(int VehicleTestBookingID)
        {
            Oracle.DataAccess.Client.OracleDataReader reader;            
            var model = new List<TestResultRecordModel>();

            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                {
                    var command = new Oracle.DataAccess.Client.OracleCommand();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    OracleParameter p_booking_id = new OracleParameter();
                    p_booking_id.ParameterName = "p_booking_id";
                    p_booking_id.OracleDbType = OracleDbType.Int32;
                    p_booking_id.Size = 8;
                    p_booking_id.Value = VehicleTestBookingID;
                    p_booking_id.Direction = ParameterDirection.Input;
                    command.Parameters.Add(p_booking_id);

                    OracleParameter pResults = new OracleParameter();
                    pResults.ParameterName = "o_result";
                    pResults.OracleDbType = OracleDbType.RefCursor;
                    pResults.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pResults);                   


                    try
                    {
                        reader = ExecuteUnManagedReader(command, "TIS.VEHICLE_TESTING.GET_RESULTS", connection);

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var testRecordResult = new TestResultRecordModel();

                                testRecordResult.Comments = reader["COMMENTS"].ToString();
                                testRecordResult.TestQuestionDescription = reader["TEST_QUESTION_DESCRIPTION"].ToString();
                                testRecordResult.TestQuestionResult = reader["TEST_QUESTION_RESULT"].ToString();
                                testRecordResult.TestTypeDescription = reader["TEST_TYPE_DESCRIPTION"].ToString();
                                testRecordResult.TextAnswer = reader["TEXT_ANSWER"].ToString();

                                model.Add(testRecordResult);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }

            return Ok(model);
        }

        

        [HttpPost]
        [Route("GetBookingTestResultsPaginatedList")]        
        [ResponseType(typeof(PaginationListModel<Models.Vehicle.TestBookingRecordModel>))]
        public IHttpActionResult GetBookingTestResultsPaginatedList(BookingSearchTypeModel model)
        {
            Oracle.DataAccess.Client.OracleDataReader reader;
            var paginationList = new PaginationListModel<Models.Vehicle.TestBookingRecordModel>();

            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                {
                    var command = new Oracle.DataAccess.Client.OracleCommand();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    TMT.Build.OracleTableTypeClasses.BookingSearchType bookingSearchType = new TMT.Build.OracleTableTypeClasses.BookingSearchType();
                    bookingSearchType.BookingDate = model.BookingDate == null ? new DateTime(0001,01,01) : DateTime.ParseExact(model.BookingDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    bookingSearchType.TestCategoryID = model.TestCategoryID;
                    bookingSearchType.IsPassed = model.IsPassed;
                    bookingSearchType.EngineNumber = model.EngineNumber;
                    bookingSearchType.VehicleIdentificationNumber = model.VehicleIdentificationNumber;
                    bookingSearchType.VLN = model.VLN;
                    bookingSearchType.BookingReference = model.BookingReference;
                    bookingSearchType.DateIndicator = model.DateIndicator;
                    bookingSearchType.Quantity = model.Quantity;
                    bookingSearchType.PageNumber = model.PageNumber;
                    

                    Oracle.DataAccess.Client.OracleParameter par = new Oracle.DataAccess.Client.OracleParameter("P_BOOKING_SEARCH_TYPE", Oracle.DataAccess.Client.OracleDbType.Object);
                    par.Direction = ParameterDirection.Input;
                    par.Value = bookingSearchType;
                    par.UdtTypeName = "TIS.BOOKING_SEARCH_TYPE";
                    command.Parameters.Add(par);

                    OracleParameter pTotalRecords = new OracleParameter();
                    pTotalRecords.ParameterName = "O_TOTAL_RECORDS";
                    pTotalRecords.OracleDbType = OracleDbType.Int32;
                    pTotalRecords.Size = 8;                    
                    pTotalRecords.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pTotalRecords);

                    OracleParameter pResults = new OracleParameter();
                    pResults.ParameterName = "o_result";
                    pResults.OracleDbType = OracleDbType.RefCursor;
                    pResults.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pResults);

                    var testBookingRecordModels = new List<TestBookingRecordModel>();
                    

                    try
                    {
                        reader = ExecuteUnManagedReader(command, "TIS.VEHICLE_TESTING.GET_BOOKINGS", connection);
                        //DataTable dt = new DataTable();
                        //dt.Load(reader);
                        
                        if (!(command.Parameters["O_TOTAL_RECORDS"].Value is DBNull))
                        {
                            while (reader.Read())
                            {
                                var testBookingRecord = new TestBookingRecordModel();
                                
                                testBookingRecord.ID = Convert.ToInt32(reader["BOOKING_ID"]);
                                testBookingRecord.BookingReference = reader["BOOKING_REFERENCE"].ToString();
                                testBookingRecord.EngineNumber = reader["ENGINE"].ToString();
                                testBookingRecord.Make = reader["MAKE"].ToString();
                                testBookingRecord.Model = reader["MODEL"].ToString();                                
                                testBookingRecord.IsPassed = reader["ISPASSED"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ISPASSED"]);
                                testBookingRecord.TestDate = Convert.ToDateTime(reader["TESTDATE"]);
                                testBookingRecord.TestType = reader["TESTTYPE"].ToString();
                                testBookingRecord.VIN = reader["VIN"].ToString();
                                testBookingRecord.VLN = reader["VLN"].ToString();
                                testBookingRecord.InsuranceExpiryDate = reader["INSURANCEEXPIRYDATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["INSURANCEEXPIRYDATE"]);
                                testBookingRecord.RoadworthyDate = reader["ROADWORTHYDATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ROADWORTHYDATE"]);
                                testBookingRecord.LicenceExpiryDate = reader["LICENCEEXPIRYDATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LICENCEEXPIRYDATE"]);
                                testBookingRecord.CapturedDate = reader["CAPTUREDDATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CAPTUREDDATE"]);
                                testBookingRecord.NetWeight = Convert.ToInt32(reader["NETWEIGHT"]);
                                testBookingRecord.GVM = Convert.ToInt32(reader["GVM"]);
                                testBookingRecord.YearOfMake = reader["YEAROFMAKE"].ToString();


                                testBookingRecordModels.Add(testBookingRecord);
                            }

                            paginationList.Models = testBookingRecordModels;
                            paginationList.TotalCount = testBookingRecordModels.Count();

                        }
                    }
                    catch (Exception ex)
                    {
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }

            return Ok(paginationList);

        }

        

        [HttpPost]
        [Route("GetBookingTestResults")]
        [ResponseType(typeof(List<Models.Vehicle.TestBookingRecordModel>))]
        public IHttpActionResult GetBookingTestResults(BookingSearchTypeModel model)
        {
            Oracle.DataAccess.Client.OracleDataReader reader;
            var paginationList = new PaginationListModel<Models.Vehicle.TestBookingRecordModel>();

            try
            {
                using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                {
                    var command = new Oracle.DataAccess.Client.OracleCommand();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    TMT.Build.OracleTableTypeClasses.BookingSearchType bookingSearchType = new TMT.Build.OracleTableTypeClasses.BookingSearchType();
                    bookingSearchType.BookingDate = DateTime.ParseExact(model.BookingDate, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    bookingSearchType.TestCategoryID = model.TestCategoryID;
                    bookingSearchType.IsPassed = model.IsPassed;
                    bookingSearchType.EngineNumber = model.EngineNumber;
                    bookingSearchType.VehicleIdentificationNumber = model.VehicleIdentificationNumber;
                    bookingSearchType.VLN = model.VLN;
                    bookingSearchType.BookingReference = model.BookingReference;
                    bookingSearchType.DateIndicator = model.DateIndicator;
                    bookingSearchType.Quantity = model.Quantity;
                    bookingSearchType.PageNumber = model.PageNumber;                   


                    Oracle.DataAccess.Client.OracleParameter par = new Oracle.DataAccess.Client.OracleParameter("P_BOOKING_SEARCH_TYPE", Oracle.DataAccess.Client.OracleDbType.Object);
                    par.Direction = ParameterDirection.Input;
                    par.Value = bookingSearchType;
                    par.UdtTypeName = "TIS.BOOKING_SEARCH_TYPE";
                    command.Parameters.Add(par);

                    OracleParameter pTotalRecords = new OracleParameter();
                    pTotalRecords.ParameterName = "O_TOTAL_RECORDS";
                    pTotalRecords.OracleDbType = OracleDbType.Int32;
                    pTotalRecords.Size = 8;
                    pTotalRecords.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pTotalRecords);

                    OracleParameter pResults = new OracleParameter();
                    pResults.ParameterName = "o_result";
                    pResults.OracleDbType = OracleDbType.RefCursor;
                    pResults.Direction = ParameterDirection.Output;
                    command.Parameters.Add(pResults);

                    var testBookingRecordModels = new List<TestBookingRecordModel>();


                    try
                    {
                        reader = ExecuteUnManagedReader(command, "TIS.VEHICLE_TESTING.GET_BOOKINGS", connection);
                        //DataTable dt = new DataTable();
                        //dt.Load(reader);

                        if (!(command.Parameters["O_TOTAL_RECORDS"].Value is DBNull))
                        {
                            while (reader.Read())
                            {
                                var testBookingRecord = new TestBookingRecordModel();

                                testBookingRecord.ID = Convert.ToInt32(reader["BOOKING_ID"]);
                                testBookingRecord.BookingReference = reader["BOOKING_REFERENCE"].ToString();
                                testBookingRecord.EngineNumber = reader["ENGINE"].ToString();
                                testBookingRecord.Make = reader["MAKE"].ToString();
                                testBookingRecord.Model = reader["MODEL"].ToString();
                                testBookingRecord.IsPassed = reader["ISPASSED"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ISPASSED"]);
                                testBookingRecord.TestDate = Convert.ToDateTime(reader["TESTDATE"]);
                                testBookingRecord.TestType = reader["TESTTYPE"].ToString();
                                testBookingRecord.VIN = reader["VIN"].ToString();
                                testBookingRecord.VLN = reader["VLN"].ToString();

                                testBookingRecordModels.Add(testBookingRecord);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }

            return Ok(paginationList);

        }

        [HttpPost]
        [Route("VehicleDetail")]
        [ResponseType(typeof(Models.Vehicle.VehicleModel))]
        public IHttpActionResult GetVehicleDetail([FromBody] IList<FilterModel> filters, FilterJoin filterJoin)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Vehicle>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .Vehicles
              //      .Include(f => f.VehicleIDNumber)
              //      .Include(f => f.EngineNumber)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);
                
                var pageResults = query
                    .FirstOrDefault();

                var entities = new Vehicle();

                var vehicleDetail = new Models.Vehicle.VehicleModel();

                if (pageResults != null)
                {
                    entities = pageResults;

                    vehicleDetail = new Models.Vehicle.VehicleModel
                    {
                        ID = entities.ID,
                        VIN = entities.VehicleIDNumber,
                        EngineNumber = entities.EngineNumber,
                        VehicleCategoryId = entities.VehicleCategoryId,
                        VehicleTypeId = entities.VehicleTypeId,
                        VehicleMakeId = entities.VehicleMakeId,
                        VehicleModelId = entities.VehicleModelId,
                        VehicleModelNumberId = entities.VehicleModelNumberId,
                        VehicleColourId = entities.ColourId,
                        YearOfMake = entities.YearOfMake,
                        VLN = entities.VLN,
                        NetWeight = entities.NetWeight,
                        GVM = entities.GVM,
                        PropelledById = entities.ProplelledById,
                        SeatingCapacity = entities.SeatingCapacity,
                        FuelTypeId = entities.FuelTypeId,
                        RegistrationStatusId = entities.RegistrationStatusId,
                        LicenceExpiryDate = entities.LicenceExpiryDate,
                        RoadworthyExpiryDate = entities.RoadworthyExpiryDate,
                        InsuranceExpiryDate = entities.InsuranceExpiryDate,
                        CapturedDate = entities.CapturedDate,
                        CapturedCredentialId = entities.CapturedCredentialId,
                        TestBookings = entities.TestBookings.Select(g => new VehicleBookingModel { BookingReference = g.BookingReference }).ToList()
                    };
                }
                
                return Ok(vehicleDetail);
            }
        }

        [HttpPost]
        [Route("Evidence")]
        public async Task<IHttpActionResult> PostEvidence(long bookingID, long siteID, InspectionEvidenceType evidenceType, string mimeType)
        {
            using (Stream stream = await this.Request.Content.ReadAsStreamAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(memoryStream);

                    using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                    {
                        var inspectionEvidence =
                            new InspectionEvidence
                            {
                                VEHICLE_TEST_BOOKING_ID = bookingID,
                                INSPECTION_EVIDENCE_TYPE_ID = (int)evidenceType,
                                MIME_TYPE = mimeType,
                                SITE_ID = siteID
                            };

                        using (var command = new Oracle.DataAccess.Client.OracleCommand())
                        {
                            try
                            {
                                if (connection.State != ConnectionState.Open)
                                {
                                    connection.Open();
                                }

                                command.Parameters.Add(
                                    new Oracle.DataAccess.Client.OracleParameter("P_INSPECTION_EVIDENCE", Oracle.DataAccess.Client.OracleDbType.Object)
                                    {
                                        Value = inspectionEvidence,
                                        UdtTypeName = "TIS.INSPECTION_EVIDENCE_TYPE"
                                    });
                                command.Parameters.Add("O_RESULT", Oracle.DataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                                ExcecuteNonQuery(command, "TIS.VEHICLE_TESTING.SUBMIT_INSPECTION_EVIDENCE", connection);

                                if ((command.Parameters["O_RESULT"].Value is DBNull))
                                    throw new Exception("Failed to return valid file paths.");

                                var refCursor = (Oracle.DataAccess.Types.OracleRefCursor)command.Parameters["O_RESULT"].Value;

                                using (var dataReader = refCursor.GetDataReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        if (dataReader["MIME_DATA_PATH"] is DBNull || dataReader["FILENAME"] is DBNull)
                                            continue;

                                        var filePath = dataReader["MIME_DATA_PATH"] as string;
                                        var fileName = dataReader["FILENAME"] as string;

                                        Directory.CreateDirectory(filePath);
                                        filePath = Path.Combine(filePath, fileName);

                                        if (File.Exists(filePath))
                                            File.Delete(filePath);

                                        File.WriteAllBytes(filePath, memoryStream.ToArray());

                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                            }
                            finally
                            {
                                foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
                                {
                                    if (parameter.Value is IDisposable)
                                    {
                                        ((IDisposable)(parameter.Value)).Dispose();
                                    }

                                    parameter.Dispose();
                                }
                            }
                        }

                        return Ok();
                    }
                }
            }
        }

        private void ExcecuteNonQuery(Oracle.DataAccess.Client.OracleCommand command, string storedProcName, Oracle.DataAccess.Client.OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        #region DATABASE FUNCTIONS
        private void ExecuteNonQuery(Oracle.DataAccess.Client.OracleCommand command, 
            string storedProcName,
            Oracle.DataAccess.Client.OracleConnection dbConnection
            )
        {   
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            
            command.ExecuteNonQuery();          
        }

        private DataSet ExecuteQuery(Oracle.ManagedDataAccess.Client.OracleCommand command, string storedProcName, Oracle.ManagedDataAccess.Client.OracleConnection dbConnection)
        {
            DataSet dsData = new DataSet();
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;

            Oracle.ManagedDataAccess.Client.OracleDataAdapter daData = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(command);
            daData.Fill(dsData);

            return dsData;
        }

        private Oracle.DataAccess.Client.OracleDataReader ExecuteUnManagedReader(Oracle.DataAccess.Client.OracleCommand command, string storedProcName, Oracle.DataAccess.Client.OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            Oracle.DataAccess.Client.OracleDataReader reader;

            reader = command.ExecuteReader();

            return reader;
        }

        private Oracle.ManagedDataAccess.Client.OracleDataReader ExecuteReader(Oracle.ManagedDataAccess.Client.OracleCommand command, string storedProcName, Oracle.ManagedDataAccess.Client.OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            Oracle.ManagedDataAccess.Client.OracleDataReader reader;

            reader = command.ExecuteReader();

            return reader;
        }
        #endregion
    }
}
