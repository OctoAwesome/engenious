using System;

namespace ContentTool.old.Builder
{
    [Flags()]
    public enum BuildStep
    {
        Clean=1,
        Build=2,
        Abort=4,
        Finished=8,
        Built=16
    }
}

