using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PDCoreNew.Helpers
{
    public class ProgressableStreamContent : HttpContent
    {
        /// <summary>
        /// Lets keep buffer of 20kb
        /// </summary>
        private const int defaultBufferSize = 5 * 4096;

        private readonly HttpContent content;
        private readonly int bufferSize;
        //private bool contentConsumed;
        private readonly Action<long, long> progress;

        public ProgressableStreamContent(HttpContent content, Action<long, long> progress) : this(content, defaultBufferSize, progress) { }

        public ProgressableStreamContent(HttpContent content, int bufferSize, Action<long, long> progress)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            this.content = content ?? throw new ArgumentNullException(nameof(content));
            this.bufferSize = bufferSize;
            this.progress = progress;

            foreach (var h in content.Headers)
            {
                Headers.Add(h.Key, h.Value);
            }
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var buffer = new byte[bufferSize];

            TryComputeLength(out long size);

            var uploaded = 0;

            using (var sinput = await content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                while (true)
                {
                    int length = await sinput.ReadAsync(buffer).ConfigureAwait(false);

                    if (length <= 0) break;

                    //downloader.Uploaded = uploaded += length;

                    uploaded += length;

                    progress?.Invoke(uploaded, size);

                    //System.Diagnostics.Debug.WriteLine($"Bytes sent {uploaded} of {size}");

                    await stream.WriteAsync(buffer.AsMemory(0, length)).ConfigureAwait(false);

                    await stream.FlushAsync().ConfigureAwait(false);
                }
            }

            await stream.FlushAsync().ConfigureAwait(false);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Headers.ContentLength.GetValueOrDefault();

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
