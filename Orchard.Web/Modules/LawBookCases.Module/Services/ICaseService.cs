using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Services {
    public interface ICaseService : IDependency {
        CasePart Get(string path);
        ContentItem Get(int id, VersionOptions versionOptions);
        IEnumerable<CasePart> Get();
        IEnumerable<CasePart> Get(VersionOptions versionOptions);
        void Delete(ContentItem cAse);
        void ProcessCasePostsCount(int casePartId);
        IEnumerable<CasePostPart> GetCasesByYear(int year, VersionOptions published);
        int getCaseType(string casType);
    }
}