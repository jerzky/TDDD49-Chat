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

        public Message(string text, string username = "System")
        {
            Time = DateTime.Now.ToString();
            Text = text;
            Username = username;
        }
        public Message(string text, Brush color, string username = "System")
        {
            Time = DateTime.Now.ToString();
            Text = text;
            Username = username;
            Color = color;
        }
    }
}
