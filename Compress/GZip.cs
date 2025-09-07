using System.IO.Compression;
using System.Text;

namespace M56X.Core.Compress
{
    public static class GZip
    {
        #region 字节
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static byte[] GZipCompress(this byte[] rawData)
        {
            using MemoryStream ms = new();
            using (GZipStream compressor = new(ms, CompressionMode.Compress, true))
            {
                compressor.Write(rawData, 0, rawData.Length);
            }
            return ms.ToArray(); // 保持流打开返回数据
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="compressedData"></param>
        /// <returns></returns>
        public static byte[] GZipDecompress(this byte[] compressedData)
        {
            using MemoryStream inputMs = new(compressedData);
            using MemoryStream outputMs = new();
            using (GZipStream decompressor = new(inputMs, CompressionMode.Decompress))
            {
                decompressor.CopyTo(outputMs);
            }
            return outputMs.ToArray(); // 自动清除内存泄漏风险
        }
        #endregion

        #region 文件
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="compressedPath"></param>
        public static void GZipCompress(this string sourcePath, string compressedPath)
        {
            using FileStream sourceStream = File.OpenRead(sourcePath);
            using FileStream outputStream = File.Create(compressedPath);
            using GZipStream compressStream = new(outputStream, CompressionMode.Compress);
            sourceStream.CopyTo(compressStream); // 自动处理缓冲区
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="compressedPath"></param>
        /// <param name="outputPath"></param>
        public static void GZipDecompress(this string compressedPath, string outputPath)
        {
            using FileStream inputStream = File.OpenRead(compressedPath);
            using FileStream outputStream = File.Create(outputPath);
            using GZipStream decompressStream = new(inputStream, CompressionMode.Decompress);
            decompressStream.CopyTo(outputStream); // 流式处理大文件
        }
        #endregion

        #region 字符串
        /// <summary>
        /// 字符串GZip压缩
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">默认: UTF8</param>
        /// <returns></returns>
        public static byte[] GZipCompress(this string str, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] rawBytes = encoding.GetBytes(str);
            return rawBytes.GZipCompress(); // 复用字节压缩方法
        }

        /// <summary>
        /// 解压为字符串
        /// </summary>
        /// <param name="gzipData"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GZipDecompress(this byte[] gzipData, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] decompressed = gzipData.GZipDecompress();
            return encoding.GetString(decompressed); // 处理中文编码问题:
        }
        #endregion
    }
}
