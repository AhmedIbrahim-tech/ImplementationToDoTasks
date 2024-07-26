using System.Net;

namespace ImplementationToDoTasks.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "text/html";
        var errorHtml = $@"
                <html>
                <head>
                    <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css'>
                </head>
                <body>
                    <div class='modal' tabindex='-1' role='dialog' id='errorModal' style='display: block;'>
                        <div class='modal-dialog' role='document'>
                            <div class='modal-content'>
                                <div class='modal-header'>
                                    <h5 class='modal-title'>Error</h5>
                                    <button type='button' class='close' data-dismiss='modal' aria-label='Close'>
                                        <span aria-hidden='true'>&times;</span>
                                    </button>
                                </div>
                                <div class='modal-body'>
                                    <p>An unexpected error occurred. Please try again later.</p>
                                    <p>{exception.Message}</p>
                                </div>
                                <div class='modal-footer'>
                                    <button type='button' class='btn btn-primary' onclick='window.location.href=""Index""'>Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <script src='https://code.jquery.com/jquery-3.5.1.slim.min.js'></script>
                    <script src='https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.3/dist/umd/popper.min.js'></script>
                    <script src='https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js'></script>
                </body>
                </html>";

        return context.Response.WriteAsync(errorHtml);
    }
}
