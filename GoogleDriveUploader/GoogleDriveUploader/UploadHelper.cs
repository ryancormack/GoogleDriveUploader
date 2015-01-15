using System;
using System.Collections.Generic;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;

namespace GoogleDriveUploader
{
    public class UploadHelper
    {
        public static File UpdateFile(DriveService service, string uploadFile, string parent, string fileId)
        {

            if (System.IO.File.Exists(uploadFile))
            {
                var body = new File { Title = System.IO.Path.GetFileName(uploadFile), 
                    Description = "File updated by DriveUploader for Windows", 
                    MimeType = GetMimeType(uploadFile), 
                    Parents = new List<ParentReference>()
                              {
                                  new ParentReference()
                                  {
                                      Id = parent
                                  }
                              } };

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(uploadFile);
                var stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.UpdateMediaUpload request = service.Files.Update(body, fileId, stream, GetMimeType(uploadFile));
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("File does not exist: " + uploadFile);
                return null;
            }

        }

        public static File CreateDirectory(DriveService service)
        {

            File newDirectory = null;

            var body = new File
                       {
                           Title = "DriveUploader Backup", 
                           Description = "Backup of files", 
                           MimeType = "application/vnd.google-apps.folder", 
                           Parents = new List<ParentReference>()
                                     {
                                         new ParentReference()
                                         {
                                             Id = "root"
                                         }
                                     }
                       };
            try
            {
                var request = service.Files.Insert(body);
                newDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return newDirectory;
        }

        private static string GetMimeType(string fileName)
        {
            var mimeType = "application/unknown";
            var extension = System.IO.Path.GetExtension(fileName);

            if (extension == null)
            {
                return mimeType;
            }

            var ext = extension.ToLower();
            var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }

            return mimeType;
        }

        public static File UploadFile(DriveService service, string uploadFile, string parent)
        {
            if (System.IO.File.Exists(uploadFile))
            {
                var body = new File
                           {
                               Title = System.IO.Path.GetFileName(uploadFile), 
                               Description = "File uploaded by DriveUploader For Windows", 
                               MimeType = GetMimeType(uploadFile), 
                               Parents = new List<ParentReference>()
                                         {
                                             new ParentReference()
                                             {
                                                 Id = parent
                                             }
                                         }
                           };

                byte[] byteArray = System.IO.File.ReadAllBytes(uploadFile);
                var stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, GetMimeType(uploadFile));
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("File does not exist: " + uploadFile);
                return null;
            }

        }
    }
}
