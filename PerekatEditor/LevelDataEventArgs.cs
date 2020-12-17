using System;
using System.Collections.Generic;

namespace PerekatEditor
{
    class LevelDataEventArgs : EventArgs
    {
        public LevelData Data { get; private set; }

        public bool NeedsReload { get; set; }
        public bool NeedsResize { get; set; }
        public IList<string> ObjectsToUpdate { get; set; }

        public LevelDataEventArgs(LevelData levelData)
        {
            Data = levelData;
            ObjectsToUpdate = new List<string>();
        }
    }
}
