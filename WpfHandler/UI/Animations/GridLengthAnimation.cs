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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Controls;

namespace WpfHandler.UI.Animations
{
    /// <summary>
    /// Animates a grid length value just like the DoubleAnimation animates a double value
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        private bool isCompleted;

        /// <summary>
        /// Marks the animation as completed
        /// </summary>
        public bool IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value; }
        }

        /// <summary>
        /// Sets the reverse value for the second animation
        /// </summary>
        public double ReverseValue
        {
            get { return (double)GetValue(ReverseValueProperty); }
            set { SetValue(ReverseValueProperty, value); }
        }


        /// <summary>
        /// Dependency property. Sets the reverse value for the second animation
        /// </summary>
        public static readonly DependencyProperty ReverseValueProperty =
            DependencyProperty.Register("ReverseValue", typeof(double), typeof(GridLengthAnimation), new UIPropertyMetadata(0.0));


        /// <summary>
        /// Returns the type of object to animate
        /// </summary>
        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        /// <summary>
        /// Creates an instance of the animation object
        /// </summary>
        /// <returns>Returns the instance of the GridLengthAnimation</returns>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        /// <summary>
        /// Dependency property for the From property
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                typeof(GridLengthAnimation));

        /// <summary>
        /// CLR Wrapper for the From depenendency property
        /// </summary>
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the To property
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength),
                typeof(GridLengthAnimation));

        /// <summary>
        /// CLR Wrapper for the To property
        /// </summary>
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        AnimationClock clock;

        /// <summary>
        /// registers to the completed event of the animation clock
        /// </summary>
        /// <param name="clock">the animation clock to notify completion status</param>
        void VerifyAnimationCompletedStatus(AnimationClock clock)
        {
            if (this.clock == null)
            {
                this.clock = clock;
                this.clock.Completed += new EventHandler(delegate (object sender, EventArgs e) 
                { isCompleted = true; });
            }
        }

        /// <summary>
        /// Animates the grid let set
        /// </summary>
        /// <param name="defaultOriginValue">The original value to animate</param>
        /// <param name="defaultDestinationValue">The final value</param>
        /// <param name="animationClock">The animation clock (timer)</param>
        /// <returns>Returns the new grid length to set</returns>
        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue, AnimationClock animationClock)
        {
            //check the animation clock event
            VerifyAnimationCompletedStatus(animationClock);

            //check if the animation was completed
            if (isCompleted)
                return (GridLength)defaultDestinationValue;

            //if not then create the value to animate
            double fromVal = this.From.Value;
            double toVal = this.To.Value;

            //check if the value is already collapsed
            if (((GridLength)defaultOriginValue).Value == toVal)
            {
                fromVal = toVal;
                toVal = this.ReverseValue;
            }
            else
                //check to see if this is the last tick of the animation clock.
                if (animationClock.CurrentProgress.Value == 1.0)
                return To;

            if (fromVal > toVal)
                return new GridLength((1 - animationClock.CurrentProgress.Value) *
                    (fromVal - toVal) + toVal, this.From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            else
                return new GridLength(animationClock.CurrentProgress.Value *
                    (toVal - fromVal) + fromVal, this.From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
        }

        /// <summary>
        /// Creating story board with that animation.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="control"></param>
        /// <param name="propertyPath"></param>
        /// <param name="duration"></param>
        /// <param name="fillBehavior"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="initHandler"></param>
        /// <returns></returns>
        public static Storyboard StartStoryboard(
           FrameworkElement parent,
           DependencyObject control,
           PropertyPath propertyPath,
           TimeSpan duration,
           FillBehavior fillBehavior,
           GridLength from, GridLength to,
           Action<Storyboard> initHandler)
        {
            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard
            {
                FillBehavior = fillBehavior
            };

            // Create a DoubleAnimation to fade the not selected option control
            GridLengthAnimation animation = new GridLengthAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(duration)
            };

            // Configure the animation to target de property Opacity
            Storyboard.SetTarget(animation, control);
            Storyboard.SetTargetProperty(animation, propertyPath);

            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Inform subscribers
            initHandler?.Invoke(storyboard);

            // Begin the storyboard
            try
            {
                storyboard.Begin(parent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return storyboard;
        }

        /// <summary>
        /// Animates a double value 
        /// </summary>
        public class ExpanderDoubleAnimation : DoubleAnimationBase
        {
            /// <summary>
            /// Dependency property for the From property
            /// </summary>
            public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(double?),
                    typeof(ExpanderDoubleAnimation));

            /// <summary>
            /// CLR Wrapper for the From depenendency property
            /// </summary>
            public double? From
            {
                get
                {
                    return (double?)GetValue(ExpanderDoubleAnimation.FromProperty);
                }
                set
                {
                    SetValue(ExpanderDoubleAnimation.FromProperty, value);
                }
            }

            /// <summary>
            /// Dependency property for the To property
            /// </summary>
            public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(double?),
                    typeof(ExpanderDoubleAnimation));

            /// <summary>
            /// CLR Wrapper for the To property
            /// </summary>
            public double? To
            {
                get
                {
                    return (double?)GetValue(ExpanderDoubleAnimation.ToProperty);
                }
                set
                {
                    SetValue(ExpanderDoubleAnimation.ToProperty, value);
                }
            }

            /// <summary>
            /// Sets the reverse value for the second animation
            /// </summary>
            public double? ReverseValue
            {
                get { return (double)GetValue(ReverseValueProperty); }
                set { SetValue(ReverseValueProperty, value); }
            }

            /// <summary>
            /// Sets the reverse value for the second animation
            /// </summary>
            public static readonly DependencyProperty ReverseValueProperty =
                DependencyProperty.Register("ReverseValue", typeof(double?), typeof(ExpanderDoubleAnimation), new UIPropertyMetadata(0.0));


            /// <summary>
            /// Creates an instance of the animation
            /// </summary>
            /// <returns></returns>
            protected override Freezable CreateInstanceCore()
            {
                return new ExpanderDoubleAnimation();
            }

            /// <summary>
            /// Animates the double value
            /// </summary>
            /// <param name="defaultOriginValue">The original value to animate</param>
            /// <param name="defaultDestinationValue">The final value</param>
            /// <param name="animationClock">The animation clock (timer)</param>
            /// <returns>Returns the new double to set</returns>
            protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
            {
                double fromVal = this.From.Value;
                double toVal = this.To.Value;

                if (defaultOriginValue == toVal)
                {
                    fromVal = toVal;
                    toVal = this.ReverseValue.Value;
                }

                if (fromVal > toVal)
                    return (1 - animationClock.CurrentProgress.Value) *
                        (fromVal - toVal) + toVal;
                else
                    return (animationClock.CurrentProgress.Value *
                        (toVal - fromVal) + fromVal);
            }
        }
    }
}
