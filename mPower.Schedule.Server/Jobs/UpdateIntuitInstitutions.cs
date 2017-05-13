using mPower.Aggregation.Client;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Schedule.Server.Environment;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Globalization;
using System.Linq;

namespace mPower.Schedule.Server.Jobs
{
    public class UpdateIntuitInstitutions : IScheduledJob
    {
        private readonly IAggregationClient _aggregation;
        private readonly IntutitInstitutionLuceneService _intutitInstitutionLuceneService;

        public UpdateIntuitInstitutions(IAggregationClient aggregation, IntutitInstitutionLuceneService intutitInstitutionLuceneService)
        {
            _aggregation = aggregation;
            _intutitInstitutionLuceneService = intutitInstitutionLuceneService;
        }


        public void Execute(IJobExecutionContext context)
        {
            var institutions = _aggregation.GetInstitutions();
            var mapped = institutions.Select(x => new IntuitInstitutionLuceneDocument
            {
                Id = x.Id,
                IntuitId = x.IntuitId.ToString(CultureInfo.InvariantCulture),
                Name = x.Name,
                HomeUrl = x.HomeUrl,
            }).ToList();

            
            mapped.Add(new IntuitInstitutionLuceneDocument
            {
                Id = "100000",
                Name = "Test Institution Username: direct Password: any",
                IntuitId = "100000",
            });

            _intutitInstitutionLuceneService.SetFlushAfter(10000);
            _intutitInstitutionLuceneService.RemoveIndexFromDisc();

            _intutitInstitutionLuceneService.Insert(mapped.ToArray());
            _intutitInstitutionLuceneService.Flush(true);
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("Refresh finantial institutions from Intuit", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("RefreshIntuitInstitutionsOncePerWeek", Int32.MaxValue, TimeSpan.FromDays(7));
        }

        public bool IsEnabled
        {
            get { return true; }
        }
    }
}
