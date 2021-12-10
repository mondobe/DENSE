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
        private async void LoadNotebook(object sender, System.EventArgs e)
        {
            Button send = sender as Button;
            send.Text = "Loading...";
            Notebook mostRecent;
            
            ActiveNotebook.dynamicPath = createName.Text.Trim().ToLower();

            if (File.Exists("notebook.xml".ToGlobalPath()))
            {
                mostRecent = await LoadNB("notebook.xml");
                Console.WriteLine("Successfully loaded notebook!");
            }
            else
            {
                NewNotebook(sender, e);
                return;
            }

            ActiveNotebook.activeNotebook = mostRecent;
            mostRecent.SaveXMLFile("notebook.xml".ToGlobalPath());
            Console.WriteLine("Saved notebook!");
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        private async void NewNotebook(object sender, System.EventArgs e)
        {
            Button send = sender as Button;
            send.Text = "Creating...";
            Notebook mostRecent = null;

            ActiveNotebook.dynamicPath = createName.Text.Trim().ToLower();

            mostRecent = await Task.Run(() => CreateNB("notebook"));
            Console.WriteLine("Created new notebook!");

            if (File.Exists("notebook.xml".ToGlobalPath()))
            {
                LoadNotebook(sender, e);
                return;
            }

            ActiveNotebook.activeNotebook = mostRecent;
            mostRecent.SaveXMLFile("notebook.xml".ToGlobalPath());
            Console.WriteLine("Saved notebook!");
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        Notebook CreateNB(string name)
        {
            Notebook NB = new Notebook("Notebook");
            return NB;
        }

        async Task<Notebook> LoadNB(string path)
        {
            Console.WriteLine("Loading from " + path.ToGlobalPath());
            return await Notebook.FromFile(path.ToGlobalPath());
        }
    }
}
