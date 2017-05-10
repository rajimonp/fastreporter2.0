using LawBookCases.Module.Models;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LawBookCases.Module.Services    
{
    public class CommonService : ICommonService
    {
        private readonly IRepository<CasePostHeldAtPartRecord> _casePostHeldpat;

        public CommonService(IRepository<CasePostHeldAtPartRecord> casePostHeldpat) {
            _casePostHeldpat = casePostHeldpat;

        }
        public IEnumerable<SelectListItem> GetCourtList()
        {
            var crtList = _casePostHeldpat.Table.ToList().Select(o=>
                                   new SelectListItem
                                   {
                                       Value = o.Id.ToString(),
                                       Text=o.CourtName
                                   }).ToList();
            return crtList;
        }
    }
}