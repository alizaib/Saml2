﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace Kentor.AuthServices.WebSso
{
    /// <summary>
    /// The results of a command.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Status code that should be returned.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
        
        /// <summary>
        /// Cacheability of the command result.
        /// </summary>
        public Cacheability Cacheability { get; set; }
        
        /// <summary>
        /// Location, if the status code is a redirect.
        /// </summary>
        public Uri Location { get; set; }
        
        /// <summary>
        /// The extracted principal if the command has parsed an incoming assertion.
        /// </summary>
        public ClaimsPrincipal Principal { get; set; }

        /// <summary>
        /// The response body that is the result of the command.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The Mime-type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Data relayed from a previous request, such as the Owin Authenciation
        /// Properties.
        /// </summary>
        public object RelayData { get; set; }

        /// <summary>
        /// Indicates that the local session should be terminated. Used by
        /// logout functionality.
        /// </summary>
        public bool TerminateLocalSession { get; set; }

        /// <summary>
        /// Name of cookie to set.
        /// </summary>
        public string SetCookieName { get; set; }

        /// <summary>
        /// Unencrypted data to set in cookie. Data should be encrypted when
        /// applied to the response.
        /// </summary>
        public string SetCookieData { get; set; }

        /// <summary>
        /// Name of cookie to be cleared.
        /// </summary>
        public string ClearCookieName { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public CommandResult()
        {
            HttpStatusCode = HttpStatusCode.OK;
            Cacheability = Cacheability.NoCache;
        }
    }
}
