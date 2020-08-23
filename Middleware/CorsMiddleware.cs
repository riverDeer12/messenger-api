using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MessengerAPI.Middleware
{
    public class CorsMiddleware
    {
        private RequestDelegate _nextDelegate;

        public CorsMiddleware(RequestDelegate nextDelegate)
        {
            _nextDelegate = nextDelegate;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await _nextDelegate.Invoke(httpContext);

        }
    }
}
