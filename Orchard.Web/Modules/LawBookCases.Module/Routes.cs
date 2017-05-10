using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using LawBookCases.Module.Routing;

namespace LawBookCases.Module
{
    public class Routes : IRouteProvider {
        private readonly IArchiveConstraint _archiveConstraint;
        private readonly IRsdConstraint _rsdConstraint;

        public Routes(
            IArchiveConstraint archiveConstraint,
            IRsdConstraint rsdConstraint) {
            _archiveConstraint = archiveConstraint;
            _rsdConstraint = rsdConstraint;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/Create",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CaseAdmin"},
                                                                                      {"action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Edit",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CaseAdmin"},
                                                                                      {"action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Remove",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CaseAdmin"},
                                                                                      {"action", "Remove"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CaseAdmin"},
                                                                                      {"action", "Item"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Posts/Create",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePostAdmin"},
                                                                                      {"action", "Create"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Posts/{postId}/Edit",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePostAdmin"},
                                                                                      {"action", "Edit"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Posts/{postId}/Delete",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePostAdmin"},
                                                                                      {"action", "Delete"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Posts/{postId}/Publish",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePostAdmin"},
                                                                                      {"action", "Publish"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases/{caseId}/Posts/{postId}/Unpublish",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePostAdmin"},
                                                                                      {"action", "Unpublish"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Admin/Cases",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CaseAdmin"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Cases",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "Case"},
                                                                                      {"action", "List"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "YearCase",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePost"},
                                                                                      {"action", "GetYears"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "{*path}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "CasePost"},
                                                                                      {"action", "ListByArchive"}
                                                                                  },
                                                         new RouteValueDictionary {
                                                                                      {"path", _archiveConstraint},
                                                                                  },
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                             new RouteDescriptor {
                                                     Priority = 14,
                                                     Route = new Route(
                                                         "{*path}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "RemoteCasePublishing"},
                                                                                      {"action", "Rsd"}
                                                                                  },
                                                         new RouteValueDictionary {
                                                                                      {"path", _rsdConstraint}
                                                                                  },
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                              new RouteDescriptor {
                                                     Route = new Route(
                                                         "doc/{cAseId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "Case"},
                                                                                      {"action", "Docs"}
                                                                                  },
                                                         new RouteValueDictionary (),
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }, 
                               new RouteDescriptor {
                                                      Priority = -11,
                                   Route = new Route(
                                                         "{byYear}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "Case"},
                                                                                      {"action", "CaseByYear"}
                                                                                  },
                                                         new RouteValueDictionary () { { "byYear", @"\d+" } },
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                                new RouteDescriptor {

                                                Priority = -11,
                                                     Route = new Route(
                                                         "{byYear}/{cAseId}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"},
                                                                                      {"controller", "Case"},
                                                                                      {"action", "CaseByYearAndNumber"}
                                                             
                                                                                  },
                                                         new RouteValueDictionary (){ { "byYear", @"\d+" },
                                                                                      { "cAseId", @"\d+" }},
                                                         new RouteValueDictionary {
                                                                                      {"area", "LawBookCases.Module"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 }
                         };
        }
    }
}