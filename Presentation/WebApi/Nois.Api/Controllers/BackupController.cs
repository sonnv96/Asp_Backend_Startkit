using System;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using Nois.Api.Models.Backups;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.Linq;
using Nois.Helper;
using Nois.WebApi.Framework;
using System.Web.Http;

namespace Nois.Api.Controllers
{
    /// <summary>
    /// Backup Api
    /// </summary>
    [RoutePrefix("backups")]
    public class BackupController : BaseApiController
    {
        string dirBackup = "backupdb";
        string bakDateFormat = "ddMMyyyyHHmmss";

        /// <summary>
        /// Get list Backup
        /// </summary>
        /// <remarks>
        /// When user want to get list Backup, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of get list Backup", typeof(BackupListModel))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpGet]
        public IHttpActionResult List(int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var model = new BackupListModel();

            try
            {
                var backupDIR = HttpContext.Current.Server.MapPath($"~/{dirBackup}");
                if (!Directory.Exists(backupDIR))
                    Directory.CreateDirectory(backupDIR);

                var dir = new DirectoryInfo(backupDIR);
                var files = dir.GetFiles("*.bak");
                model.Total = files.Count();
                foreach (var file in files.OrderByDescending(f => f.CreationTime).Skip((pageIndex - 1) * pageSize).Take(pageSize))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.Name);
                    var strsSplit = fileName.Split('_');
                    var count = strsSplit.Count();
                    model.BackupList.Add(new BackupModel
                    {
                        Id = fileName,
                        Name = fileName.Substring(0, fileName.Length - strsSplit[count - 1].Length - strsSplit[count - 2].Length - 2),
                        CreatedBy = strsSplit[count - 2],
                        CreatedOn = DateTime.ParseExact(strsSplit[count - 1], bakDateFormat, null)
                    });
                }

                return new HttpApiActionResult(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, model);
            }
        }

        /// <summary>
        /// Add a Backup
        /// </summary>
        /// <remarks>
        /// When user want to add a Backup, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(200, "Returns the result of add a Backup", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Add(BackupAddModel model)
        {
            var res = new ApiJsonResult();

            try
            {
                //Check is valid
                if (!ModelState.IsValid)
                {
                    res.ErrorMessages.AddRange(ModelState.ToListErrorMessage());
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                var conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                var decodeConString = conString.Decrypt();
                var sSB = new SqlConnectionStringBuilder(decodeConString);

                var backupDIR = HttpContext.Current.Server.MapPath($"~/{dirBackup}");
                if (!Directory.Exists(backupDIR))
                    Directory.CreateDirectory(backupDIR);

                var con = new SqlConnection
                {
                    ConnectionString = decodeConString
                };
                con.Open();
                var filePath = $"{backupDIR}\\{model.Name}_{_workContext.CurrentUser.FullName}_{DateTime.Now.ToString(bakDateFormat)}.bak";
                var sqlcmd = new SqlCommand();
                sqlcmd = new SqlCommand($"backup database {sSB.InitialCatalog} to disk='{filePath}'", con);
                sqlcmd.ExecuteNonQuery();
                con.Close();

                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Restore a Backup
        /// </summary>
        /// <remarks>
        /// When user want to add a Backup, use this API for getting
        /// </remarks>
        /// <returns></returns>
        [Route("restore/{id}")]
        [SwaggerResponse(200, "Returns the result of restore a Backup", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpPost]
        public IHttpActionResult Restore(string id)
        {
            var res = new ApiJsonResult();

            try
            {
                var backupDIR = HttpContext.Current.Server.MapPath($"~/{dirBackup}");
                if (!Directory.Exists(backupDIR))
                    Directory.CreateDirectory(backupDIR);
                var filePath = $"{backupDIR}\\{id}.bak";

                if (File.Exists(filePath))
                {
                    var conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                    var decodeConString = conString.Decrypt();
                    var sSB = new SqlConnectionStringBuilder(decodeConString);

                    var connect = new SqlConnection(decodeConString);
                    connect.Open();
                    SqlCommand command;
                    command = new SqlCommand("use master", connect);
                    command.ExecuteNonQuery();
                    command = new SqlCommand($"ALTER DATABASE {sSB.InitialCatalog} SET OFFLINE WITH ROLLBACK IMMEDIATE", connect);
                    command.ExecuteNonQuery();
                    command = new SqlCommand($"restore database {sSB.InitialCatalog} from disk = '{filePath}' WITH REPLACE", connect);
                    command.ExecuteNonQuery();
                    command = new SqlCommand($"ALTER DATABASE {sSB.InitialCatalog} SET ONLINE", connect);
                    command.ExecuteNonQuery();
                    connect.Close();
                    return new HttpApiActionResult(HttpStatusCode.OK, res);
                }
                else
                {
                    res.ErrorMessages.Add("Backup.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }
            }
            catch (Exception ex)
            {
                res.ErrorMessages.Add(ex.Message);
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }

        /// <summary>
        /// Delete a Backup
        /// </summary>
        /// <remarks>
        /// When user want to delete a Backup, use this API for getting<br/>
        /// </remarks>
        /// <param name="id">Backup identity</param>
        /// <returns></returns>
        [Route("{id}")]
        [SwaggerResponse(200, "Returns the result of delete a Backup", typeof(ApiJsonResult))]
        [SwaggerResponse(500, "Internal Server Error")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(401, "Not Authorizated")]
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            var res = new ApiJsonResult();

            try
            {
                var backupDIR = HttpContext.Current.Server.MapPath($"~/{dirBackup}");
                if (!Directory.Exists(backupDIR))
                    Directory.CreateDirectory(backupDIR);
                var filePath = $"{backupDIR}\\{id}.bak";

                // file not found
                if (!File.Exists(filePath))
                {
                    res.ErrorMessages.Add("Backup.NotFound");
                    return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
                }

                File.Delete(filePath);
                return new HttpApiActionResult(HttpStatusCode.OK, res);
            }
            catch (Exception)
            {
                res.ErrorMessages.Add("Can not delete Backup.");
                return new HttpApiActionResult(HttpStatusCode.BadRequest, res);
            }
        }
    }
}
