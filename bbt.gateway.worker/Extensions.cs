namespace bbt.gateway.worker
{
    public static class Extensions
    {
        public static List<List<T>> DivideListIntoParts<T>(this List<T> list,int partSize)
        {
            List<List<T>> result = new List<List<T>>();

            for (int i = 0; i < (list.Count / partSize) + 1; i++)
            {
                result.Add(list.Skip(i*partSize).Take(partSize).ToList());
            }

            return result;
        }
    }
}
