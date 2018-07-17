using Kapsch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapsch.EVR.Gateway
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

        public static Error QuestionsNotFound
        {
            get
            {
                return new Error(1001, "Questions not found.");
            }
        }

        public static Error VehicleMakeDoesNotExist
        {
            get
            {
                return new Error(12001, "Vehicle Make does not exist.");
            }
        }

        public static Error VehicleMakeAlreadyExist
        {
            get
            {
                return new Error(12002, "Vehicle Make already exist.");
            }
        }

        public static Error VehicleModelDoesNotExist
        {
            get
            {
                return new Error(12003, "Vehicle Model does not exist.");
            }
        }

        public static Error VehicleModelAlreadyExist
        {
            get
            {
                return new Error(12004, "Vehicle Model already exist.");
            }
        }

        public static Error VehicleModelNumberDoesNotExist
        {
            get
            {
                return new Error(12005, "Vehicle Model Number does not exist.");
            }
        }

        public static Error VehicleModelNumberAlreadyExist
        {
            get
            {
                return new Error(12006, "Vehicle Model Number already exist.");
            }
        }

        public static Error VehicleTestBookingDoesNotExist
        {
            get
            {
                return new Error(12007, "Vehicle Test Booking does not exist.");
            }
        } 
    }
}