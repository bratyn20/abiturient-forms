using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace abiturient_forms
{
    class UserDirection
    {
        public int id_user { get; set; }
        public int id_direction { get; set; }
        public int direction_priority { get; set; }

        public List<UserProfile> userProfiles = new List<UserProfile>();
    }
}
