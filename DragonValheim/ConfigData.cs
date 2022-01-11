using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonValheim
{
    class ConfigData
    {
        private string section = null;
        private string key = null;
        private string value = null;
        private string description = null;

        public string Section
        {
            get => section;
            set => section = value;
        }
        public string Key
        {
            get => key;
            set => key = value;
        }
        public string Value
        {
            get => value;
            set => this.value = value;
        }
        public string Description
        {
            get => description;
            set => description = value;
        }

        public class ConfigJsonData
        {
            public List<ConfigData> configDataBase = new List<ConfigData>();
        }
    }
}
