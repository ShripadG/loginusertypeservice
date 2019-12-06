using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace employeeservice.Common
{
    /// <summary>
    /// Referred : https://andrewlock.net/adding-default-security-headers-in-asp-net-core/
    /// </summary>
    public class SecurityHeadersPolicy
    {
        /// <summary>
        /// Setting headers.
        /// </summary>
        public IDictionary<string, string> SetHeaders { get; }
         = new Dictionary<string, string>();
        /// <summary>
        /// Removing headers.
        /// </summary>
        public ISet<string> RemoveHeaders { get; }
            = new HashSet<string>();
    }
}
