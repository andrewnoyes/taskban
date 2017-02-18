using System.Collections.ObjectModel;
using Template10.Mvvm;

namespace TamedTasks.ViewModels.Pages
{
    public class ChatPageViewModel : ViewModelBase
    {
        private ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();
        public ObservableCollection<ChatMessage> ChatMessages
        {
            get { return _chatMessages; }
            set { Set(ref _chatMessages, value); }
        }

        private string _chatMessage;
        public string ChatMessage
        {
            get { return _chatMessage; }
            set { Set(ref _chatMessage, value); }
        }

        public void SubmitChatMessage()
        {
            if (string.IsNullOrEmpty(ChatMessage)) return;

            ChatMessages.Add(new ChatMessage { Message = ChatMessage });
            ChatMessage = string.Empty;
        }
    }

    public class ChatMessage
    {
        public string Message { get; set; }
    }
}
