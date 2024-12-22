using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winforms
{
    public class Sentence
    {
        public List<TimeSpan> Starts { get; set; }
        public List<TimeSpan> Ends { get; set; }
        public string Text { get; set; }
    }
}
