// Copyright (c) Den Delimarsky
// Den Delimarsky licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DeckSurf.SDK.Core;
using DeckSurf.SDK.Interfaces;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Tests.Core
{
    public sealed class PluginLoaderTests : IDisposable
    {
        private readonly string testDirectory;

        public PluginLoaderTests()
        {
            testDirectory = Path.Combine(Path.GetTempPath(), $"decksurf-loader-{Guid.NewGuid():N}");
            Directory.CreateDirectory(testDirectory);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(testDirectory, recursive: true);
            }
            catch
            {
                // Best-effort cleanup
            }
        }

        [Fact]
        public void LoadPlugins_ReturnsEmpty_ForDirectoryWithoutPlugins()
        {
            var plugins = PluginLoader.LoadPlugins(testDirectory);

            Assert.Empty(plugins);
        }

        [Fact]
        public void LoadPlugins_ReturnsEmpty_ForMissingDirectory()
        {
            var plugins = PluginLoader.LoadPlugins(Path.Combine(testDirectory, "does-not-exist"));

            Assert.Empty(plugins);
        }

        [Fact]
        public void LoadPlugins_IgnoresNonPluginAssemblies_AndWarnsOnBadImages()
        {
            // Matches the naming convention but is not a valid assembly.
            File.WriteAllText(Path.Combine(testDirectory, "DeckSurf.Plugin.Broken.dll"), "not a real assembly");

            // Valid name pattern violation — never touched.
            File.WriteAllText(Path.Combine(testDirectory, "SomethingElse.dll"), "irrelevant");

            var warnings = new List<string>();
            var plugins = PluginLoader.LoadPlugins(testDirectory, warnings.Add);

            Assert.Empty(plugins);
            var warning = Assert.Single(warnings);
            Assert.Contains("DeckSurf.Plugin.Broken.dll", warning);
        }

        [Fact]
        public void LoadPlugins_ScansPluginsSubdirectoryRecursively()
        {
            var nested = Path.Combine(testDirectory, "plugins", "DeckSurf.Plugin.Broken");
            Directory.CreateDirectory(nested);
            File.WriteAllText(Path.Combine(nested, "DeckSurf.Plugin.Broken.dll"), "not a real assembly");

            var warnings = new List<string>();
            PluginLoader.LoadPlugins(testDirectory, warnings.Add);

            // The nested bad assembly was found (and warned about), proving the recursive scan.
            Assert.Single(warnings);
        }

        [Fact]
        public void LoadPlugins_ScansBaseDirectoryRecursively_OnlyWhenRequested()
        {
            var nested = Path.Combine(testDirectory, "DeckSurf.Plugin.Broken");
            Directory.CreateDirectory(nested);
            File.WriteAllText(Path.Combine(nested, "DeckSurf.Plugin.Broken.dll"), "not a real assembly");

            var defaultWarnings = new List<string>();
            PluginLoader.LoadPlugins(testDirectory, defaultWarnings.Add);

            var recursiveWarnings = new List<string>();
            PluginLoader.LoadPlugins(testDirectory, recursiveWarnings.Add, scanBaseRecursively: true);

            Assert.Empty(defaultWarnings);
            Assert.Single(recursiveWarnings);
        }

        [Fact]
        public void LoadPlugins_ThrowsArgumentException_ForInvalidBaseDirectory()
        {
            Assert.Throws<ArgumentException>(() => PluginLoader.LoadPlugins(null!));
            Assert.Throws<ArgumentException>(() => PluginLoader.LoadPlugins("   "));
        }

        [Fact]
        public void GetCommandTypes_ReturnsAllCommands_WithoutModelFilter()
        {
            var types = PluginLoader.GetCommandTypes(new FakePlugin());

            Assert.Equal(2, types.Count);
        }

        [Fact]
        public void GetCommandTypes_FiltersByCompatibleModel()
        {
            var xlTypes = PluginLoader.GetCommandTypes(new FakePlugin(), DeviceModel.XL);
            var miniTypes = PluginLoader.GetCommandTypes(new FakePlugin(), DeviceModel.Mini);

            Assert.Equal(2, xlTypes.Count);
            var miniType = Assert.Single(miniTypes);
            Assert.Equal(typeof(EverywhereCommand), miniType);
        }

        [Fact]
        public void GetCommandTypes_TreatsUnannotatedCommandsAsCompatibleWithEveryModel()
        {
            var types = PluginLoader.GetCommandTypes(new UnannotatedPlugin(), DeviceModel.Mini);

            var type = Assert.Single(types);
            Assert.Equal(typeof(UnannotatedCommand), type);
        }

        [Fact]
        public void GetCommandTypes_ReturnsEmpty_ForPluginWithNullCommands()
        {
            var types = PluginLoader.GetCommandTypes(new NullCommandsPlugin());

            Assert.Empty(types);
        }

        [Fact]
        public void LoadCompatibleCommands_InstantiatesOnlyCompatibleCommands()
        {
            var commands = PluginLoader.LoadCompatibleCommands(new FakePlugin(), DeviceModel.Mini);

            var command = Assert.Single(commands);
            Assert.IsType<EverywhereCommand>(command);
        }

        [Fact]
        public void LoadCompatibleCommands_WarnsForCommandsThatFailToInstantiate()
        {
            var warnings = new List<string>();
            var commands = PluginLoader.LoadCompatibleCommands(new ThrowingPlugin(), DeviceModel.XL, warnings.Add);

            Assert.Empty(commands);
            var warning = Assert.Single(warnings);
            Assert.Contains(nameof(ThrowingCommand), warning);
        }

        [Fact]
        public void LoadCompatibleCommands_ThrowsArgumentNullException_ForNullPlugin()
        {
            Assert.Throws<ArgumentNullException>(() => PluginLoader.LoadCompatibleCommands(null!, DeviceModel.XL));
            Assert.Throws<ArgumentNullException>(() => PluginLoader.GetCommandTypes(null!));
        }

        [Fact]
        public void GetCommandIconPath_ReturnsNull_ForCommandWithoutIcon()
        {
            Assert.Null(PluginLoader.GetCommandIconPath(typeof(UnannotatedCommand)));
        }

        [Fact]
        public void GetCommandIconPath_ReturnsNull_WhenDeclaredIconFileIsMissing()
        {
            Assert.Null(PluginLoader.GetCommandIconPath(typeof(MissingIconCommand)));
        }

        [Fact]
        public void GetCommandIconPath_ResolvesRelativeToAssemblyDirectory()
        {
            // The attribute path is resolved against this test assembly's directory,
            // so create the icon file there for the duration of the test.
            var assemblyDirectory = Path.GetDirectoryName(typeof(PluginLoaderTests).Assembly.Location)!;
            var iconPath = Path.Combine(assemblyDirectory, "icons", "present.png");
            Directory.CreateDirectory(Path.GetDirectoryName(iconPath)!);
            File.WriteAllBytes(iconPath, [1, 2, 3]);

            try
            {
                var resolved = PluginLoader.GetCommandIconPath(typeof(PresentIconCommand));

                Assert.Equal(iconPath, resolved);
            }
            finally
            {
                File.Delete(iconPath);
            }
        }

        [Fact]
        public void GetCommandIconPath_ThrowsArgumentNullException_ForNullType()
        {
            Assert.Throws<ArgumentNullException>(() => PluginLoader.GetCommandIconPath(null!));
        }

        private sealed class FakePlugin : IDeckSurfPlugin
        {
            public PluginMetadata Metadata => new() { Id = "DeckSurf.Plugin.Fake", Version = "1.0.0", Author = "Test" };

            public List<Type> GetSupportedCommands() => [typeof(EverywhereCommand), typeof(XlOnlyCommand)];
        }

        private sealed class NullCommandsPlugin : IDeckSurfPlugin
        {
            public PluginMetadata Metadata => new() { Id = "DeckSurf.Plugin.Null", Version = "1.0.0", Author = "Test" };

            public List<Type> GetSupportedCommands() => null!;
        }

        private sealed class ThrowingPlugin : IDeckSurfPlugin
        {
            public PluginMetadata Metadata => new() { Id = "DeckSurf.Plugin.Throwing", Version = "1.0.0", Author = "Test" };

            public List<Type> GetSupportedCommands() => [typeof(ThrowingCommand)];
        }

        private sealed class UnannotatedPlugin : IDeckSurfPlugin
        {
            public PluginMetadata Metadata => new() { Id = "DeckSurf.Plugin.Unannotated", Version = "1.0.0", Author = "Test" };

            public List<Type> GetSupportedCommands() => [typeof(UnannotatedCommand)];
        }

        private sealed class UnannotatedCommand : TestCommand
        {
        }

        [CommandIcon("icons/does-not-exist.png")]
        private sealed class MissingIconCommand : TestCommand
        {
        }

        [CommandIcon("icons/present.png")]
        private sealed class PresentIconCommand : TestCommand
        {
        }

        [CompatibleWith(DeviceModel.XL)]
        [CompatibleWith(DeviceModel.Mini)]
        private sealed class EverywhereCommand : TestCommand
        {
        }

        [CompatibleWith(DeviceModel.XL)]
        private sealed class XlOnlyCommand : TestCommand
        {
        }

        [CompatibleWith(DeviceModel.XL)]
        private sealed class ThrowingCommand : TestCommand
        {
            public ThrowingCommand()
            {
                throw new InvalidOperationException("Cannot construct.");
            }
        }

        private abstract class TestCommand : IDeckSurfCommand
        {
            public string Name => "Test";

            public string Description => "Test command.";

            public void ExecuteOnActivation(CommandMapping mappedCommand, IConnectedDevice mappedDevice)
            {
            }

            public void ExecuteOnAction(CommandMapping mappedCommand, IConnectedDevice mappedDevice, int activatingButton = -1)
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
