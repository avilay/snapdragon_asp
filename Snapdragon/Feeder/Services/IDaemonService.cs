using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feeder.Models;

namespace Feeder.Services
{
    public interface IDaemonService
    {
        void AsyncCrawlAndClassify();
        void AsyncLearn();
        bool IsValid(string key);
        void Crawl(Feed[] feeds);
        void Classify(Guid[] userIds);
        void Learn(object userIdObjects);

    }
}
