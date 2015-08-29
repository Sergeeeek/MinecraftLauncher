using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftLauncher.Extensions
{
    public static class WebExtensions
    {
        public static async Task<byte[]> DownloadDataTaskAsync(this WebClient client, string url, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            token.Register(client.CancelAsync);
            return await client.DownloadDataTaskAsync(url);
        }

        public static async Task<string> DownloadStringTaskAsync(this WebClient client, string url, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            token.Register(client.CancelAsync);
            return await client.DownloadStringTaskAsync(url);
        }

        public static async Task DownloadFileTaskAsync(this WebClient client, string url, string file, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            token.Register(client.CancelAsync);
            await client.DownloadFileTaskAsync(url, file);
        }
    }
}
