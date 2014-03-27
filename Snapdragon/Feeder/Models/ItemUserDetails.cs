using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Feeder.Models
{
    public class ItemUserDetails
    {
        public string Author { get; set; }

        public string Description { get; set; } 
            
        public string Excerpt { get; set; }

        public int FeedId { get; set; }

        public string FeedTitle { get; set; }

        public int ItemId { get; set; }

        public string Link { get; set; }

        public DateTime PubDate { get; set; }

        public string Title { get; set; }

        public bool IsClicked { get; set; }

        public bool IsRead { get; set; }        

        public double Score { get; set; }
    }
}
