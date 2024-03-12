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
        private const string userEmail = "spolinaa@test.ru";
        private const string invalidEmail = "wrong format email";
        private const int userId = 1;
        private const int nonExistentId = 156;
        private const int negativeId = -15;

        private Func<int, string> userNameFun = x => $"user{x}";
        private Func<int, string> userEmailFun = x => $"user{x}@mail.ru";

        private IResult userNotFound = Results.NotFound(UsersError.NoUserWithSuchId);

        [SetUp]
        public void Setup()
        {
            dbContext = new ApplicationContext(DbSource.TEST);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            usersDb = new UsersDb(dbContext);
        }

        [Test]
        public void AddAndGetUserTest()
        {
            var actual = AddUser();
            CheckEquals(actual, Results.Ok(userId));

            var user = new User { Id = userId, Name = userName, Email = userEmail };
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
                .Range(1, nonExistentId - 1)
                .Select(x => new User { Id = x, Name = userNameFun(x), Email = userEmailFun(x) })
                .ToList();

            var actualResults = AddUsers().ToList();
            Assert.That(actualResults.Count, Is.EqualTo(users.Count));

            for (int i = 0; i < actualResults.Count; i++)
                CheckEquals(actualResults[i], Results.Ok(users[i].Id));

            CheckGet(Results.Ok(users));
        }

        [Test]
        public void GetMultipleUsersInEmptyDbTest()
        {
            CheckGet(Results.Ok(new List<User>()));
        }

        [TestCase(userId, true)]
        [TestCase(nonExistentId)]
        [TestCase(negativeId)]
        public void GetUserWithNonExistentId(int id, bool isEmptyDb = false)
        {
            if (!isEmptyDb)
                AddUsers();
            CheckGet(id, userNotFound);
        }

        [TestCase(userName, userEmail)]
        [TestCase("new name", userEmail)]
        [TestCase(userName, "new_mail@test.ru")]
        [TestCase("userName123", "userName123@test.ru")]
        [TestCase("name123", null)]
        [TestCase(null, "email@test.ru")]
        [TestCase(null, null)]
        public void UpdateAndGetUser(string name, string email)
        {
            AddUser();

            var user = new User 
            { 
                Id = userId, 
                Email = email ?? userEmail, 
                Name = name ?? userName 
            };

            var actual = usersDb.Update(userId, name, email);
            CheckEquals(actual, Results.Ok(user));
            CheckGet(userId, Results.Ok(user));
        }

        [Test]
        public void UpdateUserEmailToInvalidEmail()
        {
            AddUser();

            var expected = Results.BadRequest(UsersError.InvalidEmailFormat);
            var actual = usersDb.Update(userId, userName, invalidEmail);
            CheckEquals(actual, expected);
        }

        [TestCase(userId, true)]
        [TestCase(nonExistentId)]
        [TestCase(negativeId)]
        public void UpdateUserWithNonExistentId(int id, bool isEmptyDb = false)
        {
            RunTest(() => usersDb.Update(id, userName, userEmail), isEmptyDb);
        }

        [Test]
        public void DeleteAndGetUser()
        {
            AddUser();

            var actual = usersDb.Delete(userId);
            CheckEquals(actual, Results.NoContent());
            CheckGet(userId, userNotFound);

            actual = usersDb.Delete(userId);
            CheckEquals(actual, userNotFound);
        }

        [TestCase(userId, true)]
        [TestCase(nonExistentId)]
        [TestCase(negativeId)]
        public void DeleteUserWithNonExistentId(int id, bool isEmptyDb = false)
        {
            RunTest(() => usersDb.Delete(id), isEmptyDb);
        }

        private void RunTest(Func<IResult> test, bool isEmptyDb = false)
        {
            if (!isEmptyDb)
                AddUsers();
            var actual = test();
            CheckEquals(actual, userNotFound);
        }

        private void CheckGet(IResult expected)
        {
            var actual = usersDb.Get();
            CheckEquals(actual, expected);
        }

        private void CheckGet(int userId, IResult expected)
        {
            var actual = usersDb.Get(userId);
            CheckEquals(actual, expected);
        }

        private IResult AddUser() =>
            usersDb.Add(userName, userEmail);

        private IEnumerable<IResult> AddUsers()
        {
            for (int i = 1; i < nonExistentId; i++)
                yield return usersDb.Add(userNameFun(i), userEmailFun(i));
        }

        private void CheckEquals(IResult actual, IResult expected) =>
            Assert.That(actual, Is.EqualTo(expected).UsingPropertiesComparer());
    }
}