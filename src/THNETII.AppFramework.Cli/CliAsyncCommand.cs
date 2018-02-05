using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace THNETII.AppFramework.Cli
{
    public abstract class CliAsyncCommand : CliCommand
    {
        public abstract Task<int> RunAsync(CancellationToken cancellationToken = default);

        public override int Run() => RunAsync().GetAwaiter().GetResult();
    }
}
