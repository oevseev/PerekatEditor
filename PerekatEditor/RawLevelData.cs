using System;
using System.Collections.Generic;

namespace PerekatEditor
{
    public class RawLevelData
    {
        public class Size
        {
            public int width { get; set; }
            public int height { get; set; }
        }

        public class Coords
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class RawLevelObject
        {
            public string type { get; set; }
            public Coords position { get; set; }
        }

        public class RawSpawnInfo
        {
            public string type { get; set; }
            public Coords position { get; set; }
        }

        public class RawListener : RawLevelObject
        {
            public string direction { get; set; }
            public string size { get; set; }
        }

        public class RawEntity : RawListener
        {
            public Coords first { get; set; }
            public Coords second { get; set; }
        }

        public class RawArea : RawLevelObject
        {
            public double width { get; set; }
            public double height { get; set; }
            public bool hermetic { get; set; }
        }

        public Size size { get; set; }
        public RawSpawnInfo spawnInfo { get; set; }
        public IList<RawListener> listeners { get; set; }
        public IList<RawEntity> entities { get; set; }
        public IList<RawArea> areas { get; set; }
        public IList<IList<int>> blocks { get; set; }
    }
}
