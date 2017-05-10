using LawBookCases.Module.Models;
using System.Collections.Generic;


namespace LawBookCases.Module.ViewModels {
    public class CaseArchivesViewModel {
        public int CaseNumber { get; set; }
        public IEnumerable<CasePart> Cases { get; set; }
    }
}