using LawBookCases.Module.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LawBookCases.Module.Handlers
{
    [OrchardFeature("LawBook.Module.Contents")]
    public class CasePostHeldAtHandler: ContentHandler
    {
        public CasePostHeldAtHandler(IRepository<CasePostHeldAtPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }


}