using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;
using LawBookCases.Module.Models;
using LawBookCases.Module.Services;
using LawBookCases.Module.ViewModels;

namespace LawBookCases.Module.Drivers {
    public class RecentCasePostsPartDriver : ContentPartDriver<RecentCasePostsPart> {
        private readonly ICaseService _cAseService;
        private readonly IContentManager _contentManager;

        public RecentCasePostsPartDriver(
            ICaseService cAseService, 
            IContentManager contentManager) {
            _cAseService = cAseService;
            _contentManager = contentManager;
        }

        protected override DriverResult Display(RecentCasePostsPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Cases_RecentCasePosts", () => {
            var cAse = _contentManager.Get<CasePart>(part.CaseNumber);

                if (cAse == null) {
                    return null;
                }

                var cAsePosts = _contentManager.Query(VersionOptions.Published, "CasePost")
                    .Join<CommonPartRecord>().Where(cr => cr.Container.Id == cAse.Id)
                    .OrderByDescending(cr => cr.CreatedUtc)
                    .Slice(0, part.Count)
                    .Select(ci => ci.As<CasePostPart>());

                var list = shapeHelper.List();
                list.AddRange(cAsePosts.Select(bp => _contentManager.BuildDisplay(bp, "Summary")));

                var cAsePostList = shapeHelper.Parts_Cases_CasePost_List(ContentItems: list);

                return shapeHelper.Parts_Cases_RecentCasePosts(ContentItems: cAsePostList, Case: cAse);
            });
        }

        protected override DriverResult Editor(RecentCasePostsPart part, dynamic shapeHelper) {
            var viewModel = new RecentCasePostsViewModel {
                Count = part.Count,
                CaseNumber = part.CaseNumber,
                Cases = _cAseService.Get().ToList().OrderBy(b => _contentManager.GetItemMetadata(b).DisplayText)
            };

            return ContentShape("Parts_Cases_RecentCasePosts_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Cases.RecentCasePosts", Model: viewModel, Prefix: Prefix));
        }

        protected override DriverResult Editor(RecentCasePostsPart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new RecentCasePostsViewModel();

            if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                part.CaseNumber = viewModel.CaseNumber;
                part.Count = viewModel.Count;
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(RecentCasePostsPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Case", cAse =>
                part.CaseNumber = context.GetItemFromSession(cAse).Id
            );

            context.ImportAttribute(part.PartDefinition.Name, "Count", count =>
               part.Count = Convert.ToInt32(count)
            );
        }

        protected override void Exporting(RecentCasePostsPart part, ExportContentContext context) {
            var cAse = _contentManager.Get(part.CaseNumber);
            var cAseIdentity = _contentManager.GetItemMetadata(cAse).Identity;

            context.Element(part.PartDefinition.Name).SetAttributeValue("Case", cAseIdentity);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Count", part.Count);
        }
    }
}