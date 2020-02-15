using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Options;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.Controls;

namespace AutoCollectionExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Binding the int collection to the second AutoCollection control.
            var intCollection = new List<int>(new int[] { 0, 20, 40, 60 });
            col2.Value = intCollection;

            var descripor = new ACExample();
            alView.OnLayout(descripor);
        }
    }

    public class ACExample : UIDescriptor
    {
        public enum Mode
        {
            Option1,
            Option2,
            Option3
        }


        [Header("Autolayout")]

        // Declaring the int array member.

        // The collection binded to that source will not has control buttons.
        // Also you would unable to reorder the elements.
        // That's caused by the `IsFixedSize` state of the `Array`.

        // By default instiniated elements will be represented 
        // by `FlatTextBox` control with the `ValueMode` equal to `Mode.Int`.
        public int[] intArray = new int[] { 0, 1, 2, 3, 4, 5 };

        // The `List` member fully compatible with 
        // all features of the `AutoCollection` control.

        // `Add Event` not overridden so the collection will manage 
        // the string type (the first generic type) 
        // like the default for all new items added via UI.
        public List<string> stringList = new List<string>();
        

        // Redefining some collection options.
        [AutoCollectionProperties(
            // Disabling splitters.
            SplitersDraw = false,

            // Disabling items drag.
            DragAllowed = false,

            // Disabling remove button.
            RemoveButtonVisibile = false)]
        public List<object> flexibleCollection = new List<object>();

        // The empty costructor of the descriptor.
        public ACExample()
        {
            // Adding elemetns to the string list.
            stringList.Add("string 1");
            stringList.Add("string 2");
            stringList.Add("string 3");



            // Adding the enum field.
            flexibleCollection.Add(Mode.Option1);

            // Adding the string field.
            flexibleCollection.Add("example text field");

            // Adding the double field.
            flexibleCollection.Add(2.0d);


            // Subsribing to the descriptor loading event.
            Loaded += ACExample_Loaded;
        }

        // Occurs when descripto UI instiniated and ready to use.
        private void ACExample_Loaded(UIDescriptor obj)
        {
            // Looking for the UI field binded to the `flexibleCollection` member.
            var field = GetField("flexibleCollection");

            // Subscribing on the value changed event.
            field.ValueChanged += Field_ValueChanged;
            var collection = field as AutoCollection;

            // Subscribing on the `Add item` event.
            collection.OnAddClick += delegate (object sender)
            {
                // Adding the int field.
                collection.Add(0);
            };
        }

        // Occurs when `flexibleCollection` changed from UI.
        private void Field_ValueChanged(IGUIField obj, object[] args)
        {
            // Do something.
        }
    }
}
