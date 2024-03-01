using Carter;

namespace MooPing.Api
{
    public class ExampleModule : CarterModule
    {
        private readonly ILogger<ExampleModule> _logger;
        public ExampleModule(ILogger<ExampleModule> logger) : base("/api/example")
        {
            base.WithTags("Examples");
            this._logger = logger;
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            //Get Request
            app.MapGet("/", () =>
            {
                return Results.Ok("Ok");
            });

            app.MapGet("/hello", () => "Hello World");
        }
    }
}
