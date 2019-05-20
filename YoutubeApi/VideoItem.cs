using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeApi
{
    public struct Thumbnails
    {
        [JsonProperty(PropertyName = "default")]
        public Thumbnail? Default;

        [JsonProperty(PropertyName = "medium")]
        public Thumbnail? Medium;

        [JsonProperty(PropertyName = "high")]
        public Thumbnail? High;

        [JsonProperty(PropertyName = "standard")]
        public Thumbnail? Standard;

        [JsonProperty(PropertyName = "maxres")]
        public Thumbnail? MaxRes;
    }

    [JsonObject(MemberSerialization=MemberSerialization.OptIn)]
    public struct Thumbnail
    {
        [JsonProperty(PropertyName = "url")]
        public string Url;

        [JsonProperty(PropertyName = "width")]
        public int Width;

        [JsonProperty(PropertyName = "height")]
        public int Height;
    }

    public class VideoItem
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public Thumbnails Thumbnails { get; set; }
        public int CategoryId { get; set; }
        public string ChannelTitle { get; set; }
        public DateTime PublishedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int ViewCount { get; set; }
    }
}
