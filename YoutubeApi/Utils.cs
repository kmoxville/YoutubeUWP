using System;

namespace YoutubeApi
{
    public static class UriExtension
    {
        //Extension to Uri class
        public static Uri AppendQuery(this Uri uri, string name, string value)
        {
            var baseUri = new UriBuilder(uri);
            if (baseUri.Query != null && baseUri.Query.Length > 1)
            {
                baseUri.Query = string.Format("{0}&{1}=2", baseUri.Query.Substring(1), name, value);
            }
            else
            {
                baseUri.Query = string.Format("{0}={1}", name, value);
            }
            return baseUri.Uri;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QuerryAttribute : Attribute
    {
        public QuerryAttribute(string querry)
        {
            Querry = querry;
        }

        public string Querry { get; private set; }
    }
}