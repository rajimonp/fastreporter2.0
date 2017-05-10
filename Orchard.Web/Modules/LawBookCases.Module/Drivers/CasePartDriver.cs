using System;
using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Drivers {
    public class CasePartDriver : ContentPartDriver<CasePart> {
        protected override string Prefix {
            get { return "CasePart"; }
        }

        protected override DriverResult Display(CasePart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Cases_Case_Manage",
                    () => shapeHelper.Parts_Cases_Case_Manage()),
                ContentShape("Parts_Cases_Case_Description",
                    () => shapeHelper.Parts_Cases_Case_Description(Description: part.Description)),
                ContentShape("Parts_Cases_Case_SummaryAdmin",
                    () => shapeHelper.Parts_Cases_Case_SummaryAdmin()),
                ContentShape("Parts_Cases_Case_CasePostCount",
                    () => shapeHelper.Parts_Cases_Case_CasePostCount(PostCount: part.PostCount))
                );
        }

        protected override DriverResult Editor(CasePart cAsePart, dynamic shapeHelper) {
            var results = new List<DriverResult> {
                ContentShape("Parts_Cases_Case_Fields",
                             () => shapeHelper.EditorTemplate(TemplateName: "Parts.Cases.Case.Fields", Model: cAsePart, Prefix: Prefix))
            };

            
            if (cAsePart.Id > 0)
                results.Add(ContentShape("Case_DeleteButton",
                    deleteButton => deleteButton));

            return Combined(results.ToArray());
        }

        protected override DriverResult Editor(CasePart cAsePart, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(cAsePart, Prefix, null, null);
            return Editor(cAsePart, shapeHelper);
        }

        protected override void Importing(CasePart part, Orchard.ContentManagement.Handlers.ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Description", description =>
                part.Description = description
            );

            context.ImportAttribute(part.PartDefinition.Name, "PostCount", postCount =>
                part.PostCount = Convert.ToInt32(postCount)
            );

            context.ImportAttribute(part.PartDefinition.Name, "FeedProxyUrl", feedProxyUrl =>
                part.FeedProxyUrl = feedProxyUrl
            );
        }

        protected override void Exporting(CasePart part, Orchard.ContentManagement.Handlers.ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Description);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PostCount", part.PostCount);
            context.Element(part.PartDefinition.Name).SetAttributeValue("FeedProxyUrl", part.FeedProxyUrl);
        }
    }
}