using LawBookCases.Module.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using LawBookCases.Module.ViewModels;

namespace LawBookCases.Module.Driver
{
    public class CasePostAttribPartDriver : ContentPartDriver<CasePostAttribPart>
    {
        protected override DriverResult Display(CasePostAttribPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Cases_Post_Attributes",
                () =>
                { 
                  //  var content = part;
                 //   var content.Heldat=
                    return shapeHelper.Parts_Cases_Post_Attributes(DisplayData: part);
                });
                     
        }

        protected override DriverResult Editor(CasePostAttribPart part, dynamic shapeHelper)
        {
           
            return ContentShape("Parts_Cases_Post_Attributes_Edit",
              () => shapeHelper.EditorTemplate(
                  TemplateName: "Parts.Cases.Post.Attributes", 
                  Model: part,
                  Prefix: Prefix));
        }

        protected override DriverResult Editor(CasePostAttribPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, updater, shapeHelper);
        }
    }
}