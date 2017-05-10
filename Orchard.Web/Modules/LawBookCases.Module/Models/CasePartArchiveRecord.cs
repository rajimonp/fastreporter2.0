using Orchard.ContentManagement.Records;

namespace LawBookCases.Module.Models {
    public class CasePartArchiveRecord {
        public virtual int Id { get; set; }
        public virtual ContentItemRecord CasePart { get; set; }
        public virtual int Year { get; set; }
        public virtual int Month { get; set; }
        public virtual int PostCount { get; set; }
    }
}