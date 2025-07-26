using Microsoft.EntityFrameworkCore;
using ToDoListAPI;

namespace ToDoListAPI
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }

        public DbSet<ToDo> ToDos { get; set; }
    }
}
