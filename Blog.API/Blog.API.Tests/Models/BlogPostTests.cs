using Blog.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.API.Tests.Models
{
    public class BlogPostTests { 
        private List<ValidationResult> ValidateModel(BlogPost post)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(post, null, null);
            Validator.TryValidateObject(post, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void BlogPost_WithValidProperties_PassesValidation()
        {
            var post = new BlogPost
            {
                Id = 1,
                Title = "Valid Title",
                Content = "This is some valid content.",
                Author = "John Doe",
                PublishedDate = DateTime.Now
            };

            var validationResults = ValidateModel(post);
            Assert.Empty(validationResults); 
        }

        [Fact]
        public void BlogPost_WithMissingTitle_FailsValidation()
        {
            var post = new BlogPost
            {
                Id = 1,
                Title = null,
                Content = "This is some content.",
                Author = "John Doe",
                PublishedDate = DateTime.Now
            };

            var validationResults = ValidateModel(post);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("Title"));
        }

        [Fact]
        public void BlogPost_WithTitleExceedingMaxLength_FailsValidation()
        {
            var post = new BlogPost
            {
                Id = 1,
                Title = new string('A', 51), // Exceeds max length of 50
                Content = "This is some content.",
                Author = "John Doe",
                PublishedDate = DateTime.Now
            };

            var validationResults = ValidateModel(post);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Title"));
        }

        [Fact]
        public void BlogPost_WithInvalidAuthor_FailsValidation()
        {
            var post = new BlogPost
            {
                Id = 1,
                Title = "Valid Title",
                Content = "This is some content.",
                Author = "John123",
                PublishedDate = DateTime.Now
            };

            var validationResults = ValidateModel(post);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Author"));
            Assert.Contains(validationResults, v => v.ErrorMessage == "Author name can only contain letters and spaces.");
        }

        [Fact]
        public void BlogPost_WithMissingContent_FailsValidation()
        {
            var post = new BlogPost
            {
                Id = 1,
                Title = "Valid Title",
                Content = null,
                Author = "John Doe",
                PublishedDate = DateTime.Now
            };

            var validationResults = ValidateModel(post);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Content"));
        }
    }
}
