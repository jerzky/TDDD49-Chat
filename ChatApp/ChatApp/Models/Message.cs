using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;
using Newtonsoft.Json;

namespace ChatApp.Models
{
    public class Message
    {
        [DefaultValue(" ")]
        public string Username { get;  set; }
        public string Text { get;  set; }
        public string Time { get;  set; }

        public Brush Color { get; set; } = Brushes.Black;
     
        [JsonIgnore]
        public bool IsSystemMessage => Username == "System";

        public override string ToString()
        {
            return $"[{Time}] <{Username}> {Text}";
        }

        public Message()
        {

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
