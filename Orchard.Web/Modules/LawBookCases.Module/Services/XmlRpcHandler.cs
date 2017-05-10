using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Core.XmlRpc;
using Orchard.Core.XmlRpc.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Mvc.Html;
using Orchard.Core.Title.Models;
using LawBookCases.Module.Models;
using Orchard;
using LawBookCases.Module.Extensions;

namespace LawBookCases.Module.Services {
    [OrchardFeature("LawBookCases.Module.RemotePublishing")]
    public class XmlRpcHandler : IXmlRpcHandler {
        private readonly ICaseService _cAseService;
        private readonly ICasePostService _cAsePostService;
        private readonly IContentManager _contentManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMembershipService _membershipService;
        private readonly RouteCollection _routeCollection;

        public XmlRpcHandler(ICaseService cAseService, ICasePostService cAsePostService, IContentManager contentManager,
            IAuthorizationService authorizationService, IMembershipService membershipService, 
            RouteCollection routeCollection) {
            _cAseService = cAseService;
            _cAsePostService = cAsePostService;
            _contentManager = contentManager;
            _authorizationService = authorizationService;
            _membershipService = membershipService;
            _routeCollection = routeCollection;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public void SetCapabilities(XElement options) {
            const string manifestUri = "http://schemas.microsoft.com/wlw/manifest/wecAse";
            options.SetElementValue(XName.Get("supportsSlug", manifestUri), "Yes");
        }

        public void Process(XmlRpcContext context) {
            var urlHelper = new UrlHelper(context.ControllerContext.RequestContext, _routeCollection);

            if (context.Request.MethodName == "cAseser.getUsersCases") {
                var result = MetaWecAseGetUserCases(urlHelper,
                    Convert.ToString(context.Request.Params[1].Value),
                    Convert.ToString(context.Request.Params[2].Value));

                context.Response = new XRpcMethodResponse().Add(result);
            }

            if (context.Request.MethodName == "metaWecAse.getRecentPosts") {
                var result = MetaWecAseGetRecentPosts(urlHelper,
                    Convert.ToString(context.Request.Params[0].Value),
                    Convert.ToString(context.Request.Params[1].Value),
                    Convert.ToString(context.Request.Params[2].Value),
                    Convert.ToInt32(context.Request.Params[3].Value),
                    context._drivers);

                context.Response = new XRpcMethodResponse().Add(result);
            }

            if (context.Request.MethodName == "metaWecAse.newPost") {
                var result = MetaWecAseNewPost(
                    Convert.ToString(context.Request.Params[0].Value),
                    Convert.ToString(context.Request.Params[1].Value),
                    Convert.ToString(context.Request.Params[2].Value),
                    (XRpcStruct)context.Request.Params[3].Value,
                    Convert.ToBoolean(context.Request.Params[4].Value),
                    context._drivers);

                context.Response = new XRpcMethodResponse().Add(result);
            }

            if (context.Request.MethodName == "metaWecAse.getPost") {
                var result = MetaWecAseGetPost(
                    urlHelper,
                    Convert.ToInt32(context.Request.Params[0].Value),
                    Convert.ToString(context.Request.Params[1].Value),
                    Convert.ToString(context.Request.Params[2].Value),
                    context._drivers);
                context.Response = new XRpcMethodResponse().Add(result);
            }

            if (context.Request.MethodName == "metaWecAse.editPost") {
                var result = MetaWecAseEditPost(
                    Convert.ToInt32(context.Request.Params[0].Value),
                    Convert.ToString(context.Request.Params[1].Value),
                    Convert.ToString(context.Request.Params[2].Value),
                    (XRpcStruct)context.Request.Params[3].Value,
                    Convert.ToBoolean(context.Request.Params[4].Value),
                    context._drivers);
                context.Response = new XRpcMethodResponse().Add(result);
            }

            if (context.Request.MethodName == "cAseger.deletePost") {
                var result = MetaWecAseDeletePost(
                    Convert.ToString(context.Request.Params[1].Value),
                    Convert.ToString(context.Request.Params[2].Value),
                    Convert.ToString(context.Request.Params[3].Value),
                    context._drivers);
                context.Response = new XRpcMethodResponse().Add(result);
            }
        }

        private XRpcArray MetaWecAseGetUserCases(UrlHelper urlHelper,
            string userName,
            string password) {

            IUser user = ValidateUser(userName, password);

            XRpcArray array = new XRpcArray();
            foreach (CasePart cAse in _cAseService.Get()) {
                // User needs to at least have permission to edit its own cAse posts to access the service
                if (_authorizationService.TryCheckAccess(Permissions.EditCasePost, user, cAse)) {

                    CasePart cAsePart = cAse;
                    array.Add(new XRpcStruct()
                                  .Set("url", urlHelper.AbsoluteAction(() => urlHelper.Case(cAsePart)))
                                  .Set("cAseid", cAse.Id)
                                  .Set("cAseName", _contentManager.GetItemMetadata(cAse).DisplayText));
                }
            }

            return array;
        }

        private XRpcArray MetaWecAseGetRecentPosts(
            UrlHelper urlHelper,
            string cAseId,
            string userName,
            string password,
            int numberOfPosts,
            IEnumerable<IXmlRpcDriver> drivers) {

            IUser user = ValidateUser(userName, password);

            // User needs to at least have permission to edit its own cAse posts to access the service
            _authorizationService.CheckAccess(Permissions.EditCasePost, user, null);

            CasePart cAse = _contentManager.Get<CasePart>(Convert.ToInt32(cAseId));
            if (cAse == null) {
                throw new ArgumentException();
            }

            var array = new XRpcArray();
            foreach (var cAsePost in _cAsePostService.Get(cAse, 0, numberOfPosts, VersionOptions.Latest)) {
                var postStruct = CreateCaseStruct(cAsePost, urlHelper);

                foreach (var driver in drivers)
                    driver.Process(postStruct);

                array.Add(postStruct);
            }
            return array;
        }

        private int MetaWecAseNewPost(
            string cAseId,
            string userName,
            string password,
            XRpcStruct content,
            bool publish,
            IEnumerable<IXmlRpcDriver> drivers) {

            IUser user = ValidateUser(userName, password);

            // User needs permission to edit or publish its own cAse posts
            _authorizationService.CheckAccess(publish ? Permissions.PublishCasePost : Permissions.EditCasePost, user, null);

            CasePart cAse = _contentManager.Get<CasePart>(Convert.ToInt32(cAseId));
            if (cAse == null)
                throw new ArgumentException();

            var title = content.Optional<string>("title");
            var description = content.Optional<string>("description");
            var slug = content.Optional<string>("wp_slug");

            var cAsePost = _contentManager.New<CasePostPart>("CasePost");

            // BodyPart
            if (cAsePost.Is<BodyPart>()) {
                cAsePost.As<BodyPart>().Text = description;
            }

            //CommonPart
            if (cAsePost.Is<ICommonPart>()) {
                cAsePost.As<ICommonPart>().Owner = user;
                cAsePost.As<ICommonPart>().Container = cAse;
            }

            //TitlePart
            if (cAsePost.Is<TitlePart>()) {
                cAsePost.As<TitlePart>().Title = HttpUtility.HtmlDecode(title);
            }
            
            //AutoroutePart
            dynamic dCasePost = cAsePost;
            if (dCasePost.AutoroutePart!=null){
                dCasePost.AutoroutePart.DisplayAlias = slug;
            }

            _contentManager.Create(cAsePost, VersionOptions.Draft);

            // try to get the UTC timezone by default
            var publishedUtc = content.Optional<DateTime?>("date_created_gmt");
            if (publishedUtc == null) {
                // take the local one
                publishedUtc = content.Optional<DateTime?>("dateCreated");
            }
            else {
                // ensure it's read as a UTC time
                publishedUtc = new DateTime(publishedUtc.Value.Ticks, DateTimeKind.Utc);
            }

            if (publish && (publishedUtc == null || publishedUtc <= DateTime.UtcNow))
                _cAsePostService.Publish(cAsePost);

            if (publishedUtc != null) {
                cAsePost.As<CommonPart>().CreatedUtc = publishedUtc;
            }

            foreach (var driver in drivers)
                driver.Process(cAsePost.Id);

            return cAsePost.Id;
        }

        private XRpcStruct MetaWecAseGetPost(
            UrlHelper urlHelper,
            int postId,
            string userName,
            string password,
            IEnumerable<IXmlRpcDriver> drivers) {

            IUser user = ValidateUser(userName, password);
            var cAsePost = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (cAsePost == null)
                throw new ArgumentException();

            _authorizationService.CheckAccess(Permissions.EditCasePost, user, cAsePost);

            var postStruct = CreateCaseStruct(cAsePost, urlHelper);

            foreach (var driver in drivers)
                driver.Process(postStruct);

            return postStruct;
        }

        private bool MetaWecAseEditPost(
            int postId,
            string userName,
            string password,
            XRpcStruct content,
            bool publish,
            IEnumerable<IXmlRpcDriver> drivers) {

            IUser user = ValidateUser(userName, password);
            var cAsePost = _cAsePostService.Get(postId, VersionOptions.DraftRequired);
            if (cAsePost == null) {
                throw new OrchardCoreException(T("The specified Case Post doesn't exist anymore. Please create a new Case Post."));
            }

            _authorizationService.CheckAccess(publish ? Permissions.PublishCasePost : Permissions.EditCasePost, user, cAsePost);

            var title = content.Optional<string>("title");
            var description = content.Optional<string>("description");
            var slug = content.Optional<string>("wp_slug");

            // BodyPart
            if (cAsePost.Is<BodyPart>()) {
                cAsePost.As<BodyPart>().Text = description;
            }

            //TitlePart
            if (cAsePost.Is<TitlePart>()) {
                cAsePost.As<TitlePart>().Title = HttpUtility.HtmlDecode(title);
            }
            //AutoroutePart
            dynamic dCasePost = cAsePost;
            if (dCasePost.AutoroutePart != null) {
                dCasePost.AutoroutePart.DisplayAlias = slug;
            }

            // try to get the UTC timezone by default
            var publishedUtc = content.Optional<DateTime?>("date_created_gmt");
            if (publishedUtc == null) {
                // take the local one
                publishedUtc = content.Optional<DateTime?>("dateCreated");
            }
            else {
                // ensure it's read as a UTC time
                publishedUtc = new DateTime(publishedUtc.Value.Ticks, DateTimeKind.Utc);
            }

            if (publish && (publishedUtc == null || publishedUtc <= DateTime.UtcNow))
                _cAsePostService.Publish(cAsePost);

            if (publishedUtc != null) {
                cAsePost.As<CommonPart>().CreatedUtc = publishedUtc;
            }

            foreach (var driver in drivers)
                driver.Process(cAsePost.Id);

            return true;
        }

        private bool MetaWecAseDeletePost(
            string postId,
            string userName,
            string password,
            IEnumerable<IXmlRpcDriver> drivers) {

            IUser user = ValidateUser(userName, password);
            var cAsePost = _cAsePostService.Get(Convert.ToInt32(postId), VersionOptions.Latest);
            if (cAsePost == null)
                throw new ArgumentException();

            _authorizationService.CheckAccess(Permissions.DeleteCasePost, user, cAsePost);

            foreach (var driver in drivers)
                driver.Process(cAsePost.Id);

            _cAsePostService.Delete(cAsePost);
            return true;
        }

        private IUser ValidateUser(string userName, string password) {
            IUser user = _membershipService.ValidateUser(userName, password);
            if (user == null) {
                throw new OrchardCoreException(T("The username or e-mail or password provided is incorrect."));
            }

            return user;
        }

        private static XRpcStruct CreateCaseStruct(
            CasePostPart cAsePostPart,
            UrlHelper urlHelper) {

            var url = urlHelper.AbsoluteAction(() => urlHelper.ItemDisplayUrl(cAsePostPart));

            if (cAsePostPart.HasDraft()) {
                url = urlHelper.AbsoluteAction("Preview", "Item", new { area = "Contents", id = cAsePostPart.ContentItem.Id });
            }

            var cAseStruct = new XRpcStruct()
                .Set("postid", cAsePostPart.Id)
                .Set("title", HttpUtility.HtmlEncode(cAsePostPart.Title))
                
                .Set("description", cAsePostPart.Text)
                .Set("link", url)
                .Set("permaLink", url);
            
            cAseStruct.Set("wp_slug", cAsePostPart.As<IAliasAspect>().Path);
            

            if (cAsePostPart.PublishedUtc != null) {
                cAseStruct.Set("dateCreated", cAsePostPart.PublishedUtc);
                cAseStruct.Set("date_created_gmt", cAsePostPart.PublishedUtc);
            }

            return cAseStruct;
        }
    }
}
