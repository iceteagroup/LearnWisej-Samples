using AccessOps.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The share link: an ordinary ASP.NET Core endpoint beside the Wisej.NET app, mapped in
    /// Startup.cs before app.UseWisej(). There is no Wisej.NET session and no SessionContext here -
    /// whatever it answers, it answers to the whole internet - so it binds typed values only,
    /// recomputes the signature, checks the expiry, and answers every failure the same way.
    /// </summary>
    public static class ShareLinkEndpoint
    {
        public static RouteHandlerBuilder MapShareLinks(this WebApplication app)
        {
            return app.MapGet("/share/{id:int}", (int id, long? exp, string sig, ShareLinkService links,
                DocumentRepository documents, DocumentStore store, HttpContext ctx, ILogger<ShareLinkService> log) =>
            {
                var address = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                if (!links.IsValid(id, exp, sig))
                {
                    // tampered, re-signed for another id, expired or missing: one short answer, logged with the address
                    log.LogWarning("Share link refused for document {DocumentId} from {Address}", id, address);
                    return Results.NotFound();
                }

                var doc = documents.FindById(id);
                if (doc?.StoredName == null)
                {
                    log.LogWarning("Share link for missing document {DocumentId} from {Address}", id, address);
                    return Results.NotFound();
                }

                log.LogInformation("Share link opened: document {DocumentId} from {Address}", id, address);
                return Results.File(store.ReadDocument(doc.StoredName), doc.ContentType, doc.Title);
            });
        }
    }
}
