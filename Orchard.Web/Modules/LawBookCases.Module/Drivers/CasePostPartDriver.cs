using LawBookCases.Module.Extensions;
using LawBookCases.Module.Models;
using LawBookCases.Module.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Feeds;
using Orchard.Security;
using Orchard.Users.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LawBookCases.Module.Drivers {
    public class CasePostPartDriver : ContentPartDriver<CasePostPart> {
        private readonly IFeedManager _feedManager;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICommonService _commonService;

        public CasePostPartDriver(IFeedManager feedManager, IContentManager contentManager, IWorkContextAccessor workContextAccessor, ICommonService commonService) {
            _feedManager = feedManager;
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _commonService=commonService;
    }

        protected override DriverResult Display(CasePostPart part, string displayType, dynamic shapeHelper) {
            if (displayType.StartsWith("Detail")) {
                var cAseTitle = _contentManager.GetItemMetadata(part.CasePart).DisplayText;
                _feedManager.Register(part.CasePart, cAseTitle);
            }
            CasePostAttribPart attribute = part.Get<CasePostAttribPart>();
            attribute.CourtList = _commonService.GetCourtList();
            foreach (SelectListItem si in attribute.GenreList)
            {
                if (attribute.CaseHeldIn == si.Value)
                {
                    
                    attribute.CaseHeldIn = si.Text;
                }

            }
            foreach (SelectListItem si in attribute.CourtList)
            {
                if (attribute.CaseHeldCourt == si.Value)
                {
                    
                    attribute.CaseHeldCourt = si.Text;
                }

            }
            return null;
        }
        protected override DriverResult Editor(CasePostPart part, dynamic shapeHelper)
        {
            IUser user = _workContextAccessor.GetContext().CurrentUser;
            dynamic dynUser = user.ContentItem;
            
            string role = string.Empty; ;
            if (dynUser.UserRolesPart != null)
            {
                IEnumerable<string> usernames = dynUser.UserRolesPart.Roles;
                foreach (var unameloop in usernames)
                {
                    role = unameloop;
                    break;
                }
            }
            CasePostAttribPart attribute = part.Get<CasePostAttribPart>();

            attribute.CourtList = _commonService.GetCourtList();
       
                foreach (SelectListItem si in attribute.GenreList) {
                if(attribute.CaseHeldIn == si.Value)
                {
                    si.Selected = true;
                  
                }

               }
            foreach (SelectListItem si in attribute.CourtList)
            {
                if (attribute.CaseHeldCourt == si.Value)
                {
                    si.Selected = true;
                    
                }

            }



            var results = new List<DriverResult>();
            if (role.Equals("Law Interns"))
                results = new List<DriverResult> { ContentShape("Case_UpdateAbstract", saveButton => saveButton) };
            if (role.Equals("Editor"))
                results = new List<DriverResult> { ContentShape("Case_UpdateAbstract", saveButton => saveButton) };
            if (role.Equals("Author"))
                results = new List<DriverResult> { ContentShape("Case_UpdateAuthor", saveButton => saveButton) };
            //Case.UpdateAuthor
            return Combined(results.ToArray());
        }
    }
}