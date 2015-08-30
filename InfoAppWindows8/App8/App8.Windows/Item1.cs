using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App8
{
    public class Item1
    {
        static Uri baseUri = new Uri("ms-appx:///");
        private string title;

        public string Tilte
        {
            get { return title; }
            set { title = value; }
        }

        private Uri image;

        public Uri Image
        {
            get { return image; }
            set { image = value; }
        }
        public Item1(string a, string path)
        {
            this.title = a;
            this.image = new Uri(baseUri, path);
        }

    }
}
