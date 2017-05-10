using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace LawBookCases.Module{
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageCases = new Permission { Description = "Manage cases for others", Name = "ManageCases" };
        public static readonly Permission ManageOwnCases = new Permission { Description = "Manage own cases", Name = "ManageOwnCases", ImpliedBy = new[] { ManageCases } };

        public static readonly Permission ManageCasesPosts = new Permission { Description = "Manage cases post for others", Name = "ManageCasePost" };
        public static readonly Permission ManageOwnCasePosts = new Permission { Description = "Manage own cases", Name = "ManageOwnCases", ImpliedBy = new[] { ManageCases } };
        public static readonly Permission ManageCaseAbstract = new Permission { Description = "Update Case Abstract ", Name = "ManageOwnCases", ImpliedBy = new[] { ManageCases } };

        public static readonly Permission ListunpublishedCases = new Permission { Description = "List all un published case ", Name = "ListunpublishedCases", ImpliedBy = new[] { ManageCases } };
        public static readonly Permission AcquireUnpublisehdCase = new Permission { Description = "Acquier case for adding abstract ", Name = "AcquireUnpublisehdCase", ImpliedBy = new[] { ManageCases } };


        public static readonly Permission PublishCasePost = new Permission { Description = "Publish or unpublish case post for others", Name = "PublishCasePost", ImpliedBy = new[] { ManageCases } };
        public static readonly Permission PublishOwnCasePost = new Permission { Description = "Publish or unpublish own case post", Name = "PublishOwnCasePost", ImpliedBy = new[] { PublishCasePost, ManageOwnCases } };
        public static readonly Permission EditCasePost = new Permission { Description = "Edit case posts for others", Name = "EditCasePost", ImpliedBy = new[] { PublishCasePost } };
        public static readonly Permission EditOwnCasePost = new Permission { Description = "Edit own case posts", Name = "EditOwnCasePost", ImpliedBy = new[] { EditCasePost, PublishOwnCasePost } };
        public static readonly Permission DeleteCasePost = new Permission { Description = "Delete case post for others", Name = "DeleteCasePost", ImpliedBy = new[] { ManageCases } };
        public static readonly Permission DeleteOwnCasePost = new Permission { Description = "Delete own case post", Name = "DeleteOwnCasePost", ImpliedBy = new[] { DeleteCasePost, ManageOwnCases } };

        public static readonly Permission MetaListCases = new Permission { ImpliedBy = new[] { EditCasePost, PublishCasePost, DeleteCasePost }, Name = "MetaListCases"};
        public static readonly Permission MetaListOwnCases = new Permission { ImpliedBy = new[] { EditOwnCasePost, PublishOwnCasePost, DeleteOwnCasePost }, Name = "MetaListOwnCases" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageOwnCases,
                ManageCases,
                EditOwnCasePost,
                EditCasePost,
                PublishOwnCasePost,
                PublishCasePost,
                DeleteOwnCasePost,
                DeleteCasePost,
                ManageCasesPosts,
                ManageOwnCasePosts,
                ListunpublishedCases,
                AcquireUnpublisehdCase

            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageCases}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {PublishCasePost,EditCasePost,DeleteCasePost}
                },
                new PermissionStereotype {
                    Name = "Moderatorr",
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {ManageOwnCases}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] { ManageOwnCasePosts,ManageCasesPosts, EditOwnCasePost }
                },
                 new PermissionStereotype {
                    Name = "Law Interns",
                    Permissions = new[] {ListunpublishedCases, ManageCaseAbstract, AcquireUnpublisehdCase, ManageCasesPosts, EditOwnCasePost }
                },
            };
        }

    }
}


