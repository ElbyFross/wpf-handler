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
    public class NewItemTabDescriptor : UIDescriptor
    {
        [Content("Back", null, "nit_back")]
        [Background("Coral")]
        public Action ToTableTab;

        public RowDescriptor row = new RowDescriptor();

        [FontWeight(FontWeightAttribute.WeightType.ExtraBold)]
        [HorizontalAlign(System.Windows.HorizontalAlignment.Center)]
        public Action Confirm;

        public NewItemTabDescriptor()
        {
            Loaded += delegate (UIDescriptor obj)
            {
                ((System.Windows.FrameworkElement)row.GetFieldByMember("id")).Visibility =
                System.Windows.Visibility.Collapsed;
            };
        }
    }
}
