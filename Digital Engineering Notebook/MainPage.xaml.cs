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
        /// <summary>
        /// Initialize the notebook. Sometimes, this contains some debug tests.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads a notebook from a given path and opens it to a ViewNotebook page.
        /// I'd like to add this to the NewNotebook method, since they mostly do the 
        /// same stuff.
        /// </summary>
        /// <param name="sender">The button that was pressed</param>
        /// <param name="e">Unused</param>
        private async void LoadNotebook(object sender, System.EventArgs e)
        {
            // Register that the button has been clicked
            Button send = sender as Button;
            send.Text = "Loading...";
            Notebook mostRecent;
            
            // Set the path of the active notebook
            ActiveNotebook.dynamicPath = createName.Text.Trim().ToLower();

            // If the file exists, load it, otherwise, create a new notebook there
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

            // Save the notebook and open it
            mostRecent.SaveXMLFile("notebook.xml".ToGlobalPath());
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        /// <summary>
        /// Creates a new notebook at a given path and open it in a ViewNotebook page.
        /// </summary>
        /// <param name="sender">The button that was pressed</param>
        /// <param name="e">Unused</param>
        private async void NewNotebook(object sender, System.EventArgs e)
        {
            // Register that the button has been clicked
            Button send = sender as Button;
            send.Text = "Creating...";
            Notebook mostRecent = null;

            // Set the path of the active notebook
            ActiveNotebook.dynamicPath = createName.Text.Trim().ToLower();

            // If the file exists, load it, otherwise, create a new notebook there
            mostRecent = await Task.Run(() => CreateNB("notebook"));
            Console.WriteLine("Created new notebook!");

            if (File.Exists("notebook.xml".ToGlobalPath()))
            {
                LoadNotebook(sender, e);
                return;
            }

            // Save the notebook and open it
            mostRecent.SaveXMLFile("notebook.xml".ToGlobalPath());
            Console.WriteLine("Saved notebook!");
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        /// <summary>
        /// Creates a notebook with a given name
        /// </summary>
        /// <param name="name">The name for the notebook</param>
        /// <returns>The created notebook</returns>
        Notebook CreateNB(string name)
        {
            Notebook NB = new Notebook("Notebook");
            return NB;
        }

        /// <summary>
        /// Loads a notebook from a file using the FromFile method.
        /// </summary>
        /// <param name="path">The path to load from</param>
        /// <returns>The loaded notebook</returns>
        async Task<Notebook> LoadNB(string path)
        {
            Console.WriteLine("Loading from " + path.ToGlobalPath());
            return await Notebook.FromFile(path.ToGlobalPath());
        }
    }
}
