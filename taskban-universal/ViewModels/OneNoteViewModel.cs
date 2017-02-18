using System.Collections.Generic;
using System.Collections.ObjectModel;
using TamedTasks.Models.Common;
using TamedTasks.Models.OneNote;

namespace TamedTasks.ViewModels
{
    /// <summary>
    /// TODO: move all the lists from data manager here as separate VM classes
    /// Then instantiate VM properties on creation
    /// </summary>
    public class OneNoteViewModel 
    {
        //private Dictionary<Notebook, ObservableCollection<Section>> _sectionsByNotebook;
        //private Dictionary<Section, ObservableCollection<Page>> _pagesBySection;
        //private Dictionary<Page, ObservableCollection<TaskItem>> _tasksByPage;

        //private ObservableCollection<Notebook> _notebooks = new ObservableCollection<Notebook>();
        //public ObservableCollection<Notebook> Notebooks
        //{
        //    get { return _notebooks; }
        //    set
        //    {
        //        _notebooks = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private Notebook _currentNotebook;
        //public Notebook CurrentNotebook
        //{
        //    get { return _currentNotebook; }
        //    set
        //    {
        //        _currentNotebook = value;
        //        OnPropertyChanged();
        //        if (CurrentNotebook != null)
        //            UpdateCurrentSections();
        //    }
        //}

        //private ObservableCollection<Section> _sections = new ObservableCollection<Section>();
        //public ObservableCollection<Section> Sections
        //{
        //    get { return _sections; }
        //    set
        //    {
        //        _sections = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ObservableCollection<Section> _currentSections = new ObservableCollection<Section>();
        //public ObservableCollection<Section> CurrentSections
        //{
        //    get { return _currentSections; }
        //    set
        //    {
        //        _currentSections = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private Section _currentSection = new Section();
        //public Section CurrentSection
        //{
        //    get { return _currentSection; }
        //    set
        //    {
        //        _currentSection = value;
        //        OnPropertyChanged();
        //        if (CurrentSection != null)
        //            UpdateCurrentPages();
        //    }
        //}

        //private ObservableCollection<Page> _currentPages = new ObservableCollection<Page>();
        //public ObservableCollection<Page> CurrentPages
        //{
        //    get { return _currentPages; }
        //    set
        //    {
        //        _currentPages = value;
        //        OnPropertyChanged();
        //        UpdateContentsAndTasks();
        //    }
        //}

        //private Page _currentPage = new Page();
        //public Page CurrentPage
        //{
        //    get { return _currentPage; }
        //    set
        //    {
        //        _currentPage = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ObservableCollection<Page> _pages = new ObservableCollection<Page>();
        //public ObservableCollection<Page> Pages
        //{
        //    get { return _pages; }
        //    set
        //    {
        //        _pages = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ObservableCollection<PageContent> _pageContents = new ObservableCollection<PageContent>();
        //public ObservableCollection<PageContent> PageContents
        //{
        //    get { return _pageContents; }
        //    set
        //    {
        //        _pageContents = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ObservableCollection<PageContent> _currentPageContents = new ObservableCollection<PageContent>();
        //public ObservableCollection<PageContent> CurrentPageContents
        //{
        //    get { return _currentPageContents; }
        //    set
        //    {
        //        _currentPageContents = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ObservableCollection<TaskItem> _taskitems = new ObservableCollection<TaskItem>();
        //public ObservableCollection<TaskItem> TaskItems
        //{
        //    get { return _taskitems; }
        //    set
        //    {
        //        _taskitems = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private ObservableCollection<TaskItem> _currentTaskitems = new ObservableCollection<TaskItem>();
        //public ObservableCollection<TaskItem> CurrentTaskItems
        //{
        //    get { return _currentTaskitems; }
        //    set
        //    {
        //        _currentTaskitems = value;
        //        OnPropertyChanged();
        //    }
        //}

        //private void CleanUp()
        //{
        //    // TODO
        //    Notebooks?.Clear();
        //    Sections?.Clear();
        //    Pages?.Clear();
        //}

        //private void LoadAll()
        //{
        //    Notebooks = new ObservableCollection<Notebook>(_dbManager.GetNotebooks());
        //    Sections = new ObservableCollection<Section>(_dbManager.GetSections());
        //    Pages = new ObservableCollection<Page>(_dbManager.GetPages());
        //    PageContents = new ObservableCollection<PageContent>(_dbManager.GetAllPageContents());
        //    TaskItems = new ObservableCollection<TaskItem>(_dbManager.GetAllTaskItems());
        //}

    }
}
