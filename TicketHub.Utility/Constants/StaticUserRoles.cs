namespace TicketHub.Utility.Constants;

public static class StaticUserRoles
{
    public const string Admin = "ADMIN";
    public const string Member = "MEMBER";
    public const string Staff = "STAFF";
    public const string Organization = "ORGANIZATION";
    public const string Manager = "MANAGER";

    public const string AdminManagerStaff = "ADMIN,MANAGER,STAFF,ORGANIZATION";
    public const string MemberManager = "MEMBER,MANAGER";
}