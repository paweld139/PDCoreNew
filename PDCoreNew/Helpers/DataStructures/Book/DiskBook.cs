using PDCoreNew.Helpers.Calculation;
using System;
using System.IO;

namespace PDCoreNew.Helpers.DataStructures.Book
{
    public class DiskBook : Book
    {
        public DiskBook(string name) : base(name)
        {

        }

        public override event EventHandler GradeAdded;

        public override void AddGrade(double grade)
        {
            using var writer = File.AppendText($"{Name}.txt");

            writer.WriteLine(grade);

            GradeAdded?.Invoke(this, new EventArgs());
        }

        public override Statistics GetStatistics()
        {
            var result = new Statistics();

            string line;
            double number;

            using (var reader = File.OpenText($"{Name}.txt"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    number = double.Parse(line);
                    result.Add(number);
                }
            }

            return result;
        }
    }
}
