using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.CSharp.RuntimeBinder;
using TamedTasks.ViewModels.Pages;

namespace TamedTasks.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        public ChatPage()
        {
            this.InitializeComponent();
        }

        // hack to allow the enter key to submit chat message
        private void OnMessageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;

            try
            {
                dynamic obj = sender;
                var vm = obj.DataContext as ChatPageViewModel;
                if (vm == null) return;

                vm.SubmitChatMessage();
            }
            catch (RuntimeBinderException ex)
            {
                Debug.WriteLine($"chat message ex: {ex}");
            }
        }
        
    }
}
