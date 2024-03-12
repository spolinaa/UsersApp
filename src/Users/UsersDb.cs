using System.ComponentModel.DataAnnotations;
using Users.Models;

namespace UsersApp
{
    public class UsersDb
    {
        private ApplicationContext db;

        public UsersDb(ApplicationContext appContext) =>
            db = appContext;

        public IResult Add(string name, string email)
        {
            if (!IsValidEmail(email))
                return Results.BadRequest(UsersError.InvalidEmailFormat);

            db.Add(new User { Name = name, Email = email });
            db.SaveChanges();

            var userId = db.Users.Max(x => x.Id);
            return Results.Ok(userId);
        }

        public IResult Get(int id)
        {
            var user = GetUserById(id);
            return user != null
                ? Results.Ok(user)
                : Results.NotFound(UsersError.NoUserWithSuchId);
        }

        public IResult Get() => Results.Ok(db.Users.ToList());

        public IResult Update(int id, string? name, string? email)
        {
            if (email != null && !IsValidEmail(email))
                return Results.BadRequest(UsersError.InvalidEmailFormat);

            var user = GetUserById(id);
            if (user == null)
                return Results.NotFound(UsersError.NoUserWithSuchId);

            if (name == null && user == null)
                return Results.Ok(user);

            user.Name = name ?? user.Name;
            user.Email = email ?? user.Email;
            db.Update(user);
            db.SaveChanges();
            return Results.Ok(user);
        }

        public IResult Delete(int id)
        {
            var user = GetUserById(id);
            if (user == null)
                return Results.NotFound(UsersError.NoUserWithSuchId);

            db.Users.Remove(user);
            db.SaveChanges();
            return Results.NoContent();
        }

        private User? GetUserById(int id) => 
            db.Users
                .Where(x => x.Id == id)
                .FirstOrDefault();

        private bool IsValidEmail(string email) => 
            new EmailAddressAttribute().IsValid(email);
    }
}