using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Pasteimg.Backend.Logic.Exceptions
{
    /// <summary>
    /// <see cref="HttpErrorMapper"/> is responsible for mapping exception types to HTTP errors
    /// by collecting and analyzing attributes on those exception types. 
    /// </summary>
    public class HttpErrorMapper
    {

        private ReadOnlyDictionary<Type, HttpErrorMap> mappedExceptions;
        /// <summary>
        /// Gets a read-only dictionary of HttpErrorMap objects that map exception types to HTTP errors.
        /// </summary>
        public ReadOnlyDictionary<Type, HttpErrorMap> MappedExceptions
        {
            get
            {
                if (mappedExceptions is null)
                {
                    mappedExceptions = MapExceptions();
                }
                return mappedExceptions;
            }
        }
        /// <summary>
        /// Gets a dictionary of HTTP error types and their associated names.
        /// </summary>
        /// <returns>A dictionary of HTTP error types and their associated names.</returns>
        public Dictionary<int, string> GetErrorTypes() => MappedExceptions
            .OrderBy(m => m.Value.HttpError.ErrorId)
            .ToDictionary(m => m.Value.HttpError.ErrorId, m => m.Value.Name);
        /// <summary>
        /// Collects all exception types marked with the <see cref="HttpErrorAttribute"/>.
        /// </summary>
        /// <returns>An array of exception types.</returns>
        private Type[] CollectMarkedExceptions()
        {
            return Assembly.GetAssembly(typeof(HttpErrorMap)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Exception)) && t.GetCustomAttribute<HttpErrorAttribute>() is not null).ToArray();
        }
        /// <summary>
        /// Creates a getter function for the given property.
        /// </summary>
        /// <param name="propInfo">The property information for which to create the getter.</param>
        /// <returns>A getter function for the property.</returns>
        private Func<Exception, object?> CreateGetter(PropertyInfo propInfo)
        {
            var exParameter = Expression.Parameter(typeof(Exception), "ex");
            var exp = Expression.Lambda<Func<Exception, object?>>(Expression.Convert(Expression.Property(Expression.Convert(exParameter, propInfo.DeclaringType), propInfo.Name), typeof(object)), exParameter);
            Func<Exception, object?> getter = exp.Compile();
            return getter;
        }
        /// <summary>
        /// Creates an HttpErrorMap object for a given exception type by collecting the HttpErrorAttribute, HttpErrorDetailAttribute, and 
        /// their values from the type's properties.
        /// </summary>
        /// <param name="type">The type of exception to map to an HttpErrorMap object.</param>
        /// <returns>An HttpErrorMap object that contains the HttpErrorAttribute, HttpErrorDetailAttributes, and their values for the given 
        /// exception type.</returns>
        private HttpErrorMap CreateMap(Type type)
        {
            HttpErrorAttribute error = type.GetCustomAttribute<HttpErrorAttribute>();
            Dictionary<string, Func<Exception, object?>> getters = new Dictionary<string, Func<Exception, object>>();
            Dictionary<string, HttpErrorDetailAttribute> details = new Dictionary<string, HttpErrorDetailAttribute>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetCustomAttribute<HttpErrorDetailAttribute>() is HttpErrorDetailAttribute detail)
                {
                    getters.Add(prop.Name, CreateGetter(prop));
                    details.Add(prop.Name, detail);
                }
            }
            return new HttpErrorMap()
            {
                ExceptionType = type,
                HttpError = error,
                Details = new ReadOnlyDictionary<string, HttpErrorDetailAttribute>(details),
                ValueGetters = new ReadOnlyDictionary<string, Func<Exception, object?>>(getters)
            };
        }
        /// <summary>
        /// Maps all exception types marked with the HttpErrorAttribute to HttpErrorMap objects and validates that each HttpErrorAttribute 
        /// ErrorId is unique.
        /// </summary>
        /// <returns>A ReadOnlyDictionary that maps all exception types marked with the HttpErrorAttribute to HttpErrorMap objects.</returns>
        private ReadOnlyDictionary<Type, HttpErrorMap> MapExceptions()
        {
            Dictionary<Type, HttpErrorMap> mappedExceptions = new Dictionary<Type, HttpErrorMap>();
            Type[] exceptions = CollectMarkedExceptions();
            foreach (var ex in exceptions)
            {
                mappedExceptions.Add(ex, CreateMap(ex));
            }
            ValidateMaps(mappedExceptions);
            return new ReadOnlyDictionary<Type, HttpErrorMap>(mappedExceptions);
        }
        /// <summary>
        /// Validates that each <see cref="HttpErrorAttribute.ErrorId"/> is unique across all <see cref="HttpErrorMap"/> objects.
        /// </summary>
        /// <param name="maps">The dictionary of HttpErrorMap objects to validate.</param>
        private void ValidateMaps(Dictionary<Type, HttpErrorMap> maps)
        {
            var result = maps.GroupBy(map => map.Value.HttpError.ErrorId)
                .FirstOrDefault(group => group.Count() > 1);
            if (result is not null)
            {
                throw new TypeLoadException
                    ($"{nameof(HttpErrorAttribute.ErrorId)} must be unique! Inappropriate types:\n"
                    + string.Join("\n", result.Select(r => r.Value.ExceptionType.FullName)));
            }
        }
    }
    /// <summary>
    /// Represents a mapping between an exception type and an HTTP error.
    /// </summary>
    public class HttpErrorMap
    {
        /// <summary>
        /// Gets a dictionary of HTTP error detail attributes and their values for this exception type.
        /// </summary>
        public ReadOnlyDictionary<string, HttpErrorDetailAttribute> Details { get; init; }

        /// <summary>
        /// Gets the type of the exception that this HttpErrorMap represents.
        /// </summary>
        public Type ExceptionType { get; init; }

        /// <summary>
        /// Gets the HTTP error attribute for this exception type.
        /// </summary>
        public HttpErrorAttribute HttpError { get; init; }

        /// <summary>
        /// Gets the name of this HTTP error.
        /// </summary>
        public string Name => ExceptionType.Name.ToLower().Replace("exception", "");

        /// <summary>
        /// Gets a dictionary of getter functions for each property decorated with a <see cref="HttpErrorDetailAttribute"/> for this exception type.
        /// </summary>
        public ReadOnlyDictionary<string, Func<Exception, object?>> ValueGetters { get; init; }
    }

}