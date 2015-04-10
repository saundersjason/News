using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SavannahState.News
{
    public class NewsRepository:IStoryRepository
    {
        private List<NewsType> _newsTypes;
        private Boolean _getBodyContent = false;

        public NewsRepository(){
            _newsTypes = GetNewsTypes();
        }

        public List<Article> GetArticles( Boolean onlyActive,Int32 numberOfArticles=0) 
        {
            _getBodyContent = true;
            List<Article> articles = new List<Article>();
            articles = Search("", onlyActive, numberOfArticles);
            return articles;
        }

        public Article GetArticleById(String Id) {
            _getBodyContent = true;
            Article article = new Article();
            String sql = "SELECT TOP 1 ID, TYPE_ID, TIME_START, TIME_END, HEADLINE, BODY, LINK, IMAGE_NAME, OTHER_CATEGORY FROM NEWS WHERE ID = @Id";
            SqlConnection conn = new DBConnection().GetConnection("");
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int)).Value = Convert.ToInt64(Id);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    String otherCategories = "";
                    if (!String.IsNullOrEmpty(reader["ID"].ToString()))
                    {
                        article.Id = reader["ID"].ToString();
                    }

                    if (!String.IsNullOrEmpty(reader["TIME_START"].ToString()))
                    {
                        article.DatePublished = (DateTime)reader["TIME_START"];
                    }
                    article.Disabled = false;
                    if (!String.IsNullOrEmpty(reader["TIME_END"].ToString()))
                    {
                        article.DateDisabled = (DateTime)reader["TIME_END"];

                        if (article.DateDisabled <= DateTime.Now)
                        {
                            article.Disabled = true;
                        }
                    }
                    if (!String.IsNullOrEmpty(reader["HEADLINE"].ToString()))
                    {
                        article.Title = (String)reader["HEADLINE"];
                    }
                    if (!String.IsNullOrEmpty(reader["BODY"].ToString()))
                    {
                        article.Body = (String)reader["BODY"];
                    }
                    if (!String.IsNullOrEmpty(reader["LINK"].ToString()))
                    {
                        article.Link = (String)reader["LINK"];
                    }
                    if (!String.IsNullOrEmpty(reader["IMAGE_NAME"].ToString()))
                    {
                        article.Image = (String)reader["IMAGE_NAME"];
                    }
                    if (!String.IsNullOrEmpty(reader["OTHER_CATEGORY"].ToString()))
                    {
                        otherCategories = (String)reader["OTHER_CATEGORY"];
                    }
                    if (!String.IsNullOrEmpty(reader["TYPE_ID"].ToString()))
                    {
                        String tempTypeId = reader["TYPE_ID"].ToString();
                        if (otherCategories.IndexOf(tempTypeId)<0)
                        {
                            otherCategories += "~" + tempTypeId;
                        }
                    }
                    if (!String.IsNullOrEmpty(otherCategories))
                    {
                        article.Tags = GetNewsTypeNameById(otherCategories);
                    }
                    article.ArticleType = "news";
                }
            }
            reader.Close();
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
            return article;
        }

        public List<Article> Search(String keyword, Boolean onlyActive, Int32 numberOfArticles=0)
        {
            List<Article> articles = new List<Article>();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT");
            if (numberOfArticles > 0)
            {
                sql.Append(" TOP " + numberOfArticles);
            }

            sql.Append(" ID, TYPE_ID, TIME_START, TIME_END, HEADLINE,");
            
            if(_getBodyContent){
                sql.Append(" BODY,");
            }
            sql.Append(" LINK, IMAGE_NAME, OTHER_CATEGORY FROM NEWS");
            if (onlyActive) {
                sql.Append(" WHERE (TIME_START IS Null OR TIME_START <= '" + DateTime.Now + "') AND (TIME_END IS Null OR TIME_END >= '" + DateTime.Now + "')");
            }

            if (!String.IsNullOrEmpty(keyword))
            {
                if (onlyActive)
                {
                    sql.Append(" AND");
                }
                else {
                    sql.Append(" WHERE");
                }
                sql.Append(" (HEADLINE LIKE @keyword OR BODY LIKE @keyword)");
            }

            sql.Append(" ORDER BY TIME_START DESC");


            SqlConnection conn = new DBConnection().GetConnection("");
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            if (!String.IsNullOrEmpty(keyword))
            {
                cmd.Parameters.Add(new SqlParameter("@keyword", System.Data.SqlDbType.VarChar, 255)).Value = "%" + keyword + "%";
            }
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    Article article = new Article();
                    String otherCategories = "";
                    
                    if (!String.IsNullOrEmpty(reader["ID"].ToString())) 
                    {
                        article.Id = reader["ID"].ToString();
                    }
                    
                    if (!String.IsNullOrEmpty(reader["TIME_START"].ToString()))
                    {
                        article.DatePublished = (DateTime)reader["TIME_START"];
                    }
                    article.Disabled = false;
                    if (!String.IsNullOrEmpty(reader["TIME_END"].ToString()))
                    {
                        article.DateDisabled = (DateTime)reader["TIME_END"];

                        if (article.DateDisabled <= DateTime.Now) {
                            article.Disabled = true;
                        }
                    }
                    if (!String.IsNullOrEmpty(reader["HEADLINE"].ToString()))
                    {
                        article.Title = (String)reader["HEADLINE"];
                    }
                    if (_getBodyContent)
                    {
                        if (!String.IsNullOrEmpty(reader["BODY"].ToString()))
                        {
                            article.Body = (String)reader["BODY"];
                        }
                    }
                    if (!String.IsNullOrEmpty(reader["LINK"].ToString()))
                    {
                        article.Link = (String)reader["LINK"];
                    }
                    if (!String.IsNullOrEmpty(reader["IMAGE_NAME"].ToString()))
                    {
                        article.Image = (String)reader["IMAGE_NAME"];
                    }
                    if (!String.IsNullOrEmpty(reader["OTHER_CATEGORY"].ToString()))
                    {
                        otherCategories = (String)reader["OTHER_CATEGORY"];
                    }
                    if (!String.IsNullOrEmpty(reader["TYPE_ID"].ToString()))
                    {
                        otherCategories += "~" + reader["TYPE_ID"].ToString();
                    }
                    if(!String.IsNullOrEmpty(otherCategories)){
                        article.Tags = GetNewsTypeNameById(otherCategories);
                    }
                    article.ArticleType = "news";
                    articles.Add(article);
                }
            }
            reader.Close();
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
            return articles;
        }

        public List<Article> SearchByType(String typeName,  Boolean onlyActive, Int32 numberOfArticles=0)
        {
            Int16 typeId = GetNewsTypeIdByName(typeName);
            List<Article> articles = new List<Article>();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT");
            if (numberOfArticles > 0)
            {
                sql.Append(" TOP " + numberOfArticles);
            }
            sql.Append(" ID, TYPE_ID, TIME_START, TIME_END, HEADLINE, BODY, LINK, IMAGE_NAME, OTHER_CATEGORY FROM NEWS");
            if (onlyActive)
            {
                sql.Append(" WHERE (TIME_START IS Null OR TIME_START <= '" + DateTime.Now + "') AND (TIME_END IS Null OR TIME_END >= '" + DateTime.Now + "')");
            }

            if (onlyActive)
            {
                sql.Append(" AND");
            }
            else
            {
                sql.Append(" WHERE");
            }
            sql.Append(" (TYPE_ID = @typeId OR OTHER_CATEGORY LIKE @type)");
            sql.Append(" ORDER BY TIME_START DESC");

            SqlConnection conn = new DBConnection().GetConnection("");
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);

            cmd.Parameters.Add(new SqlParameter("@typeId", System.Data.SqlDbType.Int)).Value = typeId;
            cmd.Parameters.Add(new SqlParameter("@type", System.Data.SqlDbType.VarChar, 255)).Value = "%" + typeId.ToString() + "%";
            
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Article article = new Article();
                    String otherCategories = "";

                    if (!String.IsNullOrEmpty(reader["ID"].ToString()))
                    {
                        article.Id = reader["ID"].ToString();
                    }

                    if (!String.IsNullOrEmpty(reader["TIME_START"].ToString()))
                    {
                        article.DatePublished = (DateTime)reader["TIME_START"];
                    }
                    article.Disabled = false;
                    if (!String.IsNullOrEmpty(reader["TIME_END"].ToString()))
                    {
                        article.DateDisabled = (DateTime)reader["TIME_END"];

                        if (article.DateDisabled <= DateTime.Now)
                        {
                            article.Disabled = true;
                        }
                    }
                    if (!String.IsNullOrEmpty(reader["HEADLINE"].ToString()))
                    {
                        article.Title = (String)reader["HEADLINE"];
                    }
                    if (!String.IsNullOrEmpty(reader["BODY"].ToString()))
                    {
                        article.Body = (String)reader["BODY"];
                    }
                    if (!String.IsNullOrEmpty(reader["LINK"].ToString()))
                    {
                        article.Link = (String)reader["LINK"];
                    }
                    if (!String.IsNullOrEmpty(reader["IMAGE_NAME"].ToString()))
                    {
                        article.Image = (String)reader["IMAGE_NAME"];
                    }
                    if (!String.IsNullOrEmpty(reader["OTHER_CATEGORY"].ToString()))
                    {
                        otherCategories = (String)reader["OTHER_CATEGORY"];
                    }
                    if (!String.IsNullOrEmpty(reader["TYPE_ID"].ToString()))
                    {
                        otherCategories += "~" + reader["TYPE_ID"].ToString();
                    }
                    if (!String.IsNullOrEmpty(otherCategories))
                    {
                        article.Tags = GetNewsTypeNameById(otherCategories);
                    }
                    article.ArticleType = "news";
                    articles.Add(article);
                }
            }
            reader.Close();
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
            return articles;
        }

        private List<String> GetNewsTypeNameById(String otherCategories)
        {
            List<String> categoryNames = new List<String>();
            String[] categories;
            Int16 categoryId =0;
            categories = otherCategories.Split('~');
            foreach (String category in categories) {
                if (Int16.TryParse(category, out categoryId))
                {
                    categoryNames.Add(_newsTypes.Find(n => n.Id == categoryId).Name.ToLower());
                }
            }
            return categoryNames;
        }

        private Int16 GetNewsTypeIdByName(String name)
        {
            Int16 typeId = 0;
            var newsType = _newsTypes.FirstOrDefault(n => n.Name == name.ToLower());
            typeId = (newsType != null) ? Convert.ToInt16(newsType.Id) : Convert.ToInt16(0);
            return typeId;
        }

        private List<NewsType> GetNewsTypes() { 
            List<NewsType> newsTypes = new List<NewsType>();
            String sql = "SELECT ID, NAME FROM NEWS_TYPE";
            SqlConnection conn = new DBConnection().GetConnection("");
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    NewsType newsType = new NewsType(); 
                    if (!String.IsNullOrEmpty(reader["ID"].ToString()))
                    {
                        newsType.Id = Convert.ToInt16(reader["ID"]);
                    }
                    if (!String.IsNullOrEmpty(reader["NAME"].ToString()))
                    {
                        newsType.Name = reader["NAME"].ToString().ToLower().Replace("_"," ");
                    }
                    if (newsType.Id != null && !String.IsNullOrEmpty(newsType.Name)) 
                    {
                        newsTypes.Add(newsType);
                    }
                }
            }
            reader.Close();
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
            return newsTypes;
        }
    }
}
