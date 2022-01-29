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
        // The Contacts, References, and Entries contained in this notebook
        public List<Contact> contacts;
        public List<Reference> references;
        public List<Entry> entries;
        
        /// <summary>
        /// Loads the notebook from an existing XElement.
        /// </summary>
        /// <param name="element">The XElement to be loaded as a notebook</param>
        public Notebook(XElement element) : base(element)
        {
            ActiveNotebook.activeNotebook = this;
            name = element.Name.ToString();
            InitNotebook(element);
        }

        /// <summary>
        /// Creates a new Notebook from a given name.
        /// </summary>
        /// <param name="name">The name that the Notebook will have</param>
        public Notebook(string name) : base(name)
        {
            ActiveNotebook.activeNotebook = this;
            this.name = name;
            // Initialize contacts, references, and entries
            contacts = new List<Contact>();
            references = new List<Reference>();
            entries = new List<Entry>();
        }

        /// <summary>
        /// Creates a new Contact and adds it to the notebook.
        /// </summary>
        /// <returns>The Contact object</returns>
        public Contact AddContact()
        {
            XElement baseElement = new XElement("Contact_" + (contacts.Count + 1) + "_");
            Contact newContact = new Contact(baseElement);
            contacts.Add(newContact);
            return newContact;
        }

        /// <summary>
        /// Creates a new Reference and adds it to the notebook.
        /// </summary>
        /// <returns>The Reference object</returns>
        public Reference AddReference()
        {
            XElement baseElement = new XElement("Reference_" + (references.Count + 1) + "_");
            Reference newReference = new Reference(baseElement);
            references.Add(newReference);
            return newReference;
        }

        /// <summary>
        /// Creates a new Entry and adds it to the notebook.
        /// </summary>
        /// <returns>The Entry object</returns>
        public Entry AddEntry()
        {
            XElement baseElement = new XElement("Entry_" + (entries.Count + 1) + "_");
            Entry newEntry = new Entry(baseElement);
            entries.Add(newEntry);
            return newEntry;
        }

        /// <summary>
        /// Initializes the list of Contacts from the given Notebook element.
        /// </summary>
        /// <param name="element">The loaded notebook element</param>
        void CreateContacts (XElement element)
        {
            contacts = new List<Contact>();
            if (element.Element("Contacts").Elements() == null)
                return;
            // Load each Contact in the existing XElement
            foreach (XElement x in element.Element("Contacts").Elements())
                contacts.Add(new Contact(x));
        }

        /// <summary>
        /// Initializes the list of References from the given Notebook element.
        /// </summary>
        /// <param name="element">The loaded notebook element</param>
        void CreateReferences(XElement element)
        {
            references = new List<Reference>();
            if (!element.Element("References").HasElements)
                return;
            // Load each Reference in the existing XElement
            foreach (XElement x in element.Element("References").Elements())
                references.Add(new Reference(x));
        }

        /// <summary>
        /// Initializes the list of Entries from the given Notebook element.
        /// </summary>
        /// <param name="element">The loaded notebook element</param>
        void CreateEntries(XElement element)
        {
            entries = new List<Entry>();
            if (element.Element("Entries").Elements() == null)
                return;
            // Load each Entry in the existing XElement
            foreach (XElement x in element.Element("Entries").Elements())
                entries.Add(new Entry(x));
        }

        /// <summary>
        /// Initializes the Notebook's list of Contacts, References, and Entries, or loads them
        /// from an existing XElement.
        /// </summary>
        /// <param name="element">The XElement to be loaded from</param>
        void InitNotebook(XElement element)
        {
            CreateContacts(element);
            CreateReferences(element);
            CreateEntries(element);
        }

        /// <summary>
        /// Converts this notebook into an equivalent XElement that stores all of its data
        /// in tree form.
        /// </summary>
        /// <returns>An XElement that can be loaded back as a Notebook</returns>
        public override XElement ConvertToXML()
        {
            // Add the Contacts, References, and Entries lists
            XElement anchor = new XElement(name);
            XElement cElement = new XElement("Contacts");
            XElement rElement = new XElement("References");
            XElement eElement = new XElement("Entries");

            // Add each Contact, Reference, and Entry
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

        /// <summary>
        /// Save this notebook as an XML file to be opened later.
        /// </summary>
        /// <param name="path">The path to save the file to</param>
        public async void SaveXMLFile(string path)
        {
            // Convert the notebook to an XElement first
            XElement anchor = ConvertToXML();

            // If saving to a new file, create a folder for it
            if (!File.Exists(path))
                Directory.CreateDirectory(ActiveNotebook.activePath);

            // Save the XElement to a new FileStream and remove its old contents
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
            long oldLen = stream.Length;
            stream.SetLength(0);
            stream.Flush();
            stream.SetLength(oldLen);
            await Task.Run(() => anchor.Save(stream));
            stream.Close();
        }

        /// <summary>
        /// Opens a Notebook from a given XML file.
        /// </summary>
        /// <param name="path">The path to load the notebook from</param>
        /// <returns>The loaded notebook</returns>
        public static async Task<Notebook> FromFile(string path)
        {
            // If the file doesn't exist, throw an exception
            if (!File.Exists(path))
                throw new FileNotFoundException("The notebook XML file does not exist");

            // Read the text in the notebook and remove any weird oddities
            string fileRead = await Task.Run(() => File.ReadAllText(path));
            string cleanFileRead = fileRead.MakeXMLAcceptable();
            if(cleanFileRead.EndsWith(">>"))
                cleanFileRead = fileRead.Remove(fileRead.Length - 1);

            // (For debug purposes) Write the contents of the notebook to the console
            Console.WriteLine(cleanFileRead);

            // Load the notebook's contents
            XElement anchor = XElement.Parse(cleanFileRead);
            return new Notebook(anchor);
        }

        /// <summary>
        /// Adds a file to the notebook's folder path to be used in the notebook.
        /// </summary>
        /// <param name="path">The original path of the file</param>
        /// <returns>The new global path of the file in the notebook directory</returns>
        public async Task<string> AddFileLocally(string path)
        {
            // (For debug purposes) Write the global path loaded to the console
            Console.WriteLine(path);

            // If the file doesn't exist, throw an exception
            if (!File.Exists(path))
                throw new FileNotFoundException("Please add an existing file");

            // Create the new path for the file
            string localPath = Path.GetFileName(path).ToGlobalPath();

            // Read the original file and write its contents to the new one
            FileStream outfs = File.OpenRead(path);
            FileStream infs = File.Create(localPath);

            byte[] data = new byte[outfs.Length];
            await outfs.ReadAsync(data, 0, (int)outfs.Length);
            await infs.WriteAsync(data, 0, data.Length);

            return localPath;
        }

        /// <summary>
        /// Converts this notebook to a list of XAML Views representing its contents.
        /// </summary>
        /// <returns>The Notebook's contents listed as XAML Views</returns>
        public override List<View> ToXAML()
        {
            List<View> elements = new List<View>();

            // Add the name of the Notebook
            elements.Add(new Label
            {
                Text = name,
                FontSize = 48
            });

            // Add each entry
            foreach (Entry e in entries)
                elements.AddRange(e.ToXAML());

            // Add each Reference
            elements.Add(new Label
            {
                Text = "References",
                FontSize = 36
            });
            foreach (Reference r in references)
                elements.AddRange(r.ToXAML());

            // Add each Contact
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
