using Raven.Client.Documents.Session;

namespace PosizioniRoverfrutta.Compatibility;

public static class SessionExtensions
{
    public static T Load<T>(this IDocumentSession session, int id)
    {
        var name = session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(T));
        return session.Load<T>(name + "/" + id);
    }

    public static T Load<T>(this IDocumentSession session, int? id)
    {
        return id.HasValue ? session.Load<T>(id.Value) : default;
    }
}
