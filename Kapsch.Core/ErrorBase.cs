using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core
{
    public class ErrorBase
    {
        public ErrorBase()
        {
        }

        public ErrorBase(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public static ErrorBase PopulateMethodFailed(string message)
        {
            return new ErrorBase(98, message);
        }

        public static ErrorBase PopulateUnexpectedException(Exception ex)
        {
            return new ErrorBase(99, string.Format("Unexpected Exception: {0}, {1}", ex.Message, ex.InnerException));
        }

        public static ErrorBase PopulateInvalidParameter(string parameterName, string validationMessage)
        {
            return new ErrorBase(100, string.Format("Invalid parameter: {0}, {1}", parameterName, validationMessage));
        }

        public static implicit operator int(ErrorBase error)
        {
            return error.Code;
        }

        public static implicit operator string(ErrorBase error)
        {
            return error.Message;
        }

        public static bool operator ==(ErrorBase x, ErrorBase y)
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

        public static bool operator !=(ErrorBase x, ErrorBase y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (System.Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            ErrorBase convertedObj = obj as ErrorBase;

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

        public static ErrorBase Success
        {
            get
            {
                return new ErrorBase(0, "OK");
            }
        }

        public int Code { get; set; }
        public string Message { get; set; }
    }
}
