using System.Reflection;

namespace Vero.Shared
{
    public sealed class AssembliesContainer
    {
        public readonly Assembly? Application;
        public readonly Assembly? Contract;
        public readonly Assembly? Domain;
        public readonly Assembly? Infrastructure;
        public readonly Assembly? Shared;

        public AssembliesContainer(Assembly? infrastructure)
        {
            Infrastructure = infrastructure;

            var infrastructurePath = infrastructure?.Location ?? string.Empty;
            var applicationPath = infrastructurePath.Replace("Infrastructure.dll", "Application.dll");
            Application = File.Exists(applicationPath) ? Assembly.LoadFrom(applicationPath) : null;

            var domainPath = infrastructurePath.Replace("Infrastructure.dll", "Domain.dll");
            Domain = File.Exists(domainPath) ? Assembly.LoadFrom(domainPath) : null;

            var contractsPath = infrastructurePath.Replace("Infrastructure.dll", "Contracts.dll");
            Contract = File.Exists(contractsPath) ? Assembly.LoadFrom(contractsPath) : null;

            var sharedPath = Path.Combine(Path.GetDirectoryName(infrastructurePath)!, "Shared.dll");
            Shared = File.Exists(sharedPath) ? Assembly.LoadFrom(sharedPath) : null;
        }
    }
}