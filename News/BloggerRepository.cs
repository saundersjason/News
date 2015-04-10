using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace SavannahState.News
{
    public class BloggerRepository:IStoryRepository
    {
        private String _bloggerKey;
        private const String _queryURL = "https://www.googleapis.com/blogger/v3/blogs/5797214299173121016/";
        private const String _queryParameters = "fields=items(id,published,title,labels)";
        private const String _queryParametersAll = "fields=items(id,published,title,content,labels)";

        private String _feedURL = "";
        List<Article> _allArticles;
        public BloggerRepository(String key) {
            _bloggerKey = key;
            _allArticles = new List<Article>();
        }

        public List<Article> GetArticles( Boolean onlyActive,Int32 numberOfArticles=0)
        {
            ProcessFeed(GetFeed(_queryURL + "posts?key=" + _bloggerKey + "&" + _queryParameters));
            if (numberOfArticles > 0)
            {
                return _allArticles.Take(numberOfArticles).ToList();
            } else {
                return _allArticles;
            }
        }

        public Article GetArticleById(String Id) {
            ProcessFeed(GetFeed(_queryURL + "posts/" + Id + "?key=" + _bloggerKey));
            if (_allArticles.Count > 0)
            {
                return _allArticles[0];
            }
            else { 
                return new Article();
            }
        }

        public List<Article> Search(string keyword, Boolean onlyActive, Int32 numberOfArticles=0)
        {
            //URL Encode keyword
            ProcessFeed(GetFeed(_queryURL + "posts/search?q=" + keyword + "&key=" + _bloggerKey + "&" + _queryParameters));
            if (numberOfArticles > 0)
            {
                return _allArticles.Take(numberOfArticles).ToList();
            }
            else
            {
                return _allArticles;
            }
        }

        public List<Article> SearchByType(string typeName, bool onlyActive, Int32 numberOfArticles=0)
        {
            //URL Encode keyword            
            ProcessFeed(GetFeed(_queryURL + "posts/search?q=label:" + typeName + "&key=" + _bloggerKey + "&" + _queryParameters));
            if (numberOfArticles > 0)
            {
                return _allArticles.Take(numberOfArticles).ToList();
            }
            else
            {
                return _allArticles;
            }
        }

        private String GetFeed(String feedUrl){
            String feed = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(feedUrl);
            
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    feed = reader.ReadToEnd();
                    _feedURL = feedUrl;
                }
            }
            catch (WebException ex)
            {
                feed = "";
            }
            return feed;
        }

        private void ProcessFeed(String feed)
        {
            String nextToken = "";
            if (!String.IsNullOrEmpty(feed)) {
                BloggerPosts bloggerPosts = new JavaScriptSerializer().Deserialize<BloggerPosts>(feed);
                if (bloggerPosts.items != null)
                {
                    if (bloggerPosts.items.Count() > 0)
                    {
                        if (!String.IsNullOrEmpty(bloggerPosts.nextPageToken))
                        {
                            nextToken = bloggerPosts.nextPageToken;
                        }
                        else
                        {
                            nextToken = "";
                        }
                        foreach (BloggerPost post in bloggerPosts.items)
                        {
                            try
                            {
                                Article newArticle = new Article();
                                newArticle.ArticleType = "blogger";
                                newArticle.Body = post.content;
                                newArticle.DatePublished = Convert.ToDateTime(post.published); ;
                                newArticle.Id = post.id;
                                newArticle.Tags = post.labels.ToList();
                                newArticle.Title = post.title;
                                _allArticles.Add(newArticle);
                            }
                            catch
                            {
                                //dont add
                            }
                        }
                    }
                } else {
                    BloggerPost bloggerPost = new JavaScriptSerializer().Deserialize<BloggerPost>(feed);
                    try
                    {
                        Article newArticle = new Article();
                        newArticle.ArticleType = "blogger";
                        newArticle.Body = bloggerPost.content;
                        newArticle.DatePublished = Convert.ToDateTime(bloggerPost.published); ;
                        newArticle.Id = bloggerPost.id;
                        newArticle.Tags = bloggerPost.labels.ToList();
                        newArticle.Title = bloggerPost.title;
                        _allArticles.Add(newArticle);
                    }
                    catch
                    {
                        //dont add
                    }
                }
            }





















            //To DO - nextToken is returnig the same value on mutilep request. Do search for SSU



            if (!String.IsNullOrEmpty(nextToken) && !String.IsNullOrEmpty(_feedURL))
            {
                ProcessFeed(GetFeed(_feedURL + "&pageToken=" + nextToken));
            }
        }

        public class BloggerPosts
        {
            public String nextPageToken { get; set; }
            public List<BloggerPost> items { get; set; }
        }

        public class BloggerPost
        {
            public String id { get; set; }
            public String published { get; set; }
            public DateTime DatePublished { get; set; }
            public String title { get; set; }
            public String content { get; set; }
            public String[] labels { get; set; }
        }

    }
}
