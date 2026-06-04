using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Common
{
    public sealed class CommonEnums
    {
        public enum DataProviderType
        {
            EFCore,
            Dapper,
            AdoNet
        }
    }
}
