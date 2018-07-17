using Kapsch.ThirdParty.Payment;
using Kapsch.ThirdParty.Payment.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DPO.API.V5
{
    public class Provider : IProvider
    {
        public static readonly string CompanyToken = ConfigurationManager.AppSettings["CompanyToken"];

        private static readonly XmlSerializer CreateTokenXmlSerializer = new XmlSerializer(typeof(CreateToken.ResponseModel.API3G));
        private static readonly XmlSerializer UpdateTokenXmlSerializer = new XmlSerializer(typeof(UpdateToken.ResponseModel.API3G));
        private static readonly XmlSerializer VerifyTokenXmlSerializer = new XmlSerializer(typeof(CreateToken.ResponseModel.API3G));      
        private static readonly string DPOVersion5APIEndpoint = ConfigurationManager.AppSettings["DPO_V5_APIEndpoint"];
        private static readonly string DPOVersion6APIEndpoint = ConfigurationManager.AppSettings["DPO_V6_APIEndpoint"];
        private static readonly int TimeoutInMilliseconds = 20 * 1000;

        protected RestClient _restV5Client;
        protected RestClient _restV6Client;

        private Action<int, string, string, string, bool> _log;

        public TransactionIDModel RegisterTransaction(TransactionModel model)
        {
            var requestDetail = string.Empty;
            var responseDetail = string.Empty;
            var success = false;
            var exceptionMessage = string.Empty;

            try
            {
                var request = new CreateToken.RequestModel();
                request.API = new CreateToken.RequestModel.API3G();
                request.API.CompanyToken = CompanyToken;
                request.API.Request = "createToken";

                var transaction = new CreateToken.RequestModel.Transaction();
                transaction.PaymentAmount = model.Amount;
                transaction.PaymentCurrency = "ZMW";
                transaction.CompanyRef = model.CompanyRef;
                transaction.CompanyAccRef = model.CompanyAccRef;
                transaction.PTL = 24 * 20 * 365; // Twenty years
                transaction.CustomerFirstName = model.CustomerFirstName;
                transaction.CustomerLastName = model.CustomerLastName;
                transaction.CustomerEmail = model.CustomerEmail;
                transaction.CustomerDialCode = model.CustomerISODialCode;
                transaction.CustomerPhone = model.CustomerPhone;

                var service = new CreateToken.RequestModel.Service();
                service.ServiceType = (int)model.ServiceType;
                service.ServiceDescription = model.ServiceDescription;
                service.ServiceDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

                request.API.Transaction = transaction;
                request.API.Services = new[] { service };

                var restRequest = new RestRequest(Method.POST);
                restRequest.AddXmlBody(request.API);
                requestDetail = restRequest.Parameters[0].Value.ToString();

                var response = this.RestV5Client.Execute(restRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw CreateException(response);

                responseDetail = response.Content;

                using (var stringReader = new StringReader(response.Content))
                {                
                    var apiResponse = (CreateToken.ResponseModel.API3G)CreateTokenXmlSerializer.Deserialize(stringReader);
                    if (apiResponse.Result == "000")
                    {
                        success = true;
                        return
                            new TransactionIDModel
                            {
                                TransactionToken = apiResponse.TransToken,
                                TransactionReference = apiResponse.TransRef
                            };
                    }

                    throw new ProviderException(string.Format("CreateToken failed: Result = {0}, Explanation {1}", apiResponse.Result, apiResponse.ResultExplanation), apiResponse.Result);
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;

                throw;
            }
            finally
            {
                OnLog(1, requestDetail, responseDetail, exceptionMessage, success);
            }
        }

        public void UpdateTransaction(string transactionToken, TransactionModel model)
        {
            var requestDetail = string.Empty;
            var responseDetail = string.Empty;
            var success = false;
            var exceptionMessage = string.Empty;

            try
            {
                var request = new UpdateToken.RequestModel();
                request.API = new UpdateToken.RequestModel.API3G();
                request.API.CompanyToken = CompanyToken;
                request.API.Request = "updateToken";
                request.API.TransactionToken = transactionToken;
                request.API.PaymentAmount = model.Amount;
                request.API.CompanyRef = model.CompanyRef;

                var restRequest = new RestRequest(Method.POST);
                restRequest.AddXmlBody(request.API);
                requestDetail = restRequest.Parameters[0].Value.ToString();

                var response = this.RestV6Client.Execute(restRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw CreateException(response);

                responseDetail = response.Content;

                using (var stringReader = new StringReader(response.Content))
                {
                    var apiResponse = (UpdateToken.ResponseModel.API3G)UpdateTokenXmlSerializer.Deserialize(stringReader);
                    if (apiResponse.Result == "000")
                    {
                        success = true;
                        return;
                    }

                    throw new ProviderException(string.Format("UpdateToken failed: Result = {0}, Explanation {1}", apiResponse.Result, apiResponse.ResultExplanation), apiResponse.Result);
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;

                throw;
            }
            finally
            {
                OnLog(1, requestDetail, responseDetail, exceptionMessage, success);
            }           
        }

        public TransactionModel VerifyTransaction(string transactionToken)
        {
            var requestDetail = string.Empty;
            var responseDetail = string.Empty;
            var success = false;
            var exceptionMessage = string.Empty;

            try
            {
                var request = new VerifyToken.RequestModel();
                request.API = new VerifyToken.RequestModel.API3G();
                request.API.CompanyToken = CompanyToken;
                request.API.Request = "verifyToken";
                request.API.TransactionToken = transactionToken;

                var restRequest = new RestRequest(Method.POST);
                restRequest.AddXmlBody(request.API);
                requestDetail = restRequest.Parameters[0].Value.ToString();

                var response = this.RestV5Client.Execute(restRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw CreateException(response);

                responseDetail = response.Content;

                using (var stringReader = new StringReader(response.Content))
                {
                    var apiResponse = (VerifyToken.ResponseModel.API3G)VerifyTokenXmlSerializer.Deserialize(stringReader);
                    if (apiResponse.Result != "000")
                        throw new ProviderException(string.Format("VerifyToken failed: Result = {0}, Explanation {1}", apiResponse.Result, apiResponse.ResultExplanation), apiResponse.Result);

                    success = true;

                    return new TransactionModel { };
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;

                throw;
            }
            finally
            {
                OnLog(1, requestDetail, responseDetail, exceptionMessage, success);
            }  
        }

        public void CancelTransaction(string transactionToken)
        {
            var requestDetail = string.Empty;
            var responseDetail = string.Empty;
            var success = false;
            var exceptionMessage = string.Empty;

            try
            {
                var request = new VerifyToken.RequestModel();
                request.API = new VerifyToken.RequestModel.API3G();
                request.API.CompanyToken = CompanyToken;
                request.API.Request = "cancelToken";
                request.API.TransactionToken = transactionToken;

                var restRequest = new RestRequest(Method.POST);
                restRequest.AddXmlBody(request.API);
                requestDetail = restRequest.Parameters[0].Value.ToString();

                var response = this.RestV5Client.Execute(restRequest);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw CreateException(response);

                responseDetail = response.Content;

                using (var stringReader = new StringReader(response.Content))
                {
                    var apiResponse = (VerifyToken.ResponseModel.API3G)VerifyTokenXmlSerializer.Deserialize(stringReader);
                    if (apiResponse.Result != "000")
                        throw new ProviderException(string.Format("CancelToken failed: Result = {0}, Explanation {1}", apiResponse.Result, apiResponse.ResultExplanation), apiResponse.Result);

                    success = true;
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;

                throw;
            }
            finally
            {
                OnLog(1, requestDetail, responseDetail, exceptionMessage, success);
            }  
        }

        protected Exception CreateException(IRestResponse response)
        {
            if (response.ErrorException != null)
                return response.ErrorException;

            if (string.IsNullOrWhiteSpace(response.Content))
                return new Exception(response.StatusDescription);

            throw new Exception(response.Content);
        }

        private void OnLog(int paymentProverID, string requestDetail, string responseDetail, string exceptionMessage, bool isOK)
        {
            if (_log != null)
            {
                _log.Invoke(paymentProverID, requestDetail, responseDetail, exceptionMessage, isOK);
            }
        }

        public RestClient RestV5Client
        {
            get
            {
                if (_restV5Client != null)
                    return _restV5Client;

                _restV5Client = new RestClient(DPOVersion5APIEndpoint);
                _restV5Client.Timeout = TimeoutInMilliseconds;

                return _restV5Client;
            }
        }

        public RestClient RestV6Client
        {
            get
            {
                if (_restV6Client != null)
                    return _restV6Client;

                _restV6Client = new RestClient(DPOVersion6APIEndpoint);
                _restV6Client.Timeout = TimeoutInMilliseconds;

                return _restV6Client;
            }
        }


        public Action<int, string, string, string, bool> Log
        {
            set
            {
                _log = value;
            }
        }


        public int ID
        {
            get { return 1; }
        }
    }
}
