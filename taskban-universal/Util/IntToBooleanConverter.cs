using System;
using Windows.UI.Xaml.Data;
using TamedTasks.Models.Common;

namespace TamedTasks.Util
{
    public class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (TaskItem.Progress) value == TaskItem.Progress.Complete;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
