using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameOfStuff.Services
{
    public class AppState
    {
        public string ExceptionBody { get; set; }
        public bool ExceptionShow { get; set; } = false;

        public event Action OnExceptionChange;

        public void ExceptionAlert(string title, string body, bool show)
        {
            ExceptionBody = body;
            ExceptionShow = show;
            NotifyStateChanged();
        }

        public void ClearException()
        {
            ExceptionBody = null;
            ExceptionShow = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnExceptionChange?.Invoke();

    }
}
