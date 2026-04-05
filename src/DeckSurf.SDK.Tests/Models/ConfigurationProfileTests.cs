// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Models
{
    public class ConfigurationProfileTests
    {
        [Fact]
        public void DefaultConstructor_InitializesButtonMapToEmptyList()
        {
            var profile = new ConfigurationProfile();

            Assert.NotNull(profile.ButtonMap);
            Assert.Empty(profile.ButtonMap);
        }

        [Fact]
        public void DeviceIndex_CanBeSetAndRead()
        {
            var profile = new ConfigurationProfile
            {
                DeviceIndex = 42,
            };

            Assert.Equal(42, profile.DeviceIndex);
        }

        [Fact]
        public void DeviceSerial_CanBeSetAndRead()
        {
            var profile = new ConfigurationProfile
            {
                DeviceSerial = "SN-12345",
            };

            Assert.Equal("SN-12345", profile.DeviceSerial);
        }

        [Fact]
        public void DeviceModel_CanBeSetAndRead()
        {
            var profile = new ConfigurationProfile
            {
                DeviceModel = DeviceModel.XL,
            };

            Assert.Equal(DeviceModel.XL, profile.DeviceModel);
        }

        [Fact]
        public void ButtonMap_CanHaveItemsAdded()
        {
            var profile = new ConfigurationProfile();

            profile.ButtonMap.Add(new CommandMapping
            {
                Plugin = "testplugin",
                Command = "testcommand",
                ButtonIndex = 0,
            });

            Assert.Single(profile.ButtonMap);
            Assert.Equal("testplugin", profile.ButtonMap[0].Plugin);
            Assert.Equal("testcommand", profile.ButtonMap[0].Command);
            Assert.Equal(0, profile.ButtonMap[0].ButtonIndex);
        }

        [Fact]
        public void ButtonMap_CanHaveMultipleItemsAdded()
        {
            var profile = new ConfigurationProfile();

            profile.ButtonMap.Add(new CommandMapping { Plugin = "p1", Command = "c1", ButtonIndex = 0 });
            profile.ButtonMap.Add(new CommandMapping { Plugin = "p2", Command = "c2", ButtonIndex = 1 });
            profile.ButtonMap.Add(new CommandMapping { Plugin = "p3", Command = "c3", ButtonIndex = 2 });

            Assert.Equal(3, profile.ButtonMap.Count);
        }

        [Fact]
        public void JsonSerializationRoundtrip_PreservesAllProperties()
        {
            var original = new ConfigurationProfile
            {
                DeviceIndex = 7,
                DeviceSerial = "XYZ-999",
                DeviceModel = DeviceModel.Plus,
            };

            original.ButtonMap.Add(new CommandMapping
            {
                Plugin = "volume-control",
                Command = "mute-toggle",
                CommandArguments = "--device=speakers",
                ButtonIndex = 3,
                ButtonImagePath = "/images/mute.png",
            });

            original.ButtonMap.Add(new CommandMapping
            {
                Plugin = "obs-integration",
                Command = "scene-switch",
                CommandArguments = "scene=Gaming",
                ButtonIndex = 8,
                ButtonImagePath = "/images/obs.png",
            });

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<ConfigurationProfile>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.DeviceIndex, deserialized.DeviceIndex);
            Assert.Equal(original.DeviceSerial, deserialized.DeviceSerial);
            Assert.Equal(original.DeviceModel, deserialized.DeviceModel);
            Assert.Equal(original.ButtonMap.Count, deserialized.ButtonMap.Count);

            for (int i = 0; i < original.ButtonMap.Count; i++)
            {
                Assert.Equal(original.ButtonMap[i].Plugin, deserialized.ButtonMap[i].Plugin);
                Assert.Equal(original.ButtonMap[i].Command, deserialized.ButtonMap[i].Command);
                Assert.Equal(original.ButtonMap[i].CommandArguments, deserialized.ButtonMap[i].CommandArguments);
                Assert.Equal(original.ButtonMap[i].ButtonIndex, deserialized.ButtonMap[i].ButtonIndex);
                Assert.Equal(original.ButtonMap[i].ButtonImagePath, deserialized.ButtonMap[i].ButtonImagePath);
            }
        }

        [Fact]
        public void JsonSerialization_UsesExpectedPropertyNames()
        {
            var profile = new ConfigurationProfile
            {
                DeviceIndex = 1,
                DeviceSerial = "SN-001",
                DeviceModel = DeviceModel.Mini,
            };

            var json = JsonSerializer.Serialize(profile);

            Assert.Contains("\"device_index\"", json);
            Assert.Contains("\"device_serial\"", json);
            Assert.Contains("\"device_model\"", json);
            Assert.Contains("\"button_map\"", json);
        }
    }
}
