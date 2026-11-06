using System.Collections.Generic;
using System.Linq;
using AccessOps.Services;
using Xunit;

namespace AccessOps.Tests
{
    public sealed class PermissionServiceTests
    {
        [Fact]
        public void Roles_expand_through_the_configured_map()
        {
            using var host = new TestHost();

            var set = host.Permissions.ResolveFor("Employee;Manager", new List<PermissionClaim>());

            Assert.True(set.Has(Permission.ApproveRequest));
            Assert.True(set.Has(Permission.ViewDepartmentDocuments));
            Assert.False(set.Has(Permission.ManageUsers));
        }

        [Fact]
        public void A_denial_wins_over_a_role_and_over_a_grant()
        {
            using var host = new TestHost();

            var set = host.Permissions.ResolveFor("Manager", new[]
            {
                new PermissionClaim("grant", Permission.UploadDocument),
                new PermissionClaim("deny", Permission.UploadDocument),
                new PermissionClaim("deny", Permission.ApproveRequest)
            });

            Assert.False(set.Has(Permission.UploadDocument));
            Assert.False(set.Has(Permission.ApproveRequest));
            Assert.True(set.Has(Permission.SubmitRequest));
        }

        [Fact]
        public void A_grant_from_the_identity_store_adds_a_permission()
        {
            using var host = new TestHost();

            // r.almeida (4131) is an Employee with a seeded grant of UploadDocument
            var set = host.Permissions.ResolveFor(host.Employees.Find(4131));

            Assert.True(set.Has(Permission.UploadDocument));
        }

        [Fact]
        public void The_resolved_set_is_sorted_and_has_no_way_to_add()
        {
            using var host = new TestHost();
            var set = host.Permissions.ResolveFor("Administrator", new List<PermissionClaim>());

            var names = set.Select(p => p.ToString()).ToList();
            Assert.Equal(names.OrderBy(n => n, System.StringComparer.Ordinal), names);
            Assert.Null(typeof(PermissionSet).GetMethod("Add"));
        }
    }
}
