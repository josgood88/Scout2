﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library {
   public class HttpClientDownloadWithProgress : IDisposable {
      private const int BUFFER_SIZE = 32*1024;
      private readonly string _downloadUrl;
      private readonly string _destinationFilePath;
      private HttpClient _httpClient;
      public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);
      public event ProgressChangedHandler ProgressChanged;

      public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath) {
         _downloadUrl = downloadUrl;
         _destinationFilePath = destinationFilePath;
      }

      public async Task StartDownload() {
         try {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };
            using (var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false)) {
               await DownloadFileFromHttpResponseMessage(response);
            }
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "HttpClientDownloadWithProgress exception");
            throw;
         }
      }

      private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response) {
         response.EnsureSuccessStatusCode();
         var totalBytes = response.Content.Headers.ContentLength;
         using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
            await ProcessContentStream(totalBytes, contentStream);
         }
      }

      private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream) {
         var totalBytesRead = 0L;
         var readCount = 0L;
         var buffer = new byte[BUFFER_SIZE];
         var isMoreToRead = true;

         using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true)) {
            do {
               var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
               if (bytesRead == 0) {
                  isMoreToRead = false;
                  TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                  continue;
               }
               await fileStream.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
               totalBytesRead += bytesRead;
               readCount += 1;
               if (readCount % 100 == 0)
                  TriggerProgressChanged(totalDownloadSize, totalBytesRead);
            }
            while (isMoreToRead);
         }
      }

      private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead) {
         if (ProgressChanged == null) return;
         double? progressPercentage = null;
         if (totalDownloadSize.HasValue)
            progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);
         ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
      }

      public void Dispose() {
         _httpClient?.Dispose();
      }
   }
}