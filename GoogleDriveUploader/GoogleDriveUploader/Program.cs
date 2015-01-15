using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GoogleDriveUploader
{
    public class Program
    {
        static void Main(string[] args)
        {

            var scopes = new[] { DriveService.Scope.Drive,
                                 DriveService.Scope.DriveFile};

            var dataStore = new FileDataStore("DriveUploader for Windows");

            var secrets = new ClientSecrets { ClientId = Keys.ClientId, ClientSecret = Keys.ClientSecret };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, "ryancormack89", CancellationToken.None, dataStore).Result;

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveUploader for Windows",
            });

            var query = "title = 'DriveUploader Backup' and mimeType = 'application/vnd.google-apps.folder'";

            var files = FileHelper.GetFiles(service, query);

            // If there isn't a directory with this name lets create one.
            if (files.Count == 0)
            {
                files.Add(UploadHelper.CreateDirectory(service));
            }

            if (files.Count != 0)
            {
                string directoryId = files[0].Id;

                File newFile = UploadHelper.UploadFile(service,@"c:\temp\hold.txt",directoryId);

                File updatedFile = UploadHelper.UpdateFile(service, @"c:\temp\hold.txt", directoryId, newFile.Id);
            }

        }
    }
}
