﻿using System;
using System.Collections.Generic;
using GTRevo.Core.Security;

namespace GTRevo.Platform.Security
{
    public class PermissionCache
    {
        public PermissionCache()
        {
        }

        public IEnumerable<Permission> GetRolePermissions(Guid roleId, IRolePermissionResolver rolePermissionResolver)
        {
            //TODO: cache
            return rolePermissionResolver.GetRolePermissions(roleId);
        }

        public void Invalidate()
        {
            throw new NotImplementedException();
        }
    }
}
