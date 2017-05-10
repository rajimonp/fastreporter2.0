using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using LawBookCases.Module.Services;
using LawBookCases.Module.Routing;
using Orchard;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Controllers {
    [OrchardFeature("LawBookCases.Module.RemotePublishing")]
    public class RemoteCasePublishingController : Controller {
        private readonly ICaseService _cAseService;
        private readonly IRsdConstraint _rsdConstraint;
        private readonly RouteCollection _routeCollection;

        public RemoteCasePublishingController(
            IOrchardServices services, 
            ICaseService cAseService, 
            IRsdConstraint rsdConstraint,
            RouteCollection routeCollection) {
            _cAseService = cAseService;
            _rsdConstraint = rsdConstraint;
            _routeCollection = routeCollection;
            Logger = NullLogger.Instance;
        }

        protected ILogger Logger { get; set; }

        public ActionResult Rsd(string path) {
            Logger.Debug("RSD requested");

            var cAsePath = _rsdConstraint.FindPath(path);
            
            if (cAsePath == null)
                return HttpNotFound();

            CasePart cAsePart = _cAseService.Get(cAsePath);

            if (cAsePart == null)
                return HttpNotFound();

            const string manifestUri = "http://archipelago.phrasewise.com/rsd";

            var urlHelper = new UrlHelper(ControllerContext.RequestContext, _routeCollection);
            var url = urlHelper.AbsoluteAction("", "", new { Area = "XmlRpc" });

            var options = new XElement(
                XName.Get("service", manifestUri),
                new XElement(XName.Get("engineName", manifestUri), "Orchard CMS"),
                new XElement(XName.Get("engineLink", manifestUri), "http://orchardproject.net"),
                new XElement(XName.Get("homePageLink", manifestUri), "http://orchardproject.net"),
                new XElement(XName.Get("apis", manifestUri),
                    new XElement(XName.Get("api", manifestUri),
                        new XAttribute("name", "MetaWecAse"),
                        new XAttribute("preferred", true),
                        new XAttribute("apiLink", url),
                        new XAttribute("cAseID", cAsePart.Id))));

            var doc = new XDocument(new XElement(
                                        XName.Get("rsd", manifestUri),
                                        new XAttribute("version", "1.0"),
                                        options));

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return Content(doc.ToString(), "text/xml");
        }
    }
}