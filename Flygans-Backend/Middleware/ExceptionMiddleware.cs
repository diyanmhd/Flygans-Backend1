using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Flygans_Backend.Exceptions;

namespace Flygans_Backend.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException)
            {
                await WriteError(context, "Database constraint error.", HttpStatusCode.BadRequest);
            }
            catch (NotFoundException ex)
            {
                await WriteError(context, ex.Message, HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                await WriteError(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                await WriteError(context, ex.Message, HttpStatusCode.Unauthorized);
            }
            catch (Exception)
            {
                await WriteError(context, "Something went wrong.", HttpStatusCode.InternalServerError);
            }
        }

        private async Task WriteError(HttpContext context, string msg, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var response = new
            {
                success = false,
                message = msg
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
