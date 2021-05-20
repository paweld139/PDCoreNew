using PDCore.Helpers.Calculation;
using System;

namespace PDCore.Helpers.DataStructures.Book
{
    public interface IBook
    {
        void AddGrade(double grade);
        Statistics GetStatistics();
        string Name { get; }
        event EventHandler GradeAdded;
    }
}
