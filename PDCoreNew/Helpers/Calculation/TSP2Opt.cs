using PDCoreNew.Extensions;

namespace PDCoreNew.Helpers.Calculation
{
    public static class TSP2Opt
    {
        public static void Search(int[] perm, double[,] costs)
        {
            // The first city in edge #1
            for (int i1 = 0; i1 < perm.Length - 2; i1++)
            {
                // Where we go to along edge #1
                int i2 = i1 + 1;

                // The first city in edge #2
                for (int i3 = i2 + 1; i3 < perm.Length; i3++)
                {
                    // The edges cannot have a city in common
                    if ((i3 == i1) || (i3 == i2))
                        continue;

                    // Where we go to along edge #2
                    int i4 = (i3 + 1) % perm.Length;

                    // The edges cannot have a city in common
                    if ((i4 == i1) || (i4 == i2))
                        continue;

                    // How the cost changes when we change
                    // edges i1 -> i2 and i3 -> i4
                    // to i1 -> i3 and i2 -> i4
                    double delta = costs[perm[i1], perm[i3]]
                                 + costs[perm[i2], perm[i4]]
                                 - costs[perm[i1], perm[i2]]
                                 - costs[perm[i3], perm[i4]];

                    // Does the cost decrease?
                    if (delta < 0)
                        // If so, exchange the edges
                        perm.SwapValues(i2, i3);
                }
            } /* Search */
        }
    }
}
