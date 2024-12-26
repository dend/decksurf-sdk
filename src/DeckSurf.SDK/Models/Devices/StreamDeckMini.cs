namespace DeckSurf.SDK.Models.Devices
{
    internal class StreamDeckMini(int vid, int pid, string path, string name) : ConnectedDevice(vid, pid, path, name)
    {

        /// <inheritdoc/>
        public override DeviceModel Model => DeviceModel.MINI;

        /// <inheritdoc/>
        public override int ButtonCount => 6;
    }
}
