using Raspbe2ry.Framework.AuthCore.Entities;
using Raspbe2ry.Framework.AuthCore.Interfaces;
using Raspbe2ry.Framework.AuthCore.Models;
using Raspbe2ry.Framework.Domain.Specifications.PermissionSpecifications;
using Raspbe2ry.Framework.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspbe2ry.Framework.Domain.Services
{
    public class PermissionService : IPermissionService
    {
        private IRepository<RolePermission> _rolePermissionRepository;
        private IRepository<Role> _roleRepository;
        private IRepository<User> _userRepository;

        public PermissionService(
            IRepository<RolePermission> rolePermissionRepository,
            IRepository<Role> roleRepository,
            IRepository<User> userRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public List<FeaturesResponse> GetFeaturesList()
        {
            var rolePermissions = _rolePermissionRepository.Find(new RolePermissionsIncludingPermissions());

            List<FeaturesResponse> response = rolePermissions
                .GroupBy(x => new { 
                    FeatureId = x.PermissionId, 
                    FeatureName = x.Permission.FeatureName, 
                    FeatureLink = x.Permission.FeatureLink 
                })
                .Select(y => new FeaturesResponse()
                { 
                    FeatureId = y.Key.FeatureId,
                    FeatureName = y.Key.FeatureName,
                    FeatureLink = y.Key.FeatureLink,
                    RoleIds = y.Select(x=>x.RoleId).ToList()
                })
                .ToList();

            return response;
        }

        public async Task<List<RoleAssignement>> GetRoleAssignementForUser(int userId)
        {
            var roles = await _roleRepository.GetAllAsync();
            var user = await _userRepository.GetByIdAsync(userId);

            List<RoleAssignement> roleAssignements = roles.Select(x => new RoleAssignement()
            {
                RoleId = x.Id,
                RoleName = x.Name,
                Assigned = x.Id == user.RoleId,
            }).ToList();

            return roleAssignements;
        }
    }
}