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
using WpfHandler.UI.AutoLayout;
using WpfHandler.UI.AutoLayout.Controls;
using WpfHandler.UI.AutoLayout.Configuration;
using WpfHandler.UI.AutoLayout.Options;

namespace WpfHandler.UI.Controls.Logon
{
    /// <summary>
    /// A desciptor used as default form for the refisration panel.
    /// </summary>
    [System.Serializable]
    public class DefaultRegistrationPanelDescriptor : FormDescriptor
    {
        /// <summary>
        /// A login for user profile.
        /// </summary>
        [Content("Login *", null, "regForm_login")]
        public string Login { get; set; }

        /// <summary>
        /// A user's password.
        /// </summary>
        [CustomControl(typeof(FlatPasswordBox))]
        [Content("Password *", null, "regForm_pass")]
        public string Password { get; set; }

        /// <summary>
        /// An UI field that should contain the same password like in the <see cref="Password"/> one.
        /// </summary>
        [CustomControl(typeof(FlatPasswordBox))]
        [Content("Repeat passord *", null, "regForm_passR")]
        public string PasswordConfirmation { get; set; }

        /// <summary>
        /// A first name of an user.
        /// </summary>
        [Content("First name *", null, "regForm_fn")]
        public string FirstName { get; set; }

        /// <summary>
        /// A middle name of an user.
        /// </summary>
        [Content("Middle name", null, "regForm_mn")]
        public string MiddleName { get; set; }

        /// <summary>
        /// A last and of an user.
        /// </summary>
        [Content("Last name *", null, "regForm_ln")]
        public string LastName { get; set; }

        /// <summary>
        /// Check if passwords is the same and not null.
        /// </summary>
        [HideInInspector]
        public bool IsPasswordsTheSame
        {
            get
            {
                // Drop if fields is empty.
                if (string.IsNullOrEmpty(Password) ||
                    string.IsNullOrEmpty(PasswordConfirmation))
                {
                    return false;
                }

                // Comparing the firelds.
                return string.Equals(Password, PasswordConfirmation);
            }
        }


        /// <summary>
        /// Handelr that will be called when a from should validate members data.
        /// </summary>
        /// <returns>
        /// A validation report.
        /// </returns>
        public override ValidationReport OnValidation()
        {
            // Drop if not all fields filled.
            if(string.IsNullOrEmpty(Login) ||
               string.IsNullOrEmpty(Password) ||
               string.IsNullOrEmpty(PasswordConfirmation) ||
               string.IsNullOrEmpty(FirstName) ||
               string.IsNullOrEmpty(LastName))
            {
                return new ValidationReport(
                    false,
                    new GUIContent(
                        "Fill all fields with *",
                        null, "regPanel_faf_error"));
            }

            // Drop if the passwords not match.
            if(!IsPasswordsTheSame)
            {
                return new ValidationReport(
                    false,
                    new GUIContent(
                        "The passwords not match.",
                        null, "regPanel_pnm_error"));
            }

            return new ValidationReport(true);
        }

        /// <summary>
        /// A handler that will called when the form will be confirmed.
        /// Handle the stored data.
        /// </summary>
        public override void OnConfirm()
        {
            base.OnConfirm();
        }

        /// <summary>
        /// A handler that will called when the form will be canceled.
        /// Handle the stored data.
        /// </summary>
        public override void OnCancel()
        {
            base.OnCancel();
        }
    }
}
