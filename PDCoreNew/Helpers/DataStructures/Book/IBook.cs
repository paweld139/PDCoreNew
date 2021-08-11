using PDCoreNew.Helpers.Calculation;
using System;

namespace PDCoreNew.Helpers.DataStructures.Book
{
    public interface IBook
    {
        void AddGrade(double grade);
        Statistics GetStatistics();
        string Name { get; }
        event EventHandler GradeAdded;
    }
}
