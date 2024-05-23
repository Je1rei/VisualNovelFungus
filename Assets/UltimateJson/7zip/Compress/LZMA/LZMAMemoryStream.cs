using System;
using System.IO;
namespace SevenZip
{
	/// <summary>
	/// Simple class for compression/decompression of memory streams. Based upon LzmaAlone.cs.
	/// </summary>
	public static class UI
	{
		/// <summary>
		/// Compresses a memory stream and writes the compressed bytes to a second stream.
		/// </summary>
		/// <param name="inStream">The input MemoryStream to be compressed.</param>
		/// <param name="outStream">The output MemoryStream which will contain the compressed bytes.</param>
		/// <param name="compressionLevel">The compression level [0-30]. Large values on small memory systems
		/// may cause an OutOfMemory exception.</param>
		public static void CompressMemoryStream(ref MemoryStream inStream, ref MemoryStream outStream, int compressionLevel)
		{

			int dictionary = 1 << compressionLevel;
			Int32 posStateBits = 2;
			Int32 litContextBits = 3;
			Int32 litPosBits = 0;
			Int32 algorithm = 2;
			Int32 numFastBytes = 255;
			string mf = "bt4";
			bool eos = false;

			SevenZip.CoderPropID[] propIDs =
			{
				CoderPropID.DictionarySize,
				CoderPropID.PosStateBits,
				CoderPropID.LitContextBits,
				CoderPropID.LitPosBits,
				CoderPropID.Algorithm,
				CoderPropID.NumFastBytes,
				CoderPropID.MatchFinder,
				CoderPropID.EndMarker
			};
			object[] properties = 
			{
				(Int32)(dictionary),
				(Int32)(posStateBits),
				(Int32)(litContextBits),
				(Int32)(litPosBits),
				(Int32)(algorithm),
				(Int32)(numFastBytes),
				mf,
				eos
			};
			SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
			encoder.SetCoderProperties(propIDs, properties);
			encoder.WriteCoderProperties(outStream);
			Int64 fileSize;
			if (eos) fileSize = -1;
			else fileSize = inStream.Length;
			for (int i = 0; i < 8; i++)
			{
				outStream.WriteByte((Byte)(fileSize >> (8 * i)));
			}
			encoder.Code(inStream, outStream, inStream.Length, 0, null);
			outStream.Seek(0, SeekOrigin.Begin);
		}
		/// <summary>
		/// Decompresses a memory stream and writes the uncompressed bytes to a second stream.
		/// </summary>
		/// <param name="inStream">The input MemoryStream containing compressed bytes.</param>
		/// <param name="outStream">The output MemoryStream which will contain the uncompressed bytes.</param>
		public static void DecompressMemoryStream(ref MemoryStream inStream, ref MemoryStream outStream)
		{
			byte[] properties = new byte[5];
			if (inStream.Read(properties, 0, 5) != 5) throw (new Exception("input .lzma is too short"));
			SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
			decoder.SetDecoderProperties(properties);
			long outSize = 0;
			for (int i = 0; i < 8; i++)
			{
				int v = inStream.ReadByte();
				if (v < 0) throw (new Exception("Can't Read 1"));
				outSize |= ((long)(byte)v) << (8 * i);
			}
			long compressedSize = inStream.Length - inStream.Position;
			decoder.Code(inStream, outStream, compressedSize, outSize, null);
			outStream.Seek(0, SeekOrigin.Begin);
		}
	}
}