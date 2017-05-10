using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Mvc.Extensions;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Extensions {
    /// <summary>
    /// TODO: (PH:Autoroute) Many of these are or could be redundant (see controllers)
    /// </summary>
    public static class UrlHelperExtensions {
        public static string Cases(this UrlHelper urlHelper) {
            return urlHelper.Action("List", "Case", new {area = "LawBookCases.Module"});
        }

        public static string CasesForAdmin(this UrlHelper urlHelper) {
            return urlHelper.Action("List", "CaseAdmin", new {area = "LawBookCases.Module"});
        }

        public static string Case(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.Action("Item", "Case", new { cAseId = cAsePart.Id, area = "LawBookCases.Module" });
        }

        public static string CaseLiveWriterManifest(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.AbsoluteAction(() => urlHelper.Action("Manifest", "LiveWriter", new { area = "XmlRpc" }));
        }

        public static string CaseRsd(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.AbsoluteAction(() => urlHelper.Action("Rsd", "RemoteCasePublishing", new { path = cAsePart.As<IAliasAspect>().Path + "/rsd", area = "LawBookCases.Module" }));
        }

        public static string CaseArchiveYear(this UrlHelper urlHelper, CasePart cAsePart, int year) {
            var cAsePath = cAsePart.As<IAliasAspect>().Path;
            return urlHelper.Action("ListByArchive", "CasePost", new { path = (string.IsNullOrWhiteSpace(cAsePath) ? "archive/" : cAsePath + "/archive/") + year.ToString(), area = "LawBookCases.Module" });
        }

        public static string CaseArchiveMonth(this UrlHelper urlHelper, CasePart cAsePart, int year, int month) {
            var cAsePath = cAsePart.As<IAliasAspect>().Path;
            return urlHelper.Action("ListByArchive", "CasePost", new { path = (string.IsNullOrWhiteSpace(cAsePath) ? "archive/" : cAsePath + "/archive/") + string.Format("{0}/{1}", year, month), area = "LawBookCases.Module" });
        }

        public static string CaseArchiveDay(this UrlHelper urlHelper, CasePart cAsePart, int year, int month, int day) {
            var cAsePath = cAsePart.As<IAliasAspect>().Path;
            return urlHelper.Action("ListByArchive", "CasePost", new { path = (string.IsNullOrWhiteSpace(cAsePath) ? "archive/" : cAsePath + "/archive/") + string.Format("{0}/{1}/{2}", year, month, day), area = "LawBookCases.Module" });
        }

        public static string CaseForAdmin(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.Action("Item", "CaseAdmin", new { cAseId = cAsePart.Id, area = "LawBookCases.Module" });
        }

        public static string CaseCreate(this UrlHelper urlHelper) {
            return urlHelper.Action("Create", "CaseAdmin", new { area = "LawBookCases.Module" });
        }

        public static string CaseEdit(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.Action("Edit", "CaseAdmin", new { cAseId = cAsePart.Id, area = "LawBookCases.Module" });
        }

        public static string CaseRemove(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.Action("Remove", "CaseAdmin", new { cAseId = cAsePart.Id, area = "LawBookCases.Module" });
        }

        public static string CasePostCreate(this UrlHelper urlHelper, CasePart cAsePart) {
            return urlHelper.Action("Create", "CasePostAdmin", new { cAseId = cAsePart.Id, area = "LawBookCases.Module" });
        }
        
        public static string CasePostEdit(this UrlHelper urlHelper, CasePostPart cAsePostPart) {
            return urlHelper.Action("Edit", "CasePostAdmin", new { cAseId = cAsePostPart.CasePart.Id, postId = cAsePostPart.Id, area = "LawBookCases.Module" });
        }

        public static string CasePostDelete(this UrlHelper urlHelper, CasePostPart cAsePostPart) {
            return urlHelper.Action("Delete", "CasePostAdmin", new { cAseId = cAsePostPart.CasePart.Id, postId = cAsePostPart.Id, area = "LawBookCases.Module" });
        }

        public static string CasePostPublish(this UrlHelper urlHelper, CasePostPart cAsePostPart) {
            return urlHelper.Action("Publish", "CasePostAdmin", new { cAseId = cAsePostPart.CasePart.Id, postId = cAsePostPart.Id, area = "LawBookCases.Module" });
        }

        public static string CasePostUnpublish(this UrlHelper urlHelper, CasePostPart cAsePostPart) {
            return urlHelper.Action("Unpublish", "CasePostAdmin", new { cAseId = cAsePostPart.CasePart.Id, postId = cAsePostPart.Id, area = "LawBookCases.Module" });
        }
        public static string CasesYears(this UrlHelper urlHelper)
        {
            return urlHelper.Action("GetYears", "Case", new { area = "LawBookCases.Module" });
        }
        public static string CasesAutoroute(this UrlHelper urlHelper, CasePart cAsePart,int year,int casenum)
        {
            Orchard.Autoroute.Models.AutoroutePart asa = cAsePart.Get<Orchard.Autoroute.Models.AutoroutePart>();
            string  autoRoutroot = asa.Path;
               if( urlHelper.RequestContext.HttpContext.Request.RawUrl.ToString().Length > 4)
                                return urlHelper.RequestContext.HttpContext.Request.RawUrl+autoRoutroot + "/" + year + "/" + casenum;
            return  autoRoutroot + "/" + year + "/" + casenum;

        }
        public static string CasesYearAction(this UrlHelper urlHelper, CasePart cAsePart, int year)
        {
            Orchard.Autoroute.Models.AutoroutePart asa = cAsePart.Get<Orchard.Autoroute.Models.AutoroutePart>();
            string autoRoutroot = asa.Path;

            return autoRoutroot + "/" + year ;

        }
        public static string GetRoortAction(this UrlHelper urlHelper, CasePart cAsePart)
        {
            Orchard.Autoroute.Models.AutoroutePart asa = cAsePart.Get<Orchard.Autoroute.Models.AutoroutePart>();
            string autoRoutroot = asa.Path;

            return autoRoutroot;

        }

        public static string CasePostAcquier(this UrlHelper urlHelper, ContentItem cAsePostPart1,string returnurl)
        {
            CasePostPart cAsePostPart = cAsePostPart1.Get<CasePostPart>();
             
            return urlHelper.Action("Acquier", "CasePostAdmin", new { cAseId = cAsePostPart.CasePart.Id, postId = cAsePostPart.Id, returnUrl= returnurl, area = "LawBookCases.Module" });
           // return urlHelper.Action("Publish", "CasePostAdmin", new { cAseId = cAsePostPart.CasePart.Id, postId = cAsePostPart.Id, area = "LawBookCases.Module" });

        }
    }
}