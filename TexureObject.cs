using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Texturer
{
    public class TextureObject : INotifyPropertyChanged
    {
        private string name;
        private Uri imagepath;
        public string Name { get => name; set => name = value; }
        public Uri ImagePath { get => imagepath; set => imagepath = value; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
