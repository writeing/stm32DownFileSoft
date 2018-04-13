using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPoart
{
    class initList
    {
        public string name;
        public List<int> index = new List<int>();
        public List<CheckBox> hex = new List<CheckBox>();
        public List<CheckBox> select = new List<CheckBox>();
        public List<TextBox> address = new List<TextBox>();
        public List<TextBox> data = new List<TextBox>();
        public List<TextBox> info = new List<TextBox>();
    }
}
