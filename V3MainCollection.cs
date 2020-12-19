using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.ComponentModel;

namespace Lab3
{
    class V3MainCollection : IEnumerable<V3Data>
    {
        private List<V3Data> V3DataItems = new List<V3Data>();
        public event DataChangedEventHandler DataChanged;

        public int Count
        {
            get
            {
                return V3DataItems.Count;
            }
        }

        // индексатор
        public V3Data this[int index]
        {
            get { return V3DataItems[index]; }
            set
            {
                V3DataItems[index] = value;
                if (DataChanged != null)
                    DataChanged(this, new DataChangedEventArgs(ChangeInfo.Replace, $"Item link has been replaced. There are {Count} elements.\n"));
            }
        }

        // свойства с LINQ-запросами
        public int MinMeasuresCount
        {
            get
            {
                IEnumerable<int> gridMeasuresCounts = from data in (from item in V3DataItems
                                                                    where item is V3DataOnGrid
                                                                    select (V3DataOnGrid)item)
                                                      select data.XGrid.NodesCount * data.YGrid.NodesCount;

                IEnumerable<int> collMeasuresCounts = from data in (from item in V3DataItems
                                                                    where item is V3DataCollection
                                                                    select (V3DataCollection)item)
                                                      select data.DataItems.Count();

                IEnumerable<int> MeasuresCounts = gridMeasuresCounts.Union(collMeasuresCounts);

                return MeasuresCounts.Min();
            }
        }

        public float MaxDistance
        {
            get
            {
                IEnumerable<V3DataCollection> grids = from data in (from item in V3DataItems
                                                                    where item is V3DataOnGrid
                                                                    select (V3DataOnGrid)item)
                                                      select (V3DataCollection)data;

                IEnumerable<V3DataCollection> collections = from data in (from item in V3DataItems
                                                                          where item is V3DataCollection
                                                                          select (V3DataCollection)item)
                                                            select data;

                IEnumerable<V3DataCollection> items = grids.Union(collections);

                IEnumerable<Vector2> Coords = from data in items
                                              from elem in data
                                              select elem.Coord;

                float result = (from coord1 in Coords
                                select (from coord2 in Coords
                                        select Vector2.Distance(coord1, coord2)).Max()).Max();

                return result;
            }
        }

        public IEnumerable<DataItem> Duplicates
        {
            get
            {
                IEnumerable<V3DataCollection> grids = from data in (from item in V3DataItems
                                                                    where item is V3DataOnGrid
                                                                    select (V3DataOnGrid)item)
                                                      select (V3DataCollection)data;

                IEnumerable<V3DataCollection> collections = from data in (from item in V3DataItems
                                                                          where item is V3DataCollection
                                                                          select (V3DataCollection)item)
                                                            select data;

                IEnumerable<V3DataCollection> items = grids.Union(collections);

                var groups = from g in (from data in items
                                        from elem in data
                                        group elem by elem.Coord)
                             where g.Count() > 1
                             select g;

                IEnumerable<DataItem> result = from DataItem item in (from g in groups
                                                                      select g.ToList())
                                               select item;

                return result;
            }
        }

        // реализация интерфейса IEnumerable<V3Data>
        public IEnumerator<V3Data> GetEnumerator()
        {
            return V3DataItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(V3Data item)
        {
            V3DataItems.Add(item);
            if (DataChanged != null)
                DataChanged(this, new DataChangedEventArgs(ChangeInfo.Add, $"A new item has been added. Before: {Count - 1} elements. After: {Count} elements.\n"));
            item.PropertyChanged += PropertyChangedEventAction;
        }

        public bool Remove(string id, DateTime date)
        {
            bool flag = false;
            foreach (V3Data element in V3DataItems.ToList())
            {
                if (element.Measures == id && element.MeasureTime == date)
                {
                    element.PropertyChanged -= PropertyChangedEventAction;
                    V3DataItems.Remove(element);
                    if (DataChanged != null)
                        DataChanged(this, new DataChangedEventArgs(ChangeInfo.Remove, $"An item has been removed. Before: {Count + 1} elements. After: {Count} elements.\n"));
                    flag = true;
                }
            }
            return flag;
        }

        public void AddDefaults()
        {
            Grid1D XGrid0 = new Grid1D(0.0f, 0);
            Grid1D YGrid0 = new Grid1D(0.0f, 0);
            Grid1D XGrid1 = new Grid1D(150.0f, 3);
            Grid1D YGrid1 = new Grid1D(180.0f, 3);
            Grid1D XGrid2 = new Grid1D(520.0f, 5);
            Grid1D YGrid2 = new Grid1D(350.0f, 2);

            V3DataOnGrid Grid0 = new V3DataOnGrid("Empty grid", DateTime.Now, XGrid0, YGrid0);
            V3DataOnGrid Grid1 = new V3DataOnGrid("Grid 1", DateTime.Now, XGrid1, YGrid1);
            Grid1.InitRandom(50.0, 150.0);
            V3DataOnGrid Grid2 = new V3DataOnGrid("Grid 2", DateTime.Now, XGrid2, YGrid2);
            Grid2.InitRandom(500.0, 600.0);

            V3DataCollection Collection0 = new V3DataCollection("Empty collection", DateTime.Now);
            V3DataCollection Collection1 = new V3DataCollection("Collection 1", DateTime.Now);
            Collection1.InitRandom(6, 70.0f, 30.0f, 300.0, 450.0);
            V3DataCollection Collection2 = new V3DataCollection("Collection 2", DateTime.Now);
            Collection2.InitRandom(10, 100.0f, 180.0f, 1500.0, 2000.0);

            V3DataItems.Add(Grid0);
            V3DataItems.Add(Grid1);
            V3DataItems.Add(Grid2);
            V3DataItems.Add(Collection0);
            V3DataItems.Add(Collection1);
            V3DataItems.Add(Collection2);
        }

        // обработчик события PropertyChanged
        protected void PropertyChangedEventAction(object source, PropertyChangedEventArgs args)
        {
            if (DataChanged != null)
                DataChanged(this, new DataChangedEventArgs(ChangeInfo.ItemChanged, $"Item property has been changed. There are {Count} elements.\n"));
        }

        public override string ToString()
        {
            string str = "";
            foreach (V3Data element in V3DataItems)
            {
                str += element.ToString();
            }
            return str;
        }

        public string ToLongString(string format)
        {
            string str = "";
            foreach (V3Data element in V3DataItems)
            {
                str += element.ToLongString(format);
            }
            return str;
        }
    }
}
