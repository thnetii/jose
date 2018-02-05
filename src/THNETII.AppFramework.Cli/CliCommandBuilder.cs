using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace THNETII.AppFramework.Cli
{
    public class CliCommandBuilder
    {

        public Task<int> ExecuteAsync<TCommand>(string[] args,
            CancellationToken cancellationToken = default)
            where TCommand : CliAsyncCommand
        {
            throw new NotImplementedException();
        }

        public int Execute<TCommand>(string[] args)
            where TCommand : CliCommand
        {
            throw new NotImplementedException();
        }
    }
}
