// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Models;
using DeckSurf.SDK.Util;

namespace DeckSurf.SDK.Tests.Helpers
{
    public sealed class ConfigurationHelperProfileManagementTests : IDisposable
    {
        private readonly string testProfileName;

        public ConfigurationHelperProfileManagementTests()
        {
            testProfileName = $"decksurf-test-{Guid.NewGuid():N}";
        }

        public void Dispose()
        {
            try
            {
                ConfigurationHelper.DeleteProfile(testProfileName);
            }
            catch
            {
                // Best-effort cleanup
            }
        }

        [Fact]
        public void GetProfilesRootPath_IsParentOfProfilePaths()
        {
            var rootPath = ConfigurationHelper.GetProfilesRootPath();
            var profilePath = ConfigurationHelper.GetProfilePath(testProfileName);

            Assert.StartsWith(rootPath, profilePath);
            Assert.EndsWith("Profiles", rootPath);
        }

        [Fact]
        public void SaveProfile_RoundTripsThroughGetProfile()
        {
            var profile = new ConfigurationProfile
            {
                DeviceIndex = 1,
                DeviceSerial = "XYZ789",
                DeviceModel = DeviceModel.XL,
            };
            profile.ButtonMap.Add(new CommandMapping
            {
                Plugin = "DeckSurf.Plugin.Barn",
                Command = "SnakeGame",
                ButtonIndex = -1,
            });

            ConfigurationHelper.SaveProfile(testProfileName, profile);
            var loaded = ConfigurationHelper.GetProfile(testProfileName);

            Assert.NotNull(loaded);
            Assert.Equal(1, loaded.DeviceIndex);
            Assert.Equal("XYZ789", loaded.DeviceSerial);
            Assert.Equal(DeviceModel.XL, loaded.DeviceModel);
            var mapping = Assert.Single(loaded.ButtonMap);
            Assert.Equal(-1, mapping.ButtonIndex);
        }

        [Fact]
        public void SaveProfile_ReplacesExistingProfile()
        {
            var first = new ConfigurationProfile { DeviceIndex = 0 };
            first.ButtonMap.Add(new CommandMapping { Plugin = "a", Command = "b", ButtonIndex = 0 });
            first.ButtonMap.Add(new CommandMapping { Plugin = "c", Command = "d", ButtonIndex = 1 });

            var second = new ConfigurationProfile { DeviceIndex = 2 };
            second.ButtonMap.Add(new CommandMapping { Plugin = "e", Command = "f", ButtonIndex = 5 });

            ConfigurationHelper.SaveProfile(testProfileName, first);
            ConfigurationHelper.SaveProfile(testProfileName, second);

            var loaded = ConfigurationHelper.GetProfile(testProfileName);

            Assert.NotNull(loaded);
            Assert.Equal(2, loaded.DeviceIndex);
            var mapping = Assert.Single(loaded.ButtonMap);
            Assert.Equal("e", mapping.Plugin);
        }

        [Fact]
        public void ListProfiles_IncludesSavedProfile()
        {
            ConfigurationHelper.SaveProfile(testProfileName, new ConfigurationProfile());

            var profiles = ConfigurationHelper.ListProfiles();

            Assert.Contains(testProfileName, profiles);
        }

        [Fact]
        public void ListProfiles_ExcludesDirectoriesWithoutProfileFile()
        {
            var emptyDirectory = Path.Combine(ConfigurationHelper.GetProfilesRootPath(), testProfileName);
            Directory.CreateDirectory(emptyDirectory);

            var profiles = ConfigurationHelper.ListProfiles();

            Assert.DoesNotContain(testProfileName, profiles);
        }

        [Fact]
        public void DeleteProfile_RemovesSavedProfile()
        {
            ConfigurationHelper.SaveProfile(testProfileName, new ConfigurationProfile());

            var deleted = ConfigurationHelper.DeleteProfile(testProfileName);

            Assert.True(deleted);
            Assert.Null(ConfigurationHelper.GetProfile(testProfileName));
            Assert.DoesNotContain(testProfileName, ConfigurationHelper.ListProfiles());
        }

        [Fact]
        public void DeleteProfile_ReturnsFalse_WhenProfileDoesNotExist()
        {
            var deleted = ConfigurationHelper.DeleteProfile(testProfileName);

            Assert.False(deleted);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("..")]
        [InlineData("bad|name")]
        public void SaveProfile_ThrowsArgumentException_ForInvalidNames(string name)
        {
            Assert.Throws<ArgumentException>(() => ConfigurationHelper.SaveProfile(name, new ConfigurationProfile()));
            Assert.Throws<ArgumentException>(() => ConfigurationHelper.DeleteProfile(name));
        }

        [Fact]
        public void SaveProfile_ThrowsArgumentNullException_ForNullProfile()
        {
            Assert.Throws<ArgumentNullException>(() => ConfigurationHelper.SaveProfile(testProfileName, null!));
        }
    }
}
