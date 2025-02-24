using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using TicketHub.Services.IService;
using TicketHub.Services.Service;
using FirebaseAdmin;

namespace TicketHub.API.Extension;

public static class FirebaseServiceExtensions
{
    public static IServiceCollection AddFirebaseService(this IServiceCollection services)
    {
        var credentialPath = Path.Combine(Directory.GetCurrentDirectory(),
            "tickethub-af919-firebase-adminsdk-wlfom-8e5e557587.json");
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(credentialPath)
        });
        services.AddSingleton(StorageClient.Create(GoogleCredential.FromFile(credentialPath)));
        services.AddScoped<IFirebaseService, FirebaseService>();
        return services;
    }
}