using Fluxor;

namespace DemoAuth.Client
{
    public static class CommonServices
    {
        public static void ConfigureCommonServices(IServiceCollection services, IConfiguration configuration)
        {

            // Fluxor
            services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly)
                    .UseReduxDevTools(rdt => { rdt.Name = "DemoAuth"; });
            });

        }
    }
}