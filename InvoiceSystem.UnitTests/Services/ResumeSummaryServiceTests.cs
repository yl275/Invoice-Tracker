using FluentAssertions;
using InvoiceSystem.Application.DTOs.ResumeSummary;
using InvoiceSystem.Application.Services;

namespace InvoiceSystem.UnitTests.Services
{
    public class ResumeSummaryServiceTests
    {
        private readonly ResumeSummaryService _service;

        public ResumeSummaryServiceTests()
        {
            _service = new ResumeSummaryService();
        }

        [Fact]
        public void GenerateSummary_ShouldReturnEmptyBullets_WhenTechnologiesIsEmpty()
        {
            var request = new TechSummaryRequestDto { Technologies = new List<string>() };

            var result = _service.GenerateSummary(request);

            result.Should().NotBeNull();
            result.BulletPoints.Should().BeEmpty();
        }

        [Fact]
        public void GenerateSummary_ShouldReturnEmptyBullets_WhenTechnologiesIsNull()
        {
            var request = new TechSummaryRequestDto { Technologies = null! };

            var result = _service.GenerateSummary(request);

            result.Should().NotBeNull();
            result.BulletPoints.Should().BeEmpty();
        }

        [Fact]
        public void GenerateSummary_ShouldReturnTwoBullets_ForFullStack()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string>
                {
                    "Python", "Django", "PostgreSQL", "Docker", "GitHub Actions", "AWS S3"
                }
            };

            var result = _service.GenerateSummary(request);

            result.Should().NotBeNull();
            result.BulletPoints.Should().HaveCount(2);
        }

        [Fact]
        public void GenerateSummary_FirstBullet_ShouldContainLanguageAndFramework()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "Python", "Django", "PostgreSQL" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints.Should().NotBeEmpty();
            result.BulletPoints[0].Should().Contain("Python");
            result.BulletPoints[0].Should().Contain("Django");
        }

        [Fact]
        public void GenerateSummary_FirstBullet_ShouldContainDatabase()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "C#", "ASP.NET Core", "PostgreSQL" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints[0].Should().Contain("PostgreSQL");
        }

        [Fact]
        public void GenerateSummary_FirstBullet_ShouldMentionDevOpsTools()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "TypeScript", "React", "Docker", "GitHub Actions" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints[0].Should().ContainAny("Docker", "GitHub Actions");
        }

        [Fact]
        public void GenerateSummary_SecondBullet_ShouldMentionCloudService()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "Python", "Django", "AWS S3" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints.Should().HaveCountGreaterThan(1);
            result.BulletPoints[1].Should().Contain("AWS S3");
        }

        [Fact]
        public void GenerateSummary_SecondBullet_ShouldMentionApiService()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "JavaScript", "Node.js", "Stripe" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints.Should().HaveCountGreaterThan(1);
            result.BulletPoints[1].Should().Contain("Stripe");
        }

        [Fact]
        public void GenerateSummary_EachBullet_ShouldBeUnder30Words()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string>
                {
                    "Python", "Django", "PostgreSQL", "Docker", "Kubernetes",
                    "GitHub Actions", "AWS S3", "AWS Lambda", "Stripe", "Redis"
                }
            };

            var result = _service.GenerateSummary(request);

            foreach (var bullet in result.BulletPoints)
            {
                var wordCount = bullet.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                wordCount.Should().BeLessThanOrEqualTo(30, $"bullet '{bullet}' exceeds 30 words");
            }
        }

        [Fact]
        public void GenerateSummary_ShouldReturnFallbackBullet_WhenNoCategoryMatches()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "UnknownTech1", "UnknownTech2" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints.Should().HaveCount(1);
            result.BulletPoints[0].Should().Contain("UnknownTech1");
        }

        [Fact]
        public void GenerateSummary_ShouldHandleSingleTechnology()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "Python" }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints.Should().NotBeEmpty();
            result.BulletPoints[0].Should().Contain("Python");
        }

        [Fact]
        public void GenerateSummary_ShouldTrimWhitespaceFromTechnologyNames()
        {
            var request = new TechSummaryRequestDto
            {
                Technologies = new List<string> { "  Python  ", "  Django  " }
            };

            var result = _service.GenerateSummary(request);

            result.BulletPoints.Should().NotBeEmpty();
            result.BulletPoints[0].Should().Contain("Python");
        }
    }
}
