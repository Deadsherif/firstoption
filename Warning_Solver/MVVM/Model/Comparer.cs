using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warning_Solver.MVVM.Model
{
    internal class Comparer : IEqualityComparer<FailureMessage>
    {
        public bool Equals(FailureMessage x, FailureMessage y)
        {
            //Check whether the products' properties are equal.
            return x.GetDescriptionText() == y.GetDescriptionText();
        }

        public int GetHashCode(FailureMessage obj)
        {
            return obj.GetDescriptionText().GetHashCode();
        }
    }
}
