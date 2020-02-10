using System;
using System.Windows;
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

namespace SampleShapesControlApp
{
    [Serializable]
    [LabelWidth(50)]
    [HorizontalAlign(HorizontalAlignment.Center)]
    public class ControlPanel : UIDescriptor
    {
        public ShapeType shapeType = ShapeType.Ellipse;

        [Space]

        public float width = 150;
        public float height = 150;

        [Space]

        [Content("To default")]
        public Action toDefaultOnClick;

        public ControlPanel()
        {
            toDefaultOnClick += ToDefault;
        }

        /// <summary>
        /// Drops panel to default values.
        /// </summary>
        public void ToDefault()
        {
            GetFieldByMember("width").Value = 150;
            GetFieldByMember("height").Value = 150;
            GetFieldByMember("shapeType").Value = ShapeType.Ellipse;
        }
    }
}
