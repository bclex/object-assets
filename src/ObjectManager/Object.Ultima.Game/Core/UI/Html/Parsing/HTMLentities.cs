using System;
using System.Globalization;
using System.Text;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// Implements parsing of entities
    /// </summary>
    public class HTMLentities
    {
        /// <summary>
        /// Supported HTML entities
        /// </summary>
        FastHash Entities;

        /// <summary>
        /// Supported HTML entities
        /// </summary>
        public static FastHash AllEntities;

        /// <summary>
        /// Internal heuristics for entiries: these will be set to min and max string lengths of known HTML entities
        /// </summary>
        int _minEntityLen = 0, _maxEntityLen;

        static int _allMinEntityLen = 0, _allMaxEntityLen;

        /// <summary>
        /// Array to provide reverse lookup for entities
        /// </summary>
        public static string[] EntityReverseLookup;

        /// <summary>
        /// If true then only minimal set of entities will be parsed, everything else including numbers based
        /// entities will be returned as is. This is useful for when HTML content needs to be extracted with subsequent parsing, in this case resolution of entities will be a problem
        /// </summary>
        internal bool MiniEntities;

        /// <summary>
        /// If false then HTML entities (like "nbsp") will not be decoded
        /// </summary>
        internal bool CanDecodeEntities;

        static HTMLentities()
        {
            AllEntities = InitEntities(ref _allMinEntityLen, ref _allMaxEntityLen, out EntityReverseLookup);
        }

        internal HTMLentities()
        {
            Entities = InitEntities(ref _minEntityLen, ref _maxEntityLen, out EntityReverseLookup);
            MiniEntities = false;
        }
        /// <summary>
        /// This function will be called when & is found, and it will
        /// peek forward to check if its entity, should there be a success
        /// indicated by non-zero returned, the pointer will be left at the new byte
        /// after entity
        /// </summary>
        /// <returns>Char (not byte) that corresponds to the entity or 0 if it was not entity</returns>
        internal char CheckForEntity(byte[] html, ref int curPos, int dataLength)
        {
            if (!CanDecodeEntities && !MiniEntities)
                return (char)0;
            var chars = 0;
            byte c;
            // if true it means we are getting hex or decimal value of the byte
            var charCode = false;
            var charCodeHex = false;
            int entLen = 0;
            int from = curPos;
            string entity;
            try
            {
                /*
                while (!Eof())
                {
                    c = NextChar();
                */
                while (curPos < dataLength)
                {
                    c = html[curPos++];
                    // 21/10/05: not necessary
                    //if (c == 0)
                    //    break;
                    if (++chars <= 2)
                    {
                        // the first byte for numbers should be #
                        if (chars == 1)
                        {
                            if (c == '#')
                            {
                                from++;
                                charCode = true;
                                continue;
                            }
                        }
                        else
                        {
                            if (charCode && c == 'x')
                            {
                                from++;
                                entLen--;
                                charCodeHex = true;
                            }
                        }
                    }

                    //Console.WriteLine("Got entity end: {0}",sEntity);
                    // Break on:
                    // 1) ; - proper end of entity
                    // 2) number 10-based entity but current byte is not a number
                    // TODO: browsers appear to be lax about ; requirement for end of entity 
                    // we should really do the same and treat whitespace as termination of entity
                    if (c == ';' || (charCode && !charCodeHex && !(c >= '0' && c <= '9')))
                    {
                        // lets try speculative quick lookup using just first 2 chars
                        // this should be successful in almost all cases thus removing need for
                        // expensive creation of a string 
                        if (!charCode && entLen > 1)
                        {
                            var v = Entities.GetLikelyPresentValue(html[from], html[from + 1]);
                            if (v != null)
                                return (char)((int)v);
                        }

                        // check if its int - this way we can avoid having to create string 
                        if (charCode && entLen > 0 && !charCodeHex)
                        {
                            // if mini entities mode is set then we will ignore all numerics
                            if (MiniEntities)
                                break;
                            // we have to backdown one char in case when entity did not end with ; 
                            // otherwise we will lose next char in the stream, this correction suggested by Kurt Carlson! 
                            if (c != ';')
                                curPos--;
                            return (char)ParseUInt(html, from, entLen);
                        }

                        entity = Encoding.Default.GetString(html, from, entLen);
                        if (charCode)
                        {
                            // NOTE: this may fail due to wrong data format,
                            // in which case we will return 0, and entity will be
                            // ignored
                            if (entLen > 0)
                            {
                                // if mini entities mode is set then we will ignore all numerics
                                if (MiniEntities)
                                    break;
                                int cAsInt;
                                if (!charCodeHex)
                                {
#if true
                                    // we want to avoid exceptions if possible as they are slow
                                    if (!int.TryParse(entity, out cAsInt))
                                    {
                                        if (chars > 0)
                                        {
                                            if ((curPos - chars) >= 0)
                                                curPos -= chars;
                                            //PutChars(chars);
                                        }
                                        return (char)(0);
                                    }
#else
                                    cAsInt = int.Parse(entity);
#endif
                                }
                                else
                                {
#if true
                                    // we want to avoid exceptions if possible as they are very slow
                                    if (!int.TryParse(entity, NumberStyles.HexNumber, null, out cAsInt))
                                    {
                                        if (chars > 0)
                                        {
                                            if ((curPos - chars) >= 0)
                                                curPos -= chars;
                                            //PutChars(chars);
                                        }
                                        return (char)(0);
                                    }
#else

                                    cAsInt = int.Parse(entity, NumberStyles.HexNumber);
#endif
                                }
                                return (char)cAsInt;
                            }
                        }
                        if (entLen >= _minEntityLen && entLen <= _maxEntityLen)
                        {
                            var v = Entities.GetLikelyPresentValue(entity);
                            if (v != null)
                                return (char)((int)v);
                        }
                    }
                    // as soon as entity length exceed max length of entity known to us
                    // we break up the loop and return nothing found
                    // NOTE: removed due to entities being generally correct and this code costs 10% of CPU in this function
                    if (entLen > _maxEntityLen)
                        break;
                    entLen++;
                }
            }
            catch //(Exception e)
            {
                //Console.WriteLine("Entity parsing exception: " + e.ToString());
            }
            // if we have not found squat, then we will need to put point back
            // to where it was before this function was called
            if (chars > 0)
            {
                if ((curPos - chars) >= 0)
                    curPos -= chars;
                //PutChars(chars);
            }
            return (char)(0);
        }

        /// <summary>
        /// This function will decode any entities found in a string - not fast!
        /// </summary>
        /// <returns>Possibly decoded string</returns>
        internal static string DecodeEntities(string data)
        {
            char c;
            var b = new StringBuilder(data.Length);
            var entity = string.Empty;
            try
            {
                for (var i = 0; i < data.Length; i++)
                {
                    c = data[i];
                    if (c != '&' || (i + 1 >= data.Length))
                        b.Append(c);
                    else
                    {
                        // if true it means we are getting hex or decimal value of the byte
                        var charCode = false;
                        var charCodeHex = false;
                        var entLen = 0;
                        var chars = 0;
                        var j = i + 1;
                        var from = i + 1;
                        for (; j < data.Length; j++)
                        {
                            c = data[j];
                            if (++chars <= 2)
                            {
                                // the first byte for numbers should be #
                                if (chars == 1)
                                {
                                    if (c == '#')
                                    {
                                        from++;
                                        charCode = true;
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (charCode && c == 'x' && !charCodeHex)
                                    {
                                        from++;
                                        //entLen--;
                                        charCodeHex = true;
                                        continue;
                                    }
                                }
                            }

                            //Console.WriteLine("Got entity end: {0}",sEntity);
                            // Break on:
                            // 1) ; - proper end of entity
                            // 2) number 10-based entity but current byte is not a number
                            var lastChar = j + 1 >= data.Length;
                            if (c == ';' || (charCode && !charCodeHex && !(c >= '0' && c <= '9')) || (charCode && lastChar))
                            {
                                // end of string 
                                if (lastChar && c != ';')
                                    entLen++;
                                // lets try speculative quick lookup using just first 2 chars
                                // this should be successful in almost all cases thus removing need for
                                // expensive creation of a string 
                                if (!charCode && entLen > 1)
                                {
                                    // make sure we aint at the end of string
                                    if (i + 2 < data.Length)
                                    {
                                        var v = AllEntities.GetLikelyPresentValue((byte)data[i + 1], (byte)data[i + 2]);
                                        if (v != null)
                                        {
                                            b.Append((char)((int)v));
                                            break;
                                        }

                                    }
                                }

                                // check if its int - this way we can avoid having to create string 
                                if (charCode && entLen > 0 && !charCodeHex)
                                {
                                    entity = data.Substring(from, entLen);
                                    var cAsInt = 0;
                                    var success = false;
                                    try
                                    {
                                        cAsInt = (int)uint.Parse(entity);
                                        success = true;
                                    }
                                    catch { }
                                    if (success)
                                    {
                                        b.Append((char)cAsInt);
                                        // move back once when we got number done without ; at the end
                                        // of it - Firefox and IE do it this way
                                        if (c != ';' && !lastChar)
                                            j--;
                                        break;
                                    }
                                    else
                                    {
                                        // this will force to add entity as is - probably broken
                                        // or maybe not entity at all
                                        b.Append('&');
                                        j = i;
                                        break;
                                    }
                                }
                                entity = data.Substring(from, entLen);
                                if (charCode)
                                {
                                    // NOTE: this may fail due to wrong data format,
                                    // in which case we will return 0, and entity will be
                                    // ignored
                                    if (entLen > 0)
                                    {
                                        var cAsInt = 0;
                                        var success = false;
#if false
                                        if (!charCodeHex)  success = int.TryParse(sntity, out cAsInt);
                                        else success = int.TryParse(entity, NumberStyles.HexNumber, out cAsInt);
#else
                                        try
                                        {
                                            if (!charCodeHex) cAsInt = int.Parse(entity);
                                            else cAsInt = int.Parse(entity, NumberStyles.HexNumber);
                                            success = true;
                                        }
                                        catch { } // some numbers might not be parsed correctly so we will ignore them
#endif
                                        if (success)
                                        {
                                            b.Append((char)cAsInt);
                                            break;
                                        }
                                        // this will force to add entity as is - probably broken or maybe not entity at all
                                        else entLen = _allMaxEntityLen + 1;
                                    }
                                }
                                if (entLen >= _allMinEntityLen && entLen <= _allMaxEntityLen)
                                {
                                    var v = AllEntities.GetLikelyPresentValue(entity);
                                    if (v != null)
                                    {
                                        b.Append((char)((int)v));
                                        break;
                                    }
                                    // this will force to add entity as is - probably broken or maybe not entity at all
                                    else entLen = _allMaxEntityLen + 1;
                                }
                            }

                            // as soon as entity length exceed max length of entity known to us
                            // we break up the loop and return nothing found
                            // NOTE: removed due to entities being generally correct and this code costs 10% of CPU in this function
                            if (entLen > _allMaxEntityLen || lastChar)
                            {
                                // append char that triggered entity thingy in the first place
                                b.Append('&');
                                j = i;
                                break;
                            }
                            entLen++;
                        }
                        i = j;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Entity parsing exception: " + e.ToString());
                return data;
            }
            return b.ToString();
        }

        /// <summary>
        /// Multipliers for base 10 
        /// </summary>
        static uint[] DecMultipliers = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        /// <summary>
        /// Parses an unsigned integer number from byte buffer
        /// </summary>
        /// <param name="buf">Buffer to parse from</param>
        /// <param name="from">Start parsing from this point</param>
        /// <param name="length">Length of data to parse</param>
        /// <returns>Unsigned integer number</returns>
        uint ParseUInt(byte[] buf, int from, int length)
        {
            uint num = 0;
            var order = 0;
            for (var i = from + length - 1; i >= from; i--)
            {
                var b = buf[i];
                if (b < (byte)'0' || b > (byte)'9')
                    break;
                //Console.WriteLine("Order={0}, Byte={1}", order, b);
                num += (uint)(DecMultipliers[order++] * (b - (byte)'0'));
            }
            return num;
        }

        /// <summary>
        /// Initialises list of entities
        /// </summary>
        private static FastHash InitEntities(ref int minEntityLen, ref int maxEntityLen, out string[] entityReverseLookup)
        {
            var entities = new FastHash();
            // FIXIT: we will treat non-breakable space... as space!?!
            // perhaps it would be better to have separate return types for entities?
            entities.Add("nbsp", 32); //entities.Add("nbsp",160);
            entities.Add("iexcl", 161);
            entities.Add("cent", 162);
            entities.Add("pound", 163);
            entities.Add("curren", 164);
            entities.Add("yen", 165);
            entities.Add("brvbar", 166);
            entities.Add("sect", 167);
            entities.Add("uml", 168);
            entities.Add("copy", 169);
            entities.Add("ordf", 170);
            entities.Add("laquo", 171);
            entities.Add("not", 172);
            entities.Add("shy", 173);
            entities.Add("reg", 174);
            entities.Add("macr", 175);
            entities.Add("deg", 176);
            entities.Add("plusmn", 177);
            entities.Add("sup2", 178);
            entities.Add("sup3", 179);
            entities.Add("acute", 180);
            entities.Add("micro", 181);
            entities.Add("para", 182);
            entities.Add("middot", 183);
            entities.Add("cedil", 184);
            entities.Add("sup1", 185);
            entities.Add("ordm", 186);
            entities.Add("raquo", 187);
            entities.Add("frac14", 188);
            entities.Add("frac12", 189);
            entities.Add("frac34", 190);
            entities.Add("iquest", 191);
            entities.Add("Agrave", 192);
            entities.Add("Aacute", 193);
            entities.Add("Acirc", 194);
            entities.Add("Atilde", 195);
            entities.Add("Auml", 196);
            entities.Add("Aring", 197);
            entities.Add("AElig", 198);
            entities.Add("Ccedil", 199);
            entities.Add("Egrave", 200);
            entities.Add("Eacute", 201);
            entities.Add("Ecirc", 202);
            entities.Add("Euml", 203);
            entities.Add("Igrave", 204);
            entities.Add("Iacute", 205);
            entities.Add("Icirc", 206);
            entities.Add("Iuml", 207);
            entities.Add("ETH", 208);
            entities.Add("Ntilde", 209);
            entities.Add("Ograve", 210);
            entities.Add("Oacute", 211);
            entities.Add("Ocirc", 212);
            entities.Add("Otilde", 213);
            entities.Add("Ouml", 214);
            entities.Add("times", 215);
            entities.Add("Oslash", 216);
            entities.Add("Ugrave", 217);
            entities.Add("Uacute", 218);
            entities.Add("Ucirc", 219);
            entities.Add("Uuml", 220);
            entities.Add("Yacute", 221);
            entities.Add("THORN", 222);
            entities.Add("szlig", 223);
            entities.Add("agrave", 224);
            entities.Add("aacute", 225);
            entities.Add("acirc", 226);
            entities.Add("atilde", 227);
            entities.Add("auml", 228);
            entities.Add("aring", 229);
            entities.Add("aelig", 230);
            entities.Add("ccedil", 231);
            entities.Add("egrave", 232);
            entities.Add("eacute", 233);
            entities.Add("ecirc", 234);
            entities.Add("euml", 235);
            entities.Add("igrave", 236);
            entities.Add("iacute", 237);
            entities.Add("icirc", 238);
            entities.Add("iuml", 239);
            entities.Add("eth", 240);
            entities.Add("ntilde", 241);
            entities.Add("ograve", 242);
            entities.Add("oacute", 243);
            entities.Add("ocirc", 244);
            entities.Add("otilde", 245);
            entities.Add("ouml", 246);
            entities.Add("divide", 247);
            entities.Add("oslash", 248);
            entities.Add("ugrave", 249);
            entities.Add("uacute", 250);
            entities.Add("ucirc", 251);
            entities.Add("uuml", 252);
            entities.Add("yacute", 253);
            entities.Add("thorn", 254);
            entities.Add("yuml", 255);
            entities.Add("quot", 34);

            // NOTE: this is a not a proper entity but a fairly common mistake - & is important symbol
            // and we don't want to lose it even if webmaster used upper case instead of lower
            entities.Add("AMP", 38);
            entities.Add("REG", 174);

            entities.Add("amp", 38);
            entities.Add("reg", 174);

            entities.Add("lt", 60);
            entities.Add("gt", 62);
            // ' - apparently does not work in IE
            entities.Add("apos", 39);

            // unicode supported by default
            if (true)
            {
                entities.Add("OElig", 338);
                entities.Add("oelig", 339);
                entities.Add("Scaron", 352);
                entities.Add("scaron", 353);
                entities.Add("Yuml", 376);
                entities.Add("circ", 710);
                entities.Add("tilde", 732);
                entities.Add("ensp", 8194);
                entities.Add("emsp", 8195);
                entities.Add("thinsp", 8201);
                entities.Add("zwnj", 8204);
                entities.Add("zwj", 8205);
                entities.Add("lrm", 8206);
                entities.Add("rlm", 8207);
                entities.Add("ndash", 8211);
                entities.Add("mdash", 8212);
                entities.Add("lsquo", 8216);
                entities.Add("rsquo", 8217);
                entities.Add("sbquo", 8218);
                entities.Add("ldquo", 8220);
                entities.Add("rdquo", 8221);
                entities.Add("bdquo", 8222);
                entities.Add("dagger", 8224);
                entities.Add("Dagger", 8225);
                entities.Add("permil", 8240);
                entities.Add("lsaquo", 8249);
                entities.Add("rsaquo", 8250);
                entities.Add("euro", 8364);
                entities.Add("fnof", 402);
                entities.Add("Alpha", 913);
                entities.Add("Beta", 914);
                entities.Add("Gamma", 915);
                entities.Add("Delta", 916);
                entities.Add("Epsilon", 917);
                entities.Add("Zeta", 918);
                entities.Add("Eta", 919);
                entities.Add("Theta", 920);
                entities.Add("Iota", 921);
                entities.Add("Kappa", 922);
                entities.Add("Lambda", 923);
                entities.Add("Mu", 924);
                entities.Add("Nu", 925);
                entities.Add("Xi", 926);
                entities.Add("Omicron", 927);
                entities.Add("Pi", 928);
                entities.Add("Rho", 929);
                entities.Add("Sigma", 931);
                entities.Add("Tau", 932);
                entities.Add("Upsilon", 933);
                entities.Add("Phi", 934);
                entities.Add("Chi", 935);
                entities.Add("Psi", 936);
                entities.Add("Omega", 937);
                entities.Add("alpha", 945);
                entities.Add("beta", 946);
                entities.Add("gamma", 947);
                entities.Add("delta", 948);
                entities.Add("epsilon", 949);
                entities.Add("zeta", 950);
                entities.Add("eta", 951);
                entities.Add("theta", 952);
                entities.Add("iota", 953);
                entities.Add("kappa", 954);
                entities.Add("lambda", 955);
                entities.Add("mu", 956);
                entities.Add("nu", 957);
                entities.Add("xi", 958);
                entities.Add("omicron", 959);
                entities.Add("pi", 960);
                entities.Add("rho", 961);
                entities.Add("sigmaf", 962);
                entities.Add("sigma", 963);
                entities.Add("tau", 964);
                entities.Add("upsilon", 965);
                entities.Add("phi", 966);
                entities.Add("chi", 967);
                entities.Add("psi", 968);
                entities.Add("omega", 969);
                entities.Add("thetasym", 977);
                entities.Add("upsih", 978);
                entities.Add("piv", 982);
                entities.Add("bull", 8226);
                entities.Add("hellip", 8230);
                entities.Add("prime", 8242);
                entities.Add("Prime", 8243);
                entities.Add("oline", 8254);
                entities.Add("frasl", 8260);
                entities.Add("weierp", 8472);
                entities.Add("image", 8465);
                entities.Add("real", 8476);
                entities.Add("trade", 8482);
                entities.Add("alefsym", 8501);
                entities.Add("larr", 8592);
                entities.Add("uarr", 8593);
                entities.Add("rarr", 8594);
                entities.Add("darr", 8595);
                entities.Add("harr", 8596);
                entities.Add("crarr", 8629);
                entities.Add("lArr", 8656);
                entities.Add("uArr", 8657);
                entities.Add("rArr", 8658);
                entities.Add("dArr", 8659);
                entities.Add("hArr", 8660);
                entities.Add("forall", 8704);
                entities.Add("part", 8706);
                entities.Add("exist", 8707);
                entities.Add("empty", 8709);
                entities.Add("nabla", 8711);
                entities.Add("isin", 8712);
                entities.Add("notin", 8713);
                entities.Add("ni", 8715);
                entities.Add("prod", 8719);
                entities.Add("sum", 8721);
                entities.Add("minus", 8722);
                entities.Add("lowast", 8727);
                entities.Add("radic", 8730);
                entities.Add("prop", 8733);
                entities.Add("infin", 8734);
                entities.Add("ang", 8736);
                entities.Add("and", 8743);
                entities.Add("or", 8744);
                entities.Add("cap", 8745);
                entities.Add("cup", 8746);
                entities.Add("int", 8747);
                entities.Add("there4", 8756);
                entities.Add("sim", 8764);
                entities.Add("cong", 8773);
                entities.Add("asymp", 8776);
                entities.Add("ne", 8800);
                entities.Add("equiv", 8801);
                entities.Add("le", 8804);
                entities.Add("ge", 8805);
                entities.Add("sub", 8834);
                entities.Add("sup", 8835);
                entities.Add("nsub", 8836);
                entities.Add("sube", 8838);
                entities.Add("supe", 8839);
                entities.Add("oplus", 8853);
                entities.Add("otimes", 8855);
                entities.Add("perp", 8869);
                entities.Add("sdot", 8901);
                entities.Add("lceil", 8968);
                entities.Add("rceil", 8969);
                entities.Add("lfloor", 8970);
                entities.Add("rfloor", 8971);
                entities.Add("lang", 9001);
                entities.Add("rang", 9002);
                entities.Add("loz", 9674);
                entities.Add("spades", 9824);
                entities.Add("clubs", 9827);
                entities.Add("hearts", 9829);
                entities.Add("diams", 9830);
            }
            entityReverseLookup = new string[10000];
            // calculate min/max lenght of known entities
            foreach (string key in entities.Keys)
            {
                if (key.Length < minEntityLen || minEntityLen == 0)
                    minEntityLen = key.Length;
                if (key.Length > maxEntityLen || maxEntityLen == 0)
                    maxEntityLen = key.Length;
                // remember key at given offset
                if (key != "AMP" && key != "REG")
                    entityReverseLookup[(int)entities[key]] = key;
            }
            // we don't want to change spaces
            entityReverseLookup[32] = null;
            return entities;
        }

        /// <summary>
        /// Parses line and changes known entiry characters into proper HTML entiries
        /// </summary>
        /// <param name="line">Line of text</param>
        /// <param name="from">Char from which scanning should start</param>
        /// <returns>Line of text with proper HTML entities</returns>
        internal string ChangeToEntities(string line, int from, bool changeDangerousCharsOnly)
        {
            var b = new StringBuilder(line.Length);
            if (from > 0)
                b.Append(line.Substring(0, from));
            for (var i = from; i < line.Length; i++)
            {
                var c = line[i];
                // yeah I know its lame but its 3:30am and I had v.long debugging session :-/
                switch ((int)c)
                {
                    case 39:
                    case 145:
                    case 146:
                    case 147:
                    case 148:
                        b.Append("&#" + ((int)c).ToString() + ";");
                        continue;
                    default:
                        if (c < 32) // || (changeAllNonASCII && c > 127))
                        {
                            //c = (char)0x01;
                            goto case 148;
                        }
                        break;
                }
                if (c < EntityReverseLookup.Length)
                {
                    if (EntityReverseLookup[c] != null)
                    {
                        // 04/07/07 This seems to provide greater compatibility with crappy parsers like that used in PHP 4: it was choking on &raquo;    
                        if (changeDangerousCharsOnly)
                        {
                            switch (c)
                            {
                                case '>':
                                case '<':
                                case '\"':
                                case '\'':
                                case '/':
                                case '&':
                                    break;
                                // ignore most of chars then - they should be encoded using encoding then, not entities
                                default:
                                    if (c < 127)
                                        break;
                                    b.Append(c);
                                    continue;
                            }
                        }

                        // 14/05/08 we use numeric entities above ASCII level this is safer way - PHP XML parser was dieing on proper entities
                        if (!changeDangerousCharsOnly)
                        {
                            b.Append("&");
                            b.Append(EntityReverseLookup[c]);
                            b.Append(";");
                        }
                        else b.Append("&#" + ((int)c).ToString() + ";");
                        /*
                        b.Append("&");
                        b.Append(entityReverseLookup[c]);
                        b.Append(";");
                        */
                        continue;
                    }
                }
                b.Append(c);
            }
            return b.ToString();
        }

        /// <summary>
        /// Inits mini-entities mode: only "nbsp" will be converted into space, all other entities 
        /// will be left as is
        /// </summary>
        internal void InitMiniEntities()
        {
            Entities = new FastHash();
            Entities.Add("nbsp", 32);
            MiniEntities = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool Disposed;

        private void Dispose(bool bisposing)
        {
            if (!Disposed)
            {
                Disposed = true;
                if (Entities != null)
                {
                    Entities.Dispose();
                    Entities = null;
                }
            }
        }
    }
}
