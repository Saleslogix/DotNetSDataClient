using System;
using System.Text;

namespace Saleslogix.SData.Client.Framework
{
    internal static class MailBnfHelper
    {
        private const int Ascii7BitMaxValue = 0x7f;
        private static readonly bool[] _qText = new bool[0x80];
        private static readonly bool[] _tText = new bool[0x80];

        static MailBnfHelper()
        {
            for (var i = 1; i <= 9; i++)
            {
                _qText[i] = true;
            }
            _qText[11] = true;
            _qText[12] = true;
            for (var i = 14; i <= 0x21; i++)
            {
                _qText[i] = true;
            }
            for (var i = 0x23; i <= 0x5b; i++)
            {
                _qText[i] = true;
            }
            for (var i = 0x5d; i <= 0x7f; i++)
            {
                _qText[i] = true;
            }

            for (var i = 0x21; i <= 0x7e; i++)
            {
                _tText[i] = true;
            }
            _tText[40] = false;
            _tText[0x29] = false;
            _tText[60] = false;
            _tText[0x3e] = false;
            _tText[0x40] = false;
            _tText[0x2c] = false;
            _tText[0x3b] = false;
            _tText[0x3a] = false;
            _tText[0x5c] = false;
            _tText[0x22] = false;
            _tText[0x2f] = false;
            _tText[0x5b] = false;
            _tText[0x5d] = false;
            _tText[0x3f] = false;
            _tText[0x3d] = false;
        }

        internal static string ReadParameterAttribute(string data, ref int offset)
        {
            return SkipCfws(data, ref offset) ? ReadToken(data, ref offset) : null;
        }

        internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder)
        {
            offset++;
            var startIndex = offset;
            if (builder == null)
            {
                builder = new StringBuilder();
            }
            while (offset < data.Length)
            {
                if (data[offset] == '\\')
                {
                    builder.Append(data, startIndex, offset - startIndex);
                    startIndex = ++offset;
                }
                else
                {
                    if (data[offset] == '"')
                    {
                        builder.Append(data, startIndex, offset - startIndex);
                        offset++;
                        return builder.ToString();
                    }
                    if (data[offset] == '=' && data.Length > (offset + 3) && data[offset + 1] == '\r' && data[offset + 2] == '\n' && (data[offset + 3] == ' ' || data[offset + 3] == '\t'))
                    {
                        offset += 3;
                    }
                    else if (data[offset] > Ascii7BitMaxValue || !_qText[data[offset]])
                    {
                        throw new FormatException("Invalid mail header field character: " + data[offset]);
                    }
                }
                offset++;
            }
            throw new FormatException("Malformed mail header field");
        }

        internal static string ReadToken(string data, ref int offset)
        {
            var startIndex = offset;
            while (offset < data.Length)
            {
                if (data[offset] > Ascii7BitMaxValue)
                {
                    throw new FormatException("Invalid mail header field character: " + data[offset]);
                }
                if (!_tText[data[offset]])
                {
                    break;
                }
                offset++;
            }
            if (startIndex == offset)
            {
                throw new FormatException("Invalid mail header field character: " + data[offset]);
            }
            return data.Substring(startIndex, offset - startIndex);
        }

        internal static bool SkipCfws(string data, ref int offset)
        {
            var num = 0;
            while (offset < data.Length)
            {
                if (data[offset] > '\x007f')
                {
                    throw new FormatException("Invalid mail header field character: " + data[offset]);
                }
                if (data[offset] == '\\' && num > 0)
                {
                    offset += 2;
                }
                else if (data[offset] == '(')
                {
                    num++;
                }
                else if (data[offset] == ')')
                {
                    num--;
                }
                else if (data[offset] != ' ' && data[offset] != '\t' && num == 0)
                {
                    return true;
                }
                if (num < 0)
                {
                    throw new FormatException("Invalid mail header field character: " + data[offset]);
                }
                offset++;
            }
            return false;
        }
    }
}