using System.Web.Routing;
using Orchard;
using LawBookCases.Module.Models;

namespace LawBookCases.Module.Routing {
    public interface IArchiveConstraint : IRouteConstraint, ISingletonDependency {
        string FindPath(string path);
        ArchiveData FindArchiveData(string path);
    }
}