using System;
public class CommonHelper
{
    public static string GetRandomFileName(string fileName)
    {
        string tempFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        return string.Format("{0}{1}", tempFileName, GetFileExtension(fileName));
    }

    public static string GetFileExtension(string fileName)
    {
        string ext = string.Empty;
        int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
        if (fileExtPos >= 0)
            ext = fileName.Substring(fileExtPos, fileName.Length - fileExtPos);

        return ext;
    }
}