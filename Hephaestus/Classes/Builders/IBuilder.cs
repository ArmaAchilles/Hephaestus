using System.Diagnostics;
using Hephaestus.Common.Classes;

namespace Hephaestus.Classes.Builders
{
    public interface IBuilder
    {
        Process Process { get; }

        void Build(string sourceCodeDirectory, Project project, ProcessStartInfo startInfo);
    }
}