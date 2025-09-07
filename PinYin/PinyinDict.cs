namespace M56X.Core.PinYin
{
    public abstract class PinyinDict : IPinyinDict
    {
        public abstract Dictionary<string, string[]> Mapping();

        public string[] ToPinyin(string word)
        {
            var mappingResult = Mapping();
            if (mappingResult == null)
            {
                return [];
            }
            if (mappingResult.TryGetValue(word, out var result))
            {
                return result;
            }
            return [];
        }

        public List<string> Words()
        {
            var mappingResult = Mapping();
            if (mappingResult == null)
            {
                return [];
            }
            return [.. mappingResult.Keys.Select(key => key)];
        }
    }
}
