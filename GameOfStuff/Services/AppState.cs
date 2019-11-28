using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfStuff.Services
{
    public class AppState
    {
        public string AlertBody { get; set; }
        public bool AlertShow { get; set; } = false;

        public event Action OnAlertChange;

        public void ExceptionAlert(string title, string body, bool show)
        {
            AlertBody = body;
            AlertShow = show;
            NotifyStateChanged();
        }

        public void ClearException()
        {
            AlertBody = null;
            AlertShow = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnAlertChange?.Invoke();

    }
}
