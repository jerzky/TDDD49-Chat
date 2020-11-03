using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using ChatApp.Annotations;
using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly ConversationHistory _history;
        private List<Conversation> _conversations;
        private Conversation _selectedConversation;
        private string _searchText;

        public List<Conversation> Conversations
        {
            get => _conversations;
            private set
            {
                if (value.Equals(_conversations))
                    return;

                _conversations = value;
                OnPropertyChanged();
            }
        }

        public Conversation SelectedConversation
        {
            get => _selectedConversation;
            set
            {
                if (value == null || value.Equals(_selectedConversation))
                    return;


        
                _selectedConversation = value;
                OnPropertyChanged();
     
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value.Equals(_searchText))
                    return;
                _searchText = value;
                // search funkar inte av någon anledning.
                OnPropertyChanged();
                Conversations = _history.Conversations.FindAll(c => c.Username.ToLower().Contains(SearchText.ToLower()));
            }
        }


        public HistoryViewModel(ConversationHistory history)
        {
            _history = history;
            Conversations = _history.Conversations;
        }

        public void Save()
        {
            ConversationHistory.Save(_history, "history.json");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
