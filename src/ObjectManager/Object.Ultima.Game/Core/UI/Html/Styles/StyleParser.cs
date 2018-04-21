using OA.Core.UI.Html.Elements;
using OA.Core.UI.Html.Parsing;
using OA.Ultima.Core;
using OA.Ultima.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI.Html.Styles
{
    /// <summary>
    /// Style manager. As you parse html tags, add them to a manager TagCollection.
    /// Then, when you add an element, use the manager to generate an object that will contain all the styles for that element.
    /// </summary>
    public class StyleParser
    {
        public StyleState Style;

        readonly IResourceProvider _provider;
        List<OpenTag> _openTags;

        public StyleParser(IResourceProvider provider)
        {
            _provider = provider;
            _openTags = new List<OpenTag>();
            RecalculateStyle();
        }

        public void ParseTag(HTMLchunk chunk, AElement atom)
        {
            if (!chunk.Closure || chunk.EndClosure)
            {
                // create the tag and add it to the list of open tags.
                var tag = new OpenTag(chunk);
                _openTags.Add(tag);
                // parse the tag (which will update the StyleParser's current style
                ParseTag(tag, atom);
                // if the style has changed and atom is not null, set the atom's style to the current style.
                if (atom != null)
                    atom.Style = Style;
                // if this is a self-closing tag (<br/>) close it!
                if (chunk.EndClosure)
                    CloseOneTag(chunk);
            }
            else CloseOneTag(chunk);
        }

        public void CloseOneTag(HTMLchunk chunk)
        {
            var mustRecalculateStyle = false;
            for (var i = _openTags.Count - 1; i >= 0; i--)
                if (_openTags[i].Tag == chunk.Tag)
                {
                    _openTags.RemoveAt(i);
                    mustRecalculateStyle = true;
                    break;
                }
            if (mustRecalculateStyle)
                RecalculateStyle();
        }

        public void CloseAnySoloTags()
        {
            var mustRecalculateStyle = false;
            for (int i = 0; i < _openTags.Count; i++)
                if (_openTags[i].EndClosure)
                {
                    _openTags.RemoveAt(i);
                    mustRecalculateStyle = true;
                    i--;
                }
            if (mustRecalculateStyle)
                RecalculateStyle();
        }

        public void InterpretHREF(HTMLchunk chunk, AElement atom)
        {
            if (chunk.EndClosure) { } // solo anchor elements are meaningless.
            if (!chunk.Closure)
            {
                // opening a hyperlink!
                RecalculateStyle();
                var tag = new OpenTag(chunk);
                _openTags.Add(tag);
                ParseTag(tag, atom);
            }
            else RecalculateStyle(); // closing a hyperlink.
        }

        private void RecalculateStyle()
        {
            Style = new StyleState(_provider);
            for (var i = 0; i < _openTags.Count; i++)
                ParseTag(_openTags[i]);
        }

        private void ParseTag(OpenTag tag, AElement atom = null)
        {
            switch (tag.Tag)
            {
                case "b": Style.IsBold = true; break;
                case "i": Style.IsItalic = true; break;
                case "u": Style.IsUnderlined = true; break;
                case "outline": Style.IsOutlined = true; break;
                case "big": Style.Font = _provider.GetUnicodeFont((int)Fonts.UnicodeBig); break;
                case "basefont":
                case "medium": Style.Font = _provider.GetUnicodeFont((int)Fonts.UnicodeMedium); break;
                case "small": Style.Font = _provider.GetUnicodeFont((int)Fonts.UnicodeSmall); break;
                case "left": if (atom != null && atom is BlockElement) (atom as BlockElement).Alignment = Alignments.Left; break;
                case "center": if (atom != null && atom is BlockElement) (atom as BlockElement).Alignment = Alignments.Center; break;
                case "right": if (atom != null && atom is BlockElement) (atom as BlockElement).Alignment = Alignments.Right; break;
            }

            foreach (DictionaryEntry param in tag.Params)
            {
                // get key and value for this tag param
                var key = param.Key.ToString();
                var value = param.Value.ToString();
                if (value.StartsWith("0x"))
                    value = Utility.ToInt32(value).ToString();
                // trim trailing forward slash.
                if (value.EndsWith("/"))
                    value = value.Substring(0, value.Length - 1);
                switch (key)
                {
                    case "href":
                        // href paramater can only be used on 'anchor' tags.
                        if (tag.Tag == "a")
                            Style.HREF = value;
                        break;
                    case "color":
                    case "hovercolor":
                    case "activecolor":
                        // get the color!
                        var color = value;
                        Color? c = null;
                        if (color[0] == '#')
                        {
                            color = color.Substring(1);
                            if (color.Length == 3 || color.Length == 6)
                                c = Utility.ColorFromHexString(color);
                        }
                        else c = Utility.ColorFromString(color); // try to parse color by name
                        if (c.HasValue)
                        {
                            if (key == "color")
                                Style.Color = c.Value;
                            if (tag.Tag == "a")
                            {
                                // a tag colors are override, they are rendered white and then hued with a websafe hue.
                                switch (key)
                                {
                                    case "color": Style.ColorHue = _provider.GetWebSafeHue(c.Value); break;
                                    case "hovercolor": Style.HoverColorHue = _provider.GetWebSafeHue(c.Value); break;
                                    case "activecolor": Style.ActiveColorHue = _provider.GetWebSafeHue(c.Value); break;
                                    default: Utils.Warning($"Only anchor <a> tags can have attribute {key}."); break;
                                }
                            }
                        }
                        else Utils.Warning($"Improperly formatted color: {color}");
                        break;
                    case "src":
                    case "hoversrc":
                    case "activesrc":
                        if (atom is ImageElement)
                        {
                            switch (key)
                            {
                                case "src": (atom as ImageElement).ImgSrc = int.Parse(value); break;
                                case "hoversrc": (atom as ImageElement).ImgSrcOver = int.Parse(value); break;
                                case "activesrc": (atom as ImageElement).ImgSrcDown = int.Parse(value); break;
                            }
                            break;
                        }
                        Utils.Warning($"{key} param encountered within {tag.Tag} tag which does not use this param.");
                        break;
                    case "width":
                        {
                            if (atom is ImageElement || atom is BlockElement) atom.Width = int.Parse(value);
                            else Utils.Warning($"width param encountered within {tag.Tag} which does not use this param.");
                        }
                        break;
                    case "height":
                        {
                            if (atom is ImageElement || atom is BlockElement) atom.Height = int.Parse(value);
                            else Utils.Warning($"width param encountered within {tag.Tag} which does not use this param.");
                        }
                        break;
                    case "style": ParseStyle(value); break;
                    default: Utils.Warning($"Unknown parameter:{key}"); break;
                }
            }
        }

        private void ParseStyle(string css)
        {
            if (css.Length == 0)
                return;
            var key = string.Empty;
            var value = string.Empty;
            var inKey = true;
            for (var i = 0; i < css.Length; i++)
            {
                var c = css[i];
                if (c == ':' || c == '=')
                {
                    if (inKey) inKey = false;
                    else
                    {
                        Utils.Warning($"Uninterpreted, possibly malformed style parameter:{css}");
                        return;
                    }
                }
                else if (c == ';')
                {
                    if (!inKey)
                    {
                        ParseOneStyle(key, value);
                        key = string.Empty;
                        value = string.Empty;
                        inKey = true;
                    }
                    else
                    {
                        Utils.Warning($"Uninterpreted, possibly malformed style parameter:{css}");
                        return;
                    }
                }
                else
                {
                    if (inKey) key += c;
                    else value += c;
                }
            }
            if (key != string.Empty && value != string.Empty)
                ParseOneStyle(key, value);
        }

        private void ParseOneStyle(string key, string value)
        {
            value = value.Trim();
            switch (key.ToLower().Trim())
            {
                case "font-family":
                    if (value.StartsWith("ascii"))
                    {
                        if (int.TryParse(value.Replace("ascii", string.Empty), out int index))
                            Style.Font = _provider.GetAsciiFont(index);
                        else Utils.Warning($"Unknown font-family parameter:{key}");
                    }
                    else if (value.StartsWith("uni"))
                    {
                        if (int.TryParse(value.Replace("uni", string.Empty), out int index))
                            Style.Font = _provider.GetUnicodeFont(index);
                        else Utils.Warning($"Unknown font-family parameter:{value}");
                    }
                    break;
                case "text-decoration":
                    var param = value.Trim().Split(' ');
                    for (var i = 0; i < param.Length; i++)
                    {
                        if (param[i] == "none") Style.IsUnderlined = false;
                        else if (param[i] == "underline") Style.IsUnderlined = true;
                        // other possibilities? overline|line-through|initial|inherit;
                        else Utils.Warning($"Unknown text-decoration parameter:{param[i]}");
                    }
                    break;
                default: Utils.Warning($"Unknown style parameter:{key}"); break;
            }
        }
    }
}
