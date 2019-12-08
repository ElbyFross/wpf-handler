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
    /// Provide base float animations operations.
    /// </summary>
    public static class FloatAnimation
    {
        /// <summary>
        /// Start float animation.
        /// </summary>
        /// <param name="parent">Object that contains property.</param>
        /// <param name="controlName">A name of the UI Elemnt.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time would take transit.</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Finish value.</param>
        /// <returns>Created storyboard.</returns>
        public static Storyboard StartStoryboard(
            FrameworkElement parent,
            string controlName,
            PropertyPath propertyPath,
            TimeSpan duration,
            FillBehavior fillBehavior,
            float from, float to)
        {
            return StartStoryboard(
               parent,
               controlName,
               propertyPath,
               duration,
               fillBehavior,
               from, to,
               null);
        }

        /// <summary>
        /// Start float animation.
        /// </summary>
        /// <param name="parent">Object that contains property.</param>
        /// <param name="controlName">A name of the UI element.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time would take transit.</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Finish value.</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created storyboard.</returns>
        public static Storyboard StartStoryboard(
            FrameworkElement parent, 
            string controlName, 
            PropertyPath propertyPath,
            TimeSpan duration,
            FillBehavior fillBehavior,
            float from, float to,
            Action<Storyboard> initHandler)
        {
            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard
            {
                FillBehavior = fillBehavior
            };

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(duration),
                AutoReverse = false
            };

            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, controlName);
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return storyboard;
        }


        /// <summary>
        /// Start float animation.
        /// </summary>
        /// <param name="parent">Object that contains target control.</param>
        /// <param name="control">UI element that will be animated.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time would take transit.</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Finish value.</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created storyboard.</returns>
        public static Storyboard StartStoryboard(
            FrameworkElement parent,
            DependencyObject control,
            PropertyPath propertyPath,
            TimeSpan duration,
            FillBehavior fillBehavior,
            float from, float to,
            Action<Storyboard> initHandler)
        {
            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard
            {
                FillBehavior = fillBehavior
            };

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation
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
    }
}
