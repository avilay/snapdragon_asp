using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feeder.Models;

namespace Feeder.Repositories
{
    public interface IItemRepository
    {
        void Add(Item item, int feedId);
        void AddItemsForUser(Guid userId, int feedId, DateTime insertedAfter);

        IQueryable<Item> GetClickedItems(Guid userId, DateTime insertedAfter);

        IQueryable<ItemUserDetails> GetReadItemUserDetails(Guid userId, int feedId, DateTime readAfter);
        IQueryable<Item> GetReadUnclickedItems(Guid userId, DateTime insertedAfter);
        IQueryable<ItemUserDetails> GetReadInterestingItemUserDetails(Guid userId, DateTime readAfter);

        IQueryable<Item> GetUnreadItems(Guid userId, DateTime insertedAfter);
        IQueryable<ItemUserDetails> GetUnreadItemUserDetails(Guid userId, int feedId, DateTime insertedAfter);
        IQueryable<ItemUserDetails> GetUnreadInterestingItemUserDetails(Guid userId, DateTime insertedAfter);

        void SetClassification(Guid userId, Dictionary<int, Avilay.TextMining.Classification> predictions);
        void MarkItemsAsRead(Guid userId, List<int> ids);
        void MarkItemAsClicked(Guid userId, int itemId);
    }
}
