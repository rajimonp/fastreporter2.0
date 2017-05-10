using LawBookCases.Module.Models;
using Orchard;


namespace LawBookCases.Module.Services {
    public interface IArchiveService : IDependency {
        void RebuildArchive(CasePart blocAse);
    }
}