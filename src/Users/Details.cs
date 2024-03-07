namespace UsersApp
{
    public enum DbSource
    {
        PROD,
        TEST
    }

    public static class UsersError
    {
        public const string InvalidEmailFormat = "Invalid email format";
        public const string NoUserWithSuchId = "No user with such userId";
    }
}
