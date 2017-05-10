using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Handlers {
    public class CasePartHandler : ContentHandler {

        public CasePartHandler() {
            OnGetDisplayShape<CasePart>((context, blog) => {
                context.Shape.Description = blog.Description;
                context.Shape.PostCount = blog.PostCount;
            });
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var cAse = context.ContentItem.As<CasePart>();

            if (cAse == null)
                return;

            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "Case"},
                {"Action", "Item"},
                {"caseId", context.ContentItem.Id}
            };
            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CaseAdmin"},
                {"Action", "Create"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CaseAdmin"},
                {"Action", "Edit"},
                {"caseId", context.ContentItem.Id}
            };
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CaseAdmin"},
                {"Action", "Remove"},
                {"caseId", context.ContentItem.Id}
            };
            context.Metadata.AdminRouteValues = new RouteValueDictionary {
                {"Area", "LawBookCases.Module"},
                {"Controller", "CaseAdmin"},
                {"Action", "Item"},
                {"caseId", context.ContentItem.Id}
            };
        }
    }
}