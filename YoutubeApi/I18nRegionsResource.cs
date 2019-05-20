using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Web.Http;

namespace YoutubeApi
{
    public class I18nRegionsResource
    {
        private YoutubeClient client;

        public I18nRegionsResource(YoutubeClient client)
        {
            this.client = client;
        }

        public ListRequest List(ListRequest.Parts parts)
        {
            return new ListRequest(client, parts);
        }

        public class ListRequest : BaseRequest<ListRequestResponse>
        {
            public ListRequest(YoutubeClient client, Parts part) : base(client)
            {
                Part = part.Value;
            }

            protected override string Part { get; set; }
            protected override string Optional
            {
                get
                {
                    string result = "";
                    foreach (var property in this.GetType().GetProperties())
                    {
                        var attributes = property.GetCustomAttributes(false);
                        var attribute = attributes.FirstOrDefault(a => a.GetType() == typeof(QuerryAttribute));
                        if (attribute != null)
                        {
                            var value = property.GetValue(this);
                            if (value != null)
                            {
                                if (result.Length != 0) result += "&";
                                result += string.Format("{0}={1}",
                                    (attribute as QuerryAttribute).Querry,
                                    value);
                            }
                        }
                    }
                    return result;
                }
            }

            protected override string MethodName { get => "i18nRegions"; }
            protected override HttpMethod HttpMethod { get => HttpMethod.Get; }

            [Querry("hl")]
            public string Hl { get; set; }

            public class Parts
            {
                private Parts(string value) { Value = value; }

                public string Value { get; set; }

                public static Parts Snippet { get { return new Parts("snippet"); } }
            }
        }

        public class ListRequestResponse : IProcessResponse
        {
            public ListRequestResponse()
            {

            }

            public List<I18Item> I18Items { get; private set; }

            public void ProcessResponse(string json_object)
            {
                JObject o = JObject.Parse(json_object);

                var i18ItemsQuerry = from item in o["items"]
                                       let snippet = item["snippet"]
                                       select new I18Item()
                                       {
                                           Gl = (string)snippet["gl"],
                                           Name = (string)snippet["name"],
                                       };

                I18Items = i18ItemsQuerry.ToList<I18Item>();
            }
        }
    }
}
