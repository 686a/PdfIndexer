using Lucene.Net.Documents;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFIndexer
{
    public class DocumentGroup
    {
        public string _Title;
        public string Title
        {
            get { return _Title; }
        }

        public string _Path;
        public string Path
        {
            get { return _Path; }
        }

        private float IndexerScore;
        private int _MatchPages;
        public int MatchPages
        {
            get { return _MatchPages; }
        }
        public float TotalScore
        {
            get { return IndexerScore + MatchPages * 100; }
        }

        // Dictionary<Page, Score>
        public Dictionary<int, float> Documents;

        public DocumentGroup(string title, string path)
        {
            _Title = title;
            _Path = path;
            IndexerScore = 0;
            _MatchPages = 0;
            Documents = new Dictionary<int, float>();
        }

        public void Add(int page, ScoreDoc scoreDoc)
        {
            Documents.Add(page, scoreDoc.Score);
            IndexerScore += scoreDoc.Score;
            _MatchPages++;
        }
    }
}
