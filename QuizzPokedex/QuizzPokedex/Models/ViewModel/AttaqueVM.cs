using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models.ViewModel
{
    public class AttaqueVM
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Power { get; set; }

        public string Precision { get; set; }

        public string PP { get; set; }

        public string Level { get; set; }

        public string CTCS { get; set; }

        public TypeAttaque TypeAttaque { get; set; }

        public TypePok TypePok { get; set; }
    }
}
