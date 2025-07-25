using _5_7_5.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<RomanianSyllableService>();

builder.Services.AddSingleton<RomanianMarkovHaikuGenerator>();

builder.Services.AddSingleton<AzureTranslationService>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var key = config["AzureTranslator:Key"];
    var region = config["AzureTranslator:Region"];
    return new AzureTranslationService(key, region);
});

builder.Services.AddSingleton(provider =>
    new AzureTextToSpeechService(
        builder.Configuration["AzureCognitiveServices:SpeechKey"],
        builder.Configuration["AzureCognitiveServices:Region"]
    )
);

var app = builder.Build();

///http request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Haiku}/{action=Index}/{id?}");

app.Run();