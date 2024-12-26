namespace DeckSurf.SDK.Models.Devices
{
    internal class StreamDeckOriginal(int vid, int pid, string path, string name) : ConnectedDevice(vid, pid, path, name)
    {
        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.ORIGINAL;

        /// <inheritdoc/>
        public override int ButtonCount => 15;
    }
}
