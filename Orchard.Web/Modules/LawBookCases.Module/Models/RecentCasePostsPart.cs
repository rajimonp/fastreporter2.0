using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace LawBookCases.Module.Models {
    public class RecentCasePostsPart : ContentPart<RecentCasePostsPartRecord> {

        public int CaseNumber {
            get { return Record.CaseNumber; }
            set { Record.CaseNumber = value; }
        }

        [Required]
        public int Count {
            get { return Record.Count; }
            set { Record.Count = value; }
        }
    }
}
