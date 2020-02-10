using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfHandler.UI;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout.Markups;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;

namespace BusinessLogic.Descriptros
{
    public class DataTableDescriptor : UIDescriptor
    {
        [HorizontalAlign(HorizontalAlignment.Center)]
        public class ControlPanel : UIDescriptor
        {
            [BeginHorizontalGroup]

            [Content("Refresh", null, "dtt_refresh")]
            public Action Refresh;
            [Content("Add element", null, "dtt_add")]
            public Action NewItemTab;
        }
        
        [Background("Coral")]
        public ControlPanel controlPanel = new ControlPanel();

        public HeaderDescriptor header = new HeaderDescriptor();

        [MaxHeight(200)]
        [AutoCollectionProperties(AddButtonVisibile = false)]
        public List<RowDescriptor> table = new List<RowDescriptor>();
    }
}
