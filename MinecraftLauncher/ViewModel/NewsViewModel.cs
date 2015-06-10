using GalaSoft.MvvmLight;
using MinecraftLauncher.Model.Minecraft;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLauncher.ViewModel
{
    public class NewsViewModel : ViewModelBase
    {
        private ObservableCollection<MinecraftNews> _News;
        public ObservableCollection<MinecraftNews> News
        {
            get { return _News; }
            set
            {
                _News = value;
                RaisePropertyChanged("News");
            }
        }

        public NewsViewModel()
        {
            PopulateNews();
        }

        async void PopulateNews()
        {
            News = new ObservableCollection<MinecraftNews>();

            var news = await MinecraftNews.GetNews();
            if (news == null) return;

            foreach (var n in news)
            {
                News.Add(n);
                await Task.Delay(300);
            }
        }
    }
}
