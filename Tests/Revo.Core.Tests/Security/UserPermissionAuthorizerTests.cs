﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Extensions;
using Revo.Core.Security;
using Revo.Testing.Security;
using Xunit;

namespace Revo.Core.Tests.Security
{
    public class UserPermissionAuthorizerTests
    {
        private FakeUserContext userContext;
        private IPermissionAuthorizationMatcher permissionAuthorizationMatcher;
        private IUserPermissionResolver userPermissionResolver;
        private IPermissionTypeRegistry permissionTypeRegistry;
        private UserPermissionAuthorizer sut;

        public UserPermissionAuthorizerTests()
        {
            userContext = new FakeUserContext();
            permissionAuthorizationMatcher = Substitute.For<IPermissionAuthorizationMatcher>();
            userPermissionResolver = Substitute.For<IUserPermissionResolver>();
            permissionTypeRegistry = Substitute.For<IPermissionTypeRegistry>();

            permissionTypeRegistry.GetPermissionTypeById(Guid.Parse("0F292EFD-792E-48EC-93DF-CD99EEDB5885"))
                .Returns(new PermissionType(Guid.Parse("0F292EFD-792E-48EC-93DF-CD99EEDB5885"), "permission"));

            sut = new UserPermissionAuthorizer(userContext, permissionAuthorizationMatcher,
                userPermissionResolver, permissionTypeRegistry);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CheckAuthorizationAsync(bool passes)
        {
            var user = Substitute.For<IUser>();

            var userPermissions = new List<Permission>()
            {
                new Permission(new PermissionType(Guid.Parse("7AD9EEB0-2D74-4F68-BEE1-17C67D687EDF"), "aaa"), "res1", "ctx1")
            };
            
            userPermissionResolver.GetUserPermissionsAsync(user).Returns(userPermissions);
            permissionAuthorizationMatcher.CheckAuthorization(userPermissions, Arg.Any<IReadOnlyCollection<Permission>>())
                .Returns(ci =>
                {
                    var requiredPermissions = ci.ArgAt<IReadOnlyCollection<Permission>>(1);
                    if (requiredPermissions.Count == 1 &&
                        requiredPermissions.First().PermissionType.Equals(permissionTypeRegistry.GetPermissionTypeById(
                            Guid.Parse("0F292EFD-792E-48EC-93DF-CD99EEDB5885")))
                        && requiredPermissions.First().ContextId == "context_id" &&
                        requiredPermissions.First().ResourceId == "resource_id")
                    {
                        return passes;
                    }

                    throw new ArgumentException();
                });

            bool result = await sut.CheckAuthorizationAsync(user, "0F292EFD-792E-48EC-93DF-CD99EEDB5885",
                "resource_id", "context_id");
            result.Should().Be(passes);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CheckCurrentUserAuthorizationAsync(bool passes)
        {
            userContext.FakePermissions = new List<Permission>()
            {
                new Permission(new PermissionType(Guid.Parse("7AD9EEB0-2D74-4F68-BEE1-17C67D687EDF"), "aaa"), "res1", "ctx1")
            };
            
            permissionAuthorizationMatcher.CheckAuthorization(userContext.FakePermissions, Arg.Any<IReadOnlyCollection<Permission>>())
                .Returns(ci =>
                {
                    var requiredPermissions = ci.ArgAt<IReadOnlyCollection<Permission>>(1);
                    if (requiredPermissions.Count == 1 &&
                        requiredPermissions.First().PermissionType.Equals(permissionTypeRegistry.GetPermissionTypeById(
                            Guid.Parse("0F292EFD-792E-48EC-93DF-CD99EEDB5885")))
                        && requiredPermissions.First().ContextId == "context_id" &&
                        requiredPermissions.First().ResourceId == "resource_id")
                    {
                        return passes;
                    }

                    throw new ArgumentException();
                });

            bool result = await sut.CheckCurrentUserAuthorizationAsync("0F292EFD-792E-48EC-93DF-CD99EEDB5885",
                "resource_id", "context_id");
            result.Should().Be(passes);
        }
    }
}
