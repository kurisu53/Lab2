using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Globalization;

namespace Lab3
{
    class V3DataCollection : V3Data, IEnumerable<DataItem> // класс для значений поля на неравномерной сетке
    {
        public List<DataItem> DataItems { get; set; }

        public V3DataCollection(string measures, DateTime time) : base(measures, time)
        {
            DataItems = new List<DataItem>();
        }

        public V3DataCollection(string filename)
        {
            /* Конструктор, инициализирующий объект данными из файла filename.
             * Формат файла: (для удобства строки пронумерованы)
             * 1. <Информация об измерениях>
             * 2. <Дата измерений, предпочтительно в формате ДД/ММ/ГГГГ ЧЧ:ММ:СС>
             * 3. <координата Х 1-й точки> <координата Y 1-й точки> <значение поля в 1-й точке>
             * ...
             * (N+2). <координата Х N-й точки> <координата Y N-й точки> <значение поля в N-й точке>
             * (N+3). STOP
             * 
             * Координаты и значения разделяются пробелами. Все данные указываются в виде вещественных чисел.
             * Разделитель в вещественном числе - точка.
             * Последняя строка файла должна содержать слово STOP
             * 
             * !! Файл должен находиться в каталоге /bin/Debug/netcoreapp3.1, т. е. в одной папке с Lab1.exe !!
             */

            FileStream fstream = null;

            try
            {
                fstream = new FileStream(filename, FileMode.Open);
                StreamReader reader = new StreamReader(fstream);

                Measures = reader.ReadLine();
                MeasureTime = DateTime.Parse(reader.ReadLine());
                DataItems = new List<DataItem>();

                bool stopflag = false;
                Vector2 coord;
                double field;
                string currStr = System.String.Empty;
                string[] currStrArray;
                while (!stopflag)
                {
                    currStr = reader.ReadLine();
                    if (currStr == "STOP")
                        stopflag = true;
                    else
                    {
                        CultureInfo cultureInfoEN = new CultureInfo("en-US");
                        currStrArray = currStr.Split(' ');
                        coord.X = float.Parse(currStrArray[0], cultureInfoEN);
                        coord.Y = float.Parse(currStrArray[1], cultureInfoEN);
                        field = double.Parse(currStrArray[2], cultureInfoEN);
                        DataItems.Add(new DataItem(coord, field));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (fstream != null)
                    fstream.Close();
            }
        }

        // реализация интерфейса IEnumerable<DataItem>
        public IEnumerator<DataItem> GetEnumerator()
        {
            return DataItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // инициализация координат и значений поля в них случайными числами
        public void InitRandom(int nItems, float xmax, float ymax, double minValue, double maxValue)
        {
            Random rand = new Random();
            Vector2 coord;
            double field;

            for (int i = 0; i < nItems; i++)
            {
                coord.X = (float)(rand.NextDouble() * xmax);
                coord.Y = (float)(rand.NextDouble() * ymax);
                field = rand.NextDouble() * (maxValue - minValue) + minValue;
                DataItems.Add(new DataItem(coord, field));
            }
        }

        public override Vector2[] Nearest(Vector2 v)
        {
            List<Vector2> NodesList = new List<Vector2>();
            float min = Single.MaxValue;

            Vector2 currentNode;
            foreach (DataItem item in DataItems)
            {
                currentNode = item.Coord;
                if (Vector2.Distance(currentNode, v) < min)
                // если нашли более близкий узел, перезаполняем список
                {
                    min = Vector2.Distance(currentNode, v);
                    NodesList.Clear();
                    NodesList.Add(currentNode);
                }
                else if (Math.Abs(Vector2.Distance(currentNode, v) - min) <= Math.Abs(min * 0.000001))
                // проверка на равенство чисел с плавающей точкой
                // Если есть еще один узел на минимальном расстоянии, добавляем его
                {
                    NodesList.Add(currentNode);
                }
            }

            return NodesList.ToArray();
        }

        public override string ToString()
        {
            return $"\nV3DataCollection. {base.ToString()} There are {DataItems.Count} data items.";
        }

        public override string ToLongString()
        {
            string str = "";
            foreach (DataItem item in DataItems)
            {
                str += item.ToString();
            }
            return $"{this}\n{str}";
        }

        public override string ToLongString(string format)
        {
            string str = "";
            foreach (DataItem item in DataItems)
            {
                str += item.ToString(format);
            }
            return $"{this}\n{str}";
        }
    }
}
