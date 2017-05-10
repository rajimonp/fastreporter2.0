using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LawBookCases.Module.Models
 {
    [OrchardFeature("LawBookCases.Module.CasePostState")]

    public class CasePostStateRecord:ContentPartRecord
    {
        
        public virtual int CasePostPart_id { get; set; }
        public virtual string CasePostState  { get; set; }
        public virtual int CasePostStateUserId { get; set; }
        public virtual DateTime? CasePostStateUtc { get; set; }
        public virtual int CaseAcquiredStatus { get; set; }

        public enum CaseAcquiredStateEnum
        {
            NOTAcquired=0,
            INTERNAcquired,
            INTERNReleased,
            EDITORAcquired,
            EDITORReleased,
            AUTHORAcquired,
            AUTHORReleased
        }
    }

}