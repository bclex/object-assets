using OA.Core.UI.Html.Parsing;
using System.Collections;

namespace OA.Core.UI.Html.Styles
{
    class OpenTag
    {
        public string Tag;
        public bool Closure;
        public bool EndClosure;
        public Hashtable Params;

        public OpenTag(HTMLchunk chunk)
        {
            Tag = chunk.Tag;
            Closure = chunk.Closure;
            EndClosure = chunk.EndClosure;
            Params = new Hashtable();
            foreach (DictionaryEntry entry in chunk.Params)
                Params.Add(entry.Key, entry.Value);
        }
    }
}
