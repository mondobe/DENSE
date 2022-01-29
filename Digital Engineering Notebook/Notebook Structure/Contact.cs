using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Contact : ConvertibleItem
    {
        // The individual contact information fields
        public Dictionary<string, string> contacts;
        // The default contact field names
        public List<string> defaultValues;

        /// <summary>
        /// Populates each contact form with some default elements.
        /// </summary>
        /// <param name="element">The XElement to load this contact from when loading the notebook</param>
        public Contact(XElement element = null) : base(element)
        {
            // Set the default field values
            SetDefaultDictValues();
            contacts = new Dictionary<string, string>();

            // Populate the fields with values if loading from an existing XElement
            foreach(string s in defaultValues)
                if(element.Element(s) != null)
                    contacts.Add(s, element.Element(s).Value);
        }

        /// <summary>
        /// Adds a field to the Contact if the default values contain that field's name.
        /// </summary>
        /// <param name="key">The name of the field</param>
        /// <param name="value">The information in the field</param>
        public void AddField(string key, string value)
        {
            if (defaultValues.Contains(key))
                contacts.Add(key, value);
        }

        /// <summary>
        /// Initialize the default field names for the Contact.
        /// </summary>
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

        /// <summary>
        /// Converts the Contact to an XElement.
        /// </summary>
        /// <returns>The XElement representing this Contact</returns>
        public override XElement ConvertToXML()
        {
            // Add each existing field as a descendant of this XElement
            XElement anchor = new XElement(name);
            foreach (KeyValuePair<string, string> kvp in contacts)
            {
                anchor.Add(new XElement(kvp.Key, kvp.Value));
            }

            return anchor;
        }

        /// <summary>
        /// Converts this Contact to a View containing a list of its fields.
        /// </summary>
        /// <returns>A view displaying the information of this contact</returns>
        public override List<View> ToXAML()
        {
            List<View> elements = new List<View>();
            // If the Contact has a name, display it in a larger font
            if(contacts.ContainsKey("Name"))
                elements.Add(new Label
                {
                    Text = contacts["Name"],
                    FontSize = 24
                });
            // Add each of the individual fields in a smaller font if they exist
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
