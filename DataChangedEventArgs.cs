using System;
using System.Collections.Generic;
using System.Text;

namespace Lab3
{
    class DataChangedEventArgs
    {
        public ChangeInfo changeInfo { get; set; }
        public string eventInfo { get; set; }

        public DataChangedEventArgs(ChangeInfo changeI, string eventI)
        {
            changeInfo = changeI;
            eventInfo = eventI;
        }

        public override string ToString()
        {
            return $"Change type: {changeInfo}. Change information: {eventInfo}\n";
        }
    }
}
