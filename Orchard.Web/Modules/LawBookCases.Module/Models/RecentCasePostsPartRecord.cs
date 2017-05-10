using Orchard.ContentManagement.Records;

namespace LawBookCases.Module.Models {
    public class RecentCasePostsPartRecord : ContentPartRecord {
        public RecentCasePostsPartRecord() {
            Count = 5;
        }

        public virtual int CaseNumber { get; set; }
        public virtual int Count { get; set; }
    }
}
