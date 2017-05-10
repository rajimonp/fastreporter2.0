
using LawBookCases.Module.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace LawBookCases.Module.Handlers {
    public class CaseArchivesPartHandler : ContentHandler {
        public CaseArchivesPartHandler(IRepository<CaseArchivesPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}