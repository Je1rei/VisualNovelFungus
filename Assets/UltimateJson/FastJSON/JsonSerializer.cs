using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Collections.Specialized;
using UnityEngine;

namespace fastJSON
{
    internal sealed class JsonSerializer
    {
        private readonly StringBuilder _output = new StringBuilder();
        private int _before;
        private readonly int _maxDepth;
		private int _currentDepth;
        private readonly Dictionary<string, int> _globalTypes = new Dictionary<string, int>();
        private readonly Dictionary<object, int> _cirobj = new Dictionary<object, int>();
        private readonly JsonParameters _params;
        private readonly bool _useEscapedUnicode;

        internal JsonSerializer(JsonParameters param)
        {
            _params = param;
            _useEscapedUnicode = JsonParameters.UseEscapedUnicode;
            _maxDepth = JsonParameters.SerializerMaxDepth;
        }

        internal string ConvertToJson(object obj)
        {
            WriteValue(obj);
			
			if (!_params.UsingGlobalTypes || _globalTypes == null || _globalTypes.Count <= 0)
			{
				return _output.ToString();
			}
			
			var sb = new StringBuilder();
			sb.Append("\"$types\":{");
			var pendingSeparator = false;
			foreach (var kv in _globalTypes)
			{
				if (pendingSeparator) sb.Append(',');
				pendingSeparator = true;
				sb.Append('\"');
				sb.Append(kv.Key);
				sb.Append("\":\"");
				sb.Append(kv.Value);
				sb.Append('\"');
			}
			sb.Append("},");
			_output.Insert(_before, sb.ToString());
			return _output.ToString();
        }

        private void WriteValue(object obj)
        {
            if (obj == null || obj is DBNull)
                _output.Append("null");

            else if (obj is string || obj is char)
                WriteString(obj.ToString());

            else if (obj is Guid)
                WriteGuid((Guid)obj);

            else if (obj is bool)
                _output.Append(((bool)obj) ? "true" : "false"); // conform to standard

            else if (
                obj is int || obj is long ||
                obj is decimal ||
                obj is byte || obj is short ||
                obj is sbyte || obj is ushort ||
                obj is uint || obj is ulong
            )
                _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));

            else if (obj is double)
			{
				var d = (double)obj;
				_output.Append(double.IsNaN(d) ? "\"NaN\"" : ((IConvertible) obj).ToString(NumberFormatInfo.InvariantInfo));
			}
            else if (obj is float)
			{
				var d = (float)obj;
				_output.Append(float.IsNaN(d) ? "\"NaN\"" : ((IConvertible) obj).ToString(NumberFormatInfo.InvariantInfo));
			}

            else if (obj is DateTime)
                WriteDateTime((DateTime)obj);

            else if (obj is DateTimeOffset)
                WriteDateTimeOffset((DateTimeOffset)obj);

            else if (obj is IDictionary &&
                obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))
                WriteStringDictionary((IDictionary)obj);
            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);
            else if (obj is byte[])
                WriteBytes((byte[])obj);

            else if (obj is StringDictionary)
                WriteSd((StringDictionary)obj);

            else if (obj is NameValueCollection)
                WriteNv((NameValueCollection)obj);

            else if (obj is IEnumerable)
                WriteArray((IEnumerable)obj);

            else if (obj is Enum)
                WriteEnum((Enum)obj);

            else if (Reflection.Instance.IsTypeRegistered(obj.GetType()))
			{
				//Debug.Log("obj:" + obj);
				WriteCustom(obj);
			}
			else
			{
				WriteObject(obj);
			}
        }

        private void WriteDateTimeOffset(DateTimeOffset d)
        {
            write_date_value(d.DateTime);
            _output.Append(" ");
			_output.Append(d.Offset.Hours > 0 ? "+" : "-");
			_output.Append(d.Offset.Hours.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(":");
            _output.Append(d.Offset.Minutes);

            _output.Append('\"');
        }

        private void WriteNv(NameValueCollection nameValueCollection)
        {
            _output.Append('{');

            var pendingSeparator = false;

            foreach (string key in nameValueCollection)
            {
				if (pendingSeparator) _output.Append(',');
				WritePair(key, nameValueCollection[key]);
				pendingSeparator = true;
			}
            _output.Append('}');
        }

        private void WriteSd(IEnumerable stringDictionary)
        {
            _output.Append('{');

            var pendingSeparator = false;

            foreach (DictionaryEntry entry in stringDictionary)
			{
				if (pendingSeparator) _output.Append(',');

				var k = (string)entry.Key;
				WritePair(k, entry.Value);
				pendingSeparator = true;
			}
            _output.Append('}');
        }

        private void WriteCustom(object obj)
        {
            Serialize s;
            Reflection.Instance.CustomSerializer.TryGetValue(obj.GetType(), out s);
            WriteStringFast(s(obj));
        }

        private void WriteEnum(Enum e)
		{
			WriteStringFast(e.ToString());
		}

        private void WriteGuid(Guid g)
		{
			WriteBytes(g.ToByteArray());
		}

        private void WriteBytes(byte[] bytes)
        {
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
        }

        private void WriteDateTime(DateTime dateTime)
        {
            // datetime format standard : yyyy-MM-dd HH:mm:ss
            DateTime dt;
            dt = dateTime.ToUniversalTime();

            write_date_value(dt);

            _output.Append('Z');
            _output.Append('\"');
        }

        private void write_date_value(DateTime dt)
        {
            _output.Append('\"');
            _output.Append(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
            _output.Append('-');
            _output.Append(dt.Month.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append('-');
            _output.Append(dt.Day.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append('T'); // strict ISO date compliance 
            _output.Append(dt.Hour.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(':');
            _output.Append(dt.Minute.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(':');
            _output.Append(dt.Second.ToString("00", NumberFormatInfo.InvariantInfo));
		}

		private bool _typesWritten;
        private void WriteObject(object obj)
        {
            int i;
            if (_cirobj.TryGetValue(obj, out i) == false)
                _cirobj.Add(obj, _cirobj.Count + 1);
            else
            {
                if (_currentDepth > 0 && _params.InlineCircularReferences == false)
                {
                    //_circular = true;
                    _output.Append("{\"$i\":");
                    _output.Append(i.ToString());
                    _output.Append("}");
                    return;
                }
            }
            if (_params.UsingGlobalTypes == false)
                _output.Append('{');
            else
            {
                if (_typesWritten == false)
                {
                    _output.Append('{');
                    _before = _output.Length;
                    //_output = new StringBuilder();
                }
                else
                    _output.Append('{');
            }
            _typesWritten = true;
            _currentDepth++;
            if (_currentDepth > _maxDepth)
                throw new Exception("Serializer encountered maximum depth of " + _maxDepth);


            var map = new Dictionary<string, string>();
            var t = obj.GetType();
            var append = false;
            if (_params.UseExtensions)
            {
                if (_params.UsingGlobalTypes == false)
                    WritePairFast("$type", Reflection.Instance.GetTypeAssemblyName(t));
                else
                {
                    int dt;
                    var ct = Reflection.Instance.GetTypeAssemblyName(t);
                    if (_globalTypes.TryGetValue(ct, out dt) == false)
                    {
                        dt = _globalTypes.Count + 1;
                        _globalTypes.Add(ct, dt);
                    }
                    WritePairFast("$type", dt.ToString());
                }
                append = true;
            }

            var g = Reflection.Instance.GetGetters(t, false, _params.IgnoreAttributes);
            var c = g.Length;
            for (var ii = 0; ii < c; ii++)
            {
                var p = g[ii];
                var o = p.Getter(obj);
				if (append)
					_output.Append(',');
				WritePair(p.Name, o);
				if (o != null && _params.UseExtensions)
				{
					var tt = o.GetType();
					if (tt == typeof(object))
						map.Add(p.Name, tt.ToString());
				}
				append = true;
			}
            if (map.Count > 0 && _params.UseExtensions)
            {
                _output.Append(",\"$map\":");
                WriteStringDictionary(map);
            }
            _output.Append('}');
			//Debug.Log(_output);
            _currentDepth--;
        }

        private void WritePairFast(string name, string value)
        {
            WriteStringFast(name);

            _output.Append(':');

            WriteStringFast(value);
        }

        private void WritePair(string name, object value)
        {
            WriteString(name);

            _output.Append(':');

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.Append('[');

            var pendingSeperator = false;

            foreach (var obj in array)
            {
                if (pendingSeperator) _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.Append(']');
        }

        private void WriteStringDictionary(IEnumerable dic)
        {
            _output.Append('{');
			
            var pendingSeparator = false;

			var idic = (IDictionary)dic;
			
			foreach (var key in idic.Keys)
			{
				var dicValue = idic[key];
				
				if (pendingSeparator) _output.Append(',');
				var k = (string)key;
				WritePair(k, dicValue);
				pendingSeparator = true;
			}
            _output.Append('}');
        }

		private void WriteDictionary(IEnumerable dic)
        {
            _output.Append('[');

            var pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');
                _output.Append('{');
                WritePair("k", entry.Key);
                _output.Append(",");
                WritePair("v", entry.Value);
                _output.Append('}');

                pendingSeparator = true;
            }
            _output.Append(']');
        }

        private void WriteStringFast(string s)
        {
            _output.Append('\"');
            _output.Append(s);
            _output.Append('\"');
        }

        private void WriteString(string s)
        {
            _output.Append('\"');

            var runIndex = -1;
            var l = s.Length;
            for (var index = 0; index < l; ++index)
            {
                var c = s[index];

                if (_useEscapedUnicode)
                {
                    if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }
                else
                {
                    if (c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }

                if (runIndex != -1)
                {
                    _output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c)
                {
                    case '\t': _output.Append("\\t"); break;
                    case '\r': _output.Append("\\r"); break;
                    case '\n': _output.Append("\\n"); break;
                    case '"':
                    case '\\': _output.Append('\\'); _output.Append(c); break;
                    default:
                        if (_useEscapedUnicode)
                        {
                            _output.Append("\\u");
                            _output.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        }
                        else
                            _output.Append(c);

                        break;
                }
            }

            if (runIndex != -1)
                _output.Append(s, runIndex, s.Length - runIndex);

            _output.Append('\"');
        }
    }
}
