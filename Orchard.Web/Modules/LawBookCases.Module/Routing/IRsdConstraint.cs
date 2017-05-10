using Orchard;
using System.Web.Routing;

namespace LawBookCases.Module.Routing {
    public interface IRsdConstraint : IRouteConstraint, ISingletonDependency {
        string FindPath(string path);
    }
}