using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.IO.Compression;


namespace UltimateJson
{
	/// <summary>
	/// HOW TO USE:
	/// //source JSON
	/// string JSON = "{ \"name\": \"Вася\", \"age\": 35, \"isAdmin\": false, \"friends\": [0,1,2,3] }";
	///
	/// //compressed data
	///byte[] compressed = JSONCompress.CompressJSON (JSON);
	/// //decompress data
	///Debug.Log ("decompressed" + JSONCompress.DecompressJSON(compressed));
	/// 
	/// </summary>
	public class JsonCompress
	{
		/// <summary>
		/// COMPRESS JSON
		/// </summary>
		/// <returns>Array of bytes</returns>
		/// <param name="best">If set to <c>true</c> - compress with LZMA</param>
		/// <param name="json">surce JSON string</param>
		public static byte[] CompressJson(string json, bool best = true)
		{
			var sourceData = Encoding.UTF8.GetBytes(json);

			return best ? CompressFileLzmaMemoryStream(sourceData) : Compress(sourceData);
		}

		/// <summary>
		/// Decompresses to JSON
		/// </summary>
		/// <returns>JSON string</returns>
		/// <param name="best">If set to <c>true</c> try decompress from LZMA</param>
		/// <param name="sourceData">Compressed data</param>
		public static string DecompressJson(byte[] sourceData, bool best = true)
		{
			byte[] decompressedData = null;

			if (best)
			{
				try
				{
					decompressedData = DecompressFileLzmaMemoryStream(sourceData);
				}
				catch (System.Exception e)
				{
					Debug.LogError("Use another compresion: \"Error with decompressed:" + e.Message + "\"");
				}
			}
			else
			{
				try
				{
					decompressedData = Decompress(sourceData);
				}
				catch (System.Exception e)
				{
					Debug.LogError("Use another compresion: \"Error with decompressed:" + e.Message + "\"");
				}
			}

			return decompressedData != null ? Encoding.UTF8.GetString(decompressedData) : "";
		}

		/// <summary>
		/// GZIP DECOMPRESSION
		/// </summary>
		/// <param name="gzip">Compressed data</param>
		private static byte[] Decompress(byte[] gzip)
		{
			// Create a GZIP stream with decompression mode.
			// ... Then create a buffer and write into while reading from the GZIP stream.
			using (var stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
			{
				const int size = 4096;
				var buffer = new byte[size];
				using (var memory = new MemoryStream())
				{
					int count;
					do
					{
						count = stream.Read(buffer, 0, size);
						if (count > 0)
						{
							memory.Write(buffer, 0, count);
						}
					}
					while (count > 0);
					return memory.ToArray();
				}
			}
		}

		/// <summary>
		/// Compress JSON to gzip
		/// </summary>
		/// <param name="raw">Source data</param>
		private static byte[] Compress(byte[] raw)
		{
			using (var memory = new MemoryStream())
			{
				using (var gzip = new GZipStream(memory,
					CompressionMode.Compress, true))
				{
					gzip.Write(raw, 0, raw.Length);
				}
				return memory.ToArray();
			}
		}

		/// <summary>
		/// Compresses JSON in LZMA
		/// </summary>
		/// <returns>Array og bytes - lzma data</returns>
		/// <param name="inBytes">source data(JSON)</param>
		private static byte[] CompressFileLzmaMemoryStream(byte[] inBytes)
		{
			var inputMemoryStream = new MemoryStream(inBytes);
			var outputMemoryStream = new MemoryStream();

			SevenZip.UI.CompressMemoryStream(ref inputMemoryStream,
				ref outputMemoryStream,
				0);

			var outputBytes = outputMemoryStream.ToArray();

			outputMemoryStream.Flush();
			outputMemoryStream.Close();

			return outputBytes;
		}

		/// <summary>
		/// Decompresses to JSON from LZMA
		/// </summary>
		/// <returns>Array of bytes - JSON</returns>
		/// <param name="inBytes">Source LZMA data</param>
		private static byte[] DecompressFileLzmaMemoryStream(byte[] inBytes)
		{
			var inputMemoryStream = new MemoryStream(inBytes);
			var outputMemoryStream = new MemoryStream();

			SevenZip.UI.DecompressMemoryStream(ref inputMemoryStream,
				ref outputMemoryStream);

			var outputBytes = outputMemoryStream.ToArray();

			outputMemoryStream.Flush();
			outputMemoryStream.Close();

			return outputBytes;
		}
	}
}