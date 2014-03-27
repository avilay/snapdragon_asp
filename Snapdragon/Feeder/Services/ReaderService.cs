using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Feeder.Repositories;
using Feeder.Models;
using Avilay.Utils.Logging;
using System.Threading;
using System.Configuration;

namespace Feeder.Services
{
    public class ReaderService : IReaderService
    {
        // TODO: The following three values must come for a config file
        private static int cutoffPeriod = Int32.Parse(ConfigurationManager.AppSettings["daysForWhichToShowItems"]);
        private static int readItemsCutoffHours = Int32.Parse(ConfigurationManager.AppSettings["hoursForWhichToShowReadItems"]);

        private Guid _userId;
        private DateTime _cutOff = DateTime.Now.AddDays(-1 * cutoffPeriod);
        private DateTime _readItemsCutoff = DateTime.Now.AddHours(-1 * readItemsCutoffHours);
        private IItemRepository _itemRepo;
        //private DateTime _sessionStart;

        public ReaderService() {
            if( Membership.GetUser() != null ) {
                _userId = (Guid)Membership.GetUser().ProviderUserKey;
            }
            _itemRepo = new ItemRepository();
            //_sessionStart = DateTime.Now;
        }

        public List<ItemUserDetails> GetSessionItemsByFeed(int feedId) {
            //get unread items
            List<ItemUserDetails> items = _itemRepo.GetUnreadItemUserDetails(_userId, feedId, _cutOff).ToList();

            //get items read in the last x hours
            items.AddRange(_itemRepo.GetReadItemUserDetails(_userId, feedId, _readItemsCutoff).ToList());

            return items;
        }

        public List<ItemUserDetails> GetSessionInterestingItems() {
            //get unread items
            List<ItemUserDetails> items = _itemRepo.GetUnreadInterestingItemUserDetails(_userId, _cutOff).ToList();
            
            //get items read in the last x hours
            items.AddRange(_itemRepo.GetReadInterestingItemUserDetails(_userId, _readItemsCutoff).ToList());
            return items;
        }

        public void MarkItemsAsRead(List<int> ids) {
            _itemRepo.MarkItemsAsRead(_userId, ids);
        }

        public void MarkItemAsClicked(int itemId) {
            _itemRepo.MarkItemAsClicked(_userId, itemId);
        }

    }
}
