using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public abstract class ConvertibleItem
    {
        public string name;

        public ConvertibleItem(XElement element)
        {
            name = element.Name.ToString();
        }

        public ConvertibleItem(string name)
        {
            this.name = name;
        }

        public abstract XElement ConvertToXML();

        public override string ToString()
        {
            return ConvertToXML().ToString();
        }

        public abstract List<View> ToXAML();
    }
}
