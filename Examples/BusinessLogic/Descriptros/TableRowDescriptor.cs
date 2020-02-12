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
    /// <summary>
    /// The descriptors that define the view of a table data raw.
    /// </summary>
    [Serializable]
    [LabelWidth(0)]
    public class TableRowDescriptor : UIDescriptor
    {
        [BeginHorizontalGroup]
        [Width(50)]
        // Forcing using GUIContentLable element instead the default FlatTextBox.
        [CustomControl(typeof(WpfHandler.UI.Controls.GUIContentLabel))]
        public int id = -1;

        [Width(250)]
        public string title = "New item";

        public string description;

        [Width(50)]
        public float price;
    }
}
