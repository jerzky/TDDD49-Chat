using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ChatApp.Models
{
    public class Conversation
    {
        public DateTime Date { get; set; }
        [DefaultValue(" ")]
        public string Username { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
