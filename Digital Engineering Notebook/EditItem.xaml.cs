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
        int eIndex = 0;

        public EditItem()
        {
            InitializeComponent();

            ConvertibleItem item = ActiveNotebook.activeItem;
            if(item is Entry)
            {
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

        public void IncludeContact(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            if (picker.SelectedIndex == -1)
                return;

            Entry en = (Entry)ActiveNotebook.activeItem;
            Contact c = ActiveNotebook.activeNotebook.contacts[picker.SelectedIndex];
            en.AddLine(c);

            layout.Children.Add(new Label
            {
                TextColor = Color.DimGray,
                Text = DateTime.Now.ToShortTimeString(),
                HorizontalOptions = LayoutOptions.Center
            });
            c.ToXAML().ForEach(x => layout.Children.Add(x));
            picker.SelectedIndex = -1;
        }

        public void IncludeReference(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            if (picker.SelectedIndex == -1)
                return;

            Entry en = (Entry)ActiveNotebook.activeItem;
            Reference r = ActiveNotebook.activeNotebook.references[picker.SelectedIndex];
            en.AddLine(r);

            layout.Children.Add(new Label
            {
                TextColor = Color.DimGray,
                Text = DateTime.Now.ToShortTimeString(),
                HorizontalOptions = LayoutOptions.Center
            });
            r.ToXAML().ForEach(x => layout.Children.Add(x));
            picker.SelectedIndex = -1;
        }

        public async void IncludeFileAsync(object sender, EventArgs e)
        {
            string path = (await FilePicker.PickAsync()).FullPath;
            Console.WriteLine("Loading file from " + path);
            string localPath = await ActiveNotebook.activeNotebook.AddFileLocally(path);

            Entry en = (Entry)ActiveNotebook.activeItem;
            en.AddLine("FILE::" + path);

            layout.Children.Add(new Label
            {
                TextColor = Color.DimGray,
                Text = DateTime.Now.ToShortTimeString(),
                HorizontalOptions = LayoutOptions.Center
            });
            View v = localPath.FileToXAMLButton(OpenFileButtonAsync);
            layout.Children.Add(v);
        }

        public async void OpenFileButtonAsync(object sender, EventArgs e)
        {
            string openPath;
            if (sender is ImageButton)
            {
                ImageButton ib = (ImageButton)sender;
                openPath = ((FileImageSource)ib.Source).File;
            }
            else
                openPath = ((Button)sender).Text.ToGlobalPath();

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(openPath)
            });
        }

        protected override bool OnBackButtonPressed()
        {
            ExitPage();
            return true;
        }

        private void StopWorking(object sender, EventArgs e)
        {
            ExitPage();
        }

        private async Task ExitPage()
        {
            await Task.Run(() => ActiveNotebook.activeNotebook.SaveXMLFile("notebook.xml".ToGlobalPath()));
            await Navigation.PopToRootAsync();
            await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            string text = textEntry.Text;
            ConvertibleItem item = ActiveNotebook.activeItem;
            if(item is Entry)
            {
                Entry en = (Entry)item;
                en.AddLine(text);

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

                en.endDT = DateTime.Now;
            }
            else if (item is Contact)
            {
                Contact c = (Contact)item;

                if (!string.IsNullOrEmpty(text))
                {
                    c.AddField(c.defaultValues[eIndex], text);

                    layout.Children.Add(new Label
                    {
                        Text = c.defaultValues[eIndex] + ": " + text,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        FontSize = 20
                    });
                }
                eIndex++;
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

                if (!string.IsNullOrEmpty(text))
                {
                    r.AddField(r.defaultValues[eIndex], text);

                    layout.Children.Add(new Label
                    {
                        Text = r.defaultValues[eIndex] + ": " + text,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        FontSize = 20
                    });
                }
                eIndex++;
                if (eIndex == r.defaultValues.Count)
                {
                    await ExitPage();
                    return;
                }
                textEntry.Placeholder = r.defaultValues[eIndex];
            }

            textEntry.Text = "";
        }
    }
}