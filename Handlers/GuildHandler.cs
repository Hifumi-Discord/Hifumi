using Hifumi.Enums;
using Hifumi.Models;
using Hifumi.Services;
using System.Drawing;
using Raven.Client.Documents;

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
            LogService.Write(LogSource.EVT, string.IsNullOrWhiteSpace(name) ? $"Removed Server With Id: {id}" : $"Removed Config For {name}", Color.Crimson);
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
            LogService.Write(LogSource.EVT, string.IsNullOrWhiteSpace(name) ? $"Added Server With Id: {id}" : $"Created Config For {name}", Color.Orange);
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
