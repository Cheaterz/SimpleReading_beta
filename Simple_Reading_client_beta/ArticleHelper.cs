using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple_Reading_client_beta
{
    class ArticleHelper
    {
        public string Text { set; get; }
        public string Notes { set; get; }

        public ArticleHelper(string text, string n)
        {
            this.Text = text;
            this.Notes = n;
        }

        public ArticleHelper(string text)
        {
            this.Text = text;
        }
    }
}
