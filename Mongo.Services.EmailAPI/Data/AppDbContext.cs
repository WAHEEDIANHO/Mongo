﻿using Microsoft.EntityFrameworkCore;
using Mongo.Services.EmailAPI.Models;

namespace Mongo.Services.EmailAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<EmailLogger> EmailLoggers {  get; set; }
    }
}
