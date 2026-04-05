// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public sealed class ConfigurationHelperTests : IDisposable
    {
        private readonly string testProfileName;
        private readonly string profilePath;

        public ConfigurationHelperTests()
        {
            testProfileName = $"decksurf-test-{Guid.NewGuid():N}";
            profilePath = ConfigurationHelper.GetProfilePath(testProfileName);
        }

        public void Dispose()
        {
            // Clean up the test profile directory
            var profileDir = Path.GetDirectoryName(profilePath);
            if (profileDir != null && Directory.Exists(profileDir))
            {
                try
                {
                    Directory.Delete(profileDir, recursive: true);
                }
                catch
                {
                    // Best-effort cleanup
                }
            }
        }

        [Fact]
        public void GetProfilePath_ReturnsNonNullNonEmptyStringContainingProfileName()
        {
            var path = ConfigurationHelper.GetProfilePath(testProfileName);

            Assert.False(string.IsNullOrEmpty(path));
            Assert.Contains(testProfileName, path);
        }

        [Fact]
        public void GetProfilePath_ContainsExpectedDirectoryStructure()
        {
            var path = ConfigurationHelper.GetProfilePath(testProfileName);

            Assert.Contains("Den.Dev", path);
            Assert.Contains("DeckSurf", path);
            Assert.Contains("Profiles", path);
            Assert.EndsWith("profile.json", path);
        }

        [Fact]
        public void GetProfile_ReturnsNull_WhenFileDoesNotExist()
        {
            var result = ConfigurationHelper.GetProfile(testProfileName);

            Assert.Null(result);
        }

        [Fact]
        public void GetProfile_ReturnsNull_WhenFileIsEmpty()
        {
            var dir = Path.GetDirectoryName(profilePath)!;
            Directory.CreateDirectory(dir);
            File.WriteAllText(profilePath, string.Empty);

            var result = ConfigurationHelper.GetProfile(testProfileName);

            Assert.Null(result);
        }

        [Fact]
        public void GetProfile_ReturnsValidConfigurationProfile_WhenFileHasValidJson()
        {
            var dir = Path.GetDirectoryName(profilePath)!;
            Directory.CreateDirectory(dir);

            var profile = new ConfigurationProfile
            {
                DeviceIndex = 2,
                DeviceSerial = "ABC123",
                DeviceModel = DeviceModel.MK2,
            };
            profile.ButtonMap.Add(new CommandMapping
            {
                Plugin = "testplugin",
                Command = "testcommand",
                ButtonIndex = 0,
            });

            File.WriteAllText(profilePath, JsonSerializer.Serialize(profile));

            var result = ConfigurationHelper.GetProfile(testProfileName);

            Assert.NotNull(result);
            Assert.Equal(2, result.DeviceIndex);
            Assert.Equal("ABC123", result.DeviceSerial);
            Assert.Equal(DeviceModel.MK2, result.DeviceModel);
            Assert.Single(result.ButtonMap);
            Assert.Equal("testplugin", result.ButtonMap[0].Plugin);
        }

        [Fact]
        public void GetProfile_ThrowsInvalidOperationException_WhenFileHasCorruptJson()
        {
            var dir = Path.GetDirectoryName(profilePath)!;
            Directory.CreateDirectory(dir);
            File.WriteAllText(profilePath, "{ this is not valid json !!! }}}");

            Assert.Throws<InvalidOperationException>(() => ConfigurationHelper.GetProfile(testProfileName));
        }

        [Fact]
        public void WriteToConfiguration_CreatesFileAndDirectoryStructure()
        {
            var mapping = new CommandMapping
            {
                Plugin = "myplugin",
                Command = "mycommand",
                ButtonIndex = 1,
            };

            ConfigurationHelper.WriteToConfiguration(testProfileName, 0, mapping);

            Assert.True(File.Exists(profilePath));
        }

        [Fact]
        public void WriteToConfiguration_WritesValidJsonThatCanBeDeserialized()
        {
            var mapping = new CommandMapping
            {
                Plugin = "myplugin",
                Command = "mycommand",
                ButtonIndex = 3,
            };

            ConfigurationHelper.WriteToConfiguration(testProfileName, 1, mapping);

            var json = File.ReadAllText(profilePath);
            var deserialized = JsonSerializer.Deserialize<ConfigurationProfile>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(1, deserialized.DeviceIndex);
            Assert.Single(deserialized.ButtonMap);
            Assert.Equal("myplugin", deserialized.ButtonMap[0].Plugin);
            Assert.Equal("mycommand", deserialized.ButtonMap[0].Command);
            Assert.Equal(3, deserialized.ButtonMap[0].ButtonIndex);
        }

        [Fact]
        public void Roundtrip_WriteProfileWithButtonMapEntries_ReadBackAndVerifyDataIntegrity()
        {
            var mapping1 = new CommandMapping
            {
                Plugin = "plugin-alpha",
                Command = "cmd-one",
                CommandArguments = "--verbose",
                ButtonIndex = 0,
                ButtonImagePath = "/tmp/img1.png",
            };

            var mapping2 = new CommandMapping
            {
                Plugin = "plugin-beta",
                Command = "cmd-two",
                CommandArguments = "--quiet",
                ButtonIndex = 5,
                ButtonImagePath = "/tmp/img2.png",
            };

            // Write two mappings sequentially (the method appends to existing file)
            var result1 = ConfigurationHelper.WriteToConfiguration(testProfileName, 3, mapping1);
            var result2 = ConfigurationHelper.WriteToConfiguration(testProfileName, 3, mapping2);

            // Verify the write return values
            Assert.NotNull(result1);
            Assert.Single(result1.ButtonMap);
            Assert.NotNull(result2);
            Assert.Equal(2, result2.ButtonMap.Count);

            // Read back using GetProfile
            var loaded = ConfigurationHelper.GetProfile(testProfileName);

            Assert.NotNull(loaded);
            Assert.Equal(3, loaded.DeviceIndex);
            Assert.Equal(2, loaded.ButtonMap.Count);

            // Verify first mapping
            Assert.Equal("plugin-alpha", loaded.ButtonMap[0].Plugin);
            Assert.Equal("cmd-one", loaded.ButtonMap[0].Command);
            Assert.Equal("--verbose", loaded.ButtonMap[0].CommandArguments);
            Assert.Equal(0, loaded.ButtonMap[0].ButtonIndex);
            Assert.Equal("/tmp/img1.png", loaded.ButtonMap[0].ButtonImagePath);

            // Verify second mapping
            Assert.Equal("plugin-beta", loaded.ButtonMap[1].Plugin);
            Assert.Equal("cmd-two", loaded.ButtonMap[1].Command);
            Assert.Equal("--quiet", loaded.ButtonMap[1].CommandArguments);
            Assert.Equal(5, loaded.ButtonMap[1].ButtonIndex);
            Assert.Equal("/tmp/img2.png", loaded.ButtonMap[1].ButtonImagePath);
        }
    }
}
