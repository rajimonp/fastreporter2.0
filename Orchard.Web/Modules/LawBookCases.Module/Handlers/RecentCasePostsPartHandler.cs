using LawBookCases.Module.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace LawBookCases.Module.Handlers
{
    public class RecentCasePostsPartHandler : ContentHandler {
        public RecentCasePostsPartHandler(IRepository<RecentCasePostsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}