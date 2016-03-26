namespace Langwitch
{
    public interface ILanguageService
    {
        bool SwitchLanguage(bool restoreLanguageLayout);
        bool SwitchLayout(bool doWrap);
        bool SwitchLanguageAndLayout();
    }
}
