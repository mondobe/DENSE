using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Digital_Engineering_Notebook.Notebook_Structure;

namespace Digital_Engineering_Notebook
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditItem : ContentPage
    {
        int eIndex = 0;

        public EditItem()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopToRootAsync();
            Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
            return true;
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            string text = textEntry.Text;
            ConvertibleItem item = ActiveNotebook.activeItem;
            if(item is Notebook_Structure.Entry)
            {
                Notebook_Structure.Entry en = (Notebook_Structure.Entry)item;
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

                en.endDT = System.DateTime.Now;
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
                textEntry.Placeholder = c.defaultValues[eIndex];
                if (eIndex == c.defaultValues.Count)
                {
                    await Navigation.PopToRootAsync();
                    await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
                }
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

                textEntry.Placeholder = r.defaultValues[eIndex];

                if (eIndex == r.defaultValues.Count)
                {
                    await Navigation.PopToRootAsync();
                    await Navigation.PushModalAsync(new NavigationPage(new ViewNotebook()));
                }
            }

            textEntry.Text = "";
        }
    }
}