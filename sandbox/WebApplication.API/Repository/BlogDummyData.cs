using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.API.Models;

namespace WebApplication.API.Repository
{
    public class BlogDummyData
    {
        public IQueryable<Blog> Blogs { get => EnumerateBlogs().AsQueryable(); }


        private IEnumerable<Blog> EnumerateBlogs()
        {
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 3, IsPublished = true, Priority = 1, PublishDate = DateTime.Now.AddHours(-546), Title = "Hello!!" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 1, IsPublished = true, Priority = 1, PublishDate = DateTime.Now.AddHours(-500), Title = "Hello This is Sample" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 1, IsPublished = true, Priority = 7, PublishDate = DateTime.Now.AddHours(-124235), Title = "Auto Filterer" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 1, IsPublished = true, Priority = 1, PublishDate = DateTime.Now.AddHours(-3333), Title = "World!" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 2, IsPublished = true, Priority = 2, PublishDate = DateTime.Now.AddHours(-457), Title = "Hello World" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 2, IsPublished = true, Priority = 2, PublishDate = DateTime.Now.AddHours(-33), Title = "Do the best" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 2, IsPublished = true, Priority = 3, PublishDate = DateTime.Now.AddHours(-1), Title = "Trying new stuffs" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 2, IsPublished = true, Priority = 3, PublishDate = DateTime.Now.AddHours(-2344), Title = "What's new in .Net Core 3.0?" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 3, IsPublished = true, Priority = 4, PublishDate = DateTime.Now.AddHours(-3545), Title = "Mono Framework" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 3, IsPublished = true, Priority = 5, PublishDate = DateTime.Now.AddHours(-95), Title = "Blazor moved to offical AspNetCore Repository" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 3, IsPublished = true, Priority = 7, PublishDate = DateTime.Now.AddHours(-34), Title = "WebAssembly is Awesome!" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 3, IsPublished = true, Priority = 6, PublishDate = DateTime.Now.AddHours(-95), Title = "Client-Side Razor Pages wit Blazor" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 1, IsPublished = true, Priority = 5, PublishDate = DateTime.Now.AddHours(-123), Title = "Xamarin Forms 4.1 has just released!" };
            yield return new Blog { BlogId = Guid.NewGuid().ToString(), CategoryId = 1, IsPublished = true, Priority = 7, PublishDate = DateTime.Now.AddHours(-653), Title = "Material Design..." };
        }
    }
}
