using Digital_Engineering_Notebook.Notebook_Structure;
using Digital_Engineering_Notebook.File_Handling;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System;

namespace Digital_Engineering_Notebook
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void NewNotebook(object sender, System.EventArgs e)
        {
            Button send = sender as Button;
            send.Text = "Creating...";
            Notebook mostRecent = null;

            if(File.Exists("notebook.xml".ToGlobalPath()))
            {
                mostRecent = await LoadNB("notebook.xml");
                Console.WriteLine("Successfully loaded notebook!");
            }

            if (mostRecent == null)
            {
                mostRecent = await Task.Run(() => CreateNB("notebook"));
                Console.WriteLine("Created new notebook!");
            }

            Console.WriteLine("Adding contact...");
            Contact c = mostRecent.AddContact();
            c.AddField("Name", "John Smith");
            c.AddField("Occupation", "Engineer");

            Console.WriteLine("Adding reference...");
            Reference r = mostRecent.AddReference();
            r.AddField("Title_of_Work", "The Engineer's Manual");
            r.AddField("Work_Type", "Manual");
            r.AddField("Author", "Allen Brown");

            Console.WriteLine("Adding entry...");
            Notebook_Structure.Entry en = mostRecent.AddEntry();
            en.AddLine("Wrote the problem statement.");
            en.AddLine("Revised the problem statement to make it more concise.");
            Console.WriteLine("Added entry.");

            ActiveNotebook.activeNotebook = mostRecent;
            mostRecent.SaveXMLFile("notebook.xml".ToGlobalPath());
            Console.WriteLine("Saved notebook!");
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        async Task<Notebook> CreateNB(string name)
        {
            Notebook NB = new Notebook("Notebook");
            await Task.Run(() => NB.SaveXMLFile(name.ToGlobalPath() + ".xml"));
            return NB;
        }

        async Task<Notebook> LoadNB(string path)
        {
            Console.WriteLine("Loading from " + path.ToGlobalPath());
            return await Notebook.FromFile(path.ToGlobalPath());
        }
    }
}
