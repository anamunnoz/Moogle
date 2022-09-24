using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MoogleEngine;

         MoogleEngine.preSearch.documentosGuardados = preSearch.GuardarTxt(preSearch.ObtenerRuta()); 

         MoogleEngine.preSearch.diccionarioTfIDf = preSearch.TfIdf(preSearch.documentosGuardados, preSearch.ObtenerRuta());
           

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();


