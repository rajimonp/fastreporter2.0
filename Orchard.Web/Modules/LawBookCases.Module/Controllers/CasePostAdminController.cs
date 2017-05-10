using System;
using System.Reflection;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Settings;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Mvc.AntiForgery;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using LawBookCases.Module.Services;
using Orchard;
using LawBookCases.Module.Models;
using LawBookCases.Module.Extensions;
using Orchard.Security;
using System.Collections.Generic;
using Orchard.Users.Models;
using Orchard.ContentManagement.Records;

namespace LawBookCases.Module.Controllers {

    /// <summary>
    /// TODO: (PH:Autoroute) This replicates a whole lot of Core.Contents functionality. All we actually need to do is take the CaseId from the query string in the CasePostPartDriver, and remove
    /// helper extensions from UrlHelperExtensions.
    /// </summary>
    [ValidateInput(false), Admin]
    public class CasePostAdminController : Controller, IUpdateModel {
        private readonly ICaseService _cAseService;
        private readonly ICasePostService _cAsePostService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CasePostAdminController(IOrchardServices services, ICaseService cAseService, ICasePostService cAsePostService,  IWorkContextAccessor workContextAccessor) {
            Services = services;
            _cAseService = cAseService;
            _cAsePostService = cAsePostService;
            T = NullLocalizer.Instance;
            _workContextAccessor = workContextAccessor;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Create(int cAseId) {

            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest).As<CasePart>();
            if (cAse == null)
                return HttpNotFound();

            var cAsePost = Services.ContentManager.New<CasePostPart>("CasePost");
            cAsePost.CasePart = cAse;

            if (!Services.Authorizer.Authorize(Permissions.EditCasePost, cAsePost, T("Not allowed to create cAse post")))
                return new HttpUnauthorizedResult();

            var model = Services.ContentManager.BuildEditor(cAsePost);
            
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public ActionResult CreatePOST(int cAseId) {
            return CreatePOST(cAseId, false);
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public ActionResult CreateAndPublishPOST(int cAseId) {
            if (!Services.Authorizer.Authorize(Permissions.PublishOwnCasePost, T("Couldn't create content")))
                return new HttpUnauthorizedResult();

            return CreatePOST(cAseId, true);
        }

        private ActionResult CreatePOST(int cAseId, bool publish = false) {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest).As<CasePart>();

            if (cAse == null)
                return HttpNotFound();

            var cAsePost = Services.ContentManager.New<CasePostPart>("CasePost");
            cAsePost.CasePart = cAse;
            
            if (!Services.Authorizer.Authorize(Permissions.EditCasePost, cAsePost, T("Couldn't create cAse post")))
                return new HttpUnauthorizedResult();
            
            Services.ContentManager.Create(cAsePost, VersionOptions.Draft);
            var model = Services.ContentManager.UpdateEditor(cAsePost, this);

            CasePostAttribPart attri= cAsePost.Get<CasePostAttribPart>();
           int id= attri.Id;
        
            attri.CasePostNumber = cAsePost.ContentItem.Id;
            attri.CaseNumber = _cAsePostService.GetNextCaseNumber(attri.CaseYear);

            if (cAsePost.Text.Contains("PFR")) {
                string[] caseHeader = CommonExtentions.ExtractCaseHeader(cAsePost.Text).ToArray();
                if (caseHeader.Length >= 4)
                {
                    string[] clients = CommonExtentions.GetClients(caseHeader[0].ToString());
                    if (clients.Length >= 2)
                    {
                        attri.CaseClient1 = clients[0].ToString();
                        attri.CaseClient2 = clients[1].ToString();
                    }
                    attri.CaseHeldCourt = caseHeader[1].ToString();
                    attri.CaseDecisionTakenBy = caseHeader[2].ToString();
                    attri.CaseReference = caseHeader[3].ToString();

                }
            }
            

            model = Services.ContentManager.UpdateEditor(cAsePost, this);
 
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return View(model);
            }
      
            if (publish) {
                if (!Services.Authorizer.Authorize(Permissions.PublishCasePost, cAsePost.ContentItem, T("Couldn't publish cAse post")))
                    return new HttpUnauthorizedResult();

                Services.ContentManager.Publish(cAsePost.ContentItem);
            }
     

            Services.Notifier.Information(T("Your {0} has been created.", cAsePost.TypeDefinition.DisplayName));
            return Redirect(Url.CasePostEdit(cAsePost));
        }

        //todo: the content shape template has extra bits that the core contents module does not (remove draft functionality)
        //todo: - move this extra functionality there or somewhere else that's appropriate?
        public ActionResult Edit(int cAseId, int postId) {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();
            CasePostAttribPart attri = post.Get<CasePostAttribPart>();
            Boolean isAcquired = attri.CaseAcquiredBy == 0 ? false : true;

            bool isMe = false;
            //Check isit accuired by me
   
            int userId = _workContextAccessor.GetContext().CurrentUser.Id;

            isMe = userId == attri.CaseAcquiredBy;

            bool icanEdit = isAcquired && isMe;

            if (!icanEdit)
            if (!Services.Authorizer.Authorize(Permissions.EditCasePost, post, T("Couldn't edit cAse post")))
                return new HttpUnauthorizedResult();

            var model = Services.ContentManager.BuildEditor(post);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult EditPOST(int cAseId, int postId, string returnUrl) {
            return EditPOST(cAseId, postId, returnUrl, contentItem => {
                if (!contentItem.Has<IPublishingControlAspect>() && !contentItem.TypeDefinition.Settings.GetModel<ContentTypeSettings>().Draftable)
                    Services.ContentManager.Publish(contentItem);
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult EditAndPublishPOST(int cAseId, int postId, string returnUrl) {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            // Get draft (create a new version if needed)
            var cAsePost = _cAsePostService.Get(postId, VersionOptions.DraftRequired);
            if (cAsePost == null)
                return HttpNotFound();

            var casat = cAsePost.Get<CasePostAttribPart>();

            if (!Services.Authorizer.Authorize(Permissions.PublishCasePost, cAsePost, T("Couldn't publish cAse post")))
                return new HttpUnauthorizedResult();

            return EditPOST(cAseId, postId, returnUrl, contentItem => Services.ContentManager.Publish(contentItem));
        }

        public ActionResult EditPOST(int cAseId, int postId, string returnUrl, Action<ContentItem> conditionallyPublish) {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            // Get draft (create a new version if needed)
            var cAsePost = _cAsePostService.Get(postId, VersionOptions.DraftRequired);
            if (cAsePost == null)
                return HttpNotFound();
            var casat = cAsePost.Get<CasePostAttribPart>();
            CasePostAttribPart attri = casat.Get<CasePostAttribPart>();
            Boolean isAcquired = attri.CaseAcquiredBy == 0 ? false : true;

            bool isMe = false;
            //Check isit accuired by me

            int userId = _workContextAccessor.GetContext().CurrentUser.Id;

            isMe = userId == attri.CaseAcquiredBy;

            bool icanEdit = isAcquired && isMe;

            if (!icanEdit)
                if (!Services.Authorizer.Authorize(Permissions.EditCasePost, cAsePost, T("Couldn't edit cAse post")))
                return new HttpUnauthorizedResult();

            if (cAsePost.Text.Contains("PFR"))
            {
                string[] caseHeader = CommonExtentions.ExtractCaseHeader(cAsePost.Text).ToArray();
                if (caseHeader.Length > 3)
                {
                    string[] clients = CommonExtentions.GetClients(caseHeader[0].ToString());
                    if (clients.Length > 2)
                    {
                        attri.CaseClient1 = clients[0].ToString();
                        attri.CaseClient2 = clients[1].ToString();
                    }
                    attri.CaseHeldCourt = caseHeader[1].ToString();
                    attri.CaseDecisionTakenBy = caseHeader[2].ToString();
                    attri.CaseReference = caseHeader[3].ToString();

                }
            }

            // Validate form input
            var model = Services.ContentManager.UpdateEditor(cAsePost, this);
            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return View(model);
            }

            conditionallyPublish(cAsePost.ContentItem);

            Services.Notifier.Information(T("Your {0} has been saved.", cAsePost.TypeDefinition.DisplayName));

            return this.RedirectLocal(returnUrl, Url.CasePostEdit(cAsePost));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult DiscardDraft(int id) {
            // get the current draft version
            var draft = Services.ContentManager.Get(id, VersionOptions.Draft);
            if (draft == null) {
                Services.Notifier.Information(T("There is no draft to discard."));
                return RedirectToEdit(id);
            }

            // check edit permission
            if (!Services.Authorizer.Authorize(Permissions.EditCasePost, draft, T("Couldn't discard cAse post draft")))
                return new HttpUnauthorizedResult();

            // locate the published revision to revert onto
            var published = Services.ContentManager.Get(id, VersionOptions.Published);
            if (published == null) {
                Services.Notifier.Information(T("Can not discard draft on unpublished cAse post."));
                return RedirectToEdit(draft);
            }

            // marking the previously published version as the latest
            // has the effect of discarding the draft but keeping the history
            draft.VersionRecord.Latest = false;
            published.VersionRecord.Latest = true;

            Services.Notifier.Information(T("Case post draft version discarded"));
            return RedirectToEdit(published);
        }

        ActionResult RedirectToEdit(int id) {
            return RedirectToEdit(Services.ContentManager.GetLatest<CasePostPart>(id));
        }

        ActionResult RedirectToEdit(IContent item) {
            if (item == null || item.As<CasePostPart>() == null)
                return HttpNotFound();
            return RedirectToAction("Edit", new { CaseId = item.As<CasePostPart>().CasePart.Id, PostId = item.ContentItem.Id });
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Delete(int cAseId, int postId) {
            //refactoring: test PublishCasePost/PublishCasePost in addition if published

            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.DeleteCasePost, post, T("Couldn't delete cAse post")))
                return new HttpUnauthorizedResult();

            _cAsePostService.Delete(post);
            Services.Notifier.Information(T("Case post was successfully deleted"));

            return Redirect(Url.CaseForAdmin(cAse.As<CasePart>()));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Publish(int cAseId, int postId) {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishCasePost, post, T("Couldn't publish cAse post")))
                return new HttpUnauthorizedResult();

            _cAsePostService.Publish(post);
            Services.Notifier.Information(T("Case post successfully published."));

            return Redirect(Url.CaseForAdmin(cAse.As<CasePart>()));
        }

        [ValidateAntiForgeryTokenOrchard]
        public ActionResult Unpublish(int cAseId, int postId) {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.PublishCasePost, post, T("Couldn't unpublish cAse post")))
                return new HttpUnauthorizedResult();

            _cAsePostService.Unpublish(post);
            Services.Notifier.Information(T("Case post successfully unpublished."));

            return Redirect(Url.CaseForAdmin(cAse.As<CasePart>()));
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

       
        public ActionResult Acquier(int cAseId, int postId, string returnUrl)
        {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            if (!Services.Authorizer.Authorize(Permissions.AcquireUnpublisehdCase, post, T("Couldn't Acquier Case post")))
                return new HttpUnauthorizedResult();
          

            IUser user = _workContextAccessor.GetContext().CurrentUser;
            dynamic dynUser = user.ContentItem;
           

            int unameId = user.Get<UserPart>().Id;
            string role = string.Empty; ;
            if (dynUser.UserRolesPart != null)
            {
                IEnumerable<string> usernames = dynUser.UserRolesPart.Roles;
                 foreach(var unameloop in usernames)
                {
                    role = unameloop;
                        break;
                }
            }
            CasePostAttribRecord attri = post.Get<CasePostAttribPart>().Record;
            CasePostAttribPart attripart = post.Get<CasePostAttribPart>();

            attri.CaseAcquiredBy =unameId;
            attri.CaseAcquiredRole = role;
            attripart.CaseAcquiredBy = unameId;
            attripart.CaseAcquiredRole = role;


            CasePostStateRecord rec = new CasePostStateRecord();
            rec.CasePostPart_id = attri.CasePostNumber;
            rec.CasePostState = role;
            rec.CasePostStateUserId = unameId;
            rec.CasePostStateUtc = DateTime.UtcNow;
            rec.ContentItemRecord = (ContentItemRecord)post.Get<CasePostPart>().ContentItem.Record;

            if (role.Equals("Law Interns"))
                rec.CaseAcquiredStatus = (int)CasePostStateRecord.CaseAcquiredStateEnum.INTERNAcquired;
            if (role.Equals("Editor"))
                rec.CaseAcquiredStatus = (int)CasePostStateRecord.CaseAcquiredStateEnum.EDITORAcquired;
            if (role.Equals("Author"))
                rec.CaseAcquiredStatus = (int)CasePostStateRecord.CaseAcquiredStateEnum.AUTHORAcquired;
            
            _cAsePostService.Acquier(rec);
            // Validate form input
            var model = Services.ContentManager.UpdateEditor(post, this);
            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                Services.Notifier.Information(T("Case post Failed  acquired."));
            }
            else Services.Notifier.Information(T("Case post successfully acquired."));
            

            return this.RedirectLocal(returnUrl, () => RedirectToAction("List"));
            
        }
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.UpdateAbstratc")]
        public ActionResult EditAbstract(int cAseId, int postId)
        {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            IUser user = _workContextAccessor.GetContext().CurrentUser;
            dynamic dynUser = user.ContentItem;


            int unameId = user.Get<UserPart>().Id;
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

            CasePostAttribRecord attri = post.Get<CasePostAttribPart>().Record;
            CasePostAttribPart attripart = post.Get<CasePostAttribPart>();

            attri.CaseAcquiredBy = 0;
            attri.CaseAcquiredRole = string.Empty;
            attripart.CaseAcquiredBy = 0;
            attripart.CaseAcquiredRole = string.Empty;

            bool isMe = false;
            //Check isit accuired by me

            int userId = _workContextAccessor.GetContext().CurrentUser.Id;

            isMe = userId == attri.CaseAcquiredBy;

    //        bool icanEdit = isAcquired && isMe;

     //       if (!icanEdit)
              if (!Services.Authorizer.Authorize(Permissions.EditCasePost, post, T("Couldn't edit cAse post")))
                    return new HttpUnauthorizedResult();

            CasePostStateRecord rec = new CasePostStateRecord();
            rec.CasePostPart_id = attri.CasePostNumber;
            rec.CasePostState = role;
            rec.CasePostStateUserId = unameId;
            rec.CasePostStateUtc = DateTime.UtcNow;
            rec.ContentItemRecord = (ContentItemRecord)post.Get<CasePostPart>().ContentItem.Record;
            rec.CaseAcquiredStatus = (int)CasePostStateRecord.CaseAcquiredStateEnum.INTERNReleased;

            _cAsePostService.Acquier(rec);

            return Edit(cAseId, postId);
            
        }
     
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.UpdateAuthor")]
        public ActionResult EditAuthor(int cAseId, int postId)
        {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            IUser user = _workContextAccessor.GetContext().CurrentUser;
            dynamic dynUser = user.ContentItem;


            int unameId = user.Get<UserPart>().Id;
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

            CasePostAttribRecord attri = post.Get<CasePostAttribPart>().Record;
            CasePostAttribPart attripart = post.Get<CasePostAttribPart>();

            attri.CaseAcquiredBy = 0;
            attri.CaseAcquiredRole = string.Empty;
            attripart.CaseAcquiredBy = 0;
            attripart.CaseAcquiredRole = string.Empty;

            bool isMe = false;
            //Check isit accuired by me

            int userId = _workContextAccessor.GetContext().CurrentUser.Id;

            isMe = userId == attri.CaseAcquiredBy;

            //        bool icanEdit = isAcquired && isMe;

            //       if (!icanEdit)
            if (!Services.Authorizer.Authorize(Permissions.EditCasePost, post, T("Couldn't edit cAse post")))
                return new HttpUnauthorizedResult();

            CasePostStateRecord rec = new CasePostStateRecord();
            rec.CasePostPart_id = attri.CasePostNumber;
            rec.CasePostState = role;
            rec.CasePostStateUserId = unameId;
            rec.CasePostStateUtc = DateTime.UtcNow;
            rec.ContentItemRecord = (ContentItemRecord)post.Get<CasePostPart>().ContentItem.Record;
            rec.CaseAcquiredStatus = (int)CasePostStateRecord.CaseAcquiredStateEnum.AUTHORReleased;

            _cAsePostService.Acquier(rec);

            return Edit(cAseId, postId);

        }
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.UpdateEditor")]
        public ActionResult EditEditor(int cAseId, int postId)
        {
            var cAse = _cAseService.Get(cAseId, VersionOptions.Latest);
            if (cAse == null)
                return HttpNotFound();

            var post = _cAsePostService.Get(postId, VersionOptions.Latest);
            if (post == null)
                return HttpNotFound();

            IUser user = _workContextAccessor.GetContext().CurrentUser;
            dynamic dynUser = user.ContentItem;


            int unameId = user.Get<UserPart>().Id;
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

            CasePostAttribRecord attri = post.Get<CasePostAttribPart>().Record;
            CasePostAttribPart attripart = post.Get<CasePostAttribPart>();

            attri.CaseAcquiredBy = 0;
            attri.CaseAcquiredRole = string.Empty;
            attripart.CaseAcquiredBy = 0;
            attripart.CaseAcquiredRole = string.Empty;

            bool isMe = false;
            //Check isit accuired by me

            int userId = _workContextAccessor.GetContext().CurrentUser.Id;

            isMe = userId == attri.CaseAcquiredBy;

            if (!Services.Authorizer.Authorize(Permissions.EditCasePost, post, T("Couldn't edit cAse post")))
                return new HttpUnauthorizedResult();

            CasePostStateRecord rec = new CasePostStateRecord();
            rec.CasePostPart_id = attri.CasePostNumber;
            rec.CasePostState = role;
            rec.CasePostStateUserId = unameId;
            rec.CasePostStateUtc = DateTime.UtcNow;
            rec.ContentItemRecord = (ContentItemRecord)post.Get<CasePostPart>().ContentItem.Record;
            rec.CaseAcquiredStatus = (int)CasePostStateRecord.CaseAcquiredStateEnum.EDITORReleased;

            _cAsePostService.Acquier(rec);

            return Edit(cAseId, postId);

        }
    }
}