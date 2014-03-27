use Snapdragon
select Word, Label, Estimate 
from Probability P, Classification C, UserVocabulary UV, Vocabulary V
where P.UserVocabularyId = UV.Id
and P.ClassificationId = C.Id
and UV.VocabularyId = V.Id
and UV.UserId = '3FA2E575-B064-41EF-9FC7-0D0F7FF9E59A'
