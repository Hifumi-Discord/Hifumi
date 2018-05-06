using Hifumi.Enums;
using Hifumi.Models;
using Hifumi.Services;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations.Backups;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Hifumi.Handlers
{
    public class DatabaseHandler
    {
        IDocumentStore Store { get; }
        ConfigHandler ConfigHandler { get; }

        public DatabaseHandler(IDocumentStore store, ConfigHandler configHandler)
        {
            Store = store;
            ConfigHandler = configHandler;
        }

        public static DatabaseModel DatabaseConfig
        {
            get
            {
                var dbConfigPath = $"{Directory.GetCurrentDirectory()}/DatabaseConfig.json";
                if (File.Exists(dbConfigPath)) return JsonConvert.DeserializeObject<DatabaseModel>(File.ReadAllText(dbConfigPath));

                File.WriteAllText(dbConfigPath, JsonConvert.SerializeObject(new DatabaseModel(), Formatting.Indented));
                return JsonConvert.DeserializeObject<DatabaseModel>(File.ReadAllText(dbConfigPath));
            }
        }

        public async Task DatabaseCheck()
        {
            await DatabaseSetupAsync().ConfigureAwait(false);
            ConfigHandler.ConfigCheck();
        }

        async Task DatabaseSetupAsync()
        {
            if (Store.Maintenance.Server.Send(new GetDatabaseNamesOperation(0, 5)).Any(x => x == DatabaseConfig.DatabaseName)) return;
            LogService.Write(LogSource.DTB, $"Database {DatabaseConfig.DatabaseName} doesn't exist!", Color.IndianRed);

            await Store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(DatabaseConfig.DatabaseName)));
            LogService.Write(LogSource.DTB, $"Created database {DatabaseConfig.DatabaseName}.", Color.ForestGreen);

            LogService.Write(LogSource.DTB, "Setting up backup operation...", Color.YellowGreen);
            await Store.Maintenance.SendAsync(new UpdatePeriodicBackupOperation(new PeriodicBackupConfiguration
            {
                Name = "Backup",
                BackupType = BackupType.Backup,
                FullBackupFrequency = "*/10 * * * *",
                IncrementalBackupFrequency = "0 2 * * *",
                LocalSettings = new LocalSettings { FolderPath = DatabaseConfig.BackupLocation }
            })).ConfigureAwait(false);

            LogService.Write(LogSource.DTB, "Finished backup operation!", Color.YellowGreen);
            LogService.Write(LogSource.DTB, $"\n1: Restore database\n2: Import dump file", Color.LemonChiffon);

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    await RestoreAsync().ConfigureAwait(false);
                    break;
                case ConsoleKey.D2:
                    Import();
                    break;
            }
        }

        async Task RestoreAsync()
        {
            LogService.Write(LogSource.DTB, "Beginning backup restore...", Color.GreenYellow);
            LogService.Write(LogSource.DTB, "Move backup files to CurrentDIR/backup. Press any key to continue...", Color.YellowGreen);
            Console.ReadKey();
            if (!Directory.GetFiles(DatabaseConfig.BackupLocation).Any())
            {
                LogService.Write(LogSource.DTB, "No files found to restore.", Color.OrangeRed);
                return;
            }
            await (await Store.Maintenance.Server.SendAsync(new RestoreBackupOperation(new RestoreBackupConfiguration
            {
                DatabaseName = DatabaseConfig.DatabaseName,
                BackupLocation = DatabaseConfig.BackupLocation
            }))).WaitForCompletionAsync();
            LogService.Write(LogSource.DTB, "Finished database restore.", Color.ForestGreen);
        }

        void Import()
        {
            LogService.Write(LogSource.DTB, "Move dump.ravendbdump file to root of current directory. Press any key to continue...", Color.YellowGreen);
            Console.ReadKey();
            if (!File.Exists($"{Directory.GetCurrentDirectory()}/dump.ravendbdump"))
            {
                LogService.Write(LogSource.DTB, "No dump file found to restore.", Color.OrangeRed);
                return;
            }
            string arguments = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                arguments = "curl -F 'importOptions={\"IncludeExpired\":true,\"RemoveAnalyzers\":false,\"OperateOnTypes\":\"DatabaseRecord,Documents,Conflicts,Indexes,RevisionDocuments,Identities,CompareExchange\"}'" +
                    $" -F 'file=@Dump.ravendbdump' {DatabaseConfig.DatabaseUrls[0]}/databases/{DatabaseConfig.DatabaseName[0]}/smuggler/import";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                arguments = "";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
