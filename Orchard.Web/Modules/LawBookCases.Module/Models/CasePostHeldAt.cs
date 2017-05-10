using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LawBookCases.Module.Models
{

    [OrchardFeature("LawBookCases.Module.Attributes")]
    public class CasePostHeldAtPartRecord : ContentPartRecord
    {
        public virtual string CourtName { get; set; }
        public virtual string CourtType { get; set; }

    }


    [OrchardFeature("LawBookCases.Module.Attributes")]
    public class CasePostHeldAtPart : ContentPart<CasePostHeldAtPartRecord>
    {
        public string CourtName
        {
            get
            {
                return Retrieve(r => r.CourtName);
            }
            set
            {
                Store(r => r.CourtName, value);

            }
        }



        public string CourtType
        {
            get
            {
                return Retrieve(r => r.CourtType);
            }
            set
            {
                Store(r => r.CourtType, value);

            }
        }

    }
}