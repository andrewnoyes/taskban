using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.OneDrive.Sdk;
using Newtonsoft.Json;
using TamedTasks.Models.OneNote;

namespace TamedTasks.Data
{
    internal class OneDriveAPI 
    {
        #region Singleton Constructor

        private static readonly Lazy<OneDriveAPI> _lazy =
        new Lazy<OneDriveAPI>(() => new OneDriveAPI());

        /// <summary>
        /// The single, observable instance for OneDrive access.
        /// </summary>
        public static OneDriveAPI Instance => _lazy.Value;

        private OneDriveAPI() { }

        #endregion

        #region Properties

        private static OneDriveClient OneDriveClient { get; set; }

        public bool IsConnected => OneDriveClient != null && OneDriveClient.IsAuthenticated;

        #endregion

        #region Public Methods

        // Endpoints
        private static string _oneNoteRoot = "https://www.onenote.com/api/v1.0/me/notes/";
        private static string _notebooksCollectionEndpoint = "notebooks?";
        private static string _allSectionsEndPoint = "sections?";
        private static string _pagesBySection = "sections/{0}/pages?pageLevel=true&top=100"; // todo: not returning the page level for some reason...
        private static string _pageContent = "pages/{0}/content";

        /// <summary>
        /// Gets the universal client (Windows 10) and attempts to authenticate with OneDrive. 
        /// </summary>
        /// <returns>True if the authentication was successful, otherwise false.</returns>
        public async Task<bool> InitializeAsync()
        {
            var scopes = new[] { "wl.offline_access", "wl.signin", "office.onenote_update", "onedrive.readwrite" };
            var redirect = "urn:ietf:wg:oauth:2.0:oob";

            OneDriveClient = OneDriveClientExtensions.GetUniversalClient(scopes, redirect) as OneDriveClient;

            if (OneDriveClient == null) return false;

            try
            {
                await OneDriveClient.AuthenticateAsync();

                Debug.WriteLine("Successfully authenticated with OneDrive");
            }
            catch (OneDriveException ex)
            {
                Debug.WriteLine("error authenticating" + ex);
                OneDriveClient.Dispose();
                return false;
            }

            return OneDriveClient.IsAuthenticated;
        }

        /// <summary>
        /// Signs out of the OneDrive client.
        /// </summary>
        /// <returns>True if successful.</returns>
        public async Task<bool> SignOutAsync()
        {
            if (OneDriveClient == null) return true;

            await OneDriveClient.SignOutAsync();
            OneDriveClient.Dispose();
            return true;
        }

        /// <summary>
        /// Gets the entire list of sections. 
        /// </summary>
        /// <returns>A SectionCollection object containing the lists of sections.</returns>
        public async Task<SectionCollection> GetSectionsAsync()
        {
            var strResult = await Get(_allSectionsEndPoint);
            return JsonConvert.DeserializeObject<SectionCollection>(strResult);
        }

        /// <summary>
        /// Gets the entire list of notebooks.
        /// </summary>
        /// <returns>A NotebookCollection object containing its list of notebooks.</returns>
        public async Task<NotebookCollection> GetNotebookCollectionAsync()
        {
            var strResult = await Get(_notebooksCollectionEndpoint);
            return JsonConvert.DeserializeObject<NotebookCollection>(strResult);
        }

        /// <summary>
        /// Gets all list of pages for the given section.
        /// </summary>
        /// <param name="sectionId">Unique section id to fetch pages from.</param>
        /// <returns>PageCollection object with its list of pages.</returns>
        public async Task<PageCollection> GetPagesAsync(string sectionId)
        {
            var endpoint = string.Format(_pagesBySection, sectionId);
            var strResult = await Get(endpoint);

            return JsonConvert.DeserializeObject<PageCollection>(strResult);
        }

        /// <summary>
        /// Gets the page contents for a give page id.
        /// </summary>
        /// <param name="pageId">Unique page id to fetch contents from.</param>
        /// <returns>PageContent object with raw HTML content.</returns>
        public async Task<PageContent> GetPageContentAsync(string pageId)
        {
            var endpoint = string.Format(_pageContent, pageId);
            var result = await Get(endpoint);
            return new PageContent
            {
                Id = (result + pageId).GetHashCode().ToString(),
                Html = result,
                PageId = pageId
            };
        }

        #endregion

        #region Private Methods

        private static async Task<string> Get(string endpoint)
        {
            var client = GetHttpClient();
            var msg = new HttpRequestMessage(HttpMethod.Get, _oneNoteRoot + endpoint);
            var response = await client.SendAsync(msg);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsStringAsync();
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OneDriveClient.AuthenticationProvider.CurrentAccountSession.AccessToken);

            return client;
        }

        #endregion
    }
}
