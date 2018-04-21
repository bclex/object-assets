using System;
using System.Collections;
using System.Text;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// This class will control HTML tag heuristics that will allow faster matching of tags
    /// to avoid long cycles as well as creation of same strings over and over again.
    /// 
    /// This is effectively a fancy hash lookup table with attributes being hashed in context of tag
    /// </summary>
    ///<exclude/>
    public class HTMLheuristics : IDisposable
    {
        /// <summary>
        /// Maximum number of strings allowed to be set (all lower-cased)
        /// </summary>
        const int MAX_STRINGS = 1024;

        /// <summary>
        /// Maximum number of chars to be taken into account
        /// </summary>
        const int MAX_CHARS = byte.MaxValue;

        /// <summary>
        /// Array in which we will keep char hints to quickly match ID (if non-zero) of tag
        /// </summary>
        short[,] Chars = new short[byte.MaxValue + 1, byte.MaxValue + 1];

        /// <summary>
        /// Strings used, once matched they will be returned to avoid creation of a brand new string
        /// and all associated costs with it
        /// </summary>
        string[] Strings = new string[MAX_STRINGS];

        /// <summary>
        /// Binary data represending tag strings is here: case sensitive: lower case for even even value, and odd for each odd
        /// for the same string
        /// </summary>
        byte[][] TagData = new byte[MAX_STRINGS * 2][];

        /// <summary>
        /// List of added tags to avoid dups
        /// </summary>
        Hashtable AddedTags = new Hashtable();

        /// <summary>
        /// Hash that will contain single char mapping hash
        /// </summary>
        byte[][] Attributes = new byte[MAX_STRINGS * 2][];

        /// <summary>
        /// Binary data represending attribute strings is here: case sensitive: lower case for even even value, and odd for each odd
        /// for the same string
        /// </summary>
        byte[][] AttrData = new byte[MAX_STRINGS * 2][];

        /// <summary>
        /// List of added attributes to avoid dups
        /// </summary>
        Hashtable AddedAttributes = new Hashtable();

        string[] Attrs = new string[MAX_STRINGS];

        /// <summary>
        /// This array will contain all double char strings 
        /// </summary>
        static string[,] AllTwoCharStrings;

        /// <summary>
        /// Static constructor
        /// </summary>
        static HTMLheuristics()
        {
            AllTwoCharStrings = new string[(MAX_CHARS + 1), (MAX_CHARS + 1)];
            // we will create all possible strings for two bytes combinations - this will allow
            // to cater for all two char combinations at cost of mere 256kb of RAM per instance
            for (var i = 0; i < AllTwoCharStrings.Length; i++)
            {
                var c1 = (byte)(i >> 8);
                var c2 = (byte)(i & 0xFF);
                AllTwoCharStrings[c1, c2] = ((string)(((char)c1).ToString() + ((char)c2).ToString())).ToLower();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public HTMLheuristics()
        {
        }

        /// <summary>
        /// Adds tag to list of tracked tags (don't add too many, if you have got multiple same first
        /// 2 chars then duplicates won't be added, so make sure the first added tags are the MOST LIKELY to be found)
        /// </summary>
        /// <param name="sTag">Tag: strictly ASCII only</param>
        /// <param name="attributeNames">Comma delimited list of attributed</param>
        /// <param name="bAddClosed">If true then closed version of tag added</param>
        /// <returns>True if tag was added, false otherwise (it may already be added, or leads to hash clash)</returns>
        public bool AddTag(string tag, string attributeNames)
        {
            var tag2 = tag.ToLower().Trim();
            if (tag2.Length == 0 || tag2.Length > 32 || AddedTags.Contains(tag2))
                return false;
            if (AddedTags.Count >= byte.MaxValue)
                return false;
            // ID should not be zero as it is an indicator of no match
            var id = (short)(AddedTags.Count + 1);
            AddedTags[tag2] = id;
            // remember tag string: it will be returned in case of matching
            Strings[id] = tag2;
            // add both lower and upper case tag values
            if (!AddTag(tag2, id, (short)(id * 2 + 0)))
                return false;
            if (!AddTag(tag2.ToUpper(), id, (short)(id * 2 + 1)))
                return false;
            // allocate memory for attribute hashes for this tag
            AttrData[id] = new byte[byte.MaxValue + 1];
            // now add attribute names
            foreach (var attrName2 in attributeNames.ToLower().Split(','))
            {
                var attrName = attrName2.Trim();
                if (attrName.Length == 0)
                    continue;
                // only add attribute if we have not got it added for same first char of the same tag:
                if (AttrData[id][attrName[0]] > 0 || AttrData[id][char.ToUpper(attrName[0])] > 0)
                    continue;
                var attrId = AddedAttributes.Count + 1;
                if (AddedAttributes.Contains(attrName))
                    attrId = (int)AddedAttributes[attrName];
                else
                {
                    AddedAttributes[attrName] = attrId;
                    Attrs[attrId] = attrName;
                }
                // add both lower and upper case tag values
                AddAttribute(attrName, id, (short)(attrId * 2 + 0));
                AddAttribute(attrName.ToUpper(), id, (short)(attrId * 2 + 1));
            }
            return true;
        }

        void AddAttribute(string attr, short id, short attrId)
        {
            if (attr.Length == 0)
                return;
            var c = (byte)attr[0];
            Attributes[attrId] = Encoding.Default.GetBytes(attr);
            AttrData[id][c] = (byte)attrId;
        }

        /// <summary>
        /// Returns string for ID returned by GetMatch
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>string</returns>
        public string GetString(int id)
        {
            return Strings[id >> 1];
        }

        public string GetTwoCharString(byte c1, byte c2)
        {
            return AllTwoCharStrings[c1, c2];
        }

        public byte[] GetStringData(int id)
        {
            return TagData[id];
        }

        public short MatchTag(byte c1, byte c2)
        {
            return Chars[c1, c2];
        }

        public short MatchAttr(byte c, int tagId)
        {
            return AttrData[tagId >> 1][c];
        }

        public byte[] GetAttrData(int attrId)
        {
            return Attributes[attrId];
        }

        public string GetAttr(int attrId)
        {
            return Attrs[(attrId >> 1)];
        }

        bool AddTag(string tag, short id, short dataId)
        {
            if (tag.Length == 0)
                return false;
            TagData[dataId] = Encoding.Default.GetBytes(tag);
            if (tag.Length == 1)
            {
                // ok just one char, in which case we will mark possible second char that can be
                // '>', ' ' and other whitespace
                // we will use negative ID to hint that this is single char hit
                if (!SetHash(tag[0], ' ', (short)(-1 * dataId)))
                    return false;
                if (!SetHash(tag[0], '\t', (short)(-1 * dataId)))
                    return false;
                if (!SetHash(tag[0], '\r', (short)(-1 * dataId)))
                    return false;
                if (!SetHash(tag[0], '\n', (short)(-1 * dataId)))
                    return false;
                if (!SetHash(tag[0], '>', (short)(-1 * dataId)))
                    return false;
            }
            else if (!SetHash(tag[0], tag[1], dataId))
                return false;
            return true;
        }

        bool SetHash(char c1, char c2, short id)
        {
            // already exists
            if (Chars[(byte)c1, (byte)c2] != 0)
                return false;
            Chars[(byte)c1, (byte)c2] = id;
            return true;
        }

        bool Disposed;

        /// <summary>
        /// Disposes of resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Chars = null;
                AddedTags = null;
                Strings = null;
                TagData = null;
            }
            Disposed = true;
        }
    }
}
