using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TamedTasks.Views.Controls
{
    public class TaskItemWebView
    {
        public static readonly DependencyProperty HtmlStringProperty =
           DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(TaskItemWebView), 
               new PropertyMetadata("", OnHtmlStringChanged));
        public static string GetHtmlString(WebView obj) { return (string)obj.GetValue(HtmlStringProperty); }
        public static void SetHtmlString(WebView obj, string value) { obj.SetValue(HtmlStringProperty, value); }

        private static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wv = d as WebView;
            if (wv == null) return;

            var html = e.NewValue.ToString();
            if (string.IsNullOrEmpty(html)) return;

            var derp = string.Format("<div>{0}</div>", html);
            

            wv.NavigateToString(derp);
        }




    }
}
