using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SSO_Authenticatoin_with_cookie.Data;
using SSO_Authenticatoin_with_cookie.Entity;
using SSO_Authenticatoin_with_cookie.Interfaces;

namespace SSO_Authenticatoin_with_cookie.Service
{
    public class DataProcessingService : IDataProcessingService
    {
        private readonly IDataProtectionProvider _protectionProvider;
        private DataProtectionDbContext _dataProtectionDbContext {  get; set; }

        public DataProcessingService(IDataProtectionProvider protectionProvider, DataProtectionDbContext dataProtectionDbContext)
        {
            _protectionProvider = protectionProvider;
            _dataProtectionDbContext = dataProtectionDbContext;
        }

        public async Task<string> ProtectData(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data), "Data cannot be null or empty");

            var protector = _protectionProvider.CreateProtector("SSO_with_cookie");

            var protectedData = protector.Protect(data);

            var DataKeys = new DataProtectionKeys()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Key = data,
                Value = protectedData
            };

            await _dataProtectionDbContext.AddAsync(DataKeys);
            await _dataProtectionDbContext.SaveChangesAsync();

            return protectedData;
        }

        public async Task<string> UnprotectData(string protectedData)
        {
            if (string.IsNullOrEmpty(protectedData))
                throw new ArgumentNullException(nameof(protectedData), "Protected data cannot be null or empty");

            var DataProtection = await _dataProtectionDbContext.DataProtectionKeys.Where(D => D.Value == protectedData).FirstOrDefaultAsync();

            if (DataProtection != null)
                return DataProtection.Key;

            throw new KeyNotFoundException();
        }
    }
}
