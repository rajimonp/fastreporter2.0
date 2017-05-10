
using LawBookCases.Module.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace LawBookCases.Module.Drivers {
    [OrchardFeature("LawBookCases.Module.RemotePublishing")]
    public class RemoteCasePublishingDriver : ContentPartDriver<CasePart> {
        protected override DriverResult Display(CasePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Cases_RemotePublishing", shape => shape.Case(part));
        }
    }
}