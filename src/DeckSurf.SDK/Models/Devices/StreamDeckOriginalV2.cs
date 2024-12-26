namespace DeckSurf.SDK.Models.Devices
{
    internal class StreamDeckOriginalV2(int vid, int pid, string path, string name) : ConnectedDevice(vid, pid, path, name)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.ORIGINAL_V2;

        /// <inheritdoc/>
        public override int ButtonCount => 15;
    }
}
