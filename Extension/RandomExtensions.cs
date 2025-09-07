namespace M56X.Core.Extension
{
    public static class RandomExtensions
    {
        public static T Choice<T>(this Random random, IEnumerable<T> sequence)
        {
            var list = sequence as IList<T> ?? [.. sequence];
            return list[random.Next(list.Count)];
        }
    }
}
