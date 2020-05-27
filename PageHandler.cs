using RetroGameHandler.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RetroGameHandler.Views
{
    public class PageHandler
    {
        public static event EventHandler<EventArgs> ThePageChanged;

        public static Pager Instance { get; set; }

        public static bool AddPage(IPage page)
        {
            if (Instance == null) Init();
            return Instance.Add(page);
        }

        public static IPage GetPageByName(string pageName)
        {
            return Instance.GetPage(pageName);
        }

        public static IPage GetPageByType<T>()
        {
            return Instance.GetPage<T>();
        }

        public static PageControllHandler GetSelectedControl()
        {
            return Instance.GetPageControl();
        }

        public static IPage Next()
        {
            return Instance.NextPage();
        }

        public static IPage Previous()
        {
            return Instance.PreviousPage();
        }

        public static IPage SelectedPage<T>()
        {
            if (Instance == null) Init();
            return Instance.SetPage<T>();
        }

        private static void Init()
        {
            Instance = new Pager();
            Instance.PageChanged += OnThePageChanged;
        }

        private static void OnThePageChanged(object sender, EventArgs e)
        {
            ThePageChanged?.Invoke(null, new EventArgs());
        }

        public class Pager : INotifyPropertyChanged
        {
            private IPage _selectedPage;

            private int index = 0;

            private IList<IPage> pages = new List<IPage>();

            public event EventHandler<EventArgs> PageChanged;

            public event PropertyChangedEventHandler PropertyChanged;

            public IPage Page
            {
                get => _selectedPage;
                set
                {
                    _selectedPage = value;
                    OnPageChanged();
                    OnPropertyChanged();
                }
            }

            public bool Add(IPage View)
            {
                if (pages.Contains(View)) return false;
                pages.Add(View);
                if (pages.Count == 1) Page = pages[0];
                return true;
            }

            public IPage GetPage<T>()
            {
                return pages.FirstOrDefault(p => p is T) ?? null;
            }

            public IPage GetPage(string name)
            {
                return pages.FirstOrDefault(p => (p.GetType()?.Name ?? "").ToLower() == name.ToLower()) ?? null;
            }

            public PageControllHandler GetPageControl()
            {
                return Page.PageControllHandler;
            }

            public IPage NextPage()
            {
                if (index + 1 >= pages.Count()) return Page;
                return Page = pages.ElementAt(index + 1);
            }

            public void OnPageChanged()
            {
                PageChanged?.Invoke(this, new EventArgs());
            }

            public void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public IPage PreviousPage()
            {
                if (index - 1 < 0) return Page;
                return Page = pages.ElementAt(index - 1);
            }

            public IPage SetPage<T>()
            {
                Page = pages.FirstOrDefault(p => p is T) ?? Page;
                return Page;
            }
        }
    }
}