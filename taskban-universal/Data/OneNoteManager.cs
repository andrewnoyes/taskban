using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using HtmlAgilityPack;
using TamedTasks.Models.Base;
using TamedTasks.Models.Common;
using TamedTasks.Models.OneNote;

namespace TamedTasks.Data
{
    public class OneNoteManager
    {
        #region Singleton Constructor

        private static readonly Lazy<OneNoteManager> _lazy =
            new Lazy<OneNoteManager>(() => new OneNoteManager());

        public static OneNoteManager Instance => _lazy.Value;

        private OneNoteManager() { }

        #endregion

        #region Properties

        public bool IsMicrosoftConnected { get; private set; }

        public User User { get; private set;  }

        public bool IsSyncing { get; private set; }

        #endregion

        #region Public

        /// <summary>
        /// Initializes any connected accounts, then resyncs 
        /// the user's content and reloads from the database.
        /// </summary>
        public void Initialize()
        {
            InitializeAsync();
        }

        /// <summary>
        /// Connect the user's Microsoft account, 
        /// prompts for permissions needed by the app.
        /// </summary>
        public async void ConnectMicrosoftAccount()
        {
            var connect = !IsMicrosoftConnected;

            bool result;
            if (connect)
            {
                Debug.WriteLine("Connecting with Microsoft account...");

                result = await _oneDrive.InitializeAsync();
            }
            else
            {
                Debug.WriteLine("Disconnecting from Microsoft account...");

                result = !await _oneDrive.SignOutAsync(); // true on successful sign out
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsMicrosoftConnected = result);

            User.HasMicrosoftAccountConnected = result ? 1 : 0;
            _dbManager.UpdateUser(User);

            //if (!IsMicrosoftConnected) CleanUp();
            //else SyncAsync();
            SyncAsync(); // TODO: this is being reworked, no more observable collections maintained here
        }

        /// <summary>
        /// Fetches the entire list of OneNote notebooks from OneDrive.
        /// </summary>
        public async Task<IList<Notebook>> FetchNotebooksAsync()
        {
            var collection = await _oneDrive.GetNotebookCollectionAsync();
            return collection.Notebooks;
        }

        /// <summary>
        /// Import the provided notebook's sections, pages, and page contents
        /// into the database. 
        /// </summary>
        /// <param name="notebooks">The OneNote notebooks to import.</param>
        public void ImportNotebooks(IList<Notebook> notebooks)
        {
            Debug.WriteLine("Importing notebooks into database");

            _dbManager.InsertOrUpdateList(notebooks.Where(n => n.ImportIntoDb).ToArray());
            SyncAsync();
        }

        /// <summary>
        /// Refetches all OneNote content for the user's imported notebooks, 
        /// then syncs the changes with the local database.
        /// </summary>
        public async void SyncAsync()
        {
            var sw = new Stopwatch(); sw.Start();

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsSyncing = true);

            Debug.WriteLine("Syncing with OneNote...");

            var notebooks = _dbManager.GetNotebooks(); // not fetched from OneNote, must be imported before

            Debug.WriteLine("Fetching all sections...");

            // todo: fetch sections using parent notebook id, then can be done async
            var collection = await _oneDrive.GetSectionsAsync();

            Debug.WriteLine("Filtering sections...");

            var filteredSections = FilterSections(notebooks, collection.Sections);

            var conSections = new ConcurrentBag<Section>(); // todo: not sure if needed
            foreach (var section in filteredSections)
                conSections.Add(section);

            Debug.WriteLine("Fetching pages by section...");

            var filteredPages = new ConcurrentBag<Page>();
            var tasks = conSections.Select(section => FetchPagesBySectionAsync(section)
                                   .ContinueWith(filtered =>
                                   {
                                       foreach (var page in filtered.Result)
                                           filteredPages.Add(page);
                                   }));
            await Task.WhenAll(tasks);

            Debug.WriteLine("Fetching page contents by page...");

            var pageContents = new ConcurrentBag<PageContent>();
            var contentTasks = filteredPages.Select(page => _oneDrive.GetPageContentAsync(page.Id)
                                            .ContinueWith(pageContent => { pageContents.Add(pageContent.Result); }));
            await Task.WhenAll(contentTasks);

            Debug.WriteLine("Parsing new task items...");

            var taskItems = await Task.Run(() => GetNewTaskItems(pageContents.ToList()));

            Debug.WriteLine("Updating database...");

            await Task.Run(() => UpdateDatabase(filteredSections.ToArray(), filteredPages.ToArray(),
                                                pageContents.ToArray(), taskItems.ToArray()));

            sw.Stop();

            Debug.WriteLine("Sync complete - ellapsed time in ms: " + sw.ElapsedMilliseconds);

            Debug.WriteLine("Loading items from database...");

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //LoadAll();
                IsSyncing = false;
            });
        }

        #endregion

        #region Private

        // Managers 
        private OneDriveAPI _oneDrive = OneDriveAPI.Instance;
        private DbManager _dbManager = DbManager.Instance;

        /// <summary>
        /// Attempt to fetch any accounts the user 
        /// has associated with their account.
        /// </summary>
        private async void InitializeAsync()
        {
            Debug.WriteLine("Fetching all items from local storage...");

            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, LoadAll);

            Debug.WriteLine("Initializing connected accounts...");

            User = _dbManager.GetUser();
            if (User == null) throw new Exception("Null user!"); // shouldn't happen, startup creates blank user

            if (User.HasMicrosoftAccountConnected == 1 && await _oneDrive.InitializeAsync())
            {
                IsMicrosoftConnected = true;

                Debug.WriteLine("Successfully connected to Microsoft account!");

                //SyncAsync(); // todo: temp disable for now
                //LoadAll();
            }
            else
            {
                Debug.WriteLine(User.HasMicrosoftAccountConnected == 0
                    ? "User does not have a Microsoft account connected!"
                    : "Unable to connect to user's Microsoft account!");

                IsMicrosoftConnected = false;
            }
        }

        /// <summary>
        /// Filters the provided sections by their parent notebooks, 
        /// sections not associated with the user's imported 
        /// notebooks will be dumped. TODO: update fetch calls to include notebook id
        /// </summary>
        private IList<Section> FilterSections(IList<Notebook> notebooks, IList<Section> sectionsToFilter)
        {

            var filtered = new List<Section>();

            foreach (var section in sectionsToFilter)
            {
                if (notebooks.All(n => n.Id != section.ParentNotebook.Id)) continue;

                // not a json property, ParentNotebook object will be ignore on DB insert
                section.ParentNotebookId = section.ParentNotebook.Id;
                filtered.Add(section);
            }

            return filtered;
        }

        /// <summary>
        /// Fetches the pages' meta data for the provided section.
        /// </summary>
        private async Task<IList<Page>> FetchPagesBySectionAsync(Section section)
        {
            var pages = (await _oneDrive.GetPagesAsync(section.Id)).Pages;
            foreach (var page in pages)
            {
                if (page.ParentSection != null)
                {
                    // not a json property, need to manually set
                    page.ParentSectionId = page.ParentSection.Id;
                }
            }

            return pages;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="pageContents"></param>
        /// <returns></returns>
        private IList<TaskItem> GetNewTaskItems(IList<PageContent> pageContents)
        {
            var currentTasks = _dbManager.GetAllTaskItems();
            var newTasks = new List<TaskItem>();
            foreach (var content in pageContents)
            {
                if (string.IsNullOrEmpty(content.Html)) continue;

                var doc = new HtmlDocument();
                doc.LoadHtml(content.Html);

                var paragraphs = doc.DocumentNode.Descendants("p"); // to-do items contained in paragraph tags
                foreach (var p in paragraphs)
                {
                    var dataTag = p.Attributes["data-tag"];
                    if (string.IsNullOrEmpty(dataTag?.Value)) continue;

                    if (dataTag.Value.Contains("to-do")) // todo: starting with any to-do items, need to grab all tags
                    {
                        var title = WebUtility.HtmlDecode(p.InnerText); // all string content is html encoded

                        // todo: this is temp just to prevent duplicates for now, need to go off a key
                        if (string.IsNullOrEmpty(title) || currentTasks.Any(t => t.Title.Equals(title)))
                            continue;

                        /* todo: need to check if this task has any subtasks, 
                         * then recursively find each, create it, and assign the FK to parent
                         */

                        var progress = dataTag.Value.Contains("complete") // todo: incorporate in-progress from subtasks
                            ? TaskItem.Progress.Complete
                            : TaskItem.Progress.Backlog;

                        var task = new TaskItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Title = title,
                            HtmlContent = p.OuterHtml,
                            PageContentId = content.Id,
                            TaskProgress = progress
                        };

                        if (newTasks.TrueForAll(t => !t.Title.Equals(task.Title))) // todo: same reason for current check, need to update
                        {
                            newTasks.Add(task);
                        }
                    }
                }
            }

            return newTasks;
        }


        #region Update Current State

        private void UpdateCurrentSections()
        {
            //var current = new ObservableCollection<Section>();
            //foreach (var section in Sections)
            //{
            //    if (string.IsNullOrEmpty(section.ParentNotebookId) ||
            //        section.ParentNotebookId != CurrentNotebook.Id)
            //        continue;

            //    if (!current.Contains(section))
            //        current.Add(section);
            //}

            //CurrentSections = current;
        }

        private void UpdateCurrentPages()
        {
            //var current = new ObservableCollection<Page>();
            //foreach (var page in Pages)
            //{
            //    if (string.IsNullOrEmpty(page.ParentSectionId) ||
            //        page.ParentSectionId != CurrentSection.Id)
            //        continue;

            //    if (!current.Contains(page))
            //        current.Add(page);
            //}

            //CurrentPages = current;
        }

        private void UpdateContentsAndTasks()
        {
            //var contents = new List<PageContent>();
            //var tasks = new List<TaskItem>();

            //foreach (var page in CurrentPages)
            //{
            //    foreach (var pageContent in PageContents.Where(p => p.PageId.Equals(page.Id)))
            //    {
            //        contents.Add(pageContent);
            //        tasks.AddRange(TaskItems.Where(t => t.PageContentId.Equals(pageContent.Id)));
            //    }
            //}

            //CurrentPageContents = new ObservableCollection<PageContent>(contents);
            //CurrentTaskItems = new ObservableCollection<TaskItem>(tasks);
        }

        private void UpdateDatabase(Section[] sections, Page[] pages,
                                    PageContent[] pageContents, TaskItem[] taskItems)
        {
            _dbManager.InsertOrUpdateList(sections);
            _dbManager.InsertOrUpdateList(pages);
            _dbManager.InsertOrUpdateList(pageContents);
            _dbManager.InsertOrUpdateList(taskItems);
        }

        #endregion

        #endregion
    }
}
