using System;
using System.Collections.Generic;

namespace SavannahState.News
{
    public interface IStoryRepository
    {
        List<Article> GetArticles(Boolean onlyActive);
        Article GetArticleById(String Id);
        List<Article> Search(String keyword, Boolean onlyActive);
        List<Article> SearchByType(String typeName, Boolean onlyActive);
    }
}
