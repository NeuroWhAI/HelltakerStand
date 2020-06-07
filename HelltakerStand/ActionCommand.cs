using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HelltakerStand
{
    class ActionCommand : ICommand
    {
        public ActionCommand(Action<object> action, bool firstState = true)
        {
            Action = action;
            m_enabled = firstState;
        }

        public ActionCommand(Action action, bool firstState = true)
        {
            Action = (_) => action();
            m_enabled = firstState;
        }

        public Action<object> Action;

        private bool m_enabled;
        public bool Enabled
        {
            get => m_enabled;
            set
            {
                if (value != m_enabled)
                {
                    m_enabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return Enabled;
        }

        public void Execute(object parameter)
        {
            Action?.Invoke(parameter);
        }
    }
}
