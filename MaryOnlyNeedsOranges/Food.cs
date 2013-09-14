using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaryOnlyNeedsOranges
{
    class Food
    {
        public int size { get; set; }
        public int speed { get; set; }
        public int points { get; set; }
        public int line { get; set; }
        public int row { get; set; }
        public Type type { get; set; }
        public int green { get; set; }
        public int red { get; set; }
        public int blue { get; set; }

        public enum Type { Normal, Orange, Blueberry, Lemon, Apple, Grapes };
    }
}
