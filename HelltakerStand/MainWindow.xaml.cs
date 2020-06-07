using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace HelltakerStand
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 트레이 아이콘 메뉴 설정.
            TrayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            TrayIcon.ContextMenu.MenuItems.Add("Open", (s, args) =>
            {
                TrayIcon.Visible = false;
                Show();
                Focus();
            });
            TrayIcon.ContextMenu.MenuItems.Add("Quit", (s, args) =>
            {
                TrayIcon.Visible = true;
                App.Current.Shutdown();
            });

            // 트레이 아이콘 설정.
            TrayIcon.Visible = false;
            TrayIcon.Icon = Properties.Resources.Main;
            TrayIcon.Text = Title;
            TrayIcon.DoubleClick += new EventHandler((sender, arg) =>
            {
                TrayIcon.Visible = false;
                Show();
                Focus();
            });

            if (DataContext is MainWindowVM vm)
            {
                vm.StandFactory = StandFactory;
            }
        }

        private readonly string AppName = "HelltakerStandByNeuroWhAI";
        private System.Windows.Forms.NotifyIcon TrayIcon = new System.Windows.Forms.NotifyIcon();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 시작프로그램 여부 알아내기
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    chkAutorun.IsChecked = (key.GetValue(AppName) != null);
                }
            }
            catch
            { }

            // 자동 숨기기.
            if (chkAutorun.IsChecked == true)
            {
                TrayIcon.Visible = true;
                Hide();
            }

            if (DataContext is MainWindowVM vm)
            {
                vm.LoadStands();
            }

            Task.Factory.StartNew(() =>
            {
                if (UpdateManager.CheckUpdate())
                {
                    var choice = Dispatcher.Invoke(() => MessageBox.Show("There are new updates.\nWould you like to open the download page?",
                        Title,
                        MessageBoxButton.YesNo, MessageBoxImage.Question));

                    if (choice == MessageBoxResult.Yes)
                    {
                        Process.Start("https://neurowhai.tistory.com/398");
                    }
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TrayIcon.Visible)
            {
                TrayIcon.Visible = false;
                App.Current.Shutdown();
            }
            else
            {
                TrayIcon.Visible = true;
                Hide();

                e.Cancel = true;
            }
        }

        private IStand StandFactory(string name)
        {
            var stand = new StandWindow();
            stand.LoadStand(name);

            stand.Show();

            return stand;
        }

        private void ChkAutorun_CheckChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (chkAutorun.IsChecked == true)
                    {
                        key.SetValue(AppName, Process.GetCurrentProcess().MainModule.FileName);
                    }
                    else
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Fail to register!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e)
        {
            TrayIcon.Visible = true;
            Hide();
        }
    }
}
