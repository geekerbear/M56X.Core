using System;
using System.Diagnostics;
using System.Net.Http;

namespace M56X.Core.Net
{
    /// <summary>
    /// 下载变化委托
    /// </summary>
    /// <param name="total"></param>
    /// <param name="downloaded"></param>
    /// <param name="progress"></param>
    public delegate void ProgressChangedHandler(long total, long downloaded, double progress);

    /// <summary>
    /// 下载器
    /// </summary>
    public class Downloader : IDisposable
    {
        private readonly HttpClient _client;
        private long _totalBytes;
        private long _downloadedBytes;
        private bool _isResuming;
        private bool disposedValue;

        /// <summary>
        /// 下载进度变化
        /// </summary>
        public event ProgressChangedHandler? ProgressChanged;

        /// <summary>
        /// 状态变化
        /// </summary>
        public event Action<string>? StatusChanged;

        public Downloader()
        {
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            };
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="enableResume"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DownloadFileAsync(string url, string savePath, IProgress<double> progres, bool enableResume = true, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url), HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    progres.Report(0);
                    response.EnsureSuccessStatusCode();
                    _totalBytes = response.Content.Headers.ContentLength ?? -1;
                }

                var tempFilePath = savePath + ".tmp";
                _downloadedBytes = 0;
                _isResuming = false;

                if (enableResume && File.Exists(tempFilePath))
                {
                    _downloadedBytes = new FileInfo(tempFilePath).Length;
                    _isResuming = true;
                    StatusChanged?.Invoke($"正在恢复下载 {FormatFileSize(_downloadedBytes)}");
                }

                using (var fileStream = new FileStream(
                    tempFilePath,
                    _isResuming ? FileMode.Append : FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (_isResuming)
                    {
                        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(
                            _downloadedBytes, null);
                    }

                    using var response = await _client.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);
                    response.EnsureSuccessStatusCode();

                    using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var buffer = new byte[8192];
                    int bytesRead;
                    var lastProgressUpdate = DateTime.MinValue;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
                    {
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                        _downloadedBytes += bytesRead;

                        if (DateTime.Now - lastProgressUpdate > TimeSpan.FromMilliseconds(200) ||
                            _downloadedBytes == _totalBytes)
                        {
                            double progress = _totalBytes > 0 ?
                                (double)_downloadedBytes / _totalBytes * 100 : 0;

                            progres.Report(progress / 100);
                            ProgressChanged?.Invoke(_totalBytes, _downloadedBytes, progress);
                            lastProgressUpdate = DateTime.Now;
                        }
                    }
                }

                if (File.Exists(savePath))
                    File.Delete(savePath);

                File.Move(tempFilePath, savePath);
                StatusChanged?.Invoke("下载完成");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"下载失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 字节显示格式化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatFileSize(long bytes)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(bytes);
            if (bytes == 0) return "0.00 B";

            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.00} {sizes[order]}";
        }

        #region 释放
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Downloader()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
