namespace EvtapAz.Models
{
    public interface ILocalizableEntity
    {
        string Name { get; }
        string? NameRu { get; }
        string? NameEn { get; }
    }

    public static class LocalizableEntityExtensions
    {
        public static string GetCurrentName(this ILocalizableEntity entity)
        {
            if (entity == null) return string.Empty;
            var lang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (lang == "ru" && !string.IsNullOrEmpty(entity.NameRu)) return entity.NameRu;
            if (lang == "en" && !string.IsNullOrEmpty(entity.NameEn)) return entity.NameEn;
            return entity.Name;
        }
    }
}
