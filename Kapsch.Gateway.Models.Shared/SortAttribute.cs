using System;

namespace Kapsch.Gateway.Models.Shared
{
    public class SortAttribute : Attribute
    {
        private readonly string _databasePropertyName;

        public SortAttribute(string databasePropertyName)
        {
            _databasePropertyName = databasePropertyName;
        }

        public string DatabasePropertyName
        {
            get { return _databasePropertyName; }
        }
    }
}
