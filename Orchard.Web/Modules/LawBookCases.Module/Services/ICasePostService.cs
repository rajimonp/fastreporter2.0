using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using LawBookCases.Module.Models;
using Orchard;

namespace LawBookCases.Module.Services {
    public interface ICasePostService : IDependency {
        CasePostPart Get(int id);
        CasePostPart Get(int id, VersionOptions versionOptions);
        IEnumerable<CasePostPart> Get(CasePart blogPart);
        IEnumerable<CasePostPart> Get(CasePart blogPart, VersionOptions versionOptions);
        IEnumerable<CasePostPart> Get(CasePart blogPart, ArchiveData archiveData);
        IEnumerable<CasePostPart> Get(CasePart blogPart, int skip, int count);
        IEnumerable<CasePostPart> Get(CasePart blogPart, int skip, int count, VersionOptions versionOptions);
        int PostCount(CasePart blogPart);
        int PostCount(CasePart blogPart, VersionOptions versionOptions);
        IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives(CasePart blogPart);
        void Delete(CasePostPart blogPostPart);
        void Publish(CasePostPart blogPostPart);
        void Publish(CasePostPart blogPostPart, DateTime scheduledPublishUtc);
        void Unpublish(CasePostPart blogPostPart);
        DateTime? GetScheduledPublishUtc(CasePostPart blogPostPart);
        int GetNextCaseNumber(int Year);
        void UpdaetCasePostAttributes(CasePostAttribRecord record);
        IEnumerable<int> GetCaseYears();
        IEnumerable<CasePostPart> Get(CasePart cAsePart, int year, int skip, int count);
        IEnumerable<CasePostStateRecord> GetCaseStates(int id);

        void Acquier(CasePostStateRecord rec);
        void GetCurrentState(CasePostPart postpart);
        bool isInterAcquiered(int id);
    }
}