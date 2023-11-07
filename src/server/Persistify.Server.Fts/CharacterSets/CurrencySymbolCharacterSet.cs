using System.Collections.Generic;
using Persistify.Server.Fts.Abstractions;

namespace Persistify.Server.Fts.CharacterSets;

public class CurrencySymbolCharacterSet : ICharacterSet
{
    public string Code => "currency_symbol";
    public IEnumerable<char> Characters => "$€£¥₹₩₽₫฿₪₲₴₱₵₸₺¢₡₢₣₤₥₦₧₨₩₪₫€₭₮₯₰₱₲₳₴₵₸₹₺₼₽₾₿﹩￠￡￥￦";
}
