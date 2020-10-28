﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Teronis.Identity.AccountManaging;
using Teronis.Identity.Datransjects;
using Teronis.Identity.Entities;

namespace Teronis.Identity.Controllers
{
    [ApiController]
    public class AccountController<UserDescriptorType, UserType, UserCreationType, RoleDescriptorType, RoleType, RoleCreationType> : Controller
        where UserDescriptorType : IUserDescriptor
        where UserType : IAccountUserEntity
        where RoleDescriptorType : IRoleDescriptor
    {
        private readonly IAccountManager<UserType, RoleType> accountManager;
        private readonly IConvertUserDescriptor<UserDescriptorType, UserType> userDescriptorUserConverter;
        private readonly IConvertUser<UserType, UserCreationType> userUserCreationConverter;
        private readonly IConvertRoleDescriptor<RoleDescriptorType, RoleType> roleDescriptorRoleConverter;
        private readonly IConvertRole<RoleType, RoleCreationType> roleRoleCreationConverter;

        public AccountController(IAccountManager<UserType, RoleType> accountManager, IConvertUserDescriptor<UserDescriptorType, UserType> userDescriptorUserConverter,
            IConvertUser<UserType, UserCreationType> userUserCreationConverter, IConvertRoleDescriptor<RoleDescriptorType, RoleType> roleDescriptorRoleConverter,
            IConvertRole<RoleType, RoleCreationType> roleRoleCreationConverter)
        {
            this.accountManager = accountManager;
            this.userDescriptorUserConverter = userDescriptorUserConverter;
            this.userUserCreationConverter = userUserCreationConverter;
            this.roleDescriptorRoleConverter = roleDescriptorRoleConverter;
            this.roleRoleCreationConverter = roleRoleCreationConverter;
        }

        [HttpPost("roles/create")]
        [Authorize(Policy = AccountControllerDefaults.CanCreateRolePolicy)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesErrorResponseType(typeof(void))]
        //[ProducesResponseType(typeof(RoleCreationType), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoleCreationType>> Create(RoleDescriptorType roleDescriptor)
        {
            var roleEntity = roleDescriptorRoleConverter.Convert(roleDescriptor);
            var createdRoleEntity = await accountManager.CreateRoleAsync(roleEntity);
            var createdRoleDatransject = roleRoleCreationConverter.Convert(createdRoleEntity);
            return Json(createdRoleDatransject);
        }

        [HttpPost("users/create")]
        [Authorize(Policy = AccountControllerDefaults.CanCreateRolePolicy)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesErrorResponseType(typeof(void))]
        //[ProducesResponseType(typeof(RoleCreationType), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCreationType>> Create(UserDescriptorType userDescriptor)
        {
            var userEntity = userDescriptorUserConverter.Convert(userDescriptor);
            var createdUserEntity = await accountManager.CreateUserAsync(userEntity, userDescriptor.Password, roles: userDescriptor.Roles);
            var createdUserDatransject = userUserCreationConverter.Convert(createdUserEntity, userDescriptor.Roles);
            return Json(createdUserDatransject);
        }
    }
}
