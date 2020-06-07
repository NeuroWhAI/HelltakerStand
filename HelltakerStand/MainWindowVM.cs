using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HelltakerStand
{
    class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindowVM()
        {
            SyncCommand = new ActionCommand(OnSync);
            StandCommand = new ActionCommand((name) => OnStand(name as string));
        }

        public delegate IStand StandFactoryDelegate(string name);
        public StandFactoryDelegate StandFactory { get; set; } = null;

        public ActionCommand SyncCommand { get; private set; }
        public ActionCommand StandCommand { get; private set; }
        public ObservableCollection<string> StandList { get; private set; } = new ObservableCollection<string>();

        private readonly string OptionFile = "data.txt";
        private StandOption Option = new StandOption();
        private List<IStand> StandsOpened = new List<IStand>();

        public void LoadStands()
        {
            StandList.Clear();

            if (Directory.Exists("Resources"))
            {
                foreach (string standFile in Directory.EnumerateFiles("Resources", "*.gif"))
                {
                    StandList.Add(Path.GetFileNameWithoutExtension(standFile));
                }
            }

            StandsOpened.Clear();
            Option.Load(OptionFile);

            foreach (StandData data in Option.Stands)
            {
                IStand stand = StandFactory(data.Name);
                if (stand == null)
                {
                    continue;
                }

                stand.StandX = data.X;
                stand.StandY = data.Y;
                stand.StandTopmost = data.Topmost;
                stand.StandRemoved += () =>
                {
                    StandsOpened.Remove(stand);
                    SaveStandsOpened();
                };
                stand.StandChanged += SaveStandsOpened;

                StandsOpened.Add(stand);
            }

            OnSync();
        }

        private void OnSync()
        {
            foreach (var stand in StandsOpened)
            {
                stand.ResetStandTime();
            }
        }

        private void OnStand(string name)
        {
            IStand stand = StandFactory(name);
            if (stand == null)
            {
                return;
            }

            stand.StandRemoved += () =>
            {
                StandsOpened.Remove(stand);
                SaveStandsOpened();
            };
            stand.StandChanged += SaveStandsOpened;

            StandsOpened.Add(stand);

            SaveStandsOpened();

            OnSync();
        }

        private void SaveStandsOpened()
        {
            var stands = Option.Stands;
            stands.Clear();

            foreach (var stand in StandsOpened)
            {
                StandData data;
                data.Name = stand.StandName;
                data.X = stand.StandX;
                data.Y = stand.StandY;
                data.Topmost = stand.StandTopmost;

                stands.Add(data);
            }

            Option.Save(OptionFile);
        }
    }
}
