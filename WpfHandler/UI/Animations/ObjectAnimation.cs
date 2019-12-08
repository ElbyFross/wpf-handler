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
    /// Profides API for managinf objects animations.
    /// </summary>
    public static class ObjectAnimation
    {
        /// <summary>
        /// Start object animation.
        /// </summary>
        /// <param name="parent">Object that contains property.</param>
        /// <param name="propertyName">A name of the property.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time would take transit.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Finish value.</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created storyboard.</returns>
        public static Storyboard StartStoryboard(
            FrameworkElement parent,
            string propertyName,
            PropertyPath propertyPath,
            TimeSpan duration,
            object from,
            object to,
            FillBehavior fillBehavior,
            Action<Storyboard> initHandler)
        {
            return StartStoryboard(
                parent,
                propertyName,
                null,
                propertyPath,
                duration, from,
                to,
                fillBehavior,
                initHandler);
        }

        /// <summary>
        /// Start object animation.
        /// </summary>
        /// <param name="parent">Object that contains property.</param>
        /// <param name="obj">object that will animated.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time would take transit.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Finish value.</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created storyboard.</returns>
        public static Storyboard StartStoryboard(
            FrameworkElement parent,
            DependencyObject obj,
            PropertyPath propertyPath,
            TimeSpan duration,
            object from,
            object to,
            FillBehavior fillBehavior,
            Action<Storyboard> initHandler)
        {
            return StartStoryboard(
                parent,
                null,
                obj,
                propertyPath,
                duration, from,
                to,
                fillBehavior,
                initHandler);
        }

        /// <summary>
        /// Start object animation.
        /// </summary>
        /// <param name="parent">Object that contains property.</param>
        /// <param name="propertyName">A name of the property.</param>
        /// <param name="obj">Object that will animated. Using in case if @propertyName is null.</param>
        /// <param name="propertyPath">A path that describe the dependency property to be animated.</param>
        /// <param name="duration">How many time would take transit.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Finish value.</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created storyboard.</returns>
        private static Storyboard StartStoryboard(
            FrameworkElement parent,
            string propertyName,
            DependencyObject obj,
            PropertyPath propertyPath,
            TimeSpan duration,
            object from,
            object to,
            FillBehavior fillBehavior,
            Action<Storyboard> initHandler)
        {
            // Create a storyboard to contains the animations.
            Storyboard storyboard = new Storyboard
            {
                FillBehavior = fillBehavior
            };

            // Add the animation to the storyboard
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
            storyboard.Children.Add(animation);
            animation.Duration = new Duration(duration);
            animation.AccelerationRatio = 1.0f;

            // Set start position.
            DiscreteObjectKeyFrame startKey = new DiscreteObjectKeyFrame(
                from,
                KeyTime.FromPercent(0));

            // Set finish position.
            DiscreteObjectKeyFrame finishKey = new DiscreteObjectKeyFrame(
                to,
                KeyTime.FromPercent(1));

            // Configure the animation to target de property Opacity

            if (string.IsNullOrEmpty(propertyName))
            {
                Storyboard.SetTarget(animation, obj);
            }
            else
            {
                Storyboard.SetTargetName(animation, propertyName);
            }
            Storyboard.SetTargetProperty(animation, propertyPath);


            // Add keys.
            animation.KeyFrames.Add(startKey);
            //animation.KeyFrames.Add(middleKey);
            animation.KeyFrames.Add(finishKey);

            // Inform subscribers.
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
