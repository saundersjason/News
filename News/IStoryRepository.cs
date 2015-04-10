using System;
using System.Collections.Generic;

namespace SavannahState.News
{
    public interface IStoryRepository
    {
        List<Article> GetArticles( Boolean onlyActive,Int32 numberOfArticles);
        Article GetArticleById(String Id);
        List<Article> Search(String keyword, Boolean onlyActive, Int32 numberOfArticles);
        List<Article> SearchByType(String typeName, Boolean onlyActive, Int32 numberOfArticles);
    }
}
