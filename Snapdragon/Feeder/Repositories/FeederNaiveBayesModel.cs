using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Feeder.Models;
using System.Web.Security;
using Avilay.Utils.Logging;
using Avilay.TextMining;
using Avilay.TextMining.Bayes;
using System.Configuration;

namespace Feeder.Repositories
{
    public class FeederNaiveBayesModel : IFeederNaiveBayesModel
    {
        private Guid _userId;

        private Dictionary<string, UserVocabulary> _userVocabulary;
        private Dictionary<string, Probability> _interestingProbs;
        private Dictionary<string, Probability> _uninterestingProbs;
        private Prior _interestingPrior;
        private Prior _uninterestingPrior;

        private Dictionary<string, Vocabulary> _vocabulary;
        private string[] _stopwords;               
        private Feeder.Models.Classification _interesting;
        private Feeder.Models.Classification _uninteresting;
        private SnapdragonDataContext _dataContext;

        public FeederNaiveBayesModel() {
            _dataContext = new SnapdragonDataContext();
            Initialize();
        }

        public FeederNaiveBayesModel(SnapdragonDataContext dc) {
            _dataContext = dc;
            Initialize();
        }

        private void Initialize() {
            string interestingLabel = ConfigurationManager.AppSettings["interestingLabel"];
            string uninterestingLabel = ConfigurationManager.AppSettings["uninterestingLabel"];
            var cls = from cl in _dataContext.Classifications
                      where cl.Label == interestingLabel
                      select cl;
            _interesting = cls.First();
            cls = from cl in _dataContext.Classifications
                  where cl.Label == uninterestingLabel
                  select cl;
            _uninteresting = cls.First();

            var vocabs = from v in _dataContext.Vocabularies
                         select v;
            _vocabulary = vocabs.ToDictionary(v => v.Word);

            var swords = from s in _dataContext.Stopwords
                         select s.Word;
            _stopwords = swords.ToArray();
        }

        #region IFeederNaiveBayesModel Members

        public void SetUser(Guid userId) {
            _userId = userId;

            var uservocabs = from uv in _dataContext.UserVocabularies
                             join v in _dataContext.Vocabularies
                             on uv.VocabularyId equals v.Id
                             where uv.UserId == _userId
                             select new { v.Word, uv };
            if (uservocabs.Count() > 0) {
                _userVocabulary = uservocabs.ToDictionary(u => u.Word, u => u.uv);
            }
            else {
                _userVocabulary = new Dictionary<string, UserVocabulary>();
            }

            var probs = from p in _dataContext.Probabilities
                        join uv in _dataContext.UserVocabularies
                        on p.UserVocabularyId equals uv.Id
                        join v in _dataContext.Vocabularies
                        on uv.VocabularyId equals v.Id
                        where uv.UserId == _userId
                        && p.ClassificationId == _interesting.Id
                        select new { v.Word, p };
            if (probs.Count() > 0) {
                _interestingProbs = probs.ToDictionary(pr => pr.Word, pr => pr.p);
            }
            else {
                _interestingProbs = new Dictionary<string, Probability>();
            }
            probs = from p in _dataContext.Probabilities
                    join uv in _dataContext.UserVocabularies
                    on p.UserVocabularyId equals uv.Id
                    join v in _dataContext.Vocabularies
                    on uv.VocabularyId equals v.Id
                    where uv.UserId == _userId
                    && p.ClassificationId == _uninteresting.Id
                    select new { v.Word, p };
            if (probs.Count() > 0) {
                _uninterestingProbs = probs.ToDictionary(pr => pr.Word, pr => pr.p);
            }
            else {
                _uninterestingProbs = new Dictionary<string, Probability>();
            }

            var vprior = from p in _dataContext.Priors
                         where p.Classification.Label == _interesting.Label
                         && p.UserId == _userId
                         select p;
            if (vprior.Count() > 0) {
                _interestingPrior = vprior.First();
            }
            else {
                _interestingPrior = null;
            }
            vprior = from p in _dataContext.Priors
                     where p.Classification.Label == _uninteresting.Label
                     && p.UserId == _userId
                     select p;
            if (vprior.Count() > 0) {
                _uninterestingPrior = vprior.First();
            }
            else {
                _uninterestingPrior = null;
            }
        }

        public Feeder.Models.Classification GetClassification(string label) {
            if (label == _interesting.Label) return _interesting;
            else if (label == _uninteresting.Label) return _uninteresting;
            else throw new ArgumentException("Unknown label: " + label);
        }

        public bool HasModel() {
            var hm = from uh in _dataContext.UserHistories
                     where uh.UserId == _userId
                     select uh.HasModel;

            if (hm.Count() == 0) return false;
            else if (hm.Count() == 1) return hm.First();
            else throw new ArgumentException(string.Format(hm.Count() + " Multiple histories exist for user {0}", _userId));
        }

        public void DeleteModel() {
            var vhistory = from uh in _dataContext.UserHistories
                           where uh.UserId == _userId
                           select uh;
            UserHistory history;
            if (vhistory.Count() == 1) {
                history = vhistory.First();
                if (history.HasModel) {
                    history.HasModel = false;
                    _dataContext.Probabilities.DeleteAllOnSubmit(_interestingProbs.Values);
                    _dataContext.Probabilities.DeleteAllOnSubmit(_uninterestingProbs.Values);
                    _dataContext.UserVocabularies.DeleteAllOnSubmit(_userVocabulary.Values);
                    _dataContext.Priors.DeleteAllOnSubmit(new Prior[] { _interestingPrior, _uninterestingPrior });
                    _dataContext.SubmitChanges();
                }
            }                        
        }

        public void MarkAsHasModel() {
            var vhistory = from uh in _dataContext.UserHistories
                           where uh.UserId == _userId
                           select uh;
            UserHistory history;
            if (vhistory.Count() == 0) {
                history = new UserHistory {
                    UserId = _userId,
                    HasModel = true
                };
                _dataContext.UserHistories.InsertOnSubmit(history);
            }
            else if (vhistory.Count() == 1) {
                history = vhistory.First();
                history.HasModel = true;
            }
            else {
                throw new ArgumentException(string.Format(vhistory.Count() + " Multiple histories exist for user {0}", _userId));
            }
            _dataContext.SubmitChanges();
        }

        #endregion

        #region INaiveBayesModel Members

        public void Commit() {
            _dataContext.SubmitChanges();
        }

        public string[] GetClasses() {
            return new string[] { _interesting.Label, _uninteresting.Label };
        }

        public double GetPrior(string classLabel) {
            if (classLabel == _interesting.Label) {
                if (_interestingPrior != null) {
                    return _interestingPrior.Probability;
                }
                else {
                    return 0;
                }
            }
            else if (classLabel == _uninteresting.Label) {
                if (_uninterestingPrior != null) {
                    return _uninterestingPrior.Probability;
                }
                else {
                    return 0;
                }
            }
            else {
                throw new ArgumentException(classLabel + " is unnown classification");
            }
        }

        public double GetProbability(string word, string classLabel) {
            if (classLabel == _interesting.Label) {
                if (_interestingProbs.ContainsKey(word)) {
                    return _interestingProbs[word].Estimate;
                }
                else {
                    return 0;
                }
            }
            else if (classLabel == _uninteresting.Label) {
                if (_uninterestingProbs.ContainsKey(word)) {
                    return _uninterestingProbs[word].Estimate;
                }
                else {
                    return 0;
                }
            }
            else {
                throw new ArgumentException(classLabel + " is unknown classification");
            }
        }

        public string[] GetStopwords() {
            return _stopwords;
        }

        public string[] GetVocabulary() {
            return _userVocabulary.Keys.ToArray();
        }

        public void SetClasses(string[] classes) {
            
        }

        public void SetPrior(string classLabel, double prior) {
            if (classLabel == _interesting.Label) {
                if (_interestingPrior == null) {
                    _interestingPrior = new Prior {
                        ClassificationId = _interesting.Id,
                        UserId = _userId,
                        Probability = prior
                    };
                    _dataContext.Priors.InsertOnSubmit(_interestingPrior);
                }
                else {
                    _interestingPrior.Probability = prior;
                }
                if (prior == 1) {
                    _dataContext.Probabilities.DeleteAllOnSubmit(_uninterestingProbs.Values);
                    _uninterestingProbs = new Dictionary<string, Probability>();
                    SetPrior(_uninteresting.Label, 0);
                }
            }
            else if (classLabel == _uninteresting.Label) {
                if (_uninterestingPrior == null) {
                    _uninterestingPrior = new Prior {
                        ClassificationId = _uninteresting.Id,
                        UserId = _userId,
                        Probability = prior
                    };
                    _dataContext.Priors.InsertOnSubmit(_uninterestingPrior);
                }
                else {
                    _uninterestingPrior.Probability = prior;
                }
                if (prior == 1) {
                    _dataContext.Probabilities.DeleteAllOnSubmit(_interestingProbs.Values);
                    _uninterestingProbs = new Dictionary<string, Probability>();
                    SetPrior(_interesting.Label, 0);
                }
            }
            else {
                throw new ArgumentException(classLabel + " is unnown classification");
            }
        }

        public void SetProbability(string word, string classLabel, double prob) {
            if (!_userVocabulary.ContainsKey(word)) {
                throw new ArgumentException(word + " not part of user's (" + _userId + ") vocabulary");
            }

            if (classLabel == _interesting.Label) {
                if (_interestingProbs.ContainsKey(word)) {
                    _interestingProbs[word].Estimate = prob;
                }
                else {
                    Probability pr = new Probability { 
                        ClassificationId = _interesting.Id, 
                        UserVocabularyId = _userVocabulary[word].Id, 
                        Estimate = prob 
                    };
                    _dataContext.Probabilities.InsertOnSubmit(pr);
                    _interestingProbs.Add(word, pr);
                }
            }
            else if (classLabel == _uninteresting.Label) {
                if (_uninterestingProbs.ContainsKey(word)) {
                    _uninterestingProbs[word].Estimate = prob;
                }
                else {
                    Probability pr = new Probability {
                        ClassificationId = _uninteresting.Id,
                        UserVocabularyId = _userVocabulary[word].Id,
                        Estimate = prob
                    };
                    _dataContext.Probabilities.InsertOnSubmit(pr);
                    _uninterestingProbs.Add(word, pr);
                }
            }
            else {
                throw new ArgumentException(classLabel + " is unnown classification");
            }
        }

        public void SetVocabulary(string[] vocabulary) {
            //add any new words in this vocabulary that are not there in the system-wide vocabulary
            foreach (string word in vocabulary) {
                if (!_vocabulary.ContainsKey(word)) {
                    Vocabulary v = new Vocabulary { Word = word };
                    _vocabulary.Add(word, v);
                    _dataContext.Vocabularies.InsertOnSubmit(v);
                }
            }
            _dataContext.SubmitChanges();

            //add any new words in this vocabulary that are not there in the user vocabulary
            foreach (string word in vocabulary) {
                if (!_userVocabulary.ContainsKey(word)) {
                    UserVocabulary uv = new UserVocabulary { UserId = _userId, VocabularyId = _vocabulary[word].Id };
                    _userVocabulary.Add(word, uv);
                    _dataContext.UserVocabularies.InsertOnSubmit(uv);
                }
            }

            //delete any old words that are there in the user vocabulary but are not there in this vocabulary
            string[] tbdWords = _userVocabulary.Keys.Except(vocabulary).ToArray();
            foreach (string tbdWord in tbdWords) {
                Probability prInteresting = _interestingProbs[tbdWord];
                _interestingProbs.Remove(tbdWord);                
                Probability prUninteresting = _uninterestingProbs[tbdWord];
                _uninterestingProbs.Remove(tbdWord);
                _dataContext.Probabilities.DeleteAllOnSubmit(new Probability[] { prInteresting, prUninteresting });

                UserVocabulary tbd = _userVocabulary[tbdWord];
                _userVocabulary.Remove(tbdWord);
                _dataContext.UserVocabularies.DeleteOnSubmit(tbd);
            }

            _dataContext.SubmitChanges();
        }

        #endregion
    }
}