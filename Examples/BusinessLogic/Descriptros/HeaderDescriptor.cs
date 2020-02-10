using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHandler.UI;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Markups;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;

namespace BusinessLogic.Descriptros
{
    public class HeaderDescriptor : UIDescriptor
    {
        [BeginHorizontalGroup]
        [Width(50)]
        public GUIContent idLabel = new GUIContent("ID", null, "th_id");
        [Width(250)]
        public GUIContent titleLabel = new GUIContent("Title", null, "th_title");
        public GUIContent descLabel = new GUIContent("Description", null, "th_desc");
        [Width(50)]
        public GUIContent priceLabel = new GUIContent("Price", null, "th_price");
    }
}
