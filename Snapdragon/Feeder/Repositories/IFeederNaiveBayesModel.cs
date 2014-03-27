using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avilay.TextMining.Bayes;
using Feeder.Models;

namespace Feeder.Repositories
{
    public interface IFeederNaiveBayesModel : INaiveBayesModel
    {
        void SetUser(Guid userId);
        Classification GetClassification(string label);
        bool HasModel();
        void MarkAsHasModel();
        void DeleteModel();
    }
}
