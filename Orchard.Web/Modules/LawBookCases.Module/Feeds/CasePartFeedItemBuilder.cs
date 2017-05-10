using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Feeds;
using Orchard.Core.Feeds.Models;
using Orchard.Mvc.Extensions;
using Orchard.Services;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Feeds {
    public class CasePartFeedItemBuilder : IFeedItemBuilder {
        private IContentManager _contentManager;
        public CasePartFeedItemBuilder(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public void Populate(FeedContext context) {
            var containerIdValue = context.ValueProvider.GetValue("containerid");
            if (containerIdValue == null)
                return;

            var containerId = (int)containerIdValue.ConvertTo(typeof(int));
            var container = _contentManager.Get(containerId);

            if (container == null) {
                return;
            }

            if (container.ContentType != "Case") {
                return;
            }

            var cAse = container.As<CasePart>();

            if (context.Format == "rss") {
                context.Response.Element.SetElementValue("description", cAse.Description);
            }
            else {
                context.Builder.AddProperty(context, null, "description", cAse.Description);
            }
        }
    }
}