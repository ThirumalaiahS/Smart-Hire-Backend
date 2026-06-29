namespace Velora.Core.Common
{
    public sealed class CommonEnums
    {
        public enum DataProviderType
        {
            EFCore = 1,
            Dapper = 2,
            AdoNet = 3
        }
        public enum UserRole
        {
            None = 0,
            Admin = 1,
            Recruiter = 2,
            Candidate = 3
        }
    }
}

