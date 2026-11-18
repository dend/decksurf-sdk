// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DeckSurf.SDK.Models.Devices
{
    /// <summary>
    /// Implementation for a Stream Deck Plus XL connected device.
    /// </summary>
    /// <remarks>
    /// The Plus XL uses the same USB HID protocol as the Stream Deck Plus (key, screen, touch and
    /// encoder reports), with a larger key grid, a wider touch strip, and six encoders. Unlike the
    /// Plus, every image must be rotated 90 degrees counter-clockwise before upload. Key images are
    /// rotated automatically; screen images passed to <see cref="ConnectedDevice.SetScreen(byte[], int, int, int, int)"/>
    /// must be rotated by the caller (for example with
    /// <see cref="Util.ImageHelper.ResizeImage(byte[], int, int, DeviceRotation, DeviceImageFormat)"/> and
    /// <see cref="ImageRotation"/>), while offsets and sizes stay in unrotated, logical coordinates.
    /// See <see href="https://docs.elgato.com/streamdeck/hid/stream-deck-plus-xl">the Elgato HID reference</see>.
    /// </remarks>
    public class StreamDeckPlusXL(int vid, int pid, string path, string name, string serial) : StreamDeckPlus(vid, pid, path, name, serial)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.PlusXL;

        /// <summary>
        /// Gets the image rotation for the Stream Deck Plus XL: 90 degrees counter-clockwise,
        /// which is <see cref="DeviceRotation.Rotate270"/>.
        /// </summary>
        public override DeviceRotation ImageRotation => DeviceRotation.Rotate270;

        /// <inheritdoc/>
        public override int ButtonCount => 36;

        /// <inheritdoc/>
        public override int KnobCount => 6;

        /// <inheritdoc/>
        public override int ButtonResolution => 112;

        /// <inheritdoc/>
        public override int ButtonColumns => 9;

        /// <inheritdoc/>
        public override int ButtonRows => 4;

        /// <inheritdoc/>
        public override int ScreenWidth => 1200;
    }
}
