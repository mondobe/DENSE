using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Digital_Engineering_Notebook.Notebook_Structure;

namespace Digital_Engineering_Notebook
{
    public static class ActiveNotebook
    {
        // The notebook currently being worked on
        public static Notebook activeNotebook = null;

        // The Item currently being worked on
        public static ConvertibleItem activeItem = null;

        // The path of the folder that holds all of the notebooks
        public static string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // The local folder holding the current notebook
        public static string dynamicPath = "";

        // The global path of the current notebook
        public static string activePath
        {
           get
           {
               return Path.Combine(basePath, dynamicPath);
           }
        }
    }
}
