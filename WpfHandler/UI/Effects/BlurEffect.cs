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

namespace WpfHandler.UI.Effects
{
    /// <summary>
    /// Provide blur operations.
    /// </summary>
    public static class BlurEffect
    {

        /// <summary>
        /// Turning blur on.
        /// </summary>
        /// <param name="element">bluring element</param>
        /// <param name="blurRadius">blur radius</param>
        /// <param name="duration">blur animation duration</param>
        /// <param name="beginTime">blur animation delay</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <returns>Created animation.</returns>
        public static DoubleAnimation BlurApply(
            UIElement element,
            double blurRadius,
            TimeSpan duration,
            TimeSpan beginTime,
            FillBehavior fillBehavior)
        {
            return BlurApply(
                element,
                blurRadius,
                duration,
                beginTime,
                fillBehavior,
                null);
        }

        /// <summary>
        /// Turning blur on.
        /// </summary>
        /// <param name="element">bluring element</param>
        /// <param name="blurRadius">blur radius</param>
        /// <param name="duration">blur animation duration</param>
        /// <param name="beginTime">blur animation delay</param>
        /// <param name="fillBehavior">
        /// Specifies how a System.Windows.Media.Animation.Timeline behaves when it is outside
        /// its active period but its parent is inside its active or hold period.</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created animation.</returns>
        public static DoubleAnimation BlurApply(
            UIElement element,
            double blurRadius,
            TimeSpan duration, 
            TimeSpan beginTime,
            FillBehavior fillBehavior,
            Action<DoubleAnimation> initHandler)
        {
            // Configuration g animation.
            DoubleAnimation blurEnable = new DoubleAnimation(0, blurRadius, duration)
            {
                BeginTime = beginTime,
                FillBehavior = fillBehavior
            };

            // Applying effect.
            System.Windows.Media.Effects.BlurEffect blur = new System.Windows.Media.Effects.BlurEffect() { Radius = 0 };
            element.Effect = blur;

            // Inform subscribers.
            initHandler?.Invoke(blurEnable);

            // Start animation.
            try
            {
                blur.BeginAnimation(System.Windows.Media.Effects.BlurEffect.RadiusProperty, blurEnable);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return blurEnable;
        }


        /// <summary>
        /// Turning blur off.
        /// </summary>
        /// <param name="element">bluring element</param>
        /// <param name="duration">blur animation duration</param>
        /// <param name="beginTime">blur animation delay</param>
        /// <returns>Created animation.</returns>
        public static DoubleAnimation BlurDisable(
            UIElement element,
            TimeSpan duration,
            TimeSpan beginTime)
        {
            return BlurDisable(
                element,
                duration,
                beginTime,
                null);
        }

        /// <summary>
        /// Turning blur off.
        /// </summary>
        /// <param name="element">bluring element</param>
        /// <param name="duration">blur animation duration</param>
        /// <param name="beginTime">blur animation delay</param>
        /// <param name="initHandler">Handler that would be called before animation start.
        /// There you can subscrube on events or reconfigurate settigns.</param>
        /// <returns>Created animation.</returns>
        public static DoubleAnimation BlurDisable(
            UIElement element,
            TimeSpan duration, 
            TimeSpan beginTime,
            Action<DoubleAnimation> initHandler)
        {
            // Validate requirments.
            if (!(element.Effect is global::System.Windows.Media.Effects.BlurEffect blur) || blur.Radius == 0)
            {
                return null;
            }

            // Configurating animation.
            DoubleAnimation blurDisable = new DoubleAnimation(blur.Radius, 0, duration) { BeginTime = beginTime };
            blurDisable.FillBehavior = FillBehavior.HoldEnd;

            blurDisable.Completed += delegate (object sender, EventArgs e)
            {
                element.Effect = null;
            };

            // Inform subscribers.
            initHandler?.Invoke(blurDisable);

            // Start animation
            try
            {
                blur.BeginAnimation(System.Windows.Media.Effects.BlurEffect.RadiusProperty, blurDisable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return blurDisable;
        }
    }
}
