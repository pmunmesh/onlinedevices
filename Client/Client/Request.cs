using System;
using System.Net;
using System.IO;
using System.Threading;

namespace Client
{
   public static class Request
    {
       public static string response = "";
       public static string responseFromClient(string requUrl)
       {

           #region  File Conversion
           string[] filename = Environment.GetCommandLineArgs();
           string FilePath = "";
           FilePath = Directory.GetCurrentDirectory() + @"\" + filename[1];           

           String strHtml = "";

           using (StreamReader sr = new StreamReader(FilePath))
           {
               while (sr.Peek() > -1)
               {
                   String line = sr.ReadLine();
                   //Console.WriteLine("Read next Line " + line);
                   line = line.Trim();
                   strHtml = strHtml + line;
               }

           }
           #endregion
           response = strHtml;
           return response;
       }
    }
}
