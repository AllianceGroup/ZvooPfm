using System.Collections.Generic;

namespace mPower.Framework.Services
{
    public class BaseFilter
    {
        public BaseFilter(bool pagingRequired = false)
        {
            if (pagingRequired)
                PagingInfo = new PagingInfo();
            ExcludeFields = new List<string>();
        }

        public bool IsPagingEnabled
        {
            get { return PagingInfo != null; }
        }

        public PagingInfo PagingInfo { get; set; }

        public List<string> ExcludeFields { get; set; }
    }
}
