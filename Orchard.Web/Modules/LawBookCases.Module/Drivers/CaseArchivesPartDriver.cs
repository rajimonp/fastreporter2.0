using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using LawBookCases.Module.Services;
using LawBookCases.Module.Models;
using LawBookCases.Module.ViewModels;

namespace LawBookCases.Module.Drivers {
    public class CaseArchivesPartDriver : ContentPartDriver<CaseArchivesPart> {
        private readonly ICaseService _cAseService;
        private readonly ICasePostService _cAsePostService;
        private readonly IContentManager _contentManager;

        public CaseArchivesPartDriver(
            ICaseService cAseService, 
            ICasePostService cAsePostService,
            IContentManager contentManager) {
            _cAseService = cAseService;
            _cAsePostService = cAsePostService;
            _contentManager = contentManager;
        }

        protected override DriverResult Display(CaseArchivesPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Cases_CaseArchives",
                                () => {
                                    var cAse = _cAseService.Get(part.CaseNumber, VersionOptions.Published).As<CasePart>();

                                    if (cAse == null)
                                        return null;

                                    return shapeHelper.Parts_Cases_CaseArchives(Case: cAse, Archives: _cAsePostService.GetArchives(cAse));
                                });
        }

        protected override DriverResult Editor(CaseArchivesPart part, dynamic shapeHelper) {
            var viewModel = new CaseArchivesViewModel {
                CaseNumber = part.CaseNumber,
                Cases = _cAseService.Get().ToList().OrderBy(b => _contentManager.GetItemMetadata(b).DisplayText)
                };

            return ContentShape("Parts_Cases_CaseArchives_Edit",
                                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Cases.CaseArchives", Model: viewModel, Prefix: Prefix));
        }

        protected override DriverResult Editor(CaseArchivesPart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new CaseArchivesViewModel();
            if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                part.CaseNumber = viewModel.CaseNumber;
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(CaseArchivesPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Case", cAse =>
                part.CaseNumber = context.GetItemFromSession(cAse).Id
            );
        }

        protected override void Exporting(CaseArchivesPart part, ExportContentContext context) {
            var cAse = _contentManager.Get(part.CaseNumber);
            var cAseIdentity = _contentManager.GetItemMetadata(cAse).Identity;
            context.Element(part.PartDefinition.Name).SetAttributeValue("Case", cAseIdentity);
        }

    }
}