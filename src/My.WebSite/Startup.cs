using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using My.WebSite.Models;
using My.WebSite.Services;
using odec.CP.Server.Model.User.Membership.Simple.Contexts;
using odec.CP.Server.Model.User.Membership.Simple.Models;
using odec.Server.Model.Auction.Contexts;
using odec.Server.Model.Category.Contexts;
using odec.Server.Model.Conversation.Contexts;
using odec.Server.Model.OrderProcessing.Contexts;
using odec.Server.Model.Personal.Store.Contexts;
using odec.Server.Model.Store.Contexts.Blob;
using odec.Server.Model.Work.Contexts;

namespace My.WebSite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            var migrationAssemblyName = "My.WebSite";
            var connectionStringName = "DefaultConnection";
            services.AddDbContext<AuctionContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<CategoryContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<EntireMoneyProcessingContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<OrderContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<WorkContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<PortfolioContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));

            services.AddDbContext<UserContextExt>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<PersonalStoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<StoreContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName)));
            services.AddDbContext<ConversationContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly(migrationAssemblyName));
            });

            services.AddIdentity<User, Role>()
                .AddUserStore<UserStore<User, Role, UserContextExt, int>>()
                .AddRoleStore<RoleStore<Role, UserContextExt, int>>()
                .AddEntityFrameworkStores<UserContextExt, int>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
