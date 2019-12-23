//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;

namespace WpfHandler.UI.Controls.Logon
{
    /// <summary>
    /// Interaction logic for RegistrationPanel.xaml
    /// </summary>
    public partial class RegistrationPanel : UserControl
    {
        /// <summary>
        /// Occurs when all <see cref="Forms"/> filled.
        /// </summary>
        public static readonly RoutedEvent FormsFilledEvent;

        /// <summary>
        /// Ocucrs when forms filling canceled.
        /// </summary>
        public static readonly RoutedEvent CancelEvent;

        #region Properties
        /// <summary>
        /// Occurs when all <see cref="Forms"/> filled.
        /// </summary>
        public event RoutedEventHandler FormsFilled
        {
            add => this.AddHandler(FormsFilledEvent, value);
            remove => this.RemoveHandler(FormsFilledEvent, value);
        }

        /// <summary>
        /// Ocucrs when forms filling canceled.
        /// </summary>
        public event RoutedEventHandler Cancel
        {
            add => this.AddHandler(CancelEvent, value);
            remove => this.RemoveHandler(CancelEvent, value);
        }

        ///// <summary>
        ///// Margine of scalable panel.
        ///// </summary>
        //public Thickness RegFormMargin
        //{
        //    get
        //    {
        //        return new Thickness(0, 0, 0, ActualHeight / 2 - regPanel_FormBlock.ActualHeight / 2);
        //    }
        //}

        /// <summary>
        /// Auto-scalable form width.
        /// </summary>
        public double RegFormWidth
        {
            get
            {
                return Math.Min(Math.Max(150, ActualWidth - 20), 400);
            }
        }      
      
        /// <summary>
        /// Error message tha twould displayed at UI's label.
        /// </summary>
        public string ErrorMessage
        {
            set
            {
                // Hide message if null.
                if (value == null)
                {
                    errorLabel.Visibility = Visibility.Collapsed;
                    return;
                }

                // Show message.
                errorLabel.Visibility = Visibility.Visible;
                errorLabel.Content = value;
            }
        }

        /// <summary>
        /// Form descriptors that will be used in the panel.
        /// </summary>
        public List<FormDescriptor> Forms { get; set; } = new List<FormDescriptor>();

        /// <summary>
        /// Reference to a current form.
        /// </summary>
        public FormDescriptor CurrentForm
        { 
            get
            {
                return Forms[CurrentFormIndex];
            }
        }

        /// <summary>
        /// An index of a current active form.
        /// </summary>
        public int CurrentFormIndex
        {
            get { return _CurrentFormIndex; }
            set
            {
                // Claim the index.
                var indexBufer = Math.Max(0, value);
                indexBufer = Math.Min(indexBufer, Forms.Count - 1);

                // Drop if already active.
                if (indexBufer == _CurrentFormIndex) return;
                
                // Updating the value.
                _CurrentFormIndex = indexBufer;

                // Switching to the requested element.
                _ = switchPanel.SwitchToAsync(GetALView(Forms[CurrentFormIndex]),
                    SwitchPanel.AnimationType.AlphaSwipe);
            }
        }
        #endregion

        /// <summary>
        /// A hashtable that contains a bindngs of the <see cref="FormDescriptor"/> 
        /// objects to their  instiniated <see cref="AutoLayoutVeiw"/> elements.
        /// </summary>
        private readonly Hashtable FormToElement = new Hashtable();

        /// <summary>
        /// Idex of the current form.
        /// </summary>
        private int _CurrentFormIndex = 0;

        #region Constructor\desctructor
        static RegistrationPanel()
        {
            FormsFilledEvent = EventManager.RegisterRoutedEvent(
                 "Continue", RoutingStrategy.Bubble,
                 typeof(RoutedEventHandler), typeof(RegistrationPanel));

            CancelEvent = EventManager.RegisterRoutedEvent(
                 "Back", RoutingStrategy.Bubble,
                 typeof(RoutedEventHandler), typeof(RegistrationPanel));
        }

        /// <summary>
        /// The defulat constuctor.
        /// </summary>
        public RegistrationPanel()
        {
            InitializeComponent();
            DataContext = this;

            // Subscribe on events
            SizeChanged += Main_SizeChanged;

            //Loaded += delegate(object sender, RoutedEventArgs e)
            //{

            //};
        }

        /// <summary>
        /// Releasing unmanaged memory.
        /// </summary>
        ~RegistrationPanel()
        {
            // Unsubscribe from events.
            SizeChanged -= Main_SizeChanged;
        }
        #endregion

        /// <summary>
        /// Clear all data filled to form.
        /// </summary>
        public void Clear()
        {
            ErrorMessage = null;
        }

        #region Callbacks
        /// <summary>
        /// Callback that will has been calling when widow size will be changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update size of control panel.
            //BindingOperations.GetBindingExpression(regPanel_FormBlock, MarginProperty).UpdateTarget();

            // Update size of control panel.
            BindingOperations.GetBindingExpression(regPanel_FormBlock, WidthProperty).UpdateTarget();
        }

        /// <summary>
        /// Callback that will caling when panel will loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
        //    Dispatcher.Invoke(DispatcherPriority.Normal,
        //        new Action(delegate ()
        //        {
        //            // Call recomputing of size.
        //            Main_SizeChanged(sender, null);
        //        }));


        //    Dispatcher.Invoke(DispatcherPriority.Render,
        //        new Action(async delegate ()
        //        {
        //            await Task.Delay(1000);

                    // Set the first form as default in case if exist.
                    if (Forms.Count > CurrentFormIndex)
                    {
                        switchPanel.Current = GetALView(Forms[CurrentFormIndex]);
                    }
                    else
                    {
                        // Add the default one if not overrided.
                        var descriptor = new DefaultRegistrationPanelDescriptor();
                        Forms.Add(descriptor);
                        Forms.Add(new AdvancedRegistrationPanelDescriptor());
                        switchPanel.Current = GetALView(descriptor);
                    }

                    Main_SizeChanged(sender, null);
                //}));
        }

        /// <summary>
        /// Occurs when the back button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFormIndex > 0)
            {
                CurrentFormIndex--;
            }
            else
            {
               
                // Calling the handler.
                e = new RoutedEventArgs(CancelEvent, this);
                RaiseEvent(e);
            }
        }

        /// <summary>
        /// Occurs when the continue button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            #region Validation
            // Validating the form.
            var validationReport = CurrentForm.OnValidation();

            if (!validationReport.Result)
            {
                // Showing walidation lof to the error lable,
                ErrorMessage = validationReport.Message;

                // Dropping continue operation.
                return;
            }
            else
            {
                // Dropping a cuurent error message.
                ErrorMessage = "";
            }
            #endregion
                       
            #region Switching to the next
            // Swith to a next form if existed.
            if (CurrentFormIndex < Forms.Count - 1)
            {
                CurrentFormIndex++;
            }
            else
            {
                // Confirming the forms.
                foreach (FormDescriptor fd in Forms)
                {
                    try { fd.OnConfirm(); }
                    catch { };
                }

                // Forms filled. Call a binded handler.
                e = new RoutedEventArgs(FormsFilledEvent, this);
                RaiseEvent(e);
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// Getting the auto layout view binded to the descriptor.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private AutoLayoutVeiw GetALView(FormDescriptor descriptor)
        {
            // Trying to get already instiniated view.
            if(FormToElement[descriptor] is AutoLayoutVeiw alview)
            {
                return alview;
            }

            // Instiniatin a new view.
            alview = new AutoLayoutVeiw
            {
                Descriptor = descriptor
            };

            // Buferizing into the hashtable.
            FormToElement.Add(descriptor, alview);

            return alview;
        }        
    }
}
