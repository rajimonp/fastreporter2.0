using System;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Tokens;
using LawBookCases.Module.Services;
using LawBookCases.Module.Models;


namespace LawBookCases.Module.Tokens {

    public class CasePostTokens : ITokenProvider {
        private readonly IContentManager _contentManager;
        private readonly ICasePostService _caseposttService;

        public CasePostTokens(
            IContentManager contentManager,
            ICasePostService caseposttService) {
            _contentManager = contentManager;
            _caseposttService = caseposttService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("Content", T("Comments"), T("Comments"))
                .Token("CaseTitle", T("Case Title Name"), T("The Case Title."))
                .Token("CaseYear", T("Case Held year"), T("year at which case held"))
                .Token("CaseNumber", T("Case number for the yearr"), T("Year of case"))
                ;
        }

        public void Evaluate(EvaluateContext context) {
            context.For<IContent>("Content")
                .Token("CaseTitle", CaseTitle)
                .Chain("CaseTitle", "Text", CaseTitle)
              
                .Token("CaseYear", CaseYear)
                .Chain("CaseYear", "int", CaseYear)

                .Token("CaseNumber", CaseNumber)
                .Chain("CaseNumber", "int", CaseNumber)

                ;
        }
       


        private static string CaseTitle(IContent comment) {
            var commentPart = comment.As<CasePostPart>().CasePart;

            return !String.IsNullOrWhiteSpace(commentPart.Name) ? commentPart.Name.ToLower() : commentPart.FeedProxyUrl;
        }
        private static string CaseYear(IContent comment)
        {
            var commentPart = comment.As<CasePostPart>().Get<CasePostAttribPart>();
            return commentPart.CaseYear.ToString();
         }
        private static string CaseNumber(IContent comment)
        {
            var commentPart = comment.As<CasePostPart>().Get<CasePostAttribPart>();
            return commentPart.CaseNumber.ToString();
        }
    }
}
