using Carter;
using MooPing.Database;
using MooPing.Database.Entities;
using Supabase;

namespace MooPing.Api
{
    public class SupabaseExampleModule : CarterModule
    {
        private readonly ILogger<SupabaseExampleModule> _logger;
        public SupabaseExampleModule(ILogger<SupabaseExampleModule> logger) : base("/api/supabaseexample")
        {
            base.WithTags("Examples");
            _logger = logger;
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            //Get Request
            app.MapGet("/", () =>
            {
                return Results.Ok("Ok");
            });

            app.MapGet("/hello", () => "Hello World");

            app.MapPost("/user", (MooPingDbContext mooPingDbContext) =>
            {
                var temp = new User
                {
                    Username = "example",
                    Email = "emal"
                };
                mooPingDbContext.Add(temp);
                mooPingDbContext.SaveChanges();
            });
        }
    }
}
