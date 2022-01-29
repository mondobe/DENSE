using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Reference : ConvertibleItem
    {
        // The individual reference information fields
        public Dictionary<string, string> references;
        // The default reference field names
        public List<string> defaultValues;

        /// <summary>
        /// Populates each reference form with some default elements.
        /// </summary>
        /// <param name="element">The XElement to load this reference from when loading the notebook</param>
        public Reference(XElement element) : base(element)
        {
            // Set the default field values
            SetDefaultDictValues();
            references = new Dictionary<string, string>();

            // Populate the fields with values if loading from an existing XElement
            foreach (string s in defaultValues)
                if (element.Element(s) != null)
                    references.Add(s, element.Element(s).Value);
        }

        /// <summary>
        /// Adds a field to the Reference if the default values contain that field's name.
        /// </summary>
        /// <param name="key">The name of the field</param>
        /// <param name="value">The information in the field</param>
        public void AddField(string key, string value)
        {
            if (defaultValues.Contains(key))
                references.Add(key, value);
        }

        /// <summary>
        /// Initialize the default field names for the Reference.
        /// </summary>
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

        /// <summary>
        /// Converts the Reference to an XElement.
        /// </summary>
        /// <returns>The XElement representing this Reference</returns>
        public override XElement ConvertToXML()
        {
            // Add each existing field as a descendant of this XElement
            XElement anchor = new XElement(name);
            foreach (KeyValuePair<string, string> kvp in references)
                anchor.Add(new XElement(kvp.Key, kvp.Value));
            return anchor;
        }

        /// <summary>
        /// Converts this Contact to a View containing a list of its fields.
        /// </summary>
        /// <returns>A view displaying the information of this contact</returns>
        public override List<View> ToXAML()
        {
            List<View> elements = new List<View>();
            // If the Reference has a title, display it in a larger font
            if(references.ContainsKey("Title_of_Work"))
                elements.Add(new Label
                {
                    Text = references["Title_of_Work"],
                    FontSize = 24
                });
            // Add each of the individual fields in a smaller font if they exist
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
