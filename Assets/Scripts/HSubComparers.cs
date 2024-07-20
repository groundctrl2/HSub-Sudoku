using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows 3 int tuples to equal despite null elements
class TripleTupleComparer : IEqualityComparer<(int? occurrence1, int? occurrence2, int? occurrence3)>
{
    public bool Equals((int? occurrence1, int? occurrence2, int? occurrence3) x, (int? occurrence1, int? occurrence2, int? occurrence3) y)
    {
        return Nullable.Equals(x.occurrence1, y.occurrence1) &&
               Nullable.Equals(x.occurrence2, y.occurrence2) &&
               Nullable.Equals(x.occurrence3, y.occurrence3);
    }

    public int GetHashCode((int? occurrence1, int? occurrence2, int? occurrence3) obj)
    {
        int hash = 17;
        hash = hash * 23 + (obj.occurrence1?.GetHashCode() ?? 0);
        hash = hash * 23 + (obj.occurrence2?.GetHashCode() ?? 0);
        hash = hash * 23 + (obj.occurrence3?.GetHashCode() ?? 0);
        return hash;
    }
}

// Allows 4 int tuples to equal despite null elements
class QuadrupleTupleComparer : IEqualityComparer<(int? occurrence1, int? occurrence2, int? occurrence3, int? occurrence4)>
{
    public bool Equals((int? occurrence1, int? occurrence2, int? occurrence3, int? occurrence4) x, (int? occurrence1, int? occurrence2, int? occurrence3, int? occurrence4) y)
    {
        return Nullable.Equals(x.occurrence1, y.occurrence1) &&
               Nullable.Equals(x.occurrence2, y.occurrence2) &&
               Nullable.Equals(x.occurrence3, y.occurrence3) &&
               Nullable.Equals(x.occurrence4, y.occurrence4);
    }

    public int GetHashCode((int? occurrence1, int? occurrence2, int? occurrence3, int? occurrence4) obj)
    {
        int hash = 17;
        hash = hash * 23 + (obj.occurrence1?.GetHashCode() ?? 0);
        hash = hash * 23 + (obj.occurrence2?.GetHashCode() ?? 0);
        hash = hash * 23 + (obj.occurrence3?.GetHashCode() ?? 0);
        hash = hash * 23 + (obj.occurrence4?.GetHashCode() ?? 0);
        return hash;
    }
}
