using Blazored.SessionStorage;

namespace VisionSoft.Services
{
    public class SessionService
    {
        private readonly ISessionStorageService _sessionStorage;
        private const string USER_EMAIL_KEY = "userEmail";
        private const string IS_LOGGED_IN_KEY = "isLoggedIn";

        public SessionService(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task SetUserSessionAsync(string userEmail)
        {
            try
            {
                await _sessionStorage.SetItemAsync(USER_EMAIL_KEY, userEmail);
                await _sessionStorage.SetItemAsync(IS_LOGGED_IN_KEY, true);
            }
            catch (InvalidOperationException)
            {
                // JavaScript interop not available during prerendering
                // This is handled in OnAfterRenderAsync
            }
        }

        public async Task<string> GetUserEmailAsync()
        {
            try
            {
                return await _sessionStorage.GetItemAsync<string>(USER_EMAIL_KEY) ?? string.Empty;
            }
            catch (InvalidOperationException)
            {
                // JavaScript interop not available during prerendering
                return string.Empty;
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            try
            {
                return await _sessionStorage.GetItemAsync<bool>(IS_LOGGED_IN_KEY);
            }
            catch (InvalidOperationException)
            {
                // JavaScript interop not available during prerendering
                return false;
            }
        }

        public async Task ClearSessionAsync()
        {
            try
            {
                await _sessionStorage.RemoveItemAsync(USER_EMAIL_KEY);
                await _sessionStorage.RemoveItemAsync(IS_LOGGED_IN_KEY);
            }
            catch (InvalidOperationException)
            {
                // JavaScript interop not available during prerendering
                // This is handled in OnAfterRenderAsync
            }
        }

        public async Task<bool> HasValidSessionAsync()
        {
            try
            {
                var isLoggedIn = await IsLoggedInAsync();
                var userEmail = await GetUserEmailAsync();
                return isLoggedIn && !string.IsNullOrEmpty(userEmail);
            }
            catch (InvalidOperationException)
            {
                // JavaScript interop not available during prerendering
                return false;
            }
        }
    }
}