using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ChatApp.Models
{
    public class Message
    {
        public string Username { get; private set; }
        public string Text { get; private set; }
        public string Time { get; private set; }
        public Brush Color { get; private set; } = Brushes.Black;
        public override string ToString()
        {
            return $"[{Time}] <{Username}> {Text}";
        }

        public Message()
        {
            Time = "N/A";
            Username = "N/A";
        }

        public Message(string text, Brush color = null, string username = "System")
        {
            Time = DateTime.Now.ToString();
            Text = text;
            Username = username;
            if(color != null) Color = color; 
        }
    }
}
