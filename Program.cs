using System;
using System.Numerics;

namespace Lab3
{
    enum ChangeInfo
    {
        ItemChanged,
        Add,
        Remove,
        Replace
    }

    struct DataItem // структура для хранения значения поля в точке
    {
        public Vector2 Coord { get; set; }
        public double EMField { get; set; }

        public DataItem(Vector2 coord, double field)
        {
            Coord = coord;
            EMField = field;
        }

        public override string ToString()
        {
            return $"EM field at the point ({Coord.X}, {Coord.Y}) is {EMField}.\n";
        }

        public string ToString(string format)
        {
            string CoordXFormatted = String.Format(format, Coord.X);
            string CoordYFormatted = String.Format(format, Coord.Y);
            string EMFieldFormatted = String.Format(format, EMField);
            return $"EM field at the point ({CoordXFormatted}; {CoordYFormatted}) is {EMFieldFormatted}.\n";
        }
    }

    struct Grid1D // структура для хранения параметров сетки по одной оси
    {
        public float AxisStep { get; set; }
        public int NodesCount { get; set; }

        public Grid1D(float step, int count)
        {
            AxisStep = step;
            NodesCount = count;
        }

        public override string ToString()
        {
            return $"Axis step is {AxisStep}. There are {NodesCount} nodes on this axis.";
        }

        public string ToString(string format)
        {
            string AxisStepFormatted = String.Format(format, AxisStep);
            return $"Axis step is {AxisStepFormatted}. There are {NodesCount} nodes on this axis.";
        }
    }

    delegate void DataChangedEventHandler(object source, DataChangedEventArgs args);

    class Program
    {
        static void Main()
        {
            try
            {
                V3MainCollection ourCollection = new V3MainCollection();
                ourCollection.DataChanged += DataChangedEventAction;

                Grid1D XGrid1 = new Grid1D(150.0f, 3);
                Grid1D YGrid1 = new Grid1D(180.0f, 3);
                DateTime date1 = new DateTime(2020, 12, 19, 20, 30, 50);
                V3DataOnGrid Grid1 = new V3DataOnGrid("Grid 1", date1, XGrid1, YGrid1);
                Grid1.InitRandom(50.0, 150.0);
                V3DataCollection Collection1 = new V3DataCollection("Collection 1", DateTime.Now);
                Collection1.InitRandom(6, 70.0f, 30.0f, 300.0, 450.0);
                V3DataCollection Collection2 = new V3DataCollection("Collection 2", DateTime.Now);
                Collection2.InitRandom(10, 100.0f, 180.0f, 1500.0, 2000.0);

                ourCollection.Add(Grid1); // ChangeInfo.Add
                ourCollection.Add(Collection1); // ChangeInfo.Add
                ourCollection[1] = Collection2; // ChangeInfo.Replace
                Grid1.Measures = "Grid 1: changed measures"; // ChangeInfo.ItemChanged
                ourCollection.Remove("Grid 1: changed measures", date1); // ChangeInfo.Remove
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // обработчик события DataChanged
        private static void DataChangedEventAction(object source, DataChangedEventArgs args)
        {
            Console.WriteLine($"{args.ToString()}");
        }
    }
}
