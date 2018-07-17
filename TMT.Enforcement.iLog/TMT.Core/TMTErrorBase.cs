
using System;
namespace TMT.Core
{
    public class TMTErrorBase
    {
        public TMTErrorBase()
        {
        }

        public TMTErrorBase(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public static TMTErrorBase PopulateMethodFailed(string message)
        {
            return new TMTErrorBase(98, message);
        }

        public static TMTErrorBase PopulateUnexpectedException(Exception ex)
        {
            return new TMTErrorBase(99, string.Format("Unexpected Exception: {0}, {1}", ex.Message, ex.InnerException));
        }

        public static TMTErrorBase PopulateInvalidParameter(string parameterName, string validationMessage)
        {
            return new TMTErrorBase(100, string.Format("Invalid parameter: {0}, {1}", parameterName, validationMessage));
        }

        public static implicit operator int(TMTErrorBase error)
        {
            return error.Code;
        }

        public static implicit operator string(TMTErrorBase error)
        {
            return error.Message;
        }

        public static bool operator ==(TMTErrorBase x, TMTErrorBase y)
        {
            if (System.Object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.Code == y.Code;
        }

        public static bool operator !=(TMTErrorBase x, TMTErrorBase y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (System.Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            TMTErrorBase convertedObj = obj as TMTErrorBase;

            if (convertedObj == null)
            {
                return false;
            }

            return Code == convertedObj.Code;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static TMTErrorBase Success
        {
            get
            {
                return new TMTErrorBase(0, "OK");
            }
        }

        public int Code { get; set; }
        public string Message { get; set; }
    }
}
