using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace abiturient_forms
{
    class Direction
    {
        public int id_direction { get; set; }
        public string direction_name { get; set; }
        public int treeNode { get; set; }
        public List<Profile> profiles = new List<Profile>();
    }
}
