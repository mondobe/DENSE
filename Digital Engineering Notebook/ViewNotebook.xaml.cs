﻿using Digital_Engineering_Notebook.File_Handling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Digital_Engineering_Notebook
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewNotebook : ContentPage
    {
        public ViewNotebook()
        {
            InitializeComponent();
            RefreshNotebook();
        }

        public void RefreshNotebook()
        {
            topLabel.Text = ActiveNotebook.activeNotebook.name;

            foreach (View e in ActiveNotebook.activeNotebook.ToXAML())
            {
                Console.WriteLine(e.ToString());
                layout.Children.Add(e);
            }
        }

        public async void AddContact(object sender, EventArgs e)
        {
            ActiveNotebook.activeItem = ActiveNotebook.activeNotebook.AddContact();
            await Navigation.PushModalAsync(new NavigationPage(new EditItem()));
        }

        public async void AddReference(object sender, EventArgs e)
        {
            ActiveNotebook.activeItem = ActiveNotebook.activeNotebook.AddReference();
            await Navigation.PushModalAsync(new NavigationPage(new EditItem()));
        }

        public async void AddEntry(object sender, EventArgs e)
        {
            ActiveNotebook.activeItem = ActiveNotebook.activeNotebook.AddEntry();
            await Navigation.PushModalAsync(new NavigationPage(new EditItem()));
        }

        private async void Export(object sender, EventArgs e)
        {
            ActiveNotebook.activeNotebook.SaveXMLFile("notebook.xml".ToGlobalPath());
            ZipFile.CreateFromDirectory(ActiveNotebook.activePath, "notebook.zip".ToGlobalPath());

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = ActiveNotebook.activeNotebook.name,
                File = new ShareFile("notebook.zip".ToGlobalPath())
            });
        }
    }
}