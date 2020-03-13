using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Domain.ViewModel
{
    /// <summary>
    /// Use this class to model a response for register input validation
    /// </summary>
    public class RegisterValidationResult
    {
        /// <summary>
        /// A property which describe if the result is valid or not
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// A property which contains the message (optional)
        /// </summary>
        public string Message { get; set; }
    }
}
