﻿//
// Copyright (c) Seal Report (sealreport@gmail.com), http://www.sealreport.org.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. http://www.apache.org/licenses/LICENSE-2.0..
//
using System.Collections.Generic;
using System.ComponentModel;
using Seal.Helpers;
using System.Xml.Serialization;
using System;
#if WINDOWS
using DynamicTypeDescriptor;
using Seal.Forms;
using System.Drawing.Design;
#endif

namespace Seal.Model
{
    /// <summary>
    /// A SecurityLogin defines credentials that may be used by security provider during the authentication
    /// </summary>
    public class SecurityLogin : RootEditor
    {

        static string PasswordSalt = "er*$àqavnd";

#if WINDOWS
        #region Editor
        protected override void UpdateEditorAttributes()
        {
            if (_dctd != null)
            {
                //Disable all properties
                foreach (var property in Properties) property.SetIsBrowsable(false);
                //Then enable
                GetProperty("Id").SetIsBrowsable(true);
                GetProperty("Email").SetIsBrowsable(true);
                GetProperty("Phone").SetIsBrowsable(true);
                GetProperty("HashedPassword").SetIsBrowsable(true);
                GetProperty("GroupNames").SetIsBrowsable(true);
                TypeDescriptor.Refresh(this);
            }
        }
        #endregion

#endif
        /// <summary>
        /// The login identifier (e.g. name, email, etc.).
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("\tIdentifier"), Description("The login identifier (e.g. name, email, etc.)."), Id(1, 1)]
#endif
        public string Id { get; set; }

        /// <summary>
        /// The password hashed
        /// </summary>
        public string Password { get; set; }
        public bool ShouldSerializePassword() { return !string.IsNullOrEmpty(Password); }

        /// <summary>
        /// Password in clear text
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("\tPassword"), PasswordPropertyText(true), Description("Password used to connect to the database."), Id(2, 1)]
        [XmlIgnore]
#endif
        public string HashedPassword
        {
            get
            {
                return "1234567890";
            }
            set
            {
                if (value != "1234567890") Password = CryptoHelper.Hash(value, PasswordSalt);
            }
        }

        /// <summary>
        /// Check the hash code of the passwords
        /// </summary>
        public bool CheckPassword(string password)
        {
            return Password == CryptoHelper.Hash(password??"", PasswordSalt);
        }

        /// <summary>
        /// The login email, may be used for a Two-Factor Authentication
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("Email"), Description("The login email, may be used for a Two-Factor Authentication"), Id(3, 1)]
#endif
        public string Email { get; set; }

        /// <summary>
        /// The login phone number, may be used for a Two-Factor Authentication
        /// </summary>
#if WINDOWS
        [Category("Definition"), DisplayName("Phone Number"), Description("The login phone number, may be used for a Two-Factor Authentication"), Id(4, 1)]
#endif
        public string Phone { get; set; }

        /// <summary>
        /// The security groups of the user.
        /// </summary>
#if WINDOWS
        [Category("Security Groups"), DisplayName("Groups"), Description("The security groups of the user."), Id(1, 2)]
        [Editor(typeof(StringListEditor), typeof(UITypeEditor))]
#endif
        public List<string> GroupNames { get; set; } = new List<string>();
        public bool ShouldSerializeGroups() { return GroupNames.Count > 0; }
    }
}

