using Newtonsoft.Json;
using Nois.Services.Authentication;
using Nois.Services.Data;
using Nois.Services.Users;
using Nois.WebApi.Framework;
using Nois.Api.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Nois.Framework.Infrastructure;
using Nois.Framework.Histories;
using Nois.Framework.Services;
using Nois.Framework.Settings;
using Nois.Framework.Loggings;
using Nois.Framework.Localization;
using Nois.Services.Products;
using Nois.Services.Categories;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// base api 
    /// </summary>

    /// <summary>
    ///  Base API Controller
    /// </summary>
    [LogActionFilter]
    [Authorize]
    [ValidateRefreshTokenEveryWhere]
    public partial class BaseApiController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        protected JsonResult<T> PartJson<T>(T content)
        {
            var jsonResolver = new IgnorableSerializerContractResolver();
            // ignore your specific property
            jsonResolver.Ignore(typeof(T), "Name");

            var jsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = jsonResolver };
            return Json(content, jsonSettings);
        }

        private Dictionary<string, object> _serviceList;
        private T GetService<T>() where T : class
        {
            if (_serviceList == null)
                _serviceList = new Dictionary<string, object>();
            var service = _serviceList.FirstOrDefault(s => s.Key == typeof(T).FullName);
            if (service.Key == null)
            {
                var sv = EngineContext.Instance.Resolve<T>();
                _serviceList.Add(typeof(T).FullName, sv);
                return sv;
            }
            return (T)service.Value;
        }

        #region Service

        /// <summary>
        /// 
        /// </summary>
        protected IHistoryService _historyService { get { return GetService<IHistoryService>(); } }
        /// <summary>
        /// action name service
        /// </summary>
        protected IActionNameService _actionNameService { get { return GetService<IActionNameService>(); } }
        /// <summary>
        /// client service
        /// </summary>
        protected IClientService _clientService { get { return GetService<IClientService>(); } }
        /// <summary>
        /// _encryptionService
        /// </summary>
        protected IEncryptionService _encryptionService { get { return GetService<IEncryptionService>(); } }
        /// <summary>
        /// setting service
        /// </summary>
        protected ISettingService _settingService { get { return GetService<ISettingService>(); } }
        /// <summary>
        /// log service
        /// </summary>
        protected ILogger _logService { get { return GetService<ILogger>(); } }
        /// <summary>
        /// work context
        /// </summary>
        protected IWorkContext _workContext { get { return GetService<IWorkContext>(); } }
        /// <summary>
        /// permission service
        /// </summary>
        protected IPermissionService _permissionService { get { return GetService<IPermissionService>(); } }
        /// <summary>
        /// user service
        /// </summary>
        protected IUserService _userService { get { return GetService<IUserService>(); } }
        /// <summary>
        /// user role service
        /// </summary>
        protected IUserRoleService _userRoleService { get { return GetService<IUserRoleService>(); } }
        /// <summary>
        /// localization service
        /// </summary>
        protected ILanguageService _languageService { get { return GetService<ILanguageService>(); } }
        /// <summary>
        /// localization service
        /// </summary>
        protected ILocalizationService _localizationService { get { return GetService<ILocalizationService>(); } }
        /// <summary>
        /// product service
        /// </summary>
        protected IProductService _productService { get { return GetService<IProductService>(); } }
        /// <summary>
        /// category service
        /// </summary>
        protected ICategoryService _categoryService { get { return GetService<ICategoryService>(); } }
        #endregion

        #region Methods
        /// <summary>
        /// hash identity
        /// </summary>
        /// <param name="id">identity</param>
        /// <returns></returns>
        protected string HashId(int id)
        {
            var res = "";
            var idHash = id.ToString().GetHashCode(); // 10 zeros
            if (idHash < 0) idHash *= -1;
            res = idHash.ToString();
            if (res.Length < 10)
            {
                var rand = "478513942486";
                res += rand.Substring(0, 10 - res.Length);
            }
            return res;
        }
        #endregion
    }
}