using PDCoreNew.Helpers.Calculation;
using PDCoreNew.Models;
using System;

namespace PDCoreNew.Helpers.DataStructures.Book
{
    public abstract class Book : NamedObject, IBook
    {
        protected Book(string name) : base(name)
        {

        }

        public abstract event EventHandler GradeAdded;
        public abstract void AddGrade(double grade);
        public abstract Statistics GetStatistics();
    }
}
