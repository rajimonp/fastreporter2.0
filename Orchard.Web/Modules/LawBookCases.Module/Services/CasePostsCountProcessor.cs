using LawBookCases.Module.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;

namespace LawBookCases.Module.Services {
    public class CasePostsCountProcessor : ICasePostsCountProcessor {
        private readonly IContentManager _contentManager;

        public CasePostsCountProcessor(
            IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public void Process(int casePartId) {
            var casePart = _contentManager.Get<CasePart>(casePartId);
            if (casePart != null) {
                var count = _contentManager.Query(VersionOptions.Published, "CasePost")
                    .Join<CommonPartRecord>().Where(
                        cr => cr.Container.Id == casePartId)
                    .Count();

                casePart.PostCount = count;
            }
        }
    }
}