using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace YoutubeApi
{
    public class BaseRequest<ResponseType>
        where ResponseType : IProcessResponse, new()
    {
        protected const string BASE_URI = "https://www.googleapis.com/youtube/v3/";
        private YoutubeClient client;

        public BaseRequest(YoutubeClient client)
        {
            this.client = client;
        }

        virtual protected HttpMethod HttpMethod { get; set; }
        virtual protected string MethodName { get; set; }
        virtual protected string Part { get; set; }
        virtual protected string Filter { get; set; }
        virtual protected string Optional { get; set; }

        public virtual async Task<ResponseType> ExecuteAsync()
        {
            HttpClient httpClient = new HttpClient();
            var uriString = new StringBuilder(string.Format("{0}{1}?{2}&{3}",
                BASE_URI,
                MethodName,
                "part=" + Part,
                Filter));

            var optionalCache = Optional;
            if (optionalCache != null && optionalCache.Length != 0)
            {
                uriString.Append("&" + optionalCache);
            }

            uriString.AppendFormat("&key=" + client.ApiToken);

            Uri requestUri = new Uri(uriString.ToString());
            var httpRequest = new HttpRequestMessage(this.HttpMethod, requestUri);

            HttpResponseMessage httpResponse = new HttpResponseMessage();

            string httpResponseBody = "";

            ResponseType response = new ResponseType();
            try
            {
                httpResponse = await httpClient.SendRequestAsync(httpRequest);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                response.ProcessResponse(httpResponseBody);
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            return response;
        }
    }
}
