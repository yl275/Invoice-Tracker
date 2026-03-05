using InvoiceSystem.Application.DTOs.ResumeSummary;
using InvoiceSystem.Application.Interfaces.Services;

namespace InvoiceSystem.Application.Services
{
    public class ResumeSummaryService : IResumeSummaryService
    {
        private static readonly Dictionary<string, string> TechCategories =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // Languages
                { "Python", "language" },
                { "JavaScript", "language" },
                { "TypeScript", "language" },
                { "C#", "language" },
                { "Java", "language" },
                { "Go", "language" },
                { "Ruby", "language" },
                { "Rust", "language" },
                { "Swift", "language" },
                { "Kotlin", "language" },
                { "PHP", "language" },
                { "Scala", "language" },
                // Frameworks / Libraries
                { "Django", "framework" },
                { "Flask", "framework" },
                { "FastAPI", "framework" },
                { "React", "framework" },
                { "Angular", "framework" },
                { "Vue", "framework" },
                { "ASP.NET Core", "framework" },
                { "Spring Boot", "framework" },
                { "Express", "framework" },
                { "Next.js", "framework" },
                { "Node.js", "framework" },
                { "Rails", "framework" },
                { "Laravel", "framework" },
                // Databases
                { "PostgreSQL", "database" },
                { "MySQL", "database" },
                { "MongoDB", "database" },
                { "SQLite", "database" },
                { "Redis", "database" },
                { "SQL Server", "database" },
                { "DynamoDB", "database" },
                { "Cassandra", "database" },
                { "Elasticsearch", "database" },
                // ORMs / Data Access
                { "Entity Framework Core", "orm" },
                { "SQLAlchemy", "orm" },
                { "Hibernate", "orm" },
                { "Prisma", "orm" },
                { "Sequelize", "orm" },
                // DevOps / Containerisation
                { "Docker", "devops" },
                { "Kubernetes", "devops" },
                { "Terraform", "devops" },
                { "Ansible", "devops" },
                { "Nginx", "devops" },
                // CI/CD
                { "GitHub Actions", "cicd" },
                { "Jenkins", "cicd" },
                { "GitLab CI", "cicd" },
                { "CircleCI", "cicd" },
                { "Travis CI", "cicd" },
                // Cloud / Hosting
                { "AWS S3", "cloud" },
                { "AWS Lambda", "cloud" },
                { "AWS EC2", "cloud" },
                { "AWS", "cloud" },
                { "Azure", "cloud" },
                { "GCP", "cloud" },
                { "Google Cloud", "cloud" },
                { "Render", "cloud" },
                { "Heroku", "cloud" },
                { "Vercel", "cloud" },
                { "Netlify", "cloud" },
                // Third-party APIs / Auth
                { "Stripe", "api" },
                { "Clerk", "api" },
                { "Twilio", "api" },
                { "SendGrid", "api" },
                { "Auth0", "api" },
                { "Firebase", "api" },
                { "OpenAI", "api" },
            };

        public TechSummaryResponseDto GenerateSummary(TechSummaryRequestDto request)
        {
            if (request.Technologies == null || request.Technologies.Count == 0)
                return new TechSummaryResponseDto();

            var categorized = Categorize(request.Technologies);
            var bullets = BuildBulletPoints(categorized, request.Technologies);

            return new TechSummaryResponseDto { BulletPoints = bullets };
        }

        private static Dictionary<string, List<string>> Categorize(List<string> technologies)
        {
            var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var tech in technologies)
            {
                var category = TechCategories.TryGetValue(tech.Trim(), out var cat) ? cat : "other";
                if (!result.ContainsKey(category))
                    result[category] = new List<string>();
                result[category].Add(tech.Trim());
            }

            return result;
        }

        private static List<string> BuildBulletPoints(
            Dictionary<string, List<string>> categorized,
            List<string> allTechs)
        {
            var bullets = new List<string>();

            // --- First bullet: language + framework + database + devops/cicd ---
            var primaryParts = new List<string>();

            var coreStack = new List<string>();
            if (categorized.TryGetValue("language", out var langs))
                coreStack.AddRange(langs.Take(2));
            if (categorized.TryGetValue("framework", out var frameworks))
                coreStack.AddRange(frameworks.Take(2));

            if (coreStack.Count > 0)
                primaryParts.Add($"using {JoinWithAnd(coreStack)}");

            if (categorized.TryGetValue("database", out var dbs) && dbs.Count > 0)
                primaryParts.Add($"integrated {JoinWithAnd(dbs.Take(2).ToList())} database");

            if (categorized.TryGetValue("orm", out var orms) && orms.Count > 0)
                primaryParts.Add($"leveraging {JoinWithAnd(orms.Take(1).ToList())}");

            var devopsTools = new List<string>();
            if (categorized.TryGetValue("devops", out var devops)) devopsTools.AddRange(devops);
            if (categorized.TryGetValue("cicd", out var cicd)) devopsTools.AddRange(cicd);
            if (devopsTools.Count > 0)
                primaryParts.Add($"automated deployment with {JoinWithAnd(devopsTools.Take(2).ToList())}");

            if (primaryParts.Count > 0)
            {
                var bullet = TrimToWordLimit($"Developed a web application {string.Join(", ", primaryParts)}.", 30);
                bullets.Add(bullet);
            }

            // --- Second bullet: cloud + API services ---
            var secondaryParts = new List<string>();

            if (categorized.TryGetValue("cloud", out var cloud) && cloud.Count > 0)
                secondaryParts.Add($"Deployed on {JoinWithAnd(cloud.Take(2).ToList())}");

            if (categorized.TryGetValue("api", out var apis) && apis.Count > 0)
            {
                var apiStr = JoinWithAnd(apis.Take(2).ToList());
                secondaryParts.Add(secondaryParts.Count > 0
                    ? $"integrated {apiStr}"
                    : $"Integrated {apiStr}");
            }

            if (secondaryParts.Count > 0)
            {
                var bullet = TrimToWordLimit(
                    $"{string.Join(", ", secondaryParts)}, enhancing app scalability and reliability.", 30);
                bullets.Add(bullet);
            }

            // --- Fallback: generic bullet when no categories matched ---
            if (bullets.Count == 0)
            {
                var techList = JoinWithAnd(allTechs.Take(5).ToList());
                bullets.Add(TrimToWordLimit($"Built and deployed applications utilizing {techList}.", 30));
            }

            return bullets;
        }

        private static string JoinWithAnd(List<string> items)
        {
            return items.Count switch
            {
                0 => string.Empty,
                1 => items[0],
                2 => $"{items[0]} and {items[1]}",
                _ => $"{string.Join(", ", items.Take(items.Count - 1))}, and {items[^1]}"
            };
        }

        private static string TrimToWordLimit(string sentence, int maxWords)
        {
            var words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length <= maxWords)
                return sentence;

            var trimmed = string.Join(" ", words.Take(maxWords));
            // Ensure sentence ends properly
            trimmed = trimmed.TrimEnd(',', ';') + ".";
            return trimmed;
        }
    }
}
