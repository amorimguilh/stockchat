using System.Collections.Generic;

namespace ChatApplication.Configurations
{
    public class ChatConfiguration : IChatConfiguration
    {
        private readonly static HashSet<string> _allowedCommands = new HashSet<string> { "/STOCK" };

        public HashSet<string> AllowedCommands 
        { 
            get
            {
                return _allowedCommands;
            }
        }
    }
}
