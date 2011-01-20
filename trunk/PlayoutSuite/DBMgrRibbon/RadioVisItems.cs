using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBMgrRibbon
{
    class RadioVisItems
    {

        public List<String> images = new List<String>();
        public List<String> texts = new List<String>();


        public RadioVisItems(List<String> images, List<String> texts)
        {
            this.images = images;
            this.texts = texts;
        }

        public static RadioVisItems getFromDataItem(LogListPrj.DataItems.DataSongItem item)
        {
            List<String> img = new List<String>();
            List<String> text = new List<String>();
            img.Add(item.getField("radiovis1"));
            img.Add(item.getField("radiovis2"));
            img.Add(item.getField("radiovis3"));
            img.Add(item.getField("radiovis4"));
            text.Add(item.getField("radiovistxt"));

            return new RadioVisItems(img, text);
        }
    }
}
