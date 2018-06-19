using Hifumi.Models;
using Hifumi.Services;
using Raven.Client.Documents;
using System;

namespace Hifumi.Handlers
{
    public class GuildHandler
    {
        IDocumentStore Store { get; }

        public GuildHandler(IDocumentStore store) => Store = store;

        public GuildModel GetGuild(ulong id)
        {
            using (var session = Store.OpenSession())
                return session.Load<GuildModel>($"{id}");
        }

        public void RemoveGuild(ulong id, string name = null)
        {
            using (var session = Store.OpenSession())
                session.Delete($"{id}");
            LogService.Write("GUILD", string.IsNullOrWhiteSpace(name) ? $"Removed server with id: {id}" : $"Removed config for {name}", ConsoleColor.DarkCyan);
        }

        public void AddGuild(ulong id, string name = null)
        {
            using (var session = Store.OpenSession())
            {
                if (session.Advanced.Exists($"{id}")) return;
                session.Store(new GuildModel
                {
                    Id = $"{id}",
                    Prefix = "h!"
                });
                session.SaveChanges();
            }
            LogService.Write("GUILD", string.IsNullOrWhiteSpace(name) ? $"Added server with id: {id}" : $"Added config for {name}", ConsoleColor.DarkCyan);
        }

        public void Save(GuildModel server)
        {
            if (server == null) return;
            using (var session = Store.OpenSession())
            {
                session.Store(server, server.Id);
                session.SaveChanges();
            }
        }
    }
}
