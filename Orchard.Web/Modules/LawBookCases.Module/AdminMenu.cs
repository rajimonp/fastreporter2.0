using System.Linq;

using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using LawBookCases.Module.Services;
using Orchard;

namespace LawBookCases.Module{
    public class AdminMenu : INavigationProvider {
        private readonly ICaseService _cAseService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public AdminMenu(ICaseService cAseService, IAuthorizationService authorizationService, IWorkContextAccessor workContextAccessor) {
            _cAseService = cAseService;
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;
        }

        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("blog")
                .Add(T("Case"), "1.5", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu) {

            //For 
            var cAses = _cAseService.Get().ToArray();


          // var cAses = _cAseService.Get().Where(x => _authorizationService.TryCheckAccess(Permissions.MetaListCases, _workContextAccessor.GetContext().CurrentUser, x)).ToArray();
            var cAseCount = cAses.Count();
            var singleCase = cAseCount == 1 ? cAses.ElementAt(0) : null;

            if (cAseCount > 0 && singleCase == null) {
                menu.Add(T("Manage Cases"), "3",
                         item => item.Action("List", "CaseAdmin", new { area = "LawBookCases.Module" }).Permission(Permissions.MetaListOwnCases));
            }
            else if (singleCase != null)
                menu.Add(T("Manage Case"), "1.0",
                    item => item.Action("Item", "CaseAdmin", new { area = "LawBookCases.Module", cAseId = singleCase.Id }).Permission(Permissions.MetaListOwnCases));

            if (singleCase != null)
                menu.Add(T("New Post"), "1.1",
                         item =>
                         item.Action("Create", "CasePostAdmin", new {area = "LawBookCases.Module", cAseId = singleCase.Id}).Permission(Permissions.MetaListOwnCases));

            menu.Add(T("New Case"), "1.2",
                     item =>
                     item.Action("Create", "CaseAdmin", new { area = "LawBookCases.Module" }).Permission(Permissions.ManageCases));

        }
    }
}