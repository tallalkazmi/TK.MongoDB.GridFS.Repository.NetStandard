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
using TK.MongoDB.GridFS.Data;
using TK.MongoDB.GridFS.Tests.Models;

namespace TK.MongoDB.GridFS.Tests
{
    [TestClass]
    public class DocumentUnitTest : ServiceTestsBase
    {
        protected override void ConfigureConfiguration([NotNull] IConfigurationBuilder configuration)
        {
            configuration.AddJsonFile("appsettings.json").Build();
        }

        protected override void ConfigureServices([NotNull] IServiceCollection services)
        {
            base.ConfigureServices(services);

            // Configure your services here
            services.AddTransient(typeof(IFileRepository<Document>), typeof(FileRepository<Document>));
        }

        protected override void ConfigureLogging([NotNull] ILoggingBuilder builder)
        {
            base.ConfigureLogging(builder);

            // Add additional loggers or configuration
            builder.AddFilter(logLevel => true);
        }

        [TestMethod]
        public void GetById()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Document> DocumentRepository = this.GetRequiredService<IFileRepository<Document>>();
            try
            {
                Document file = DocumentRepository.Get(new ObjectId("5e36b5e698d2c103d438e163"));
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
            IFileRepository<Document> DocumentRepository = this.GetRequiredService<IFileRepository<Document>>();
            IEnumerable<Document> files = DocumentRepository.Get("sample.pdf");
            Assert.IsNotNull(files);

            Console.WriteLine($"Output:\n{(files.Count() > 0 ? string.Join(", ", files.Select(x => x.Filename)) : "No record found")}");
        }

        [TestMethod]
        public void Insert()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Document> DocumentRepository = this.GetRequiredService<IFileRepository<Document>>();
            byte[] fileContent = File.ReadAllBytes("Files/sample.pdf");
            Document doc = new Document()
            {
                Filename = "sample.pdf",
                Content = fileContent,
                IsPrivate = true
            };

            string Id = DocumentRepository.Insert(doc);
            Assert.AreNotEqual(string.Empty, Id);
            Assert.AreNotEqual(null, Id);

            Console.WriteLine($"Inserted document Id: {Id}");
        }

        [TestMethod]
        public void Delete()
        {
            Settings.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            IFileRepository<Document> DocumentRepository = this.GetRequiredService<IFileRepository<Document>>();
            try
            {
                DocumentRepository.Delete(new ObjectId("5e36b9d698d2c124886edc67"));
                Console.WriteLine($"Output:\nFile with Id '5e36b9d698d2c124886edc67' deleted.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Output:\n{ex.Message}");
            }
        }
    }
}
