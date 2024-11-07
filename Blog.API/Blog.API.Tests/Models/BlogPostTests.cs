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
                Title = "A Beginner’s Guide to Gardening",
                Content = "A beginner’s guide to growing plants and creating a flourishing garden.",
                Author = "Marcelo",
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
                Content = "A beginner’s guide to growing plants and creating a flourishing garden.",
                Author = "Lamine Yamal",
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
                Title = new string('A', 55),
                Content = "A beginner’s guide to growing plants and creating a flourishing garden.",
                Author = "Jadon Sanchez",
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
                Title = "A Beginner’s Guide to Gardening",
                Content = "A beginner’s guide to growing plants and creating a flourishing garden.",
                Author = "Elliot123",
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
                Title = "A Beginner’s Guide to Gardening",
                Content = null,
                Author = "Roy Keane",
                PublishedDate = DateTime.Now
            };

            var validationResults = ValidateModel(post);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Content"));
        }
    }
}
