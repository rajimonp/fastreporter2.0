using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using LawBookCases.Module.Services;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Handlers {
    public class CasePostPartHandler : ContentHandler {
        private readonly ICaseService _CaseService;
      

        public CasePostPartHandler(ICaseService CaseService, ICasePostService CasePostService, RequestContext requestContext) {
            _CaseService = CaseService;

            OnGetDisplayShape<CasePostPart>(SetModelProperties);
            OnGetEditorShape<CasePostPart>(SetModelProperties);
            OnUpdateEditorShape<CasePostPart>(SetModelProperties);

            OnCreated<CasePostPart>((context, part) => ProcessCasePostsCount(part));
            OnPublished<CasePostPart>((context, part) => ProcessCasePostsCount(part));
            OnUnpublished<CasePostPart>((context, part) => ProcessCasePostsCount(part));
            OnVersioned<CasePostPart>((context, part, newVersionPart) => ProcessCasePostsCount(newVersionPart));
            OnRemoved<CasePostPart>((context, part) => ProcessCasePostsCount(part));

            OnRemoved<CasePart>(
                (context, b) =>
                CasePostService.Get(context.ContentItem.As<CasePart>()).ToList().ForEach(
                    CasePost => context.ContentManager.Remove(CasePost.ContentItem)));

            OnInitializing<CasePostPart>((context, casePart) => {
                
            });
        }
        protected override void Published(PublishContentContext context)
        {
            base.Published(context);
            
        }

        private void ProcessCasePostsCount(CasePostPart CasePostPart) {
            CommonPart commonPart = CasePostPart.As<CommonPart>();
            if (commonPart != null &&
                commonPart.Record.Container != null) {

                _CaseService.ProcessCasePostsCount(commonPart.Container.Id);
            }
        }

        private static void SetModelProperties(BuildShapeContext context, CasePostPart CasePost) {
            context.Shape.Case = CasePost.CasePart;
                   var casat = CasePost.Get<CasePostAttribPart>();
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var CasePost = context.ContentItem.As<CasePostPart>();

            // CasePart can be null if this is a new Case Post item.
            if (CasePost == null || CasePost.CasePart == null) {
                return;
            }

            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CasePostAdmin"},
                {"Action", "Create"},
                {"CaseId", CasePost.CasePart.Id}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CasePostAdmin"},
                {"Action", "Edit"},
                {"postId", context.ContentItem.Id},
                {"CaseId", CasePost.CasePart.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CasePostAdmin"},
                {"Action", "Delete"},
                {"postId", context.ContentItem.Id},
                {"CaseId", CasePost.CasePart.Id}
            };
        }
    }
}