using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Texturer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TextureObject> TextureObjects { get; set; }
        public ObservableCollection<MenuObject> MenuObjects { get; set; }
        private Image image;
        private TextureObject selectedTextureObject;
        Point prevPrevPos = new Point();
        Point prevPos = new Point();
        string format = "image";
        private Window _dragdropWindow = null;

        public MainWindow()
        {
            InitializeComponent();

            TextureObjects=new ObservableCollection<TextureObject>();
            MenuObjects = new ObservableCollection<MenuObject>();
            RightPanelLB.ItemsSource = TextureObjects;
        }

        private void RightPanelAddTextureObject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Картинки(*.JPG;*.GIF;*.PNG)|*.JPG;*.GIF;*.PNG" + "|Все файлы (*.*)|*.* ";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = true;
            if (myDialog.ShowDialog() == true)
            {
                foreach (string fiilename in myDialog.FileNames) 
                {
                    TextureObject texobj = new TextureObject();
                    texobj.ImagePath = new Uri(fiilename, UriKind.Absolute);
                    if (texobj.ImagePath.IsFile)
                    {
                        texobj.Name = System.IO.Path.GetFileNameWithoutExtension(texobj.ImagePath.LocalPath);
                    }
                    TextureObjects.Add(texobj);
                }
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            image = e.Source as Image;
            ListBox lb = FindParent<ListBox>(image);
            ListBoxItem lbi = FindParent<ListBoxItem>(image);
            ((ListBoxItem)lbi).IsSelected = true;
            selectedTextureObject = (TextureObject)lb.SelectedItem;

            if (selectedTextureObject != null) textBox.Text = selectedTextureObject.Name;
            else textBox.Text = "NULL";
            //textBox.Text = selectedTextureObject.ImagePath.ToString();
            CreateDragDropWindow(image);
            DataObject data = new DataObject(typeof(ImageSource), image.Source);
            DragDrop.DoDragDrop(image, data, DragDropEffects.Copy);
 
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MainCanvas_Drop(object sender, DragEventArgs e)
        {
            //ImageSource image = e.Data.GetData(typeof(ImageSource)) as ImageSource;
            MenuObject menuObject = new MenuObject(selectedTextureObject);
            MenuObjects.Add(menuObject);

            Image imageControl = new Image() { Width = image.Width, Height = image.Height, Source = new BitmapImage(selectedTextureObject.ImagePath) };
            imageControl.Stretch = 0;

            Canvas.SetLeft(imageControl, e.GetPosition(this.MainCanvas).X);

            Canvas.SetTop(imageControl, e.GetPosition(this.MainCanvas).Y);
            if (this._dragdropWindow != null)
            {
                this._dragdropWindow.Close();
                this._dragdropWindow = null;
            }

            this.MainCanvas.Children.Add(imageControl);

        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            image= e.Source as Image;
            if (image!=null&&image.IsMouseCaptured)
            {   
                Canvas.SetLeft(image, e.GetPosition(MainCanvas).X);
                Canvas.SetTop(image, e.GetPosition(MainCanvas).Y);
                prevPrevPos = prevPos;
                prevPos = e.GetPosition(MainCanvas);
                DataObject data = new DataObject(typeof(ImageSource), image.Source);
                DragDrop.DoDragDrop(image, data, DragDropEffects.Move);
            }
        }

        private void RightPanelLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTextureObject = (TextureObject)RightPanelLB.SelectedItem;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

 
        private void Image_DragOver(object sender, DragEventArgs e)
        {
            if (image != null && image.IsMouseCaptured)
            {
                Canvas.SetLeft(image, e.GetPosition(MainCanvas).X);
                Canvas.SetTop(image, e.GetPosition(MainCanvas).Y);
                prevPrevPos = prevPos;
                prevPos = e.GetPosition(MainCanvas);
                DataObject data = new DataObject(typeof(ImageSource), image.Source);

                DragDrop.DoDragDrop(image, data, DragDropEffects.Move);
            }

        }

        private void MainCanvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(format))
                e.Effects = DragDropEffects.Move;

        }

        private void CreateDragDropWindow(Image dragElement)
        {
            this._dragdropWindow = new Window();
            _dragdropWindow.WindowStyle = WindowStyle.None;
            _dragdropWindow.AllowsTransparency = true;
            _dragdropWindow.AllowDrop = false;
            _dragdropWindow.Background = null;
            _dragdropWindow.IsHitTestVisible = false;
            _dragdropWindow.SizeToContent = SizeToContent.WidthAndHeight;
            _dragdropWindow.Topmost = true;
            _dragdropWindow.ShowInTaskbar = false;
            _dragdropWindow.Opacity = 0.5f;
            

            Rectangle r = new Rectangle();
            Image imageControl = new Image() { Width = image.Width, Height = image.Height, Source = new BitmapImage(selectedTextureObject.ImagePath) };
            imageControl.Stretch = 0;
            imageControl.SnapsToDevicePixels=false;
            
            r.Width = image.Width;
            r.Height = image.Height;
            r.Fill = new VisualBrush(dragElement);
           
            this._dragdropWindow.Content = r;


            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);


            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;
            this._dragdropWindow.Show();
        }

        private void Image_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this._dragdropWindow.Left = w32Mouse.X;
            this._dragdropWindow.Top = w32Mouse.Y;

        }



        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
    }
}
