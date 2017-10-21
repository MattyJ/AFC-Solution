using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fujitsu.AFC.Extensions.CSOM
{
    [ExcludeFromCodeCoverage]
    public static class FolderExtensions
    {
        private const string FileExtension = ".aspx";
        private const string ServerRelativeUrl = "ServerRelativeUrl";
        public static string GetPageName(this Folder folder, FileCollection files, string filename)
        {
            var index = 0;
            var pageName = $"{filename}{FileExtension}";
            var pageUrl = $"{folder.ServerRelativeUrl}/{pageName}";

            while (files.Any(file => file.ServerRelativeUrl == pageUrl))
            {
                index++;
                pageName = $"{filename}{index}{FileExtension}";
                pageUrl = $"{folder.ServerRelativeUrl}/{pageName}";
            }
            return pageName;
        }

        public static void CheckFolderFileSizesWithinSizeLimit(this Folder folder, ClientContext context, int pin, string listName, long maximumLengthBytes)
        {
            context.Load(folder, f => f.Name, f => f.Folders, f => f.Files);
            context.ExecuteQuery();
            var fileCol = folder.Files;
            foreach (var file in fileCol)
            {
                if (file.Length > maximumLengthBytes)
                {
                    throw new ApplicationException($"MergePin Source File Size Violation : PIN - {pin} ; List Name - {listName} ; Folder Name - {folder.Name} ; File Name - {file.Name} ; File Size - {file.Length} bytes");
                }
            }

            foreach (var subFolder in folder.Folders.Where(f => f.Name != "Forms"))
            {
                subFolder.CheckFolderFileSizesWithinSizeLimit(context, pin, listName, maximumLengthBytes);
            }
        }

        public static void MoveFilesTo(this Folder folder, string folderUrl)
        {
            var ctxSrc = (ClientContext)folder.Context;

            if (!ctxSrc.Web.IsPropertyAvailable(ServerRelativeUrl))
            {
                ctxSrc.Load(ctxSrc.Web, w => w.ServerRelativeUrl);
            }
            ctxSrc.Load(folder, f => f.Files, f => f.ServerRelativeUrl, f => f.Folders);
            ctxSrc.ExecuteQuery();

            //Ensure target folder exists
            EnsureFolder(ctxSrc.Web.RootFolder, folderUrl.Replace(ctxSrc.Web.ServerRelativeUrl, string.Empty));
            foreach (var file in folder.Files)
            {
                var targetFileUrl = file.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                file.MoveTo(targetFileUrl, MoveOperations.Overwrite);
            }
            ctxSrc.ExecuteQuery();

            foreach (var subFolder in folder.Folders)
            {
                var targetFolderUrl = subFolder.ServerRelativeUrl.Replace(folder.ServerRelativeUrl, folderUrl);
                subFolder.MoveFilesTo(targetFolderUrl);
            }
        }

        public static void MoveFilesTo(this Folder source, Folder destination, string folderUrl, string listName)
        {
            var ctxSrc = (ClientContext)source.Context;
            if (!ctxSrc.Web.IsPropertyAvailable(ServerRelativeUrl))
            {
                ctxSrc.Load(ctxSrc.Web, w => w.ServerRelativeUrl);
            }
            ctxSrc.Load(source, f => f.Files, f => f.ServerRelativeUrl, f => f.Folders);
            ctxSrc.ExecuteQuery();

            //Ensure target folder exists
            var ctxDest = (ClientContext)destination.Context;
            if (!ctxDest.Web.IsPropertyAvailable(ServerRelativeUrl))
            {
                ctxDest.Load(ctxDest.Web, w => w.ServerRelativeUrl);
            }
            ctxDest.Load(destination, f => f.ServerRelativeUrl);
            ctxDest.ExecuteQuery();

            EnsureFolder(destination, folderUrl.Replace(ctxDest.Web.ServerRelativeUrl, string.Empty).Replace("/" + listName, string.Empty));

            foreach (var file in source.Files)
            {
                if (file.CheckOutType != CheckOutType.None)
                {
                    file.UndoCheckOut();
                    ctxSrc.ExecuteQuery();
                }

                var targetFileUrl = file.ServerRelativeUrl.Replace(source.ServerRelativeUrl, folderUrl).Replace(file.Name, file.Name.GetMergeFilename());

                var fileInfo = File.OpenBinaryDirect(ctxSrc, file.ServerRelativeUrl);
                File.SaveBinaryDirect(ctxDest, targetFileUrl, fileInfo.Stream, true);
            }
            ctxSrc.ExecuteQuery();

            foreach (var subFolder in source.Folders)
            {
                if (subFolder.Name != "Forms")
                {
                    var targetFolderUrl = subFolder.ServerRelativeUrl.Replace(source.ServerRelativeUrl, folderUrl);
                    subFolder.MoveFilesTo(destination, targetFolderUrl, listName);
                }
            }
        }

        private static Folder EnsureFolder(Folder parentFolder, string folderUrl)
        {
            var ctx = parentFolder.Context;
            var folderNames = folderUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (folderNames.Length > 0)
            {
                var folderName = folderNames[0];
                var folders = parentFolder.Folders;
                ctx.Load(folders);
                ctx.ExecuteQuery();

                var folder = folders.FirstOrDefault(i => i.Name == folderName);

                if (folder == null)
                {
                    folder = parentFolder.Folders.Add(folderName);
                    ctx.Load(folder);
                    ctx.ExecuteQuery();
                }

                if (folderNames.Length > 1)
                {
                    var subFolderUrl = string.Join("/", folderNames, 1, folderNames.Length - 1);
                    return EnsureFolder(folder, subFolderUrl);
                }
                return folder;
            }
            return parentFolder;
        }

    }
}
