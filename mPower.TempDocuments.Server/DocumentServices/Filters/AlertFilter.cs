using System.Collections.Generic;
using mPower.Domain.Application.Enums;
using mPower.Framework.Services;

namespace mPower.TempDocuments.Server.DocumentServices.Filters
{
    public class AlertFilter : BaseFilter
    {
        public List<string> Ids { get; set; }

        public EmailTypeEnum? Type { get; set; }

        public List<string> PublicKeys { get; set; }
    }
}