﻿using System.Collections.Generic;
using System.Text;
using Fixie.Execution;

namespace Fixie.Tests
{
    public class StubListener : Listener
    {
        readonly List<string> log = new List<string>();

        public void Handle(AssemblyInfo message)
        {
        }

        public void Handle(SkipResult message)
        {
            var optionalReason = message.SkipReason == null ? null : ": " + message.SkipReason;
            log.Add($"{message.Name} skipped{optionalReason}");
        }

        public void Handle(PassResult message)
        {
            log.Add($"{message.Name} passed");
        }

        public void Handle(FailResult message)
        {
            var entry = new StringBuilder();

            var primaryException = message.Exceptions.PrimaryException;

            entry.AppendFormat("{0} failed: {1}", message.Name, primaryException.Message);

            var walk = primaryException;
            while (walk.InnerException != null)
            {
                walk = walk.InnerException;
                entry.AppendLine();
                entry.AppendFormat("    Inner Exception: {0}", walk.Message);
            }

            foreach (var secondaryException in message.Exceptions.SecondaryExceptions)
            {
                entry.AppendLine();
                entry.AppendFormat("    Secondary Failure: {0}", secondaryException.Message);

                walk = secondaryException;
                while (walk.InnerException != null)
                {
                    walk = walk.InnerException;
                    entry.AppendLine();
                    entry.AppendFormat("        Inner Exception: {0}", walk.Message);
                }
            }

            log.Add(entry.ToString());
        }

        public void Handle(AssemblyCompleted message)
        {
        }

        public IEnumerable<string> Entries
        {
            get { return log; }
        }
    }
}