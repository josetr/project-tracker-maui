namespace ProjectTracker;

using System.Linq;

public static class ListExtensions
{
    public static T? GetNextItem<T>(this List<T> items, int i, int n) where T : class
    {
        if (!items.Any())
            return null;

        if (i == -1)
            i = 0;
        else
        {
            i += n;
            if (i < 0)
                i = items.Count - 1;
            if (i >= items.Count)
                i = 0;
        }

        if (i < items.Count && i >= 0)
            return items[i];

        return null;
    }
}
