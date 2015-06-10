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
    public interface IDialogService
    {
        Task<MessageDialogResult> ShowMessageDialog(string title, string message, MessageDialogStyle dialogStyle);
        Task<LoginDialogData> ShowLoginDialog(string title, string message, bool animateShow = true, bool animateHide = true);
        Task<ProgressDialogController> ShowProgressDialog(string title, string message, bool animateShow = true, bool animateHide = true);
    }
}
