using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPoart
{

    public class hstoryCombox
    {
        public List<string> file1 = new List<string>();
        public List<string> file2 = new List<string>();
        public List<string> file3 = new List<string>();
    }
    public class initList
    {
        public string name;
        public List<int> index = new List<int>();
        public List<CheckBox> hex = new List<CheckBox>();
        public List<CheckBox> select = new List<CheckBox>();
        public List<TextBox> address = new List<TextBox>();
        public List<TextBox> data = new List<TextBox>();
        public List<TextBox> info = new List<TextBox>();
    }
    public class initListinfo
    {
        public string name;
        public List<int> index = new List<int>();
        public List<bool> hex = new List<bool>();
        public List<bool> select = new List<bool>();
        public List<string> address = new List<string>();
        public List<string> data = new List<string>();
        public List<string> info = new List<string>();
    }
    public class Downinfo
    {
        public int initIndex;
        public bool[] checkbox = new bool[3];
        public string[] filePath = new string[3];
    }

}
