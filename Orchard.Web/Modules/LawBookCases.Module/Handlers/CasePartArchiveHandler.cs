using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using LawBookCases.Module.Models;
using Orchard;
using LawBookCases.Module.Services;

namespace LawBookCases.Module.Handlers {
    public class CasePartArchiveHandler : ContentHandler {
        private readonly IRepository<CasePartArchiveRecord> _blogArchiveRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IContentManager _contentManager;
        // contains the creation time of a blog part before it has been changed
        private readonly Dictionary<int, DateTime> _previousCreatedUtc = new Dictionary<int,DateTime>();

        public CasePartArchiveHandler(
            IRepository<CasePartArchiveRecord> blogArchiveRepository, 
            ICasePostService blogPostService,
            IWorkContextAccessor workContextAccessor,
            IContentManager contentManager) {
            _blogArchiveRepository = blogArchiveRepository;
            _workContextAccessor = workContextAccessor;
            _contentManager = contentManager;

            OnUpdating<CommonPart>((context, cp) => { if(context.ContentItem.Has<CasePostPart>()) SavePreviousCreatedDate(context.Id);});
            OnRemoving<CasePostPart>((context, bp) => SavePreviousCreatedDate(context.Id));
            OnUnpublishing<CasePostPart>((context, bp) => SavePreviousCreatedDate(context.Id));

            OnPublished<CasePostPart>((context, bp) => IncreaseCaseArchive(bp));
            OnUnpublished<CasePostPart>((context, bp) => ReduceCaseArchive(bp));
            OnRemoved<CasePostPart>((context, bp) => ReduceCaseArchive(bp));
        }

        private void SavePreviousCreatedDate(int contentItemId) {
            if (_previousCreatedUtc.ContainsKey(contentItemId)) {
                return;
            }

            var previousPublishedVersion = _contentManager.Get(contentItemId, VersionOptions.Published);

            // retrieve the creation date when it was published
            if (previousPublishedVersion != null) {
                var versionCommonPart = previousPublishedVersion.As<ICommonPart>();
                if (versionCommonPart.CreatedUtc.HasValue) {
                    _previousCreatedUtc[contentItemId] = versionCommonPart.CreatedUtc.Value;
                }
            }
        }

        private void ReduceCaseArchive(CasePostPart blogPostPart) {
            _blogArchiveRepository.Flush();

            // don't reduce archive count if the content item is not published
            if (!_previousCreatedUtc.ContainsKey(blogPostPart.Id)) {
                return;
            }

            var commonPart = blogPostPart.As<ICommonPart>();
            if (commonPart == null || !commonPart.CreatedUtc.HasValue)
                return;

            var timeZone = _workContextAccessor.GetContext().CurrentTimeZone;
            var datetime = TimeZoneInfo.ConvertTimeFromUtc(commonPart.CreatedUtc.Value, timeZone);

            var previousArchiveRecord = _blogArchiveRepository.Table
                .FirstOrDefault(x => x.CasePart.Id == blogPostPart.CasePart.Id
                                    && x.Month == datetime.Month
                                    && x.Year == datetime.Year);

            if(previousArchiveRecord == null)
                return;

            if (previousArchiveRecord.PostCount > 1)
                previousArchiveRecord.PostCount--;
            else
                _blogArchiveRepository.Delete(previousArchiveRecord);
        }

        private void IncreaseCaseArchive(CasePostPart blogPostPart) {
            _blogArchiveRepository.Flush();
            
            var commonPart = blogPostPart.As<ICommonPart>();
            if(commonPart == null || !commonPart.CreatedUtc.HasValue)
                return;

            // get the time zone for the current request
            var timeZone = _workContextAccessor.GetContext().CurrentTimeZone;

            var previousCreatedUtc = _previousCreatedUtc.ContainsKey(blogPostPart.Id) ? _previousCreatedUtc[blogPostPart.Id] : DateTime.MinValue;
            previousCreatedUtc = TimeZoneInfo.ConvertTimeFromUtc(previousCreatedUtc, timeZone);

            var previousMonth = previousCreatedUtc.Month;
            var previousYear = previousCreatedUtc.Year;

            var newCreatedUtc = commonPart.CreatedUtc;
            newCreatedUtc = TimeZoneInfo.ConvertTimeFromUtc(newCreatedUtc.Value, timeZone);

            var newMonth = newCreatedUtc.Value.Month;
            var newYear = newCreatedUtc.Value.Year;

            // if archives are the same there is nothing to do
            if (previousMonth == newMonth && previousYear == newYear) {
                return;
            }
            
            // decrement previous archive record
            var previousArchiveRecord = _blogArchiveRepository
                .Table
                .FirstOrDefault(x => x.CasePart.Id == blogPostPart.CasePart.Id
                                     && x.Month == previousMonth
                                     && x.Year == previousYear);

            if (previousArchiveRecord != null && previousArchiveRecord.PostCount > 0) {
                previousArchiveRecord.PostCount--;
            }

            // if previous count is now zero, delete the record
            if (previousArchiveRecord != null && previousArchiveRecord.PostCount == 0) {
                _blogArchiveRepository.Delete(previousArchiveRecord);
            }
            
            // increment new archive record
            var newArchiveRecord = _blogArchiveRepository
                .Table
                .FirstOrDefault(x => x.CasePart.Id == blogPostPart.CasePart.Id
                                     && x.Month == newMonth
                                     && x.Year == newYear);

            // if record can't be found create it
            if (newArchiveRecord == null) {
                newArchiveRecord = new CasePartArchiveRecord { CasePart = blogPostPart.CasePart.ContentItem.Record, Year = newYear, Month = newMonth, PostCount = 0 };
                _blogArchiveRepository.Create(newArchiveRecord);
            }

            newArchiveRecord.PostCount++;            
        }
    }
}