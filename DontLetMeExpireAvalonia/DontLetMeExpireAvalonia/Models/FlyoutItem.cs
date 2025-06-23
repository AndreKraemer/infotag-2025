using System;
using System.Collections.Generic;

namespace DontLetMeExpireAvalonia.Models
{

    public record FlyoutItem(Type ModelType, string Icon, string Title, Dictionary<string, object> NavigationParameters);

}
