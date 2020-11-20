using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace Texturer
{
    public class MenuObject : INotifyPropertyChanged
    {
        private TextureObject textureObject;
        private int posX, posY;
        private float streach;

        public MenuObject(TextureObject texture)
        {
            textureObject = texture;
            posX = 0;
            posY = 0;
            streach = 1.0f;
        }
        public int PosX { get=>posX; set=>posX=value; }
        public int PosY { get=>posY; set=>posY=value; }
        public TextureObject TextureObject { get=>textureObject; set=>textureObject=value; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
