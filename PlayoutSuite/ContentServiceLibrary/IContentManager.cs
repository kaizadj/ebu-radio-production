using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ContentServiceLibrary
{
    public interface IContentManager
    {

        List<String> getAvailableSlides();

        List<String> getAvailableSlides(String prefix);

        Canvas broadcast(String slidekey);

        Canvas getPreview(String slidekey);
    }
}
