using System.ComponentModel.DataAnnotations;

namespace WordAnalytics.Api.Filters
{
    public sealed class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();

            if (argument is null)
            {
                return TypedResults.Problem(
                    detail: $"Request body of type {typeof(T).Name} is missing.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var validationContext = new ValidationContext(argument);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(argument, validationContext, results, validateAllProperties: true))
            {
                return await next(context);
            }

            var errors = results
                .SelectMany(
                    result => result.MemberNames.DefaultIfEmpty(string.Empty),
                    (result, member) => (Member: member, result.ErrorMessage))
                .GroupBy(x => x.Member)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(x => x.ErrorMessage ?? "Invalid value.").ToArray());

            return TypedResults.ValidationProblem(errors);
        }
    }
}
