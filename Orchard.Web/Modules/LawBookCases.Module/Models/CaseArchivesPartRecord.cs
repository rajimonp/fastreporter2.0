using Orchard.ContentManagement.Records;

namespace LawBookCases.Module.Models {
    /// <summary>
    /// The content part used by the CaseArchives widget
    /// </summary>
    public class CaseArchivesPartRecord : ContentPartRecord {
        public virtual int CaseNumber { get; set; }
    }
}
