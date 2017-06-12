using System.Linq;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.Settings;
using Orchard.ContentManagement;
using Orchard;
using LawBookCases.Module.Services;
using Orchard.Core.Feeds;
using LawBookCases.Module.Models;
using LawBookCases.Module.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LawBookCases.Module.Controllers {

    [Themed]
    public class CaseController : Controller {
        private readonly IOrchardServices _services;
        private readonly ICaseService _cAseService;
        private readonly ICasePostService _cAsePostService;
        private readonly IFeedManager _feedManager;
        private readonly ISiteService _siteService;

        public CaseController(
            IOrchardServices services, 
            ICaseService cAseService,
            ICasePostService cAsePostService,
            IFeedManager feedManager, 
            IShapeFactory shapeFactory,
            ISiteService siteService) {
            _services = services;
            _cAseService = cAseService;
            _cAsePostService = cAsePostService;
            _feedManager = feedManager;
            _siteService = siteService;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        protected ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ActionResult List() {
            var cAses = _cAseService.Get()
                .Where(b => _services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent,b))
                .Select(b => _services.ContentManager.BuildDisplay(b, "Summary"));

            var list = Shape.List();
            list.AddRange(cAses);

            var viewModel = Shape.ViewModel();
    //            .ContentItems(list);
    //IContainer _container;
           
    //        Resolve(_container);

            return View(viewModel);
        }

        public ActionResult Item(int cAseId, PagerParameters pagerParameters) {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var cAsePart = _cAseService.Get(cAseId, VersionOptions.Published).As<CasePart>();
            if (cAsePart == null)
                return HttpNotFound();

            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, cAsePart, T("Cannot view content"))) {
                return new HttpUnauthorizedResult();
            }

             pager.PageSize = cAsePart.PostsPerPage;

            _feedManager.Register(cAsePart, _services.ContentManager.GetItemMetadata(cAsePart).DisplayText);
            var cAsePosts = _cAsePostService.Get(cAsePart, pager.GetStartIndex(), pager.PageSize)
                .Select(b => _services.ContentManager.BuildDisplay(b, "Summary"));
            dynamic cAse = _services.ContentManager.BuildDisplay(cAsePart);

            var list = Shape.List();
            list.AddRange(cAsePosts);
            
            cAse.Content.Add(Shape.Parts_Cases_CasePost_List(ContentItems: list), "5");

            var totalItemCount = _cAsePostService.PostCount(cAsePart);
            cAse.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");


           return new ShapeResult(this, cAse);
        }

       
        public JsonResult GetYears()
        {
            var json = _cAsePostService.GetCaseYears();
            List<string> years = new List<string>();
            foreach(int year in json)
            {
                years.Add(year.ToString());
            }
            return Json(years, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CaseByYear(int byYear, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);


            string caseType = ""; getCaseType();

            var cAsePart = _cAseService.Get(caseType);

                      
            if (cAsePart == null) 
                return HttpNotFound();

                       

            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, cAsePart, T("Cannot view content")))
            {
                return new HttpUnauthorizedResult();

            }
            pager.PageSize = cAsePart.PostsPerPage;

            _feedManager.Register(cAsePart, _services.ContentManager.GetItemMetadata(cAsePart).DisplayText);

            _feedManager.Register(cAsePart, _services.ContentManager.GetItemMetadata(cAsePart).DisplayText);
            var cAsePostsview = _cAsePostService.Get(cAsePart, byYear,  pager.GetStartIndex(), pager.PageSize)
                .Select(b => _services.ContentManager.BuildDisplay(b, "Summary"));
            dynamic cAse = _services.ContentManager.BuildDisplay(cAsePart);
                                 
            var list = Shape.List();
            list.AddRange(cAsePostsview);
            cAse.Content.Add(Shape.Parts_Cases_CasePost_List(ContentItems: list), "5");

            var totalItemCount = _cAsePostService.PostCount(cAsePart);
            cAse.Content.Add(Shape.Pager(pager).TotalItemCount(totalItemCount), "Content:after");


            return new ShapeResult(this, cAse);
        }
        public ActionResult Docs(int cAseId, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var cAsePostPart = _cAsePostService.Get(cAseId);
               // .Select(b => _services.ContentManager.BuildDisplay(b, "Details"));
            ;// _cAseService.Get(cAseId, VersionOptions.Published).As<CasePart>();
            if (cAsePostPart == null)
                return HttpNotFound();

            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, cAsePostPart, T("Cannot view content")))
            {
                return new HttpUnauthorizedResult();
            }

           

           dynamic cAse = _services.ContentManager.BuildDisplay(cAsePostPart);

            
            return new ShapeResult(this, cAse);
        }

        public ActionResult CaseByYearAndNumber(int byYear,int cAseId, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            string caseType = ""; getCaseType();

            var cAsePart = _cAseService.Get(caseType);

            var cAsePosts = _cAsePostService.Get(cAsePart, byYear, pager.GetStartIndex(), pager.PageSize);
              

      //      var cAsePosts = _cAseService.GetCasesByYear(byYear, VersionOptions.Published);

            if (cAsePosts == null | cAsePosts.Count()<1)
                return HttpNotFound();

            var CasePart = cAsePosts.Where(y => y.Get<CasePostAttribPart>().CaseNumber == cAseId).FirstOrDefault();

            // .Select(b => _services.ContentManager.BuildDisplay(b, "Details"));
            ;// _cAseService.Get(cAseId, VersionOptions.Published).As<CasePart>();
            if (CasePart == null)
                return HttpNotFound();

            if (!_services.Authorizer.Authorize(Orchard.Core.Contents.Permissions.ViewContent, CasePart, T("Cannot view content")))
            {
                return new HttpUnauthorizedResult();
            }
bi
             dynamic cAse = _services.ContentManager.BuildDisplay(CasePart);


            return new ShapeResult(this, cAse);
        }

        private string getCaseType() {
            string curexepath= this.HttpContext.Request.AppRelativeCurrentExecutionFilePath;

            string [] patht=curexepath.Split('/');

            return patht[1];
          }
    }
}
