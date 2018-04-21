using System;
using System.IO;
using System.Text;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// Allows to parse HTML by splitting it into small token (HTMLchunks) such as tags, text, comments etc.
    /// 
    /// Do NOT create multiple instances of this class - REUSE single instance
    /// Do NOT call same instance from multiple threads - it is NOT thread safe
    /// </summary>
    public class HTMLparser : IDisposable
    {
        /// <summary>
        /// If false (default) then HTML entities (like "&nbsp;") will not be decoded, otherwise they will
        /// be decoded: this should be set if you deal with unicode data that takes advantage of entities
        /// and in cases when you need to deal with final string representation
        /// </summary>
        public bool CanDecodeEntities
        {
            set { _decodeEntities = _e.CanDecodeEntities = value; }
            get { return _e.CanDecodeEntities; }
        }

        bool _decodeEntities;

        /// <summary>
        /// If false (default) then mini entity set (&nbsp;) will be decoded, but not all of them
        /// </summary>
        public bool DecodeMiniEntities
        {
            set { _e.MiniEntities = value; }
            get { return _e.MiniEntities; }
        }

        /// <summary>
        /// If true (default) then heuristics engine will be used to match tags and attributes quicker, it is
        /// possible to add new tags to it, <see cref="_he"/>
        /// </summary>
        public bool EnableHeuristics
        {
            get { return _tp.bEnableHeuristics; }
            set { _tp.bEnableHeuristics = value; }
        }

        /// <summary>
        /// If true then exception will be thrown in case of inability to set encoding taken
        /// from HTML - this is possible if encoding was incorrect or not supported, this would lead
        /// to abort in processing. Default behavior is to use Default encoding that should keep symbols as
        /// is - most likely garbage looking things if encoding was not supported.
        /// </summary>
        public bool ThrowExceptionOnEncodingSetFailure;

        /// <summary>
        /// If true (default: false) then parsed tag chunks will contain raw HTML, otherwise only comments will have it set
        /// <p>
        /// Performance hint: keep it as false, you can always get to original HTML as each chunk contains
        /// offset from which parsing started and finished, thus allowing to set exact HTML that was parsed
        /// </p>
        /// </summary>
        /// <exclude/>
        public bool KeepRawHTML;

        /// <summary>
        /// If true (default) then HTML for comments tags themselves AND between them will be set to oHTML variable, otherwise it will be empty
        /// but you can always set it later 
        /// </summary>
        public bool AutoKeepComments = true;

        /// <summary>
        /// If true (default: false) then HTML for script tags themselves AND between them will be set to oHTML variable, otherwise it will be empty
        /// but you can always set it later
        /// </summary>
        public bool AutoKeepScripts = true;

        /// <summary>
        /// If true (and either bAutoKeepComments or bAutoKeepScripts is true), then oHTML will be set
        /// to data BETWEEN tags excluding those tags themselves, as otherwise FULL HTML will be set, ie:
        /// '<!-- comments -->' but if this is set to true then only ' comments ' will be returned
        /// </summary>
        public bool AutoExtractBetweenTagsOnly = true;

        /// <summary>
        /// Long winded name... by default if tag is closed BUT it has got parameters then we will consider it
        /// open tag, this is not right for proper XML parsing
        /// </summary>
        public bool AutoMarkClosedTagsWithParamsAsOpen = true;

        /// <summary>
        /// If true (default), then all whitespace before TAG starts will be compressed to single space char (32 or 0x20)
        /// this makes parser run a bit faster, if you need exact whitespace before tags then change this flag to FALSE
        /// </summary>
        public bool CompressWhiteSpaceBeforeTag = true;

        /// <summary>
        /// If true then pure whitespace before tags will be ignored - but only IF its purely whitespace. 
        /// Enabling this feature will increase performance, however this will be at cost of correctness as 
        /// some text has essential whitespacing done just before tags.
        /// 
        /// REMOVED
        /// </summary>
        //public bool IgnoreWhiteSpaceBeforeTags=false;

        /// <summary>
        /// Heuristics engine used by Tag Parser to quickly match known tags and attribute names, can be disabled
        /// or you can add more tags to it to fit your most likely cases, it is currently tuned for HTML
        /// </summary>
        public HTMLheuristics _he = new HTMLheuristics();

        /// <summary>
        /// Internal -- dynamic string for text accumulation
        /// </summary>
        DynaString _text = new DynaString("");

        /// <summary>
        /// This chunk will be returned when it was parsed
        /// </summary>
        HTMLchunk _chunk = new HTMLchunk(true);

        /// <summary>
        /// Tag parser object
        /// </summary>
        TagParser _tp = new TagParser();

        /// <summary>
        /// Encoding used to convert binary data into string
        /// </summary>
        public Encoding Enc;

        /// <summary>
        /// Byte array with HTML will be kept here
        /// </summary>
        byte[] _html;

        /// <summary>
        /// The input html string. Saved because we parse the html as bytes, but we want to output unicode chars.
        /// </summary>
        string _originalHtml;

        /// <summary>
        /// Current position pointing to byte in bHTML
        /// </summary>
        int _curPos;

        /// <summary>
        /// Length of bHTML -- it appears to be faster to use it than bHTML.Length
        /// </summary>
        int _dataLength;

        /// <summary>
        /// Whitespace lookup table - 0 is not whitespace, otherwise it is
        /// </summary>
        static byte[] _whiteSpace = new byte[byte.MaxValue + 1];

        /// <summary>
        /// Entities manager
        /// </summary>
        HTMLentities _e = new HTMLentities();

        static HTMLparser()
        {
            // set bytes that are whitespace
            _whiteSpace[' '] = 1;
            _whiteSpace['\t'] = 1;
            _whiteSpace[13] = 1;
            _whiteSpace[10] = 1;
        }

        public HTMLparser()
        {
            // init heuristics engine
            _he.AddTag("a", "href");
            _he.AddTag("b", "");
            _he.AddTag("p", "class");
            _he.AddTag("i", "");
            _he.AddTag("s", "");
            _he.AddTag("u", "");

            _he.AddTag("td", "align,valign,bgcolor,rowspan,colspan");
            _he.AddTag("table", "border,width,cellpadding");
            _he.AddTag("span", "");
            _he.AddTag("option", "");
            _he.AddTag("select", "");

            _he.AddTag("tr", "");
            _he.AddTag("div", "class,align");
            _he.AddTag("img", "src,width,height,title,alt");
            _he.AddTag("input", "");
            _he.AddTag("br", "");
            _he.AddTag("li", "");
            _he.AddTag("ul", "");
            _he.AddTag("ol", "");
            _he.AddTag("hr", "");
            _he.AddTag("h1", "");
            _he.AddTag("h2", "");
            _he.AddTag("h3", "");
            _he.AddTag("h4", "");
            _he.AddTag("h5", "");
            _he.AddTag("h6", "");
            _he.AddTag("font", "size,color");
            _he.AddTag("meta", "name,content,http-equiv");
            _he.AddTag("base", "href");

            // these are pretty rare
            _he.AddTag("script", "");
            _he.AddTag("style", "");
            _he.AddTag("html", "");
            _he.AddTag("body", "");
        }

        /// <summary>
        /// Sets chunk param hash mode
        /// </summary>
        /// <param name="hashMode">If true then tag's params will be kept in Chunk's hashtable (slower), otherwise kept in arrays (sParams/sValues)</param>
        public void SetChunkHashMode(bool hashMode)
        {
            _chunk.HashMode = hashMode;
        }

        bool Disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool bDisposing)
        {
            if (!Disposed)
            {
                Disposed = true;
                if (_chunk != null)
                {
                    _chunk.Dispose();
                    _chunk = null;
                }
                if (_text != null)
                {
                    _text.Dispose();
                    _text = null;
                }
                _html = null;
                if (_e != null)
                {
                    _e.Dispose();
                    _e = null;
                }
                if (_tp != null)
                {
                    _tp.Dispose();
                    _tp = null;
                }
            }
        }

        /// <summary>
        /// Sets oHTML variable in a chunk to the raw HTML that was parsed for that chunk.
        /// </summary>
        /// <param name="chunk">Chunk returned by ParseNext function, it must belong to the same HTMLparser that
        /// was initiated with the same HTML data that this chunk belongs to</param>
        public void SetRawHTML(HTMLchunk chunk)
        {
            // note: this really should have been byte array assigned rather than string
            // it would be more correct originality-wise
            chunk.Html = Enc.GetString(_html, chunk.ChunkOffset, chunk.ChunkLength);
        }

        /// <summary>
        /// Closes object and releases all allocated resources
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Sets encoding 
        /// </summary>
        /// <param name="enc">Encoding object</param>
        public void SetEncoding(Encoding enc)
        {
            _chunk.SetEncoding(enc);
            _text.SetEncoding(enc);
            if (enc != null)
                Enc = enc;
            /*
            if (_charset.IsSupported(charset))
            {
                _charset.SetCharset(charset);
                //charConv = true;
            }
            //else Console.WriteLine("Charset '{0}' is not supported!", charset);
            */
        }

        /// <summary>
        /// Sets current encoding in format used in HTTP headers and HTML META tags
        /// </summary>
        /// <param name="sCharSet">Charset as </param>
        /// <returns>True if encoding was set, false otherwise (in which case Default encoding will be used)</returns>
        public bool SetEncoding(string charSet)
        {
            var charSet2 = charSet;
            try
            {
                if (charSet2.IndexOf(";") != -1)
                    charSet2 = GetCharSet(charSet2);
                charSet2 = charSet2.Replace("?", "");
                Enc = Encoding.GetEncoding(charSet2);
                // FIXIT: check here that the encoding set was not some fallback to default
                // due to lack of support on target machine
                //Utils.WriteLine("Setting encoding: " + charSet2);
                SetEncoding(Enc);
            }
            catch (Exception e)
            {
                if (ThrowExceptionOnEncodingSetFailure)
                    throw new Exception("Failed to set encoding '" + charSet2 + "', original encoding string: '" + charSet + "'. Original exception message: " + e.Message);
                // encoding was not supported - we will fall back to default one
                Enc = Encoding.Default;
                SetEncoding(Enc);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves charset information from format used in HTTP headers and META descriptions
        /// </summary>
        /// <param name="data">Data to find charset info from</param>
        /// <returns>Charset</returns>
        static string GetCharSet(string data)
        {
            try
            {
                if (data == null)
                    return string.Empty;
                var idx = data.ToLower().IndexOf("charset=");
                if (idx != -1)
                    return data.Substring(idx + 8, data.Length - idx - 8).ToLower().Trim();
            }
            catch //(Exception e)
            {
                // FIXIT: not ideal because it wont be visible in WinForms builds...
                //Console.WriteLine(e.ToString());
            }
            return "";
        }

        /// <summary>
        /// Inits mini-entities mode: only "nbsp" will be converted into space, all other entities 
        /// will be left as is
        /// </summary>
        public void InitMiniEntities()
        {
            _e.InitMiniEntities();
        }

        public string ChangeToEntities(string sLine)
        {
            return ChangeToEntities(sLine, false);
        }

        /// <summary>
        /// Parses line and changes known entiry characters into proper HTML entiries
        /// </summary>
        /// <param name="line">Line of text</param>
        /// <returns>Line of text with proper HTML entities</returns>
        public string ChangeToEntities(string line, bool changeDangerousCharsOnly)
        {
            // PHP does not handle that well, fsckers
            //changeAllNonASCII=false;
            try
            {
                // scan string first and if 
                for (var i = 0; i < line.Length; i++)
                {
                    int c = line[i];
                    // yeah I know its lame but its 3:30am and I had v.long debugging session :-/
                    switch (c)
                    {
                        case 0:
                        case 39:
                        case 145:
                        case 146:
                        case 147:
                        case 148: return _e.ChangeToEntities(line, i, changeDangerousCharsOnly);
                        default:
                            if (c < 32) // || (bChangeAllNonASCII && cChar>=127))
                                goto case 148;
                            break;
                    }
                    if (c < HTMLentities.EntityReverseLookup.Length && HTMLentities.EntityReverseLookup[c] != null)
                        return _e.ChangeToEntities(line, i, changeDangerousCharsOnly);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Entity exception: " + e.ToString());
            }
            // nothing need to be changed
            return line;
        }


        /// <summary>
        /// Constructs parser object using provided HTML as source for parsing
        /// </summary>
        /// <param name="html"></param>
        public HTMLparser(string html)
        {
            Init(html);
        }

        /// <summary>
        /// Initialises parses with HTML to be parsed from provided string
        /// </summary>
        /// <param name="html">String with HTML in it</param>
        public void Init(string html)
        {
            // set default encoding
            if (Enc == null)
                Enc = Encoding.Default;
            _originalHtml = html;
            Init(Encoding.Default.GetBytes(html));
        }

        /// <summary>
        /// Initialises parses with HTML to be parsed from provided data buffer: this is best in terms of
        /// correctness of parsing of various encodings that can be used in HTML
        /// </summary>
        /// <param name="html">Data buffer with HTML in it</param>
        public void Init(byte[] html)
        {
            Init(html, html.Length);
        }

        /// <summary>
        /// Inits parsing
        /// </summary>
        /// <param name="html">Data buffer</param>
        /// <param name="htmlLength">Length of data (buffer itself can be longer) - start offset assumed to be 0</param>
        public void Init(byte[] html, int htmlLength)
        {
            // set default encoding
            if (Enc == null)
                Enc = Encoding.Default;
            CleanUp();
            _html = html;
            // check whether we have got data that is actually in Unicode format
            // normally this would mean we have got plenty of zeroes
            // this and related code was contributed by Martin B�chtold from TTN Tele.Translator.Network
            if (_html.Length > 2)
                if (_html[0] == 255 && _html[1] == 254)
                    _html = Encoding.Default.GetBytes(Encoding.Unicode.GetString(_html, 2, _html.Length - 2));
            _dataLength = htmlLength;
            _tp.Init(this, _chunk, _text, _html, _dataLength, _e, _he);
        }

        /// <summary>
        /// Cleans up parser in preparation for next parsing
        /// </summary>
        public void CleanUp()
        {
            /*
            if (entities == null)
            {
                entities = InitEntities(ref minEntityLen, ref maxEntityLen, out entityReverseLookup);
                biniEntities = false;
            }
            */
            _tp.CleanUp();
            _html = null;
            _curPos = 0;
            _dataLength = 0;
        }

        /// <summary>
        /// Resets current parsed data to start
        /// </summary>
        public void Reset()
        {
            _curPos = 0;
        }

        /// <summary>
        /// Font sizes as described by W3C: http://www.w3.org/TR/REC-CSS2/fonts.html#propdef-font-size
        /// </summary>
        /// <exclude/>
        public enum FontSize
        {
            Small_xx = 0,
            Small_x = 1,
            Small = 2,
            Medium = 3,
            Large = 4,
            Large_x = 5,
            Large_xx = 6,
            Unknown = 7,
        }

        /// <summary>
        /// Checks if first font is bigger than the second
        /// </summary>
        /// <param name="font1">Font #1</param>
        /// <param name="font2">Font #2</param>
        /// <returns>True if Font #1 bigger than the second, false otherwise</returns>
        public static bool IsBiggerFont(FontSize font1, FontSize font2)
        {
            return (int)font1 > (int)font2;
        }

        /// <summary>
        /// Checks if first font is equal or bigger than the second
        /// </summary>
        /// <param name="font1">Font #1</param>
        /// <param name="font2">Font #2</param>
        /// <returns>True if Font #1 equal or bigger than the second, false otherwise</returns>
        public static bool IsEqualOrBiggerFont(FontSize font1, FontSize font2)
        {
            return (int)font1 >= (int)font2;
        }

        /// <summary>
        /// Parses font's tag size param 
        /// </summary>
        /// <param name="size">String value of the size param</param>
        /// <param name="iBaseFontSize">Optional base font size, use -1 if its not present</param>
        /// <param name="iCurSize"></param>
        /// <returns>Relative size of the font size or Unknown if it was not determined</returns>
        public static FontSize ParseFontSize(string size, FontSize curSize)
        {
            // TODO: read more http://www.w3.org/TR/REC-CSS2/fonts.html#propdef-font-size
            size = size.Trim();
            if (size.Length == 0)
                return FontSize.Unknown;
            // check if its relative or absolute value
            var sign = 0;
            var digits = string.Empty;
            var digitsSize = 0;
            for (var i = 0; i < size.Length; i++)
            {
                var c = size[i];
                if (char.IsWhiteSpace(c))
                    continue;
                switch (c)
                {
                    case '+':
                        if (sign == 0) sign = 1;
                        else return FontSize.Unknown;
                        break;
                    case '-':
                        if (sign == 0) sign = -1;
                        else return FontSize.Unknown;
                        break;
                    default:
                        if (char.IsDigit(c))
                        {
                            digitsSize++;
                            if (digits.Length == 0) digits = c.ToString();
                            else digits += c;
                        }
                        break;
                }
            }
            if (digits.Length == 0 || digitsSize == 0)
                return FontSize.Unknown;
            var fontSize = 0;
            if (digits.Length > 3)
                return FontSize.Unknown;
            var size2 = 0;
            try { size2 = int.Parse(digits); }
            catch { return FontSize.Unknown; }
            if (sign == 0)
                // absolute set
                fontSize = size2;
            else    // relative change 
            {
                if (sign < 0) fontSize = (int)curSize - size2;
                else fontSize = (int)curSize + size2;
            }
            // sanity check
            if (fontSize < 0) fontSize = 0;
            else if (fontSize > 6) fontSize = 6;
            return (FontSize)fontSize;
        }

        /// <summary>
        /// Returns next tag or null if end of document, text will be ignored completely
        /// </summary>
        /// <returns>Tag chunk or null</returns>
        public HTMLchunk ParseNextTag()
        {
            // skip till first < char
            while (_curPos < _dataLength)
                if (_html[_curPos++] == (byte)'<') // check if we have got tag start
                {
                    _chunk.ChunkOffset = _curPos - 1;
                    return GetNextTag();
                }
            return null;
        }

        /// <summary>
        /// Parses next chunk and returns it with 
        /// </summary>
        /// <returns>HTMLchunk or null if end of data reached</returns>
        public HTMLchunk ParseNext()
        {
            if (_curPos >= _dataLength)
                return null;
            _chunk.Clear();
            _chunk.ChunkOffset = _curPos;
            var c = _html[_curPos++];
            // most likely what we have here is a normal char, 
            if (c == (byte)'<')
                // tag parsing route - we know for sure that we have not had some text chars before 
                // that point to worry about
                return GetNextTag();
            else
            {
                // check if it's whitespace - typically happens after tag end and before new tag starts
                // so it makes sense make it special case
                if (CompressWhiteSpaceBeforeTag && c <= 32 && _whiteSpace[c] == 1)
                {
                    // ok first char is empty space, this can often lead to new tag
                    // thus causing us to create essentially empty strings where as we could have
                    // returned fixed single space string when it is necessary
                    while (_curPos < _dataLength)
                    {
                        c = _html[_curPos++];
                        if (c <= 32 && _whiteSpace[c] == 1)
                            continue;
                        // ok we got tag, but all we had before it was spaces, most likely end of lines
                        // so we will return compact representation of that text data
                        if (c == (byte)'<')
                        {
                            _curPos--;
                            _chunk.Type = HTMLchunkType.Text;
                            _chunk.Html = " ";
                            return _chunk;
                        }
                        break;
                    }
                }

                // ok normal text, we just scan it until tag or end of text
                // statistically this loop will have plenty of iterations
                // thus it makes sense to deal with pointers, we only do that if
                // we have got plenty of bytes to scan left
                var quadBytes = ((_dataLength - _curPos) >> 2) - 1;
                if (!_e.CanDecodeEntities && !_e.MiniEntities)
                {
                    while (_curPos < _dataLength)
                    {
                        // ok we got tag, but all we had before it was spaces, most likely end of lines
                        // so we will return compact representation of that text data
                        if (_html[_curPos++] == (byte)'<')
                        {
                            _curPos--;
                            break;
                        }
                    }
                }
                else
                {
                    // TODO: might help skipping data in quads but we need to perfect bitmap operations for that:
                    // stop when at least one & or < is detected in quad
                    /*
                    fixed (byte* bpData=&bHTML[iCurPos])
                    {
                        uint* uiData=(uint*)bpData;
                        for(int i=0; i<iQuadBytes; i++)
                        {
                            // use bitmask operation to quickly check if any of the 4 bytes
                            // has got < in them - should be FAIRLY unlikely thus allowing us to skip
                            // few bytes in one go
                            if((~(*uiData &  0x3C3C3C3C)) )
                            {
                                iCurPos+=4;
                                uiData++;
                                continue;
                            }
                            break;
                        }
                    }
                     */

                    // we might have entity here, which is first char of the text:
                    if (c == (byte)'&')
                    {
                        var lastCurPos = _curPos - 1;
                        var entityChar = _e.CheckForEntity(_html, ref _curPos, _dataLength);
                        // restore current symbol
                        if (entityChar != 0)
                        {
                            // ok, we have got entity on our hand, it means that we can't just copy
                            // data from start of the buffer to end of text thereby avoiding having to
                            // accumulate same data elsewhere
                            _text.Clear();
                            _chunk.Entities = true;
                            if (entityChar == (byte)'<')
                                _chunk.LtEntity = true;
                            _text.Append(entityChar);
                            return ParseTextWithEntities();
                        }
                    }

                    while (_curPos < _dataLength)
                    {
                        c = _html[_curPos++];
                        // ok we got tag, but all we had before it was spaces, most likely end of lines
                        // so we will return compact representation of that text data
                        if (c == (byte)'<')
                        {
                            _curPos--;
                            break;
                        }
                        // check if we got entity
                        if (c == (byte)'&')
                        {
                            var lastCurPos = _curPos - 1;
                            var entityChar = _e.CheckForEntity(_html, ref _curPos, _dataLength);
                            // restore current symbol
                            if (entityChar != 0)
                            {
                                // ok, we have got entity on our hand, it means that we can't just copy
                                // data from start of the buffer to end of text thereby avoiding having to
                                // accumulate same data elsewhere
                                _text.Clear();
                                var len = lastCurPos - _chunk.ChunkOffset;
                                if (len > 0)
                                {
                                    Array.Copy(_html, _chunk.ChunkOffset, _text._buffer, 0, len);
                                    _text._bufPos = len;
                                }
                                _chunk.Entities = true;
                                if (entityChar == (byte)'<')
                                    _chunk.LtEntity = true;
                                _text.Append(entityChar);
                                return ParseTextWithEntities();
                            }
                        }
                    }
                }
                _chunk.ChunkLength = _curPos - _chunk.ChunkOffset;
                if (_chunk.ChunkLength == 0)
                    return null;
                _chunk.Type = HTMLchunkType.Text;
                // oChunk.oHTML = oEnc.GetString(bHTML, oChunk.iChunkOffset, oChunk.iChunkLength);
                _chunk.Html = _originalHtml.Substring(_chunk.ChunkOffset, _chunk.ChunkLength);
                return _chunk;
            }
        }

        HTMLchunk ParseTextWithEntities()
        {
            // okay, now that we got our first entity we will need to continue
            // parsing by copying data into temporary buffer and when finished
            // convert it to string
            while (_curPos < _dataLength)
            {
                var c = _html[_curPos++];
                // ok we got tag, but all we had before it was spaces, most likely end of lines
                // so we will return compact representation of that text data
                if (c == (byte)'<')
                {
                    _curPos--;
                    break;
                }
                // check if we got entity again
                if (c == (byte)'&')
                {
                    var newEntityChar = _e.CheckForEntity(_html, ref _curPos, _dataLength);
                    // restore current symbol
                    if (newEntityChar != 0)
                    {
                        if (newEntityChar == (byte)'<')
                            _chunk.LtEntity = true;
                        _text.Append(newEntityChar);
                        // we continue here since we fully parsed entity
                        continue;
                    }
                    // ok we did not parse entity in which case we add & char and continue along the way
                    _text._buffer[_text._bufPos++] = c;
                    continue;
                }
                _text._buffer[_text._bufPos++] = c;
            }
            _chunk.ChunkLength = _curPos - _chunk.ChunkOffset;
            _chunk.Type = HTMLchunkType.Text;
            _chunk.Html = _text.SetToString();
            return _chunk;
        }

        /// <summary>
        /// Internally parses tag and returns it from point when open bracket (&lt;) was found
        /// </summary>
        /// <returns>Chunk</returns>
        HTMLchunk GetNextTag()
        {
            //curPos++;
            _chunk = _tp.ParseTag(ref _curPos);
            // for backwards compatibility mark closed tags with params as open
            if (_chunk.ParamsCount > 0 && AutoMarkClosedTagsWithParamsAsOpen && _chunk.Type == HTMLchunkType.CloseTag)
                _chunk.Type = HTMLchunkType.OpenTag;
            //                    012345
            // check for start of script
            if (_chunk.Tag.Length == 6 && _chunk.Tag[0] == 's' && _chunk.Tag == "script")
            {
                if (!_chunk.Closure)
                {
                    _chunk.Type = HTMLchunkType.Script;
                    _chunk = _tp.ParseScript(ref _curPos);
                    return _chunk;
                }
            }
            _chunk.ChunkLength = _curPos - _chunk.ChunkOffset;
            if (KeepRawHTML)
                _chunk.Html = Enc.GetString(_html, _chunk.ChunkOffset, _chunk.ChunkLength);
            return _chunk;

        }

        /// <summary>
        /// Parses WIDTH param and calculates width
        /// </summary>
        /// <param name="width">WIDTH param from tag</param>
        /// <param name="availWidth">Currently available width for relative calculations, if negative width will be returned as is</param>
        /// <param name="relative">Flag that will be set to true if width was relative</param>
        /// <returns>Width in pixels</returns>
        public static int CalculateWidth(string width, int availWidth, ref bool relative)
        {
            width = width.Trim();
            if (width.Length == 0)
                return availWidth;
            try
            {
                // check if its relative %-t                
                relative = false;
                for (var i = 0; i < width.Length; i++)
                {
                    if (width[i] == '%')
                        relative = true;
                    if (!char.IsNumber(width[i]))
                    {
                        width = width.Substring(0, i);
                        break;
                    }
                }
                var value = int.Parse(width);
                if (relative && availWidth > 0) return value * availWidth / 100;
                else return value;
            }
            catch { }
            return availWidth;
        }

        /// <summary>
        /// This function will decode any entities found in a string - not fast!
        /// </summary>
        /// <returns>Possibly decoded string</returns>
        public static string DecodeEntities(string data)
        {
            return HTMLentities.DecodeEntities(data);
        }

        /// <summary>
        /// Loads HTML from file
        /// </summary>
        /// <param name="sFileName">Full filename</param>
        public void LoadFromFile(string sFileName)
        {
            CleanUp();
            using (var s = File.OpenRead(sFileName))
            {
                var data = new byte[s.Length];
                var read = s.Read(data, 0, data.Length);
                if (read != data.Length)
                    throw new Exception("Number of bytes read is less than expected number of bytes in a file!");
                Init(data);
            }
            return;
        }

        /// <summary>
        /// Handles META tags that set page encoding
        /// </summary>
        /// <param name="p">HTML parser object that is used for parsing</param>
        /// <param name="chunk">Parsed chunk that should contain tag META</param>
        /// <param name="encodingSet">Your own flag that shows whether encoding was already set or not, if set
        /// once then it should not be changed - this is the logic applied by major browsers</param>
        /// <returns>True if this was META tag setting Encoding, false otherwise</returns>
        public static bool HandleMetaEncoding(HTMLparser p, HTMLchunk chunk, ref bool encodingSet)
        {
            if (chunk.Tag.Length != 4 || chunk.Tag[0] != 'm' || chunk.Tag != "meta")
                return false;
            // if we do not use hashmode already then we call conversion explicitly
            // this is slow, but METAs are very rare so performance penalty is low
            if (!chunk.HashMode)
                chunk.ConvertParamsToHash();
            var key = chunk.Params["http-equiv"] as string;
            if (key != null)
            {
                // FIXIT: even though this is happening rare I really don't like lower casing stuff
                // that most likely would not need to be - if you feel bored then rewrite this bit
                // to make it faster, it is really easy...
                switch (key.ToLower())
                {
                    case "content-type":
                    // rare case (appears to work in IE) reported to exist in some pages by Martin B�chtold
                    case "content-category":
                        // we might have charset here that may hint at necessity to decode page
                        // check for possible encoding change

                        // once encoding is set it should not be changed, but you can be damn
                        // sure there are web pages out there that do that!!!
                        if (!encodingSet)
                        {
                            var data = chunk.Params["content"] as string;
                            // it is possible we have broken META tag without Content part
                            if (data != null)
                            {
                                if (p.SetEncoding(data))
                                {
                                    // we may need to re-encode title
                                    if (!encodingSet)
                                    {
                                        // here you need to reencode any text that you found so far
                                        // most likely it will be just TITLE, the rest can be ignored anyway
                                        encodingSet = true;
                                    }
                                }
                                else
                                {
                                    // failed to set encoding - most likely encoding string
                                    // was incorrect or your machine lacks codepages or something
                                    // else - might be good idea to put warning message here
                                }
                            }
                        }
                        return true;
                    default: break;
                }
            }
            return false;
        }
    }
}
