using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.Core.Feeds;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using Orchard;
using LawBookCases.Module.Services;
using LawBookCases.Module.Routing;
using LawBookCases.Module.Models;
using LawBookCases.Module.Extensions;
using Orchard.UI.Navigation;
using Orchard.ContentManagement;
using Orchard.Settings;

namespace LawBookCases.Module.Controllers {
    [Themed]
    public class CasePostController : Controller {
        private readonly IOrchardServices _services;
        private readonly ICaseService _cAseService;
        private readonly ICasePostService _cAsePostService;
        private readonly IFeedManager _feedManager;
        private readonly IArchiveConstraint _archiveConstraint;
        private ISiteService _siteService;

        public CasePostController(
            IOrchardServices services, 
            ICaseService cAseService, 
            ICasePostService cAsePostService,
            IFeedManager feedManager,
            IShapeFactory shapeFactory,
            IArchiveConstraint archiveConstraint, ISiteService siteService) {
            _services = services;
            _cAseService = cAseService;
            _cAsePostService = cAsePostService;
            _feedManager = feedManager;
            _archiveConstraint = archiveConstraint;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
            _siteService = siteService;
        }

        dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ActionResult ListByArchive(string path) {

            var cAsePath = _archiveConstraint.FindPath(path);
            var archive = _archiveConstraint.FindArchiveData(path);

            if (cAsePath == null)
                return HttpNotFound();

            if (archive == null)
                return HttpNotFound();

            CasePart cAsePart = _cAseService.Get(cAsePath);
            if (cAsePart == null)
                return HttpNotFound();


            if (archive.ToDateTime() == DateTime.MinValue) {
                // render the archive data
                return new ShapeResult(this, Shape.Parts_Cases_CaseArchives(Case: cAsePart, Archives: _cAsePostService.GetArchives(cAsePart)));
            }

            var list = Shape.List();
            list.AddRange(_cAsePostService.Get(cAsePart, archive).Select(b => _services.ContentManager.BuildDisplay(b, "Summary")));

            _feedManager.Register(cAsePart, _services.ContentManager.GetItemMetadata(cAsePart).DisplayText);

            var viewModel = Shape.ViewModel()
                .ContentItems(list)
                .Case(cAsePart)
                .ArchiveData(archive);

            return View(viewModel);
        }
      
    }
}