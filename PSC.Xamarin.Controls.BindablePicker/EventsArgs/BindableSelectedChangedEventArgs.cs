using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC.Xamarin.Controls.BindablePicker.EventsArgs {
    public class BindableSelectedChangedEventArgs : EventArgs {
        public int Index { get; set; } = 0;
        public object Item { get; set; } = null;
    }
}
