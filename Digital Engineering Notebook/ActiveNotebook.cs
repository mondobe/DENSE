using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Digital_Engineering_Notebook.Notebook_Structure;

namespace Digital_Engineering_Notebook
{
    public static class ActiveNotebook
    {
        public static Notebook activeNotebook = null;
        public static ConvertibleItem activeItem = null;
        public static string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string dynamicPath = "";
        public static string activePath
        {
           get
           {
               return Path.Combine(basePath, dynamicPath);
           }
        }
    }
}
