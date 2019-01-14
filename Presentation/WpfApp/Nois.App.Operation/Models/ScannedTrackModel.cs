using Nois.Core.Domain.ScannedLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nois.App.Operation.Models
{
    public class ScannedTrackModel
    {
        public int Index { get; set; }
        public DateTime ScannedTime { get; set; }
        public ScannedLog ScannedLog { get; set; }
    }
}
