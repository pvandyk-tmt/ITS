using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kapsch.Core.Gateway
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

        public static Error TokenDoesNotExist
        {
            get
            {
                return new Error(2000, "Token does not exist.");
            }
        }

        public static Error TokenExpired
        {
            get
            {
                return new Error(2001, "Token has expired.");
            }
        }

        public static Error UserDoesNotExist
        {
            get
            {
                return new Error(3000, "User does not exist.");
            }
        }

        public static Error MobileDeviceDoesNotExist
        {
            get
            {
                return new Error(3100, "Mobile Device does not exist.");
            }
        }

        public static Error MobileDeviceAlreadyExist
        {
            get
            {
                return new Error(3101, "Mobile Device already exist.");
            }
        }

        public static Error MobileDeviceItemDoesNotExist
        {
            get
            {
                return new Error(3200, "Mobile Device Item does not exist.");
            }
        }

        public static Error MobileDeviceItemAlreadyExist
        {
            get
            {
                return new Error(3201, "Mobile Device Item already exist.");
            }
        }

        public static Error PaymentTransactionAlreadyExist
        {
            get
            {
                return new Error(3300, "Payment transaction already exist.");
            }
        }

        public static Error PaymentTransactionDoesNotExist
        {
            get
            {
                return new Error(3301, "Payment transaction does not exist.");
            }
        }

        public static Error PaymentTransactionInvalidAmount
        {
            get
            {
                return new Error(3302, "Payment transaction invalid amount.");
            }
        }

        public static Error PaymentTransactionItemEmpty
        {
            get
            {
                return new Error(3303, "Payment transaction item is invalid.");
            }
        }

        public static Error PaymentTerminalDoesNotExist
        {
            get
            {
                return new Error(3304, "Payment Terminal does not exist.");
            }
        }

        public static Error PaymentTerminalIsNotActive
        {
            get
            {
                return new Error(3305, "Payment Terminal is not active.");
            }
        }

        public static Error PaymentTerminalAlreadyExist
        {
            get
            {
                return new Error(3306, "Payment Terminal already exist.");
            }
        }
        

        public static Error DataQueryIllegalKeyword
        {
            get
            {
                return new Error(3400, "Illegal keyword used in query.");
            }
        } 

        public static Error RegisterItemDoesNotExist
        {
            get
            {
                return new Error(3500, "Register Item does not exist.");
            }
        }


        public static Error RegisterItemNotPayable
        {
            get
            {
                return new Error(3501, "Register Item can not be paid. Check status.");
            }
        }

        public static Error AddressInformationNotFound
        {
            get
            {
                return new Error(3600, "Address information not found.");
            }
        }

        public static Error DistrictDoesNotExist
        {
            get
            {
                return new Error(3700, "District does not exist.");
            }
        }

        public static Error ComputerDoesNotExist
        {
            get
            {
                return new Error(3800, "Computer does not exist.");
            }
        }

        public static Error ComputerAlreadyExist
        {
            get
            {
                return new Error(3801, "Computer already exist.");
            }
        }

        public static Error ComputerSettingDoesNotExist
        {
            get
            {
                return new Error(3802, "Computer Setting does not exist.");
            }
        }

        public static Error ComputerSettingAlreadyExist
        {
            get
            {
                return new Error(3803, "Computer Setting already exist.");
            }
        }

        public static Error MobileNumberInvalid
        {
            get
            {
                return new Error(3900, "Invalid mobile number for the country.");
            }
        }
    }
}
