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
    public class VideosResource
    {
        private YoutubeClient client;

        public VideosResource(YoutubeClient client)
        {
            this.client = client;
        }

        public ListRequest List(ListRequest.Parts parts, ListRequest.Filters filter)
        {
            return new ListRequest(client, parts, filter);
        }

        public class ListRequest : BaseRequest<ListRequestResponse>
        {
            public ListRequest(YoutubeClient client, Parts part, Filters filter) : base(client)
            {
                Part = part.Value;
                Filter = filter.Value;
            }

            protected override string Part { get; set; }
            protected override string Filter { get; set; }
            protected override string Optional { get
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

            protected override string MethodName { get => "videos" ; }
            protected override HttpMethod HttpMethod { get => HttpMethod.Get; }

            [Querry("hl")]
            public string Hl { get; set; }

            [Querry("maxHeight")]
            public uint? MaxHeight { get; set; }

            [Querry("maxResults")]
            public uint? MaxResults { get; set; }

            [Querry("maxWidth")]
            public uint? MaxWidth { get; set; }

            [Querry("pageToken")]
            public string PageToken { get; set; }

            [Querry("regionCode")]
            public string RegionCode { get; set; }

            [Querry("videoCategoryId")]
            public string VideoCategoryId { get; set; }

            /*public class Optionals
            {
                private Optionals(string value) { Value = value; }

                public string Value { get; set; }

                //FixME check limits
                public static Optionals Hl(string hl) { return new Optionals("hl=" + hl); }
                public static Optionals MaxHeight(uint maxHeight) { return new Optionals("maxHeight=" + maxHeight); }
                public static Optionals MaxResults(uint maxResults) { return new Optionals("maxResults=" + maxResults); }
                public static Optionals MaxWidth(uint maxWidth) { return new Optionals("maxWidth=" + maxWidth); }
                public static Optionals OnBehalfOfContentOwner() { throw new NotImplementedException(); }
                public static Optionals PageToken(string pageToken) { return new Optionals("pageToken=" + pageToken); }
                public static Optionals RegionCode(string regionCode) { return new Optionals("regionCode=" + regionCode); }
                public static Optionals VideoCategoryId(string videoCategoryId) { return new Optionals("videoCategoryId=" + videoCategoryId); }

                public static Optionals operator +(Optionals lhs, Optionals rhs)
                {
                    return new Optionals(string.Format("{0}%{1}",
                        lhs.Value,
                        rhs.Value));
                }
            }*/

            public class Parts
            {
                private Parts(string value) { Value = value; }

                public string Value { get; set; }

                public static Parts ContentDetails { get { return new Parts("contentDetails"); } }
                public static Parts FileDetails { get { return new Parts("fileDetails"); } }
                public static Parts Id { get { return new Parts("id"); } }
                public static Parts LiveStreamingDetails { get { return new Parts("liveStreamingDetails"); } }
                public static Parts Localizations { get { return new Parts("localizations"); } }
                public static Parts Player { get { return new Parts("player"); } }
                public static Parts ProcessingDetails { get { return new Parts("processingDetails"); } }
                public static Parts RecordingDetails { get { return new Parts("recordingDetails"); } }
                public static Parts Snippet { get { return new Parts("snippet"); } }
                public static Parts Statistics { get { return new Parts("statistics"); } }
                public static Parts Status { get { return new Parts("status"); } }
                public static Parts Suggestions { get { return new Parts("suggestions"); } }
                public static Parts TopicDetails { get { return new Parts("topicDetails"); } }

                public static Parts operator+(Parts lhs, Parts rhs)
                {
                    return new Parts(string.Format("{0},{1}", 
                        lhs.Value,
                        rhs.Value));
                }
            }

            public class Filters
            {
                public Filters(Charts chart)
                {
                    this.Chart = chart;
                }

                public Filters(MyRatings myRating)
                {
                    this.MyRating = myRating;
                }

                public Filters(string id)
                {
                    this.Id = id;
                }

                public string Value {
                    get
                    {
                        return Id 
                            + (Chart != null ? Chart.Value : "") 
                            + (MyRating != null ? MyRating.Value : "") ;
                    }
                }
                
                public Charts Chart { get; }
                public MyRatings MyRating { get; } 
                public string Id { get; } = "";

                public class Charts
                {
                    private Charts(string value) { Value = value; }

                    public string Value { get; set; }

                    public static Charts MostPopular { get { return new Charts("chart=mostPopular"); } }
                }

                public class MyRatings
                {
                    private MyRatings(string value) { Value = value; }

                    public string Value { get; set; }

                    public static MyRatings Dislike { get { return new MyRatings("myRating=dislike"); } }
                    public static MyRatings Like { get { return new MyRatings("myRating=like"); } }
                }
            }          
        }

        public class ListRequestResponse : IProcessResponse
        {
            public ListRequestResponse()
            {

            }

            public string NextPageToken { get; private set; }
            public List<VideoItem> VideoItems { get; private set; } = new List<VideoItem>();

            public void ProcessResponse(string json_object)
            {
                JObject o = JObject.Parse(json_object);

                var videoItemsQuerry = from item in o["items"]
                                       let snippet = item["snippet"]
                                       let thumbnails = snippet["thumbnails"]
                                       where (string)snippet["title"] != "Deleted video"
                                       select new VideoItem() {
                                           Id = (string)item["id"],
                                           Title = (string)snippet["title"],
                                           Description = (string)snippet["description"],
                                           ChannelTitle = (string)snippet["channelTitle"],
                                           CategoryId = (int)snippet["categoryId"],
                                           Thumbnails = thumbnails.ToObject<Thumbnails>(),
                                           Duration = XmlConvert.ToTimeSpan((string)item["contentDetails"]["duration"]),
                                           PublishedAt = (DateTime)snippet["publishedAt"],
                                           ViewCount = (int)item["statistics"]["viewCount"]
                                 };

                NextPageToken = (string)o["nextPageToken"];
                try
                {
                    VideoItems = videoItemsQuerry.ToList<VideoItem>();
                }
                catch (Exception)
                {

                }
            }
        }
    }

    public interface IProcessResponse
    {
        void ProcessResponse(string json_object);
    }

    
}
