using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace employeeservice.Common
{
    /// <summary>
    /// Referred : https://andrewlock.net/adding-default-security-headers-in-asp-net-core/
    /// </summary>
    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy _policy = new SecurityHeadersPolicy();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public SecurityHeadersBuilder AddCustomHeader(string header, string value)
        {
            _policy.SetHeaders[header] = value;
            return this;
        }
        /// <summary>
        /// This method is used to remove header.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            _policy.RemoveHeaders.Add(header);
            return this;
        }
        /// <summary>
        /// used to build security policy.
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }

}