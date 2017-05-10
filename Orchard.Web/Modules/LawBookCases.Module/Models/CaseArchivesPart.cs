using Orchard.ContentManagement;

namespace LawBookCases.Module.Models {
    /// <summary>
    /// The content part used by the CaseArchives widget
    /// </summary>
    public class CaseArchivesPart : ContentPart<CaseArchivesPartRecord> {

        public int CaseNumber {
            get { return Record.CaseNumber; }
            set { Record.CaseNumber = value; }
        }
    }
}
