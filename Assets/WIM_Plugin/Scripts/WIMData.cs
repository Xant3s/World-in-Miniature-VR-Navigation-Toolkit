using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WIM_Plugin {
    // Data describing the current WIM state. Data only. Modified at runtime.
    internal interface WIMData {
        bool InNewDestination { get; set; }

        // TODO: Add rest
    }

    internal class WIMDataImpl : WIMData {
        public bool InNewDestination { get; set; }
    }
}