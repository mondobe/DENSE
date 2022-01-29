using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public abstract class ConvertibleItem
    {
        // The name of the item
        public string name;

        /// <summary>
        /// Initializes the name of the item from a loaded XElement
        /// </summary>
        /// <param name="element">The loaded XElement</param>
        public ConvertibleItem(XElement element)
        {
            name = element.Name.ToString();
        }

        /// <summary>
        /// Creates a new item with a given name.
        /// </summary>
        /// <param name="name">The entered name</param>
        public ConvertibleItem(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Converts the item to an XElement.
        /// </summary>
        /// <returns>An XElement that can be saved and loaded</returns>
        public abstract XElement ConvertToXML();

        /// <summary>
        /// Converts the item to a nicely formatted string based on the XElement conversion.
        /// </summary>
        /// <returns>A string based on a tree, itself based on the XElement</returns>
        public override string ToString()
        {
            return ConvertToXML().ToString();
        }

        /// <summary>
        /// Converts the item to a collection of XAML Views.
        /// </summary>
        /// <returns>A visual representation of the item as a View</returns>
        public abstract List<View> ToXAML();
    }
}
