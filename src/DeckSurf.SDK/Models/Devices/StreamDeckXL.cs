using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckSurf.SDK.Models.Devices
{
    public class StreamDeckXL : ConnectedDevice
    {
        public StreamDeckXL(int vid, int pid, string path, string name, DeviceModel model) : base(vid, pid, path, name, model)
        {
        }
    }
}
