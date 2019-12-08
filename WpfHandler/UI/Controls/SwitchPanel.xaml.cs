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
using System.Windows.Media.Animation;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for SwitchPanel.xaml
    /// </summary>
    public partial class SwitchPanel : UserControl
    {
        /// <summary>
        /// Type of animation.
        /// </summary>
        public enum AnimationType
        {
            /// <summary>
            /// Animation not defined. Control will switch to the next element immediately.
            /// </summary>
            None,
            /// <summary>
            /// Switching between elements via making current element transparency 
            /// and rolling of the next element from the border of the current control.
            /// </summary>
            AlphaSwipe,
            /// <summary>
            /// Shadowing the background and swipe current element out of the control's border. 
            /// Removing the shadow when timeline is finished.
            /// </summary>
            DarkSwipe
        }

        /// <summary>
        /// Event that woul be called when animation will be finished.
        /// </summary>
        public event Action AnimationFinished;

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
          "Duration", typeof(TimeSpan), typeof(SwitchPanel));

        /// <summary>
        /// How many time woult take animation duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)this.GetValue(DurationProperty); }
            set { this.SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Current active element.
        /// </summary>
        public UIElement Current
        {
            get
            {
                return current.Children.Count > 0 ? current.Children[0] : null;
            }
        }

        /// <summary>
        /// Animation type that was used during last switch request.
        /// </summary>
        public AnimationType LastAnmimationType { get; protected set; }

        /// <summary>
        /// Element that now in processing.
        /// </summary>
        public UIElement InProcessing { get; protected set; }

        /// <summary>
        /// Element that was ordered during processing.
        /// Would be automaticly call switch to that element after finishing processing.
        /// </summary>
        public UIElement OrderBufer { get; protected set; }

        /// <summary>
        /// Instiniating the switch panel.
        /// </summary>
        public SwitchPanel()
        {
            #region WPF Init
            InitializeComponent();
            DataContext = this;
            #endregion
        }

        /// <summary>
        /// Requesting switch of UI to new Element.
        /// </summary>
        /// <param name="element">Element that would be showed instead current.</param>
        /// <param name="animationType">The type of an animation that will be used during elemetns switching.</param>
        public void SwitchTo(UIElement element, AnimationType animationType)
        {
            // Buferize animation.
            LastAnmimationType = animationType;

            // If switch already in processing.
            if (InProcessing != null)
            {
                // Add element to order.
                OrderBufer = element;
            }
            else
            {
                // Set element as current.
                InProcessing = element;

                next.Children.Clear();
                next.Children.Add(element);

                switch (animationType)
                {
                    case AnimationType.None:
                        Animation_Completed(null, null);
                        break;

                    case AnimationType.AlphaSwipe:
                        AlphaSwipe();
                        break;

                    case AnimationType.DarkSwipe:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Starting animation of elements switching.
        /// </summary>
        protected void StartSwitchAnimation()
        {
            // Disable current menu input.
            current.IsHitTestVisible = false;


        }

        /// <summary>
        /// Swipe animation.
        /// </summary>
        protected virtual void AlphaSwipe()
        {
            // Enable switch panel
            switchPanel.Opacity = 1;

            // Activate opacity drop.
            Animations.FloatAnimation.StartStoryboard(
                current,
                current.Name,
                new PropertyPath(Control.OpacityProperty),
                Duration,
                FillBehavior.Stop,
                1, 0);

            // Moving current UI out of grid.
            Animations.ThinknessAnimation.StartStoryboard(
                current,
                current.Name,
                new PropertyPath(Control.MarginProperty),
                Duration,
                current.Margin,
                new Thickness(ActualWidth,
                              current.Margin.Top,
                              -ActualWidth,
                              current.Margin.Bottom),
                FillBehavior.Stop,
                delegate (Storyboard sb)
                {
                    sb.Completed += Swipe_Completed;
                });

            void Swipe_Completed(object sender, EventArgs e)
            {
                current.Opacity = 0;

                // Activate opacity drop.
                Animations.FloatAnimation.StartStoryboard(
                    switchPanel,
                    switchPanel.Name,
                    new PropertyPath(Control.OpacityProperty),
                    Duration,
                    FillBehavior.Stop,
                    1, 0,
                    delegate (Storyboard sb)
                    {
                        sb.Completed += Animation_Completed;
                    });
            }
        }

        /// <summary>
        /// Callback that will has been calling when swwitch animation would be fisnished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Animation_Completed(object sender, EventArgs e)
        {
            // Drop stoked item.
            InProcessing = null;

            try
            {
                // Drop hidded data.
                UIElement ui = next.Children[0];
                next.Children.Clear();
                next.UpdateLayout();

                // Move next object to current.
                current.Children.Clear();
                current.Margin = next.Margin;
                current.Children.Add(ui);
                current.IsHitTestVisible = true;
                current.Opacity = 1;

                // Enable switch pamel.
                switchPanel.Opacity = 1;

                // Request next order.
                if (OrderBufer != null)
                {
                    SwitchTo(OrderBufer, LastAnmimationType);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            // Inform subscribers.
            AnimationFinished?.Invoke();
        }
    }
}
