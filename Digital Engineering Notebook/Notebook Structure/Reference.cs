using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Reference : ConvertibleItem
    {
        public Dictionary<string, string> references;
        public List<string> defaultValues;

        public Reference(XElement element) : base(element)
        {
            SetDefaultDictValues();
            references = new Dictionary<string, string>();

            foreach (string s in defaultValues)
                if(element.Element(s) != null)
                    references.Add(s, element.Element(s).Value);
        }

        public void AddField(string key, string value)
        {
            if (defaultValues.Contains(key))
                references.Add(key, value);
        }

        void SetDefaultDictValues()
        {
            defaultValues = new List<string>()
            {
                "Title_of_Work",
                "Work_Type",
                "Author",
                "Publishing_Date",
                "Database",
                "Publisher",
                "URL",
                "Access_Date"
            };
        }

        public override XElement ConvertToXML()
        {
            XElement anchor = new XElement(name);
            foreach (KeyValuePair<string, string> kvp in references)
                anchor.Add(new XElement(kvp.Key, kvp.Value));
            return anchor;
        }

        public override List<View> ToXAML()
        {
            List<View> elements = new List<View>();
            elements.Add(new Label
            {
                Text = references["Title_of_Work"],
                FontSize = 24
            });
            foreach (KeyValuePair<string, string> kvp in references)
                if (!string.IsNullOrEmpty(kvp.Value) && kvp.Key != "Title_of_Work")
                    elements.Add(new Label
                    {
                        Text = kvp.Value,
                        HorizontalOptions = LayoutOptions.StartAndExpand
                    });

            return elements;
        }
    }
}
