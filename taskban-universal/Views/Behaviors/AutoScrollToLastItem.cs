using System.Collections.Specialized;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace TamedTasks.Views.Behaviors
{
    public class AutoScrollToLastItem : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged += collection_CollectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            var collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (collection != null)
                collection.CollectionChanged -= collection_CollectionChanged;
        }

        private void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ScrollToLastItem();
            }
        }

        private void ScrollToLastItem()
        {
            if (AssociatedObject.Items == null) return;

            var count = AssociatedObject.Items.Count;
            if (count <= 0) return;

            var last = AssociatedObject.Items[count - 1];
            AssociatedObject.ScrollIntoView(last);
        }
    }
}
