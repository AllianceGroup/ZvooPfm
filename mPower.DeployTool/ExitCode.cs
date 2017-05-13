using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.DeployTool
{
    public enum ExitCode
    {
        Success = 0,
        ErrorRegenerateReadModel = 1,
        ErrorCreateBackup = 2,
        ErrorPublish = 3,
        InvalidArgument = 4,
        UnexpectedError = 5
    }
}
