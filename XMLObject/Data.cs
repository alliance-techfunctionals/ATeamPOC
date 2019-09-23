using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLObject
{
    public class Data
    {

        public string Name { get; set; }
        public string DisplayName  { get; set; }
        public string NodePath { get; set; }
        public int Childrens { get; set; }
        public string NodeType { get; set; }
        public List<Data> Children { get; set; }
    }

   
}
