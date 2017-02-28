using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reinforced.Typings.Samples.Difficult.CodeGenerators.Models
{
    /// <summary>
    /// Sample model that we will use for tests
    /// </summary>
    public class SampleResponseModel
    {
        /// <summary>
        /// String property - message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Boolean flag
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// String containing date
        /// </summary>
        public string CurrentTime { get; set; }
    }


    
}