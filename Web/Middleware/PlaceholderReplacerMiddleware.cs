namespace Web.Middleware;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Web;

public class PlaceholderReplacerMiddleware(RequestDelegate next)
{
    private readonly Dictionary<string, string[]> _bodyPropertyMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "publishers/update", new [] { "Publisher", "Id" } },
        { "platforms/update", new [] { "Platform", "Id" } },
        { "games/update", new [] { "Game", "Id" } }
    };

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.Value != null)
        {
            string path = context.Request.Path.Value.ToLower();

            if (path.Contains("/add"))
            {
                await next(context);
                return;
            }

            var placeholders = new[] { "{id}", "{genreId}", "{platformId}", "{companyName}" };
            List<string> segments = context.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            bool replaced = false;

            for (int i = 0; i < segments.Count; i++)
            {
                string? seg = segments[i];
                if (placeholders.Contains(seg))
                {
                    string? actualValue = null;

                    var referer = context.Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrWhiteSpace(referer))
                    {
                        var uri = new Uri(referer);
                        List<string> refSegments = uri.Segments
                            .Select(s => s.Trim('/'))
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToList();

                        if (refSegments.Any())
                        {
                            actualValue = refSegments.Last();
                            actualValue = HttpUtility.UrlDecode(actualValue);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(actualValue) &&
                        (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
                         context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase)))
                    {
                        context.Request.EnableBuffering();
                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                        {
                            var body = await reader.ReadToEndAsync();
                            context.Request.Body.Position = 0; 

                            if (!string.IsNullOrWhiteSpace(body))
                            {
                                using var doc = JsonDocument.Parse(body);
                                foreach (var mapping in _bodyPropertyMappings.Where(mapping => path.Contains(mapping.Key.ToLower())))
                                {
                                    actualValue = ExtractValueFromJson(doc.RootElement, mapping.Value);
                                    break;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(actualValue))
                    {
                        segments[i] = actualValue; 
                        replaced = true;
                    }
                }
            }
            if (replaced)
            {
                var newPath = "/" + string.Join("/", segments);
                context.Request.Path = new PathString(newPath);
            }
        }

        await next(context);
    }

    private string? ExtractValueFromJson(JsonElement element, string[] propertyPath)
    {
        JsonElement current = element;
        foreach (var prop in propertyPath)
        {
            if (current.TryGetProperty(prop, out var child))
            {
                current = child;
            }
            else
            {
                return null;
            }
        }

        return current.ValueKind == JsonValueKind.String ? current.GetString() : current.ToString();
    }
}
