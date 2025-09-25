﻿using System;

namespace BotCore.States.BotStates
{
    public class StateMetaInfo : Attribute
    {
        public string Version { get; set; }
        public string DateUpdated { get; set; }

        public StateMetaInfo(string Version, string DateUpdated)
        {
            this.Version = Version;
            this.DateUpdated = DateUpdated;
        }
    }

    public class StateAttribute : Attribute
    {
        public string Desc { get; set; }
        public string Author { get; set; }
        public int DefaultPriority { get; set; }

        public StateAttribute(string Author, string Desc, int DefaultPriority = 50)
        {
            this.Author = Author;
            this.Desc = Desc;
            this.DefaultPriority = DefaultPriority;
        }
    }
}
