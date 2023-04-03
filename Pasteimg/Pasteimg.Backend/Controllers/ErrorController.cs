using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Pasteimg.Backend.Logic.Exceptions;
using System.Security.AccessControl;

namespace Pasteimg.Backend.Controllers
{
    /// <summary>
    /// Controller responsible for handling errors that occur within the application.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly HttpErrorMapper mapper;
        public ErrorController(IWebHostEnvironment environment, HttpErrorMapper mapper)
        {
            this.mapper = mapper;
            this.environment = environment;
        }
        /// <summary>
        /// Action for handling errors within the application.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///     <item><strong>failedlogin 1 - Unauthorized 401:</strong> Failed login attempt.</item>
        ///     <item><strong>invalidentity 2 - BadRequest 400:</strong> Posted resource was invalid.</item>
        ///     <item><strong>lockout 3 - BadRequest 400:</strong> Resource was locked out.</item>
        ///     <item><strong>notfound 4 - NotFound 404:</strong> Given ID not found.</item>
        ///     <item><strong>passwordrequired 5 - Unauthorized 401:</strong> Password was not entered.</item>
        ///     <item><strong>sessionkeymissing 6 - Unauthorized 401:</strong> Session key was missing.</item>
        ///     <item><strong>somethingwentwrong 7 - InternalServerError 500:</strong> Something went wrong.</item>
        ///     <item><strong>unauthorized 8 - Unauthorized 401:</strong> Client was not authorized to access a resource.</item>
        ///     <item><strong>wrongpassword 9 - Unauthorized 401:</strong> Given password was wrong. </item>
        /// </list>
        /// </returns>
        [Route("/error")]
        public ActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var model = GetErrorModel(context.Error);
            return new ObjectResult(model)
            {
                StatusCode = (int)model.StatusCode
            };
        }
     
        /// <summary>
        /// Creates the details dictionary for the given exception.
        /// </summary>
        /// <param name="ex">The exception to create the details for.</param>
        /// <param name="map">The HttpErrorMap to use for details.</param>
        /// <returns>A dictionary of details.</returns>
        private Dictionary<string,object> CreateDetails(Exception ex, HttpErrorMap map)
        {
            Dictionary<string,object> details = new Dictionary<string, object>();
            foreach (var propName in map.Details.Keys)
            {
                string? customName = map.Details[propName].CustomName;
                object? value = map.ValueGetters[propName](ex);
                if (value is not null)
                {
                    string name = customName ?? propName;
                    if(details.ContainsKey(name))
                    {
                        name = propName + "-" + name;
                        if(details.ContainsKey(name))
                        {
                            name = map.Name + "-" + name;
                        }
                    }

                    if(!details.ContainsKey(name))
                    {
                        details.Add(name, value);
                    }
                }
            }

            if (environment.IsDevelopment())
            {
                if (ex.InnerException is not null)
                {
                    details.Add("InnerException", ex.InnerException.Message);
                }
            }

            return details;
        }
        /// <summary>
        /// Gets the error model for the given exception.
        /// </summary>
        /// <param name="ex">The exception to get the error model for.</param>
        /// <returns>The error model.</returns>
        private ErrorModel GetErrorModel(Exception ex)
        {
            Type type = ex.GetType();
            if (!mapper.MappedExceptions.ContainsKey(type))
            {
                ex = new SomethingWentWrongException(ex);
                type = ex.GetType();
            }

            HttpErrorMap map = mapper.MappedExceptions[type];

            return new ErrorModel()
            {
                Name = map.Name,
                Message = map.HttpError.CustomHttpMessage ?? ex.Message,
                StatusCode = map.HttpError.StatusCode,
                ErrorId = map.HttpError.ErrorId,
                Details = CreateDetails(ex, map)
            };
        }
    }
}