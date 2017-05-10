using System;
using System.Linq;
using System.Xml.Linq;

using Orchard.Commands;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentPicker.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Navigation.Models;
using Orchard.Security;

using Orchard.Core.Navigation.Services;
using Orchard.Settings;
using Orchard.Core.Title.Models;
using Orchard.UI.Navigation;
using LawBookCases.Module.Services;
using LawBookCases.Module.Models;
using Orchard;

namespace LawBookCases.ModuleCommands{
    public class CaseCommands : DefaultOrchardCommandHandler {
        private readonly IContentManager _contentManager;
        private readonly IMembershipService _membershipService;
        private readonly ICaseService _CaseService;
        private readonly IMenuService _menuService;
        private readonly ISiteService _siteService;
        private readonly INavigationManager _navigationManager;
        private readonly IArchiveService _archiveService;

        public CaseCommands(
            IContentManager contentManager,
            IMembershipService membershipService,
            ICaseService CaseService,
            IMenuService menuService,
            ISiteService siteService,
            INavigationManager navigationManager,
            IArchiveService archiveService) {
            _contentManager = contentManager;
            _membershipService = membershipService;
            _CaseService = CaseService;
            _menuService = menuService;
            _siteService = siteService;
            _navigationManager = navigationManager;
            _archiveService = archiveService;
        }

        [OrchardSwitch]
        public string FeedUrl { get; set; }

        [OrchardSwitch]
        public int CaseId { get; set; }

        [OrchardSwitch]
        public string Owner { get; set; }

        [OrchardSwitch]
        public string Slug { get; set; }

        [OrchardSwitch]
        public string Title { get; set; }

        [OrchardSwitch]
        public string Description { get; set; }

        [OrchardSwitch]
        public string MenuText { get; set; }

        [OrchardSwitch]
        public string MenuName { get; set; }

        [OrchardSwitch]
        public bool Homepage { get; set; }

        [CommandName("Case create")]
        [CommandHelp("Case create [/Slug:<slug>] /Title:<title> [/Owner:<username>] [/Description:<description>] [/MenuName:<name>] [/MenuText:<menu text>] [/Homepage:true|false]\r\n\t" + "Creates a new Case")]
        [OrchardSwitches("Slug,Title,Owner,Description,MenuText,Homepage,MenuName")]
        public void Create() {
            if (String.IsNullOrEmpty(Owner)) {
                Owner = _siteService.GetSiteSettings().SuperUser;
            }
            var owner = _membershipService.GetUser(Owner);

            if (owner == null) {
                Context.Output.WriteLine(T("Invalid username: {0}", Owner));
                return;
            }

            var Case = _contentManager.New("Case");
            Case.As<ICommonPart>().Owner = owner;
            Case.As<TitlePart>().Title = Title;
            if (!String.IsNullOrEmpty(Description)) {
                Case.As<CasePart>().Description = Description;
            }

            if (Homepage || !String.IsNullOrWhiteSpace(Slug)) {
                dynamic dCase = Case;
                if (dCase.AutoroutePart != null) {
                    dCase.AutoroutePart.UseCustomPattern = true;
                    dCase.AutoroutePart.CustomPattern = Homepage ? "/" : Slug;
                }
            }
            
            _contentManager.Create(Case);

            if (!String.IsNullOrWhiteSpace(MenuText)) {
                var menu = _menuService.GetMenu(MenuName);

                if (menu != null) {
                    var menuItem = _contentManager.Create<ContentMenuItemPart>("ContentMenuItem");
                    menuItem.Content = Case;
                    menuItem.As<MenuPart>().MenuPosition = _navigationManager.GetNextPosition(menu);
                    menuItem.As<MenuPart>().MenuText = MenuText;
                    menuItem.As<MenuPart>().Menu = menu;
                }
            }

            Context.Output.WriteLine(T("Case created successfully"));
        }

        [CommandName("Case import")]
        [CommandHelp("Case import /CaseId:<id> /FeedUrl:<feed url> /Owner:<username>\r\n\t" + "Import all items from <feed url> into the Case specified by <id>")]
        [OrchardSwitches("FeedUrl,CaseId,Owner")]
        public void Import() {
            var owner = _membershipService.GetUser(Owner);

            if(owner == null) {
                Context.Output.WriteLine(T("Invalid username: {0}", Owner));
                return;
            }

            XDocument doc;

            try {
                Context.Output.WriteLine(T("Loading feed..."));
                doc = XDocument.Load(FeedUrl);
                Context.Output.WriteLine(T("Found {0} items", doc.Descendants("item").Count()));
            }
            catch (Exception ex) {
                throw new OrchardException(T("An error occured while loading the feed at {0}.", FeedUrl), ex);
            }

            var Case = _CaseService.Get(CaseId, VersionOptions.Latest);

            if ( Case == null ) {
                Context.Output.WriteLine(T("Case not found with specified Id: {0}", CaseId));
                return;
            }

            foreach ( var item in doc.Descendants("item") ) {
                if (item != null) {
                    var postName = item.Element("title").Value;

                    Context.Output.WriteLine(T("Adding post: {0}...", postName.Substring(0, Math.Min(postName.Length, 40))));
                    var post = _contentManager.New("CasePost");
                    post.As<ICommonPart>().Owner = owner;
                    post.As<ICommonPart>().Container = Case;
                    post.As<TitlePart>().Title = postName;
                    post.As<BodyPart>().Text = item.Element("description").Value;
                    _contentManager.Create(post);
                }
            }

            Context.Output.WriteLine(T("Import feed completed."));
        }

        [CommandName("Case build archive")]
        [CommandHelp("Case build archive /CaseId:<id> \r\n\t" + "Rebuild the archive information for the Case specified by <id>")]
        [OrchardSwitches("CaseId")]
        public void BuildArchive() {

            var Case = _CaseService.Get(CaseId, VersionOptions.Latest);

            if (Case == null) {
                Context.Output.WriteLine(T("Case not found with specified Id: {0}", CaseId));
                return;
            }

            _archiveService.RebuildArchive(Case.As<CasePart>());
        }
    }
}