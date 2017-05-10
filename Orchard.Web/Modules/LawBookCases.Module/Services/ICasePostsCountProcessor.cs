using Orchard.Events;

namespace LawBookCases.Module.Services {
    public interface ICasePostsCountProcessor : IEventHandler {
        void Process(int casePartId);
    }
}