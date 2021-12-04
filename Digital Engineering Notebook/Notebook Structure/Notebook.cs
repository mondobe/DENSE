using Digital_Engineering_Notebook.File_Handling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Notebook : ConvertibleItem
    {
        public List<Contact> contacts;
        public List<Reference> references;
        public List<Entry> entries;
        

        public Notebook(XElement element) : base(element)
        {
            name = element.Name.ToString();
            InitNotebook(element);
        }

        public Notebook(string name) : base(name)
        {
            this.name = name;
            contacts = new List<Contact>();
            references = new List<Reference>();
            entries = new List<Entry>();
        }

        public Contact AddContact()
        {
            XElement baseElement = new XElement("Contact_" + (contacts.Count + 1) + "_");
            Contact newContact = new Contact(baseElement);
            contacts.Add(newContact);
            return newContact;
        }

        public Reference AddReference()
        {
            XElement baseElement = new XElement("Reference_" + (references.Count + 1) + "_");
            Reference newReference = new Reference(baseElement);
            references.Add(newReference);
            return newReference;
        }

        public Entry AddEntry()
        {
            XElement baseElement = new XElement("Entry_" + (entries.Count + 1) + "_");
            Entry newEntry = new Entry(baseElement);
            entries.Add(newEntry);
            return newEntry;
        }

        void CreateContacts (XElement element)
        {
            contacts = new List<Contact>();
            if (element.Element("Contacts").Elements() == null)
                return;
            foreach (XElement x in element.Element("Contacts").Elements())
                contacts.Add(new Contact(x));
        }

        void CreateReferences(XElement element)
        {
            references = new List<Reference>();
            if (!element.Element("References").HasElements)
                return;
            foreach (XElement x in element.Element("References").Elements())
                references.Add(new Reference(x));
        }

        void CreateEntries(XElement element)
        {
            entries = new List<Entry>();
            if (element.Element("Entries").Elements() == null)
                return;
            foreach (XElement x in element.Element("Entries").Elements())
                entries.Add(new Entry(x));
        }

        void InitNotebook(XElement element)
        {
            CreateContacts(element);
            CreateReferences(element);
            CreateEntries(element);
        }

        public override XElement ConvertToXML()
        {
            XElement anchor = new XElement(name);
            XElement cElement = new XElement("Contacts");
            XElement rElement = new XElement("References");
            XElement eElement = new XElement("Entries");

            foreach (Contact c in contacts)
                cElement.Add(c.ConvertToXML());
            foreach (Reference r in references)
                rElement.Add(r.ConvertToXML());
            foreach (Entry e in entries)
                eElement.Add(e.ConvertToXML());

            anchor.Add(cElement);
            anchor.Add(rElement);
            anchor.Add(eElement);

            return anchor;
        }

        public async void SaveXMLFile(string path)
        {
            XElement anchor = ConvertToXML();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            long oldLen = stream.Length;
            stream.SetLength(0);
            stream.Flush();
            stream.SetLength(oldLen);
            await Task.Run(() => anchor.Save(stream));
            stream.Close();
        }

        public static async Task<Notebook> FromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("The notebook XML file does not exist");
            string fileRead = await Task.Run(() => File.ReadAllText(path));
            string cleanFileRead = fileRead.MakeXMLAcceptable();
            if(cleanFileRead.EndsWith(">>"))
                cleanFileRead = fileRead.Remove(fileRead.Length - 1);
            Console.WriteLine(cleanFileRead);
            XElement anchor = XElement.Parse(cleanFileRead);
            return new Notebook(anchor);
        }

        public override List<View> ToXAML()
        {
            List<View> elements = new List<View>();

            elements.Add(new Label
            {
                Text = name,
                FontSize = 48
            });

            foreach (Entry e in entries)
                elements.AddRange(e.ToXAML());

            elements.Add(new Label
            {
                Text = "References",
                FontSize = 36
            });
            foreach (Reference r in references)
                elements.AddRange(r.ToXAML());

            elements.Add(new Label
            {
                Text = "Contacts",
                FontSize = 36
            });
            foreach (Contact c in contacts)
                elements.AddRange(c.ToXAML());

            return elements;
        }
    }
}
