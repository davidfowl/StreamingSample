using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace client
{
    public class PostStreamContent : HttpContent
    {
        private readonly Func<Stream, Task> _generator;

        public PostStreamContent(Func<Stream, Task> contentGenerator)
        {
            _generator = contentGenerator;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _generator(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}