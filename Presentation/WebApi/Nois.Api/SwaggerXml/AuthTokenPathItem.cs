using Nois.WebApi.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swashbuckle.Swagger;

namespace Nois.Api.SwaggerXml
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthTokenPathItem : IPathItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetApiKey()
        {
            return "/token";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PathItem GetPathItem()
        {
            return new PathItem
            {
                post = new Operation
                {
                    responses = new Dictionary<string, Response>()
                    {
                        { "400",new Response()
                            {
                                description = "Bad Request, If someone login before, server will return 1 code and it will be used for kick previous user in next API call"
                            }
                        },
                        { "401",new Response()
                            {
                                description = "Invalid credential"
                            }
                        },
                        { "500",new Response()
                            {
                                description = "Internal Server Error"
                            }
                        },
                        { "200",new Response
                            {
                                description = "Success, return information of user session",
                                schema = new Schema()
                                {
                                    example = new LoginResult
                                            {
                                                access_token = "wim6jmO_jqYLfDA8MokMJRNNXSfLkyw94fThDNevlXKJYXtJwfytdXnHzIEipuHJEkUrlcnQWpvc7xUUTFRONb8SZjpWsHckspTmhGiUzjqnbb5EGWErFMJTBby4wERQj8lHi3Dr2GehurdF4sjoCkvx_qAYrjs0dSZvBDTdizDpB42m_5WjR-oC4_TKsA0Mz8d5KxDjj7yxrN82lcMNbQHz-DBo29lxvbshf5rP656B5K4ZywV1kPcmFKrnH8wofzLsh2yBpoQ0RUU_8fFUxBnnTO9xbo6ZAE-LsleyPaHrdhS04QuL4pfopAjU9mbJ",
                                                refresh_token = "c545d692961140bdaf5794a71a150fc1",
                                                expires = "Mon, 05 Sep 2017 08:41:05 GMT",
                                                client_id = "a868cec2-5c40-4a90-b473-672f097e1695",
                                                expires_in = 1799,
                                                token_type = "bearer",
                                                userId="A33DF655-CDCA-4F15-904E-0033C19111E0",
                                                username="admin@yourApp.com"

                                            },



                            } }
                        }
                    },

                    summary = "Login",
                    tags = new List<string> { "Account" },
                    description = " For native device, add \"<span style = 'font-weight: bold;' > Authorization: Basic YTg2OGNlYzItNWM0MC00YTkwLWI0NzMtNjcyZjA5N2UxNjk1OjU4YmExMzFiLTAwOWUtNGU3Ny05OGExLTlmNTcwYmQxZGQ4OQ== </span > \" into Header of request.<br />"
                                + "For browser, add \"<span style='font-weight: bold;'>client_id</span>\" and \"<span style='font-weight: bold;'>client_secret</span>\" into body of request. <br />"
                                + "<span style='font-weight: bold;'>client_secret</span>: 58ba131b-009e-4e77-98a1-9f570bd1dd89  <br />"
                                + "<span style='font-weight: bold;'>client_id</span>: a868cec2-5c40-4a90-b473-672f097e1695 <br />",
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter> {
                    new Parameter
                    {
                        type = "string",
                        name = "grant_type",
                        description = "alway be 'password'",
                        required = true,
                        @in = "formData",
                        @default = "password"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "username",
                        required = true,
                        description = "email address",
                        @in = "formData",
                        @default = "admin@nois.com"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "password",
                        description = "password",
                        required = true,
                        @in = "formData",
                        @default = "123456"
                    }
                }
                }
            };
        }
    }
    /// <summary>
    /// Login result
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// access token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double expires_in { get; set; }

        /// <summary>
        /// as:client_id
        /// </summary>
        public string client_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// .issued
        /// </summary>
        public string issued { get; set; }

        /// <summary>
        /// expires
        /// </summary>
        public string expires { get; set; }


    }
}