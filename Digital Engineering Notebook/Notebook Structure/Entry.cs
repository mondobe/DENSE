﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xamarin.Forms;

namespace Digital_Engineering_Notebook.Notebook_Structure
{
    public class Entry : ConvertibleItem
    {
        public Dictionary<DateTime, object> entries;
        public DateTime startDT, endDT;

        public Entry(XElement element) : base(element)
        {
            entries = new Dictionary<DateTime, object>();

            if (element.HasElements)
            {
                IEnumerable<XElement> elements = element.Elements();

                foreach (XElement x in elements)
                {
                    Console.WriteLine(x);
                    if (x.Name == "DateTimes")
                    {
                        startDT = DateTime.Parse(x.Element("Start").Value);
                        endDT = DateTime.Parse(x.Element("End").Value);
                    }
                    else
                        entries.Add(DateTime.Parse(x.Element("Time").Value), x.Element("Data").Value);
                }
            }
            else
            {
                InitDT();
                EndDT();
            }
        }

        public void InitDT()
        {
            startDT = DateTime.Now;
            endDT = new DateTime();
        }

        public void EndDT()
        {
            endDT = DateTime.Now;
        }

        public void AddLine(string line)
        {
            entries.Add(DateTime.Now, line);
        }

        public override XElement ConvertToXML()
        {
            XElement anchor = new XElement(name);

            foreach (KeyValuePair<DateTime, object> kvp in entries)
                anchor.Add(new XElement("Entry",
                    new XElement("Data", kvp.Value), 
                    new XElement("Time", kvp.Key)));

            anchor.Add(new XElement("DateTimes",
                new XElement("Start", startDT),
                new XElement("End", endDT)
                ));
            return anchor;
        }

        public override List<View> ToXAML()
        {
            if(endDT == null)
                EndDT();

            List<View> elements = new List<View>();
            elements.Add(new Label
            {
                Text = startDT.ToString() + " - " + endDT.ToString(),
                FontSize = 24
            });

            foreach (KeyValuePair<DateTime, object> kvp in entries)
            {
                elements.Add(new Label
                {
                    TextColor = Color.DimGray,
                    Text = kvp.Key.ToShortTimeString(),
                    HorizontalOptions = LayoutOptions.Center
                });
                elements.Add(new Label
                {
                    TextColor = Color.Black,
                    Text = kvp.Value.ToString(),
                    HorizontalOptions = LayoutOptions.Start
                });
            }

            return elements;
        }
    }
}
