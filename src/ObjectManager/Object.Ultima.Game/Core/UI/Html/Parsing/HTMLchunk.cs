using System;
using System.Collections;
using System.Text;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// Type of parsed HTML chunk (token), each non-null returned chunk from HTMLparser will have oType set to 
    /// one of these values
    /// </summary>
    public enum HTMLchunkType
    {
        /// <summary>
        /// Text data from HTML
        /// </summary>
        Text = 0,

        /// <summary>
        /// Open tag, possibly with attributes
        /// </summary>
        OpenTag = 1,

        /// <summary>
        /// Closed tag (it may still have attributes)
        /// </summary>
        CloseTag = 2,

        /// <summary>
        /// Comment tag (<!-- -->)depending on HTMLparser boolean flags you may have:
        /// a) nothing to oHTML variable - for faster performance, call SetRawHTML function in parser
        /// b) data BETWEEN tags (but not including comment tags themselves) - DEFAULT
        /// c) complete RAW HTML representing data between tags and tags themselves (same as you get in a) when
        /// you call SetRawHTML function)
        /// 
        /// Note: this can also be CDATA part of XML document - see sTag value to determine if its proper comment
        /// or CDATA or (in the future) something else
        /// </summary>
        Comment = 3,

        /// <summary>
        /// Script tag (<!-- -->) depending on HTMLparser boolean flags
        /// a) nothing to oHTML variable - for faster performance, call SetRawHTML function in parser
        /// b) data BETWEEN tags (but not including comment tags themselves) - DEFAULT
        /// c) complete RAW HTML representing data between tags and tags themselves (same as you get in a) when
        /// you call SetRawHTML function)
        /// </summary>
        Script = 4,
    }

    /// <summary>
    /// Parsed HTML token that is either text, comment, script, open or closed tag as indicated by the oType variable.
    /// </summary>
    public class HTMLchunk : IDisposable
    {
        /// <summary>
        /// Maximum default capacity of buffer that will keep data
        /// </summary>
        /// <exclude/>
        public static int TEXT_CAPACITY = 1024 * 256;

        /// <summary>
        /// Maximum number of parameters in a tag - should be high enough to fit most sensible cases
        /// </summary>
        /// <exclude/>
        public static int MAX_PARAMS = 256;

        /// <summary>
        /// Chunk type showing whether its text, open or close tag, comments or script.
        /// WARNING: if type is comments or script then you have to manually call Finalise(); method
        /// in order to have actual text of comments/scripts in oHTML variable
        /// </summary>
        public HTMLchunkType Type;

        /// <summary>
        /// If true then tag params will be kept in a hash rather than in a fixed size arrays. 
        /// This will be slow down parsing, but make it easier to use.
        /// </summary>
        public bool HashMode = true;

        /// <summary>
        /// For TAGS: it stores raw HTML that was parsed to generate thus chunk will be here UNLESS
        /// HTMLparser was configured not to store it there as it can improve performance
        /// <p>
        /// For TEXT or COMMENTS: actual text or comments - you MUST call Finalise(); first.
        /// </p>
        /// </summary>
        public string Html = string.Empty;

        /// <summary>
        /// Offset in bHTML data array at which this chunk starts
        /// </summary>
        public int ChunkOffset;

        /// <summary>
        /// Length of the chunk in bHTML data array
        /// </summary>
        public int ChunkLength;

        /// <summary>
        /// If its open/close tag type then this is where lowercased Tag will be kept
        /// </summary>
        public string Tag = string.Empty;

        /// <summary>
        /// If true then it must be closed tag
        /// </summary>
        /// <exclude/>
        public bool Closure;

        /// <summary>
        /// If true then it must be closed tag and closure sign / was at the END of tag, ie this is a SOLO
        /// tag 
        /// </summary>
        /// <exclude/>
        public bool EndClosure;

        /// <summary>
        /// If true then it must be comments tag
        /// </summary>
        /// <exclude/>
        public bool Comments;

        /// <summary>
        /// True if entities were present (and transformed) in the original HTML
        /// </summary>
        /// <exclude/>
        public bool Entities;

        /// <summary>
        /// Set to true if &lt; entity (tag start) was found 
        /// </summary>
        /// <exclude/>
        public bool LtEntity;

        /// <summary>
        /// Hashtable with tag parameters: keys are param names and values are param values.
        /// ONLY used if bHashMode is set to TRUE.
        /// </summary>
        public Hashtable Params;

        /// <summary>
        /// Number of parameters and values stored in sParams array, OR in oParams hashtable if
        /// bHashMode is true
        /// </summary>
        public int ParamsCount;

        /// <summary>
        /// Param names will be stored here - actual number is in iParams.
        /// ONLY used if bHashMode is set to FALSE.
        /// </summary>
        public string[] ParamsNames = new string[MAX_PARAMS];

        /// <summary>
        /// Param values will be stored here - actual number is in iParams.
        /// ONLY used if bHashMode is set to FALSE.
        /// </summary>
        public string[] ParamsValues = new string[MAX_PARAMS];

        /// <summary>
        /// Character used to quote param's value: it is taken actually from parsed HTML
        /// </summary>
        public byte[] ParamChars = new byte[MAX_PARAMS];

        /// <summary>
        /// Encoder to be used for conversion of binary data into strings, Encoding.Default is used by default,
        /// but it can be changed if top level user of the parser detects that encoding was different
        /// </summary>
        public Encoding Enc = Encoding.Default;

        bool Disposed;

        /// <summary>
        /// This function will convert parameters stored in sParams/sValues arrays into oParams hash
        /// Useful if generally parsing is done when bHashMode is FALSE. Hash operations are not the fastest, so
        /// its best not to use this function.
        /// </summary>
        public void ConvertParamsToHash()
        {
            if (Params != null) Params.Clear();
            else Params = new Hashtable();
            for (int i = 0; i < ParamsCount; i++)
                Params[ParamsNames[i]] = ParamsValues[i];
        }

        /// <summary>
        /// Sets encoding to be used for conversion of binary data into string
        /// </summary>
        /// <param name="enc">Encoding object</param>
        public void SetEncoding(Encoding enc)
        {
            Enc = enc;
        }

        /// <summary>
        /// Generates HTML based on current chunk's data 
        /// Note: this is not a high performance method and if you want ORIGINAL HTML that was parsed to create
        /// this chunk then use relevant HTMLparser method to obtain such HTML then you should use
        /// function of parser: SetRawHTML
        /// 
        /// </summary>
        /// <returns>HTML equivalent of this chunk</returns>
        public string GenerateHTML()
        {
            var hmtl = string.Empty;
            switch (Type)
            {
                // matched open tag, ie <a href="">
                case HTMLchunkType.OpenTag:
                    hmtl += "<" + Tag;
                    if (ParamsCount > 0)
                        hmtl += " " + GenerateParamsHTML();
                    hmtl += ">";
                    break;
                // matched close tag, ie </a>
                case HTMLchunkType.CloseTag:
                    if (ParamsCount > 0 || EndClosure)
                    {
                        hmtl += "<" + Tag;
                        if (ParamsCount > 0)
                            hmtl += " " + GenerateParamsHTML();
                        hmtl += "/>";
                    }
                    else hmtl += "</" + Tag + ">";
                    break;
                case HTMLchunkType.Script:
                    if (Html.Length == 0) hmtl = "<script>n/a</script>";
                    else hmtl = Html;
                    break;
                case HTMLchunkType.Comment:
                    // note: we might have CDATA here that we treat as comments
                    if (Tag == "!--")
                    {
                        if (Html.Length == 0) hmtl = "<!-- n/a -->";
                        else hmtl = "<!--" + Html + "-->";
                    }
                    // ref: http://www.w3schools.com/xml/xml_cdata.asp
                    else if (Tag == "![CDATA[")
                    {
                        if (Html.Length == 0) hmtl = "<![CDATA[ n/a \n]]>";
                        else hmtl = "<![CDATA[" + Html + "]]>";
                    }
                    break;
                // matched normal text
                case HTMLchunkType.Text:
                    return Html;
            }
            return hmtl;
        }

        /// <summary>
        /// Returns value of a parameter
        /// </summary>
        /// <param name="param">Parameter</param>
        /// <returns>Parameter value or empty string</returns>
        public string GetParamValue(string param)
        {
            if (!HashMode)
            {
                for (var i = 0; i < ParamsCount; i++)
                    if (ParamsNames[i] == param)
                        return ParamsValues[i];
            }
            else
            {
                var value = Params[param];
                if (value != null)
                    return (string)value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates HTML for params in this chunk
        /// </summary>
        /// <returns>String with HTML corresponding to params</returns>
        public string GenerateParamsHTML()
        {
            var html = string.Empty;
            if (HashMode)
            {
                if (Params.Count > 0)
                    foreach (string param in Params.Keys)
                    {
                        var value = (string)Params[param];
                        if (html.Length > 0)
                            html += " ";
                        // FIXIT: this is really not correct as we do not use same char used
                        html += GenerateParamHTML(param, value, '\'');
                    }
            }
            else
            {
                // this is alternative method of getting params -- it may look less convinient
                // but it saves a LOT of CPU ticks while parsing. It makes sense when you only need
                // params for a few
                if (ParamsCount > 0)
                    for (var i = 0; i < ParamsCount; i++)
                    {
                        if (html.Length > 0)
                            html += " ";
                        html += GenerateParamHTML(ParamsNames[i], ParamsValues[i], (char)ParamChars[i]);
                    }

            }
            return html;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool bDisposing)
        {
            if (!Disposed)
            {
                if (Params != null)
                    Params = null;
                ParamsNames = null;
                ParamsValues = null;
            }
            Disposed = true;
        }

        /// <summary>
        /// Generates HTML for param/value pair
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="value">Value (empty if not specified)</param>
        /// <returns>String with HTML</returns>
        public string GenerateParamHTML(string param, string value, char paramChar)
        {
            if (value.Length > 0)
            {
                // check param's value for whitespace or quote chars, if they are not present, then
                // we can save 2 bytes by not generating quotes
                if (value.Length > 20)
                    return param + "=" + paramChar + MakeSafeParamValue(value, paramChar) + paramChar;
                for (var i = 0; i < value.Length; i++)
                    switch (value[i])
                    {
                        case ' ':
                        case '\t':
                        case '\'':
                        case '\"':
                        case '\n':
                        case '\r': return param + "='" + MakeSafeParamValue(value, '\'') + "'";
                        default: break;
                    }
                return param + "=" + value;
            }
            else return param;
        }

        /// <summary>
        /// Makes parameter value safe to be used in param - this will check for any conflicting quote chars,
        /// but not full entity-encoding
        /// </summary>
        /// <param name="line">Line of text</param>
        /// <param name="quoteChar">Quote char used in param - any such chars in text will be entity-encoded</param>
        /// <returns>Safe text to be used as param's value</returns>
        public static string MakeSafeParamValue(string line, char quoteChar)
        {
            // we speculatievly expect that in most cases we don't actually need to entity-encode string,
            for (var i = 0; i < line.Length; i++)
                if (line[i] == quoteChar)
                {
                    // have to restart here
                    var b = new StringBuilder(line.Length + 10);
                    b.Append(line.Substring(0, i));
                    for (var j = i; j < line.Length; j++)
                    {
                        var c = line[j];
                        if (c == quoteChar) b.Append("&#" + ((int)c).ToString() + ";");
                        else b.Append(c);
                    }
                    return b.ToString();
                }
            return line;
        }

        /// <summary>
        /// Adds tag parameter to the chunk
        /// </summary>
        /// <param name="param">Parameter name (ie color)</param>
        /// <param name="value">Value of the parameter (ie white)</param>
        public void AddParam(string param, string value, byte paramChar)
        {
            if (!HashMode)
            {
                if (ParamsCount < MAX_PARAMS)
                {
                    ParamsNames[ParamsCount] = param;
                    ParamsValues[ParamsCount] = value;
                    ParamChars[ParamsCount] = paramChar;
                    ParamsCount++;
                }
            }
            else
            {
                ParamsCount++;
                Params[param] = value;
            }
        }

        /// <summary>
        /// Clears chunk preparing it for 
        /// </summary>
        public void Clear()
        {
            Tag = Html = string.Empty;
            LtEntity = Entities = Comments = Closure = EndClosure = false;
            ParamsCount = 0;
            if (HashMode)
                if (Params != null)
                    Params.Clear();
        }

        /// <summary>
        /// Initialises new HTMLchunk
        /// </summary>
        /// <param name="hashMode">Sets <seealso cref="HashMode"/></param>
        public HTMLchunk(bool hashMode)
        {
            HashMode = hashMode;
            if (HashMode)
                Params = new Hashtable();
        }

        public override string ToString()
        {
            return Tag;
        }
    }
}
