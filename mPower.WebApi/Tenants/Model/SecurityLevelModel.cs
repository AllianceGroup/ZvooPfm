using mPower.Domain.Membership.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Model
{
    public class SecurityLevelModel
    {
        public SecurityLevelEnum SelectedLevel { get; set; }

        public SelectList SecurityLevels { get; set; }
    }
}