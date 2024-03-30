namespace GS_Shop_UserManagement.Infrastructure.SmartLimit;

public class SmartLimit
{
    private readonly Dictionary<int, List<int>> _userShopAccess;

    public SmartLimit(Dictionary<int, List<int>> userShopAccess)
    {
        _userShopAccess = userShopAccess ?? throw new ArgumentNullException(nameof(userShopAccess));
    }

    public List<int> GetAllowedShopIds(int userId)
    {
        return _userShopAccess.TryGetValue(userId, out var value) ? value : new List<int>();
    }
}