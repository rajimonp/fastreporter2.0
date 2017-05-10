using Orchard.Alias.Implementation.Holder;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace LawBookCases.Module.Routing {
    public class RsdConstraint : IRsdConstraint {
        private readonly IAliasHolder _aliasHolder;

        public RsdConstraint(IAliasHolder aliasHolder) {
            _aliasHolder = aliasHolder;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {
            if (routeDirection == RouteDirection.UrlGeneration)
                return true;

            object value;
            if (values.TryGetValue(parameterName, out value)) {
                var parameterValue = Convert.ToString(value);

                var path = FindPath(parameterValue);
                if(path == null) {
                    return false;
                }

                AliasInfo aliasInfo;
                if (!_aliasHolder.GetMap("LawBookCases.Module").TryGetAlias(path, out aliasInfo))
                {
                    return false;
                }

                var isCase =
                    //routeValues.ContainsKey("area") &&
                    //routeValues["area"] == "LawBookCases.Module" &&
                    aliasInfo.RouteValues.ContainsKey("controller") &&
                    aliasInfo.RouteValues["controller"] == "Case" &&
                    aliasInfo.RouteValues.ContainsKey("action") &&
                    aliasInfo.RouteValues["action"] == "Item"
                    ;

                return isCase;
            }

            return false;
        }

        public string FindPath(string path) {
            if (path.EndsWith("/rsd", StringComparison.OrdinalIgnoreCase)) {
                return path.Substring(0, path.Length - "/rsd".Length);
            }

            // case is on homepage
            if(path.Equals("rsd", StringComparison.OrdinalIgnoreCase)) {
                return String.Empty;
            }

            return null;
        }
    }
}