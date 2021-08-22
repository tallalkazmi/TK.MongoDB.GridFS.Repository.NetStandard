using fm.Extensions.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using TK.MongoDB.GridFS.Classes;
using TK.MongoDB.GridFS.Data;
using TK.MongoDB.GridFS.Tests.Models;

namespace TK.MongoDB.GridFS.Tests
{
    [TestClass]
    public class ImageUnitTest : ServiceTestsBase
    {
        protected override void ConfigureConfiguration([NotNull] IConfigurationBuilder configuration)
        {
            configuration.AddJsonFile("appsettings.json").Build();
        }

        protected override void ConfigureServices([NotNull] IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddTransient(typeof(IFileRepository<Image>), typeof(FileRepository<Image>));
        }

        protected override void ConfigureLogging([NotNull] ILoggingBuilder builder)
        {
            base.ConfigureLogging(builder);

            // Add additional loggers or configuration
            builder.AddFilter(logLevel => true);
        }

        [TestMethod]
        public void Get()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            IEnumerable<Image> files = ImageRepository.Get(x => x.Filename.Contains("Omega") && x.UploadDateTime < DateTime.UtcNow.AddDays(-1));
            Assert.IsNotNull(files);
            Console.WriteLine($"Output:\n {(files.Count() > 0 ? string.Join(", ", files.Select(x => x.Filename)) : "No record found")}");
        }

        [TestMethod]
        public void GetById()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            try
            {
                Image file = ImageRepository.Get(new ObjectId("5e36b5a698d2c14fe8b0ecbe"));
                Assert.IsNotNull(file);
                Console.WriteLine($"Output:\n{file.Filename}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Output:\n{ex.Message}");
            }
        }

        [TestMethod]
        public void GetByFilename()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            IEnumerable<Image> files = ImageRepository.Get("Omega1.png");
            Assert.IsNotNull(files);

            Console.WriteLine($"Output:\n{(files.Count() > 0 ? string.Join(", ", files.Select(x => x.Filename)) : "No record found")}");
        }

        [TestMethod]
        public void Insert()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            byte[] fileContent = File.ReadAllBytes("../../Files/Omega.png");
            DateTime now = DateTime.UtcNow;
            Image img = new Image()
            {
                Filename = $"Omega-{now.Year}{now.Month:D2}{now.Day:D2}.png",
                Content = fileContent,
                IsDisplay = false
            };

            string Id = ImageRepository.Insert(img);
            Assert.AreNotEqual(string.Empty, Id);
            Assert.AreNotEqual(null, Id);

            Console.WriteLine($"Inserted document Id: {Id}");
        }

        [TestMethod]
        public void InsertLargeFile()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            byte[] fileContent = File.ReadAllBytes("../../Files/LargeFile.jpg");

            DateTime now = DateTime.UtcNow;
            Image img = new Image()
            {
                Filename = $"LargeFile-{now.Year}{now.Month:D2}{now.Day:D2}.jpg",
                Content = fileContent,
                IsDisplay = false
            };

            Assert.ThrowsException<FileSizeException>(() => { string id = ImageRepository.Insert(img); });
        }

        [TestMethod]
        public void InsertWithId()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            byte[] fileContent = File.ReadAllBytes("../../Files/Omega.png");
            DateTime now = DateTime.UtcNow;
            Image img = new Image()
            {
                Id = ObjectId.GenerateNewId(),
                Filename = $"Omega-{now.Year}{now.Month:D2}{now.Day:D2}.png",
                Content = fileContent,
                IsDisplay = false
            };

            string Id = ImageRepository.Insert(img);
            Assert.AreNotEqual(string.Empty, Id);
            Assert.AreNotEqual(null, Id);

            Console.WriteLine($"Inserted document Id: {Id}");
        }

        [TestMethod]
        public void Rename()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            try
            {
                ImageRepository.Rename(new ObjectId("5e37cdcf98d2c12ba0231fbb"), "Omega-new.png");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Output:\n{ex.Message}");
            }
        }

        [TestMethod]
        public void Delete()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Image> ImageRepository = this.GetRequiredService<IFileRepository<Image>>();
            try
            {
                ImageRepository.Delete(new ObjectId("5e36b5a698d2c14fe8b0ecbe"));
                Console.WriteLine($"Output:\nFile with Id '5e36b5a698d2c14fe8b0ecbe' deleted.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Output:\n{ex.Message}");
            }
        }
    }
}
