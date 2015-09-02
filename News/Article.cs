using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SavannahState.News
{
    public class Article
    {
        public String Id { get; set; }
        public String Title { get; set; }
        public String Body { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateDisabled { get; set; }
        public Boolean Disabled { get; set; }
        public List<String> Tags { get; set; }
        public String Link { get; set; }
        public String Image { get; set; }
        public String ArticleType { get; set; }


        public Article() {
            Disabled = false;
        }
    }
}
