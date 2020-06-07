using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace HelltakerStand
{
    /// <summary>
    /// StandWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StandWindow : Window, IStand
    {
        public StandWindow()
        {
            InitializeComponent();
        }

        public event Action StandRemoved;
        public event Action StandChanged;

        public string StandName { get; set; } = string.Empty;
        public int StandX
        {
            get => (int)Left;
            set => Left = value;
        }
        public int StandY
        {
            get => (int)Top;
            set => Top = value;
        }
        public bool StandTopmost
        {
            get => Topmost;
            set
            {
                if (Topmost != value)
                {
                    Topmost = value;
                    chkTopmost.IsChecked = value;
                }
            }
        }

        public void ResetStandTime()
        {
            ImageBehavior.GetAnimationController(imgStand).GotoFrame(0);
        }

        public void LoadStand(string name)
        {
            StandName = name;

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new FileStream($"Resources/{name}.gif", FileMode.Open, FileAccess.Read, FileShare.Read);
            image.EndInit();

            ImageBehavior.SetAnimatedSource(imgStand, image);
        }

        private void ImgStand_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void ImgStand_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                StandChanged?.Invoke();
            }
        }

        private void ChkTopmost_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (Topmost != chkTopmost.IsChecked)
            {
                Topmost = chkTopmost.IsChecked;
                StandChanged?.Invoke();
            }
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            StandRemoved?.Invoke();
            Close();
        }
    }
}
