using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class LoadingMiddleware
{
    private readonly RequestDelegate _next;

    public LoadingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(async () =>
        {
            if (context.Response.ContentType != null && context.Response.ContentType.Contains("text/html"))
            {
                var loadingHtml = @"
                <div id='loading' style='display:none;position:fixed;width:100%;height:100%;top:0;left:0;right:0;bottom:0;background-color:rgba(255,255,255,0.8);z-index:9999;text-align:center;'>
                    <img src='/images/loading.gif' style='position:absolute;top:50%;left:50%;transform:translate(-50%,-50%);' />
                </div>
                <script type='text/javascript'>
                    document.onreadystatechange = function () {
                        if (document.readyState === 'loading') {
                            document.getElementById('loading').style.display = 'block';
                        } else {
                            document.getElementById('loading').style.display = 'none';
                        }
                    };
                </script>";

                context.Response.Headers.Add("X-Inject", "true");
                context.Response.Body = new InjectHtmlStream(context.Response.Body, loadingHtml);
            }
        });

        await _next(context);
    }
}

public class InjectHtmlStream : Stream
{
    private readonly Stream _innerStream;
    private readonly string _htmlToInject;

    public InjectHtmlStream(Stream innerStream, string htmlToInject)
    {
        _innerStream = innerStream;
        _htmlToInject = htmlToInject;
    }

    public override void Flush()
    {
        throw new InvalidOperationException("Synchronous operations are disallowed. Use FlushAsync instead.");
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        await _innerStream.FlushAsync(cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
    public override void SetLength(long value) => _innerStream.SetLength(value);

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var html = Encoding.UTF8.GetString(buffer, offset, count);

        if (html.Contains("</body>"))
        {
            html = html.Replace("</body>", $"{_htmlToInject}</body>");
            var newBuffer = Encoding.UTF8.GetBytes(html);
            await _innerStream.WriteAsync(newBuffer, 0, newBuffer.Length, cancellationToken);
        }
        else
        {
            await _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException("Synchronous operations are disallowed. Use WriteAsync instead.");
    }

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => _innerStream.CanSeek;
    public override bool CanWrite => _innerStream.CanWrite;
    public override long Length => _innerStream.Length;
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }
}
