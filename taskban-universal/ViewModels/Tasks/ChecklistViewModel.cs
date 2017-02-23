using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TamedTasks.Data;
using TamedTasks.Models.Common;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Tasks
{
    public class ChecklistViewModel : ViewModelBase
    {
        private ObservableCollection<ChecklistItem> _checklistItems;
        public ObservableCollection<ChecklistItem> ChecklistItems
        {
            get { return _checklistItems; }
            set
            {
                Set(ref _checklistItems, value);
                HasChecklist = ChecklistItems != null && ChecklistItems.Count > 0;
            }
        }

        private bool _hasChecklist;
        public bool HasChecklist
        {
            get { return _hasChecklist; }
            set { Set(ref _hasChecklist, value); }
        }

        private int? _completeChecklistItems;
        public int? CompleteChecklistItems
        {
            get
            {
                return _completeChecklistItems ?? (_completeChecklistItems = ChecklistItems.Count(c => c.IsComplete));
            }
            set { Set(ref _completeChecklistItems, value); }
        }

        private DelegateCommand _addChecklistItemCommand;
        public DelegateCommand AddChecklistItemCommand =>
            _addChecklistItemCommand ??
            (_addChecklistItemCommand = new DelegateCommand(AddChecklistItem, CanAddChecklistItem));

        public event EventHandler ChecklistItemAdded;

        private string _checklistItem;
        public string ChecklistItem
        {
            get { return _checklistItem; }
            set
            {
                _checklistItem = value;
                RaisePropertyChanged();
                AddChecklistItemCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly string _taskItemId;
        private bool _autoSave;

        private void UpdateChecklistProps()
        {
            if (ChecklistItems != null)
            {
                CompleteChecklistItems = ChecklistItems.Count(c => c.IsComplete);
            }

            HasChecklist = ChecklistItems != null && ChecklistItems.Count > 0;
        }

        public ChecklistViewModel(string taskItemId, bool autoSave = false)
        {
            _taskItemId = taskItemId;
            _autoSave = autoSave;
            ChecklistItems = new ObservableCollection<ChecklistItem>(DbManager.Instance.GetChecklistItemsInTaskItem(taskItemId));
        }

        internal bool Save()
        {
            UpdateChecklistProps();
            return DbManager.Instance.InsertOrUpdateList(ChecklistItems.ToArray());
        }

        internal Task DeleteAsync(ChecklistItem toDelete)
        {
            return Task.Run(() => DbManager.Instance.DeleteEntity(toDelete));
        }

        public void AddChecklistItem()
        {
            var item = new ChecklistItem
            {
                Contents = ChecklistItem,
                Order = ChecklistItems.Count,
                TaskItemId = _taskItemId
            };

            ChecklistItems.Add(item);
            ChecklistItem = string.Empty;
            HasChecklist = true;

            ChecklistItemAdded?.Invoke(this, EventArgs.Empty);

            if (_autoSave) Save();
        }

        public Task DeleteChecklistItemAsync(ChecklistItem itemToDelete)
        {
            ChecklistItems.Remove(itemToDelete);
            UpdateChecklistProps();
            return Task.Run(() => DbManager.Instance.DeleteEntity(itemToDelete));
        }

        public bool CanAddChecklistItem()
        {
            return !string.IsNullOrEmpty(ChecklistItem);
        }
    }
}
