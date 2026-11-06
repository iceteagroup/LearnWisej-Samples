using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The redaction policy, applied at the sink so nobody has to remember it: whatever a log call
    /// passes, these never reach the console or the log file - passwords, secrets, tokens (bearer
    /// and JWT-shaped), cookies, authorization headers, connection-string credentials and e-mail
    /// addresses. Log by reference instead: a user id, a document id, an outcome.
    /// </summary>
    public static class LogRedactor
    {
        private static readonly string[] SensitiveNames =
        {
            "password", "pwd", "secret", "client_secret", "clientsecret", "token", "access_token", "refresh_token",
            "id_token", "authorization", "cookie", "set-cookie", "signingkey", "sig", "code_verifier", "connectionstring", "ticket"
        };

        private static readonly Regex KeyValue = new Regex(
            @"(?<key>\b(?:" + string.Join("|", Array.ConvertAll(SensitiveNames, Regex.Escape)) + @")\b\s*[=:]\s*)(?<value>""[^""]*""|'[^']*'|[^\s;,&]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex Bearer = new Regex(@"\bBearer\s+[A-Za-z0-9\-._~+/]+=*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex Jwt = new Regex(@"\beyJ[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]*", RegexOptions.Compiled);
        private static readonly Regex Email = new Regex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}", RegexOptions.Compiled);

        public static string Redact(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = Jwt.Replace(text, "[redacted-jwt]");
            text = Bearer.Replace(text, "Bearer [redacted]");
            text = KeyValue.Replace(text, m => m.Groups["key"].Value + "[redacted]");
            text = Email.Replace(text, "[redacted-email]");
            return text;
        }

        public static object RedactValue(string name, object value)
        {
            if (value == null)
                return null;
            foreach (var sensitive in SensitiveNames)
            {
                if (name.Replace("_", "").Equals(sensitive.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
                    return "[redacted]";
            }
            return value is string s ? Redact(s) : value;
        }
    }

    /// <summary>
    /// One logger provider for the console and a JSON-lines file in AccessOpsData/logs. Every line
    /// carries the active scopes - the CorrelationId and UserId a command opens - and goes through
    /// <see cref="LogRedactor"/> first.
    /// </summary>
    public sealed class RedactingLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly string _folder;
        private readonly object _gate = new object();
        private IExternalScopeProvider _scopes = new LoggerExternalScopeProvider();

        public RedactingLoggerProvider(string folder)
        {
            _folder = folder;
            Directory.CreateDirectory(folder);
        }

        public string CurrentFile => Path.Combine(_folder, "accessops-" + DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".jsonl");

        public ILogger CreateLogger(string categoryName) => new RedactingLogger(this, categoryName);

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopes = scopeProvider;

        public void Dispose() { }

        private void Write(string category, LogLevel level, string message, Exception exception)
        {
            var fields = new Dictionary<string, object>
            {
                ["ts"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture),
                ["level"] = level.ToString(),
                ["category"] = category
            };
            _scopes.ForEachScope((scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> pairs)
                {
                    foreach (var pair in pairs)
                        if (!pair.Key.StartsWith("{", StringComparison.Ordinal))
                            state[pair.Key] = LogRedactor.RedactValue(pair.Key, pair.Value);
                }
            }, fields);
            fields["message"] = LogRedactor.Redact(message);
            if (exception != null)
                fields["exception"] = LogRedactor.Redact(exception.GetType().Name + ": " + exception.Message);

            var json = JsonSerializer.Serialize(fields);
            var correlation = fields.TryGetValue("CorrelationId", out var c) ? " [" + c + "]" : "";
            lock (_gate)
            {
                File.AppendAllText(CurrentFile, json + Environment.NewLine, Encoding.UTF8);
                Console.WriteLine($"{level.ToString().ToLowerInvariant()}: {category}{correlation}{Environment.NewLine}      {fields["message"]}");
            }
        }

        private sealed class RedactingLogger : ILogger
        {
            private readonly RedactingLoggerProvider _provider;
            private readonly string _category;

            public RedactingLogger(RedactingLoggerProvider provider, string category)
            {
                _provider = provider;
                _category = category;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => _provider._scopes.Push(state);

            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                    return;

                // structured state: redact the named values before the message is rendered from them
                var message = formatter(state, exception);
                if (state is IEnumerable<KeyValuePair<string, object>> pairs)
                {
                    foreach (var pair in pairs)
                    {
                        if (pair.Key != "{OriginalFormat}" && pair.Value is string value && LogRedactor.RedactValue(pair.Key, value) is string redacted && redacted != value)
                            message = message.Replace(value, redacted);
                    }
                }
                _provider.Write(_category, logLevel, message, exception);
            }
        }
    }
}
