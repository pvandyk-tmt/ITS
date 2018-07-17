using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Core
{
    [Serializable]
    public class TMTException : Exception
    {
        private KeyValuePair<string, string>[] _additionalInfo;
        private TMTErrorBase _tmtError;

        public TMTException(string message)
            : base(message)
        {
            this._additionalInfo = null;
        }

        public TMTException(TMTErrorBase tmtError)
            : base(tmtError.Message)
        {
            this._additionalInfo = null;
            this._tmtError = tmtError;
        }

        public TMTException(string message, TMTErrorBase tmtError)
            : base(message)
        {
            this._additionalInfo = null;
            this._tmtError = tmtError;
        }

        public TMTException(string message, int tmtErrorCode)
            : base(message)
        {
            this._additionalInfo = null;
            this._tmtError = new TMTErrorBase(tmtErrorCode, message);
        }

        public TMTException(string message, KeyValuePair<string, string>[] additionalInfo)
            : base(message)
        {
            this._additionalInfo = additionalInfo;
        }

        public TMTException(string message, int tmtError, KeyValuePair<string, string>[] additionalInfo)
            : base(message)
        {
            this._additionalInfo = additionalInfo;
        }

        public TMTException(string message, TMTErrorBase tmtError, KeyValuePair<string, string>[] additionalInfo)
            : base(message)
        {
            this._additionalInfo = additionalInfo;
            this._tmtError = tmtError;
        }

        public TMTException(string message, Exception innerException)
            : base(message, innerException)
        {
            this._additionalInfo = null;
        }

        public TMTException(string message, Exception innerException, KeyValuePair<string, string>[] additionalInfo)
            : base(message, innerException)
        {
            this._additionalInfo = additionalInfo;
        }

        protected TMTException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                try
                {
                    int errorCode = info.GetInt32("errorCode");
                    if (errorCode != 0)
                    {
                        string message = info.GetString("errorMessage");
                        this._tmtError = new TMTErrorBase(errorCode, message);
                    }
                    this._additionalInfo = (KeyValuePair<string, string>[])info.GetValue("additionalInfo", typeof(KeyValuePair<string, string>[]));
                }
                catch
                {
                }
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                try
                {
                    if (this._tmtError == null)
                    {
                        info.AddValue("errorCode", 0);
                    }
                    else
                    {
                        info.AddValue("errorCode", (int)this._tmtError);
                        info.AddValue("errorMessage", (string)this._tmtError);
                    }
                    info.AddValue("additionalInfo", _additionalInfo);
                }
                catch
                {
                }
            }
        }

        public KeyValuePair<string, string>[] AdditionalInfo
        {
            get
            {
                return this._additionalInfo;
            }
        }

        public TMTErrorBase TMTError
        {
            get
            {
                return this._tmtError;
            }
        }
    }
}
