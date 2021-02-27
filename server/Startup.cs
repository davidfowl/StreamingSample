using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;

namespace server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context => context.Response.WriteAsync("Hello World"));

                endpoints.MapPost("/api/streaming", async context =>
                {
                    // We're streaming here so there's no max body size nor is there a min data rate
                    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
                    context.Features.Get<IHttpMinRequestBodyDataRateFeature>().MinDataRate = null;

                    // Flush the headers so that the client can start sending
                    await context.Response.Body.FlushAsync();

                    // Echo stream
                    await context.Request.Body.CopyToAsync(context.Response.Body);
                });
            });
        }
    }
}
