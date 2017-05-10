using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Settings;
using Orchard;
using LawBookCases.Module.Models;
using LawBookCases.Module.Services;
using LawBookCases.Module.Extensions;
using Orchard.Security;
using System.Collections.Generic;

namespace LawBookCases.Module.Controllers {

    [ValidateInput(false), Admin]
    public class CaseAdminController : Controller, IUpdateModel {
        private readonly ICaseService _caseService;
        private readonly ICasePostService _casePostService;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ISiteService _siteService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CaseAdminController(
            IOrchardServices services,
            ICaseService caseService,
            ICasePostService casePostService,
            IContentManager contentManager,
            ITransactionManager transactionManager,
            ISiteService siteService,
            IShapeFactory shapeFactory, IWorkContextAccessor workContextAccessor, IAuthorizationService authorizationService) {
            Services = services;
            _caseService = caseService;
            _casePostService = casePostService;
            _contentManager = contentManager;
            _transactionManager = transactionManager;
            _siteService = siteService;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Create() {
            if (!Services.Authorizer.Authorize(Permissions.ManageCases, T("Not allowed to create case")))
                return new HttpUnauthorizedResult();

            CasePart cAse = Services.ContentManager.New<CasePart>("Case");
            if (cAse == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(cAse);
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            if (!Services.Authorizer.Authorize(Permissions.ManageCases, T("Couldn't create case")))
                return new HttpUnauthorizedResult();

            var cAse = Services.ContentManager.New<CasePart>("Case");

            _contentManager.Create(cAse, VersionOptions.Draft);
            var model = _contentManager.UpdateEditor(cAse, this);

            if (!ModelState.IsValid) {
                _transactionManager.Cancel();
                return View(model);
            }

            _contentManager.Publish(cAse.ContentItem);
            return Redirect(Url.CaseForAdmin(cAse));
        }

        public ActionResult Edit(int caseId) {
            var cAse = _caseService.Get(caseId, VersionOptions.Latest);

            if (!Services.Authorizer.Authorize(Permissions.ManageCases, cAse, T("Not allowed to edit case")))
                return new HttpUnauthorizedResult();

            if (cAse == null)
                return HttpNotFound();

            var model = Services.ContentManager.BuildEditor(cAse);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Delete")]
        public ActionResult EditDeletePOST(int caseNumber) {
            if (!Services.Authorizer.Authorize(Permissions.ManageCases, T("Couldn't delete case")))
                return new HttpUnauthorizedResult();

            var cAse = _caseService.Get(caseNumber, VersionOptions.DraftRequired);
            if (cAse == null)
                return HttpNotFound();
            _caseService.Delete(cAse);

            Services.Notifier.Information(T("Case deleted"));

            return Redirect(Url.CasesForAdmin());
        }


        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int caseId) {
            var cAse = _caseService.Get(caseId, VersionOptions.DraftRequired);

            if (!Services.Authorizer.Authorize(Permissions.ManageCases, cAse, T("Couldn't edit case")))
                return new HttpUnauthorizedResult();

            if (cAse == null)
                return HttpNotFound();

            var model = Services.ContentManager.UpdateEditor(cAse, this);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return View(model);
            }

            _contentManager.Publish(cAse);
            Services.Notifier.Information(T("Case information updated"));

            return Redirect(Url.CasesForAdmin());
        }

        [HttpPost]
        public ActionResult Remove(int caseId) {
            if (!Services.Authorizer.Authorize(Permissions.ManageCases, T("Couldn't delete case")))
                return new HttpUnauthorizedResult();

            var cAse = _caseService.Get(caseId, VersionOptions.Latest);

            if (cAse == null)
                return HttpNotFound();

            _caseService.Delete(cAse);

            Services.Notifier.Information(T("Case was successfully deleted"));
            return Redirect(Url.CasesForAdmin());
        }

        public ActionResult List() {
            var list = Services.New.List();
            list.AddRange(_caseService.Get(VersionOptions.Latest)
                .Where(x => Services.Authorizer.Authorize(Permissions.MetaListOwnCases, x))
                .Select(b => {
                            var cAse = Services.ContentManager.BuildDisplay(b, "SummaryAdmin");
                        cAse.TotalPostCount = _casePostService.PostCount(b, VersionOptions.Latest);
                            return cAse;
                        }));

            var viewModel = Services.New.ViewModel()
                .ContentItems(list);
            return View(viewModel);
        }

        public ActionResult Item(int caseId, PagerParameters pagerParameters) {


            IUser user= _workContextAccessor.GetContext().CurrentUser;
            dynamic dynUser = user.ContentItem;

            if (dynUser.UserRolesPart != null)
            {
                IEnumerable<string> userRoles = dynUser.UserRolesPart.Roles;
              //  isInRole = userRoles.Any(roles.Contains);
            }

            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            CasePart casePart = _caseService.Get(caseId, VersionOptions.Latest).As<CasePart>();

            if (casePart == null)
                return HttpNotFound();

            var casePosts = _casePostService.Get(casePart, pager.GetStartIndex(), pager.PageSize, VersionOptions.Latest);

            //filtrt all not for intern 
            

            //   var cAses = _cAseService.Get().Where(x => _authorizationService.TryCheckAccess(Permissions.MetaListCases, _workContextAccessor.GetContext().CurrentUser, x)).ToArray();
        //    bool isLawInter=_authorizationService.TryCheckAccess(Permissions.ListunpublishedCases, _workContextAccessor.GetContext().CurrentUser, casePart);
          //  if (isLawInter)
        // //   {
       //         casePosts = casePosts.Where(pub => pub.IsPublished == false).Where(  pub=> _casePostService.isInterAcquiered(pub.ContentItem.Id));
         //   }
              
           

            casePosts = casePosts.ToArray();
            foreach(CasePostPart postpart in casePosts)
            {
                _casePostService.GetCurrentState(postpart);
            }

            //_authorizationService.TryCheckAccess()
            

            var casePostsShapes = casePosts.Select(bp => _contentManager.BuildDisplay(bp, "SummaryAdmin")).ToArray();

          
            var cAse = Services.ContentManager.BuildDisplay(casePart, "DetailAdmin");

            var list = Shape.List();
            list.AddRange(casePostsShapes);
            cAse.Content.Add(Shape.Parts_Cases_CasePost_ListAdmin(ContentItems: list), "5");

            var totalItemCount = _casePostService.PostCount(casePart, VersionOptions.Latest);
            //if (isLawInter)
             //   totalItemCount = _casePostService.PostCount(casePart, VersionOptions.Draft);

            cAse.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");

            return View(cAse);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}