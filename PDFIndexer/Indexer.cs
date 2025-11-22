using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using PDFIndexer.Journal;
using static Lucene.Net.Documents.Field;
using static Lucene.Net.Util.Packed.PackedInt32s;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Lucene.Net.Analysis.Cjk;

public delegate void ReadyEventHandler();

namespace PDFIndexer
{
    public class Indexer : IDisposable
    {
        private bool _IsDisposed;

        private LuceneDirectory indexDirectory;

        private const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        private string indexPath;

        private Analyzer analyzer;
        private IndexReader reader;
        private IndexSearcher searcher;

        private bool _ready = false;
        public bool ready { get { return _ready; } }

        /// <summary>
        /// Indexer가 준비될때 발생합니다. 이벤트가 Main Thread에서 발생하지 않을 수도 있습니다.
        /// </summary>
        public event ReadyEventHandler OnReady;

        public Indexer(string indexPath)
        {
            this.indexPath = indexPath;
            Initalize();
        }

        public void Initalize()
        {
            Logger.Write(JournalLevel.Info, "Indexer 초기화");

            indexDirectory = FSDirectory.Open(Path.Combine(indexPath, ".index"));

            analyzer = new CJKAnalyzer(luceneVersion);

            try
            {
                searcher = GetIndexSearcher();
                SetReadyState(true);

                Logger.Write("기존 인덱스 찾음");
            }
            catch (DirectoryNotFoundException)
            {
                // 인덱스 없음
                searcher = null;

                Logger.Write(JournalLevel.Warning, "디스크에 저장된 인덱스 없음");
            }

            Logger.Write(JournalLevel.Info, "Indexer 초기화 --> 완료");
        }

        #region Finalize 관련
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_IsDisposed) return;

            if (disposing)
            {
                if (indexDirectory != null) indexDirectory.Dispose();
                if (analyzer != null) analyzer.Dispose();
                if (reader != null) reader.Dispose();
            }

            _IsDisposed = true;
        }

        ~Indexer()
        {
            Dispose(false);
        }
        #endregion Finalize 관련

        #region 이벤트 관련
        private void SetReadyState(bool ready)
        {
            _ready = ready;

            if (ready) Ready();
        }

        protected virtual void Ready()
        {
            Logger.Write(JournalLevel.Info, "Indexer Ready");
            OnReady?.Invoke();
        }
        #endregion 이벤트 관련

        #region 유틸리티 메서드
        private static string GetHashFromFile(string path)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.md5?view=net-9.0
            var md5 = MD5.Create();
            using (var stream = File.OpenRead(path))
            {
                var hash = md5.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return hashString;
            }
        }

        private static long GetLastModifiedFromFile(string path)
        {
            return ((DateTimeOffset)File.GetLastWriteTimeUtc(path)).ToUnixTimeSeconds();
        }
        #endregion 유틸리티 메서드

        private IndexSearcher GetIndexSearcher()
        {
            reader = DirectoryReader.Open(indexDirectory);
            return new IndexSearcher(reader);
        }

        private IndexWriter GetIndexWriter()
        {
            IndexWriterConfig indexConfig = new IndexWriterConfig(luceneVersion, analyzer);
            indexConfig.OpenMode = OpenMode.CREATE;
            IndexWriter writer = new IndexWriter(indexDirectory, indexConfig);

            return writer;
        }

        /// <summary>
        /// 주어진 PDF 파일들을 인덱싱합니다.
        /// </summary>
        /// <param name="pdfs">인덱싱할 pdf들의 경로입니다</param>
        public void IndexPdfs(string[] pdfs)
        {
            Logger.Write(JournalLevel.Info, $"IndexPdfs 요청 - 요청된 PDF 수:{pdfs.Length}");

            var writer = GetIndexWriter();

            foreach (var path in pdfs)
            {
                var filename = (path.Split('\\').LastOrDefault() ?? path).Replace(".pdf", "");

                var hash = GetHashFromFile(path);
                var lastModified = GetLastModifiedFromFile(path).ToString();

                using (PdfDocument pdf = PdfDocument.Open(path))
                {
                    // 페이지 별로 인덱스
                    var pages = pdf.GetPages();
                    foreach (var page in pages)
                    {
                        // 기본 메타데이터
                        Document doc = new Document
                        {
                            new TextField("title", filename, Field.Store.YES),
                            new StringField("path", path, Field.Store.YES),
                            new Int32Field("page", page.Number, Field.Store.YES),
                            new StringField("md5", hash, Field.Store.YES),
                            new StringField("lastModified", lastModified, Field.Store.YES),
                        };

                        // 페이지 내용 (텍스트만)
                        string content = ContentOrderTextExtractor.GetText(page);
                        doc.Add(new TextField("content", content, Field.Store.YES));

                        // TODO: Cron task에 이미지 OCR

                                                writer.AddDocument(doc);
                    }

                    // Logger.Write($"IndexPdfs - Index done: {path} with {pages.Count()} pages");
                }
            }

            Logger.Write(JournalLevel.Info, "IndexPdfs 정리 중..");
            writer.Commit();
            writer.Dispose();

            Logger.Write(JournalLevel.Info, "IndexPdfs 완료.");

            Initalize();
        }

        /// <summary>주어진 쿼리로 검색</summary>
        /// <param name="text">검색 쿼리. <c>IsNullOrWhitespace</c>가 아닌 <c>string</c>이어야 함</param>
        /// <param name="max">최대 반환 결과. 검색 자체는 최대한 하지만, 일치도가 높은 상위 최대 <c>max</c>개를 반환합니다.</param>
        /// <returns>Nullable&lt;TopDocs&gt;를 반환</returns>
        public TopDocs SearchQuery(string text, int max = 10)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            QueryParser parser = new QueryParser(luceneVersion, "content", analyzer);
            Query query = parser.Parse(text);
            TopDocs topDocs = searcher.Search(query, n: max);

            return topDocs;
        }

        public Document SearchDocument(int doc)
        {
            return searcher.Doc(doc);
        }

        public static DocumentGroup[] GroupDocuments(Indexer indexer, ScoreDoc[] scoreDocs)
        {
            // 경로를 기준으로 그룹
            Dictionary<string, DocumentGroup> result = new Dictionary<string, DocumentGroup>();

            foreach (var scoreDoc in scoreDocs)
            {
                Document doc = indexer.SearchDocument(scoreDoc.Doc);
                string path = doc.Get("path");
                int page = int.Parse(doc.Get("page"));

                if (result.ContainsKey(path))
                {
                    if (result.TryGetValue(path, out DocumentGroup group))
                    {
                        result.Remove(path);
                        group.Add(page, scoreDoc);
                        result.Add(path, group);
                    }
                }
                else
                {
                    string title = doc.Get("title");

                    var group = new DocumentGroup(title, path);
                    group.Add(page, scoreDoc);

                    result.Add(path, group);
                }
            }

            var resultArray = result.Values.ToArray();
            Array.Sort(resultArray, (a, b) => b.TotalScore.CompareTo(a.TotalScore));

            return resultArray;
        }

        private static bool IsMissingPdf(Indexer indexer, string pdf, bool strict = false)
        {
            string queryRaw = pdf;
            if (strict) queryRaw = GetHashFromFile(pdf);

            Term term;
            if (strict)
                term = new Term("md5", queryRaw);
            else
                term = new Term("path", queryRaw);

            Query query = new TermQuery(term);
            TopDocs topDocs = indexer.searcher.Search(query, n: 1);

            // 결과 없음 == 인덱싱 안됨
            //TopDocs topDocs = indexer.;
            if (topDocs.ScoreDocs.Length == 0) return true;

            // 마지막 수정 시간이 다르면 인덱스 업데이트 --> 없다고 간주
            Document document = indexer.SearchDocument(topDocs.ScoreDocs[0].Doc);
            long lastModifiedFromIndex = Int64.Parse(document.Get("lastModified"));
            long lastModifiedFromFile = GetLastModifiedFromFile(pdf);
            if (lastModifiedFromIndex != lastModifiedFromFile) return true;

            return false;
        }

        /// <summary>
        /// 인덱싱되어있지 않은 PDF를 찾습니다.
        /// </summary>
        /// <param name="indexer">인덱서</param>
        /// <param name="pdfs">검사할 PDF들의 Path를 담은 array</param>
        /// <param name="strict">(Optional) 엄격 검사 모드. 경로가 아닌 Hash로 검사합니다.</param>
        /// <returns>
        /// 인덱싱되어있지 않은 PDF의 Path를 반환합니다.
        /// </returns>
        public static string[] GetMissingPdfs(Indexer indexer, string[] pdfs, bool strict = false)
        {
            var missing = new List<string>();

            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 5,
            };

            Parallel.ForEach(pdfs, parallelOptions, (pdf, _) =>
            {
                if (IsMissingPdf(indexer, pdf, strict)) missing.Add(pdf);
            });

            return missing.ToArray();
        }

        public static void CleanupIndexes(Indexer indexer)
        {
            // 인덱스를 업데이트하기 위한 writer 가져오기
            var writer = indexer.GetIndexWriter();

            // 업데이트가 필요한 문서
            List<string> toUpdate = new List<string>();

            // 인덱스를 구성하는 모든 리프 순회
            foreach (var leafContext in indexer.reader.Leaves)
            {
                var reader = leafContext.Reader;

                // 삭제되지 않은 문서 정보 가져오기
                var liveDocs = MultiFields.GetLiveDocs(reader);

                // 모든 문서 순회
                for (int id = 0; id < reader.MaxDoc; id++)
                {
                    // 삭제된 문서 스킵
                    if (liveDocs != null && !liveDocs.Get(id)) continue;

                    Document document = reader.Document(id);

                    string path = document.Get("path");
                    if (!File.Exists(path))
                    {
                        writer.DeleteDocuments(new Term("path", path));
                        continue;
                    }

                    // 해시 구하는 건 CPU 시간을 많이 사용하니, cron task에서는 미사용.
                    // hash로 비교는 toIndexingFiles < indexedFiles 일 때만 사용.
                    //
                    // 해당 path에는 존재하는데 hash가 다르면
                    // 파일이 업데이트/변경된거 --> 새로 인덱싱 해야함
                    // string indexedHash = document.Get("hash");
                    // string realHash = GetHashFromFile(path);

                    // 인덱스의 last modified와 실제 last modified의 시간이 다르면
                    // 파일이 변경되었으므로, 새로 인덱싱 해야 함
                    long lastModifiedFromIndex = Int64.Parse(document.Get("lastModified"));
                    long lastModifiedFromFile = GetLastModifiedFromFile(path);
                    if (lastModifiedFromIndex != lastModifiedFromFile)
                    {
                        toUpdate.Add(path);
                    }
                }
            }

            // 삭제 완료 --> 인덱스 저장
            writer.Commit();

            // 업데이트가 필요한 문서들 업데이트
            indexer.IndexPdfs(toUpdate.ToArray());
        }

        public static string[] GetDuplicateFiles(Indexer indexer)
        {
            HashSet<string> hashes = new HashSet<string>();
            HashSet<string> duplicates = new HashSet<string>();

            // 인덱스를 구성하는 모든 리프 순회
            foreach (var leafContext in indexer.reader.Leaves)
            {
                var reader = leafContext.Reader;

                // 삭제되지 않은 문서 정보 가져오기
                var liveDocs = MultiFields.GetLiveDocs(reader);

                // 모든 문서 순회
                for (int id = 0; id < reader.MaxDoc; id++)
                {
                    // 삭제된 문서 스킵
                    if (liveDocs != null && !liveDocs.Get(id)) continue;

                    Document document = reader.Document(id);

                    string hash = document.Get("md5");
                    if (hashes.Contains(hash))
                    {
                        duplicates.Add(hash);
                    } else
                    {
                        hashes.Add(hash);
                    }
                }
            }

            return duplicates.ToArray();
        }

        public static SearchItem[] GetFilesFromHash(Indexer indexer, string hash)
        {
            List<SearchItem> result = new List<SearchItem>();

            Term term = new Term("md5", hash);
            Query query = new TermQuery(term);
            TopDocs topDocs = indexer.searcher.Search(query, int.MaxValue);

            for (int i = 0; i < topDocs.ScoreDocs.Length; i++)
            {
                var doc = indexer.SearchDocument(topDocs.ScoreDocs[i].Doc);
                result.Add(new SearchItem(doc));
            }

            return result.ToArray();
        }
    }
}
