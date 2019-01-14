using Nois.Core.Domain.Users;
using Nois.Framework.Infrastructure;
using Nois.Helper;
using Nois.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace Nois.Api.App_Start
{
    /// <summary>
    /// config for permission
    /// </summary>
    public static class PermissionConfig
    {
        /// <summary>
        /// update permission - action name every web is started
        /// </summary>
        /// <param name="isCreateUser"></param>
        public static void UpdatePermission(bool? isCreateUser = false)
        {
            //var _permissionService = EngineContext.Instance.Resolve<IPermissionService>();
            var actionNameService = EngineContext.Instance.Resolve<IActionNameService>();
            //var _encryptionService = EngineContext.Instance.Resolve<IEncryptionService>();
            //var _userRoleService = EngineContext.Instance.Resolve<IUserRoleService>();
            //var _userService = EngineContext.Instance.Resolve<IUserService>();
            //var _clientService = EngineContext.Instance.Resolve<IClientService>();

            var actionNameModels = new List<ActionName>();

            var asm = Assembly.GetExecutingAssembly();
            var controllers = asm.GetTypes()
                 .Where(type => typeof(ApiController).IsAssignableFrom(type) && type.Name != "BaseApiController");
            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Substring(0, controller.Name.Length - 10);
                var actionNames = controller.GetMethods().Where(method => method.IsPublic && method.Name != "ExecuteAsync" && !method.IsDefined(typeof(NonActionAttribute))
                 && (method.ReturnType == typeof(IHttpActionResult)
                 || method.ReturnType == typeof(HttpResponseMessage)
                 || method.ReturnType == typeof(Task<IHttpActionResult>)
                 || method.ReturnType == typeof(Task<HttpResponseMessage>))).Select(x => x.Name);

                actionNameModels.AddRange(actionNames.Select(a => new ActionName() { Name = a, Controller = controllerName }));
            }

            var actionNameEntities = actionNameService.GetAll();

            LinQHelper.Update(actionNameEntities, actionNameModels, (e, m) => e.Name == m.Name && e.Controller == m.Controller,
                delete: es=>actionNameService.Delete(es),
                insert: es=>actionNameService.Insert(es),
                insertedMapper: (m)=> { return m; });

            #region action name import
            /*
            //update all action name value
            var asm = Assembly.GetExecutingAssembly();
            var newPermissions = new List<Permission>();
            var oldActionNames = new List<ActionName>();
            //define allow anonymous action 
            var sharedActions = new List<ActionName> {
                        new ActionName
                        {
                            Controller = "User",
                            Name = "Detail"
                        },
                        new ActionName
                        {
                            Controller = "User",
                            Name = "Update"
                        }
                    };
            //get data from db for checking
            var actionNameDb = _actionNameService.GetAll();
            var permissionDb = _permissionService.GetAll();
            var index = permissionDb.Count - 1;
            //get all controller
            var _controllers = asm.GetTypes()
                 .Where(type => typeof(ApiController).IsAssignableFrom(type) && type.Name != "BaseApiController");
            foreach (var controller in _controllers)
            {
                var controllerName = controller.Name.Substring(0, controller.Name.Length - 10);
                var _actionNames = controller.GetMethods().Where(method => method.IsPublic && method.Name != "ExecuteAsync" && !method.IsDefined(typeof(NonActionAttribute))
                 && (method.ReturnType == typeof(IHttpActionResult)
                 || method.ReturnType == typeof(HttpResponseMessage)
                 || method.ReturnType == typeof(Task<IHttpActionResult>)
                 || method.ReturnType == typeof(Task<HttpResponseMessage>))).Select(x => x.Name);

                //get old action name
                oldActionNames.AddRange(actionNameDb.Where(x => string.Equals(x.Controller, controllerName, StringComparison.OrdinalIgnoreCase) && !_actionNames.Contains(x.Name)).ToList());
                //check new action name
                _actionNames = _actionNames.Where(x => !actionNameDb.Any(a => string.Equals(a.Controller, controllerName, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Name, x, StringComparison.OrdinalIgnoreCase)));
                //list action name for permisson
                var actionNameForRead = _actionNames.Where(x =>
                   string.Equals(x, "list", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(x, "Detail", StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                var actionNameForCreate = _actionNames.Where(x =>
                    string.Equals(x, "add", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(x, "import", StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                var actionNameForUpdate = _actionNames.Where(x =>
                     string.Equals(x, "update", StringComparison.OrdinalIgnoreCase)
                || string.Equals(x, "import", StringComparison.OrdinalIgnoreCase)
                   ).ToList();
                var actionNameForDelete = _actionNames.Where(x =>
                    string.Equals(x, "delete", StringComparison.OrdinalIgnoreCase)
                || string.Equals(x, "deactive", StringComparison.OrdinalIgnoreCase)
                || string.Equals(x, "cancel", StringComparison.OrdinalIgnoreCase)
                   ).ToList();
                var actionNameForOption = _actionNames.Except(actionNameForRead)
                    .Except(actionNameForCreate)
                    .Except(actionNameForUpdate)
                    .Except(actionNameForDelete)
                    .Where(x => !sharedActions.Any(a => a.Controller == controllerName && a.Name == x)).ToList();

                //add action name to permission 
                // read - screen/export/paper
                UpdatePermission(ref permissionDb, ref newPermissions, actionNameForRead, "read", controllerName, index);
                // create - import/manual
                UpdatePermission(ref permissionDb, ref newPermissions, actionNameForCreate, "create", controllerName, index);
                // update - import/manual
                UpdatePermission(ref permissionDb, ref newPermissions, actionNameForUpdate, "update", controllerName, index);
                // delete - temporary/permanly/canceled
                UpdatePermission(ref permissionDb, ref newPermissions, actionNameForDelete, "delete", controllerName, index);
                // another permission
                UpdatePermission(ref permissionDb, ref newPermissions, actionNameForOption, "option", controllerName, index);
            }
            //remove old action name 
            _actionNameService.Delete(oldActionNames);
            // update old permission
            _permissionService.Update(permissionDb.ToList());
            //add shared action name
            foreach (var item in sharedActions.Where(x => !actionNameDb.Any(a => string.Equals(a.Controller, x.Controller, StringComparison.OrdinalIgnoreCase) && string.Equals(a.Name, x.Name, StringComparison.OrdinalIgnoreCase))))
            {
                _actionNameService.Insert(item);
            }*/
            #endregion

            #region update user
            /*
            // create new user for test
            if (isCreateUser.Value)
            {
                var clientGuid = "a868cec2-5c40-4a90-b473-672f097e1695";
                var clientSecret = "c545d692961140bdaf5794a71a150fc1";
                var password = "123456";
                var username = "admin@nois.com";
                var salt = _encryptionService.CreateSaltKey(5);
                //create client
                var client = new Client()
                {
                    Active = true,
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationTypes.JavaScript,
                    ClientGuid = clientGuid,
                    Name = "WEB APPLICATION",
                    RefreshTokenLifeTime = int.MaxValue,
                    Secret = clientSecret,
                };
                _clientService.Insert(client);
                //create user
                var user = new User
                {
                    Password = _encryptionService.CreatePasswordHash(password, salt),
                    PasswordSalt = salt,
                    Email = "admin@nois.com",
                    Gender = Gender.Male,
                    IsActive = true,
                    FullName = "Admin",
                    UserGuid = Guid.NewGuid(),
                    Username = username,
                    UserRoles = new List<UserRole>
                        {
                             // create role
                        new UserRole
                        {
                            IsActive = true,
                            Name = "admin",
                            SystemName = "Administrator",
                            Permissions  = newPermissions
                        }
                        }
                };
                _userService.Insert(user);
            }
            //update action name for all administrator
            else
            {
                var roles = _userRoleService.GetAll();
                var role = roles.FirstOrDefault(x => string.Equals(x.SystemName, "Administrator", StringComparison.OrdinalIgnoreCase));
                if (role != null)
                {
                    foreach (var permission in newPermissions)
                    {
                        role.Permissions.Add(permission);
                    }
                    _userRoleService.Update(role);
                }
            }
            */
            #endregion
        }
        static void UpdatePermission(ref IList<Permission> permissionDb, ref List<Permission> newPermissions, List<string> actionNames, string permissionName, string controllerName, int index)
        {
            if (actionNames.Any())
            {
                var oldPermission = permissionDb.FirstOrDefault(x => string.Equals(x.SystemName, $"{controllerName}.{permissionName}", StringComparison.OrdinalIgnoreCase));
                if (oldPermission == null)
                {
                    newPermissions.Add(new Permission
                    {
                        Name = $"{controllerName} {permissionName}",
                        SystemName = $"{controllerName}.{permissionName}",
                        Order = index++,
                        Category = controllerName,
                        ActionNames = actionNames.Select(name => new ActionName
                        {
                            Controller = controllerName,
                            Name = name
                        }).ToList()
                    });
                }
                //already permission
                else
                {
                    foreach (var actionName in actionNames)
                    {
                        oldPermission.ActionNames.Add(new ActionName
                        {
                            Controller = controllerName,
                            Name = actionName
                        });
                    }
                }
            }
        }
    }
}