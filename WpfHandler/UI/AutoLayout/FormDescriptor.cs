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

namespace WpfHandler.UI.AutoLayout
{
    /// <summary>
    /// An abstract UI descriptor suitable for the form-like interfaces.
    /// </summary>
    public abstract class FormDescriptor : UIDescriptor
    {
        /// <summary>
        /// Sturcture that contains a validation report data.
        /// </summary>
        [Serializable]
        public struct ValidationReport
        { 
            /// <summary>
            /// A result of validation.
            /// </summary>
            public bool Result { get; private set; }

            /// <summary>
            /// A report's messasge.
            /// </summary>
            public GUIContent Message { get; private set; }

            /// <summary>
            /// Instiniating the simple report.
            /// </summary>
            /// <param name="result">A result of validation.</param>
            public ValidationReport(bool result)
            {
                Result = result;
                Message = null;
            }

            /// <summary>
            /// Instiniate the report data.
            /// </summary>
            /// <param name="result">A result of validation.</param>
            /// <param name="message">A report's messasge.</param>
            public ValidationReport(bool result, GUIContent message)
            {
                Result = result;
                Message = message;
            }
        }

        /// <summary>
        /// Occurs avter calling the <see cref="OnConfirm"/> handler.
        /// </summary>
        public event Action<FormDescriptor> OnConfirmEvent;

        /// <summary>
        /// Occurs avter calling the <see cref="OnCancel"/> handler.
        /// </summary>
        public event Action<FormDescriptor> OnCancelEvent;

        /// <summary>
        /// Handelr that will be called when a from should validate members data.
        /// </summary>
        /// <returns>
        /// A validation report.
        /// </returns>
        public virtual ValidationReport OnValidation()
        {
            // Mark as valid.
            return new ValidationReport(true);
        }

        /// <summary>
        /// A handler that will called when the form will be confirmed.
        /// Handle the stored data.
        /// </summary>
        public virtual void OnConfirm() { OnConfirmEvent?.Invoke(this); }

        /// <summary>
        /// A handler that will called when the form will be canceled.
        /// Handle the stored data.
        /// </summary>
        public virtual void OnCancel() { OnCancelEvent?.Invoke(this); }
    }
}
