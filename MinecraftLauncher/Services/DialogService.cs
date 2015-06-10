using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinecraftLauncher.Services
{
    public class DialogService : IDialogService
    {
        public async Task<MessageDialogResult> ShowMessageDialog(string title, string message, MessageDialogStyle dialogStyle)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

            return await metroWindow.ShowMessageAsync(title, message, dialogStyle, new MetroDialogSettings() { AffirmativeButtonText = "Да", NegativeButtonText = "Нет" });
        }

        public async Task<LoginDialogData> ShowLoginDialog(string title, string message, bool animateShow = true, bool animateHide = true)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

            return await metroWindow.ShowLoginAsync("Вход", "Введите свои данные",
                new LoginDialogSettings()
                {
                    PasswordWatermark = "пароль",
                    UsernameWatermark = "логин",
                    AffirmativeButtonText = "войти",
                    AnimateHide = animateHide,
                    AnimateShow = animateShow
                });
        }

        public async Task<ProgressDialogController> ShowProgressDialog(string title, string message, bool animateShow = true, bool animateHide = true)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;
            return await metroWindow.ShowProgressAsync(title, message, false, new MetroDialogSettings() { AnimateShow = animateShow, AnimateHide = animateHide });
        }
    }
}
