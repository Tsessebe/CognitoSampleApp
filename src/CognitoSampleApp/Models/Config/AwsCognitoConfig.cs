using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitoSampleApp.Models.Config
{
    public class AwsCognitoConfig
    {
        public string Region { get; set; }

        public string UserPoolClientId { get; set; }

        public string UserPoolClientSecret { get; set; }
        
        public string UserPoolId { get; set; }

    }
}
