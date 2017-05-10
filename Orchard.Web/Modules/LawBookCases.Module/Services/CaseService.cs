using System.Collections.Generic;
using System.Linq;
using Orchard.Autoroute.Models;
using Orchard.Autoroute.Services;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.State;
using LawBookCases.Module.Models;
using System;

namespace LawBookCases.Module.Services {
    public class CaseService : ICaseService {
        private readonly IContentManager _contentManager;
        private readonly IProcessingEngine _processingEngine;
        private readonly ShellSettings _shellSettings;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly HashSet<int> _processedCaseParts = new HashSet<int>();
        IPathResolutionService _pathResolutionService;

        public CaseService(
            IContentManager contentManager,
            IProcessingEngine processingEngine,
            ShellSettings shellSettings,
            IShellDescriptorManager shellDescriptorManager,
            IPathResolutionService pathResolutionService) {
            _contentManager = contentManager;
            _processingEngine = processingEngine;
            _shellSettings = shellSettings;
            _shellDescriptorManager = shellDescriptorManager;
            _pathResolutionService = pathResolutionService;
        }

        public CasePart Get(string path) {
            var cAse = _pathResolutionService.GetPath(path);

            if (cAse == null) {
                return null;
            }

            if (!cAse.Has<CasePart>()) {
                return null;
            }

            return cAse.As<CasePart>();
        }

        public ContentItem Get(int id, VersionOptions versionOptions) {
            var cAsePart = _contentManager.Get<CasePart>(id, versionOptions);
            return cAsePart == null ? null : cAsePart.ContentItem;
        }

        public IEnumerable<CasePart> Get() {
            return Get(VersionOptions.Published);
        }

        public IEnumerable<CasePart> Get(VersionOptions versionOptions) {
            return _contentManager.Query<CasePart>(versionOptions, "Case")
                .Join<TitlePartRecord>()
                .OrderBy(br => br.Title)
                .List();
        }

        public void Delete(ContentItem cAse) {
            _contentManager.Remove(cAse);
        }

        public void ProcessCasePostsCount(int cAsePartId) {
            if (!_processedCaseParts.Contains(cAsePartId)) {
                _processedCaseParts.Add(cAsePartId);
                _processingEngine.AddTask(_shellSettings, _shellDescriptorManager.GetShellDescriptor(), "ICasePostsCountProcessor.Process", new Dictionary<string, object> { { "cAsePartId", cAsePartId } });
            }
        }

        public IEnumerable<CasePostPart> GetCasesByYear(int year, VersionOptions published)
        {
            IEnumerable<CasePostPart> casePostParts= _contentManager.Query<CasePostPart> (published, "CasePost")
                .Join< CasePostAttribRecord>()
                .Where(y=>y.CaseYear == year)
                .List();
            return casePostParts;

        }

        public int getCaseType(string caseType)
        {
             
            //     _contentManager.Query<CasePart>(VersionOptions.Published, "Case")
               // .Join<TitlePartRecord>()
           //     .Where((br=>br.Title==caseType) ).Where(cs=>cs.)
           //     .List();

            throw new NotImplementedException();
        }
    }
}