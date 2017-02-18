using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using TamedTasks.Models.Base;
using TamedTasks.Models.Common;
using TamedTasks.Models.OneNote;

namespace TamedTasks.Data
{
    public class DbManager
    {
        #region Singleton Constructor

        private static readonly Lazy<DbManager> _lazy =
            new Lazy<DbManager>(() => new DbManager());

        public static DbManager Instance => _lazy.Value;

        private DbManager()
        {
            // init db - only creates tables if they don't exist
            using (var db = DbConnection)
            {
                db.CreateTable<Notebook>();
                db.CreateTable<ParentNotebook>();
                db.CreateTable<Section>();
                db.CreateTable<ParentSection>();
                db.CreateTable<Page>();
                db.CreateTable<PageContent>();
                db.CreateTable<TaskItem>();
                db.CreateTable<TaskList>();
                db.CreateTable<ChecklistItem>();
                db.CreateTable<Board>();
                db.CreateTable<Project>();
                db.CreateTable<User>();

                if (!db.Table<User>().Any())
                {
                    db.InsertOrReplace(new User { HasMicrosoftAccountConnected = 0 });
                }
            }
        }

        #endregion

        #region Props

        private readonly string _dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "taskban-db.sqlite");

        private SQLiteConnection DbConnection => new SQLiteConnection(new SQLitePlatformWinRT(), _dbPath);

        #endregion

        #region CRUD

        #region Generic

        /// <summary>
        /// For each entity, if it exists then their record is updated, 
        /// otherwise the entity will be inserted into the db.
        /// </summary>
        /// <typeparam name="T">The entity type to insert, must be of DbEntity.</typeparam>
        /// <param name="entities">The IList of entities to insert.</param>
        public bool InsertOrUpdateList<T>(params T[] entities) where T : DbEntity
        {
            var changed = false;

            foreach (var entity in entities)
            {
                if (!string.IsNullOrEmpty(entity.Id) && GetEntityById<T>(entity.Id) != null)
                {
                    UpdateEntity(entity);
                    changed = true;
                }
                else
                {
                    InsertEntity(entity);
                    changed = true;
                }
            }

            return changed;
        }

        public T InsertOrUpdateEntity<T>(T entity) where T : DbEntity
        {
            if (!string.IsNullOrEmpty(entity.Id) && GetEntityById<T>(entity.Id) != null)
            {
                return UpdateEntity(entity);
            }

            return InsertEntity(entity);
        }

        public T InsertEntity<T>(T entity) where T : DbEntity
        {
            using (var db = DbConnection)
            {
                if (entity.DateCreated == null)
                {
                    entity.DateCreated = DateTime.Now;
                }

                if (entity.LastModified == null)
                {
                    entity.LastModified = DateTime.Now;
                }

                if (string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }

                db.Insert(entity);
                return entity;
            }
        }

        public T GetEntityById<T>(string id) where T : DbEntity
        {
            using (var db = DbConnection)
            {
                return db.Find<T>(id);
            }
        }

        public T UpdateEntity<T>(T entity) where T : DbEntity
        {
            using (var db = DbConnection)
            {
                entity.LastModified = DateTime.Now;
                db.Update(entity);
                return entity;
            }
        }

        public void DeleteEntity<T>(T entity) where T : DbEntity
        {
            using (var db = DbConnection)
            {
                db.Delete(entity);
            }
        }

        #endregion

        #region User

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public User GetUser()
        {
            using (var db = DbConnection)
            {
                return db.Table<User>().FirstOrDefault();
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(User user)
        {
            using (var db = DbConnection)
            {
                db.Update(user);
            }
        }

        #endregion

        #region Notebook

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public List<Notebook> GetNotebooks()
        {
            using (var db = DbConnection)
            {
                return db.Query<Notebook>("Select * from Notebook");
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="notebooks"></param>
        public void InsertOrUpdateNotebooks(IList<Notebook> notebooks)
        {
            DeleteNotebooks(notebooks.Where(n => !n.ImportIntoDb).ToList());
            using (var db = DbConnection)
            {
                foreach (var nb in notebooks.Where(n => n.ImportIntoDb))
                {
                    if (db.Find<Notebook>(nb.Id) != null)
                    {
                        db.Update(nb);
                    }
                    else
                    {
                        db.Insert(nb);
                    }
                }
                //db.InsertOrReplaceAll(notebooks.Where(n => n.ImportIntoDb));
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="notebooks"></param>
        public void DeleteNotebooks(IList<Notebook> notebooks)
        {
            using (var db = DbConnection)
            {
                foreach (var nb in notebooks)
                {
                    db.Delete<Notebook>(nb.Id);
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public List<Section> GetSections()
        {
            using (var db = DbConnection)
            {
                return db.Query<Section>("Select * from Section");
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public List<Page> GetPages()
        {
            using (var db = DbConnection)
            {
                return db.Query<Page>("Select * from Page");
            }
        }

        #endregion

        #region Pages

        public void InsertOrReplacePageContents(IList<PageContent> pageContents)
        {
            using (var db = DbConnection)
            {
                foreach (var pageContent in pageContents)
                {
                    if (db.Find<PageContent>(pageContent.Id) != null)
                    {
                        db.Update(pageContent);
                    }
                    else
                    {
                        db.Insert(pageContent);
                    }
                }
            }
        }

        public List<PageContent> GetPageContentsForPages(IList<Page> pages)
        {
            var contents = new List<PageContent>();
            using (var db = DbConnection)
            {
                var query = "Select * from PageContent where PageId = ?";
                foreach (var page in pages)
                {
                    contents.AddRange(db.Query<PageContent>(query, page.Id));
                }
            }

            return contents;
        }

        public List<PageContent> GetAllPageContents()
        {
            using (var db = DbConnection)
            {
                return db.Query<PageContent>("Select * from PageContent");
            }
        }


        #endregion

        #region Projects

        public List<Project> GetAllProjects()
        {
            using (var db = DbConnection)
            {
                return db.Query<Project>("Select * from Project");
            }
        }

        /// <summary>
        /// Deletes a project and all of its references from the database. 
        /// References: boards, tasklists, tasks
        /// </summary>
        /// <param name="project">The project to delete</param>
        public void DeleteProjectAndReferences(Project project)
        {
            using (var db = DbConnection)
            {
                // get all boards for this project
                var boards = GetAllBoards(project.Id);

                // for each board, delete it and its references
                foreach (var board in boards)
                    DeleteBoardAndReferences(board);

                // then delete the project - do this last
                db.Delete(project);
            }
        }

        #endregion

        #region Boards

        public List<Board> GetAllBoards(string projectId)
        {
            using (var db = DbConnection)
            {
                return db.Query<Board>("Select * from Board where ProjectId = ?", projectId);
            }
        }

        public void DeleteBoardAndReferences(Board board)
        {
            using (var db = DbConnection)
            {
                var lists = GetAllTaskLists(board.Id);
                var tasks = GetAllTaskItemsInLists(lists.Select(l => l.Id).ToList());

                foreach (var task in tasks) db.Delete(task);
                foreach (var list in lists) db.Delete(list);

                db.Delete(board);
            }
        }

        #endregion

        #region TaskLists

        public List<TaskList> GetAllTaskLists(string boardId)
        {
            using (var db = DbConnection)
                return (from l in db.Table<TaskList>()
                        where l.BoardId == boardId
                        select l).ToList();
        }

        #endregion

        #region Tasks

        public List<TaskItem> GetAllTaskItems()
        {
            using (var db = DbConnection)
            {
                return db.Query<TaskItem>("Select * from TaskItem");
            }
        }

        public List<TaskItem> GetAllTaskItemsInLists(List<string> taskListIds)
        {
            var tasks = new List<TaskItem>();
            foreach (var id in taskListIds)
                tasks.AddRange(GetAllTaskItemsInList(id));
            return tasks;
        }

        public List<TaskItem> GetAllTaskItemsInList(string taskListId)
        {
            using (var db = DbConnection)
            {
                return db.Query<TaskItem>("Select * from TaskItem where TaskListId = ? ", taskListId);
            }
        }

        public List<ChecklistItem> GetChecklistItemsInTaskItem(string taskItemId)
        {
            using (var db = DbConnection)
            {
                return (from c in db.Table<ChecklistItem>()
                        where c.TaskItemId == taskItemId
                        orderby c.Order
                        select c).ToList();
            }
        }

        public void DeleteTaskItemAndReferences(TaskItem taskItem)
        {
            foreach (var cItem in GetChecklistItemsInTaskItem(taskItem.Id))
                DeleteEntity(cItem);

            DeleteEntity(taskItem);
        }

        /// <summary>
        /// Deletes all task items belonging to this list, then deletes the task list.
        /// </summary>
        /// <param name="taskList">The TaskList record to delete.</param>
        public void DeleteTaskListAndReferences(TaskList taskList, bool deleteTaskList = true)
        {
            foreach (var taskItem in GetAllTaskItemsInList(taskList.Id))
            {
                DeleteChecklistItems(taskItem.Id);
                DeleteEntity(taskItem);
            }

            if (deleteTaskList)
            {
                DeleteEntity(taskList);
            }
        }

        public void DeleteChecklistItems(string taskItemId)
        {
            foreach (var checkList in GetChecklistItemsInTaskItem(taskItemId))
                DeleteEntity(checkList);
        }

        #endregion


        #endregion


        // TODO: temp testing for calendar

        public List<TaskItem> GetTasksWithDueDates()
        {
            using (var db = DbConnection)
            {
                return db.Query<TaskItem>("Select * from TaskItem where DateCreated is not null");
            }
        }
    }


}
