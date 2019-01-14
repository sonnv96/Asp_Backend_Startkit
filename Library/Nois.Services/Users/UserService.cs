using Nois.Framework.Services;
using System;
using System.Linq;
using Nois.Framework.Caching;
using Nois.Framework.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using Nois.Core.Domain.Users;
using Nois.Services.Data;
using Nois.Helper;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;

namespace Nois.Services.Users
{
    /// <summary>
    /// Implement User service
    /// </summary>
    public partial class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IWorkContext _workContext;
        private readonly IEncryptionService _encryptionService;
        /// <summary>
        /// 0: email address
        /// </summary>
        private const string GET_USER_BY_EMAIL = "Nois.Api.User.GetUser.byEmail-{0}";
        /// <summary>
        /// 0: username
        /// </summary>
        private const string GET_USER_BY_USERNAME = "Nois.Api.User.GetUser.byusername-{0}";
        /// <summary>
        /// 0: barcode
        /// </summary>
        private const string GET_USER_BY_BARCODE = "Nois.Api.User.GetUser.bybarcode-{0}";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="cacheManager"></param>
        public UserService(IRepository<User> userRepository, IEncryptionService encryptionService, IUserRoleService userRoleService, IWorkContext workContext, ICacheManager cacheManager) : base(userRepository, cacheManager)
        {
            _workContext = workContext;
            _encryptionService = encryptionService;
            _userRoleService = userRoleService;
        }
        /// <summary>
        /// Pattern key
        /// </summary>
        protected override string PatternKey
        {
            get
            {
                return "Nois.Api.User.";
            }
        }
        #region Synchronous
        /// <summary>
        /// Get user by email address
        /// </summary>
        /// <param name="email">user email address</param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            var key = String.Format(GET_USER_BY_EMAIL, email);

            return _cacheManager.Get(key, () => { return _tRepository.Table.FirstOrDefault(u => u.Email == email); });
        }
        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">username address</param>
        /// <returns></returns>
        public User GetUserByUsername(string username)
        {
            var key = String.Format(GET_USER_BY_USERNAME, username);

            return _cacheManager.Get(key, () => { return _tRepository.Table.FirstOrDefault(u => u.Username == username); });
        }
        //public User GetUserByBarcode(string barcode)
        //{
        //    var key = String.Format(GET_USER_BY_BARCODE + "async", barcode);

        //    return _cacheManager.Get(key, () => { return _tRepository.Table.FirstOrDefault(u => u.Barcode.Value == barcode); });
        //}
        public IPagedList<User> Search(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false, bool includeCurrentUser = true)
        {
            var query = _tRepository.Table;
            query = query.Where(x => !x.Username.ToLower().Equals("admin"));
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Username.Contains(textSearch) || x.FullName.Contains(textSearch) || x.Email.Contains(textSearch));
            if (!includeCurrentUser && _workContext.CurrentUser != null)
                query = query.Where(x => x.Username != _workContext.CurrentUser.Username);
            return new PagedList<User>(query.OrderByDynamic(propertySorting, "Username", orderDescending), pageIndex, pageSize) as IPagedList<User>;
        }
        #endregion
        #region Asynchronous
        /// <summary>
        /// Get user by email address
        /// </summary>
        /// <param name="email">user email address</param>
        /// <returns></returns>
        public Task<User> GetUserByEmailAsync(string email)
        {
            var key = String.Format(GET_USER_BY_EMAIL + "async", email);

            return _cacheManager.Get(key, () => { return _tRepository.Table.FirstOrDefaultAsync(u => u.Email == email); });
        }
        /// <summary>
        /// Get user by username async
        /// </summary>
        /// <param name="username">username address</param>
        /// <returns></returns>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var key = String.Format(GET_USER_BY_USERNAME + "async", username);

            return await _cacheManager.Get(key, async () => { return await _tRepository.Table.FirstOrDefaultAsync(u => u.Username == username); });
        }

        //public async Task<User> GetUserByBarcodeAsync(string barcode)
        //{
        //    var key = String.Format(GET_USER_BY_BARCODE + "async", barcode);

        //    return await _cacheManager.Get(key, async () => { return await _tRepository.Table.FirstOrDefaultAsync(u => u.Barcode.Value == barcode); });
        //}
        public async Task<IPagedList<User>> SearchAsync(int pageIndex = 0, int pageSize = int.MaxValue, string textSearch = null, string propertySorting = null, bool orderDescending = false)
        {
            var query = _tRepository.Table;
            if (!string.IsNullOrEmpty(textSearch))
                query = query.Where(x => x.Username.Contains(textSearch) || x.FullName.Contains(textSearch) || x.Email.Contains(textSearch));
            return await Task.FromResult(new PagedList<User>(query.OrderByDynamic(propertySorting, "Username", orderDescending), pageIndex, pageSize) as IPagedList<User>);
        }

        public void ImportUser(Stream stream, string fileName)
        {
            var users = ImportPartSuppliers(stream, fileName);

            var spls = _tRepository.Table.ToList();

            var x = users.Where(u => u.UserRoles == null);
            var y = spls.Where(s => s.UserRoles == null);

            var up = spls.Join(users, ds => ds.Username.ToLower(), es => es.Username.ToLower(), (ds, es) =>
            {
                if (ds == null)
                    return null;
                if (es == null)
                    return ds;
                var isChange = false;
                if (ds.FullName != es.FullName)
                { ds.FullName = es.FullName; isChange = true; }
                if (ds.IsActive != es.IsActive)
                { ds.IsActive = es.IsActive; isChange = true; }
                if (ds.Password != es.Password)
                { ds.Password = es.Password; isChange = true; }
                try
                {
                    if (ds.UserRoles.Any(ur => es.UserRoles.Any(r => r.Id != ur.Id)))
                    { ds.UserRoles = es.UserRoles; isChange = true; }
                }
                catch (Exception e)
                {

                }

                if (isChange)
                    return ds;
                else
                    return null;
            }).Where(p => p != null).Distinct().ToList();
            if (up.Count > 0)
                _tRepository.Update(up);

            var ip = (from ep in users
                      join dp in spls on ep.Username equals dp.Username into ljp
                      from p in ljp.DefaultIfEmpty()
                      select p == null ? ep : null).Where(p => p != null).Distinct().ToList();
            if (ip.Count > 0)
            {
                _tRepository.Insert(ip);
            }
        }

        private List<User> ImportPartSuppliers(Stream stream, string fileName)
        {
            var users = new List<User>();
            var ext = Path.GetExtension(fileName);

            var userroles = new List<UserRole>();
            userroles.Add(_userRoleService.GetBySystemName("Administrators"));
            userroles.Add(_userRoleService.GetBySystemName("AdministratorStaffs"));
            userroles.Add(_userRoleService.GetBySystemName("Operators"));
            userroles.Add(_userRoleService.GetBySystemName("OperatorStaffs"));

            if (ext == ".csv")
            {
                // read csv file
                using (var sr = new StreamReader(stream))
                {
                    string Fulltext;
                    while (!sr.EndOfStream)
                    {
                        Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                        string[] rows = Fulltext.Split('\n'); //split full file text into rows
                        for (int i = 1; i < rows.Count() - 1; i++)
                        {
                            string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values  

                            var user = new User
                            {
                                Username = rowValues[1],
                                Password = _encryptionService.CreatePasswordHash(rowValues[2], "123456"),
                                FullName = rowValues[3],
                                PasswordSalt = "123456",
                                IsActive = rowValues[5] == "0",
                                UserGuid = Guid.NewGuid()
                            };

                            var roleIndex = 0;
                            if (int.TryParse(rowValues[4], out roleIndex) && roleIndex < 4)
                                user.UserRoles.Add(userroles[roleIndex]);

                            users.Add(user);
                        }
                    }
                }
            }
            else
            {
                // read excel file
                using (var pck = new ExcelPackage(stream))
                {
                    var ws = pck.Workbook.Worksheets.First();
                    var startRow = 5;
                    var endRow = ws.Dimension.End.Row;

                    for (var rowNum = startRow; rowNum <= endRow; rowNum++)
                    {
                        try
                        {
                            var user = new User
                            {
                                Username = ws.Cells[$"B{rowNum}"].Text,
                                Password = _encryptionService.CreatePasswordHash(ws.Cells[$"C{rowNum}"].Text, "123456"),
                                FullName = ws.Cells[$"D{rowNum}"].Text,
                                PasswordSalt = "123456",
                                IsActive = ws.Cells[$"F{rowNum}"].Text == "0"
                            };

                            var roleIndex = 0;
                            if (int.TryParse(ws.Cells[$"E{rowNum}"].Text, out roleIndex))
                                user.UserRoles.Add(userroles[roleIndex]);

                            users.Add(user);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }

            return users;
        }
        #endregion
    }
}