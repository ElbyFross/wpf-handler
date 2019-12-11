using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHandler.UI.Controls;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Options;

namespace TextFiledControls
{
    public class CustomAutolayoutDescriptor : UIDescriptor
    {
        [Header("Autolayout example")]
        [Label("Look to the MainWindow constructor")]
        [Label("to find the descriptor.")]
        public string stringField = "abc";

        [CustomControl(typeof(FlatPasswordBox))]
        public string PasswordProperty { get; set; } = "pass";
    }
}
