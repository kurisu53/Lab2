using System;
using System.Numerics;
using System.ComponentModel;

namespace Lab3
{
    abstract class V3Data : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
        }

        private string measures;
        public string Measures
        {
            get
            {
                return measures;
            }
            set
            {
                measures = value;
                OnPropertyChanged("Measures");
            }
        }

        private DateTime measureTime;
        public DateTime MeasureTime
        {
            get
            {
                return measureTime;
            }
            set
            {
                measureTime = value;
                OnPropertyChanged("MeasureTime");
            }
        }

        public V3Data(string measures, DateTime time)
        {
            Measures = measures;
            MeasureTime = time;
        }

        public V3Data()
        {
            Measures = "Default measures";
            MeasureTime = DateTime.Now;
        }

        public abstract Vector2[] Nearest(Vector2 v);
        public abstract string ToLongString();
        public abstract string ToLongString(string format);

        public override string ToString()
        {
            return $"Measures: {Measures}. Measurement time: {MeasureTime}.";
        }

    }
}
