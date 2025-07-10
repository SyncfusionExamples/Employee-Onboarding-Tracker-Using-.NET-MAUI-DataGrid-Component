using Microsoft.Maui.Platform;
#if ANDROID
using Android.Content.Res;
#endif

namespace DataGridMAUI
{
    public class SfEntry : Entry
    {
        public SfEntry()
        {
#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                if (v is Entry)
                {
                    h.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
                }
            });
#endif
#if WINDOWS
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                if (v is Entry)
                {
                    h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                    h.PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 0);
                    h.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
                }
            });
#endif
#if MACCATALYST || IOS
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                if (v is Entry)
                {
                    h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                }
            });
#endif
        }
    }
}
