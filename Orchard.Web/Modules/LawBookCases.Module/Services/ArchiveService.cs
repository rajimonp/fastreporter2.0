using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Services {
    public class ArchiveService : IArchiveService {
        private readonly IRepository<CasePartArchiveRecord> _caseArchiveRepository;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ArchiveService(
            IRepository<CasePartArchiveRecord> caseArchiveRepository,
            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor) {
            _caseArchiveRepository = caseArchiveRepository;
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
        }

        public void RebuildArchive(CasePart casePart) {

            var first = _contentManager.Query<CasePostPart>().Where<CommonPartRecord>(bp => bp.Container.Id == casePart.Id).OrderBy<CommonPartRecord>(x => x.CreatedUtc).Slice(0, 1).FirstOrDefault();

            if (first == null) {
                return;
            }

            var last = _contentManager.Query<CasePostPart>().Where<CommonPartRecord>(bp => bp.Container.Id == casePart.Id).OrderByDescending<CommonPartRecord>(x => x.CreatedUtc).Slice(0, 1).FirstOrDefault();

            DateTime? start = DateTime.MaxValue;
            if (first.As<CommonPart>() != null) {
                start = first.As<CommonPart>().CreatedUtc;
            }

            DateTime? end = DateTime.MinValue;
            if (last.As<CommonPart>() != null) {
                end = last.As<CommonPart>().CreatedUtc;
            }

            // delete previous archive records
            foreach (var record in _caseArchiveRepository.Table.Where(x => x.CasePart.Id == casePart.Id)) {
                _caseArchiveRepository.Delete(record);
            }

            if (!start.HasValue || !end.HasValue) {
                return;
            }

            // get the time zone for the current request
            var timeZone = _workContextAccessor.GetContext().CurrentTimeZone;

            // build a collection of all the post dates
            var casePostDates = new List<DateTime>();
            var casePosts = _contentManager.Query<CasePostPart>().Where<CommonPartRecord>(bp => bp.Container.Id == casePart.Id);
            foreach (var casePost in casePosts.List()) {
                if (casePost.As<CommonPart>() != null)
                    if (casePost.As<CommonPart>().CreatedUtc.HasValue) {
                        DateTime timeZoneAdjustedCreatedDate = TimeZoneInfo.ConvertTimeFromUtc(casePost.As<CommonPart>().CreatedUtc.Value, timeZone);
                        casePostDates.Add(timeZoneAdjustedCreatedDate);
                    }
            }

            for (int year = start.Value.Year; year <= end.Value.Year; year++) {
                for (int month = 1; month <= 12; month++) {
                    var fromDateUtc = new DateTime(year, month, 1);
                    var from = TimeZoneInfo.ConvertTimeFromUtc(fromDateUtc, timeZone);
                    var to = TimeZoneInfo.ConvertTimeFromUtc(fromDateUtc.AddMonths(1), timeZone);

                    // skip the first months of the first year until a month has posts
                    //  for instance, if started posting in May 2000, don't write counts for Jan 200 > April 2000... start May 2000
                    if (from < TimeZoneInfo.ConvertTimeFromUtc(new DateTime(start.Value.Year, start.Value.Month, 1), timeZone))
                        continue;
                    // skip the last months of the last year if no posts
                    //  for instance, no need to have archives for months in the future
                    if (to > end.Value.AddMonths(1))
                        continue;

                    //var count = _contentManager.Query<CasePostPart>().Where<CommonPartRecord>(x => x.CreatedUtc.Value >= from && x.CreatedUtc.Value < to).Count();
                    var count = casePostDates.Count(bp => bp >= @from && bp < to);

                    var newArchiveRecord = new CasePartArchiveRecord { CasePart = casePart.ContentItem.Record, Year = year, Month = month, PostCount = count };
                    _caseArchiveRepository.Create(newArchiveRecord);
                }
            }
        }
    }
}