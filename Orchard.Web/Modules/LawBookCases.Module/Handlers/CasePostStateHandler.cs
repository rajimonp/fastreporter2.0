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
    public class CasePostStateHandler: ContentHandler
    {
        public CasePostStateHandler(IRepository<CasePostStateRecord> repository)
        {
         //   Filters.Add(StorageFilter.For(repository));
        }
    }
}