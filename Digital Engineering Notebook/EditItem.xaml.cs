using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Digital_Engineering_Notebook.Notebook_Structure;
using Digital_Engineering_Notebook.File_Handling;
using Xamarin.Essentials;
using Contact = Digital_Engineering_Notebook.Notebook_Structure.Contact;
using Entry = Digital_Engineering_Notebook.Notebook_Structure.Entry;

namespace Digital_Engineering_Notebook
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditItem : ContentPage
    {
        // The number of the field or line currently being edited
        int eIndex = 0;

        /// <summary>
        /// Initializes the dropdowns for adding Contacts, References, or files to an Entry.
        /// </summary>
        public EditItem()
        {
            InitializeComponent();

            // Get the active item an determine the type
            ConvertibleItem item = ActiveNotebook.activeItem;
            if(item is Entry)
            {
                // Add the dropdown to choose a contact
                Picker conP = new Picker
                {
                    Title = "Add from Contacts",
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End
                };
                foreach (Contact c in ActiveNotebook.activeNotebook.contacts)
                    conP.Items.Add(c.contacts[c.contacts.Keys.First()]);
                conP.SelectedIndexChanged += new EventHandler(IncludeContact);
                layout.Children.Add(conP);

                // Add the dropdown to choose a reference
                Picker refP = new Picker
                {
                    Title = "Add from References",
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End
                };
                foreach (Reference r in ActiveNotebook.activeNotebook.references)
                    refP.Items.Add(r.references[r.references.Keys.First()]);
                refP.SelectedIndexChanged += new EventHandler(IncludeReference);
                layout.Children.Add(refP);

                // Add the button to choose an external file
                Button fileB = new Button
                {
                    Text = "Add File",
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.End
                };
                fileB.Clicked += IncludeFileAsync;
                layout.Children.Add(fileB);
            }
        }

        /// <summary>
        /// Adds the representation of a Contact to the entry.
        /// </summary>
        /// <param name="sender">The dropdown used to select the contact</param>
        /// <param name="e">Unused</param>
        public void IncludeContact(object sender, EventArgs e)
        {
            // If the picker is reset, don't include anything
            Picker picker = (Picker)sender;
            if (picker.SelectedIndex == -1)
                return;

            // Add the contact as a line internally
            Entry en = (Entry)ActiveNotebook.activeItem;
            Contact c = ActiveNotebook.activeNotebook.contacts[picker.SelectedIndex];
            en.AddLine(c);

            // Add a visual representation of the contact
            layout.Children.Add(new Label
            {
                TextColor = Color.DimGray,
                Text = DateTime.Now.ToShortTimeString(),
                HorizontalOptions = LayoutOptions.Center
            });
            c.ToXAML().ForEach(x => layout.Children.Add(x));

            // Reset the dropdown
            picker.SelectedIndex = -1;
        }

        /// <summary>
        /// Adds the representation of a Reference to the entry.
        /// </summary>
        /// <param name="sender">The dropdown used to select the reference</param>
        /// <param name="e">Unused</param>
        public void IncludeReference(object sender, EventArgs e)
        {
            // If the picker is reset, don't include anything
            Picker picker = (Picker)sender;
            if (picker.SelectedIndex == -1)
                return;

            // Add the reference as a line internally
            Entry en = (Entry)ActiveNotebook.activeItem;
            Reference r = ActiveNotebook.activeNotebook.references[picker.SelectedIndex];
            en.AddLine(r);

            // Add a visual representation of the reference
            layout.Children.Add(new Label
            {
                TextColor = Color.DimGray,
                Text = DateTime.Now.ToShortTimeString(),
                HorizontalOptions = LayoutOptions.Center
            });
            r.ToXAML().ForEach(x => layout.Children.Add(x));

            // Reset the dropdown
            picker.SelectedIndex = -1;
        }

        /// <summary>
        /// Adds a file to the notebook and includes it in the entry as a button or ImageButton
        /// that opens the file.
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        public async void IncludeFileAsync(object sender, EventArgs e)
        {
            // Add a copy of the file to the notebook directory (for ease of exporting)
            string path = (await FilePicker.PickAsync()).FullPath;
            Console.WriteLine("Loading file from " + path);
            string localPath = await ActiveNotebook.activeNotebook.AddFileLocally(path);

            // Add the file to the entry as an internal reference
            Entry en = (Entry)ActiveNotebook.activeItem;
            en.AddLine("FILE::" + path);

            // Add a visual representation of the file
            layout.Children.Add(new Label
            {
                TextColor = Color.DimGray,
                Text = DateTime.Now.ToShortTimeString(),
                HorizontalOptions = LayoutOptions.Center
            });
            View v = localPath.FileToXAMLButton(OpenFileButtonAsync);
            layout.Children.Add(v);
        }

        /// <summary>
        /// Opens a file presented as a Button or ImageButton.
        /// </summary>
        /// <param name="sender">The button clicked containing a file</param>
        /// <param name="e">Unused</param>
        public async void OpenFileButtonAsync(object sender, EventArgs e)
        {
            string openPath;
            // If the button is an ImageButton, treat the file as an image
            if (sender is ImageButton)
            {
                ImageButton ib = (ImageButton)sender;
                openPath = ((FileImageSource)ib.Source).File;
            }
            else
                // Otherwise, use the path on the button
                openPath = ((Button)sender).Text.ToGlobalPath();

            // Open the file externally
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(openPath)
            });
        }

        /// <summary>
        /// When the back button is pressed, go back to the root and open the ViewNotebook page
        /// instead of popping this page. This will refresh the page. (This can probably be done 
        /// better)
        /// </summary>
        /// <returns>Always true, so the page doesn't automatically close</returns>
        protected override bool OnBackButtonPressed()
        {
            ExitPage();
            return true;
        }

        private void StopWorking(object sender, EventArgs e)
        {
            ExitPage();
        }

        /// <summary>
        /// See explanation on OnBackButtonPressed.
        /// </summary>
        private async Task ExitPage()
        {
            await Task.Run(() => ActiveNotebook.activeNotebook.SaveXMLFile("notebook.xml".ToGlobalPath()));
            await Navigation.PopToRootAsync();
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        /// <summary>
        /// When the user finished the current line, add its contents to the current
        /// Contact, Reference, or Entry being edited, then act accordingly.
        /// </summary>
        /// <param name="sender">Unused</param>
        /// <param name="e">Unused</param>
        private async void Entry_Completed(object sender, EventArgs e)
        {
            // Get the current text and active item open
            string text = textEntry.Text;
            ConvertibleItem item = ActiveNotebook.activeItem;

            if(item is Entry)
            {
                // Add the text as a line to the Entry
                Entry en = (Entry)item;
                en.AddLine(text);

                // Add a timestamp and visual representation of the text
                layout.Children.Add(new Label
                {
                    TextColor = Color.DimGray,
                    Text = DateTime.Now.ToShortTimeString(),
                    HorizontalOptions = LayoutOptions.Center
                });
                layout.Children.Add(new Label
                {
                    TextColor = Color.Black,
                    Text = text,
                    HorizontalOptions = LayoutOptions.Start
                });

                // Reset the end timestamp for the Entry
                en.endDT = DateTime.Now;
            }
            else if (item is Contact)
            {
                Contact c = (Contact)item;

                // Add the text to the field currently being worked on
                if (!string.IsNullOrEmpty(text))
                {
                    c.AddField(c.defaultValues[eIndex], text);

                    // Add a visual representation of the field and the text
                    layout.Children.Add(new Label
                    {
                        Text = c.defaultValues[eIndex] + ": " + text,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        FontSize = 20
                    });
                }
                eIndex++;
                // If the Contact is completely filled out, end the session
                if (eIndex == c.defaultValues.Count)
                {
                    await ExitPage();
                    return;
                }
                textEntry.Placeholder = c.defaultValues[eIndex];
            }
            else if (item is Reference)
            {
                Reference r = (Reference)item;

                // Add the text to the field currently being worked on
                if (!string.IsNullOrEmpty(text))
                {
                    r.AddField(r.defaultValues[eIndex], text);

                    // Add a visual representation of the field and the text
                    layout.Children.Add(new Label
                    {
                        Text = r.defaultValues[eIndex] + ": " + text,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        FontSize = 20
                    });
                }
                eIndex++;
                // If the Contact is completely filled out, end the session
                if (eIndex == r.defaultValues.Count)
                {
                    await ExitPage();
                    return;
                }
                textEntry.Placeholder = r.defaultValues[eIndex];
            }

            // Clear the entered text
            textEntry.Text = "";
        }
    }
}