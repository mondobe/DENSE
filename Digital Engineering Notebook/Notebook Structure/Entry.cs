using Digital_Engineering_Notebook.File_Handling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Entry : ConvertibleItem
    {
        // The individual lines of the entry
        public Dictionary<DateTime, object> entries;
        // The timestamps for the start and end of the entry
        public DateTime startDT, endDT;

        /// <summary>
        /// Creates a new entry or loads one based on an existing XElement
        /// </summary>
        /// <param name="element">The XElement to load</param>
        public Entry(XElement element = null) : base(element)
        {
            entries = new Dictionary<DateTime, object>();

            if (element.HasElements)
            {
                // Load an existing XElement
                IEnumerable<XElement> elements = element.Elements();

                foreach (XElement x in elements)
                {
                    Console.WriteLine(x);
                    if (x.Name == "DateTimes")
                    {
                        // Load the start and end timestamps
                        startDT = DateTime.Parse(x.Element("Start").Value);
                        endDT = DateTime.Parse(x.Element("End").Value);
                    }
                    // Load the embedded contacts, references, or files
                    else if (x.Name == "Contact")
                        entries.Add(DateTime.Parse(x.Element("Time").Value), ActiveNotebook.activeNotebook.contacts[int.Parse(x.Element("Data").Value)]);
                    else if (x.Name == "Reference")
                        entries.Add(DateTime.Parse(x.Element("Time").Value), ActiveNotebook.activeNotebook.references[int.Parse(x.Element("Data").Value)]);
                    else if (x.Name == "File")
                        entries.Add(DateTime.Parse(x.Element("Time").Value), "FILE::" + x.Element("Data").Value);
                    else
                        // Load the text lines
                        entries.Add(DateTime.Parse(x.Element("Time").Value), x.Element("Data").Value);
                }
            }
            else
            {
                // Immediately start and end the creation session
                InitDT();
                EndDT();
            }
        }

        /// <summary>
        /// Start the work session.
        /// </summary>
        public void InitDT()
        {
            startDT = DateTime.Now;
            endDT = new DateTime();
        }

        /// <summary>
        /// End the work session.
        /// </summary>
        public void EndDT()
        {
            endDT = DateTime.Now;
        }

        /// <summary>
        /// Add a line to the entry along with a timestamp.
        /// </summary>
        /// <param name="line">The item making up the content of the line</param>
        public void AddLine(object line)
        {
            entries.Add(DateTime.Now, line);
        }

        /// <summary>
        /// Converts this entry to an XElement that can be loaded back.
        /// </summary>
        /// <returns>An XElement that is equivalent to this Entry</returns>
        public override XElement ConvertToXML()
        {
            XElement anchor = new XElement(name);

            foreach (KeyValuePair<DateTime, object> kvp in entries)
                // Add any Contacts, References, or files
                if(kvp.Value is Contact)
                    anchor.Add(new XElement("Contact",
                        new XElement("Data", ActiveNotebook.activeNotebook.contacts.IndexOf((Contact)kvp.Value)),
                        new XElement("Time", kvp.Key)));
                else if (kvp.Value is Reference)
                    anchor.Add(new XElement("Reference",
                        new XElement("Data", ActiveNotebook.activeNotebook.references.IndexOf((Reference)kvp.Value)),
                        new XElement("Time", kvp.Key)));
                else if (((string)kvp.Value).StartsWith("FILE::"))
                    anchor.Add(new XElement("File",
                        new XElement("Data", ((string)kvp.Value).Substring(6)),
                        new XElement("Time", kvp.Key)));
                else
                    // Add regular text lines
                    anchor.Add(new XElement("Entry",
                    new XElement("Data", kvp.Value), 
                    new XElement("Time", kvp.Key)));

            // Add the start and end timestamps
            anchor.Add(new XElement("DateTimes",
                new XElement("Start", startDT),
                new XElement("End", endDT)
                ));
            return anchor;
        }

        /// <summary>
        /// Converts this Entry into a visual representation as a View.
        /// </summary>
        /// <returns>A View including all of the lines of this entry</returns>
        public override List<View> ToXAML()
        {
            // End the work session if necessary
            if(endDT == null)
                EndDT();

            // Add the timestamps in a larger font
            List<View> elements = new List<View>();
            elements.Add(new Label
            {
                Text = startDT.ToString() + " - " + endDT.ToString(),
                FontSize = 24
            });

            // Add each line in a smaller font
            foreach (KeyValuePair<DateTime, object> kvp in entries)
            {
                // Add the timestamp of the line
                elements.Add(new Label
                {
                    TextColor = Color.DimGray,
                    Text = kvp.Key.ToShortTimeString(),
                    HorizontalOptions = LayoutOptions.Center
                });

                // Add any included Contact, Reference, or Entry
                if (kvp.Value is Contact)
                {
                    Contact c = (Contact)kvp.Value;
                    elements.AddRange(c.ToXAML());
                }
                else if (kvp.Value is Reference)
                {
                    Reference r = (Reference)kvp.Value;
                    elements.AddRange(r.ToXAML());
                }
                else if (((string)kvp.Value).StartsWith("FILE::"))
                {
                    elements.Add(((string)kvp.Value).Substring(6).FileToXAMLButton(new EditItem().OpenFileButtonAsync));
                }
                else
                    // Add any text line
                    elements.Add(new Label
                    {
                        TextColor = Color.Black,
                        Text = kvp.Value.ToString(),
                        HorizontalOptions = LayoutOptions.Start
                    });
            }

            return elements;
        }
    }
}
