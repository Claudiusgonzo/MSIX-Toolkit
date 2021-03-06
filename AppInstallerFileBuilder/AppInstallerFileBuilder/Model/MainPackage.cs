﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;

namespace AppInstallerFileBuilder.Model
{
    [DataContract(Name = "MainPackage")]
    public class MainPackage
    {
        [DataMember(Name = "Uri")]
        private String _filePath;

        [DataMember(Name = "PackageType")]
        private PackageType _packageType;

        [DataMember(Name = "Version")]
        private String _version;

        [DataMember(Name = "Publisher")]
        private String _publisher;

        [DataMember(Name = "Name")]
        private String _name;

        [DataMember(Name = "ResourceId")]
        private String _resourceId;

        [DataMember(Name = "ProcessorArchitecture")]
        private String _processorArchitecture;

        [DataMember(Name = "FullUriPath")]
        private String _uriPath;


        public MainPackage()
        {
            _filePath = "";
            _version = "";
            _packageType = PackageType.MSIX;
            _publisher = "";
            _name = "";
            _processorArchitecture = "";
            _resourceId = "";
            _uriPath = "";
        }

        public MainPackage(String filePath, String version, String publisher, String name, PackageType packageType, String processorArchitecture, String resourceId, String uriPath)
        {
            _filePath = filePath;
            _version = version;
            _publisher = publisher;
            _packageType = packageType;
            _name = name;
            _processorArchitecture = processorArchitecture;
            _resourceId = resourceId;
            _uriPath = uriPath;
        }

        public String FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
            }
        }

        public String Version
        {
            get { return _version; }
            set
            {
                _version = value;
            }
        }

        public String Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
            }
        }

        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        public String ResourceId
        {
            get { return _resourceId; }
            set
            {
                _resourceId = value;
            }
        }

        public String FullUriPath
        {
            get { return _uriPath; }
            set
            {
                _uriPath = value;
            }
        }

        public PackageType PackageType
        {
            get { return _packageType; }
            set
            {
                _packageType = value;
            }
        }

        public IList<PackageType> PackageTypes
        {
            get
            {
                // Will result in a list like {"Appx", "AppxBundle"}
                return Enum.GetValues(typeof(PackageType)).Cast<PackageType>().ToList<PackageType>();
            }
        }

        public IList<ProcessorArchitecture> ProcessorTypes
        {
            get
            {
                // Will result in a list like {"Arm", "x64"}
                return Enum.GetValues(typeof(ProcessorArchitecture)).Cast<ProcessorArchitecture>().ToList<ProcessorArchitecture>();
            }
        }

        public String ProcessorArchitecture
        {
            get { return _processorArchitecture; }
            set
            {
                _processorArchitecture = value;
            }
        }
    }
}
