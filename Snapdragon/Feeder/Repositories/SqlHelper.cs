using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feeder.Repositories
{
    public static class SqlHelper
    {
        public static DateTime GetSqlMinDateTime() {
            return new DateTime(1753, 1, 1);
        }
    }
}
