using Kapsch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapsch.ITS.Gateway
{
    public class Error : ErrorBase
    {
        public Error()
            : base()
        {
        }

        protected internal Error(int code, string message)
            : base(code, message)
        {

        }

        public new static ErrorBase PopulateUnexpectedException(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new HttpException(500, string.Format("Unexpected Exception: {0}, {1}", ex.Message, ex.InnerException)));

            return new ErrorBase(99, string.Format("Unexpected Exception: {0}, {1}", ex.Message, ex.InnerException));
        }

        public static Error CredentialNotFound
        {
            get
            {
                return new Error(1001, "Credential is not found.");
            }
        }

        public static Error PasswordIncorrect
        {
            get
            {
                return new Error(1002, "Password is incorrect.");
            }
        }

        public static Error CredentialNotActive
        {
            get
            {
                return new Error(1003, "Credential is not active.");
            }
        }

        public static Error PasswordHasExpired
        {
            get
            {
                return new Error(1004, "Password has expired.");
            }
        }

        public static Error CredentialInvalidEntityType
        {
            get
            {
                return new Error(1005, "Credential entity type is invalid.");
            }
        }

        public static Error UserNameAlreadyUsed
        {
            get
            {
                return new Error(1006, "User Name already used.");
            }
        }

        public static Error RegisterItemDoesNotExist
        {
            get
            {
                return new Error(3500, "Register Item does not exist.");
            }
        }

        public static Error DeviceNotFound
        {
            get
            {
                return new Error(11001, "Device is not found.");
            }
        }

        public static Error DistrictDoesNotExist
        {
            get
            {
                return new Error(11002, "District does not exist.");
            }
        }

        public static Error MsisdnInvalid
        {
            get
            {
                return new Error(12000, "Msisdn is invalid.");
            }
        }
    }
}