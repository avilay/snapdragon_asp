using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feeder.Models
{
    public static class ModelExtensions
    {
        public static bool IsSame(this Item item, Item that) {
            if( item.Link == that.Link &&
                item.Title == that.Title ) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
