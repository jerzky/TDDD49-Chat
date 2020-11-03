using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using ChatApp.Network.Packets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ChatApp.Models
{
    public class ConversationHistory
    {
        public List<Conversation> Conversations { get; private set; } = new List<Conversation>();

        public static void Save(ConversationHistory history, string path)
        {
         
            var json = JsonConvert.SerializeObject(history, Formatting.Indented);
            File.WriteAllText(path, json);
        }


        public static ConversationHistory Load(string path)
        {
            if(!File.Exists(path))
                return new ConversationHistory();
            var json = File.ReadAllText(path);
            try
            {
                var history = JsonConvert.DeserializeObject<ConversationHistory>(json);
                history.Conversations.Sort((a, b) => a.Date.CompareTo(b.Date));
                return history;
            }
            catch (JsonException e)
            {
                 MessageBox.Show($"File: {path} is not a valid conversation history file.");
                 MessageBox.Show(e.ToString());
            }
            return new ConversationHistory();
        }

    }

}
