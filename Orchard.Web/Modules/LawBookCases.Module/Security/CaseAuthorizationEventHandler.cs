using System.Web.UI.WebControls;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Security;
using Orchard.Security.Permissions;


namespace LawBookCases.Module.Security {
    public class CaseAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Checking(CheckAccessContext context) { }
        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context) {
            if (!context.Granted &&
                context.Content.Is<ICommonPart>()) {

                if (context.Permission.Name == Orchard.Core.Contents.Permissions.PublishContent.Name && context.Content.ContentItem.ContentType == "CasePost") {
                    context.Adjusted = true;
                    context.Permission = Permissions.PublishCasePost;
                }
                else if (OwnerVariationExists(context.Permission) &&
                    HasOwnership(context.User, context.Content)) {
                    context.Adjusted = true;
                    context.Permission = GetOwnerVariation(context.Permission);
                }
            }
        }

        private static bool HasOwnership(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            if (HasOwnershipOnContainer(user, content)) {
                return true;
            }

            var common = content.As<ICommonPart>();
            if (common == null || common.Owner == null)
                return false;

            return user.Id == common.Owner.Id;
        }

        private static bool HasOwnershipOnContainer(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var common = content.As<ICommonPart>();
            if (common == null || common.Container == null)
                return false;

            common = common.Container.As<ICommonPart>();
            if (common == null || common.Container == null)
                return false;

            return user.Id == common.Owner.Id;
        }

        private static bool OwnerVariationExists(Permission permission) {
            return GetOwnerVariation(permission) != null;
        }

        private static Permission GetOwnerVariation(Permission permission) {
            if (permission.Name == Permissions.PublishCasePost.Name)
                return Permissions.PublishOwnCasePost;
            if (permission.Name == Permissions.EditCasePost.Name)
                return Permissions.EditOwnCasePost;
            if (permission.Name == Permissions.DeleteCasePost.Name)
                return Permissions.DeleteOwnCasePost;
            if (permission.Name == Orchard.Core.Contents.Permissions.ViewContent.Name)
                return Orchard.Core.Contents.Permissions.ViewOwnContent;
            if (permission.Name == Permissions.MetaListCases.Name)
                return Permissions.MetaListOwnCases;

            return null;
        }
    }
}