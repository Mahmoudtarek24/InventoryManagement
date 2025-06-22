namespace EducationPlatform.Constants
{
	public static class RegexPatterns
	{
		public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{6,}";
		public const string Username = "^[a-zA-Z0-9-._@]*$";
		public const string Email = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
	}
}
