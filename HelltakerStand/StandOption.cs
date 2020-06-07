using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelltakerStand
{
    class StandOption : OptionBase
    {
        public int StandCount => Stands.Count;
        public List<StandData> Stands = new List<StandData>();

        protected override void AfterLoad()
        {
            Stands.Clear();

            int standCount = GetProperty(nameof(StandCount), (s) => int.Parse(s), 0);

            foreach (int idx in Enumerable.Range(0, standCount))
            {
                StandData data;
                data.Name = GetProperty($"StandName{idx}", (s) => s);
                data.X = GetProperty($"StandX{idx}", (s) => int.Parse(s), 0);
                data.Y = GetProperty($"StandY{idx}", (s) => int.Parse(s), 0);
                data.Topmost = GetProperty($"StandTopmost{idx}", (s) => bool.Parse(s), false);

                if (!string.IsNullOrEmpty(data.Name))
                {
                    Stands.Add(data);
                }
            }
        }

        protected override void BeforeSave()
        {
            SetProperty(nameof(StandCount), StandCount);

            foreach (int idx in Enumerable.Range(0, StandCount))
            {
                var data = Stands[idx];

                SetProperty($"StandName{idx}", data.Name);
                SetProperty($"StandX{idx}", data.X);
                SetProperty($"StandY{idx}", data.Y);
                SetProperty($"StandTopmost{idx}", data.Topmost);
            }
        }
    }
}
