using System;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// Internal class used to parse tag itself from the point it was found in HTML
    /// The main reason for this class is to split very long HTMLparser file into parts that are reasonably
    /// self-contained
    /// </summary>
    class TagParser : IDisposable
    {
        HTMLparser _p;
        HTMLchunk _chunk;
        DynaString _text;
        byte[] _html;
        int _dataLength;

        /// <summary>
        /// Minimum data size for heuristics engine to kick in
        /// </summary>
        const int MIN_DATA_SIZE_FOR_HEURISTICS = 8;

        /// <summary>
        /// Max data length for heuristical checks
        /// </summary>
        int _maxHeuDataLength;

        //byte[] _whiteSpace = null;
        HTMLentities _e;
        HTMLheuristics _he;

        /// <summary>
        /// Tag char types lookup table: allows one off lookup to determine if char used in tag is acceptable
        /// </summary>
        static byte[] _tagCharTypes = new byte[256];

        /// <summary>
        /// If true then heuristics engine will be used to match tags quicker
        /// </summary>
        internal bool EnableHeuristics = true;

        enum TagCharType
        {
            /// <summary>
            /// Unclassified
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Whitespace
            /// </summary>
            WhiteSpace = 1,

            /// <summary>
            /// Lower case or digit 
            /// </summary>
            LowerCasedASCIIorDigit = 2,

            /// <summary>
            /// semicolon - used for namespaces, ie: <namespace:a>
            /// </summary>
            NameSpaceColon = 3,

            // anything above will be uppercased ASCII returned value is a lower cased char
        }

        static TagParser()
        {
            // whitespace
            _tagCharTypes[' '] = (byte)TagCharType.WhiteSpace;
            _tagCharTypes['\t'] = (byte)TagCharType.WhiteSpace;
            _tagCharTypes[13] = (byte)TagCharType.WhiteSpace;
            _tagCharTypes[10] = (byte)TagCharType.WhiteSpace;
            _tagCharTypes[(byte)':'] = (byte)TagCharType.NameSpaceColon;
            for (var i = 33; i < 127; i++)
            {
                if (char.IsDigit((char)i) || (i >= (65 + 32) && i <= (90 + 32)))
                {
                    _tagCharTypes[i] = (byte)TagCharType.LowerCasedASCIIorDigit;
                    continue;
                }
                // UPPER CASED ASCII
                if (i >= 65 && i <= 90)
                {
                    _tagCharTypes[i] = (byte)(i + 32);
                    continue;
                }
            }
        }

        internal TagParser()
        {
        }

        /// <summary>
        /// Inits tag parser
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="text"></param>
        internal void Init(HTMLparser p, HTMLchunk chunk, DynaString text, byte[] html, int dataLength, HTMLentities e, HTMLheuristics he)
        {
            _p = p;
            _chunk = chunk;
            _text = text;
            _html = html;
            _dataLength = dataLength;
            // we don't want to be too close to end of data when dealing with heuristics
            _maxHeuDataLength = _dataLength - MIN_DATA_SIZE_FOR_HEURISTICS;
            _e = e;
            _he = he;
        }

        /// <summary>
        /// Cleans up tag parser
        /// </summary>
        internal void CleanUp()
        {
            _html = null;
            _dataLength = 0;
        }

        /// <summary>
        /// Internal: parses tag that started from current position
        /// </summary>
        /// <returns>HTMLchunk with tag information</returns>
        internal HTMLchunk ParseTag(ref int curPos)
        {
            /*
             *  WARNING: this code was optimised for performance rather than for readability, 
             *  so be extremely careful at changing it -- your changes could easily result in wrongly parsed HTML
             * 
             *  This routine takes about 60% of CPU time, in theory its the best place to gain extra speed,
             *  but I've spent plenty of time doing it, so it won't be easy... and if it is easy then please post
             *  your changes for everyone to enjoy!
             * */

            //var whiteSpaceHere = false;

            //var paramValue = false;
            byte c = 0;
            byte cPeek = 0;

            // if true it means we have parsed complete tag
            //var gotTag = false;

            //var equalIdx = 0;

            // we reach this function immediately after tag's byte (<) was
            // detected, so we need to save it in order to keep correct HTML copy
            // _hunk.Append((byte)'<'); // (byte)'<'

            /*
            _chunk.Buffer[0] = 60;
            _chunk.BufPos = 1;
            _chunk.HTMLen = 1;
            */

            // initialise peeked char - this will point to the next after < character
            if (curPos < _dataLength)
            {
                cPeek = _html[curPos];
                // in case of comments ! must follow immediately after <
                if (cPeek == (byte)'!')
                {
                    if (curPos + 2 < _dataLength &&
                        _html[curPos + 1] == (byte)'-' && _html[curPos + 2] == (byte)'-')
                    {
                        // we detected start of comments here, instead of parsing the rest here we will
                        // call special function tuned to do the job much more effectively
                        _chunk.Tag = "!--";
                        _chunk.Type = HTMLchunkType.Comment;
                        _chunk.Comments = true;
                        // _chunk.Append((byte)'!');
                        // _chunk.Append((byte)'-');
                        // _chunk.Append((byte)'-');
                        curPos += 3;
                        _chunk = ParseComments(ref curPos, out bool fullTag);
                        _chunk.ChunkLength = curPos - _chunk.ChunkOffset;
                        if (_p.AutoKeepComments || _p.KeepRawHTML)
                        {
                            if (!_p.AutoExtractBetweenTagsOnly) _chunk.Html = GetString(_chunk.ChunkOffset, _chunk.ChunkLength);
                            else _chunk.Html = GetString(_chunk.ChunkOffset + 4, _chunk.ChunkLength - fullTag ? 7 : 4);
                        }
                        return _chunk;
                    }

                    // ok we might have here CDATA element of XML:
                    // ref: http://www.w3schools.com/xml/xml_cdata.asp
                    if (curPos + 7 < _dataLength &&
                        _html[curPos + 1] == (byte)'[' &&
                        _html[curPos + 2] == (byte)'C' &&
                        _html[curPos + 3] == (byte)'D' &&
                        _html[curPos + 4] == (byte)'A' &&
                        _html[curPos + 5] == (byte)'T' &&
                        _html[curPos + 6] == (byte)'A' &&
                        _html[curPos + 7] == (byte)'[')
                    {
                        // we detected start of comments here, instead of parsing the rest here we will
                        // call special function tuned to do the job much more effectively
                        _chunk.Tag = "![CDATA[";
                        _chunk.Type = HTMLchunkType.Comment;
                        _chunk.Comments = true;
                        // _chunk.Append((byte)'!');
                        // _chunk.Append((byte)'-');
                        // _chunk.Append((byte)'-');
                        curPos += 8;
                        _chunk = ParseCDATA(ref curPos, out bool fullTag);
                        _chunk.ChunkLength = curPos - _chunk.ChunkOffset;
                        if (_p.AutoKeepComments || _p.KeepRawHTML)
                        {
                            if (!_p.AutoExtractBetweenTagsOnly) _chunk.Html = GetString(_chunk.ChunkOffset, _chunk.ChunkLength);
                            else _chunk.Html = GetString(_chunk.ChunkOffset + 4 + 5, _chunk.ChunkLength - fullTag ? 7 + 5 : 4 + 5);
                        }
                        return _chunk;
                    }
                }
            }
            else
            {
                // empty tag but its not closed, so we will call it open...
                _chunk.Type = HTMLchunkType.OpenTag;
                // end of data... before it started
                return _chunk;
            }

            // tag ID, non-zero if matched by heuristics engine
            var tagId = 0;

            // STAGE 0: lets try some heuristics to see if we can quickly identify most common tags
            // that should be present most of the time, this should save a lot of looping and string creation
            if (EnableHeuristics && curPos < _maxHeuDataLength)
            {
                // check if we have got closure of the tag
                if (cPeek == (byte)'/')
                {
                    _chunk.Closure = true;
                    _chunk.EndClosure = false;
                    _chunk.Type = HTMLchunkType.CloseTag;
                    curPos++;
                    cPeek = _html[curPos];
                }
                c = _html[curPos + 1];
                // probability of having a match is very high (or so we expect)
                tagId = _he.MatchTag(cPeek, c);
                if (tagId != 0)
                {
                    if (tagId < 0)
                    {
                        tagId *= -1;
                        // single character tag
                        _chunk.Tag = _he.GetString(tagId);
                        // see if we got fully closed tag
                        if (c == (byte)'>')
                        {
                            curPos += 2;
                            goto ReturnChunk;
                        }
                        cPeek = c;
                        curPos++;
                        // everything else means we need to continue scanning as we may have params and stuff
                        goto AttributeParsing;
                    }
                    else
                    {
                        // ok, we have here 2 or more character string that we need to check further
                        // often when we have full 2 char match the next char will be >, if that's the case
                        // then we definately matched our tag
                        var nextChar = _html[curPos + 2];
                        if (nextChar == (byte)'>')
                        {
                            //oChunk.sTag=oHE.GetString(iTagID);
                            _chunk.Tag = _he.GetTwoCharString(cPeek, c);
                            curPos += 3;
                            goto ReturnChunk;
                        }

                        // ok, check next char for space, if that's the case we still got our tag
                        // but need to skip to attribute parsing
                        if (nextChar == (byte)' ')
                        {
                            //_chunk.Tag = _he.GetString(tagId);
                            _chunk.Tag = _he.GetTwoCharString(cPeek, c);
                            curPos += 2;
                            cPeek = nextChar;
                            goto AttributeParsing;
                        }

                        // ok, we are not very lucky, but it is still worth fighting for
                        // now we need to check fully long string against what we have matched, maybe
                        // we got exact match and we can avoid full parsing of the tag
                        var tag = _he.GetStringData(tagId);

                        if (curPos + tag.Length + 5 >= _dataLength)
                            goto TagParsing;

                        // in a loop (and this is not an ideal solution, but still)
                        for (int i = 2; i < tag.Length; i++)
                            // if a single char is not matched, then we 
                            if (tag[i] != _html[curPos + i])
                                goto TagParsing;

                        // ok we matched full long word, but we need to be sure that char
                        // after the word is ' ' or '>' as otherwise we may have matched prefix of even longer word
                        nextChar = _html[curPos + tag.Length];
                        if (nextChar == (byte)'>')
                        {
                            _chunk.Tag = _he.GetString(tagId);
                            curPos += tag.Length + 1;
                            goto ReturnChunk;
                        }
                        if (nextChar == (byte)' ')
                        {
                            cPeek = nextChar;
                            _chunk.Tag = _he.GetString(tagId);
                            curPos += tag.Length;
                            goto AttributeParsing;
                        }
                        // no luck: we need to parse tag fully as our heuristical matching failed miserably :'o(
                    }
                }
            }
            TagParsing:

            _text.Clear();
            var charType = 0;

            // STAGE 1: parse tag (anything until > or /> or whitespace leading to start of attribute)
            while (cPeek != 0)
            {
                charType = _tagCharTypes[cPeek];

                //if (cPeek <= 32 && whiteSpace[cPeek] == 1)
                if (charType == (byte)TagCharType.WhiteSpace)
                {
                    curPos++;
                    // speculative loop unroll -- we have a very good chance of seeing non-space char next
                    // so instead of setting up loop we will just read it directly, this should save ticks
                    // on having to prepare while() loop
                    if (curPos < _dataLength) c = _html[curPos++];
                    else c = 0;
                    charType = _tagCharTypes[c];

                    //if (c == ' ' || c == '\t' || c == 13 || c == 10)
                    //if (c <= 32 && whiteSpace[c] == 1)
                    if (charType == (byte)TagCharType.WhiteSpace)
                    {
                        while (curPos < _dataLength)
                        {
                            c = _html[curPos++];
                            charType = _tagCharTypes[c];
                            if (charType == (byte)TagCharType.WhiteSpace)
                            //if(c != ' ' && c != '\t' && c != 13 && c != 10)
                            {
                                //cPeek = _html[curPos];
                                continue;
                            }
                            break;
                        }
                        if (curPos >= _dataLength)
                            c = 0;
                    }

                    //whiteSpaceHere = true;

                    // now, if we have already got tag it means that we are most likely
                    // going to need to parse tag attributes
                    if (_text._bufPos > 0)
                    {
                        _chunk.Tag = _text.SetToStringASCII();
                        // _chunk.Append((byte)' ');
                        curPos--;
                        if (curPos < _dataLength) cPeek = _html[curPos];
                        else cPeek = 0;
                        break;
                    }
                }
                else
                {
                    // reuse Peeked char from previous run
                    //c = cPeek; curPos++;
                    if (curPos < _dataLength) c = _html[curPos++];
                    else c = 0;
                }
                if (curPos < _dataLength) cPeek = _html[curPos];
                else cPeek = 0;
                // most likely we should have lower-cased ASCII char
                if (charType == (byte)TagCharType.LowerCasedASCIIorDigit)
                {
                    _text._buffer[_text._bufPos++] = c;
                    // _chunk.Append(c);
                    continue;
                }

                // tag end - we did not have any params
                if (c == (byte)'>')
                {
                    if (_text._bufPos > 0)
                        _chunk.Tag = _text.SetToStringASCII();
                    if (!_chunk.Closure)
                        _chunk.Type = HTMLchunkType.OpenTag;
                    return _chunk;
                }

                // closure of tag sign
                if (c == (byte)'/')
                {
                    _chunk.Closure = true;
                    _chunk.EndClosure = (_text._bufPos > 0);
                    _chunk.Type = HTMLchunkType.CloseTag;
                    continue;
                }

                // 03/08/08 XML support: ?xml tags - grrr
                if (c == (byte)'?')
                {
                    _text._buffer[_text._bufPos++] = c;
                    continue;
                }

                // nope, we have got upper cased ASCII char - this seems to be LESS likely than > and /
                //if (c >= 65 && c <= 90)
                if (charType > 32)
                {
                    // bCharType in this case contains already lower-cased char
                    _text._buffer[_text._bufPos++] = charType;
                    // _chunk.Append(bCharType);
                    continue;
                }

                // we might have namespace : sign here - all text before would have to be
                // saved as namespace and we will need to continue parsing actual tag
                if (charType == (byte)TagCharType.NameSpaceColon)
                {
                    // ok here we got a choice - we can just continue and treat the whole
                    // thing as a single tag with namespace stuff prefixed, OR
                    // we can separate first part into namespace and keep tag as normal
                    _text._buffer[_text._bufPos++] = (byte)':';
                    continue;
                }
                // ok, we have got some other char - we break out to deal with it in attributes part
                break;
            }

            if (cPeek == 0)
                return _chunk;

            // if true then equal sign was found 
            //var equalsSign = false;

            // STAGE 2: parse attributes (if any available)
            // attribute name can be standalone or with value after =
            // attribute itself can't have entities or anything like this - we expect it to be in ASCII characters
            AttributeParsing:

            string attrName;
            if (tagId != 0)
            {
                // first, skip whitespace:
                if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
                {
                    // most likely next char is not-whitespace
                    curPos++;
                    if (curPos >= _dataLength)
                        goto ReturnChunk;
                    cPeek = _html[curPos];
                    if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
                    {
                        // ok long loop here then
                        while (curPos < _dataLength)
                        {
                            cPeek = _html[curPos++];
                            if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
                                continue;
                            break;
                        }
                        if (cPeek == (byte)'>')
                            goto ReturnChunk;
                        curPos--;
                        if (curPos >= _dataLength)
                            goto ReturnChunk;
                    }
                    if (curPos >= _dataLength)
                        goto ReturnChunk;
                }

                // ok we have got matched tag, it is possible that we might be able to quickly match
                // attribute name known to be used for that tag:
                var attrId = _he.MatchAttr(cPeek, tagId);
                if (attrId > 0)
                {
                    var attr = _he.GetAttrData(attrId);
                    if (curPos + attr.Length + 2 >= _dataLength)
                        goto ActualAttributeParsing;
                    // in a loop (and this is not an ideal solution, but still)
                    for (var i = 1; i < attr.Length; i++)
                        // if a single char is not matched, then we 
                        if (attr[i] != _html[curPos + i])
                            goto ActualAttributeParsing;
                    var nextChar = _html[curPos + attr.Length];
                    // ok, we expect next symbol to be =
                    if (nextChar == (byte)'=')
                    {
                        attrName = _he.GetAttr(attrId);
                        curPos += attr.Length + 1;
                        cPeek = _html[curPos];
                        goto AttributeValueParsing;
                    }
                }
            }

            ActualAttributeParsing:

            _text.Clear();
            // doing exactly the same thing as in tag parsing
            while (cPeek != 0)
            {
                charType = _tagCharTypes[cPeek];
                //if (cPeek <= 32 && whiteSpace[cPeek] == 1)
                if (charType == (byte)TagCharType.WhiteSpace)
                {
                    curPos++;
                    // speculative loop unroll -- we have a very good chance of seeing non-space char next
                    // so instead of setting up loop we will just read it directly, this should save ticks
                    // on having to prepare while() loop
                    if (curPos < _dataLength)
                        c = _html[curPos++];
                    else
                    {
                        cPeek = 0;
                        break;
                    }
                    charType = _tagCharTypes[c];
                    //if (c == ' ' || c == '\t' || c == 13 || c == 10)
                    //if (c <= 32 && whiteSpace[c] == 1)
                    if (charType == (byte)TagCharType.WhiteSpace)
                    {
                        while (curPos < _dataLength)
                        {
                            c = _html[curPos++];
                            charType = _tagCharTypes[c];
                            if (charType == (byte)TagCharType.WhiteSpace)
                            //if(c != ' ' && c != '\t' && c != 13 && c != 10)
                            {
                                //cPeek = _html[curPos];
                                continue;
                            }
                            //if (c == (byte)'>')
                            // goto ReturnChunk;
                            //curPos--;
                            break;
                        }
                        if (curPos >= _dataLength)
                        {
                            c = 0;
                            cPeek = 0;
                            break;
                        }
                    }

                    //whiteSpaceHere = true;

                    // now, if we have already got attribute name it means that we need to go to parse value (which may not be present)
                    if (_text._bufPos > 0)
                    {
                        // _chunk.Append((byte)' ');
                        curPos--;
                        if (curPos < _dataLength) cPeek = _html[curPos];
                        else cPeek = 0;
                        // ok, we have got attribute name and now we have got next char there
                        // most likely we have got = here  and then value
                        if (cPeek == (byte)'=')
                        {
                            //equalsSign = true;
                            // move forward one char
                            curPos++;
                            if (curPos < _dataLength) cPeek = _html[curPos];
                            else cPeek = 0;
                            break;
                        }
                        // or we can have end of tag itself, doh!
                        if (cPeek == (byte)'>')
                        {
                            // move forward one char
                            curPos++;
                            if (_text._bufPos > 0)
                                _chunk.AddParam(_text.SetToStringASCII(), "", (byte)' ');
                            if (!_chunk.Closure)
                                _chunk.Type = HTMLchunkType.OpenTag;
                            return _chunk;
                        }
                        // closure
                        if (cPeek == (byte)'/')
                        {
                            _chunk.Closure = true;
                            _chunk.EndClosure = true;
                            _chunk.Type = HTMLchunkType.CloseTag;
                            continue;
                        }
                        // ok, we have got new char starting after current attribute name is fully parsed
                        // this means the attribute name is on its own and the char we found is start
                        // of a new attribute
                        _chunk.AddParam(_text.SetToStringASCII(), "", (byte)' ');
                        _text.Clear();
                        goto AttributeParsing;
                    }
                }
                else
                {
                    // reuse Peeked char from previous run
                    //c = cPeek; curPos++;
                    if (curPos < _dataLength) c = _html[curPos++];
                    else c = 0;
                }
                if (curPos < _dataLength) cPeek = _html[curPos];
                else cPeek = 0;
                // most likely we should have lower-cased ASCII char here
                if (charType == (byte)TagCharType.LowerCasedASCIIorDigit)
                {
                    _text._buffer[_text._bufPos++] = c;
                    // _chunk.Append(cChar);
                    continue;
                }

                // = with attribute value to follow
                if (c == (byte)'=')
                {
                    //equalsSign=true;
                    break;
                }

                // nope, we have got upper cased ASCII char - this seems to be LESS likely than > and /
                //if(c >= 65 && c <= 90)
                if (charType > 32)
                {
                    // bCharType in this case contains already lower-cased char
                    _text._buffer[_text._bufPos++] = charType;
                    // _chunk.Append(bCharType);
                    continue;
                }

                // tag end - we did not have any params
                if (c == (byte)'>')
                {
                    if (_text._bufPos > 0)
                        _chunk.AddParam(_text.SetToStringASCII(), "", (byte)' ');
                    if (!_chunk.Closure)
                        _chunk.Type = HTMLchunkType.OpenTag;
                    return _chunk;
                }

                // closure of tag sign
                if (c == (byte)'/')
                {
                    _chunk.Closure = true;
                    _chunk.EndClosure = true;
                    _chunk.Type = HTMLchunkType.CloseTag;
                    continue;
                }

                // some other char
                _text._buffer[_text._bufPos++] = c;
                // _chunk.Append(cChar);
            }

            if (cPeek == 0)
            {
                if (_text._bufPos > 0)
                    _chunk.AddParam(_text.SetToStringASCII(), "", (byte)' ');
                if (!_chunk.Closure)
                    _chunk.Type = HTMLchunkType.OpenTag;
                return _chunk;
            }

            attrName = _text.SetToStringASCII();

            AttributeValueParsing:

            /// ***********************************************************************
            /// STAGE 3: parse attribute value
            /// ***********************************************************************

            // the value could be just string, or in quotes (single or double)
            // or we can have next attribute name start, in which case we will jump back to attribute parsing

            // for tracking quotes purposes
            var quotes = cPeek;

            int valueStartOffset;

            // skip whitespace if any
            if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
            {
                curPos++;
                // speculative loop unroll -- we have a very good chance of seeing non-space char next
                // so instead of setting up loop we will just read it directly, this should save ticks
                // on having to prepare while() loop
                if (curPos < _dataLength) cPeek = _html[curPos];
                else
                {
                    valueStartOffset = curPos - 1;
                    goto AttributeValueEnd;
                }

                //if (c == ' ' || c == '\t' || c == 13 || c == 10)
                //if (c <= 32 && whiteSpace[c] == 1)
                if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
                {
                    while (curPos < _dataLength)
                    {
                        cPeek = _html[curPos++];
                        if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
                        //if(c != ' ' && c != '\t' && c != 13 && c != 10)
                        {
                            //cPeek = _html[curPos];
                            continue;
                        }
                        curPos--;
                        break;
                    }
                    if (curPos >= _dataLength)
                    {
                        valueStartOffset = curPos - 1;
                        goto AttributeValueEnd;
                    }
                }
                quotes = cPeek;
            }

            // because we deal with VALUE of the attribute it means we can't lower-case it, 
            // or skip whitespace (if in quotes), which in practice means that we don't need to copy
            // it to temporary string buffer, we can just remember starting offset and then create string from
            // data in bHTML

            // ok, first char can be one of the quote chars or something else
            if (cPeek != '\"' && cPeek != '\'')
            {
                valueStartOffset = curPos;
                quotes = (byte)' ';
                // any other char here means we have value up until next whitespace or end of tag
                // this gives us good opportunity to scan fairly quickly without otherwise redundant
                // checks - this should happen fairly rarely, however loop dealing with data between quotes
                // will happen often enough and its best to eliminate as much stuff from it as possible
                //sText.bBuffer[sText.iBufPos++]=cPeek;

                // move to next char
                if (curPos < _dataLength) cPeek = _html[curPos++];
                else goto AttributeValueEnd;

                while (cPeek != 0)
                {
                    // if whitespace then we got our value and need to go back to param
                    if (cPeek <= 32 && _tagCharTypes[cPeek] == (byte)TagCharType.WhiteSpace)
                    {
                        _chunk.AddParam(attrName, GetString(valueStartOffset, curPos - valueStartOffset - 1), (byte)' ');
                        curPos--;
                        goto AttributeParsing;
                    }
                    // end of tag?
                    if (cPeek == (byte)'>')
                    {
                        //curPos--;
                        break;
                    }
                    if (curPos < _dataLength) cPeek = _html[curPos++];
                    else
                    {
                        curPos = _dataLength + 1;
                        goto AttributeValueEnd;
                    }
                }

                // ok we are done, add outstanding attribute
                _chunk.AddParam(attrName, GetString(valueStartOffset, curPos - valueStartOffset - 1), (byte)' ');

                goto ReturnChunk;
            }

            // move one step forward
            curPos++;
            valueStartOffset = curPos;
            if (curPos < _dataLength) cPeek = _html[curPos++];
            else goto AttributeValueEnd;

            // attribute value parsing from between two quotes
            while (cPeek != 0)
            {
                // check whether we have got possible entity (can be anything starting with &)
                if (cPeek == 38)
                {
                    var prevPos = curPos;
                    var entityChar = _e.CheckForEntity(_html, ref curPos, _dataLength);
                    // restore current symbol
                    if (entityChar == 0)
                    {
                        if (curPos < _dataLength) cPeek = _html[curPos++];
                        else break;
                        //_text.Buffer[_text.BufPos++] = 38; //(byte)'&';;
                        continue;
                    }
                    else
                    {
                        // okay we have got an entity, our hope of not having to copy stuff into variable
                        // is over, we have to continue in a slower fashion :(
                        // but thankfully this should happen very rarely, so, annoying to code, but
                        // most codepaths will run very fast!
                        var preEntLen = prevPos - valueStartOffset - 1;

                        // 14/05/08 need to clear text - it contains attribute name text
                        _text.Clear();
                        // copy previous data
                        if (preEntLen > 0)
                        {
                            Array.Copy(_html, valueStartOffset, _text._buffer, 0, preEntLen);
                            _text._bufPos = preEntLen;
                        }
                        // we have to skip now to next byte, since 
                        // some converted chars might well be control chars like >
                        _chunk.Entities = true;
                        if (c == (byte)'<')
                            _chunk.LtEntity = true;
                        // unless is space we will ignore it
                        // note that this won't work if &nbsp; is defined as it should
                        // byte int value of 160, rather than 32.
                        //if (c != ' ')
                        _text.Append(entityChar);
                        if (curPos < _dataLength) cPeek = _html[curPos++];
                        else goto AttributeValueEnd;

                        // okay, we continue here using in effect new inside loop as we might have more entities here
                        // attribute value parsing from between two quotes
                        while (cPeek != 0)
                        {
                            // check whether we have got possible entity (can be anything starting with &)
                            if (cPeek == 38)
                            {
                                var newEntityChar = _e.CheckForEntity(_html, ref curPos, _dataLength);
                                // restore current symbol
                                if (newEntityChar != 0)
                                {
                                    if (newEntityChar == (byte)'<')
                                        _chunk.LtEntity = true;
                                    _text.Append(newEntityChar);
                                    if (curPos < _dataLength) cPeek = _html[curPos++];
                                    else goto AttributeValueEnd;
                                    continue;
                                }
                            }

                            // check if is end of quotes
                            if (cPeek == quotes)
                            {
                                // ok we finished scanning it: add param with value and then go back to param name parsing
                                _chunk.AddParam(attrName, _text.SetToString(), quotes);
                                if (curPos < _dataLength) cPeek = _html[curPos];
                                else break;
                                goto AttributeParsing;
                            }
                            _text._buffer[_text._bufPos++] = cPeek;
                            //_text.Append(cPeek);
                            if (curPos < _dataLength) cPeek = _html[curPos++];
                            else break;
                        }
                        _chunk.AddParam(attrName, _text.SetToString(), quotes);
                        goto ReturnChunk;
                    }
                }

                // check if is end of quotes
                if (cPeek == quotes)
                {
                    // ok we finished scanning it: add param with value and then go back to param name parsing
                    //_text.Clear();
                    _chunk.AddParam(attrName, GetString(valueStartOffset, curPos - valueStartOffset - 1), quotes);
                    if (curPos < _dataLength) cPeek = _html[curPos];
                    else { /*curPos++;*/ break; }
                    goto AttributeParsing;
                }

                if (curPos < _dataLength) cPeek = _html[curPos++];
                else { /*curPos++;*/ break; }
            }

            AttributeValueEnd:

            // ok we are done, add outstanding attribute
            var len = curPos - valueStartOffset - 1;
            if (len > 0) _chunk.AddParam(attrName, GetString(valueStartOffset, len), quotes);
            else _chunk.AddParam(attrName, "", quotes);

            ReturnChunk:

            if (_chunk.Closure) _chunk.Type = HTMLchunkType.CloseTag;
            else _chunk.Type = HTMLchunkType.OpenTag;
            return _chunk;
        }

        /// <summary>
        /// Finishes parsing of comments tag
        /// </summary>
        /// <returns>HTMLchunk object</returns>
        internal HTMLchunk ParseComments(ref int curPos, out bool fullTag)
        {
            while (curPos < _dataLength)
                if (_html[curPos++] == 62)
                    if (curPos >= 3)
                        if (_html[curPos - 2] == (byte)'-' && _html[curPos - 3] == (byte)'-')
                        {
                            fullTag = true;
                            return _chunk;
                        }
            fullTag = false;
            return _chunk;
        }

        /// <summary>
        /// Finishes parsing of CDATA component
        /// </summary>
        /// <param name="curPos"></param>
        /// <param name="fullTag"></param>
        /// <returns></returns>
        internal HTMLchunk ParseCDATA(ref int curPos, out bool fullTag)
        {
            // 19/07/08 yes this is copy/paste - moving to same function would make source code 
            // look nice but such function won't be inlined by compiler as it would be too complex for it
            // so this is manual inlining, well that and lack of time and desire to do it!
            while (curPos < _dataLength)
                if (_html[curPos++] == 62)
                    if (curPos >= 3)
                        if (_html[curPos - 2] == (byte)']' && _html[curPos - 3] == (byte)']')
                        {
                            fullTag = true;
                            return _chunk;
                        }
            fullTag = false;
            return _chunk;
        }

        /// <summary>
        /// /script sequence indicating end of script tag
        /// </summary>
        static byte[] ClosedScriptTag = { (byte)'/', (byte)'s', (byte)'c', (byte)'r', (byte)'i', (byte)'p', (byte)'t', (byte)'>' };

        /// <summary>
        /// Finishes parsing of data after scripts tag - makes extra checks to avoid being broken
        /// with >'s used to denote comparison
        /// </summary>
        /// <returns>HTMLchunk object</returns>
        internal HTMLchunk ParseScript(ref int curPos)
        {
            byte c = 0;
            var start = curPos;
            var lastPos = -1;
            while (curPos < _dataLength)
            {
                c = _html[curPos++];
                if (c == (byte)'<')
                {
                    lastPos = curPos;
                    var pos = 0;
                    // check here if its an HTML comment
                    if (curPos < _dataLength)
                        if (_html[curPos] == (byte)'!')
                            if ((curPos + 3) < _dataLength)
                                if (_html[curPos + 1] == (byte)'-' && _html[curPos + 2] == (byte)'-')
                                {
                                    // FIXIT: perhaps it is more correct here to return straight away?
                                    ParseComments(ref curPos, out bool fullTag);
                                    continue;
                                    //Console.WriteLine("");
                                }
                    while (curPos < _dataLength)
                    {
                        c = _html[curPos++];
                        //if (c == ' ' || c == '\t' || c == 13 || c == 10)
                        //if (c <= 32 && bWhiteSpace[c] == 1)
                        if (c <= 32 && _tagCharTypes[c] == (byte)TagCharType.WhiteSpace)
                            continue;
                        if (c >= 65 && c <= 90)
                            c += 32;
                        // if next char it out of expected sequence then we will ignore it
                        // and restart scanning from previous position
                        if (c != ClosedScriptTag[pos])
                        {
                            //curPos = lastPos;
                            lastPos = curPos;
                            break;
                        }
                        // if we got whole length then we found end of script tag and can return back
                        if (++pos == ClosedScriptTag.Length)
                            goto ReturnChunk; // don't ever tell me about usage of goto...
                    }
                }
                // oChunk.Append(c);
            }
            // ok we run out of space for scripts - must be broken HTML, we will take all we can
            lastPos = _dataLength + 1;
            ReturnChunk:
            _chunk.ChunkLength = curPos - _chunk.ChunkOffset;
            if (_p.AutoKeepScripts || _p.KeepRawHTML)
            {
                if (!_p.AutoExtractBetweenTagsOnly)
                    _chunk.Html = GetString(_chunk.ChunkOffset, _chunk.ChunkLength);
                else
                {
                    if (lastPos == -1)
                        lastPos = curPos + 1;
                    _chunk.Html = GetString(start, lastPos - start - 1);
                }
            }
            return _chunk;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool Disposed;

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Disposed = true;
                _html = null;
                _chunk = null;
                _text = null;
                _e = null;
                _p = null;
            }
        }

        string GetString(int offset, int len)
        {
            // check boundaries: sometimes they are exceeded
            if (offset >= _dataLength || len == 0)
                return "";
            if (offset + len > _dataLength)
                len = _html.Length - offset;
            try { return _chunk.Enc.GetString(_html, offset, len); }
            catch { return string.Empty; }
        }
    }
}