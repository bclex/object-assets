using System;
using System.Text;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// Class for fast dynamic string building - it is faster than StringBuilder
    /// </summary>
    ///<exclude/>
    class DynaString : IDisposable
    {
        /// <summary>
        /// Finalised text will be available in this string
        /// </summary>
        public string _text;

        /// <summary>
        /// CRITICAL: that much capacity will be allocated (once) for this object -- for performance reasons
        /// we do NOT have range checks because we make reasonably safe assumption that accumulated string will
        /// fit into the buffer. If you have very abnormal strings then you should increase buffer accordingly.
        /// </summary>
        public static int TEXT_CAPACITY = 1024 * 256 - 1;

        public byte[] _buffer;
        public int _bufPos;
        int _length;
        Encoding _enc = Encoding.Default;
        bool _disposed;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="text">Initial string</param>
        internal DynaString(string text)
        {
            _text = text;
            _bufPos = 0;
            _buffer = new byte[TEXT_CAPACITY + 1];
            _length = text.Length;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool bDisposing)
        {
            if (!_disposed)
                _buffer = null;
            _disposed = true;
        }

        /// <summary>
        /// Resets object to zero length string
        /// </summary>
        public void Clear()
        {
            _text = "";
            _length = 0;
            _bufPos = 0;
        }

        /// <summary>
        /// Sets encoding to be used for conversion of binary data into string
        /// </summary>
        /// <param name="p_oEnc">Encoding object</param>
        public void SetEncoding(Encoding p_oEnc)
        {
            _enc = p_oEnc;
        }

        /*
        /// <summary>
        /// Appends a "char" to the buffer
        /// </summary>
        /// <param name="cChar">Appends char (byte really)</param>
        public void Append(byte cChar)
        {
            // Length++;
            if (iBufPos>=TEXT_CAPACITY)
            {
                if (sText.Length==0)
                    sText=oEnc.GetString(bBuffer,0,iBufPos);
                else
                    //sText+=new string(bBuffer,0,iBufPos);
                    sText+=oEnc.GetString(bBuffer,0,iBufPos);

                iLength+=iBufPos;

                iBufPos=0;
            }

            bBuffer[iBufPos++]=cChar;
        }
        */

        /// <summary>
        /// Appends proper char with smart handling of Unicode chars
        /// </summary>
        /// <param name="c">Char to append</param>
        public void Append(char c)
        {
            if (c <= 127)
                _buffer[_bufPos++] = (byte)c;
            else
            {
                // unicode character - this is really bad way of doing it, but 
                // it seems to be called almost never
                var bytes = _enc.GetBytes(c.ToString());

                // 16/09/07 Possible bug reported by Martin Bächtold: 
                // test case: 
                // <meta http-equiv="Content-Category" content="text/html; charset=windows-1251">
                // &#1329;&#1378;&#1400;&#1406;&#1397;&#1377;&#1398; &#1341;&#1377;&#1401;&#1377;&#1407;&#1400;&#1410;&#1408;

                // the problem is that some unicode chars might not be mapped to bytes by specified encoding
                // in the HTML itself, this means we will get single byte ? - this will look like failed conversion
                // Not good situation that we need to deal with :(
                if (bytes.Length == 1 && bytes[0] == (char)'?')
                    for (int i = 0; i < bytes.Length; i++)
                        _buffer[_bufPos++] = bytes[i];
                else
                    for (int i = 0; i < bytes.Length; i++)
                        _buffer[_bufPos++] = bytes[i];
            }
        }

        /// <summary>
        /// Creates string from buffer using set encoder
        /// </summary>
        internal string SetToString()
        {
            if (_bufPos > 0)
            {
                if (_text.Length == 0) _text = _enc.GetString(_buffer, 0, _bufPos);
                else _text += _enc.GetString(_buffer, 0, _bufPos);
                _length += _bufPos;
                _bufPos = 0;
            }
            return _text;
        }

        /// <summary>
        /// Creates string from buffer using default encoder
        /// </summary>
        internal string SetToStringASCII()
        {
            if (_bufPos > 0)
            {
                if (_text.Length == 0) _text = Encoding.Default.GetString(_buffer, 0, _bufPos);
                else _text += Encoding.Default.GetString(_buffer, 0, _bufPos);
                _length += _bufPos;
                _bufPos = 0;
            }
            return _text;
        }

    }
}
