using AutoFilterer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Models;

public class User
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public bool IsActive { get; set; }

    public List<Book> Books { get; set; }

    public Preferences Preferences { get; set; }
}
