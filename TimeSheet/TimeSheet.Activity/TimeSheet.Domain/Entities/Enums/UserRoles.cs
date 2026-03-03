namespace TimeSheet.Domain.Entities.Enums
{
    public static class UserRoles
    {
        public const string Admin = nameof(MemberRole.ADMIN);
        public const string Worker = nameof(MemberRole.WORKER);
        public const string AdminOrWorker = "ADMIN,WORKER";
    }
}
