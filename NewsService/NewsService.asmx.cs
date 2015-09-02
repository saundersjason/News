using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SavannahState.News;
namespace SavannahState
{
    /// <summary>
    /// Summary description for NewsService
    /// </summary>
    [WebService(Namespace = "http://www.savannahstate.edu", Description = "Return news articles from various sources.")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class NewsService : System.Web.Services.WebService
    {
        private const Int32 _maxArticles = 50;
        public NewsService()
        {}

        [WebMethod(CacheDuration = 1800)]//30 minutes
        [ScriptMethod(UseHttpGet = true)]
        public List<Article> GetAllArticles(Boolean onlyActive, Int32 numberOfArticles)
        {
            if(numberOfArticles==0){
                numberOfArticles = _maxArticles;
            }

            List<Article> allArticles = new List<Article>();
            List<Article> articles = new List<Article>();
            List<Article> blogPosts = new List<Article>();

            IStoryRepository newsRepo = new NewsRepository();
            IStoryRepository blogRepo = new BloggerRepository(System.Configuration.ConfigurationManager.AppSettings["blogger_access_token"]);

            articles = newsRepo.GetArticles(onlyActive, numberOfArticles);
            blogPosts = blogRepo.GetArticles(onlyActive, numberOfArticles);

            allArticles = articles.Concat(blogPosts).OrderByDescending(a => a.DatePublished).Take(numberOfArticles).ToList();

            Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            Context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            return allArticles;
        }

        [WebMethod(CacheDuration = 1800)]//30 minutes
        [ScriptMethod(UseHttpGet = true)]
        public Article GetArticleById(String id)
        {
            Article article = new Article();
            if (id.Length > 6)
            {
                IStoryRepository blogRepo = new BloggerRepository(System.Configuration.ConfigurationManager.AppSettings["blogger_access_token"]);
                article = blogRepo.GetArticleById(id);
            }
            else
            {
                IStoryRepository newsRepo = new NewsRepository();
                article = newsRepo.GetArticleById(id);
            }

            Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            Context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            return article;
        }

        [WebMethod(CacheDuration = 900)]//15 minutes
        [ScriptMethod(UseHttpGet = true)]
        public List<Article> Search(String keyword, Boolean onlyActive, Int32 numberOfArticles)
        {
            keyword = HttpUtility.UrlDecode(keyword);

            if (numberOfArticles == 0)
            {
                numberOfArticles = _maxArticles;
            }
            List<Article> allArticles = new List<Article>();
            List<Article> articles = new List<Article>();
            List<Article> blogPosts = new List<Article>();

            IStoryRepository newsRepo = new NewsRepository();
            IStoryRepository blogRepo = new BloggerRepository(System.Configuration.ConfigurationManager.AppSettings["blogger_access_token"]);

            articles = newsRepo.Search(keyword, onlyActive, numberOfArticles);
            blogPosts = blogRepo.Search(keyword, onlyActive, numberOfArticles);

            allArticles = articles.Concat(blogPosts).OrderByDescending(a => a.DatePublished).Take(numberOfArticles).ToList();

            Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            Context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            return allArticles;
        }

        [WebMethod(CacheDuration = 900)]//15 minutes
        [ScriptMethod(UseHttpGet = true)]
        public List<Article> SearchByType(String type, Boolean onlyActive, Int32 numberOfArticles)
        {
            if (numberOfArticles == 0)
            {
                numberOfArticles = _maxArticles;
            }
            List<Article> allArticles = new List<Article>();
            List<Article> articles = new List<Article>();
            List<Article> blogPosts = new List<Article>();

            IStoryRepository newsRepo = new NewsRepository();
            IStoryRepository blogRepo = new BloggerRepository(System.Configuration.ConfigurationManager.AppSettings["blogger_access_token"]);

            articles = newsRepo.SearchByType(type, onlyActive, numberOfArticles);
            blogPosts = blogRepo.SearchByType(type, onlyActive, numberOfArticles);

            allArticles = articles.Concat(blogPosts).OrderByDescending(a => a.DatePublished).Take(numberOfArticles).ToList();

            Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            Context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            return allArticles;
        }
    }
}
