namespace Homework2.Tests
{

    public class TestCredentials
    {
        public string Email { get; }
        public string Password { get; }

        public TestCredentials()
        {
            Email = GetEnvVarOrThrow("TEST_USERNAME");
            Password = GetEnvVarOrThrow("TEST_PASSWORD");
        }

        private static string GetEnvVarOrThrow(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException($"Environment variable '{name}' is not set or empty.");
            return value;
        }

        // Optional: factory method to allow default values
        public static TestCredentials FromEnvironmentOrDefaults(string? defaultUsername = null, string? defaultPassword = null)
        {
            var email = Environment.GetEnvironmentVariable("TEST_USERNAME") ?? defaultUsername;
            var password = Environment.GetEnvironmentVariable("TEST_PASSWORD") ?? defaultPassword;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Missing credentials and no defaults provided.");

            return new TestCredentials(email, password);
        }

        private TestCredentials(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}