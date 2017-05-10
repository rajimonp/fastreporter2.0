using LawBookCases.Module.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LawBookCases.Module.Handlers
{
    public class CasePostAttribPartHandler: ContentHandler
    {
        private int currentCaseIndx = 0;
        public CasePostAttribPartHandler(IRepository<CasePostAttribRecord> repository)
            {
                Filters.Add(StorageFilter.For(repository));


            OnInitializing<CasePostAttribPart>((context, attributePart) => {
                attributePart.currentCaseIndx = ++currentCaseIndx;
            });
        }
         
    }
}