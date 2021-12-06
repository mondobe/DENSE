using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Digital_Engineering_Notebook.File_Handling
{
    public static class FileHandlingExtensionMethods
    {
        public static string ToGlobalPath(this string localPath)
        {
            return Path.Combine(ActiveNotebook.activePath, localPath);
        }

        public static string XElementToNameString(this XElement element)
        {
            string toRet = element.Name.ToString();
            if(element.Descendants() as List<XElement> != null)
                (element.Descendants() as List<XElement>).ForEach(x => toRet += "/n" + x.XElementToNameString());
            return toRet;
        }

        public static string MakeXMLAcceptable(this string start)
        {
            string end = "";
            foreach (char c in start)
                if (c != '\0')
                    end += c;
            return end;
        }
    }
}
