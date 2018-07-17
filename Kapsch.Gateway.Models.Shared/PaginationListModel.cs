using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Gateway.Models.Shared
{
    public class PaginationListModel<T>
    {
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get { return (int)Math.Ceiling(TotalCount / (double)PageSize); } }
        public int TotalCount { get; set; }
        public IList<T> Models { get; set; }
    }
}
