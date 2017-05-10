using System;
using System.Web.Routing;
using Orchard.Core.Feeds;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Extensions {
    public static class FeedManagerExtensions {
        public static void Register(this IFeedManager feedManager, CasePart casePart, string blogTitle) {

            if (String.IsNullOrWhiteSpace(casePart.FeedProxyUrl)) {
                feedManager.Register(blogTitle, "rss", new RouteValueDictionary {{"containerid", casePart.Id}});
            }
            else {
                feedManager.Register(blogTitle, "rss", casePart.FeedProxyUrl);
            }

            if (casePart.EnableCommentsFeed) {
                feedManager.Register(blogTitle + " - Comments", "rss", new RouteValueDictionary {{"commentedoncontainer", casePart.Id}});
            }
        }
    }
}
