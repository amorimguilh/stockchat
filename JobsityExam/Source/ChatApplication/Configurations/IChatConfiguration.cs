using System.Collections.Generic;

namespace ChatApplication.Configurations
{
    public interface IChatConfiguration
    {
        public HashSet<string> AllowedCommands { get; }
    }
}
