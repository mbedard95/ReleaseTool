namespace ReleaseTool.Models.Enums
{
    [Flags]
    public enum UserPermissions
    {
        ReadRequest = 1,
        CommentRequest = 2,
        WriteRequest = 4,
        ApproveRequest = 8,
        ManageUsers = 16
    }
}
