﻿//-----------------------------------------------------------------------
// <copyright file="AppxMetadata.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Packaging.SDKUtils.AppxPackaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using Microsoft.Packaging.SDKUtils.AppxPackagingInterop;
    using Windows.Storage.Streams;

    /// <summary>
    /// Class that represents appx package information.
    /// </summary>
    public class AppxMetadata : PackageMetadata
    {
        public AppxMetadata(IRandomAccessStream randomAccessStream)
        {
            if (randomAccessStream == null)
            {
                throw new ArgumentNullException("file is null");
            }

            Guid guid = new Guid("0000000c-0000-0000-C000-000000000046");
            IStream packageStream = StreamUtils.CreateStreamOverRandomAccessStream(randomAccessStream, ref guid);

            IAppxFactory packageFactory = (IAppxFactory)new AppxFactory();
            packageFactory.CreatePackageReader(packageStream);

            this.AppxReader = packageFactory.CreatePackageReader(packageStream);

            this.BlockmapStream = this.AppxReader.GetBlockMap().GetStream();

            IAppxManifestReader appxManifestReader = this.AppxReader.GetManifest();
            this.ManifestStream = appxManifestReader.GetStream();
            IAppxManifestPackageId packageId = appxManifestReader.GetPackageId();
            this.PackageName = packageId.GetName();
            this.PackageFamilyName = packageId.GetPackageFamilyName();
            this.PackageFullName = packageId.GetPackageFullName();
            this.Publisher = packageId.GetPublisher();
            this.Version = new VersionInfo(packageId.GetVersion());

            IAppxManifestProperties packageProperties = appxManifestReader.GetProperties();
            string displayName;
            packageProperties.GetStringValue("DisplayName", out displayName);
            this.DisplayName = displayName;

            string publisherDisplayName;
            packageProperties.GetStringValue("PublisherDisplayName", out publisherDisplayName);
            this.PublisherDisplayName = publisherDisplayName;

            // Get the min versions
            IAppxManifestReader3 appxManifestReader3 = (IAppxManifestReader3)appxManifestReader;
            IAppxManifestTargetDeviceFamiliesEnumerator targetDeviceFamiliesEnumerator = appxManifestReader3.GetTargetDeviceFamilies();
            while (targetDeviceFamiliesEnumerator.GetHasCurrent())
            {
                IAppxManifestTargetDeviceFamily targetDeviceFamily = targetDeviceFamiliesEnumerator.GetCurrent();
                this.TargetDeviceFamiliesMinVersions.Add(
                    targetDeviceFamily.GetName(),
                    new VersionInfo(targetDeviceFamily.GetMinVersion()));

                targetDeviceFamiliesEnumerator.MoveNext();
            }

            this.MinOSVersion = this.TargetDeviceFamiliesMinVersions.OrderBy(t => t.Value).FirstOrDefault().Value;

            this.PopulateCommonFields();
        }


        /// <summary>
        /// Initializes a new instance of the AppxMetadata class for an appx package.
        /// </summary>
        /// <param name="filePath">the path to the appx file</param>
        public AppxMetadata(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("{0} not found", filePath));
            }

            this.FilePath = filePath;
            IStream packageStream = StreamUtils.CreateInputStreamOnFile(this.FilePath);
            IAppxFactory packageFactory = (IAppxFactory)new AppxFactory();
            this.AppxReader = packageFactory.CreatePackageReader(packageStream);

            this.BlockmapStream = this.AppxReader.GetBlockMap().GetStream();

            IAppxManifestReader appxManifestReader = this.AppxReader.GetManifest();
            this.ManifestStream = appxManifestReader.GetStream();
            IAppxManifestPackageId packageId = appxManifestReader.GetPackageId();
            this.PackageName = packageId.GetName();
            this.PackageFamilyName = packageId.GetPackageFamilyName();
            this.PackageFullName = packageId.GetPackageFullName();
            this.Publisher = packageId.GetPublisher();
            this.Version = new VersionInfo(packageId.GetVersion());

            IAppxManifestProperties packageProperties = appxManifestReader.GetProperties();
            string displayName;
            packageProperties.GetStringValue("DisplayName", out displayName);
            this.DisplayName = displayName;

            string publisherDisplayName;
            packageProperties.GetStringValue("PublisherDisplayName", out publisherDisplayName);
            this.PublisherDisplayName = publisherDisplayName;

            // Get the min versions
            IAppxManifestReader3 appxManifestReader3 = (IAppxManifestReader3)appxManifestReader;
            IAppxManifestTargetDeviceFamiliesEnumerator targetDeviceFamiliesEnumerator = appxManifestReader3.GetTargetDeviceFamilies();
            while (targetDeviceFamiliesEnumerator.GetHasCurrent())
            {
                IAppxManifestTargetDeviceFamily targetDeviceFamily = targetDeviceFamiliesEnumerator.GetCurrent();
                this.TargetDeviceFamiliesMinVersions.Add(
                    targetDeviceFamily.GetName(),
                    new VersionInfo(targetDeviceFamily.GetMinVersion()));

                targetDeviceFamiliesEnumerator.MoveNext();
            }

            this.MinOSVersion = this.TargetDeviceFamiliesMinVersions.OrderBy(t => t.Value).FirstOrDefault().Value;

            this.PopulateCommonFields();
        }

        /// <summary>
        /// Gets the display name of the package
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the package publisher display name.
        /// </summary>
        public string PublisherDisplayName { get; private set; }

        /// <summary>
        /// Gets the min OS versions for all the devices the package targets
        /// </summary>
        public Dictionary<string, VersionInfo> TargetDeviceFamiliesMinVersions { get; private set; }
            = new Dictionary<string, VersionInfo>();

        /// <summary>
        /// Gets the min OS version across all the devices this package targets
        /// </summary>
        public VersionInfo MinOSVersion { get; private set; }

        /// <summary>
        /// Gets the reader for this package
        /// </summary>
        public IAppxPackageReader AppxReader { get; private set; }
    }
}
