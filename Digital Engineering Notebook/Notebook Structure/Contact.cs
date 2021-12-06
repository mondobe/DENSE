using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Contact : ConvertibleItem
    {
        public Dictionary<string, string> contacts;
        public List<string> defaultValues;

        public Contact(XElement element) : base(element)
        {
            SetDefaultDictValues();
            contacts = new Dictionary<string, string>();

            foreach(string s in defaultValues)
                if(element.HasElement(s))
                    contacts.Add(s, element.Element(s).Value);
        }

        public void AddField(string key, string value)
        {
            if (defaultValues.Contains(key))
                contacts.Add(key, value);
        }

        void SetDefaultDictValues()
        {
            defaultValues = new List<string>()
            {
                "Name",
                "Occupation",
                "Place_of_Work",
                "Address",
                "Zip_Code",
                "Phone_Number",
                "Phone_Number_2",
                "Email_Address",
                "Other_Contact",
                "Notes"
            };
        }

        public override XElement ConvertToXML()
        {
            XElement anchor = new XElement(name);
            foreach (KeyValuePair<string, string> kvp in contacts)
            {
                anchor.Add(new XElement(kvp.Key, kvp.Value));
            }

            return anchor;
        }

        public override List<View> ToXAML()
        {
            List<View> elements = new List<View>();
            if(contacts.ContainsKey("Name"))
                elements.Add(new Label
                {
                    Text = contacts["Name"],
                    FontSize = 24
                });
            foreach (KeyValuePair<string, string> kvp in contacts)
                if (!string.IsNullOrEmpty(kvp.Value) && kvp.Key != "Name")
                    elements.Add(new Label
                    {
                        Text = kvp.Value,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        FontSize = 11
                    });

            return elements;
        }
    }
}
