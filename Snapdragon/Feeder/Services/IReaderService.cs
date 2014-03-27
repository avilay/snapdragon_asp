using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feeder.Models;

namespace Feeder.Services
{
    interface IReaderService
    {
        List<ItemUserDetails> GetSessionItemsByFeed(int feedId);
        List<ItemUserDetails> GetSessionInterestingItems();
        void MarkItemsAsRead(List<int> ids);
        void MarkItemAsClicked(int itemId);
    }
}
