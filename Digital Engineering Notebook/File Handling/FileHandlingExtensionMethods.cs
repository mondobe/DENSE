using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.File_Handling
{
    public static class FileHandlingExtensionMethods
    {
        /// <summary>
        /// Extension for string class. Converts a file path to one starting at the root notebook directory.
        /// Alternatively, if a notebook hasn't been opened, starts at the root directory for all notebooks.
        /// </summary>
        /// <param name="localPath">The name of the initial file path.</param>
        /// <param name="startAtRoot">Whether to override the active notebook and start at the directory for 
        /// all notebooks.</param>
        /// <returns>The file path at the root directory as previously specified</returns>
        public static string ToGlobalPath(this string localPath, bool startAtRoot = false)
        {
            // Combine the given path with the active notebook directory
            return Path.Combine( startAtRoot ? ActiveNotebook.basePath : ActiveNotebook.activePath, localPath);
        }

        /// <summary>
        /// For debug purposes only. Converts an XElement to a string with a tree structure.
        /// </summary>
        /// <param name="element">The root of the tree.</param>
        /// <returns>The final name tree.</returns>
        public static string XElementToNameString(this XElement element)
        {
            // Create StringBuilder in case a string is very large
            var sbRet = new StringBuilder();

            // private method created as support for this method
            addRecursiveNameString(element, ref sbRet, 0);
            return sbRet.ToString();
        }

        private static void addRecursiveNameString(XElement element, ref StringBuilder sb, int level)
        {
            // Add newline and dashes to denote tree level
            sb.Append("\n");
            for (int i = 0; i < level; i++)
                sb.Append("-");

            // Add name of element
            sb.Append(element.Name);

            // Add descendents of element, if it exists
            if (element.HasElements)
                foreach (XElement x in element.Elements())
                    addRecursiveNameString(x, ref sb, level + 1);
        }

        /// <summary>
        /// Removes all null characters from a given string. Used for naming an XElement.
        /// </summary>
        /// <param name="start">The original (not necessarily acceptable) string</param>
        /// <returns>A string that is somewhat more acceptable for an XElement name.</returns>

        public static string MakeXMLAcceptable(this string start)
        {
            string end = "";

            // Add each character if it is acceptable
            foreach (char c in start)
                if (c != '\0')
                    end += c;

            return end;
        }

        /// <summary>
        /// Converts a file to a button that triggers an event. If the file is an image, the 
        /// image is displayed.
        /// </summary>
        /// <param name="path">The global path of the file to use as the button</param>
        /// <param name="eh">The event to trigger</param>
        /// <returns>The button as a View.</returns>
        public static View FileToXAMLButton(this string path, EventHandler eh)
        {
            // If the file is an image, open it as an image.
            if (new List<string>
            {
                // List of file types used
                ".png",
                ".jpg",
                ".jpeg",
                ".gif",
                ".bmp",
            }.Contains(Path.GetExtension(path).ToLower()))
            {
                // Display an image button with the file
                ImageButton ib = new ImageButton
                {
                    Source = ImageSource.FromFile(path),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                };
                // Add the event handler to the Clicked trigger
                ib.Clicked += eh;
                return ib;
            }
            else
            {
                // If not an image, display as a normal button
                Button b = new Button
                {
                    Text = Path.GetFileName(path),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                };
                // Add the event handler to the Clicked trigger
                b.Clicked += eh;
                return b;
            }
        }
    }
}
