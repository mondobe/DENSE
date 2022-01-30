using Digital_Engineering_Notebook.File_Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace Digital_Engineering_Notebook
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewNotebook : ContentPage
    {
        /// <summary>
        /// Initializes and refreshes the page.
        /// </summary>
        public ViewNotebook()
        {
            InitializeComponent();
            RefreshNotebook();
        }

        /// <summary>
        /// Loads the name and contents of the notebook
        /// </summary>
        public void RefreshNotebook()
        {
            topLabel.Text = ActiveNotebook.activeNotebook.name;

            // Adds all the contents of the notebook as XAML Views
            foreach (View e in ActiveNotebook.activeNotebook.ToXAML())
            {
                Console.WriteLine(e.ToString());
                layout.Children.Add(e);
            }
        }

        /// <summary>
        /// Adds a contact to the notebook and opens it in the EditItem page.
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        public async void AddContact(object sender, EventArgs e)
        {
            ActiveNotebook.activeItem = ActiveNotebook.activeNotebook.AddContact();
            await Navigation.PushModalAsync(new NavigationPage(new EditItem()));
        }

        /// <summary>
        /// Adds a reference to the notebook and opens it in the EditItem page.
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        public async void AddReference(object sender, EventArgs e)
        {
            ActiveNotebook.activeItem = ActiveNotebook.activeNotebook.AddReference();
            await Navigation.PushModalAsync(new NavigationPage(new EditItem()));
        }

        /// <summary>
        /// Adds a entry to the notebook and opens it in the EditItem page.
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        public async void AddEntry(object sender, EventArgs e)
        {
            ActiveNotebook.activeItem = ActiveNotebook.activeNotebook.AddEntry();
            await Navigation.PushModalAsync(new NavigationPage(new EditItem()));
        }

        /// <summary>
        /// Uses the device's native export feature to save the notebook somewhere
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Export(object sender, EventArgs e)
        {
            // Saves the notebook and inserts all of its contents into a .zip file
            ActiveNotebook.activeNotebook.SaveXMLFile("notebook.xml".ToGlobalPath());
            string zipPath = Path.Combine(ActiveNotebook.basePath, ActiveNotebook.activeNotebook.name.Trim().ToLower() + ".zip");
            File.Delete(zipPath);
            ZipFile.CreateFromDirectory(ActiveNotebook.activePath, zipPath);

            // Opens the native export dialog
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = ActiveNotebook.activeNotebook.name,
                File = new ShareFile(zipPath)
            });
        }
    }
}