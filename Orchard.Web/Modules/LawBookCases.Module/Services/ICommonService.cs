using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LawBookCases.Module.Services
{
    public interface ICommonService : IDependency
    {
        IEnumerable<SelectListItem> GetCourtList();
    }
}
