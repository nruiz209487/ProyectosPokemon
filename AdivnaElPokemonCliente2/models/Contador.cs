using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AdivnaElPokemon.models
{
    public class Contador : INotifyPropertyChanged
    {
        private int _seconds = 120;
        public int Seconds { get { return _seconds; } set { _seconds = value; OnPropertyChanged(nameof(Seconds)); } }
        private System.Timers.Timer _timer;


        public Contador()
        {

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerTick;
            _timer.AutoReset = true;
            _timer.Start();
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            Seconds--;
            if (_seconds == 0)
            {
                _timer.Stop();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
