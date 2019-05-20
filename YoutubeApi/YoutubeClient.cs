using JWT;
using JWT.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;

namespace YoutubeApi
{
    public class YoutubeClient
    {
        public string ApiToken { get; private set; }        

        public YoutubeClient(string ApiToken)
        {
            this.ApiToken = ApiToken;

            Videos = new VideosResource(this);
            I18nRegions = new I18nRegionsResource(this);
        }

        public VideosResource Videos { get; }
        public I18nRegionsResource I18nRegions { get; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public async Task<ProviderUserDetails> AuthentificateAsync(string clientId, string clientSecret)
        {
            String YouTubeURL = "https://accounts.google.com/o/oauth2/v2/auth?client_id=" 
                + Uri.EscapeDataString(clientId) + "&redirect_uri=" 
                + Uri.EscapeDataString("urn:ietf:wg:oauth:2.0:oob") 
                + "&response_type=code&scope=" 
                + Uri.EscapeDataString("https://www.googleapis.com/auth/youtube email");

            Uri StartUri = new Uri(YouTubeURL);
            // As I use "urn:ietf:wg:oauth:2.0:oob" as the redirect_uri, the success code is displayed in the html title of following end uri
            Uri EndUri = new Uri("https://accounts.google.com/o/oauth2/approval");

            WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, StartUri, EndUri);
            if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var autorizationCode = WebAuthenticationResult.ResponseData.Substring(13);
                var pairs = new Dictionary<string, string>
                {
                    { "code", autorizationCode },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", "urn:ietf:wg:oauth:2.0:oob" },
                    { "grant_type", "authorization_code" }
                };

                var formContent = new HttpFormUrlEncodedContent(pairs);

                var client = new HttpClient();
                var httpResponseMessage = await client.PostAsync(new Uri("https://accounts.google.com/o/oauth2/token"), formContent);
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                JObject o = JObject.Parse(response);

                AccessToken = (string)o["access_token"];
                RefreshToken = (string)o["refresh_token"];

                var details = await GetUserDetails((string)o["id_token"]);
                details.AccessToken = AccessToken;
                details.RefreshToken = RefreshToken;
                return details;
            }

            return null;
        }

        private async Task<ProviderUserDetails> GetUserDetails(string providerToken)
        {
            string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";
            var httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, providerToken));

            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await httpClient.GetAsync(requestUri);
            }
            catch (Exception ex)
            {
                return null;
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.Ok)
            {
                //return null;
            }

            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            var googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);

            /*if (!SupportedClientsIds.Contains(googleApiTokenInfo.aud))
            {
                Log.WarnFormat("Google API Token Info aud field ({0}) not containing the required client id", googleApiTokenInfo.aud);
                return null;
            }*/

            return new ProviderUserDetails
            {
                Email = googleApiTokenInfo.email,
                FirstName = googleApiTokenInfo.given_name,
                LastName = googleApiTokenInfo.family_name,
                Locale = googleApiTokenInfo.locale,
                Name = googleApiTokenInfo.name,
                ProviderUserId = googleApiTokenInfo.sub,
            };
        }

        public class ProviderUserDetails
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Locale { get; set; }
            public string Name { get; set; }
            public string ProviderUserId { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }

    

    public class GoogleApiTokenInfo
    {
        /// <summary>
        /// The Issuer Identifier for the Issuer of the response. Always https://accounts.google.com or accounts.google.com for Google ID tokens.
        /// </summary>
        public string iss { get; set; }

        /// <summary>
        /// Access token hash. Provides validation that the access token is tied to the identity token. If the ID token is issued with an access token in the server flow, this is always
        /// included. This can be used as an alternate mechanism to protect against cross-site request forgery attacks, but if you follow Step 1 and Step 3 it is not necessary to verify the 
        /// access token.
        /// </summary>
        public string at_hash { get; set; }

        /// <summary>
        /// Identifies the audience that this ID token is intended for. It must be one of the OAuth 2.0 client IDs of your application.
        /// </summary>
        public string aud { get; set; }

        /// <summary>
        /// An identifier for the user, unique among all Google accounts and never reused. A Google account can have multiple emails at different points in time, but the sub value is never
        /// changed. Use sub within your application as the unique-identifier key for the user.
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// True if the user's e-mail address has been verified; otherwise false.
        /// </summary>
        public string email_verified { get; set; }

        /// <summary>
        /// The client_id of the authorized presenter. This claim is only needed when the party requesting the ID token is not the same as the audience of the ID token. This may be the
        /// case at Google for hybrid apps where a web application and Android app have a different client_id but share the same project.
        /// </summary>
        public string azp { get; set; }

        /// <summary>
        /// The user's email address. This may not be unique and is not suitable for use as a primary key. Provided only if your scope included the string "email".
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// The time the ID token was issued, represented in Unix time (integer seconds).
        /// </summary>
        public string iat { get; set; }

        /// <summary>
        /// The time the ID token expires, represented in Unix time (integer seconds).
        /// </summary>
        public string exp { get; set; }

        /// <summary>
        /// The user's full name, in a displayable form. Might be provided when:
        /// The request scope included the string "profile"
        /// The ID token is returned from a token refresh
        /// When name claims are present, you can use them to update your app's user records. Note that this claim is never guaranteed to be present.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The URL of the user's profile picture. Might be provided when:
        /// The request scope included the string "profile"
        /// The ID token is returned from a token refresh
        /// When picture claims are present, you can use them to update your app's user records. Note that this claim is never guaranteed to be present.
        /// </summary>
        public string picture { get; set; }

        public string given_name { get; set; }

        public string family_name { get; set; }

        public string locale { get; set; }

        public string alg { get; set; }

        public string kid { get; set; }
    }
}
