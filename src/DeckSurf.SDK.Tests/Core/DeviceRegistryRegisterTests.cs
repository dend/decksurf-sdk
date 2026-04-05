// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using DeckSurf.SDK.Core;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Models.Devices;

namespace DeckSurf.SDK.Tests.Core
{
    public class DeviceRegistryRegisterTests
    {
        private const int TestVid = 0x0FD9;
        private const string TestPath = "test-path";
        private const string TestName = "Test Device";
        private const string TestSerial = "REG-001";

        [Fact]
        public void Register_CustomFactory_CreateDeviceReturnsCustomType()
        {
            // Use a PID that is not mapped to any built-in model to avoid collisions.
            int customPid = 0xFE;

            // Register a custom factory that returns a StreamDeckMK2 for the custom PID.
            DeviceRegistry.Register(
                (DeviceModel)customPid,
                (vid, pid, path, name, serial) => new StreamDeckMK2(vid, pid, path, name, serial));

            try
            {
                var device = DeviceRegistry.CreateDevice(TestVid, customPid, TestPath, TestName, TestSerial);

                Assert.NotNull(device);
                Assert.IsType<StreamDeckMK2>(device);
                Assert.Equal(TestVid, device.VendorId);
                Assert.Equal(TestPath, device.Path);
                Assert.Equal(TestName, device.Name);
                Assert.Equal(TestSerial, device.Serial);
            }
            finally
            {
                // Clean up: re-register with a factory that won't affect other tests.
                // Since we used a non-standard PID, the worst case is it stays registered
                // but doesn't conflict with real tests.
                DeviceRegistry.Register(
                    (DeviceModel)customPid,
                    (vid, pid, path, name, serial) => null);
            }
        }

        [Fact]
        public void Register_OverwritesExistingFactory()
        {
            int customPid = 0xFD;

            // Register first factory.
            DeviceRegistry.Register(
                (DeviceModel)customPid,
                (vid, pid, path, name, serial) => new StreamDeckMK2(vid, pid, path, name, serial));

            // Register second factory that returns a StreamDeckMini.
            DeviceRegistry.Register(
                (DeviceModel)customPid,
                (vid, pid, path, name, serial) => new StreamDeckMini(vid, pid, path, name, serial));

            try
            {
                var device = DeviceRegistry.CreateDevice(TestVid, customPid, TestPath, TestName, TestSerial);

                Assert.NotNull(device);
                Assert.IsType<StreamDeckMini>(device);
            }
            finally
            {
                DeviceRegistry.Register(
                    (DeviceModel)customPid,
                    (vid, pid, path, name, serial) => null);
            }
        }

        [Fact]
        public void CreateDevice_UsesRegisteredFactory()
        {
            int customPid = 0xFC;
            bool factoryCalled = false;

            DeviceRegistry.Register(
                (DeviceModel)customPid,
                (vid, pid, path, name, serial) =>
                {
                    factoryCalled = true;
                    return new StreamDeckXL(vid, pid, path, name, serial);
                });

            try
            {
                var device = DeviceRegistry.CreateDevice(TestVid, customPid, TestPath, TestName, TestSerial);

                Assert.True(factoryCalled, "The registered factory should have been invoked.");
                Assert.NotNull(device);
                Assert.IsType<StreamDeckXL>(device);
            }
            finally
            {
                DeviceRegistry.Register(
                    (DeviceModel)customPid,
                    (vid, pid, path, name, serial) => null);
            }
        }

        [Fact]
        public void Register_NullFactory_DoesNotThrowOnRegister()
        {
            // The Register method accepts a Func delegate; passing null should either
            // be stored (and fail later on CreateDevice with NullReferenceException)
            // or the method may guard against it. We verify behavior here.
            int customPid = 0xFB;

            // Registering null should not throw at registration time.
            DeviceRegistry.Register((DeviceModel)customPid, null);

            try
            {
                // CreateDevice with a null factory will throw NullReferenceException
                // because the dictionary lookup succeeds but invoking null throws.
                Assert.Throws<NullReferenceException>(() =>
                    DeviceRegistry.CreateDevice(TestVid, customPid, TestPath, TestName, TestSerial));
            }
            finally
            {
                // Clean up by restoring a safe factory.
                DeviceRegistry.Register(
                    (DeviceModel)customPid,
                    (vid, pid, path, name, serial) => null);
            }
        }
    }
}
