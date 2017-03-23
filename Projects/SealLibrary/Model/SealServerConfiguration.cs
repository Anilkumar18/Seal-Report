﻿//
// Copyright (c) Seal Report, Eric Pfirsch (sealreport@gmail.com), http://www.sealreport.org.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. http://www.apache.org/licenses/LICENSE-2.0..
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel;
using Seal.Converter;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.IO;
using Seal.Helpers;
using System.Text.RegularExpressions;
using Seal.Forms;
using DynamicTypeDescriptor;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace Seal.Model
{
    public class SealServerConfiguration : RootComponent
    {
        [XmlIgnore]
        public string FilePath;

        [XmlIgnore]
        public Repository Repository;

        #region Editor

        protected override void UpdateEditorAttributes()
        {
            if (_dctd != null)
            {
                //Disable all properties
                foreach (var property in Properties) property.SetIsBrowsable(false);
                //Then enable
                GetProperty("DefaultConnectionString").SetIsBrowsable(!ForPublication);
                GetProperty("TaskFolderName").SetIsBrowsable(!ForPublication);
                GetProperty("DefaultCulture").SetIsBrowsable(!ForPublication);
                GetProperty("LogoName").SetIsBrowsable(!ForPublication);
                GetProperty("WebProductName").SetIsBrowsable(!ForPublication);
                GetProperty("LogDays").SetIsBrowsable(!ForPublication);
                GetProperty("CsvSeparator").SetIsBrowsable(!ForPublication);
                GetProperty("NumericFormat").SetIsBrowsable(!ForPublication);
                GetProperty("DateTimeFormat").SetIsBrowsable(!ForPublication);
                GetProperty("InitScript").SetIsBrowsable(!ForPublication);
                GetProperty("TasksScript").SetIsBrowsable(!ForPublication);

                GetProperty("WebApplicationPoolName").SetIsBrowsable(ForPublication);
                GetProperty("WebApplicationName").SetIsBrowsable(ForPublication);
                GetProperty("WebPublicationDirectory").SetIsBrowsable(ForPublication);

                TypeDescriptor.Refresh(this);
            }
        }

        #endregion

        [XmlIgnore]
        public bool ForPublication = false;


        string _defaultConnectionString = "Provider=SQLOLEDB;data source=localhost;initial catalog=adb;Integrated Security=SSPI;";
        [Category("Server Settings"), DisplayName("Default Connection String"), Description("The OLE DB Default Connection String used when a new Data Source is created. The string can contain the keyword " + Repository.SealRepositoryKeyword + " to specify the repository root folder."), Id(1, 1)]
        public string DefaultConnectionString
        {
            get { return _defaultConnectionString; }
            set { _defaultConnectionString = value; }
        }

        string _taskFolderName = Repository.SealRootProductName + " Report";
        [Category("Server Settings"), DisplayName("Task Folder Name"), Description("The name of the Task Scheduler folder containg the schedules of the reports. Warning: Changing this name will affect all existing schedules !"), Id(2, 1)]
        public string TaskFolderName
        {
            get { return _taskFolderName; }
            set { _taskFolderName = value; }
        }

        string _logoName = "logo.jpg";
        [Category("Server Settings"), DisplayName("Logo file name"), Description("The logo file name used by the report templates. The file must be located in the Repository folder '<Repository Path>\\Views\\Images'."), Id(5, 1)]
        public string LogoName
        {
            get { return _logoName; }
            set { _logoName = value; }
        }

        int _logDays = 30;
        [Category("Server Settings"), DisplayName("Log days to keep"), Description("Number of days of log files to keep in the repository 'Logs' subfolder. If 0, the log feature is disabled."), Id(6, 1)]
        public int LogDays
        {
            get { return _logDays; }
            set { _logDays = value; }
        }

        string _webProductName = "Seal Report";
        [Category("Server Settings"), DisplayName("Web Product Name"), Description("The name of the product displayed on the Web site."), Id(7, 1)]
        public string WebProductName
        {
            get { return _webProductName; }
            set { _webProductName = value; }
        }



        string _initScript = "";
        [Category("Scripts"), DisplayName("Init Script"), Description("If set, the script is executed when a report is initialized for an execution. Default values for report execution can be set here."), Id(4, 3)]
        [Editor(typeof(TemplateTextEditor), typeof(UITypeEditor))]
        public string InitScript
        {
            get { return _initScript; }
            set { _initScript = value; }
        }

        string _tasksScript = "";
        [Category("Scripts"), DisplayName("Tasks Script"), Description("If set, the script is added to all task scripts executed. This may be useful to defined common functions."), Id(5, 3)]
        [Editor(typeof(TemplateTextEditor), typeof(UITypeEditor))]
        public string TasksScript
        {
            get { return _tasksScript; }
            set { _tasksScript = value; }
        }


        string _defaultCulture = "";
        [Category("Formats"), DisplayName("Culture"), Description("The name of the culture used when a report is created. If not specified, the current culture of the server is used."), Id(1, 2)]
        [TypeConverter(typeof(Seal.Converter.CultureInfoConverter))]
        public string DefaultCulture
        {
            get { return _defaultCulture; }
            set { _defaultCulture = value; }
        }
 
        string _numericFormat = "N0";
        [Category("Formats"), DisplayName("Numeric Format"), Description("The numeric format used for numeric column having the default format"), Id(2, 2)]
        [TypeConverter(typeof(CustomFormatConverter))]
        public string NumericFormat
        {
            get { return _numericFormat; }
            set { _numericFormat = value; }
        }

        string _dateFormat = "d";
        [Category("Formats"), DisplayName("Date Time Format"), Description("The date time format used for date time column having the default format"), Id(3, 2)]
        [TypeConverter(typeof(CustomFormatConverter))]
        public string DateTimeFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }


        string _csvSeparator = "";
        [Category("Formats"), DisplayName("CSV Separator"), Description("If not specified in the report, separator used for the CSV template. If empty, the separator of the user culture is used."), Id(4, 2)]
        public string CsvSeparator
        {
            get { return _csvSeparator; }
            set { _csvSeparator = value; }
        }

        string _webApplicationPoolName = Repository.SealRootProductName + " Application Pool";
        [Category("Web Server IIS Publication"), DisplayName("Application Pool Name"), Description("The name of the IIS Application pool used by the web application."), Id(2, 1)]
        public string WebApplicationPoolName
        {
            get { return _webApplicationPoolName; }
            set { _webApplicationPoolName = value; }
        }

        string _webApplicationName = "/Seal";
        [Category("Web Server IIS Publication"), DisplayName("Application Name"), Description("The name of the IIS Web application."), Id(1, 1)]
        public string WebApplicationName
        {
            get { return _webApplicationName; }
            set { _webApplicationName = value; }
        }

        string _webPublicationDirectory = "";
        [Category("Web Server IIS Publication"), DisplayName("Publication directory"), Description("The directory were the web site files are published."), Id(3, 1)]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string WebPublicationDirectory
        {
            get { return _webPublicationDirectory; }
            set { _webPublicationDirectory = value; }
        }

        //Set by the server manager...
        string _installationDirectory = "";
        public string InstallationDirectory
        {
            get { return _installationDirectory;}
            set {_installationDirectory = value;}
        }

        [XmlIgnore]
        public DateTime LastModification;
        static public SealServerConfiguration LoadFromFile(string path, bool ignoreException)
        {
            SealServerConfiguration result = null;
            try
            {
                StreamReader sr = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(typeof(SealServerConfiguration));
                result = (SealServerConfiguration)serializer.Deserialize(sr);
                sr.Close();
                result.FilePath = path;
                result.LastModification = File.GetLastWriteTime(path);
            }
            catch (Exception ex)
            {
                if (!ignoreException) throw new Exception(string.Format("Unable to read the configuration file '{0}'.\r\n{1}", path, ex.Message));
            }
            return result;
        }


        public void SaveToFile()
        {
            SaveToFile(FilePath);
        }

        public void SaveToFile(string path)
        {
            //Check last modification
            if (LastModification != DateTime.MinValue && File.Exists(path))
            {
                DateTime lastDateTime = File.GetLastWriteTime(path);
                if (LastModification != lastDateTime)
                {
                    throw new Exception("Unable to save the Server Configuration file. The file has been modified by another user.");
                }
            }
            var xmlOverrides = new XmlAttributeOverrides();
            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlIgnore = true;
            xmlOverrides.Add(typeof(RootComponent), "Name", attrs);
            xmlOverrides.Add(typeof(RootComponent), "GUID", attrs);

            //Set installation path, used by, to define schedules
            _installationDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            XmlSerializer serializer = new XmlSerializer(typeof(SealServerConfiguration), xmlOverrides);
            StreamWriter sw = new StreamWriter(path);
            serializer.Serialize(sw, (SealServerConfiguration)this);
            sw.Close();
            FilePath = path;
            LastModification = File.GetLastWriteTime(path);
        }
    }
}
