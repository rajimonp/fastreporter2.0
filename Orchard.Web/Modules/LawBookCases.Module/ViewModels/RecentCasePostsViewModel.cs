using LawBookCases.Module.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LawBookCases.Module.ViewModels {
    public class RecentCasePostsViewModel {
        [Required]
        public int Count { get; set; }
        
        [Required]
        public int CaseNumber { get; set; }

        public IEnumerable<CasePart> Cases { get; set; }

    }
}