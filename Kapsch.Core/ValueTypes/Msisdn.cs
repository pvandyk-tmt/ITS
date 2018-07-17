using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core
{
    public class Msisdn
    {
        public enum Format
        {
            /// <summary>
            /// Without country code
            /// </summary>
            Short,
            /// <summary>
            /// With country code, but not +
            /// </summary>
            Msisdn,
            /// <summary>
            /// Msisdn plus '+' infront
            /// </summary>
            International
        }

        static private Dictionary<Country, string> countryCodes = 
            new Dictionary<Country, string>()
            {
                { Country.Zambia, "260" },
                { Country.SouthAfrica, "27" }
            };

        static private Dictionary<Country, Dictionary<MobileNetwork, List<string>>> prefixes = 
            new Dictionary<Country, Dictionary<MobileNetwork, List<string>>>()
            {
                {
                    Country.Zambia,
                    new Dictionary<MobileNetwork, List<string>>()
                    {
                        { MobileNetwork.Zamtel , new List<string> { "95" }  },
                        { MobileNetwork.MTN , new List<string> { "96" }  },
                        { MobileNetwork.Airtel , new List<string> { "97" }  },
                    }
                },
                {
                    Country.SouthAfrica,
                    new Dictionary<MobileNetwork, List<string>>()
                    {
                        { MobileNetwork.MTN , new List<string> { "603", "604", "605", "710", "717", "718", "719", "73", "78", "83", "810", "851" }  },
                        { MobileNetwork.VodaCom , new List<string> { "711", "712", "713", "714", "715", "716", "72", "76", "79", "82", "818", "606", "607", "608", "609", "850", "852", "853" }  },
                        { MobileNetwork.CellC , new List<string> { "74", "84", "610", "611", "612", "613" }  },
                        { MobileNetwork.Eighta , new List<string> { "811", "812", "813", "814", "815", "817" }  }
                    }
                }
            };

        private string msisdn;
        private Country country;

        public Msisdn(string msisdn, Country country)
        {
            if (!Msisdn.IsValid(msisdn, country))
                throw new Exception("Msisdn is not valid." + msisdn);

            msisdn = msisdn.Replace(" ", "").Replace("(", "").Replace(")", "");
            msisdn = msisdn.TrimStart('+');
            msisdn = Convert(msisdn, Format.Msisdn, country);

            this.msisdn = msisdn;
            this.country = country;
        }

        public static bool IsValid(string msisdn, Country country)
        {
            if (string.IsNullOrWhiteSpace(msisdn))
                return false;

            msisdn = msisdn.Replace(" ", "").Replace("(", "").Replace(")", "");
            msisdn = msisdn.TrimStart('+');

            if (msisdn.Any(f => !Char.IsDigit(f)))
                return false;

            switch (country)
            {
                case Country.SouthAfrica:
                    if (!(msisdn.Length == 10 || msisdn.Length == 11))
                        return false;

                    if ((msisdn.Length == 10) && (msisdn.StartsWith("0")))
                        msisdn = msisdn.Substring(1);

                    if (msisdn.Length == 11)
                    {
                        if (msisdn.StartsWith(countryCodes[country]))
                            msisdn = msisdn.Substring(countryCodes[country].Length);
                        else
                            return false;
                    }

                    return prefixes[country].Select(f => f.Value).SelectMany(f => f).Any(f => msisdn.StartsWith(f));

                case Country.Zambia:
                    if (!(msisdn.Length == 10 || msisdn.Length == 12))
                        return false;

                    if ((msisdn.Length == 10) && (msisdn.StartsWith("0")))
                        msisdn = msisdn.Substring(1);

                    if (msisdn.Length == 12)
                    {
                        if (msisdn.StartsWith(countryCodes[country]))
                            msisdn = msisdn.Substring(countryCodes[country].Length);
                        else
                            return false;
                    }

                    return prefixes[country].Select(f => f.Value).SelectMany(f => f).Any(f => msisdn.StartsWith(f));
                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return this.ToString(Format.Msisdn);
        }

        public string ToString(Format format)
        {
            switch (format)
            {
                case Format.Short:
                case Format.Msisdn:
                case Format.International:
                    return Convert(this.msisdn, format, this.country);
            }

            throw new ArgumentOutOfRangeException("format");
        }

        private string Convert(string msisdn, Format format, Country country)
        {
            switch (country)
            {
                case Country.SouthAfrica:
                    string code = countryCodes[country];
                    int codeLength = code.Length;

                    switch (format)
                    {
                        case Format.Short:
                            if (msisdn.Length == 10)
                                return msisdn;
                            else
                                return "0" + msisdn.Substring(codeLength, msisdn.Length - codeLength);
                        case Format.Msisdn:
                            if (msisdn.Length == 10)
                                return code + msisdn.Substring(1, msisdn.Length - 1);
                            else
                                return msisdn;
                        case Format.International:
                            if (msisdn.Length == 10)
                                return "+" + code + msisdn.Substring(1, msisdn.Length - 1);
                            else
                                return "+" + msisdn;
                    }
                    break;
                case Country.Zambia:
                    code = countryCodes[country];
                    codeLength = code.Length;

                    switch (format)
                    {
                        case Format.Short:
                            if (msisdn.Length == 10)
                                return msisdn;
                            else
                                return "0" + msisdn.Substring(codeLength, msisdn.Length - codeLength);
                        case Format.Msisdn:
                            if (msisdn.Length == 10)
                                return code + msisdn.Substring(1, msisdn.Length - 1);
                            else
                                return msisdn;
                        case Format.International:
                            if (msisdn.Length == 10)
                                return "+" + code + msisdn.Substring(1, msisdn.Length - 1);
                            else
                                return "+" + msisdn;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}
