using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using UsersApp;
using Users.Models;

namespace UsersTests
{
    public class UsersAppTests
    {
        private ApplicationContext dbContext;
        private UsersDb usersDb;

        private const string userName = "spolinaa";
        private const string email = "spolinaa@test.ru";
        private const string invalidEmail = "wrong format email";
        private const int userId = 1;
        private const int nonExistentId = 156;

        private Func<int, string> userNameFun = x => $"user{x}";
        private Func<int, string> userEmailFun = x => $"user{x}@mail.ru";

        [SetUp]
        public void Setup()
        {
            dbContext = new ApplicationContext();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            usersDb = new UsersDb(dbContext);
        }

        public void CheckGet(IResult expected)
        {
            var actual = usersDb.Get();
            CheckEquals(actual, expected);
        }

        public void CheckGet(int userId, IResult expected)
        {
            var actual = usersDb.Get(userId);
            CheckEquals(actual, expected);
        }

        [Test]
        public void AddAndGetUserTest()
        {
            var actual = usersDb.Add(userName, email);
            CheckEquals(actual, Results.Ok());

            var user = new User { Id = userId, Name = userName, Email = email };
            CheckGet(userId, Results.Ok(user));
        }

        [Test]
        public void AddUserWithInvalidEmailTest()
        {
            var actual = usersDb.Add(userName, invalidEmail);
            CheckEquals(actual, Results.BadRequest(UsersError.InvalidEmailFormat));
        }

        [Test]
        public void AddAndGetMultipleUsersTest()
        {
            var users = Enumerable
                .Range(0, 5)
                .Select(x => new User { Id = x + 1, Name = userNameFun(x), Email = userEmailFun(x) })
                .ToList();

            AddUsers();
            CheckGet(Results.Ok(users));
        }

        [Test]
        public void GetMultipleUsersInEmptyDbTest()
        {
            CheckGet(Results.Ok(new List<User>()));
        }

        [Test]
        public void GetUserWithNonExistentId()
        {
            CheckGet(userId, Results.NotFound(UsersError.NoUserWithSuchId));
            AddUsers();
            CheckGet(nonExistentId, Results.NotFound(UsersError.NoUserWithSuchId));
        }

        [Test]
        public void UpdateAndGetUser()
        {
            AddUser();

            var newName = "test_name";
            var updateResult = usersDb.Update(userId, newName, email);
            Assert.That(updateResult, Is.EqualTo(Results.Ok()));

            var user = new User { Id = userId, Name = newName, Email = email };
            CheckGet(userId, Results.Ok(user));
        }

        [Test]
        public void UpdateUserWithInvalidEmail()
        {
            AddUser();

            var expected = Results.BadRequest(UsersError.InvalidEmailFormat);
            var actual = usersDb.Update(userId, userName, invalidEmail);
            CheckEquals(actual, expected);
        }

        [Test]
        public void UpdateUserWithNonExistentId()
        {
            var actual = usersDb.Update(userId, userName, email);
            CheckEquals(actual, Results.NotFound(UsersError.NoUserWithSuchId));

            AddUser();

            actual = usersDb.Update(nonExistentId, userName, email);
            CheckEquals(actual, Results.NotFound(UsersError.NoUserWithSuchId));
        }

        [Test]
        public void DeleteAndGetUser()
        {
            AddUser();

            var actual = usersDb.Delete(userId);
            CheckEquals(actual, Results.Ok());

            var expected = Results.NotFound(UsersError.NoUserWithSuchId);
            CheckGet(userId, expected);

            actual = usersDb.Delete(userId);
            CheckEquals(actual, expected);
        }

        [Test]
        public void DeleteUserWithNonExistentId()
        {
            var expected = Results.NotFound(UsersError.NoUserWithSuchId);
            var actual = usersDb.Delete(userId);
            CheckEquals(actual, expected);

            AddUsers();

            actual = usersDb.Delete(nonExistentId);
            CheckEquals(actual, expected);
        }

        private void AddUser() =>
            usersDb.Add(userName, email);

        private void AddUsers()
        {
            for (int i = 0; i < 5; i++)
                usersDb.Add(userNameFun(i), userEmailFun(i));
        }

        private void CheckEquals(IResult actual, IResult expected) =>
            Assert.That(actual, Is.EqualTo(expected).UsingPropertiesComparer());

    }
}