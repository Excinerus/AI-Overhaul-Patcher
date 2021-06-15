using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIOverhaulPatcher.Settings
{
    class Settings
    {
        public bool IgnoreIdenticalToLastOverride { get; set; } = false;
        public bool IgnorePlayerRecord { get; set; } = true;
        public bool MaintainHighestProtectionLevel { get; set; } = true;
    }
}
